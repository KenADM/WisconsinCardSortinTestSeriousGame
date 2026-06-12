using UnityEngine;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    private Button StartButton;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        StartButton = root.Q<Button>("StartTest");


        if (StartButton != null)
        {
            StartButton.clicked += OnButtonPressed;
        }
        else
        {
            Debug.LogWarning("Could not find a button named 'StartTest'!");
        }
    }

    private void OnDisable()
    {
        if (StartButton != null)
        {
            StartButton.clicked -= OnButtonPressed;
        }
    }

    private void OnButtonPressed()
    {
        Debug.Log("The VR Button was successfully poked!");

    }
}
