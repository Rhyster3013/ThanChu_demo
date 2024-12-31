
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static PlayerOnlineModel;

public class FunctOnlineController : NetworkBehaviour
{
    public int roomSize = 2;

    public Dictionary<ulong, PlayerOnlineModel> playerList = new Dictionary<ulong, PlayerOnlineModel>();
    private Dictionary<ulong, RoundOnlineController> roundList = new Dictionary<ulong, RoundOnlineController>();

    public int playerIndex = 0;
    public int respondIndex = 0;
    public int processCase = 0;

    public DeckOnlineManager deckManager;
    public CardFunctions cardName;
    TimingController timingController;

    public PlayerOnlineController playerController;
    public List<GameObject> GOList = new List<GameObject>();
    public List<EnemyOnlineController> enemyList = new List<EnemyOnlineController>();

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

    public void AfterPickCard()
    {
        foreach (EnemyOnlineController target in enemyList)
        {
            target.isPickable = true;
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
        //if (source != null)
        //{
        //    int damage = 1 + source.buffDamage;
        //    PlayerOnlineModel target = source.isPickTarget;
        //    //source.isNeedCard = false;

        //    target.isAfterTargetted = source.AfterPick1Card;
        //    source.AfterPick1Card.isProcessing = true;

        //    Debug.Log("Card used");

        //    cardName = GameObject.Find("GameManager").GetComponent<CardFunctions>();
        //    switch (target.isAfterTargetted.Name)
        //    {
        //        case "Attack":
        //            damage += source.buffAttack;
        //            source.limitAttack--;
        //            target.isAfterTargetted.Damage = damage;
        //            Debug.Log("Attacked");

        //            AfterTargetted(target);

        //            // Temporary stop player from using card
        //            CardClear(source.cardsInHand, 3);
        //            break;
        //        case "Heal":
        //            cardName.Heal(source, target);
        //            Debug.Log("Healed");
        //            break;
        //    }

        //    PlayerClear(target, 0);
        //    PlayerClear(source, 1);
        //}
    }

    #endregion


    #region Heal and Damage

    [ServerRpc(RequireOwnership = false)]
    public void LoseHPServerRpc(ulong targetClientId, int amount)
    {
        if (playerList.TryGetValue(targetClientId, out PlayerOnlineModel player))
        {
            int newHp = player.HP.Value - amount;

            UpdateHpClientRpc(newHp, targetClientId);

            //if (player.HP <= 0)
            //{
            //    Dying(player, true);
            //}
            //else if (processCase != 1)
            //{
            //    FinishProcess();
            //}
        }
    }

    [ClientRpc]
    private void UpdateHpClientRpc(int newHp, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            foreach (EnemyOnlineController enemy in enemyList)
            {
                if (enemy.enemyId == targetClientId)
                    enemy.UpdateHp(newHp);
            }
        }
        else
        {
            playerController.UpdateHP(newHp);
        }
    }

    public void HealHP(PlayerModel player)
    {
        player.HP++;

        //if (processCase == 1)
        //{
        //    if (player.HP <= 0)
        //    {
        //        Dying(player, true);
        //    }
        //    else
        //    {
        //        Dying(player, false);
        //    }
        //}
    }

    //public void Dying(PlayerModel player, bool stillNeed)
    //{
    //    if (stillNeed)
    //    {
    //        PlayerClear(2);

    //        player.Status = -1;
    //        processCase = 1;

    //        Debug.Log(player + " is dying");

    //        PlayerScan();
    //    }
    //    else
    //    {
    //        processCase = 0;
    //        Debug.Log(player + " is rescued");

    //        // Continue current player's turn
    //        FinishProcess();
    //    }
    //}

    //public PlayerOnlineModel DyingPlayer()
    //{
    //    PlayerModel dying = null;
    //    foreach (PlayerModel player in playerList)
    //    {
    //        if (player != null && player.Status == -1)
    //        {
    //            dying = player; break;
    //        }
    //    }

    //    return dying;
    //}
    #endregion


    #region Player Scans

    //public void PlayerScan()
    //{
    //    respondIndex = playerIndex;

    //    ContinueScan();
    //}

