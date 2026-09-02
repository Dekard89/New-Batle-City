using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text characterText;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private GameObject notReadyIcon;

    public void Setup (PlayerModel playerModel)
    {
        nameText.text = playerModel.Name;
        characterText.text = playerModel.SelectCharacter;
        readyIcon.SetActive(playerModel.IsReady);
        notReadyIcon.SetActive(!playerModel.IsReady);
            
    }
}
