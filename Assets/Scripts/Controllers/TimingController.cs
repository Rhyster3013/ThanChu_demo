using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            List<Deck> list = new List<Deck>();
            list = player.handCard;

            if (list != null)
            {
                foreach (Deck deck in list)
                {
                    if (string.Compare(deck.Name, cardName) == 0)
                    {
                        deck.isUsable = true;
                        deck.isActive = true;
                    }
                    else
                    {
                        deck.isUsable = false;
                        deck.isActive = false;
                    }
                }
            }
        }
    }

    public void SetUsableByName(Player player, List<string> cardName)
    {
        if (player != null)
        {
            List<Deck> list = new List<Deck>();
            list = player.handCard;

            if (list != null)
            {
                foreach (Deck deck in list)
                {
                    if (cardName.Contains(deck.Name))
                    {
                        deck.isUsable = true;
                        deck.isActive = true;
                    }
                    else
                        deck.isUsable = false;
                }
            }
        }
    }

    public void StageAction(Player player)
    {
        if (player != null)
        {
            SetUsableByName(player, attacks);
        }
    }

    public void IsAfterTargetted(Player target, Deck cardUsed)
    {
        if (target != null)
        {
            target.isNeedCard = true;
            List<Deck> list = target.handCard;

            if (list != null)
            {
                switch (cardUsed.Name)
                {
                    case "Attack":
                        SetUsableByName(target, "Dodge");
                        break;
                }
            }
            else return;
        }
    }
}
