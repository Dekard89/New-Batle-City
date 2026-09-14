using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour,IPointerClickHandler
{
    [Header("Ui Elemenst")]
    [SerializeField] private TMP_Text serverNameText;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private TMP_Text playerCountText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
    [SerializeField] private Color selectedColor;

    private LobbyModel _model;
    private Image _bgImage;
    private bool _isSelected;

    private Action<string> _onJoinClickedCallback;
    private Action<ServerListItem> _onSelectedCallback;

    
    public void Setup(LobbyModel model, Action<string> onJoinClickedCallback, Action<ServerListItem> onSelectedCallback)
    {
        _model=model;
        _onJoinClickedCallback = onJoinClickedCallback;
        _onSelectedCallback = onSelectedCallback;
        playerCountText.text = RatioCurrentToMax(_model);
        serverNameText.text = model.ServerName;
        gameModeText.text =model.GamaModeName;
        

    }
    private void Awake()
    {
        _bgImage = GetComponent<Image>();
        ResetSelection();
    }
    public void ResetSelection()
    {
        _isSelected = false;
        if (_bgImage != null)
        {
            _bgImage.color = normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _isSelected = true;
            _bgImage.color = selectedColor;

            _onSelectedCallback?.Invoke(this);
        }
        else
        {
            _onJoinClickedCallback?.Invoke(_model.Id);
        }
    }
    private string RatioCurrentToMax(LobbyModel model) => $"{model.CurrentPlayers} / {model.MaxPlayers}";
}
