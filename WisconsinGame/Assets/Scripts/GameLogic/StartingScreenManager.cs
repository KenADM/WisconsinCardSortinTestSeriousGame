using UnityEngine;
using System;
using TMPro;

//temporary pause integration for development
using System.Collections;

public class StartingScreenManager : MonoBehaviour
{
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;

    private int numOfVariantPerCharacteristic = 4;

    public static event Action<string, int> startGame;

    void Start()
    {
        StartPause();
        startGame?.Invoke("ciao", numOfVariantPerCharacteristic);
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

    // PAUSA TEMPORANEA
    public void StartPause()
    {
        StartCoroutine(PauseRoutine());
    }

    private IEnumerator PauseRoutine()
    {
        // 1. Pause the script
        this.enabled = false; 
        
        // 2. Wait for exactly 2 seconds in the background
        yield return new WaitForSeconds(2f);
        
        // 3. Unpause the script
        this.enabled = true; 
    }
}