using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Recognition
{
    public class ZernikeRecognizer
    {
        private readonly RecognitionData _data;

        public ZernikeRecognizer(RecognitionData recognitionData)
        {
            _data = recognitionData;
        }

        public void Recognize(IReadOnlyList<Vector2> points)
        {
            if (points.Count == 0) return;
            var newPoints = new List<Vector2>(points.Count);
            newPoints.AddRange(points);

            NormalizePoints(newPoints);
            var features = ExtractFeatures(newPoints);

            var zernikeMomentsString = string.Join(' ', features.zernikeMoments);
            var angularHistogramString = string.Join(' ', features.angularHistogram);
            Debug.Log($"Zernike moments: {zernikeMomentsString}, Angular moments: {angularHistogramString}");
        }

        private void NormalizePoints(List<Vector2> points)
        {
            // Находим центр масс
            var center = Vector2.zero;
            foreach (var point in points)
                center += point;
            center /= points.Count;

            // Центрируем и находим максимальное расстояние
            var maxDist = 0f;
            for (var i = 0; i < points.Count; i++)
            {
                var centeredPoint = points[i] - center;
                points[i] = centeredPoint;
                maxDist = Mathf.Max(maxDist, centeredPoint.magnitude);
            }

            // Масштабируем к кругу с максимальной дистанцией от центра массы - 1
            if (maxDist == 0f) return;
            for (var i = 0; i < points.Count; i++)
                points[i] /= maxDist;
        }

        private SymbolFeatures ExtractFeatures(List<Vector2> points)
        {
            var features = new SymbolFeatures();
            features.zernikeMoments = new float[4];
            
            // Моменты Цернике (упрощенно)
            // В реальном проекте используйте полноценные полиномы Цернике
            foreach (var point in points)
            {
                var radius = point.magnitude;
                var theta = Mathf.Atan2(point.y, point.x);
            
                if (radius <= 1f) // Только внутри единичного круга
                {
                    // M0,0 - площадь (количество точек)
                    features.zernikeMoments[0] += 1;
                
                    // M1,1 - центр масс по x (упрощенно)
                    features.zernikeMoments[1] += radius * Mathf.Cos(theta);
                
                    // M2,0 - момент инерции (упрощенно)
                    features.zernikeMoments[2] += radius * radius;
                
                    // M2,2 - астигматизм (упрощенно)
                    features.zernikeMoments[3] += radius * radius * Mathf.Cos(2 * theta);
                }
            }
            
            // Угловая гистограмма (8 направлений)
            features.angularHistogram = new float[8];
            foreach (var point in points)
            {
                var theta = Mathf.Atan2(point.y, point.x);
                if (theta < 0) theta += 2 * Mathf.PI;
            
                var bin = Mathf.FloorToInt(theta / (2 * Mathf.PI / 8));
                bin = Mathf.Clamp(bin, 0, 7);
                features.angularHistogram[bin]++;
            }
            
            // Нормализация
            var total = features.angularHistogram.Sum();
            if (total > 0)
            {
                for (var i = 0; i < 8; i++)
                    features.angularHistogram[i] /= total;
            }
            
            return features;
        }

        private float CompareFeatures(SymbolFeatures a, SymbolFeatures b)
        {
            // Сравнение моментов Цернике (L2 норма)
            var zernikeDiff = 0f;
            for (var i = 0; i < a.zernikeMoments.Length; i++)
            {
                var diff = a.zernikeMoments[i] - b.zernikeMoments[i];
                zernikeDiff += diff * diff;
            }
            var zernikeSimilarity = 1f / (1f + Mathf.Sqrt(zernikeDiff));
            
            // Сравнение гистограмм (пересечение гистограмм)
            var histogramSimilarity = 0f;
            for (var i = 0; i < 8; i++)
            {
                histogramSimilarity += Mathf.Min(a.angularHistogram[i], b.angularHistogram[i]);
            }
            
            // Взвешенная сумма (70% форма, 30% ориентация)
            return 0.7f * zernikeSimilarity + 0.3f * histogramSimilarity;
        }
        
        [System.Serializable]
        public class SymbolFeatures
        {
            public float[] zernikeMoments;    // Моменты Цернике
            public float[] angularHistogram;  // Угловая гистограмма (8 бинов)
        }
    }
}