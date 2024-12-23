using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : NetworkBehaviour
{
    public static LobbySceneManager Instance;

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

    // Hàm Host gọi khi nhấn StartGame
    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        Debug.Log("Host is starting the game...");
        LoadPickCharacterSceneClientRpc();
    }

    // ClientRpc gửi lệnh chuyển scene tới tất cả các client
    [ClientRpc]
    private void LoadPickCharacterSceneClientRpc()
    {
        Debug.Log("Loading PickCharacter scene...");
        NetworkManager.Singleton.SceneManager.LoadScene("PickCharacter", LoadSceneMode.Single);
    }
}
