using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> drawDecks = new List<Cards>();
    public List<Cards> discardDecks = new List<Cards>();
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
        // Chỉ chạy đoạn mã này trong Editor
#if UNITY_EDITOR
        // Get every asset files in Assets/Data
        string[] deckPaths = AssetDatabase.FindAssets("t:Cards", new[] { "Assets/Data" });
        Debug.Log("Begin load deck" + deckPaths);

        // A temporary deck to save all loaded cards
        List<Cards> allDecks = new List<Cards>();

        // Load every card from the deckPath
        foreach (string deckGUID in deckPaths)
        {
            string deckPath = AssetDatabase.GUIDToAssetPath(deckGUID);
            Cards deck = AssetDatabase.LoadAssetAtPath<Cards>(deckPath);

            if (deck != null)
            {
                allDecks.Add(deck);
            }
        }

        // Shuffle the allDecks randomly and add them to the Deck
        while (drawDecks.Count < 30 && allDecks.Count > 0)
        {
            int randomIndex = Random.Range(0, allDecks.Count);
            drawDecks.Add(allDecks[randomIndex]);
            allDecks.RemoveAt(randomIndex); // Remove the added cards to prevent repetition
        }
#else
            Debug.LogError("LoadRandomDecks is only available in the Unity Editor.");
#endif
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
