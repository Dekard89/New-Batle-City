using Assets.Scripts.Public.Infrasructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services
{
    public class CharactersHandlerRegistratonService : IInitializable
    {
        private readonly DiContainer _container;

        private readonly CharacterDataBase _characterData;

        private readonly IPrefabLoad _prefabLoader;

        public CharactersHandlerRegistratonService( DiContainer diContainer,
            CharacterDataBase characterDataBase,
            IPrefabLoad prefabLoad)
        {
            _container = diContainer;
            _characterData = characterDataBase;
            _prefabLoader = prefabLoad;
        }

        public async void Initialize()
        {
            foreach (var character in _characterData.Characters)
            {
                GameObject prefab = await _prefabLoader.GetPrefab(character.referenceGameObject);

                var handler = new CharacterInstanceHandler(_container, character.referenceGameObject, _prefabLoader);

                NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, handler);

                Debug.Log($"[REGISTER] обработчик {prefab} зарегистрирован");
            }
        }
    }
}
