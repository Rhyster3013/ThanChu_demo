using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class FunctionController : MonoBehaviour
{
    public List<PlayerController> playerList = new List<PlayerController>();
    public List<GameObject> playerGOList = new List<GameObject>();

    int roomSize = 2;
    [SerializeField] int playerIndex = 0;

    public void loseHP(Player player, int dmg)
    {
        player.HP -= dmg;
    }

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

    public void MoveCard(List<Deck> target, List<Deck> source, Deck card)
    {
        target.Add(card);
        source.Remove(card);
        card.isActive = false;
        card.isInHand = false;
        card.isUsable = false;
        card.isPickCard = false;
    }

    public void DrawFromDeck(Player player, int amount)
    {
        DeckManager deckManager = GameObject.Find("DrawDeck").GetComponent<DeckManager>();

        List<Deck> drawDeck = deckManager.drawDecks;
        //List<Deck> source;
        if (amount < drawDeck.Count)
        {
            for (int i = 0; i < amount; i++)
            {
                player.handCard.Add(drawDeck[0]);
                drawDeck.RemoveAt(0);
            }
        }
        else
        {
            Debug.Log("Not enough cards");
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
            }
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


    #region Game Initialize

    public void MatchInitialize()
    {
        setPlayerGO();
        getPlayerQueue();
    }

    public void GameStart()
    {
        foreach (PlayerController player in playerList)
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
        RoundController roundManager = playerGOList[playerIndex].GetComponent<RoundController>();

        // Start the current player's turn and activate the button
        roundManager.RoundStart();
    }

    private void getPlayerQueue()
    {
        for (int i = 0; i < roomSize; i++)
        {
            string playerName = "Player" + i;
            playerList.Add(GetPlayerFromGO(playerName));
        }
    }

    private PlayerController GetPlayerFromGO(string playerName)
    {
        PlayerController player = GameObject.Find(playerName).GetComponent<PlayerController>();

        return player;
    }

    private RoundController GetRoundFromGOList(int index)
    {
        RoundController player = playerGOList[index].GetComponent<RoundController>();

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

            playerGOList.Add(currentPlayerGO);
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
