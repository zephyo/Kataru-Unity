
using UnityEngine;
using System;

public class KataruManager : MonoBehaviour
{
    [SerializeField] Kataru.Runner runner;

    void Start()
    {
        runner.Init();

        runner.OnChoices -= OnChoices;
        runner.OnDialogue += OnDialogue;
        runner.OnCommand += OnCommand;
        runner.OnInputCommand += OnInputCommand;
    }

    void OnDisable()
    {
        runner.OnChoices -= OnChoices;
        runner.OnDialogue -= OnDialogue;
        runner.OnCommand -= OnCommand;
        runner.OnInputCommand -= OnInputCommand;
    }

    void Update()
    {
        //Detect when the Return key is pressed down
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Next!");
            runner.Next("");
        }
    }

    void OnDialogue(Kataru.Dialogue dialogue)
    {
        Debug.Log(String.Format("Speaker: {0}", dialogue.name));
        Debug.Log(String.Format("Speech: {0}", dialogue.text));
    }

    void OnCommand(Kataru.Command command)
    {
        Debug.Log(String.Format("Command: {0}", command.name));
        foreach (var parameter in command.parameters)
        {
            Debug.Log(parameter);
        }
    }

    void OnChoices(Kataru.Choices choices)
    {
        foreach (string choice in choices.choices)
        {
            Debug.Log(choice);
        }
    }

    void OnInputCommand(Kataru.InputCommand inputCommand)
    {
        Debug.Log(inputCommand.prompt);
    }
}