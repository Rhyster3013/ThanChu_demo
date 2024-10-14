using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
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
    public DeckManager deckManager;

    #region Gameplay

    public int getDistance(Player souce, Player target)
    {
        int distance = 0;

        return distance;
    }

    #endregion

    #region Card Activations 
    public void CardActivate(Deck card, bool isActive)
    {
        if (isActive)
        {
            card.isActive = true;
        }
        else
        {
            card.isActive = false;
        }
    }

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


    public void GetPickedCard(int limit, Player currentPlayer)
    {
        if (currentPlayer.handCard != null
            && currentPlayer.AfterPickCard != null
            && currentPlayer.AfterPickCard.Count < limit
            && currentPlayer != null)
        {
            foreach (Deck card in currentPlayer.handCard)
            {
                if (card.isPickCard
                    && isNotPicked(currentPlayer.AfterPickCard, card))
                {
                    currentPlayer.AfterPickCard.Add(card);
                }
                else if (card.isPickCard == false)
                {
                    currentPlayer.AfterPickCard.Remove(card);
                }
            }
        }
    }

    public void SetInteractability(int limit, Player currentPlayer)
    {
        if (currentPlayer != null && currentPlayer.handCard != null)
        {
            if (currentPlayer.AfterPickCard != null
                && currentPlayer.AfterPickCard.Count == limit)
            {
                foreach (Deck card in currentPlayer.handCard)
                {
                    if (card.isPickCard == false)
                    {
                        card.isActive = false;
                    }
                }

                for (int i = currentPlayer.AfterPickCard.Count - 1; i >= 0; i--)
                {
                    if (!currentPlayer.AfterPickCard[i].isPickCard)
                    {
                        currentPlayer.AfterPickCard.RemoveAt(i);
                    }
                }
            }
            else
            {
                foreach (Deck deck in currentPlayer.handCard)
                {
                    deck.isActive = true;
                }
            }
        }
    }

    public void CardClear(Deck deck, int index)
    {
        if (deck != null)
        {
            switch (index)
            {
                case 0:
                    deck.isActive = false;
                    deck.isUsable = false;
                    deck.isPickCard = false;
                    deck.isInHand = false;
                    deck.isPickTarget = null;
                    break;
                case 1:
                    deck.isPickCard = false;
                    break;
                case 2:
                    deck.isActive = false;
                    deck.isUsable = false;
                    deck.isPickCard = false;
                    break;
                case 3:
                    deck.isActive = false;
                    deck.isUsable = false;
                    break;
                case 4:
                    deck.isPickTarget = null;
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
        list.Clear();
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

    public void DrawFromDeck(Player player, int amount)
    {
        deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        if (deckManager != null && deckManager.drawDecks != null && deckManager.drawDecks.Count > 0)
        {
            List<Deck> drawDeck = deckManager.drawDecks;
            //List<Deck> source;
            if (amount < drawDeck.Count && player.handCard != null)
            {
                for (int i = 0; i < amount; i++)
                {
                    Deck deck = drawDeck[0];
                    player.handCard.Add(drawDeck[0]);
                    drawDeck.RemoveAt(0);
                    deck.isInHand = true;
                }
            }
            else
            {
                Debug.Log("Not enough cards");
            }
        }
        else
        {
            Debug.LogError("DeckManager or drawDeck is null or empty!");
        }
    }

    public void DiscardCard(Player player, List<Deck> handCards)
    {
        DeckManager deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        List<Deck> discardDeck = deckManager.discardDecks;
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            Deck deck = handCards[i];
            if (player.AfterPickCard.Contains(deck))
            {
                MoveCard(discardDeck, player.handCard, deck);
                player.AfterPickCard.Remove(deck);
                CardClear(deck, 0);
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

    public void GameStart()
    {
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
}
