using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    //public Deck currentCard { get; set; }
    public Deck currentCard;

    public GameObject overlayImage; // Image overlay
    private Outline outline;        // Card Outline

    private void Start()
    {
        overlayImage = transform.Find("Active").gameObject;
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void Update()
    {
        ViewCard();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentCard != null)
        {
            if (currentCard.isUsable)
            {
                Debug.Log(gameObject.name + " pressed!");

                if (currentCard.isPickCard == false)
                {
                    outline.enabled = true;
                    currentCard.isPickCard = true;
                }
                else
                {
                    outline.enabled = false;
                    currentCard.isPickCard = false;
                }
            }
            else
            {
                Debug.Log(gameObject.name + " is not interactable!");
                return;
            }
        }
    }

    #region UI Elements
    // Display every UI elements based on the Deck currentCard
    private void ViewCard()
    {
        if (currentCard != null && currentCard.isInHand)
        {
            setElement(gameObject, currentCard.Element, currentCard.Number.ToString());
            setName(gameObject, currentCard.Name);
            SetCardOverlay();
            ActiveCard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Enable or Disable a card based on the isActive attribute of the card
    private void ActiveCard()
    {
        if (currentCard.isActive && currentCard.isUsable)
        {
            overlayImage.SetActive(false);
        }
        else
        {
            overlayImage.SetActive(true);
            return;
        }
    }

    // Set the card's usability
    private void SetCardOverlay()
    {
        Transform layer = gameObject.transform.Find("Active");

        RectTransform overlayRect = overlayImage.GetComponent<RectTransform>();
        overlayRect.anchorMin = new Vector2(0, 0); // Anchors the bottom left cornet
        overlayRect.anchorMax = new Vector2(1, 1); // Anchors the top right cornet
        overlayRect.offsetMin = Vector2.zero; // Set Left, Bottom to 0
        overlayRect.offsetMax = Vector2.zero; // Set Right, Top to 0
    }

    // Set UI elements to the Card
    public void setElement(GameObject card, string element, string number)
    {
        Transform image = card.transform.Find("Element");
        Transform point = card.transform.Find("Element/Point");

        Image cardImage = image.GetComponent<Image>();
        TextMeshProUGUI cardPoint = point.GetComponent<TextMeshProUGUI>();

        cardPoint.text = number;

        switch (element)
        {
            case ("Fire"):
                cardImage.color = Color.red;
                cardPoint.color = Color.black;
                break;
            case ("Wind"):
                cardImage.color = Color.green;
                cardPoint.color = Color.black;
                break;
            case ("Water"):
                cardImage.color = Color.blue;
                cardPoint.color = Color.white;
                break;
            case ("Earth"):
                cardImage.color = Color.grey;
                cardPoint.color = Color.white;
                break;
        }
    }

    public void setName(GameObject card, string nameInput)
    {
        Transform name = card.transform.Find("Name");
        TextMeshProUGUI cardName = name.GetComponent<TextMeshProUGUI>();

        cardName.text = nameInput;
    }
    #endregion
}
