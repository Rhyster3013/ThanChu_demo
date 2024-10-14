using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Initial stats
    public int Id { get; set; }         // Player id or number of order
    public int MaxHP { get; set; } = 4;
    public int HP { get; set; } = 4;         // Current HP
    public string Faction { get; set; }
    public int stage = 6;

    // Player limits
    public int limitHand { get; set; }
    public int limitAttack { get; set; } = 1;
    public int limitCard { get; set; } = 0;

    // Alternative stats
    public int buff { get; set; } = 0;
    public int buffAttack { get; set; } = 0;

    // Player card
    public List<Deck> handCard;     // List of cards currently in hand
    public int numberOfCard;        // The number of cards currently in hand

    #region Timing

    // Card Timing
    // 1. Can dung the do duoc Lenh hoac trong giai doan Ra the
    public bool isNeedCard;	    // Enable usable Cards

    // 3
    public List<Deck> AfterPickCard; // A list of picked cards

    // 5
    public bool isUseCard;         // Confirm Using card

    // 6
    public List<Player> isTargetPlayer;	    // When targetting
    // 7
    public bool isTargetted;	        // When being targetted, mostly passively
    // 8
    public bool isAfterTargetPlayer;	// After targetting
    // 9
    public bool isAfterTargetted;      // After being targetted, mostly passively

    // State timing
    public bool isStageStart { get; set; } = true;      // Indicates if the player has a round
    public bool isStageJudge { get; set; } = false;     // If the player has a card in Judge Area
    public bool isStageDraw { get; set; } = true;       // If the player can draw
    public bool isStageAction { get; set; } = true;     // If the player can use cards in their turn
    public bool isStageDiscard { get; set; } = true;    // If the player needs to discard cards
    public bool isStageEnd { get; set; } = true;        // Indicates if it is about to begin another player's turn

    #endregion
}
