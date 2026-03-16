using System;
using Effects;
using Spells;
using UniRx;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Services
{
    public class CastService : IInitializable, IDisposable
    {
        private readonly EffectsSettings _settings;
        private readonly VFXDrawerService _vfxDrawerService;
        private readonly SpellService _spellService;
        private readonly LifetimeScope _scope;

        private readonly CompositeDisposable _disposables = new();
        
        public CastService(EffectsSettings settings, VFXDrawerService vfxDrawerService, 
            SpellService spellService, LifetimeScope scope)
        {
            _settings = settings;
            _vfxDrawerService = vfxDrawerService;
            _spellService = spellService;
            _scope = scope;
        }

        public void Initialize()
        {
            _spellService.OnCast.Subscribe(OnCast).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCast(SpellBuilder builder)
        {
            if (_settings.UseVFX) _vfxDrawerService.ClearRequests();

            var spell = builder.GetSpell();
            if (!spell) return;
            
            var effect = Object.Instantiate(spell.PrefabEffect); // TODO position
            Object.Destroy(effect, spell.EffectLifetime);
            _scope.Container.InjectGameObject(effect);
        }
    }
}