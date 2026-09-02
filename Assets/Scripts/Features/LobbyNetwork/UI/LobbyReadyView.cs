using Assets.Scripts.Features.LobbyNetwork.Data.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyReadyView : MonoBehaviour
{
    
        [Header("Character")]
        [SerializeField] private Transform characterContainer;
        [SerializeField] private CharacterCard characterCardPrefab;

        [Header("Players")]
        [SerializeField] private Transform playersContainer;
        [SerializeField] private LobbyPlayerRow playerCardPrefab;

        [Header("Controls")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button readyButton;

        
        public event Action OnReadyClicked;
        public event Action OnStartClicked;
        public event Action<string> OnCharacterSelected;

        private readonly List<CharacterCard> _spawnedCards = new();

        private void OnEnable()
        {
            readyButton.onClick.AddListener(() => OnReadyClicked?.Invoke());
            startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
        }

        private void OnDisable()
        {
            readyButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();
        }

        
        public void SetStartButtonVisibility(bool isVisible)
        {
            startButton.gameObject.SetActive(isVisible);
        }

        public void SetStartButtonInteractable(bool isInteractable)
        {
            startButton.interactable = isInteractable;
        }

        
        public void GenerateCharacterMenu(IEnumerable<CharacterData> characters, string selectedId)
        {
            
            foreach (Transform child in characterContainer)
            {
                if (child.gameObject == characterCardPrefab.gameObject)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }
                Destroy(child.gameObject);
            }
            _spawnedCards.Clear();

           
            foreach (var character in characters)
            {
                CharacterCard newCard = Instantiate(characterCardPrefab, characterContainer);
                newCard.Setup(character.Id, character.Name, character.Avatar, HandleCardSelected);
                _spawnedCards.Add(newCard);
            }

            UpdateCardVisualSelection(selectedId);
        }

        private void HandleCardSelected(string characterId)
        {
            OnCharacterSelected?.Invoke(characterId);
        }

        public void UpdateCardVisualSelection(string selectedId)
        {
            foreach (var card in _spawnedCards)
            {
                card.SetSelection(card.GetId() == selectedId);
            }
        }

        public void ClearPlayersList()
        {
            foreach (Transform child in playersContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void PopulatePlayersList(IReadOnlyList<PlayerModel> players)
        {
            foreach (var player in players)
            {
                var row = Instantiate(playerCardPrefab, playersContainer);
                row.Setup(player);
            }
        }
    }




