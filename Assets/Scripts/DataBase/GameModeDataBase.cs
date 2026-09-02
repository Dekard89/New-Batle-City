using Assets.Scripts.Public.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Public.DataBase
{
    [CreateAssetMenu(fileName = "GameModeDataBase", menuName = "Scriptable Objects/GameModeDataBase")]
    public class GameModeDataBase : ScriptableObject
    {
        
        public List<GameMode> GameModes;

        public GameMode GetById(int id)=> GameModes.Find(x=> x.Id == id);

        public GameMode GetByName(string name) => GameModes.Find(x=> x.Name == name);

        public List<GameMode> GetAll()=> GameModes;
    }
}
