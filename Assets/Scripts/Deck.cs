using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Deck", order = 1)]
[CreateAssetMenu]
public class Deck : ScriptableObject
{
    [Range(0, 30)]
    public int Id;
    public string Name;
    public string Description;

    [Range(1, 13)]
    public int Number;
    public string Color;
    public string Element;


    public bool isActive;           // Decides whether the card is interactable
    public bool isUsable;           // Decides whether the card is usable
    public bool isInHand;           // Decides whether the card is in a player's handCards

    // 2
    public bool isPickCard;	        // Choose Cards that are needed/available	// Just a confirmation step
    // 4
    public bool isPickTarget;	        // Show available targets and choose the amount of target as needed/available
}
