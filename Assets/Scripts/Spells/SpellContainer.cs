using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class SpellContainer
    {
        private readonly Dictionary<SpellStep, TreeNode> _startNodes = new();

        public void Initialize(SpellsSettings settings)
        {
            foreach (var spell in settings.Spells)
            {
                var startStep = spell.Steps[0];
                if (!_startNodes.TryGetValue(startStep, out var currentNode))
                {
                    currentNode = new TreeNode(startStep);
                    _startNodes.Add(startStep, currentNode);
                }

                for (var i = 1; i < spell.Steps.Count; i++)
                {
                    var nextStep = spell.Steps[i];
                    if (!currentNode.NextSteps.TryGetValue(nextStep, out var nextNode))
                    {
                        nextNode = new TreeNode(nextStep);
                        currentNode.NextSteps.Add(nextStep, nextNode);
                    }
                    currentNode = nextNode;
                }

                if (currentNode.Spell)
                {
                    Debug.LogWarning($"Duplicate spell: {spell.name}");
                }
                else
                {
                    currentNode.Spell = spell;
                }
            }
        }
        public void Clear()
        {
            _startNodes.Clear();
        }

        public class TreeNode
        {
            public SpellStep SpellStep;
            public SpellScriptable Spell;
            public Dictionary<SpellStep, TreeNode> NextSteps;

            public TreeNode(SpellStep spellStep)
            {
                SpellStep = spellStep;
                Spell = null;
                NextSteps = new Dictionary<SpellStep, TreeNode>();
            }
            public TreeNode(SpellStep spellStep, SpellScriptable spell)
            {
                SpellStep = spellStep;
                Spell = spell;
                NextSteps = new Dictionary<SpellStep, TreeNode>();
            }
        }

        public TreeNode StartSearch(SpellStep step) => _startNodes.GetValueOrDefault(step);
    }
}