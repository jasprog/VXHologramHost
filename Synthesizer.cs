// Jasper
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;

public class HelloWorld : MonoBehaviour
{
    public static HelloWorld Instance;

    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    public Text outputText;
    public InputField inputField;
    public Button speakButton;
    public AudioSource audioSource;

    // Replace with your own subscription key and service region (e.g., "westus").
    string SubscriptionKey = Key.subscriptionKey;
    string Region = Key.region;
    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    public bool audioSourceNeedStop;
    private string message;
    private string message1;
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    public void SynthesizeSpeech(string text)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        // Limit the string length to avoid overloading the synthesizer.
        int maxLength = 1000; // Define the maximum length for each segment
        var segments = SplitTextIntoChunks(text, maxLength);

        foreach (var segment in segments)
        {
            try
            {
                SynthesizeSegment(segment);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error synthesizing segment: {ex.Message}");
                message = $"Error: {ex.Message}";
            }
        }

        lock (threadLocker)
        {
            waitingForSpeak = false;
        }
    }

    private void SynthesizeSegment(string text)
    {
        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(text).Result)
        {
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 60, // Limit to 60 seconds per segment
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        Debug.Log(newMessage);
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });

            audioSource.clip = audioClip;
            audioSource.Play();
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }
        }
    }

    private IEnumerable<string> SplitTextIntoChunks(string text, int maxLength)
    {
        for (int i = 0; i < text.Length; i += maxLength)
        {
            yield return text.Substring(i, Math.Min(maxLength, text.Length - i));
        }
    }

    void Start()
    {
        Instance = this;

        message1 = "Hello";
        message = "Click button to synthesize speech";

        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };

        if (speakButton != null)
        {
            speakButton.interactable = !waitingForSpeak;
        }
    }

    void Update()
    {
        lock (threadLocker)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }

            if (audioSourceNeedStop)
            {
                audioSource.Stop();
                audioSourceNeedStop = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}
