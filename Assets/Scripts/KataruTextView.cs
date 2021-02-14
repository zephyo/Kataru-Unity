using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

class KataruTextView : MonoBehaviour
{
    [SerializeField] Kataru.Runner runner;
    [SerializeField] Text text = null;

    void OnValidate()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        runner.OnDialogue += OnDialogue;
    }

    public void OnDialogue(Kataru.Dialogue dialogue)
    {
        if (dialogue.name == "Narrator")
        {
            text.text = dialogue.text;
            text.fontStyle = FontStyle.Italic;
        }
        else
        {
            text.text = String.Format("{0}: {1}", dialogue.name, dialogue.text);
            text.fontStyle = FontStyle.Normal;
        }
    }
}