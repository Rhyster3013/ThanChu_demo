using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Initial stats
    public int Id { get; set; }         // Player id or number of order
    public int MaxHP { get; set; } = 4;
    public int HP { get; set; } = 4;         // Current HP
    public string Faction { get; set; }

    // Player limits
    public int CardLimit { get; set; }
    public int AttackLimit { get; set; } = 1;

    // Alternative stats
    public int Buff { get; set; } = 0;
    public int AttackBuff { get; set; } = 0;

    // Player card
    public List<Deck> handCard;     // List of cards currently in hand
    public int numberOfCard;        // The number of cards currently in hand

    #region Timing

    // Card Timing
    // 1. Can dung the do duoc Lenh hoac trong giai doan Ra the
    public bool isNeedCard { get; set; }	    // Enable usable Cards

    // 3
    public List<Deck> AfterPickCard; // A list of picked cards

    // 5
    bool isUseCard;         // Confirm Using card

    // 6
    bool isTargetPlayer;	    // When targetting
    // 7
    bool isTargetted;	        // When being targetted, mostly passively
    // 8
    bool isAfterTargetPlayer;	// After targetting
    // 9
    public bool isAfterTargetted { get; set; }      // After being targetted, mostly passively

    // State timing
    public bool isStageStart { get; set; } = true;      // Indicates if the player has a round
    public bool isStageJudge { get; set; } = false;     // If the player has a card in Judge Area
    public bool isStageDraw { get; set; } = true;       // If the player can draw
    public bool isStageAction { get; set; } = true;     // If the player can use cards in their turn
    public bool isStageDiscard { get; set; } = true;    // If the player needs to discard cards
    public bool isStageEnd { get; set; } = true;        // Indicates if it is about to begin another player's turn

    #endregion
}
