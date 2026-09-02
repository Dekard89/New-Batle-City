using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Features.LobbyNetwork.Data.Model
{
    [Serializable]
    public class CharacterData
    {
        public string Id;

        public string Name;

        public AssetReferenceGameObject referenceGameObject;

        public Sprite Avatar;
    }
}
