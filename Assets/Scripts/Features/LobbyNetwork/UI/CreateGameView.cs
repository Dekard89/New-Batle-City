using Assets.Scripts.Public.DataBase;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CreateGameView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private Button finalizeCreateButton;
    [SerializeField] private TMP_Text statusTXT;
    [SerializeField] private TMP_InputField _inputLobbyName;

    private int _choosedIndex;
    private string _enteredName;

    public Action<int, string> OnFinalizezeEvent;

    private void Start()
    {
        finalizeCreateButton.onClick.AddListener(OnFinalizeCreateClicked);
    }
    public void GenerateGameModeDropDpwn(IReadOnlyList<string> gameModeNames)
    {
        modeDropdown.ClearOptions();
        var options = new List<string>();
        foreach (var mode in gameModeNames)
        {
            options.Add(mode);
        }
        modeDropdown.AddOptions(options);
    }
  

    private async void OnFinalizeCreateClicked()
    {
        finalizeCreateButton.interactable=false;
        statusTXT.text = "Создание комнаты...";

        _choosedIndex = modeDropdown.value;
        _enteredName = _inputLobbyName.text;

        if (String.IsNullOrWhiteSpace(_enteredName)) return;
       
        OnFinalizezeEvent?.Invoke(_choosedIndex,_enteredName);
    }
    private void OnDestroy()
    {
        finalizeCreateButton.onClick.RemoveAllListeners();
    }
}
