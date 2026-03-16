using System;
using Effects;
using Spells;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using World;
using Object = UnityEngine.Object;

namespace Services
{
    public class CastService : IInitializable, IDisposable
    {
        private readonly EffectsSettings _settings;
        private readonly VFXDrawerService _vfxDrawerService;
        private readonly GameModesService _gameModesService;
        private readonly Player _player;
        private readonly SpellService _spellService;
        private readonly LifetimeScope _scope;
        
        private readonly GameObject _spawnMarker;
        private Vector3 _spawnPosition;

        private readonly CompositeDisposable _disposables = new();
        
        public CastService(EffectsSettings settings,
            VFXDrawerService vfxDrawerService, GameModesService gameModesService,
            Player player, SpellService spellService, LifetimeScope scope)
        {
            _settings = settings;
            _vfxDrawerService = vfxDrawerService;
            _gameModesService = gameModesService;
            _player = player;
            _spellService = spellService;
            _scope = scope;
            
            _spawnMarker = Object.Instantiate(_settings.SpawnMarkerPrefab);
            _spawnMarker.SetActive(false);
        }

        public void Initialize()
        {
            _spellService.OnCast.Subscribe(OnCast).AddTo(_disposables);
            _gameModesService.IsMagicMode.Subscribe(OnMagicModeChanged).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnMagicModeChanged(bool value)
        {
            if (value)
            {
                _spawnPosition = GetSpawnPosition();
                _spawnMarker.transform.position = _spawnPosition;
                _spawnMarker.SetActive(true);
            }
            else
            {
                _spawnMarker.SetActive(false);
            }
        }
        private void OnCast(SpellBuilder builder)
        {
            if (_settings.UseVFX) _vfxDrawerService.ClearRequests();

            var spell = builder.GetSpell();
            if (!spell) return;
            
            var effect = Object.Instantiate(spell.PrefabEffect, _spawnPosition, Quaternion.identity);
            Object.Destroy(effect, spell.EffectLifetime);
            _scope.Container.InjectGameObject(effect);
        }

        private Vector3 GetSpawnPosition()
        {
            var ray = new Ray(_player.CameraTransform.position, _player.CameraTransform.forward);
            return Physics.Raycast(ray, out var hit, _settings.SpawnEffectDistance, _settings.LayerMask) 
                ? hit.point : ray.GetPoint(_settings.SpawnEffectDistance);
        }
    }
}