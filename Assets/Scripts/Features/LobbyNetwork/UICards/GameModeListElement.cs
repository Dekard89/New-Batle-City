using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameModeListElement : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TMP_Text _gameModeName;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
    [SerializeField] private Color selectedColor;

    private int _gameModeId;
    private Image _bgImage;
    private bool _selected;

    private Action<GameModeListElement> _gameModeAction;
    private Action<int> _selectedElementAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_selected)
        {
            _selected = true;
            _bgImage.color = selectedColor;
            _gameModeAction?.Invoke(this);
        }
        else
        {
            _selectedElementAction?.Invoke(_gameModeId);
        }
    }

    public void Setup(string gameModeName, Action<int> selectAction, Action<GameModeListElement> gameModeAction)
    {
        _gameModeName.text = gameModeName;
        _gameModeAction=gameModeAction;
        _selectedElementAction = selectAction;
    }
    private void Awake()
    {
        _bgImage = GetComponent<Image>();
        ResetSelection();
    }
    private void ResetSelection()
    {
        _selected = false;
        if (_bgImage != null)
        {
            _bgImage.color = normalColor;
        }
    }
}
