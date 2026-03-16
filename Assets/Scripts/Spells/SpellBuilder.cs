using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Spells
{
    public class SpellBuilder
    {
        private readonly SpellContainer _container;
        private readonly List<SpellStep> _steps = new();
        private SpellContainer.TreeNode _treeNode;
        
        public IReadOnlyList<SpellStep> Steps => _steps;

        public string GetStringSteps() => string.Join(" ", _steps.Select(s => s.GetName()));
        
        public bool Active => _treeNode != null;

        public SpellBuilder(SpellContainer container)
        {
            _container = container;
        }

        public void Start(SpellStep step)
        {
            var startNode = _container.StartSearch(step);
            if (startNode == null) return;
            _steps.Add(step);
            _treeNode = startNode;
        }
        public void Stop()
        {
            _steps.Clear();
            _treeNode = null;
        }
        
        [CanBeNull] public SpellScriptable GetSpell() => _treeNode?.Spell;
        public bool Next(SpellStep step)
        {
            var result = _treeNode.NextSteps.TryGetValue(step, out _treeNode);
            _steps.Add(step);
            return result;
        }
    }
}