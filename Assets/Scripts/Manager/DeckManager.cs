using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Deck> drawDecks = new List<Deck>();
    public List<Deck> discardDecks = new List<Deck>();
    public int countDraw;
    public int countDiscard;

    // Start is called before the first frame update
    void Start()
    {
        // Call the method for randomize deck
        LoadRandomDecks();
    }

    // Update is called once per frame
    void Update()
    {
        countDraw = drawDecks.Count;
        countDiscard = discardDecks.Count;
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
        while (drawDecks.Count < 25 && allDecks.Count > 0)
        {
            int randomIndex = Random.Range(0, allDecks.Count);
            drawDecks.Add(allDecks[randomIndex]);
            allDecks.RemoveAt(randomIndex); // Remove the added cards from the allDeck to prevent repeat
        }

        // Log the result
        foreach (Deck deck in drawDecks)
        {
            Debug.Log("Added Deck: " + deck.Name);
        }
    }


    #endregion
}
