using Assets.Scripts.Abstraction;
using Assets.Scripts.Features.LobbyNetwork.Data.Model;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorService : ISelectorService
{
    private readonly CharacterDataBase _db;

    public CharacterSelectorService(CharacterDataBase dataBase)
    {
            _db = dataBase;
    }

    public string SelectedCharacter { get; set; } = "";

    public IEnumerable<CharacterData> GetSelectedAllCharacters() => _db.Characters;
   

    public CharacterData GetSelectedCharacter() => _db.GetById(SelectedCharacter);


    
}
