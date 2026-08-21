using UnityEngine;
using System;
using TMPro;



public class StartingScreenManager : MonoBehaviour
{
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;

    private int numOfVariantPerCharacteristic = 4;

    public static event Action<string, int> startGame;
    public static event Action<string, int> tutorialGame;

    void Start()
    {
        //startGame?.Invoke("ciao", numOfVariantPerCharacteristic);
    }

    bool CheckInputField(){
        return input.text == "" ? false : true;
    }

    public void StartGame(){
        if(CheckInputField()){
            startGame?.Invoke(input.text, numOfVariantPerCharacteristic);
            startMenu.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }
    public void StartTutorial(){
        if(CheckInputField()){
            tutorialGame?.Invoke(input.text, numOfVariantPerCharacteristic);
            startMenu.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }

}