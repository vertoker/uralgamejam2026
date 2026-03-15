using System.Collections.Generic;

namespace Spells
{
    public class SpellContainer
    {
        private Dictionary<SpellStep, SpellScriptable> _dictionary;
        
        public SpellContainer(SpellsSettings settings)
        {
            _dictionary = new Dictionary<SpellStep, SpellScriptable>();
        }
    }
}