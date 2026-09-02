using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public interface ICameraTarget
{
    UniTask SetCameraTargetAsync(Transform target);

    UniTask SetCameraTargetAsync(NetworkObject networkObject);

    UniTask ResetCamera();

}
