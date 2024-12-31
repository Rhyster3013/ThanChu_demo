using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyOnlineController : MonoBehaviour, IPointerClickHandler
{
    FunctOnlineController functionController;

    public Outline outline;
    public GameObject overlayImage; // Image overlay

    public TextMeshProUGUI txtHP;
    public TextMeshProUGUI txtCardCount;

    public ulong enemyId;
    public bool isPickable;
    public bool isPickedAsTarget;

    #region MonoBehaviour

    private void Start()
    {

        overlayImage = transform.Find("Active").gameObject;

        Transform HPs = transform.Find("HPs");
        txtHP = HPs.Find("TxtHP").GetComponent<TextMeshProUGUI>();
        txtCardCount = transform.Find("CardCountBG").GetComponentInChildren<TextMeshProUGUI>();
        outline = GetComponent<Outline>();

        functionController = GameObject.Find("GameManager").GetComponent<FunctOnlineController>();

        outline.enabled = false;
    }

    #endregion

    public void UpdateHp(int newHP)
    {
        txtHP.text = newHP.ToString();
    }

    public void UpdateCardCount(int count)
    {
        txtCardCount.text = count.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPickable)
        {
            //functionController.PlayerUpdate(currentPlayer);
        }
        else
        {
            Debug.Log("Target not pickable");
        }
    }
}
