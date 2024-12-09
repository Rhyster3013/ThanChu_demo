using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id { get; set; }         // Player id or number of order

    #region UI

    public bool isPickedAsTarget = false;
    public bool isPickable = false;

    #endregion

    #region Buttons

    public bool isConfirm;
    public bool isCancel;

    #endregion


    #region Stats

    // Initial stats
    public int MaxHP = 4;
    public int HP = 4;         // Current HP
    public string Faction;

    // Ingame stats
    public int Status = 1;              // -1: Dying;    0: Dead;   1: Active;  2: Discarding or Responding
    public int Stage = 6;               // Current Stage of player, as in Draw, Action, Discard,...

    // Player limits
    public int limitKeep;
    public int limitAttack = 1;
    public int limitPick = 1;

    // Alternative stats
    public int buffDamage = 0;
    public int buffAttack = 0;

    // Player card
    public List<Cards> cardsInHand;     // List of cards currently in hand
    public int cardsCount;        // The number of cards currently in hand

    #endregion

    #region Timing

    // Card Timing
    // 1. Can dung the do duoc Lenh hoac trong giai doan Ra the
    public bool isNeedCard;	    // Enable usable Cards
    public bool isRespond;
    public bool isDiscard;

    // 3
    public List<Cards> AfterPickCard; // A list of picked cards
    public Cards AfterPick1Card;

    // 4 Show available targets and choose the amount of target as needed/available
    public List<Player> isPickTargets;
    public Player isPickTarget;

    // 5
    public Cards isUseCard;         // Confirm Using card

    // 6
    public List<Player> isTargetPlayer;	    // When targetting
    // 7
    public Cards isTargetted;	        // When being targetted, mostly passively
    // 8
    public bool isAfterTargetPlayer;	// After targetting
    // 9
    public Cards isAfterTargetted;      // After being targetted, mostly passively

    // Stage timing
    public bool isStageStart { get; set; } = true;      // Indicates if the player has a round
    public bool isStageJudge { get; set; } = false;     // If the player has a card in Judge Area
    public bool isStageDraw { get; set; } = true;       // If the player can draw
    public bool isStageAction { get; set; } = true;     // If the player can use cards in their turn
    public bool isStageDiscard { get; set; } = true;    // If the player needs to discard cards
    public bool isStageEnd { get; set; } = true;        // Indicates if it is about to begin another player's turn

    #endregion
}
