using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.UI
{
    public class ConnectedView : MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button refreshLobbyListButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private TMP_Text statusText;

        [Header("Server List UI")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject serverListItemPrefab;

        
        private ServerListItem _activeSelectedRow;

        private readonly List<GameObject> spawnedItems = new();

        public Action<string> OnServerSelectedEvent;
        public Action OnCreateLobbyEvent;
        public Action OnRefreshLobbyListEvent;
        public Action OnOptionsEvent;
        private void OnEnable()
        {
            createLobbyButton.onClick.AddListener(()=> OnCreateLobbyEvent?.Invoke());
            refreshLobbyListButton.onClick.AddListener(() => OnRefreshLobbyListEvent?.Invoke());
            optionsButton.onClick.AddListener(() => OnOptionsEvent?.Invoke());
        }
        private void OnDisable()
        {
            createLobbyButton.onClick.RemoveAllListeners();
            refreshLobbyListButton.onClick.RemoveAllListeners();
            optionsButton.onClick.RemoveAllListeners();
        }


        public void PopulatrServerList(IReadOnlyList<LobbyModel> lobbies)
        {
            ClearServerList();

            foreach(var lobby in lobbies)
            {
                GameObject go = Instantiate(serverListItemPrefab, listContainer);

                if (go.TryGetComponent<ServerListItem>(out var item))
                {
                    item.Setup(lobby, HandleServerSelected, HandleRowHighlight);
                }
            }
        }
       public void SetStatusText( string text)
        {
            statusText.text= text;
        }
        
        private async void HandleServerSelected(string selectedLoddyId)
        {
            OnServerSelectedEvent?.Invoke(selectedLoddyId);
        }
        private void HandleRowHighlight(ServerListItem serverListItem)
        {
            if(_activeSelectedRow!= null && _activeSelectedRow!= serverListItem)
            {
                serverListItem.ResetSelection();
            }
            _activeSelectedRow = serverListItem;
        }
        public void ClearServerList()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
            spawnedItems.Clear();
        }
        public void SetControlsInteractable(bool state)
        {
            createLobbyButton.interactable=state;
            refreshLobbyListButton.interactable=state;
        }

    }
}
