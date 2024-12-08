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

    public void SetActiveAll(Player player, bool isActive)
    {
        if (player != null)
        {
            List<Deck> list = new List<Deck>();
            list = player.handCard;

            if (list != null)
            {
                foreach (Deck deck in list)
                {
                    deck.isActive = isActive;
                    deck.isUsable = isActive;
                }
            }
        }
    }

    public void SetUsableByName(Player player, string cardName)
    {
        if(player != null)
        {
            List<Deck> list = player.handCard;

            if (HasNo(player, cardName))
            {
                Debug.Log(player + " have no " + cardName + " cards");
            }
            else
            {
                player.limitCard = 1;
                player.isNeedCard = true;

                foreach (Deck deck in list)
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

    public void SetUsableByName(Player player, List<string> cardName)
    {
        if (player != null)
        {
            List<Deck> list = player.handCard;

            if (HasNo(player, cardName))
            {
                Debug.Log("You have no " + cardName + " cards");
            }
            else
            {
                player.limitCard = 1;
                player.isNeedCard = true;

                foreach (Deck deck in list)
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

    public void StageAction(Player player)
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

    public void IsAfterTargetted(Player target, Deck cardUsed)
    {
        if (target != null)
        {
            Debug.Log(target + " please respond with a card");

            target.isNeedCard = true;
            target.limitCard = 1;
            List<Deck> list = target.handCard;

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

    public bool HasNo(Player player, string cardName)
    {
        bool hasNo = true;
        List<Deck> handCard = player.handCard;

        foreach (Deck deck in handCard)
        {
            if (deck.Name == cardName)
            {
                hasNo = false;
            }
        }

        return hasNo;
    }

    public bool HasNo(Player player, List<string> cardName)
    {
        bool hasNo = true;
        List<Deck> handCard = player.handCard;

        foreach (Deck deck in handCard)
        {
            if (cardName.Contains(deck.Name))
            {
                hasNo = false;
            }
        }

        return hasNo;
    }
}
