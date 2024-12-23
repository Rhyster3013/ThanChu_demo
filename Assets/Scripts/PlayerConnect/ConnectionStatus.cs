using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConnectionStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            statusText.text = "Host is running.";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            statusText.text = "Connected as Client.";
        }
    }
}
