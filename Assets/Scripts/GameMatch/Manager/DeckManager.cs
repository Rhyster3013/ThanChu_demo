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
        TextAsset jsonFile = Resources.Load<TextAsset>("DeckData");

        if (jsonFile != null)
        {
            string json = jsonFile.text;
            DeckData deckData = JsonUtility.FromJson<DeckData>(json);

            // A temporary deck to save all loaded cards
            List<Cards> allDecks = new List<Cards>();

            if (deckData != null && deckData.cards.Count > 0)
            {
                foreach (CardData cardData in deckData.cards)
                {
                    Cards card = ScriptableObject.CreateInstance<Cards>();

                    card.Id = cardData.Id;
                    card.Name = cardData.Name;
                    card.Description = cardData.Description;
                    card.Number = cardData.Number;
                    card.Element = cardData.Element;
                    card.Color = cardData.Color;
                    card.Damage = cardData.Damage;
                    card.Targets = cardData.Targets;

                    allDecks.Add(card);
                }
            }

            // Shuffle the allDecks randomly and add them to the Deck
            while (drawDecks.Count < 30 && allDecks.Count > 0)
            {
                int randomIndex = Random.Range(0, allDecks.Count);
                drawDecks.Add(allDecks[randomIndex]);
                allDecks.RemoveAt(randomIndex); // Remove the added cards to prevent repetition
            }

            Debug.Log("Loaded decks from Resources.");
        }
        else
        {
            Debug.LogError("DeckData.json not found in Resources.");
        }
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
