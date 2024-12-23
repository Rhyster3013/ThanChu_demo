using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerReady : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> PlayerAlias = new NetworkVariable<FixedString64Bytes>("Unknown");
    public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false);
    public NetworkVariable<int> playOrder = new NetworkVariable<int>();

    [Header("UI References")]
    public TMP_Text playerNameText;
    public TMP_Text playerStatusText;
    public TMP_Text playOrderText;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player instantiated");
        if (IsServer)
        {
            PlayerAlias.Value = $"Player {OwnerClientId}";
            isReady.Value = false;
        }

        if (!playerNameText || !playerStatusText || !playOrderText)
        {
            Debug.LogError("UI References are not set in PlayerReady!");
            return;
        }

        //PlayerAlias.OnValueChanged += OnPlayerAliasChanged;
        isReady.OnValueChanged += OnIsReadyChanged;
        playOrder.OnValueChanged += OnPlayOrderChanged;

        UpdateUI();
    }

    private void OnPlayerAliasChanged(string oldAlias, string newAlias)
    {
        UpdateUI();
    }

    private void OnIsReadyChanged(bool oldValue, bool newValue)
    {
        UpdateUI();
    }

    private void OnPlayOrderChanged(int oldOrder, int newOrder)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        playerNameText.text = PlayerAlias.Value.ToString();
        playOrderText.text = $"STT: {playOrder.Value}";
        playerStatusText.text = isReady.Value ? "Ready" : "Not ready";
        playerStatusText.color = isReady.Value ? Color.red : Color.gray;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerInfoServerRpc(FixedString64Bytes playerName, bool readyStatus, int order)
    {
        PlayerAlias.Value = playerName;
        isReady.Value = readyStatus;
        playOrder.Value = order;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(bool readyStatus)
    {
        isReady.Value = readyStatus;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayOrderServerRpc(int order)
    {
        playOrder.Value = order;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(FixedString64Bytes playerName)
    {
        PlayerAlias.Value = playerName; 
    }
    //[ServerRpc]
    //public void SetPlayerNameServerRpc(string playerName)
    //{
    //    PlayerName = playerName;

    //    // Send to other client
    //    UpdatePlayerNameClientRpc(playerName);
    //}

    //// Server send the updated name to other client
    //[ClientRpc]
    //public void UpdatePlayerNameClientRpc(string playerName)
    //{
    //    PlayerName = playerName;
    //    Debug.Log($"Player Name Updated: {PlayerName}");
    //}
}
