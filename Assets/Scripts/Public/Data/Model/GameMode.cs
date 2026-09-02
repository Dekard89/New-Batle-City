using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Public.Data.Model
{
    [Serializable]
    public class GameMode
    {
        public int Id;

        public string Name;

        public int MaxPlayer;

        public int TeamCount;

        public MatchScheme MatchScheme;
    }
}
