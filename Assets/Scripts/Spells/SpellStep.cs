using System;
using Recognition;
using Runes;
using UnityEngine;

namespace Spells
{
    [Serializable]
    public struct SpellStep : IEquatable<SpellStep>
    {
        [field: SerializeField] public SymbolFeaturesScriptable Symbol { get; private set; }
        [field: SerializeField] public RuneGroup RuneGroup { get; private set; }

        public bool IsValid => Symbol;
        
        public string GetName() => IsValid ? Symbol.name : "None";

        public SpellStep(SymbolFeaturesScriptable symbol, RuneGroup runeGroup)
        {
            Symbol = symbol;
            RuneGroup = runeGroup;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, RuneGroup);
        }
        public override bool Equals(object obj)
        {
            return obj is SpellStep other && Equals(other);
        }
        public bool Equals(SpellStep other)
        {
            return Equals(Symbol, other.Symbol) && RuneGroup == other.RuneGroup;
        }
        
        public static bool operator ==(SpellStep left, SpellStep right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(SpellStep left, SpellStep right)
        {
            return !(left == right);
        }
    }
}