using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textMessage;

    public void SetText(string s)
    {
        textMessage.text = s;
    }
}

