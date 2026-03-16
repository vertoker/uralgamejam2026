using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Recognition;
using Spells;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class SpellService : IInitializable, IDisposable
    {
        private readonly SpellsSettings _settings;
        private readonly SymbolService _symbolService;
        private readonly RunesProvider _runesProvider;
        private readonly SpellBuilder _spellBuilder;

        private CancellationTokenSource _tokenSource = new();
        private readonly CompositeDisposable _disposables = new();
        
        private readonly Subject<SpellBuilder> _onCancel = new();
        private readonly Subject<SpellBuilder> _onCast = new();
        
        public IObservable<SpellBuilder> OnCancel => _onCancel;
        public IObservable<SpellBuilder> OnCast => _onCast;
        
        public SpellService(SpellsSettings settings, SymbolService symbolService, RunesProvider runesProvider)
        {
            _settings = settings;
            _symbolService = symbolService;
            _runesProvider = runesProvider;

            var spellContainer = new SpellContainer();
            spellContainer.Initialize(settings);
            _spellBuilder = new SpellBuilder(spellContainer);
        }
        public void Initialize()
        {
            _symbolService.OnStartDraw.Subscribe(OnStartDraw).AddTo(_disposables);
            _symbolService.OnDraw.Subscribe(OnDraw).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnStartDraw(Unit unit)
        {
            Stop();
        }
        private void OnDraw(SymbolFeaturesScriptable symbol)
        {
            if (!symbol)
            {
                StartCancel();
                return;
            }
            
            var runeGroup = _runesProvider.RuneGroup.Value;
            var step = new SpellStep(symbol, runeGroup);

            Debug.Log($"{BgnClrGrn}Step{EndClr}, " +
                      $"symbol: {BgnClrGrn}{step.Symbol.name}{EndClr}, " +
                      $"runes: ({BgnClrGrn}{step.RuneGroup}{EndClr})");
            
            if (_spellBuilder.Active)
                _spellBuilder.Next(step);
            else _spellBuilder.Start(step);
            
            StartCast();
        }
        
        private void StartCancel()
        {
            Stop();
            CancelAsync(_tokenSource.Token).Forget();
        }
        private void StartCast()
        {
            Stop();
            CastAsync(_tokenSource.Token).Forget();
        }
        private void Stop()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private async UniTask CancelAsync(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_settings.ActivateTime);
            if (token.IsCancellationRequested) return;
            
            Debug.Log($"{BgnClrYlw}Cancel{EndClr}, steps count: {_spellBuilder.Steps.Count}");
            
            _onCancel.OnNext(_spellBuilder);
            
            _spellBuilder.Stop();
        }
        private async UniTask CastAsync(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_settings.ActivateTime);
            if (token.IsCancellationRequested) return;
            
            LogSpell(_spellBuilder.GetSpell());
            _onCast.OnNext(_spellBuilder);

            _spellBuilder.Stop();
        }

        private static void LogSpell(SpellScriptable spell)
        {
            var spellName = spell ? $"{BgnBldClrGrn}{spell.name}{EndBldClr}" : $"{BgnBldClrYlw}None{EndBldClr}";
            Debug.Log($"{BgnClrGrn}Cast{EndClr}, spell: {spellName}");
        }

        private const string BgnBld = "<b>";
        private const string EndBld = "</b>";
        private const string BgnClrGrn = "<color=green>";
        private const string BgnClrYlw = "<color=yellow>";
        private const string EndClr = "</color>";
        private const string BgnBldClrGrn = "<color=green><b>";
        private const string BgnBldClrYlw = "<color=yellow><b>";
        private const string EndBldClr = "</b></color>";
    }
}