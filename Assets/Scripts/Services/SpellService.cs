using System;
using Recognition;
using Spells;
using UniRx;
using VContainer.Unity;

namespace Services
{
    public class SpellService : IInitializable, IDisposable
    {
        private readonly SpellsSettings _settings;
        private readonly SymbolService _symbolService;
        private readonly RunesProvider _runesProvider;
        
        private readonly SpellContainer _spellContainer;
        private readonly SpellCounter _spellCounter;

        private readonly CompositeDisposable _disposables = new();
        
        public SpellService(SpellsSettings settings, SymbolService symbolService, RunesProvider runesProvider)
        {
            _settings = settings;
            _symbolService = symbolService;
            _runesProvider = runesProvider;

            _spellContainer = new SpellContainer(_settings);
            _spellCounter = new SpellCounter(_spellContainer);
        }

        public void Initialize()
        {
            _symbolService.OnSymbolDraw.Subscribe(OnSymbolDraw).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        private void OnSymbolDraw(SymbolFeaturesScriptable symbol)
        {
            if (!symbol) return;
            
            var runeGroup = _runesProvider.RuneGroup.Value;
            var step = new SpellStep(symbol, runeGroup);
            var spell = _spellCounter.Step(step);
            // TODO спаунить spell
        }
    }
}