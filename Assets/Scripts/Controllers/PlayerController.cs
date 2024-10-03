using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] Player currentPlayer;
    public Player currentPlayer;
    FunctionController functionController = new FunctionController();
    TimingController timingController = new TimingController();

    GameManager gameManager;

    [SerializeField] bool needCard = false;
    //[SerializeField] GameObject playerGO;

    public Transform areaHand;      // The GO Canvas in which Card prefabs will be generate into
    public GameObject cardPrefab;   // The Card prefab to view cards


    #region MonoBehaviour
    private void Start()
    {
        areaHand = transform.Find("HandCards").transform;
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        currentPlayer = GetComponent<Player>();
    }

    private void Update()
    {
        // Constantly updating the number of cards in hand
        currentPlayer.numberOfCard = currentPlayer.handCard.Count;

        // Constantly showing the cards in hand
        needCard = currentPlayer.isNeedCard;

        // Consstantly update player's hand card limit
        GetCardLimit();
    }


    #endregion


    #region Gameplay

    public void PlayerDraw(int amount)
    {
        functionController.DrawFromDeck(currentPlayer, amount);
        viewHandCards();
    }

    public void GetCardLimit()
    {
        currentPlayer.CardLimit = currentPlayer.HP;
    }

    public void GetPickedCard(int limit)
    {
        if (currentPlayer.handCard != null)
        {
            foreach (Deck card in currentPlayer.handCard)
            {
                if (card.isPickCard 
                    && functionController.isNotPicked(currentPlayer.AfterPickCard, card))
                {
                    currentPlayer.AfterPickCard.Add(card);
                }
                else if (card.isPickCard == false)
                {
                    currentPlayer.AfterPickCard.Remove(card);
                }
            }
        }
    }

    public void SetInteractability(int limit)
    {
        if (currentPlayer.AfterPickCard != null)
        {
            if (currentPlayer.AfterPickCard.Count == limit)
            {
                foreach (Deck card in currentPlayer.handCard)
                {
                    if (card.isPickCard == false)
                    {
                        card.isActive = false;
                    }
                }
            }
            else 
            {
                foreach (Deck deck in currentPlayer.handCard)
                {
                    deck.isActive = true;
                }
            }

            for (int i = currentPlayer.AfterPickCard.Count - 1; i >= 0; i--) 
            {
                if (!currentPlayer.AfterPickCard[i].isPickCard)
                {
                    currentPlayer.AfterPickCard.RemoveAt(i);

                }
            }
        }
    }

    #endregion


    #region UI

    public void viewHandCards()
    {
        foreach(Transform child in areaHand)
        {
            Destroy(child.gameObject);
        }
        // Repeate for each Deck in handCard
        for (int i = 0; i < currentPlayer.handCard.Count; i++)
        {
            getCardView(currentPlayer.handCard[i], i);
            currentPlayer.handCard[i].isInHand = true;
        }
    }

    public void getCardView(Deck card, int index)
    {
        GameObject cardView = Instantiate(cardPrefab, areaHand);
        cardView.name = "Card" + index;

        CardController cardController = cardView.GetComponent<CardController>();
        cardController.currentCard = card;
    }

    #endregion
}
