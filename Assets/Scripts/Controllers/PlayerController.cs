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
        getCardLimit();
    }


    #endregion


    #region Gameplay

    public void PlayerDraw(int amount)
    {
        functionController.DrawFromDeck(currentPlayer, amount);
        viewHandCards();
    }

    public void ActiveCard(bool inTurn)
    {
        foreach(Deck card in currentPlayer.handCard)
        {
            functionController.CardActivate(card, inTurn);
        }
    }

    public void getCardLimit()
    {
        currentPlayer.CardLimit = currentPlayer.HP;
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
