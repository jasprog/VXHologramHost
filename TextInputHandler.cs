using System;
using UnityEngine;
using UnityEngine.UI;

public class TextInputHandler : MonoBehaviour
{
    public static TextInputHandler Instance;

    public Text outputText;
    public InputField inputField;
    public Button submitButton;

    private object threadLocker = new object();
    private bool waitingForInput;
    private string message;
    private string userMessage;

    void Start()
    {
        Instance = this;

        message = "Type something and click the button!";
        if (outputText != null)
        {
            outputText.text = message;
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(ProcessInput);
        }
    }

    void ProcessInput()
    {
        lock (threadLocker)
        {
            waitingForInput = true;
        }

        if (inputField != null)
        {
            userMessage = inputField.text;
            message = $"You said: {userMessage}";
        }

        lock (threadLocker)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
            waitingForInput = false;
        }
    }

    void Update()
    {
        lock (threadLocker)
        {
            if (!waitingForInput && outputText != null)
            {
                outputText.text = message;
            }
        }
    }
}
