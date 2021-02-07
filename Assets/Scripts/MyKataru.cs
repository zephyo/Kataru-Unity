using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class MyKataru : Kataru
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.dataPath + "/Kataru/story");
        LoadStory(Application.dataPath + "/Kataru/story");
        LoadBookmark(Application.dataPath + "/Kataru/bookmark.yml");
        InitRunner();

        Debug.Log(String.Format("Passage: {0}", GetPassage()));
        
    }

    void Update()
    {
        //Detect when the Return key is pressed down
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Next!");
            Next("");
        }
    }

    protected override void OnText(string text)
    {
        Debug.Log(text);
    }

    protected override void OnDialogue(string speaker, string speech)
    {
        Debug.Log(String.Format("Speaker: {0}", speaker));
        Debug.Log(String.Format("Speech: {0}", speech));
    }

    protected override void OnCommand(string command, IDictionary<string, object> parameters) {
        Debug.Log(String.Format("Command: {0}", command));
        foreach (var parameter in parameters) {
            Debug.Log(parameter);
        }
    }

    protected override void OnChoices(ISet<string> choices, double timeout) { 
        foreach (string choice in choices) {
            Debug.Log(choice);
        }
    }
}