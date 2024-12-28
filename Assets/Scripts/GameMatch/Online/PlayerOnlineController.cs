using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerOnlineController : NetworkBehaviour, IPointerClickHandler
{
    public PlayerModel currentPlayer;
    FunctionController functionController;

    GameManager gameManager;

    public Transform areaHand;      // The GO Canvas in which Card prefabs will be generate into
    public GameObject cardPrefab;   // The Card prefab to view cards

    public Outline outline;
    public GameObject overlayImage; // Image overlay
    public TextMeshProUGUI txtHP;


    #region MonoBehaviour
    public override void OnNetworkSpawn()
    {
        if (gameObject.name == "Player")
        {
            areaHand = transform.Find("HandCards").transform;
            cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        }

        overlayImage = transform.Find("Active").gameObject;

        Transform HPs = transform.Find("HPs");
        txtHP = HPs.Find("TxtHP").GetComponent<TextMeshProUGUI>();
        outline = GetComponent<Outline>();

        currentPlayer = GetComponent<PlayerModel>();
        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();

        outline.enabled = false;
    }

    private void Update()
    {
        UpdatePlayerStates();
    }


    #endregion


    #region Gameplay

    public void PlayerDraw(int amount)
    {
        if (currentPlayer.cardsInHand != null)
        {
            functionController.DrawFromDeck(currentPlayer, amount);
        }

        if (gameObject.name == "Player")
        {
            viewHandCards();
        }
    }

    public void GetCardLimit()
    {
        currentPlayer.limitKeep = currentPlayer.HP;
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

            if (currentPlayer.isPickedAsTarget)
                outline.enabled = true;
            else
                outline.enabled = false;

            // Consstantly update player's hand card limit
            GetCardLimit();

            ActivePlayer();

            txtHP.text = currentPlayer.HP.ToString();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentPlayer.isPickable)
        {
            functionController.PlayerUpdate(currentPlayer);
        }
        else
        {
            Debug.Log("Target not pickable");
        }
    }

    // Enable or Disable a card based on the isActive attribute of the card
    private void ActivePlayer()
    {
        if (currentPlayer.isPickable == false)
        {
            overlayImage.SetActive(true);
        }
        if (currentPlayer.isPickable || currentPlayer.isPickedAsTarget)
        {
            overlayImage.SetActive(false);
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
            currentPlayer.cardsInHand[i].isInHand = currentPlayer;
        }
        //functionController.SetInteractability(currentPlayer);
    }

    public void getCardView(Cards card, int index)
    {
        GameObject cardView = Instantiate(cardPrefab, areaHand);
        cardView.name = "Card" + index;

        CardController cardController = cardView.GetComponent<CardController>();
        cardController.currentCard = card;
    }

    #endregion



}
