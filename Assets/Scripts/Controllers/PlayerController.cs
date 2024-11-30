using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class PlayerController : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] Player currentPlayer;
    public Player currentPlayer;
    FunctionController functionController;

    GameManager gameManager;

    [SerializeField] bool needCard = false;
    //[SerializeField] GameObject playerGO;

    public Transform areaHand;      // The GO Canvas in which Card prefabs will be generate into
    public GameObject cardPrefab;   // The Card prefab to view cards

    public Outline outline;
    public GameObject overlayImage; // Image overlay
    public TextMeshProUGUI txtHP;


    #region MonoBehaviour
    private void Start()
    {
        areaHand = transform.Find("HandCards").transform;
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");

        overlayImage = transform.Find("Active").gameObject;

        txtHP = transform.Find("HP").gameObject.GetComponent<TextMeshProUGUI>();
        outline = GetComponent<Outline>();

        currentPlayer = GetComponent<Player>();
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
        functionController.DrawFromDeck(currentPlayer, amount);
        viewHandCards();
    }

    public void GetCardLimit()
    {
        currentPlayer.limitHand = currentPlayer.HP;
    }

    #endregion


    #region UI

    public void UpdatePlayerStates()
    {
        if (currentPlayer != null)
        {
            if (currentPlayer.handCard != null)
            {
                // Constantly updating the number of cards in hand
                currentPlayer.numberOfCard = currentPlayer.handCard.Count;
            }

            if (currentPlayer.isPickedTarget)
                outline.enabled = true;
            else
                outline.enabled = false;

            // Constantly showing the cards in hand
            needCard = currentPlayer.isNeedCard;

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
        if (currentPlayer.isPickable || currentPlayer.isPickedTarget)
        {
            overlayImage.SetActive(false);
        }
    }

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
            currentPlayer.handCard[i].isInHand = currentPlayer;
        }
        //functionController.SetInteractability(currentPlayer);
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
