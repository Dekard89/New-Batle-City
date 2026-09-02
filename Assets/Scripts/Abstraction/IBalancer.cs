using Assets.Scripts.Public.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace Assets.Scripts.Abstraction
{
    public interface IBalancer
    {
        Dictionary<ulong, int> BalanceTeam(IReadOnlyList<NetworkClient> actionClients, GameMode gameMode);
    }
}
