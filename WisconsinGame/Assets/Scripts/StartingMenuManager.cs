using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using SpecialObjects;

public class StartingMenuManager : MonoBehaviour
{
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;

    public static event Action<string> startGame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckInputField(){
        return input.text == "" ? false : true;
    }

    public void StartGame(){ //MODIFICO cosi non sceglie text preview
        if(CheckInputField()){
            startGame?.Invoke(input.text);
            startMenu.SetActive(false);
        }else{
            testo.text = "INSERIRE CODICE UTENTE";
        }
    }

}
