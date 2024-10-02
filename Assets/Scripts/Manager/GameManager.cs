using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
   FunctionController functionController;

    public UnityEngine.UI.Button btnStartGame;

    void Start()
    {
        btnStartGame = GameObject.Find("btnStartGame").GetComponent<UnityEngine.UI.Button>();
        btnStartGame.onClick.AddListener(GameStart);

        functionController = GameObject.Find("GameManager").GetComponent<FunctionController>();
        functionController.MatchInitialize();
    }

    private void GameStart()
    {
        btnStartGame.gameObject.SetActive(false);
        functionController.GameStart();
    }
}
