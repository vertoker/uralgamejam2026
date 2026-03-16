using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Recognition
{
    [CreateAssetMenu(menuName = "Recognition/" + nameof(SymbolFeaturesScriptable), 
        fileName = nameof(SymbolFeaturesScriptable))]
    public class SymbolFeaturesScriptable : ScriptableObject
    {
        [field: SerializeField] public SymbolFeatures Data { get; private set; }
        [field: SerializeField] public Texture2D Texture { get; private set; }
        
        [SerializeField] private Vector2[] _sourceInput = Array.Empty<Vector2>();
        public IReadOnlyList<Vector2> SourceInput => _sourceInput;
        
#if UNITY_EDITOR
        public static void CreateScriptableAndSave(string path, IReadOnlyList<Vector2> sourceInput, SymbolFeatures data)
        {
            var scriptable = CreateInstance<SymbolFeaturesScriptable>();
            scriptable._sourceInput = sourceInput.ToArray();
            scriptable.Data = data;
            
            AssetDatabase.CreateAsset(scriptable, path);
            EditorUtility.SetDirty(scriptable);
        }

        private const int TextureLength = 256;
        private const int ResampleCount = 64;
        private static readonly Color[] TextureArray = new Color[TextureLength * TextureLength];
        
        [Button]
        public void CreateTexture()
        {
            var path = $"{Application.dataPath}/{name}.png";
            var assetPath = $"Assets/{name}.png";

            var points = _sourceInput.ToList();
            points = ZernikeStatic.RemoveDuplicates(points);
            
            var center = ZernikeStatic.ComputeCentroid(points);
            var maxRadius = 0f;
            
            // Центрируем и находим максимальное расстояние
            for (var i = 0; i < points.Count; i++)
            {
                var centeredPoint = points[i] - center;
                points[i] = centeredPoint;
                maxRadius = Mathf.Max(maxRadius, centeredPoint.magnitude);
            }

            // Масштабируем к единичному кругу
            if (maxRadius > 0.0001f)
            {
                for (var i = 0; i < points.Count; i++)
                    points[i] /= maxRadius;
            }
            
            // Ресемплинг для равномерного распределения
            points = ZernikeStatic.ResamplePoints(points, ResampleCount);

            // Пересборка точек под текстуру
            var texturePoints = new List<Vector2Int>(points.Count);
            foreach (var point in points)
            {
                var textureCoord = (point + new Vector2(1f, 1f)) / 2f * TextureLength;
                texturePoints.Add(new Vector2Int((int)textureCoord.x, (int)textureCoord.y));
            }

            var texture = new Texture2D(TextureLength, TextureLength, TextureFormat.RGBA32, false);
            Array.Clear(TextureArray, 0, TextureArray.Length);
            
            for (var i = 1; i < texturePoints.Count; i++)
            {
                Vector2Int last = texturePoints[i - 1], next = texturePoints[i];
                TextureStatic.DrawLine(TextureArray, TextureLength, TextureLength, 
                    last.x, last.y, next.x, next.y, Color.white, 1f);
            }
            
            texture.SetPixels(TextureArray);
            texture.Apply();

            var pngBytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngBytes);
            AssetDatabase.Refresh();
            
            Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}