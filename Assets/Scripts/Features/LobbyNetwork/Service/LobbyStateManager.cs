using System.Net;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyStateManager
{
    public Lobby CurrentLobby {  get; private set; }

    public string LocalPlayerId => AuthenticationService.Instance.PlayerId;

    public string LocalPlayerName => AuthenticationService.Instance.PlayerName ?? "Player";

    public void SetLobby(Lobby lobby) => CurrentLobby = lobby;

    public void Clear() => CurrentLobby = null;
}
