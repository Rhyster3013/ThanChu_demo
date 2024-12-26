
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class FunctOnlineController : MonoBehaviour
{
    int roomSize = 2;
    public List<GameObject> GOList = new();
    public List<PlayerController> controllerList = new();
    public List<PlayerModel> playerList = new();
    public List<RoundController> roundList = new();

    public int playerIndex = 0;
    public int respondIndex = 0;
    public int processCase = 0;

    public DeckManager deckManager;
    public CardFunctions cardName;
    TimingController timingController;

    #region Gameplay

    public int getDistance(PlayerModel source, PlayerModel target)
    {
        int distance = 0;

        return distance;
    }

    public void AfterTargetted(PlayerModel target)
    {
        timingController = GameObject.Find("GameManager").GetComponent<TimingController>();
        if (target.isAfterTargetted != null && target != null)
            timingController.IsAfterTargetted(target, target.isAfterTargetted);
    }

    public void AfterPickCard(Cards deck, PlayerModel source)
    {
        if (source != null)
        {
            foreach (PlayerModel target in playerList)
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
    }

    #endregion


    #region Use Card and Respond

    public void RespondCard(PlayerModel target, bool cancel)
    {
        if (target != null)
        {
            cardName = GameObject.Find("GameManager").GetComponent<CardFunctions>();
            Cards cardUsed = target.isAfterTargetted;
            PlayerModel user = cardUsed.isInHand;

            switch (cardUsed.Name)
            {
                case "Attack":
                    if (cancel)
                    {
                        cardName.Attack(user, target, cardUsed.Damage);
                    }
                    else
                    {
                        cardName.Dodge(target);
                    }
                    break;
            }
        }
    }

    public void UseCard(PlayerModel source)
    {
        if (source != null)
        {
            int damage = 1 + source.buffDamage;
            PlayerModel target = source.isPickTarget;
            //source.isNeedCard = false;

            target.isAfterTargetted = source.AfterPick1Card;
            source.AfterPick1Card.isProcessing = true;

            Debug.Log("Card used");

            cardName = GameObject.Find("GameManager").GetComponent<CardFunctions>();
            switch (target.isAfterTargetted.Name)
            {
                case "Attack":
                    damage += source.buffAttack;
                    source.limitAttack--;
                    target.isAfterTargetted.Damage = damage;
                    Debug.Log("Attacked");

                    AfterTargetted(target);

                    // Temporary stop player from using card
                    CardClear(source.cardsInHand, 3);
                    break;
                case "Heal":
                    cardName.Heal(source, target);
                    Debug.Log("Healed");
                    break;
            }

            PlayerClear(target, 0);
            PlayerClear(source, 1);
        }
    }

    #endregion


    #region Heal and Damage

    public void LoseHP(PlayerModel player, int amount)
    {
        player.HP -= amount;

        if (player.HP <= 0)
        {
            Dying(player, true);
        }
        else if (processCase != 1)
        {
            FinishProcess();
        }
    }

    public void HealHP(PlayerModel player)
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

    public void Dying(PlayerModel player, bool stillNeed)
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
            processCase = 0;
            Debug.Log(player + " is rescued");

            // Continue current player's turn
            FinishProcess();
        }
    }

    public PlayerModel DyingPlayer()
    {
        PlayerModel dying = null;
        foreach (PlayerModel player in playerList)
        {
            if (player != null && player.Status == -1)
            {
                dying = player; break;
            }
        }

        return dying;
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
                DiscardCard(playerList[i]);
                // Continue current player's turn
                RoundController currentPlayer = GetRoundFromGOList(i);
                currentPlayer.ScanStages();
            }
        }
    }

    public void AssignTarget(PlayerModel target, PlayerModel user)
    {
        if (!target.isPickedAsTarget)
        {
            target.isPickedAsTarget = true;
            user.isPickTarget = target;
        }
        else
        {
            target.isPickedAsTarget = false;
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

    public void AssignTargetAuto(Cards cardUsed, PlayerModel user)
    {
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
                    else if (user.Stage == 3)
                    {
                        user.isPickTarget = user;
                    }
                    break;


            }
            user.isConfirm = true;

            //if ((user.isPickTarget != null && cardUsed.Targets == -1)
            //    || user.isRespond)
            //{
            //}
        }
    }


    #endregion


    #region Card Activations 

    public void GetPickedCards(Cards deck, PlayerModel owner)
    {
        if (owner.limitPick == 1)
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
        ButtonInteractability(owner);
    }

    public void SetInteractability(PlayerModel currentPlayer)
    {
        if (currentPlayer != null && currentPlayer.cardsInHand != null)
        {
            int limit = currentPlayer.limitPick;

            if ((currentPlayer.AfterPickCard != null && currentPlayer.AfterPickCard.Count == limit && currentPlayer.limitPick != 1)
                || (currentPlayer.AfterPick1Card != null && currentPlayer.limitPick == 1))
            {
                foreach (Cards card in currentPlayer.cardsInHand)
                {
                    if (card.isPickCard == false)
                    {
                        card.isActive = false;
                    }
                }
            }
            else
            {
                foreach (Cards deck in currentPlayer.cardsInHand)
                {
                    deck.isActive = true;
                }
            }
        }
    }

    public void ButtonInteractability(PlayerModel currentPlayer)
    {
        if (currentPlayer != null)
        {
            if (currentPlayer.cardsInHand != null)
            {
                int limit = currentPlayer.limitPick;

                if ((currentPlayer.AfterPickCard.Count == limit)
                    || (currentPlayer.AfterPick1Card != null))
                {
                    if (currentPlayer.isDiscard || currentPlayer.isRespond)
                    {
                        currentPlayer.isConfirm = true;
                    }
                    else
                    {
                        if (currentPlayer.isPickTarget != null)
                            currentPlayer.isConfirm = true;
                        else
                            currentPlayer.isConfirm = false;
                    }
                }
                else
                {
                    currentPlayer.isConfirm = false;
                }
            }

            if (currentPlayer.isAfterTargetted != null && processCase != 0 || currentPlayer.isRespond)
            {
                currentPlayer.isCancel = true;
            }
        }
    }

    #endregion


    #region Clear for cards and player


    public void CardClear(Cards deck, int index)
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

    public void CardClear(List<Cards> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardClear(list[i], index);
        }
        //list.Clear();
    }

    public void CardClear(PlayerModel owner, String name, int index)
    {
        List<Cards> list = owner.cardsInHand;
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Compare(list[i].Name, name) == 0)
            {
                CardClear(list[i], index);
            }
        }
        //list.Clear();
    }

    public void PlayerClear(PlayerModel player, int index)
    {
        if (player != null)
        {
            switch (index)
            {
                case 0:
                    player.isPickable = false;
                    player.isPickedAsTarget = false;
                    break;
                case 1:
                    player.isPickTarget = null;
                    player.isPickTargets.Clear();
                    break;
                case 2:
                    player.isNeedCard = false;
                    player.isRespond = false;
                    player.limitPick = 0;
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

    public void ClearCardAndTarget(PlayerModel currentPlayer)
    {
        CardClear(currentPlayer.AfterPickCard, 1);
        CardClear(currentPlayer.AfterPick1Card, 1);

        currentPlayer.AfterPickCard.Clear();
        currentPlayer.AfterPick1Card = null;

        PlayerClear(0);
    }


    #endregion


    #region Discard and Draw
    public void MoveCard(List<Cards> target, List<Cards> source, Cards card)
    {
        if (target != null && source != null && card != null)
        {
            target.Add(card);
            source.Remove(card);
        }
    }

    public void OpenFromDeck(PlayerModel player, int amount)
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();
        List<Cards> drawDeck = deckManager.drawDecks;

        if (player == null)
        {
            Debug.LogError("Player is null in OpenFromDeck.");
            return;
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                Cards deck = drawDeck[0];
                player.cardsInHand.Add(deck);
                drawDeck.RemoveAt(0);
                deck.isInHand = player;
            }
        }
    }

    public void DrawFromDeck(PlayerModel player, int amount)
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        if (deckManager != null && deckManager.drawDecks != null)
        {
            List<Cards> drawDeck = deckManager.drawDecks;
            //List<Deck> source;
            if (amount < drawDeck.Count && player.cardsInHand != null)
            {
                OpenFromDeck(player, amount);
            }
            else
            {
                int tempCount = amount - drawDeck.Count;
                OpenFromDeck(player, drawDeck.Count);

                deckManager.RefillDeck();
                OpenFromDeck(player, tempCount);
            }
        }
        else
        {
            Debug.LogError("DeckManager or drawDeck is null or empty!");
        }
    }

    public void DiscardCard(PlayerModel player)
    {
        DeckManager deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        List<Cards> discardDeck = deckManager.discardDecks;

        List<Cards> handCards = player.cardsInHand;
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            Cards deck = handCards[i];
            if (player.AfterPickCard.Contains(deck))
            {
                MoveCard(discardDeck, player.cardsInHand, deck);
                player.AfterPickCard.Remove(deck);
                CardClear(deck, 0);
            }
            if (deck == player.AfterPick1Card)
            {
                MoveCard(discardDeck, player.cardsInHand, deck);
                player.AfterPick1Card = null;
                CardClear(deck, 0);
            }
        }

        if (player.Stage == 4)
        {
            GetRoundFromGOList(playerIndex).ProceedToNextStage();
            player.isDiscard = false;
        }
    }

    #endregion


    #region Check duplication in list

    public bool isNotPicked(List<Cards> cards, Cards cardToCheck)
    {
        bool check = true;

        if (cards != null)
        {
            foreach (Cards deck in cards)
            {
                if (deck.name == cardToCheck.name)
                    check = false;
            }
        }

        return check;
    }

    public bool isNotPicked(List<PlayerModel> target, PlayerModel playerToCheck)
    {
        bool check = true;

        if (playerToCheck != null)
        {
            foreach (PlayerModel player in target)
            {
                if (player == playerToCheck)
                    check = false;
            }
        }

        return check;
    }
    #endregion


    #region Update States


    public void CardUpdate(Cards deck)
    {
        if (deck != null)
        {
            PlayerModel owner = deck.isInHand;

            if (deck.isPickCard == false && deck.isActive == true)
            {
                deck.isPickCard = true;

                if (!owner.isDiscard)
                {
                    if (deck.Targets == -1 || deck.Targets == 0)
                    {
                        AssignTargetAuto(deck, owner);
                    }
                    else
                    {
                        AfterPickCard(deck, owner);
                    }
                }
            }
            else
            {
                deck.isPickCard = false;
                if (!deck.isInHand.isRespond || !deck.isInHand.isDiscard)
                {
                    PlayerClear(0);
                }
            }

            GetPickedCards(deck, owner);
        }
    }

    public void PlayerUpdate(PlayerModel target)
    {
        if (target != null)
        {
            PlayerModel user = null;

            foreach (PlayerModel player in playerList)
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
        foreach (PlayerModel player in playerList)
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
        if (playerIndex < playerList.Count - 1)
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
        controllerList.Add(GetControllerFromGO("Player"));
        playerList.Add(GetPlayerFromGO("Player"));

        for (int i = 0; i < roomSize - 1; i++)
        {
            string enemyName = "Enemy" + roomSize + i;

            controllerList.Add(GetControllerFromGO(enemyName));
            playerList.Add(GetPlayerFromGO(enemyName));
            playerList[i].Status = 1;
        }
    }

    private PlayerModel GetPlayerFromGO(string playerName)
    {
        PlayerModel player = GameObject.Find(playerName).GetComponent<PlayerModel>();

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
        GameObject currentPlayerGO = GameObject.Find("Player");
        SetPlayerComponent(currentPlayerGO);

        if (roomSize == 2)
        {
            string enemyName = "Enemy20";
            GameObject enemyGO = GameObject.Find(enemyName);

            SetPlayerComponent(enemyGO);
        }
        //for (int i = 0; i < roomSize - 1; i++)
        //{
        //}
    }

    private void SetPlayerComponent(GameObject currentPlayerGO)
    {
        PlayerModel player = currentPlayerGO.GetComponent<PlayerModel>();
        PlayerController playerController = currentPlayerGO.GetComponent<PlayerController>();
        RoundController roundController = currentPlayerGO.GetComponent<RoundController>();

        GOList.Add(currentPlayerGO);

        if (currentPlayerGO != null)
        {
            if (playerController == null)
                currentPlayerGO.AddComponent<PlayerController>();
            if (player == null)
                currentPlayerGO.AddComponent<PlayerModel>();
            if (roundController == null)
                currentPlayerGO.AddComponent<RoundController>();
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
    //        if (source.limitPick == 1)
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
    //        if (target.isPickedAsTarget && isNotPicked(player.isTargetPlayer, target))
    //        {
    //            player.isTargetPlayer.Add(target);
    //        }
    //        else if (target.isPickedAsTarget == false)
    //        {
    //            player.isTargetPlayer.Remove(target);
    //        }
    //    }
    //}

    //public void GetPickedCard(int limit, Player currentPlayer)
    //{
    //    if (currentPlayer.cardsInHand != null
    //        && currentPlayer.AfterPickCard != null
    //        && currentPlayer.AfterPickCard.Count < limit
    //        && currentPlayer != null)
    //    {
    //        foreach (Deck card in currentPlayer.cardsInHand)
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
    //                        int damage = 1 + source.buffDamage + source.buffAttack;
    //                        cardName.Attack(player, damage);
    //                        break;
    //                }
    //            }
    //        }
    //    }
    //}

    #endregion


}
