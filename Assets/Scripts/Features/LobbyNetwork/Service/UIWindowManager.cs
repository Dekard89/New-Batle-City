using Assets.Scripts.Features.LobbyNetwork.UICards;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Zenject;

namespace Assets.Scripts.Features.LobbyNetwork.Service
{
    public class UIWindowManager : IInitializable, IDisposable
    {
        private readonly ConnectionWindow _connectionWindow;

        private readonly ChangeCharacterWindow _changeCharacterWindow;

        private readonly CreateLobbyWindow _createLobbyWindow;

        private readonly OptionsWindow _optionsWindow;

        private readonly SignalBus _signalBus;

        public UIWindowManager(ConnectionWindow connectionWindow, ChangeCharacterWindow changeCharacterWindow,
             CreateLobbyWindow createLobbyWindow,OptionsWindow optionsWindow, SignalBus bus)
        {
            _changeCharacterWindow = changeCharacterWindow;
            _connectionWindow = connectionWindow;
            _createLobbyWindow = createLobbyWindow;
            _optionsWindow = optionsWindow;
            _signalBus = bus;
        }
        public void Initialize()
        {
            OpenConnectionWindow();
            _signalBus.Subscribe<LobbyJoinedSignal>(OpenChangeCharacterWindow);
            
        }
        public void OpenConnectionWindow()
        {
            _connectionWindow.Show();
            _changeCharacterWindow.Hide();
            _createLobbyWindow.Hide();
            _optionsWindow.Hide();
   
        }
        public void OpenChangeCharacterWindow()
        {
            _changeCharacterWindow.Show();
            _connectionWindow.Hide();
            _createLobbyWindow.Hide();
     
        }
        public void OpenCreateLobbyWindow()
        {
            _createLobbyWindow.Show();
            _changeCharacterWindow?.Hide();
            _connectionWindow?.Hide();
        }
        public void OpenOptionsWindow()
        {
            _optionsWindow.Show();
            _connectionWindow.Hide();
        }
      

        public void Dispose()
        {
            _signalBus.Unsubscribe<LobbyJoinedSignal>(OpenChangeCharacterWindow);
        }
    }
}
