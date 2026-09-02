using Assets.Scripts.Abstraction;
using Assets.Scripts.Public.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace Assets.Scripts.Public.Infrasructure
{
    public class TeamBalancer : IBalancer
    {
        public Dictionary<ulong, int> BalanceTeam(IReadOnlyList<NetworkClient> actionClients, GameMode gameMode)
        {
            var assigments = new Dictionary<ulong, int>();

            if(gameMode.MatchScheme == MatchScheme.AnyVsAny)
            {
                int currentId = 1;
                foreach(NetworkClient client in actionClients)
                {
                    assigments.Add(client.ClientId, currentId);
                    currentId++;
                }
            }
            else if (gameMode.MatchScheme == MatchScheme.TeamVsTeam)
            {
                Dictionary<int, int> teamSezes = new();
                for(int i =0; i < gameMode.TeamCount; i++)
                {
                    teamSezes.Add(i + 1, 0);
                }
                foreach(NetworkClient client in actionClients)
                {
                    var targetTeam = GetSmallestTeam(teamSezes);

                    assigments.Add(client.ClientId, targetTeam);

                    teamSezes[targetTeam]++;
                }
            }

            return assigments;
        }
        private int GetSmallestTeam(Dictionary<int,int> teamSizes)
        {
            int smallestTeam = -1;
            int minSize = int.MaxValue;

            foreach(var team in teamSizes)
            {
                if(team.Value< minSize)
                {
                    minSize=team.Value;
                    smallestTeam = team.Key;
                }
            }
            return smallestTeam;
        }
    }
}
