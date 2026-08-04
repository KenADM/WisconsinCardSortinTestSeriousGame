using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class StartingMenuManager : MonoBehaviour
{
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;
    public GameObject ray;

    public static event System.Action<string> startGame;


    bool CheckInputField(){
        return input.text == "" ? false : true;
    }

    public void StartGame(){
        if(CheckInputField()){
            startGame?.Invoke(input.text);
            startMenu.SetActive(false);
            ray.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }

}
