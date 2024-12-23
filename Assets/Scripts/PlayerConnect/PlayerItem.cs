using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    // Biến tham chiếu đến Text hoặc TMP Text
    public TMP_Text playerNameText;

    // Hàm để cập nhật tên
    public void SetPlayerName(string playerName)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
        else
        {
            Debug.LogError("PlayerNameText is not assigned!");
        }
    }
}
