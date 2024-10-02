using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInstance : MonoBehaviour
{
    // Reference to the ScriptableObject asset
    public Deck m_Deck;

    //
    public void Initialize(Deck deck)
    {
        m_Deck = deck;
    }
}
