using System;
using Runes;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class RunesProvider : IInitializable, IDisposable
    {
        private readonly InputProvider _inputProvider;
        
        private readonly ReactiveProperty<RuneGroup> _runeGroup = new();
        public IReadOnlyReactiveProperty<RuneGroup> RuneGroup => _runeGroup;

        private readonly CompositeDisposable _disposables = new();
        
        public RunesProvider(InputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }
        public void Initialize()
        {
            _inputProvider.Modifiers[0].Subscribe(value => ModifyGroup(value, RuneType.Alpha)).AddTo(_disposables);
            _inputProvider.Modifiers[1].Subscribe(value => ModifyGroup(value, RuneType.Beta)).AddTo(_disposables);
            _inputProvider.Modifiers[2].Subscribe(value => ModifyGroup(value, RuneType.Gamma)).AddTo(_disposables);
            _inputProvider.Modifiers[3].Subscribe(value => ModifyGroup(value, RuneType.Delta)).AddTo(_disposables);
            _inputProvider.Modifiers[4].Subscribe(value => ModifyGroup(value, RuneType.Epsilon)).AddTo(_disposables);
            _inputProvider.Modifiers[5].Subscribe(value => ModifyGroup(value, RuneType.Zeta)).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void ModifyGroup(bool value, RuneType runeType)
        {
            var group = value ? _runeGroup.Value.Add(runeType) : _runeGroup.Value.Remove(runeType);
            _runeGroup.SetValueAndForceNotify(group);
        }
        
        public Color GetColor(RuneGroup group)
        {
            const float mult1 = 1f / 3f; // 0.(3)
            const float mult2 = 1f - mult1; // 0.(6)7
                
            var alpha = group.Contains(RuneType.Alpha);
            var beta = group.Contains(RuneType.Beta);
            var gamma = group.Contains(RuneType.Gamma);
            var delta = group.Contains(RuneType.Delta);
            var epsilon = group.Contains(RuneType.Epsilon);
            var zeta = group.Contains(RuneType.Zeta);

            var r = 1f - (alpha ? mult1 : 0f) - (delta ? mult2 : 0f);
            var g = 1f - (beta ? mult1 : 0f) - (epsilon ? mult2 : 0f);
            var b = 1f - (gamma ? mult1 : 0f) - (zeta ? mult2 : 0f);
            
            return new Color(r, g, b);
        }
    }
}