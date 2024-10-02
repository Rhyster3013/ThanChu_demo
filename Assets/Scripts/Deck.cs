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


    public bool isActive;
    public bool isInHand;
}
