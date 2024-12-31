
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class CardsOnline : INetworkSerializable
{
    [Range(0, 30)]
    public string Name;
    public string Description;

    [Range(1, 13)]
    public int Number;
    public string Color;
    public string Element;

    public int Targets = 1;
    public int Damage = 0;

    public ulong ownerId;         

    public bool isActive;           // Decides whether the card is interactable
    public bool isUsable;           // Decides whether the card is usable
    public bool isInHand;           // Decides whether the card is in a player's handCards

    public bool isProcessing;

    // 2
    public bool isPickCard;         // Choose Cards that are needed/available	// Just a confirmation step

    public CardsOnline(string name, string description, int number, string color, string element, int targets, int damage)
    {
        Name = name;
        Description = description;
        Number = number;
        Color = color;
        Element = element;
        Targets = targets;
        Damage = damage;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Description);
        serializer.SerializeValue(ref Number);
        serializer.SerializeValue(ref Color);
        serializer.SerializeValue(ref Element);
        serializer.SerializeValue(ref Targets);
        serializer.SerializeValue(ref Damage);
    }
}
