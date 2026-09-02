using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Features.LobbyNetwork.Data.Signals
{
    public class ShowLoadingSignal
    {
        public string Message { get;}

        public ShowLoadingSignal(string message) => Message = message;
    }
}
