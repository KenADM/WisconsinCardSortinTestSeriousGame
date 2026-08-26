using UnityEngine;
using System;
using TMPro;



public class StartingScreenManager : MonoBehaviour
{
    // may be used
    public TextMeshProUGUI testo;
    public TMP_InputField input;
    public GameObject startMenu;

    public static event Action StartGame;
    public static event Action TutorialGame;

    void Awake()
    {
        input.text = ExperimentSettings.userCode;
    }
    void Start()
    {
        //StartGame?.Invoke("ciao");
    }

    bool CheckInputField(){
        return input.text == "" ? false : true;
    }

    public void StartTutorialHandler(){
        if(CheckInputField()){
            Debug.Log("ciao");
            ExperimentSettings.userCode = input.text;
            TutorialGame?.Invoke();
            startMenu.SetActive(false);
        }else{
            testo.text = testo.text + "!";
        }
    }

}