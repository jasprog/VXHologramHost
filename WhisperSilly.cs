/*using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.IO;
using System.Threading.Tasks;

public class Whisper1 : MonoBehaviour
{
    public Text outputText;
    public Button startRecoButton;
    public Image progressBar;

    private object threadLocker = new object();
    private bool micPermissionGranted = false;
    private bool keywordDetected = false;
    private readonly string keyword = "Hi Vivi"; // The keyword to listen for
    private readonly string fixedCommand = "tell me about the lab"; // Fixed command to send
    private string modelFilePath;
    private SpeechConfig speechConfig;
    private bool isListening = false;

    private void Start()
    {
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else if (startRecoButton == null)
        {
            UnityEngine.Debug.LogError("startRecoButton property is null! Assign a UI Button to it.");
        }
        else
        {
            micPermissionGranted = true;
            outputText.text = "Speak into your microphone.";
            startRecoButton.onClick.AddListener(StartListening);
        }

        // Handle the model file path
        string modelFile = "669dc1cf-9942-41f9-baa5-273e674e71ce.table";
        string persistentFilePath = Path.Combine(Application.persistentDataPath, modelFile);

        if (!File.Exists(persistentFilePath))
        {
            Debug.Log("Model file not in Persistent path");
            string streamingAssetPath = Path.Combine(Application.streamingAssetsPath, modelFile);

            // Load from StreamingAssets and copy to persistent data path
            File.Copy(streamingAssetPath, persistentFilePath);
            Debug.Log("Model file copied to Persistent path");
        }

        modelFilePath = persistentFilePath;

        // Initialize speech configuration
        speechConfig = SpeechConfig.FromSubscription(Key.subscriptionKey, Key.region);
    }

    private async void StartListening()
    {
        if (isListening) return;

        isListening = true;
        Debug.Log("Speak into your microphone.");

        await ContinuousRecognitionWithKeywordSpottingAsync().ConfigureAwait(false);
    }

    public async Task ContinuousRecognitionWithKeywordSpottingAsync()
    {
        var model = KeywordRecognitionModel.FromFile(modelFilePath);
        using (var recognizer = new SpeechRecognizer(speechConfig))
        {
            // Subscribe to events.
            recognizer.Recognized += (s, e) =>
            {
                var result = e.Result;
                if (result.Reason == ResultReason.RecognizedKeyword)
                {
                    Debug.Log($"RECOGNIZED KEYWORD: Text={e.Result.Text}");
                    Debug.Log("Keyword recognized, sending fixed command...");
                    SendCommand(fixedCommand);
                    isListening = false; // Stop listening after keyword is recognized
                }
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Debug.Log("Session stopped event.");
            };

            // Starts continuous recognition using the keyword model.
            await recognizer.StartKeywordRecognitionAsync(model).ConfigureAwait(false);

            // Continue listening indefinitely until the keyword is detected
            while (isListening)
            {
                await Task.Delay(100); // Small delay to prevent tight loop
            }

            // Stops recognition.
            await recognizer.StopKeywordRecognitionAsync().ConfigureAwait(false);
        }
    }

    private void SendCommand(string command)
    {
        // Assuming there's a manager that processes commands
        ManagerTestScript.instance.onSendCommand(command);
        outputText.text = "Sent command: " + command;
    }

    private void OnDestroy()
    {
        isListening = false; // Ensure listening stops if the object is destroyed
    }
}
*/