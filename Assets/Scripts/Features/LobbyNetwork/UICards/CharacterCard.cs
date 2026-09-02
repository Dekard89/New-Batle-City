using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] TMP_Text _name;

    [SerializeField] Image _avatar;

    [SerializeField] Button _selectionButton;

    [SerializeField] Image _selectionHightLight;


    private string _characterId;

    private Action<string> onCardSelection;

    private void HandleClick()
    {
        onCardSelection?.Invoke(_characterId);
    }
    public void SetSelection(bool isSelected)
    {
        if(_selectionHightLight != null) 
            _selectionHightLight.enabled = isSelected;
    }
    public void Setup(string id, string name, Sprite icon, Action<string> onSeclectCallBack)
    {
        _characterId = id;
        _name.text= name;
        _avatar.sprite= icon;
        onCardSelection = onSeclectCallBack;

        _selectionButton.onClick.RemoveAllListeners();
        _selectionButton.onClick.AddListener(HandleClick);

        SetSelection(false);
    }
    public string GetId()=> _characterId;
}
