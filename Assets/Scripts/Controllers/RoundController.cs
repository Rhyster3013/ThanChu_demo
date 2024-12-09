using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class RoundController : MonoBehaviour
{
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
        currentPlayer.Stage = 0; // Reset Stage

        StageDefault();

        ScanStages();
    }

    // Called when btnNextStage is pressed
    public void ProceedToNextStage()
    {
        functionController.CardClear(currentPlayer.AfterPickCard, 2);
        functionController.CardClear(currentPlayer.AfterPick1Card, 2);

        functionController.ClearCardAndTarget(currentPlayer);

        currentPlayer.Stage++; // update current Stage

        if (currentPlayer.Stage == 3)
            currentPlayer.limitAttack = 1;

        current = Stages[currentPlayer.Stage];
        UpdateStageIndicator(current);

        CardTiming(false);
        ScanStages();
    }

    public void ScanStages()
    {
        functionController.CardClear(currentPlayer.cardsInHand, 2);

        // Check the currentStage, then act based on it
        switch (currentPlayer.Stage)
        {
            case 0:
                if (currentPlayer.isStageStart)
                {
                    ProceedToNextStage();
                }
                else
                {
                    currentPlayer.Stage = 5;
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
                    currentPlayer.limitPick = 1;
                    CardTiming(true);

                    timingController.StageAction(currentPlayer);
                }
                break;
            case 4:
                if (currentPlayer.isStageDiscard)
                {
                    if (currentPlayer.cardsInHand.Count > currentPlayer.limitKeep)
                    {
                        currentPlayer.limitPick = currentPlayer.cardsInHand.Count - currentPlayer.limitKeep;
                        Debug.Log("Please discard " + currentPlayer.limitPick + " cards");
                        currentPlayer.isDiscard = true;

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
                // Stop player from using more cards
                functionController.CardClear(currentPlayer.cardsInHand, 2);
                // isNeedCard and isRespond is disabled
                functionController.PlayerClear(currentPlayer, 2);
                break;
            default:
                break;
        }

        //functionController.ButtonInteractability(currentPlayer);
    }

    #endregion


    #region UI setters
    void CardTiming(bool active)
    {
        if (currentPlayer != null)
        {
            currentPlayer.isNeedCard = active;
            btnNextStage.gameObject.SetActive(active);

            if (currentPlayer.Stage == 4)
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
        if (currentPlayer.isConfirm)
            btnConfirm.interactable = true;
        else if (!currentPlayer.isConfirm)
            btnConfirm.interactable = false;
    }

    public void btnCancelActive()
    {
        if (currentPlayer.isCancel)
            btnCancel.interactable = true;
        else if (!currentPlayer.isCancel)
            btnCancel.interactable = false;
    }

    public void Confirm()
    {
        currentPlayer.isConfirm = false;
        currentPlayer.isCancel = false;

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
        else if (currentPlayer.isDiscard)
        {
            functionController.DiscardCard(currentPlayer);
        }

        functionController.SetInteractability(currentPlayer);
        functionController.ButtonInteractability(currentPlayer);
    }

    public void Cancel()
    {
        List<Cards> pickedList = currentPlayer.AfterPickCard;
        Cards pickedDeck = currentPlayer.AfterPick1Card;

        currentPlayer.isCancel = false;

        if (pickedDeck != null || (pickedList != null && pickedList.Count != 0))
        {
            functionController.ClearCardAndTarget(currentPlayer);
        }
        else
        {
            if (functionController.processCase != 0)
            {
                functionController.SkipScan();
                functionController.CardClear(currentPlayer.cardsInHand, 3);
            }
            if (currentPlayer.isRespond)
                functionController.RespondCard(currentPlayer, true);
        }

        functionController.SetInteractability(currentPlayer);
        functionController.ButtonInteractability(currentPlayer);
    }

    #endregion
}
