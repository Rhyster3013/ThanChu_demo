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

    public void SetUsableByName(Player player, string cardName)
    {
        List<Deck> list = new List<Deck>();
        list = player.handCard;

        if (list != null)
        {
            foreach (Deck deck in list)
            {
                if (string.Compare(deck.name, cardName) == 0)
                {
                    deck.isUsable = true;
                }
                else
                    deck.isUsable = false;
            }
        }
    }

    public void SetActiveAll(Player player, bool isActive)
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

    public void SetUsableByName(Player player, List<string> cardName)
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

    public void StageAction(Player player)
    {
        SetUsableByName(player, attacks);
    }

    public void IsAfterTargetted(Player source, Player target, Deck cardUsed)
    {
        List<Deck> list = new List<Deck>();
        list = target.handCard;

        if (list != null)
        {
            switch (cardUsed.name)
            {
                case "Attack":
                    SetUsableByName(target, "Dodge");
                    break;
            }
        }
    }
}
