using System;
using Runes;
using UniRx;
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
    }
}