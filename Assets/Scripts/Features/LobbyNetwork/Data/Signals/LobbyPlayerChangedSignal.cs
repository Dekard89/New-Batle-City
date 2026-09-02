using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerChangedSignal
{
    public IReadOnlyList<PlayerModel> Players { get; }

    public LobbyPlayerChangedSignal(IReadOnlyList<PlayerModel> players)=> Players = players;
    
}
