//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;
//using Unity.Collections; // Thêm thư viện này để sử dụng FixedString

//public class PlayerPrefab : NetworkBehaviour
//{
//    // Biến lưu trữ tên người chơi, sử dụng FixedString32Bytes
//    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

//    public string PlayerName => playerName.Value.ToString();

//    public override void OnNetworkSpawn()
//    {
//        if (IsServer)
//        {
//            // Đặt tên mặc định cho Host và các Client
//            playerName.Value = IsHost ? "HostPlayer" : $"Player{OwnerClientId}";
//        }

//        // Cập nhật tên khi có thay đổi
//        playerName.OnValueChanged += OnPlayerNameChanged;

//        // Hiển thị tên trên giao diện
//        UpdateUI(playerName.Value.ToString());
//    }

//    private void OnPlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
//    {
//        UpdateUI(newName.ToString());
//    }

//    private void UpdateUI(string name)
//    {
//        // Gửi tên người chơi đến ListViewManager
//        if (IsOwner && IsClient)
//        {
//            FindObjectOfType<ListViewManager>().AddPlayerToList(OwnerClientId, name);
//        }
//    }
//}
