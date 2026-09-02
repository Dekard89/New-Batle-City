using Assets.Scripts.Features.LobbyNetwork.Data.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Abstraction
{
    public interface ISelectorService
    {
        string SelectedCharacter {  get; set; }

        CharacterData GetSelectedCharacter();

        IEnumerable<CharacterData> GetSelectedAllCharacters();
    }
}
