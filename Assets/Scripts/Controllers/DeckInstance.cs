using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInstance : MonoBehaviour
{
    // Reference to the ScriptableObject asset
    public Cards m_Deck;

    //
    public void Initialize(Cards deck)
    {
        m_Deck = deck;
    }
}
