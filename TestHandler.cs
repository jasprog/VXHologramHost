using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestHandler : MonoBehaviour
{

    public NPC TestNPC;
    public InputField inputField;
    public InputField keywordField;
    public InputField resposneField;
    private EntityResponses _responses;
    private string input;

    void Start()
    {
        //Get responses from json
        string json = File.ReadAllText(Application.dataPath + "/ResponseDataFile.json");
        _responses = JsonUtility.FromJson<EntityResponses>(json);
    }
    public void ReadStringInput()
    {
        //Get Given input text, then find corrosponding category and display returns values.
        var e = TestNPC.GetResponseFromEntity(inputField.text, _responses);
        keywordField.text = e.CategoryKey;
        resposneField.text = e.TextResponse;

        Debug.Log(e.TextResponse);
        Debug.Log(e.CategoryKey);
        Debug.Log(e.AnimationTrigger);
    }
    public void SaveToJson()
    {
        //Take given text from fields and save to json as new category.
        EntityResponse data = new EntityResponse();
        data.CategoryKey = keywordField.text;
        data.TextResponse = resposneField.text;
        data.AnimationTrigger = "IsTalking";
        Array.Resize(ref _responses.responses, _responses.responses.Length + 1);
        _responses.responses[_responses.responses.Length - 1] = data;

        string json = JsonUtility.ToJson(_responses, true);
        File.WriteAllText(Application.dataPath + "/ResponseDataFile.json", json);
    }

}
