using Cysharp.Threading.Tasks;
using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class CameraTargetService : ICameraTarget
{
    private readonly CinemachineCamera _camera;

    private bool _disposed;

    [Inject]
    public CameraTargetService(CinemachineCamera camera)
    {
          _camera=camera;
    }
    public async UniTask ResetCamera()
    {
      if(_disposed ||  _camera == null) return;

        try
        {
            _camera.Follow = null;
            _camera.LookAt = null;

            await UniTask.WaitForEndOfFrame();
        }
        catch(Exception e)
        {
            Debug.LogWarning($"Camera terget setyp failed {e.Message}");
        }
    }

    public async UniTask SetCameraTargetAsync(Transform target)
    {
        if (_disposed || _camera == null || target == null) return;


        try
        {
            await UniTask.WaitUntil(() => _camera!=null && _camera.isActiveAndEnabled );

            _camera.Follow = target;
            _camera.LookAt = target;
            _camera.enabled = true;

            await UniTask.WaitForEndOfFrame();
        }catch(Exception e)
        {
            Debug.LogError($"Error setting camera target { e.Message}");
        }
    }

    public async UniTask SetCameraTargetAsync(NetworkObject networkObject)
    {
        if(networkObject== null) return;

        await UniTask.WaitUntil(() => networkObject.IsSpawned|| _disposed);

        if (_disposed) return;

        await SetCameraTargetAsync(networkObject.transform);
    }
}
