using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuUI;       // Assign the MainMenuUI in the inspector
    public GameObject textInputHandler; // Assign the TextInputHandler in the inspector
    public Button basicButton;          // Assign the Basic button in the inspector

    void Start()
    {
        textInputHandler.SetActive(false); // Hide TextInputHandler at the start

        basicButton.onClick.AddListener(OnBasicSelected); // Set up listener for Basic button
    }

    void OnBasicSelected()
    {
        mainMenuUI.SetActive(false);        // Hide the main menu
        textInputHandler.SetActive(true);   // Show the text input field
    }
}
