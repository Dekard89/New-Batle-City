using System.Net;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyStateManager
{
    private const string PlayerNamePrefsKey = "SavedPlayerNamr";
    public Lobby CurrentLobby {  get; private set; }

    public string LocalPlayerId => AuthenticationService.Instance.PlayerId;

    public string LocalPlayerName
    {
        get
        {
            if(PlayerPrefs.HasKey(PlayerNamePrefsKey))
                return PlayerPrefs.GetString(PlayerNamePrefsKey);

            if(UnityServices.State==ServicesInitializationState.Initialized &&
                AuthenticationService.Instance.IsSignedIn)
            {
                if (!string.IsNullOrWhiteSpace(AuthenticationService.Instance.PlayerName))
                {
                    var unityName = AuthenticationService.Instance.PlayerName;

                    SavePlayerNameToPrefs(unityName);

                    return unityName;
                }
            }
            string defaultName = $"Player # {Random.Range(1,100)}";

            return defaultName;
        }
        
    }

    public void SavePlayerNameToPrefs(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        PlayerPrefs.SetString(PlayerNamePrefsKey, newName);
        PlayerPrefs.Save();
        Debug.Log($"[LobbyStateManager] Имя '{newName}' сохранено через PlayerPrefs.");
    }

    public void SetLobby(Lobby lobby) => CurrentLobby = lobby;

    public void Clear() => CurrentLobby = null;
}
