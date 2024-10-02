using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundController : MonoBehaviour
{
    [SerializeField] Player currentPlayer;
    [SerializeField] PlayerController playerController;
    [SerializeField] FunctionController functionController;
    
    GameObject playerGO;

    TextMeshProUGUI txbStageIndicator;
    public Button btnNextStage;

    string[] Stages = new string[7] { "Start", "Judge", "Draw", "Action", "Discard", "End", "Outside" };
    public int currentStage = 6;
    [SerializeField] private string current;

    #region Mono Behaviour
    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();

        txbStageIndicator = transform.Find("txbStageIndicator").GetComponent<TextMeshProUGUI>();

        btnNextStage = transform.Find("btnNextStage").GetComponent<Button>();
        btnNextStage.onClick.AddListener(ProceedToNextStage);
    }

    // Update is called once per frame
    void Update()
    {
        current = Stages[currentStage];
        UpdateStageIndicator(Stages[currentStage]);
    }
    #endregion

    public void RoundStart()
    {
        currentStage = 0; // Reset stage

        StageDefault();

        ScanStages();
    }

    // Called when btnNextStage is pressed
    void ProceedToNextStage()
    {
        currentStage++; // update current stage

        CardTiming(false);
        ScanStages();
    }

    public void ScanStages()
    {
        switch (currentStage)
        {
            case 0:
                if (currentPlayer.isStageStart)
                {
                    ProceedToNextStage();
                }
                else
                {
                    currentStage = 5;
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
                    CardTiming(true);
                }
                break;
            case 4:
                if (currentPlayer.isStageDiscard)
                {
                    if(currentPlayer.handCard.Count > currentPlayer.CardLimit)
                    {
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

    void CardTiming(bool active)
    {
        if (active)
        {
            currentPlayer.isNeedCard = true;
            btnNextStage.interactable = true;
            playerController.ActiveCard(true);
        }
        else
        {
            btnNextStage.interactable = false;
            currentPlayer.isNeedCard = false;
            playerController.ActiveCard(false);
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
}
