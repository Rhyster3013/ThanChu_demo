using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSizeInstance : MonoBehaviour
{
    public static RoomSizeInstance Instance { get; private set; }

    public int roomSize { get; set; }

    public bool online { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
