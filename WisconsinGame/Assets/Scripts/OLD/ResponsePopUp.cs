using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class ResponsePopUp : MonoBehaviour
{   
    private TextMeshProUGUI testo;
    
    void Awake()
    {   
        testo = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameLogic.responsePopUp += ShowTextHandler;
    }

    void OnDisable()
    {
        GameLogic.responsePopUp -= ShowTextHandler;
    }

    void ShowTextHandler(bool show)
    {
        testo.enabled = true;
        testo.text = show ? "Corretto!" : "Sbagliato!";
    }
    void Start()
    {
        
    }


}
