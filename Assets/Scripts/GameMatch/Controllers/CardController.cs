
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    //public Deck currentCard { get; set; }
    public Cards currentCard;

    public GameObject overlayImage; // Image overlay
    private Outline outline;        // Card Outline

    FunctionController function;

    private void Start()
    {
        overlayImage = transform.Find("Active").gameObject;
        outline = GetComponent<Outline>();
        outline.enabled = false;

        function = GameObject.Find("GameManager").GetComponent<FunctionController>();
    }

    private void Update()
    {
        ViewCard();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentCard != null)
        {
            if (currentCard.isUsable && currentCard.isActive)
            {
                Debug.Log(currentCard.isInHand + " has pressed " + gameObject.name);

                function.CardUpdate(currentCard);
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
        if (currentCard != null && currentCard.isInHand != null && !currentCard.isProcessing)
        {
            setElement(gameObject, currentCard.Element, currentCard.Number.ToString());
            setName(gameObject, currentCard.Name);
            SetImage(gameObject, currentCard.Name);

            SetCardOverlay();
            ActiveCard();

            if (currentCard.isPickCard)
                outline.enabled = true;
            else
                outline.enabled = false;
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

        Sprite sprite = Resources.Load<Sprite>("Assets/Images/CardImages/Elements/" + element + ".png");

        if (sprite != null)
        {
            cardImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Sprite '{name}' not found in Resources!");
        }

        cardPoint.text = number;

        switch (element)
        {
            case ("Fire"):
                cardPoint.color = Color.black;
                break;
            case ("Wind"):
                cardPoint.color = Color.black;
                break;
            case ("Water"):
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
        Transform nameBG = card.transform.Find("CardName");
        Transform name = nameBG.transform.Find("Name");
        TextMeshProUGUI cardName = name.GetComponent<TextMeshProUGUI>();

        cardName.text = nameInput;
    }

    public void SetImage(GameObject card, string name)
    {
        Transform illus = card.transform.Find("Illustration");
        Image illustrate = illus.GetComponent<Image>();

        Sprite sprite = Resources.Load<Sprite>("Assets/Images/CardImages/Basic/" + name + ".png");

        if (sprite != null)
        {
            illustrate.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Sprite '{name}' not found in Resources!");
        }
    }

    #endregion
}
