using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

class KataruChoicesView : MonoBehaviour
{
    [SerializeField] Kataru.Runner runner;

    [SerializeField] RectTransform optionContainer = null;
    [SerializeField] GameObject optionButtonTemplate = null;

    private bool interrupt = false;
    private List<GameObject> currentOptionButtons = new List<GameObject>();

    void Start()
    {
        runner.OnChoices += OnChoices;
    }

    public void OnChoices(Kataru.Choices choices)
    {
        currentOptionButtons.Clear();
        foreach (string choice in choices.choices)
        {

            var newOption = Instantiate(optionButtonTemplate);
            newOption.SetActive(true);
            newOption.transform.SetParent(optionContainer, false);

            var button = newOption.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OnChoice(choice));

            var text = newOption.GetComponentInChildren<Text>();
            text.text = choice;

            text.color = Color.white;

            currentOptionButtons.Add(newOption);
        }
    }

    public void OnChoice(string choice)
    {
        Debug.Log("OptionButtonSelected '" + choice + "'");
        foreach (var button in currentOptionButtons)
        {
            Destroy(button);
        }

        currentOptionButtons.Clear();
        runner.Next(choice);
    }

    void OnDisable()
    {
        runner.OnChoices -= OnChoices;
    }
}