    //public void ContinueScan()
    //{
    //    switch (processCase)
    //    {
    //        case 1:
    //            Debug.Log("Would " + playerList[respondIndex] + " like to rescue");
    //            timingController.SetUsableByName(playerList[respondIndex], "Heal");
    //            break;
    //    }

    //    playerList[respondIndex].isCancel = true;
    //}

    //public void SkipScan()
    //{
    //    // If respondIndex = last player, reset. Else, continue ++
    //    if (respondIndex == playerList.Count - 1)
    //        respondIndex = 0;
    //    else
    //        respondIndex++;

    //    if (respondIndex == playerIndex)
    //    {
    //        switch (processCase)
    //        {
    //            case 1:
    //                Debug.Log(DyingPlayer() + " is dead");
    //                DyingPlayer().Status = 0;
    //                GameEnd();
    //                break;
    //        }

    //        return;
    //    }

    //    ContinueScan();
    //}

    //public void FinishProcess()
    //{
    //    int alive = CheckAlive();
    //    for (int i = 0; i < alive; i++)
    //    {
    //        if (playerList[i].Status == 1)
    //        {
    //            DiscardCard(playerList[i]);
    //            // Continue current player's turn
    //            RoundController currentPlayer = GetRoundFromGOList(i);
    //            currentPlayer.ScanStages();
    //        }
    //    }
    //}

    public void AssignTarget(EnemyOnlineController target, PlayerOnlineModel user)
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

    public void AssignTargetAuto(CardsOnline cardUsed, PlayerOnlineModel user)
    {
        if (cardUsed != null && user != null)
        {
            string cardName = cardUsed.Name;

            switch (cardName)
            {
                case "Heal":
                    if (processCase == 1)
                    {
                        //user.isPickTarget = DyingPlayer();
                    }
                    //else if (user.Stage.Value == 3)
                    //{
                    //    user.isPickTarget = user;
                    //}
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

    public void GetPickedCards(CardsOnline deck, PlayerOnlineModel owner)
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

    public void SetInteractability(PlayerOnlineModel currentPlayer)
    {
        if (currentPlayer != null && currentPlayer.cardsInHand != null)
        {
            int limit = currentPlayer.limitPick;

            if ((currentPlayer.AfterPickCard != null && currentPlayer.AfterPickCard.Count == limit && currentPlayer.limitPick != 1)
                || (currentPlayer.AfterPick1Card != null && currentPlayer.limitPick == 1))
            {
                foreach (CardsOnline card in currentPlayer.cardsInHand)
                {
                    if (card.isPickCard == false)
                    {
                        card.isActive = false;
                    }
                }
            }
            else
            {
                foreach (CardsOnline deck in currentPlayer.cardsInHand)
                {
                    deck.isActive = true;
                }
            }
        }
    }

    public void ButtonInteractability(PlayerOnlineModel currentPlayer)
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


    public void CardClear(CardsOnline deck, int index)
    {
        if (deck != null)
        {
            switch (index)
            {
                case 0: // Card goes into Discard pile
                    deck.isActive = false;
                    deck.isUsable = false;
                    deck.isPickCard = false;
                    deck.ownerId = ulong.MaxValue;
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

    public void CardClear(List<CardsOnline> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardClear(list[i], index);
        }
        //list.Clear();
    }

    public void CardClear(PlayerOnlineModel owner, String name, int index)
    {
        List<CardsOnline> list = owner.cardsInHand;
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Compare(list[i].Name, name) == 0)
            {
                CardClear(list[i], index);
            }
        }
        //list.Clear();
    }

    public void PlayerClear(PlayerOnlineModel player, int index)
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
        for (int i = 0; i < enemyList.Count; i++)
        {
            //PlayerClear(enemyList[i], index);
        }
    }

    public void EnemyClear()
    {
        foreach(var item in enemyList)
        {
            item.isPickable = false;
        }
    }

    public void ClearCardAndTarget(PlayerOnlineModel currentPlayer)
    {
        CardClear(currentPlayer.AfterPickCard, 1);
        CardClear(currentPlayer.AfterPick1Card, 1);

        currentPlayer.AfterPickCard.Clear();
        currentPlayer.AfterPick1Card = null;

        //PlayerClear(0);
    }


    #endregion


    #region Discard and Draw

    [ServerRpc]
    public void DrawCardServerRpc(ulong clientId, int amount)
    {
        PlayerOnlineModel player = GetPlayerByClientId(clientId);
        if (player == null)
        {
            Debug.LogError("Player not found for clientId: " + clientId);
            return;
        }


        Debug.Log("Player : " + clientId + " draws " + amount);
        for (int i = 0; i < amount; i++)
        {
            CardsOnline drawnCard = deckManager.DrawCardFromDeck();
            if (drawnCard != null)
            {
                if (player.cardsInHand == null)
                {
                    Debug.LogWarning("cardsInHand is null for player with clientId: " + clientId);
                    player.cardsInHand = new List<CardsOnline>();
                }
                Debug.Log("Found card " + drawnCard.Name);
                player.cardsInHand.Add(drawnCard);
                drawnCard.ownerId = clientId;
            }
        }

        UpdateHandClientRpc(clientId);
    }

    [ClientRpc]
    private void UpdateHandClientRpc(ulong clientId)
    {
        List<CardsOnline> updatedHand = GetPlayerByClientId(clientId).cardsInHand;
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            if(updatedHand != null)
            {
                foreach (EnemyOnlineController enemy in enemyList)
                {
                    enemy.UpdateCardCount(updatedHand.Count);
                }
            }
        }
        else
        {
            PlayerOnlineModel player = GetPlayerByClientId(clientId);
            if (player != null)
            {
                player.cardsInHand = updatedHand;
                playerController.viewHandCards();
            }
        }
    }

    [ServerRpc]
    public void DiscardCardServerRpc(ulong clientId)
    {
        PlayerOnlineModel player = GetPlayerByClientId(clientId);
        if (player == null)
        {
            Debug.LogError("Player not found for clientId: " + clientId);
            return;
        }

        List<CardsOnline> handCards = player.cardsInHand;
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            CardsOnline deck = handCards[i];
            if (player.AfterPickCard.Contains(deck))
            {
                deckManager.DiscardCard(deck);
                player.AfterPickCard.Remove(deck);
                CardClear(deck, 0);
            }
            if (deck == player.AfterPick1Card)
            {
                deckManager.DiscardCard(deck);
                player.AfterPick1Card = null;
                CardClear(deck, 0);
            }
        }

        UpdateHandClientRpc(clientId);
    }

