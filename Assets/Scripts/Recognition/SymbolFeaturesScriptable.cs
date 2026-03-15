using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Recognition
{
    [CreateAssetMenu(menuName = "Recognition/" + nameof(SymbolFeaturesScriptable), 
        fileName = nameof(SymbolFeaturesScriptable))]
    public class SymbolFeaturesScriptable : ScriptableObject
    {
        [field: SerializeField] public SymbolFeatures Data { get; private set; } = new();
        
#if UNITY_EDITOR
        public static void CreateScriptableAndSave(string path, SymbolFeatures data)
        {
            var scriptable = CreateInstance<SymbolFeaturesScriptable>();
            scriptable.Data = data;
            
            AssetDatabase.CreateAsset(scriptable, path);
            EditorUtility.SetDirty(scriptable);
        }
#endif
    }
}