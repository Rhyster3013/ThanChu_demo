using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerOnlineModel : NetworkBehaviour
{
    #region Stats

    // Initial stats

    public void InitializePlayer(string playerName, string faction)
    {
        nameAndFaction.Value = new NameAndFaction
        {
            Name = playerName,
            Faction = faction
        };
    }

    public NetworkVariable<NameAndFaction> nameAndFaction = new NetworkVariable<NameAndFaction>(
        new NameAndFaction
        {
            Name = new FixedString128Bytes("DefaultName"),
            Faction = new FixedString64Bytes("DefaultFaction")
        });

    public struct NameAndFaction : INetworkSerializable
    {
        public FixedString128Bytes Name;
        public FixedString64Bytes Faction;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Faction);
        }
    }
    public NetworkVariable<int> HP = new NetworkVariable<int>(4);
    public NetworkVariable<int> HPMax = new NetworkVariable<int>(4);

    // Ingame stats
    public NetworkVariable<int> Status = new NetworkVariable<int>(1);   // -1: Dying;    0: Dead;   1: Active;  2: Discarding or Responding
    public NetworkVariable<int> Stage = new NetworkVariable<int>(6);    // Current Stage of player, as in Draw, Action, Discard,...

    // Player limits
    public int limitKeep;
    public int limitAttack = 1;
    public int limitPick = 1;

    // Alternative stats
    public int buffDamage = 0;
    public int buffAttack = 0;

    // Player card
    public List<CardsOnline> cardsInHand;     // List of cards currently in hand
    public int cardsCount;        // The number of cards currently in hand

    #endregion


    #region Timing

    // Card Timing
    // 1. Can dung the do duoc Lenh hoac trong giai doan Ra the
    public bool isNeedCard;	    // Enable usable Cards
    public bool isRespond;
    public bool isDiscard;

    // 3
    public List<CardsOnline> AfterPickCard; // A list of picked cards
    public CardsOnline AfterPick1Card;

    // 4 Show available targets and choose the amount of target as needed/available
    public List<EnemyOnlineController> isPickTargets;
    public EnemyOnlineController isPickTarget;

    // 5
    public CardsOnline isUseCard;         // Confirm Using card

    // 6
    public List<EnemyOnlineController> isTargetPlayer;	    // When targetting
    // 7
    public CardsOnline isTargetted;	        // When being targetted, mostly passively
    // 8
    public bool isAfterTargetPlayer;	// After targetting
    // 9
    public CardsOnline isAfterTargetted;      // After being targetted, mostly passively

    // Stage timing
    public bool isStageStart { get; set; } = true;      // Indicates if the player has a round
    public bool isStageJudge { get; set; } = false;     // If the player has a card in Judge Area
    public bool isStageDraw { get; set; } = true;       // If the player can draw
    public bool isStageAction { get; set; } = true;     // If the player can use cards in their turn
    public bool isStageDiscard { get; set; } = true;    // If the player needs to discard cards
    public bool isStageEnd { get; set; } = true;        // Indicates if it is about to begin another player's turn

    #endregion


    #region UI

    public bool isPickedAsTarget = false;
    public bool isPickable = false;

    #endregion


    #region Buttons

    public bool isConfirm;
    public bool isCancel;

    #endregion

}
