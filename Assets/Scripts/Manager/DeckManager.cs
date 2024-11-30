using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Deck> drawDecks = new();
    public List<Deck> discardDecks = new();
    public int countDraw;
    public int countDiscard;

    FunctionController functionController;

    // Start is called before the first frame update
    void Start()
    {
        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();

        // Call the method for randomize deck
        LoadRandomDecks();
        ResetDeck();
    }

    // Update is called once per frame
    void Update()
    {
        countDraw = drawDecks.Count;
        countDiscard = discardDecks.Count;
    }

    void ResetDeck()
    {
        functionController.CardClear(drawDecks, 0);
    }

    #region Deck Generator
    void LoadRandomDecks()
    {
        // Get every asset files in Assets/Data
        string[] deckPaths = AssetDatabase.FindAssets("t:Deck", new[] { "Assets/Data" });

        // A temporary deck to save all loaded cards
        List<Deck> allDecks = new List<Deck>();

        // Load every cards from the deckPath
        foreach (string deckGUID in deckPaths)
        {
            string deckPath = AssetDatabase.GUIDToAssetPath(deckGUID);
            Deck deck = AssetDatabase.LoadAssetAtPath<Deck>(deckPath);

            if (deck != null)
            {
                allDecks.Add(deck);
            }
        }

        // Shuffle the AllDeck randomly và add them to the Deck
        while (drawDecks.Count < 30 && allDecks.Count > 0)
        {
            int randomIndex = Random.Range(0, allDecks.Count);
            drawDecks.Add(allDecks[randomIndex]);
            allDecks.RemoveAt(randomIndex); // Remove the added cards from the allDeck to prevent repeat
        }

        // Log the result
        //foreach (Deck deck in drawDecks)
        //{
        //    Debug.Log("Added Deck: " + deck.Name);
        //}
    }

    public void RefillDeck()
    {
        int discard = discardDecks.Count;
        while (drawDecks.Count < discard && discardDecks.Count > 0)
        {
            int randomIndex = Random.Range(0, discardDecks.Count);
            drawDecks.Add(discardDecks[randomIndex]);
            discardDecks.RemoveAt(randomIndex); // Remove the added cards from the allDeck to prevent repeat
        }
    }

    #endregion
}
