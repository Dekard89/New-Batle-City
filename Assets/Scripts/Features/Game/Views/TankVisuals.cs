using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class TankVisuals : MonoBehaviour
{
    
    private MovementController _movementController;

    private Transform _towerTransform;

    private Animator[] _animators;

    private ParticleSystem _muzzleFlash;

    
    private SpriteRenderer[] _renderers;

    private Color[] _originalColors;

    private bool _isDead=false;
    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _towerTransform = GetComponentInChildren<TowerMarker>().GetComponent<Transform>();
        _animators = GetComponentsInChildren<Animator>();
        _muzzleFlash=_towerTransform.GetComponentInChildren<ParticleSystem>();
    }
    private void Start()
    {
        SetupRendersColor();
    }

    private void OnEnable()
    {
        _movementController.TowerAngle.OnValueChanged += SyncTowerRotate;
        _movementController.SyncSpeed.OnValueChanged += SyncAnimators;
    }

    private void SyncAnimators(float previousValue, float newValue)
    {
        foreach (var animator in _animators)
        {
            animator.SetFloat("Speed",Mathf.Abs( newValue));
        }
    }

    public void SyncTowerRotate(float oldAngle, float newAngle)
    {
        
        _towerTransform.localRotation = Quaternion.Euler(0, 0, newAngle);
            
    }
    public void PlayShootEffect()
    {
        _muzzleFlash.Play(withChildren:true);
    }
    private void OnDisable()
    {
        _movementController.SyncSpeed.OnValueChanged -= SyncAnimators;
        _movementController.TowerAngle.OnValueChanged -= SyncTowerRotate;
    }
    private void SetupRendersColor()
    {
        _renderers=GetComponentsInChildren<SpriteRenderer>();
        _originalColors = new Color[_renderers.Length];
        for(int i=0; i< _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].color;
        }
    }
    public async UniTaskVoid FlashRoutine()
    {
        SetColor(Color.red);

        await UniTask.Delay(100);

        ResetColor();
    }
    private void SetColor(Color color)
    {
        foreach(var renderer in _renderers) renderer.color= color;
    }
    private void ResetColor()
    {
        if (_isDead) return;
        for (int i=0; i < _renderers.Length; i++)
            _renderers[i].color= _originalColors[i];
    }
   
    public async UniTask SyncDeath()
    {
        _isDead = true;
        SetColor(Color.black);
        await UniTask.Delay(100);
    }
    private void OnDestroy()
    {
        _renderers= null;
    }
}