    #endregion


    #region Check duplication in list

    public bool isNotPicked(List<Cards> cards, Cards cardToCheck)
    {
        bool check = true;

        //if (cards != null)
        //{
        //    foreach (Cards deck in cards)
        //    {
        //        if (deck.name == cardToCheck.name)
        //            check = false;
        //    }
        //}

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


    public void CardUpdate(CardsOnline deck)
    {
        if (deck != null)
        {
            PlayerOnlineModel owner = GetPlayerByClientId(deck.ownerId);

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
                        AfterPickCard();
                    }
                }
            }
            else
            {
                deck.isPickCard = false;
                if (!GetPlayerByClientId(deck.ownerId).isRespond || !GetPlayerByClientId(deck.ownerId).isDiscard)
                {
                    EnemyClear();
                }
            }

            GetPickedCards(deck, owner);
        }
    }

    public void PlayerUpdate(EnemyOnlineController target)
    {
        Debug.Log(playerController.currentPlayer + " is targetting " + target);

        AssignTarget(target, playerController.currentPlayer);
    }
    #endregion


    #region Game Initialize

    [ServerRpc(RequireOwnership = false)]
    public void MatchInitializeServerRpc()
    {
        SetPlayerGOClientRpc();

        Debug.Log("Finish initializing");
    }

    public int CheckAlive()
    {
        int playerAlive = 0;
        //foreach (PlayerModel player in playerList)
        //{
        //    if (player.Status == 1)
        //        playerAlive++;
        //}

        //Debug.Log("There are " + playerAlive + " player alive");

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

    [ServerRpc(RequireOwnership = false)]
    public void GameStartServerRpc()
    {
        GetPlayerQueueClientRpc();
        Debug.Log("Game starting");
        DisableGameEndClientRpc();

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"Connected client: {clientId}");
            DrawCardServerRpc(clientId, 4);
        }

        //StartRoundForPlayer();
    }

    [ClientRpc]
    private void DisableGameEndClientRpc()
    {
        Debug.Log($"DisableGameEndClientRpc called on Client {NetworkManager.Singleton.LocalClientId}");
        GameObject EndGame = GameObject.Find("EndGame");
        if (EndGame != null)
        {
            EndGame.SetActive(false);
        }
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

    [ClientRpc]
    private void GetPlayerQueueClientRpc()
    {
        Debug.Log("Create game with " + roomSize + " players");

        var connectedClients = NetworkManager.Singleton.ConnectedClientsIds;
        foreach (var client in connectedClients)
        {
            Debug.Log($"Setting Client {client}");
            MapPlayerToClientIdClientRpc(client);
        }
    }

    [ClientRpc]
    private void SetPlayerGOClientRpc()
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckOnlineManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerOnlineController>();
        GameObject currentPlayerGO = GameObject.Find("Player");
        SetPlayerComponent(currentPlayerGO);

        if (roomSize == 2)
        {
            string enemyName = "Enemy20";
            GameObject enemyGO = GameObject.Find(enemyName);
            EnemyOnlineController enemy = enemyGO.GetComponent<EnemyOnlineController>();

            GOList.Add(enemyGO);
            enemyList.Add(enemy);
        }
    }

    private void SetPlayerComponent(GameObject currentPlayerGO)
    {
        PlayerOnlineModel player = currentPlayerGO.GetComponent<PlayerOnlineModel>();
        PlayerOnlineController playerController = currentPlayerGO.GetComponent<PlayerOnlineController>();
        RoundOnlineController roundController = currentPlayerGO.GetComponent<RoundOnlineController>();

        GOList.Add(currentPlayerGO);

        if (currentPlayerGO != null)
        {
            if (playerController == null)
                currentPlayerGO.AddComponent<PlayerOnlineController>();
            if (player == null)
                currentPlayerGO.AddComponent<PlayerOnlineModel>();
            if (roundController == null)
                currentPlayerGO.AddComponent<RoundOnlineController>();
        }

        Debug.Log("Components added");
    }


    #endregion


    #region Get and Set List

    public PlayerOnlineModel GetPlayerByClientId(ulong clientId)
    {
        if (playerList.TryGetValue(clientId, out PlayerOnlineModel player))
        {
            return player;
        }
        else
        {
            Debug.LogError($"Player with ClientId {clientId} not found!");
            return null;
        }
    }

    [ClientRpc]
    public void MapPlayerToClientIdClientRpc(ulong clientId)
    {
        try
        {
            Debug.Log($"MapPlayerToClientId called on Client {NetworkManager.Singleton.LocalClientId}" +
                $"with playerId {LobbyInstance.Instance.PlayerLobbyID}");
            GameObject playerGameObject = GameObject.Find("Player");
            if (playerGameObject == null)
            {
                Debug.LogError($"Cant find GO Player for clientId: {clientId}");
                return;
            }

            var playerModel = playerGameObject.GetComponent<PlayerOnlineModel>();
            if (playerModel == null)
            {
                Debug.LogError($"GO Player {playerGameObject.name} does not have PlayerOnlineModel!");
                return;
            }
            else
            {
                Debug.Log("Found player");
                playerModel.InitializePlayer(LobbyInstance.Instance.PlayerName, "Anivia");
            }

            if (!playerGameObject.TryGetComponent<NetworkObject>(out var networkObject) || !networkObject.IsSpawned)
            {
                Debug.LogWarning($"Player GameObject for clientId {clientId} is not ready yet!");
            }

            if (!playerList.ContainsKey(clientId))
            {
                playerList[clientId] = playerModel;
                Debug.Log($"Player {playerModel.nameAndFaction.Value.Name} mapped for ClientID {clientId}");
            }
            else
            {
                Debug.LogWarning($"ClientId {clientId} already existed!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    [ClientRpc]
    private void UpdatePlayerDataClientRpc(ulong clientId, NameAndFaction nameAndFaction, int hp, int maxHp)
    {
        var player = GetPlayerByClientId(clientId);
        if (player != null)
        {
            player.nameAndFaction.Value = nameAndFaction;
            player.HP.Value = hp;
            player.HPMax.Value = maxHp;
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
