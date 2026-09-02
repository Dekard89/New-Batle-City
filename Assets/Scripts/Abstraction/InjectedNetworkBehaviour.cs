using Unity.Netcode;
using UnityEngine;
using Zenject;

public class InjectedNetworkBehaviour : NetworkBehaviour
{
    private bool _injected=false;

    private void EnsureInjected()
    {

        if (_injected) return;
        SceneContext context = FindFirstObjectByType<SceneContext>();
        if (context != null) 
            context.Container.InjectGameObject(this.gameObject);

        else if(ProjectContext.HasInstance)
            ProjectContext.Instance.Container.Inject(this);

        _injected = true;
    }
    protected virtual void Awake()
    {
        EnsureInjected();
    }
}
