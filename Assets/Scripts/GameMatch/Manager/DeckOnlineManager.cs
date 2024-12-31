using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class DeckOnlineManager : NetworkBehaviour
{
    public List<CardsOnline> drawDecks = new List<CardsOnline>();
    public List<CardsOnline> discardDecks = new List<CardsOnline>();
    public int countDraw;
    public int countDiscard;

    #region NetworkBehaviour

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

    #endregion


    #region Draw and Discard

    public CardsOnline DrawCardFromDeck()
    {
        if (drawDecks.Count == 0)
        {
            RefillDeck(); 
        }
        if (drawDecks.Count > 0)
        {
            CardsOnline drawnCard = drawDecks[0];
            drawDecks.RemoveAt(0);
            return drawnCard;
        }
        else
        {
            Debug.LogError("Deck is empty, cannot draw a card!");
            return null;
        }
    }

    public void DiscardCard(CardsOnline card)
    {
        if (card != null)
        {
            discardDecks.Add(card);
        }
        else
        {
            Debug.LogError("Cannot discard a null card!");
        }
    }

    #endregion


    #region Deck Generator
    void LoadRandomDecks()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("DeckData");

        if (jsonFile != null)
        {
            string json = jsonFile.text;
            DeckData deckData = JsonUtility.FromJson<DeckData>(json);

            // A temporary deck to save all loaded cards
            List<CardsOnline> allDecks = new List<CardsOnline>();

            if (deckData != null && deckData.cards.Count > 0)
            {
                foreach (CardData cardData in deckData.cards)
                {
                    CardsOnline card = new CardsOnline(
                        cardData.Name,
                        cardData.Description,
                        cardData.Number,
                        cardData.Element,
                        cardData.Color,
                        cardData.Damage,
                        cardData.Targets);

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
