using UnityEngine;

public record LobbyModel(
    string Id,
    string ServerName,
    string GamaModeName,
    int CurrentPlayers,
    int MaxPlayers
    );

