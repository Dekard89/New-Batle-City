using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    [SerializeField] private Button enteredNameButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text placeholderText;
    [SerializeField] private TMP_InputField inputPlayerName;

    private string _inputName;
    public Action<string> InputEvent;
    public Action ReturnEvent;
    private void OnEnable()
    {
        returnButton.onClick.AddListener(OnReturnClicked);
        enteredNameButton.onClick.AddListener(OnEnteredClicked);
    }

    private void OnEnteredClicked()
    {
        _inputName = inputPlayerName.text;
        if (String.IsNullOrWhiteSpace(_inputName)) return;

        InputEvent?.Invoke(_inputName);
    }

    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
    }

    private void OnReturnClicked()
    {
        ReturnEvent?.Invoke();
    }

    public void SetPlaceholder(string placeholder)
    {
        placeholderText.text = placeholder;
    }
}
