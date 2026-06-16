using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using SpecialObjects;

public class ResponsePopUp : MonoBehaviour
{   
    private TextMeshProUGUI testo;
    
    void Awake()
    {   
        Debug.Log("Hello");
        testo = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Debug.Log("Hello again");
        GameLogic.responsePopUp += ShowTextHandler;
    }

    void OnDisable()
    {
        Debug.Log("Goodbye");
        GameLogic.responsePopUp -= ShowTextHandler;
    }

    void ShowTextHandler(bool show)
    {
        Debug.Log("Show Text: " + show);
        testo.enabled = true;
        testo.text = show ? "Corretto!" : "Sbagliato!";
    }
    void Start()
    {
        
    }


}
