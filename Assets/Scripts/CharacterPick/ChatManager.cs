using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Qos.V2.Models;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private CanvasGroup chatBox;

    [SerializeField] private Button btnSend;
    private LobbyManager lobbyManager;
    private string playerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        btnSend.onClick.AddListener(HandleSendChatMessage);
        lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager == null)
        {
            Debug.LogError("LobbyManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrWhiteSpace(chatInput.text))
        {
            HandleSendChatMessage();
        }
    }

    private async void HandleSendChatMessage()
    {
        if (lobbyManager == null)
        {
            Debug.LogError("LobbyManager is not initialized.");
            return;
        }

        playerName = await lobbyManager.GetPlayerName(NetworkManager.Singleton.LocalClientId);

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name not found for this client!");
            return;
        }

        SendChatMessage(chatInput.text, playerName);
        chatInput.text = "";
    }

    public void SendChatMessage(string message, string from = null)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string fullMessage = (from != null ? from : "Anonymous") + ": " + message;

        if (IsServer)
        {
            ReceiveChatMessageClientRpc(fullMessage);
        }
        else
        {
            SendChatMessageServerRpc(fullMessage);
        }
    }

    private void AddMessage(string message)
    {
        if (chatMessagePrefab != null && chatBox != null)
        {
            ChatMessage chatMessage = Instantiate(chatMessagePrefab, chatBox.transform);
            chatMessage.SetText(message);
        }
        else
        {
            Debug.LogError("ChatMessagePrefab or ChatBox is not assigned.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message)
    {
        AddMessage(message);
    }
}