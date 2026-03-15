using System.Collections.Generic;

namespace Spells
{
    public class SpellCounter
    {
        private readonly SpellContainer _container;
        private List<SpellStep> _steps = new();

        public SpellCounter(SpellContainer container)
        {
            _container = container;
        }

        public SpellScriptable Step(SpellStep step)
        {
            // TODO закончить эти алгоритмы
            return null;
        }
    }
}