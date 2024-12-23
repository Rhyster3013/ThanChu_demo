using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicSettings : MonoBehaviour
{
    public Button btnOtherSettings;
    public GameObject OtherSetting;
    [SerializeField] Button btnHost;
    [SerializeField] Button btnBack;

    public TextMeshProUGUI txtRoomName;
    public TextMeshProUGUI txtRoomSize;
    public Slider sldRoomSize;

    [SerializeField] LobbyManager lobby;

    // Start is called before the first frame update
    void Start()
    {
        lobby = GetComponent<LobbyManager>();

        btnOtherSettings.onClick.AddListener(ChangeSetting);
        btnHost.onClick.AddListener(HostRoom);
        btnBack.onClick.AddListener(GoBack);

        sldRoomSize.onValueChanged.AddListener(UpdateText);

        UpdateText(sldRoomSize.value);

    }

    public void ChangeSetting()
    {
        gameObject.SetActive(false);
        OtherSetting.SetActive(true);
    }

    public void UpdateText(float value)
    {
        txtRoomSize.text = value.ToString("0"); 
    }

    public void HostRoom()
    {
        Debug.Log("Requested for lobby");
        lobby.CreateLobby(txtRoomName.text, int.Parse(txtRoomSize.text));
    }

    public void GoBack()
    {
        SceneManager.LoadScene("JoinRoom");
    }
}
