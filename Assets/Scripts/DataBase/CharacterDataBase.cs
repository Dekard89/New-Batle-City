using Assets.Scripts.Features.LobbyNetwork.Data.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PlayerDataBase", menuName = "Scriptable Objects/PlayerDataBase")]
public class CharacterDataBase : ScriptableObject
{
    

    public List<CharacterData> Characters;

    public CharacterData GetById (string Id)=> Characters.Find(x=>x.Id == Id);

    
}
