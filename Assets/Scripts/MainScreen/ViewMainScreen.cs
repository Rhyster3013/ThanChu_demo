using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ViewMainScreen : MonoBehaviour
{
    public Button btnGameOffline;
    public Button btnGameOnline;
    // Start is called before the first frame update
    void Start()
    {
        btnGameOffline.onClick.AddListener(CreateOffline);
        btnGameOnline.onClick.AddListener(CreateOnline);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateOffline()
    {
        SceneManager.LoadScene("GameMatch");
        RoomSizeInstance.Instance.online = false;
    }

    private void CreateOnline()
    {
        SceneManager.LoadScene("JoinRoom");
        RoomSizeInstance.Instance.online = true;
    }
}
