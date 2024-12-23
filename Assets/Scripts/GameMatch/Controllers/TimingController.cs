using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TimingController : MonoBehaviour
{
    List<string> attacks = new List<string>()
        {
            "Attack",
            "AttackFire",
            "AttackPoison"
        };

    public void SetActiveAll(PlayerModel player, bool isActive)
    {
        if (player != null)
        {
            List<Cards> list = new List<Cards>();
            list = player.cardsInHand;

            if (list != null)
            {
                foreach (Cards deck in list)
                {
                    deck.isActive = isActive;
                    deck.isUsable = isActive;
                }
            }
        }
    }

    public void SetUsableByName(PlayerModel player, string cardName)
    {
        if(player != null)
        {
            List<Cards> list = player.cardsInHand;

            if (HasNo(player, cardName))
            {
                Debug.Log(player + " have no " + cardName + " cards");
            }
            else
            {
                player.limitPick = 1;
                player.isNeedCard = true;

                foreach (Cards deck in list)
                {
                    if (string.Compare(deck.Name, cardName) == 0)
                    {
                        deck.isUsable = true;
                        deck.isActive = true;
                    }
                }
            }
        }
    }

    public void SetUsableByName(PlayerModel player, List<string> cardName)
    {
        if (player != null)
        {
            List<Cards> list = player.cardsInHand;

            if (HasNo(player, cardName))
            {
                Debug.Log("You have no " + cardName + " cards");
            }
            else
            {
                player.limitPick = 1;
                player.isNeedCard = true;

                foreach (Cards deck in list)
                {
                    if (cardName.Contains(deck.Name))
                    {
                        deck.isUsable = true;
                        deck.isActive = true;
                    }
                }
            }
        }
    }

    public void StageAction(PlayerModel player)
    {
        if (player != null)
        {
            if (player.limitAttack > 0)
            {
                SetUsableByName(player, "Attack");
            }
            else
            {
                Debug.Log(player + " can no longer attack");
            }

            if (player.HP < player.MaxHP)
            {
                SetUsableByName(player, "Heal");
            }
            else
            {
                Debug.Log(player + " can not heal");
            }
        }
    }

    public void IsAfterTargetted(PlayerModel target, Cards cardUsed)
    {
        if (target != null)
        {
            Debug.Log(target + " please respond with a card");

            target.isNeedCard = true;
            target.limitPick = 1;
            List<Cards> list = target.cardsInHand;

            if (list != null)
            {
                switch (cardUsed.Name)
                {
                    case "Attack":
                        SetUsableByName(target, "Dodge");
                        target.isRespond = true;
                        break;
                }
            }
            else return;
        }
    }

    public bool HasNo(PlayerModel player, string cardName)
    {
        bool hasNo = true;
        List<Cards> handCard = player.cardsInHand;

        foreach (Cards deck in handCard)
        {
            if (deck.Name == cardName)
            {
                hasNo = false;
            }
        }

        return hasNo;
    }

    public bool HasNo(PlayerModel player, List<string> cardName)
    {
        bool hasNo = true;
        List<Cards> handCard = player.cardsInHand;

        foreach (Cards deck in handCard)
        {
            if (cardName.Contains(deck.Name))
            {
                hasNo = false;
            }
        }

        return hasNo;
    }
}
