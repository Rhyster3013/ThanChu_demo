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

        currentPlayer.stage++; // update current stage

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
                }
                break;
            case 4:
                if (currentPlayer.isStageDiscard)
                {
                    if (currentPlayer.handCard.Count > currentPlayer.limitHand)
                    {
                        currentPlayer.limitCard = currentPlayer.handCard.Count - currentPlayer.limitHand;
                        Debug.Log("Please discard " + currentPlayer.limitCard + " cards");
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
                }
                ProceedToNextStage();
                break;
            case 6:
                break;
            default:
                break;
        }
    }

    #region State setters
    void CardTiming(bool active)
    {
        if (currentPlayer != null)
        {
            currentPlayer.isNeedCard = active;
            btnNextStage.gameObject.SetActive(active);
            timingController.SetActiveAll(currentPlayer, active);

            if (currentPlayer.stage == 4)
            {
                currentPlayer.isNeedCard = false;
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
        if (isInitialized)
        {
            ActiveButton();
            ActivePlayers();
        }
    }

    public void ActivePlayers()
    {
        //if (currentPlayer.AfterPickCard != null)
        //{
        //    functionController.AfterPickCard(currentPlayer, currentPlayer.AfterPickCard);
        //}
    }

    public void ActiveButton()
    {
        List<Deck> picked = currentPlayer.AfterPickCard;
        if (picked != null)
        {
            btnConfirm.gameObject.SetActive(true);
            btnCancel.gameObject.SetActive(true);
            if (picked.Count == currentPlayer.limitCard)
            {
                btnCancel.interactable = true;
                if (currentPlayer.stage == 4)
                {
                    btnConfirm.interactable = true;
                }
                if (currentPlayer.isNeedCard)
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
            else
            {
                btnConfirm.interactable = false;
                btnCancel.interactable = false;
            }
        }
        else
        {
            btnConfirm.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
        }
    }

    public void Confirm()
    {
        if (currentPlayer.isNeedCard)
        {
            if(currentPlayer.AfterPickCard.Count == 1)
            {
                //functionController.UseCard(currentPlayer,)
            }
        }
        else
        {
            functionController.DiscardCard(currentPlayer, currentPlayer.handCard);
            if(currentPlayer.stage == 4)
                ProceedToNextStage();
        }
    }

    public void Cancel()
    {
        functionController.CardClear(currentPlayer.AfterPickCard, 1);
    }

    #endregion
}
