using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoundController : MonoBehaviour
{
    bool isInitialized = false;

    [SerializeField] Player currentPlayer;
    [SerializeField] PlayerController playerController;
    [SerializeField] FunctionController functionController;
    [SerializeField] TimingController timingController;

    TextMeshProUGUI txbStageIndicator;
    public Button btnNextStage;
    Button btnConfirm;
    Button btnCancel;

    string[] Stages = new string[7] { "Start", "Judge", "Draw", "Action", "Discard", "End", "Outside" };
    [SerializeField] private string current;

    #region Mono Behaviour
    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();
        timingController = GameObject.Find("GameManager").GetComponent<TimingController>();

        txbStageIndicator = transform.Find("txbStageIndicator").GetComponent<TextMeshProUGUI>();

        btnNextStage = transform.Find("btnNextStage").GetComponent<Button>();
        btnConfirm = transform.Find("btnConfirm").GetComponent<Button>();
        btnCancel = transform.Find("btnCancel").GetComponent<Button>();

        btnNextStage.onClick.AddListener(ProceedToNextStage);
        btnNextStage.gameObject.SetActive(false);
        btnConfirm.onClick.AddListener(Confirm);
        btnCancel.onClick.AddListener(Cancel);

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerStates();
    }
    #endregion


    #region Stage setter
    public void RoundStart()
    {
        currentPlayer.stage = 0; // Reset stage

        StageDefault();

        ScanStages();
    }

    // Called when btnNextStage is pressed
    public void ProceedToNextStage()
    {
        functionController.CardClear(currentPlayer.AfterPickCard, 2);
        functionController.CardClear(currentPlayer.AfterPick1Card, 2);

        currentPlayer.stage++; // update current stage
        if (currentPlayer.stage == 3)
            currentPlayer.limitAttack = 1;

        current = Stages[currentPlayer.stage];
        UpdateStageIndicator(current);

        CardTiming(false);
        ScanStages();
    }

    public void ScanStages()
    {
        // Check the currentStage, then act based on it
        switch (currentPlayer.stage)
        {
            case 0:
                if (currentPlayer.isStageStart)
                {
                    ProceedToNextStage();
                }
                else
                {
                    currentPlayer.stage = 5;
                    ScanStages();
                }
                break;
            case 1:
                if (currentPlayer.isStageJudge)
                {
                }
                ProceedToNextStage();
                break;
            case 2:
                if (currentPlayer.isStageDraw)
                {
                    playerController.PlayerDraw(2);
                }
                ProceedToNextStage();
                break;
            case 3:
                if (currentPlayer.isStageAction)
                {
                    currentPlayer.limitCard = 1;
                    CardTiming(true);
                    timingController.StageAction(currentPlayer);
                    if (currentPlayer.limitAttack == 0)
                        Debug.Log("You can no longer attack");
                }
                break;
            case 4:
                if (currentPlayer.isStageDiscard)
                {
                    if (currentPlayer.handCard.Count > currentPlayer.limitHand)
                    {
                        currentPlayer.limitCard = currentPlayer.handCard.Count - currentPlayer.limitHand;
                        Debug.Log("Please discard " + currentPlayer.limitCard + " cards");
                        currentPlayer.Status = 2;

                        CardTiming(true);
                    }
                    else
                        ProceedToNextStage();
                }
                else
                    ProceedToNextStage();
                break;
            case 5:
                if (currentPlayer.isStageEnd)
                {
                    functionController.NextPlayerTurn();
                    currentPlayer.limitAttack = 0;
                }
                ProceedToNextStage();
                break;
            case 6:
                break;
            default:
                break;
        }
    }

    #endregion


    #region UI setters
    void CardTiming(bool active)
    {
        if (currentPlayer != null)
        {
            currentPlayer.isNeedCard = active;
            btnNextStage.gameObject.SetActive(active);

            if (currentPlayer.stage == 4)
            {
                currentPlayer.isNeedCard = false;
                timingController.SetActiveAll(currentPlayer, active);
                btnNextStage.gameObject.SetActive(false);
            }
        }
    }

    void UpdateStageIndicator(string currentStage)
    {
        txbStageIndicator.text = currentStage;
    }

    public void StageDefault()
    {
        currentPlayer.isStageStart = true;
        currentPlayer.isStageEnd = true;

        currentPlayer.isStageJudge = false;
        currentPlayer.isStageDraw = true;
        currentPlayer.isStageDiscard = true;
    }

    public void GetPlayerStates()
    {
        btnCancelActive();
        btnConfirmActive();
    }

    #endregion

    #region Buttons

    public void btnConfirmActive()
    {
        List<Deck> pickedList = currentPlayer.AfterPickCard;
        Deck pickedDeck = currentPlayer.AfterPick1Card;

        if (pickedDeck != null || (pickedList != null && pickedList.Count == currentPlayer.limitCard && pickedList.Count != 0))
        {
            if (currentPlayer.stage == 4)
            {
                btnConfirm.interactable = true;
            }
            else if (currentPlayer.isNeedCard)
            {
                if (currentPlayer.isRespond)
                {
                    btnConfirm.interactable = true;
                }
                else
                {
                    if (currentPlayer.isPickTarget == null && (currentPlayer.isPickTargets == null || currentPlayer.isPickTargets.Count == 0))
                    {
                        btnConfirm.interactable = false;
                    }
                    else
                    {
                        btnConfirm.interactable = true;
                    }
                }
            }
        }
        else
        {
            btnConfirm.interactable = false;
        }
    }

    public void btnCancelActive()
    {
        List<Deck> pickedList = currentPlayer.AfterPickCard;
        Deck pickedDeck = currentPlayer.AfterPick1Card;

        if (pickedDeck != null 
            || (pickedList != null && pickedList.Count == currentPlayer.limitCard && pickedList.Count != 0) 
            || currentPlayer.isRespond
            || (functionController.processCase != 0 && functionController.playerList[functionController.respondIndex] == currentPlayer))
        {
            btnCancel.interactable = true;
        }
        else
        {
            btnCancel.interactable = false;
        }
    }

    public void Confirm()
    {
        if (currentPlayer.isNeedCard)
        {
            if (!currentPlayer.isRespond)
            {
                functionController.UseCard(currentPlayer);
            }
            else if (currentPlayer.isRespond)
            {
                functionController.RespondCard(currentPlayer, false);
            }
        }
        else
        {
            functionController.DiscardCard(currentPlayer);
            if (currentPlayer.stage == 4)
                ProceedToNextStage();
        }

        //functionController.SetInteractability(currentPlayer);
    }

    public void Cancel()
    {
        List<Deck> pickedList = currentPlayer.AfterPickCard;
        Deck pickedDeck = currentPlayer.AfterPick1Card;

        if (pickedDeck != null || (pickedList != null && pickedList.Count != 0))
        {
            functionController.CardClear(currentPlayer.AfterPickCard, 1);
            functionController.CardClear(currentPlayer.AfterPick1Card, 1);

            currentPlayer.AfterPickCard.Clear();
            currentPlayer.AfterPick1Card = null;

            functionController.PlayerClear(0);
        }
        else
        {
            if (functionController.processCase != 0)
                functionController.SkipScan();
            if (currentPlayer.isRespond)
                functionController.RespondCard(currentPlayer, true);
        }

        functionController.SetInteractability(currentPlayer);
    }

    #endregion
}
