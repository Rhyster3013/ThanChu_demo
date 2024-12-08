using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSettings : MonoBehaviour
{
    Button btnOtherSettings;
    GameObject OtherSetting;
    // Start is called before the first frame update
    void Start()
    {
        btnOtherSettings = transform.Find("ButtonOther").GetComponent<Button>();
        btnOtherSettings.onClick.AddListener(ChangeSetting);
        OtherSetting = GameObject.Find("CanvasOtherSettings").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSetting()
    {
        gameObject.SetActive(false);
        OtherSetting.SetActive(true);
    }
}
