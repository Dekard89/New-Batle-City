using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Features.LobbyNetwork.Data.Signals
{
    public class UpdateLoadingProgressSignal
    {
        public float Progress { get; }

        public UpdateLoadingProgressSignal(float progress) => Progress = progress;
    }
}
