
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class FunctionController : MonoBehaviour
{
    int roomSize = 2;
    public List<GameObject> GOList = new List<GameObject>();
    public List<PlayerController> controllerList = new List<PlayerController>();
    public List<Player> playerList = new List<Player>();

    public int playerIndex = 0;
    public int respondIndex = 0;
    public int processCase = 0;

    public DeckManager deckManager;
    public CardFunctions cardName;
    TimingController timingController;

    #region Gameplay

    public void loseHP(Player player, int amount)
    {
        player.HP -= amount;

        if (player.HP <= 0)
        {
            Dying(player, true);
        }
    }

    public void HealHP(Player player)
    {
        player.HP++;
        
        if (processCase == 1)
        {
            if (player.HP <= 0)
            {
                Dying(player, true);
            }
            else
            {
                Dying(player, false);
            }
        }
    }

    public void Dying(Player player, bool stillNeed)
    {
        if (stillNeed)
        {
            PlayerClear(2);

            player.Status = -1;
            processCase = 1;

            Debug.Log(player + " is dying");

            PlayerScan();
        }
        else
        {
            player.Status = 1;
            processCase = 0;

            Debug.Log(player + " is rescued");

            // Continue current player's turn
            RoundController currentPlayer = GetRoundFromGOList(playerIndex);
            currentPlayer.ScanStages();
        }
    }

    public Player DyingPlayer()
    {
        Player dying = null;
        foreach(Player player in playerList)
        {
            if(player != null && player.Status == -1)
            {
                dying = player; break;
            }
        }

        return dying;
    }

    public int getDistance(Player source, Player target)
    {
        int distance = 0;

        return distance;
    }

    public void AfterTargetted(Player target)
    {
        timingController = GameObject.Find("GameManager").GetComponent<TimingController>();
        if (target.isAfterTargetted != null && target != null)
            timingController.IsAfterTargetted(target, target.isAfterTargetted);
    }

    public void RespondCard(Player target, bool cancel)
    {
        if (target != null)
        {
            cardName = GameObject.Find("GameManager").GetComponent<CardFunctions>();
            Deck cardUsed = target.isAfterTargetted;
            Player user = cardUsed.isInHand;

            if (cancel)
            {
                switch (cardUsed.Name)
                {
                    case "Attack":
                        cardName.Attack(user, target, cardUsed.Damage);
                        break;
                }
            }
            else
            {
                cardName.Dodge(target);
                DiscardCard(user);
            }

            // If the player respond to a card during other's turn
            if (target.stage == 6)
            {
                // isNeedCard and isRespond is disabled
                PlayerClear(target, 2);

                // Stop player from using more cards
                CardClear(target.handCard, 3);

                target.Status = 1;
            }

            FinishProcess();
        }
    }

    public void UseCard(Player source)
    {
        if (source != null)
        {
            Player target = source.isPickTarget;
            //source.isNeedCard = false;

            target.isAfterTargetted = source.AfterPick1Card;
            source.AfterPick1Card.isProcessing = true;

            Debug.Log("Card used");

            cardName = GameObject.Find("GameManager").GetComponent<CardFunctions>();
            switch (target.isAfterTargetted.Name)
            {
                case "Attack":
                    int damage = 1 + source.buff + source.buffAttack;
                    source.limitAttack--;
                    target.isAfterTargetted.Damage = damage;
                    Debug.Log("Attacked");

                    AfterTargetted(target);
                    target.Status = 2;
                    target.isCancel = true;

                    // Temporary stop player from using card
                    CardClear(source.handCard, 3);
                    break;
                case "Heal":
                    cardName.Heal(source, target);
                    Debug.Log("Healed");
                    break;
            }

            if (target == source)
            {
                FinishProcess();
            }

            PlayerClear(target, 0);
            PlayerClear(source, 1);
        }
    }

    public void AfterPickCard(Deck deck)
    {
        Player source = deck.isInHand;

        if (source != null && source.isNeedCard)
        {
            if (!source.isRespond)
            {
                foreach (Player target in playerList)
                {
                    if (target != source)
                    {
                        target.isPickable = true;
                    }
                    else
                    {
                        target.isPickable = false;
                    }
                }
            }
            else
            {

            }
        }
    }

    #endregion


    #region Player Scans

    public void PlayerScan()
    {
        respondIndex = playerIndex;

        ContinueScan();
    }

    public void ContinueScan()
    {
        switch (processCase)
        {
            case 1:
                Debug.Log("Would " + playerList[respondIndex] + " like to rescue");
                timingController.SetUsableByName(playerList[respondIndex], "Heal");
                break;
        }

        playerList[respondIndex].isCancel = true;
    }

    public void SkipScan()
    {
        // If respondIndex = last player, reset. Else, continue ++
        if (respondIndex == playerList.Count - 1)
            respondIndex = 0;
        else
            respondIndex++;

        if (respondIndex == playerIndex)
        {
            switch (processCase)
            {
                case 1:
                    Debug.Log(DyingPlayer() + " is dead");
                    DyingPlayer().Status = 0;
                    GameEnd();
                    break;
            }

            return;
        }

        ContinueScan();
    }

    public void FinishProcess()
    {
        int alive = CheckAlive();
        for (int i = 0; i < alive; i++)
        {
            if (playerList[i].Status == 1)
            {
                // Continue current player's turn
                RoundController currentPlayer = GetRoundFromGOList(i);
                currentPlayer.ScanStages();
            }
        }
    }

    public void AssignTarget(Player target, Player user)
    {
        if (!target.isPickedTarget)
        {
            target.isPickedTarget = true;
            user.isPickTarget = target;
        }
        else
        {
            target.isPickedTarget = false;
            user.isPickTarget = null;
        }

        if (user.isPickTarget != null || user.isPickTargets.Count > 0)
        {
            user.isConfirm = true;
        }
        else
        {
            user.isConfirm = false;
        }
    }

    public void AssignTargetAuto(Deck cardUsed)
    {
        Player user = cardUsed.isInHand;
        if (cardUsed != null && user != null)
        {
            string cardName = cardUsed.Name;

            switch (cardName)
            {
                case "Heal":
                    if (processCase == 1)
                    {
                        user.isPickTarget = DyingPlayer();
                    }
                    else if (user.stage == 3)
                    {
                        user.isPickTarget = user;
                    }
                    break;

            }

            user.isConfirm = true;
        }
    }


    #endregion


    #region Card Activations 

    public void GetPickedCards(Deck deck)
    {
        Player owner = deck.isInHand;

        if (owner.limitCard == 1)
        {
            if (deck.isPickCard)
            {
                owner.AfterPick1Card = deck;
                owner.isCancel = true;
            }
            else
            {
                owner.AfterPick1Card = null;
                owner.isCancel = false;
            }
        }
        else
        {
            if (deck.isPickCard)
            {
                owner.AfterPickCard.Add(deck);
                owner.isCancel = true;
            }
            else
            {
                owner.AfterPickCard.Remove(deck);
                if (owner.AfterPickCard.Count == 0)
                    owner.isCancel = false;
            }
        }

        SetInteractability(owner);
    }

    public void SetInteractability(Player currentPlayer)
    {
        if (currentPlayer != null && currentPlayer.handCard != null)
        {
            int limit = currentPlayer.limitCard;

            if ((currentPlayer.AfterPickCard != null && currentPlayer.AfterPickCard.Count == limit && currentPlayer.limitCard != 1) 
                || (currentPlayer.AfterPick1Card != null && currentPlayer.limitCard == 1))
            {
                foreach (Deck card in currentPlayer.handCard)
                {
                    if (card.isPickCard == false)
                    {
                        card.isActive = false;
                    }
                }

                if (currentPlayer.Status == 2)
                    currentPlayer.isConfirm = true;
            }
            else
            {
                foreach (Deck deck in currentPlayer.handCard)
                {
                    deck.isActive = true;
                }

                currentPlayer.isConfirm = false;
            }

            if (currentPlayer.isAfterTargetted != null)
            {
                currentPlayer.isCancel = true;
            }
        }
    }

    public void CardClear(Deck deck, int index)
    {
        if (deck != null)
        {
            switch (index)
            {
                case 0: // Card goes into Discard pile
                    deck.isActive = false;
                    deck.isUsable = false;
                    deck.isPickCard = false;
                    deck.isInHand = null;
                    deck.isProcessing = false;
                    break;
                case 1: // Card is no longer picked
                    deck.isPickCard = false;
                    break;
                case 2:
                    deck.isActive = false;
                    deck.isUsable = false;
                    deck.isPickCard = false;
                    deck.isProcessing = false;
                    break;
                case 3:
                    deck.isActive = false;
                    deck.isUsable = false;
                    break;
            }
        }
    }

    public void CardClear(List<Deck> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardClear(list[i], index);
        }
        //list.Clear();
    }

    public void CardClear(Player owner, String name, int index)
    {
        List<Deck> list = owner.handCard;
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Compare(list[i].Name, name) == 0)
            {
                CardClear(list[i], index);
            }
        }
        //list.Clear();
    }

    public void PlayerClear(Player player, int index)
    {
        if (player != null)
        {
            switch (index)
            {
                case 0:
                    player.isPickable = false;
                    player.isPickedTarget = false;
                    break;
                case 1:
                    player.isPickTarget = null;
                    player.isPickTargets.Clear();
                    break;
                case 2:
                    player.isNeedCard = false;
                    player.isRespond = false;
                    player.limitCard = 0;
                    break;
            }
        }
    }

    public void PlayerClear(int index)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerClear(playerList[i], index);
        }
    }
    #endregion


    #region Discard and Draw
    public void MoveCard(List<Deck> target, List<Deck> source, Deck card)
    {
        if (target != null && source != null && card != null)
        {
            target.Add(card);
            source.Remove(card);
        }
    }

    public void OpenFromDeck(Player player, int amount)
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();
        List<Deck> drawDeck = deckManager.drawDecks;

        for (int i = 0; i < amount; i++)
        {
            Deck deck = drawDeck[0];
            player.handCard.Add(drawDeck[0]);
            drawDeck.RemoveAt(0);
            deck.isInHand = player;
        }
    }

    public void DrawFromDeck(Player player, int amount)
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        if (deckManager != null && deckManager.drawDecks != null)
        {
            List<Deck> drawDeck = deckManager.drawDecks;
            //List<Deck> source;
            if (amount < drawDeck.Count && player.handCard != null)
            {
                OpenFromDeck(player, amount );
            }
            else
            {
                int tempCount = amount - drawDeck.Count;
                OpenFromDeck(player, drawDeck.Count);

                deckManager.RefillDeck();
                OpenFromDeck(player, tempCount );
            }
        }
        else
        {
            Debug.LogError("DeckManager or drawDeck is null or empty!");
        }
    }

    public void DiscardCard(Player player)
    {
        DeckManager deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        List<Deck> discardDeck = deckManager.discardDecks;

        List<Deck> handCards = player.handCard;
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            Deck deck = handCards[i];
            if (player.AfterPickCard.Contains(deck))
            {
                MoveCard(discardDeck, player.handCard, deck);
                player.AfterPickCard.Remove(deck);
                CardClear(deck, 0);
            }
            if (deck == player.AfterPick1Card)
            {
                MoveCard(discardDeck, player.handCard, deck);
                player.AfterPick1Card = null;
                CardClear(deck, 0);
            }
        }
    }

    #endregion


    #region Check duplication in list

    public bool isNotPicked(List<Deck> cards, Deck cardToCheck)
    {
        bool check = true;

        if (cards != null)
        {
            foreach (Deck deck in cards)
            {
                if (deck.name == cardToCheck.name)
                    check = false;
            }
        }

        return check;
    }

    public bool isNotPicked(List<Player> target, Player playerToCheck)
    {
        bool check = true;

        if (playerToCheck != null)
        {
            foreach (Player player in target)
            {
                if (player == playerToCheck)
                    check = false;
            }
        }

        return check;
    }
    #endregion


    #region Update States


    public void CardUpdate(Deck deck)
    {
        if (deck != null)
        {
            if (deck.isPickCard == false && deck.isActive == true)
            {
                deck.isPickCard = true;

                if (deck.Targets != -1)
                {
                    AfterPickCard(deck);
                }
                else
                {
                    AssignTargetAuto(deck);
                }
            }
            else
            {
                deck.isPickCard = false;
                if (!deck.isInHand.isRespond)
                {
                    PlayerClear(0);
                }
            }

            GetPickedCards(deck);
        }
    }

    public void PlayerUpdate(Player target)
    {
        if (target != null)
        {
            Player user = null;

            foreach (Player player in playerList)
            {
                if (player != target)
                {
                    user = player;
                    break;
                }
            }

            Debug.Log(user + " is targetting " + target);
            if (user != null)
            {
                AssignTarget(target, user);
            }
        }
    }
    #endregion


    #region Game Initialize

    public void MatchInitialize()
    {
        setPlayerGO();
        getPlayerQueue();
    }

    public int CheckAlive()
    {
        int playerAlive = 0;
        foreach (Player player in playerList)
        {
            if (player.Status == 1)
                playerAlive++;
        }

        Debug.Log("There are " + playerAlive + " player alive");

        return playerAlive;
    }

    public void GameEnd()
    {
        if (CheckAlive() == 1)
        {
            GameObject EndGame = GameObject.Find("EndGame");
            EndGame.SetActive(true);

            Debug.Log("Game end");
        }
    }

    public void GameStart()
    {
        GameObject EndGame = GameObject.Find("EndGame");
        EndGame.SetActive(false);

        foreach (PlayerController player in controllerList)
        {
            player.PlayerDraw(4);
        }
        StartRoundForPlayer();
    }

    public void NextPlayerTurn()
    {
        if (playerIndex < playerList.Count-1)
        {
            playerIndex++;
        }
        else
        {
            playerIndex = 0;
        }
        StartRoundForPlayer(); // Start the next player's turn
    }

    void StartRoundForPlayer()
    {
        RoundController roundManager = GOList[playerIndex].GetComponent<RoundController>();

        // Start the current player's turn and activate the button
        roundManager.RoundStart();
    }

    private void getPlayerQueue()
    {
        for (int i = 0; i < roomSize; i++)
        {
            string playerName = "Player" + i;

            controllerList.Add(GetControllerFromGO(playerName));
            playerList.Add(GetPlayerFromGO(playerName));
            playerList[i].Status = 1;
        }
    }

    private Player GetPlayerFromGO(string playerName)
    {
        Player player = GameObject.Find(playerName).GetComponent<Player>();

        return player;
    }

    private PlayerController GetControllerFromGO(string playerName)
    {
        PlayerController player = GameObject.Find(playerName).GetComponent<PlayerController>();

        return player;
    }

    private RoundController GetRoundFromGOList(int index)
    {
        RoundController player = GOList[index].GetComponent<RoundController>();

        return player;
    }

    private void setPlayerGO()
    {
        for (int i = 0; i < roomSize; i++)
        {
            string playerName = "Player" + i;
            GameObject currentPlayerGO = GameObject.Find(playerName);

            Player player = currentPlayerGO.GetComponent<Player>();
            PlayerController playerController = currentPlayerGO.GetComponent<PlayerController>();
            RoundController roundController = currentPlayerGO.GetComponent<RoundController>();

            GOList.Add(currentPlayerGO);
            if (currentPlayerGO != null)
            {
                if (playerController == null)
                    currentPlayerGO.AddComponent<PlayerController>();
                if (player == null)
                    currentPlayerGO.AddComponent<Player>();
                if (roundController == null)
                    currentPlayerGO.AddComponent<RoundController>();
            }
        }
    }


    #endregion


    #region Unused code

    //public void AfterPickCard(Player source, List<Deck> pickedCards)
    //{
    //    if (pickedCards.Count == 0)
    //    {
    //        PlayerClear(0);
    //    }
    //    else
    //    {
    //        Deck pickedDeck = null;
    //        Player target = null;
    //        if (source.limitCard == 1)
    //        {
    //            pickedDeck = pickedCards[0];
    //            if (source.isPickTarget != null)
    //            {
    //                target = source.isPickTarget;
    //                Debug.Log("Target is: " + target.name);
    //            }
    //        }

    //        foreach (Player player in playerList)
    //        {
    //            if (pickedDeck != null)
    //            {
    //                if (source.isNeedCard && player != source && target == null)
    //                {
    //                    player.isPickable = true;
    //                }
    //                else if (player != source && target != null)
    //                {
    //                    player.isPickable = false;
    //                }
    //            }
    //        }
    //        //AssignTarget(source);
    //    }
    //}

    //public void AssignTargets(Player player)
    //{
    //    foreach(Player target in playerList)
    //    {
    //        if (target.isPickedTarget && isNotPicked(player.isTargetPlayer, target))
    //        {
    //            player.isTargetPlayer.Add(target);
    //        }
    //        else if (target.isPickedTarget == false)
    //        {
    //            player.isTargetPlayer.Remove(target);
    //        }
    //    }
    //}

    //public void GetPickedCard(int limit, Player currentPlayer)
    //{
    //    if (currentPlayer.handCard != null
    //        && currentPlayer.AfterPickCard != null
    //        && currentPlayer.AfterPickCard.Count < limit
    //        && currentPlayer != null)
    //    {
    //        foreach (Deck card in currentPlayer.handCard)
    //        {
    //            if (limit == 1)
    //            {
    //                if (card.isPickCard)
    //                {
    //                    currentPlayer.AfterPick1Card = card;
    //                    return;
    //                }
    //                else if (card.isPickCard == false)
    //                {
    //                    currentPlayer.AfterPick1Card = null;
    //                }
    //            }
    //            else
    //            {
    //                if (card.isPickCard
    //                    && isNotPicked(currentPlayer.AfterPickCard, card))
    //                {
    //                    currentPlayer.AfterPickCard.Add(card);
    //                }
    //                else if (card.isPickCard == false)
    //                {
    //                    currentPlayer.AfterPickCard.Remove(card);
    //                }
    //            }
    //        }
    //    }
    //}

    //public void UseCard(Player source, List<Player> target)
    //{
    //    if (source != null && target != null)
    //    {
    //        foreach (Player player in target)
    //        {
    //            List<Deck> decks = player.AfterPickCard;
    //            if (decks.Count == 1)
    //            {
    //                player.isAfterTargetted = decks[1];
    //                switch (player.isAfterTargetted.Name)
    //                {
    //                    case "Attack":
    //                        int damage = 1 + source.buff + source.buffAttack;
    //                        cardName.Attack(player, damage);
    //                        break;
    //                }
    //            }
    //        }
    //    }
    //}

    #endregion
}
