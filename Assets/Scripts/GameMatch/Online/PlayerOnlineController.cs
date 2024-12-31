using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerOnlineController : NetworkBehaviour
{
    public PlayerOnlineModel currentPlayer;
    FunctOnlineController functionController;

    GameManager gameManager;

    public Transform areaHand;      // The GO Canvas in which Card prefabs will be generate into
    public GameObject cardPrefab;   // The Card prefab to view cards

    public TextMeshProUGUI txtHP;


    #region MonoBehaviour
    public override void OnNetworkSpawn()
    {
        areaHand = transform.Find("HandCards").transform;
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");

        Transform HPs = transform.Find("HPs");
        txtHP = HPs.Find("TxtHP").GetComponent<TextMeshProUGUI>();

        currentPlayer = GetComponent<PlayerOnlineModel>();
        functionController = GameObject.Find("GameManager").GetComponent<FunctOnlineController>();
    }

    #endregion


    #region Gameplay

    public void GetCardLimit()
    {
        currentPlayer.limitKeep = currentPlayer.HP.Value;
    }

    #endregion


    #region UI

    public void UpdatePlayerStates()
    {
        if (currentPlayer != null)
        {
            if (currentPlayer.cardsInHand != null)
            {
                // Constantly updating the number of cards in hand
                currentPlayer.cardsCount = currentPlayer.cardsInHand.Count;
            }

            // Consstantly update player's hand card limit
            GetCardLimit();

            txtHP.text = currentPlayer.HP.ToString();
        }
    }

    public void viewHandCards()
    {
        foreach (Transform child in areaHand)
        {
            Destroy(child.gameObject);
        }

        // Repeate for each Deck in cardsInHand
        for (int i = 0; i < currentPlayer.cardsInHand.Count; i++)
        {
            getCardView(currentPlayer.cardsInHand[i], i);
        }
    }

    public void getCardView(CardsOnline card, int index)
    {
        GameObject cardView = Instantiate(cardPrefab, areaHand);
        cardView.name = "Card" + index;

        CardOnlineController cardController = cardView.GetComponent<CardOnlineController>();
        cardController.currentCard = card;
    }

    #endregion


    #region Update stats

    public void UpdateHP(int newHP)
    {
        txtHP.text = newHP.ToString();
    }

    #endregion
}
