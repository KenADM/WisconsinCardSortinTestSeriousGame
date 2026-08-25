using UnityEngine;
using System;
using TMPro;



public class StartingScreenManager : MonoBehaviour
{
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;

    public static event Action<string> StartGame;
    public static event Action<string> TutorialGame;

    void Start()
    {
        //StartGame?.Invoke("ciao");
    }

    bool CheckInputField(){
        return input.text == "" ? false : true;
    }

    public void StartGameHandler(){
        if(CheckInputField()){
            StartGame?.Invoke(input.text);
            startMenu.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }
    public void StartTutorialHandler(){
        if(CheckInputField()){
            TutorialGame?.Invoke(input.text);
            startMenu.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }

}