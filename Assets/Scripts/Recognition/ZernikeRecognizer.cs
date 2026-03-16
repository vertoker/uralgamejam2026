using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Recognition
{
    public class ZernikeRecognizer
    {
        private readonly RecognitionSettings _settings;

        public ZernikeRecognizer(RecognitionSettings settings)
        {
            _settings = settings;
        }

        public SymbolFeaturesScriptable Recognize(IReadOnlyList<Vector2> points)
        {
            if (points.Count == 0) return null;
            Debug.Log($"<b>Recognizing {points.Count} points</b>");
            
            var newPoints = new List<Vector2>(points);
            newPoints = NormalizePoints(newPoints, _settings.ResamplePoints);
            
            var zernikeMoments = CalculateZernikeMoments(newPoints, _settings.ZernikeOrder);
            var angularHistogram = CalculateAngularHistogram(newPoints, _settings.AngularCount);
            zernikeMoments = NormalizeZernikeMoments(zernikeMoments);
            
            var features = new SymbolFeatures(zernikeMoments, angularHistogram);
            
#if UNITY_EDITOR
            if (_settings.CreateAssetOnRecognition)
            {
                var path = $"Assets/Symbol_{Random.Range(1000, 10000)}.asset";
                SymbolFeaturesScriptable.CreateScriptableAndSave(path, points, features);
            }
#endif
            
            // TODO сделать поиск через JobSystem если будет подвисать
            SymbolFeaturesScriptable bestMatch = null;
            var bestSimilarity = 0f;
            
            foreach (var template in _settings.Templates)
            {
                var zernikeSimilarity = CompareZernike(features, template.Data);
                var angularSimilarity = CompareAngular(features, template.Data);
                
                var similarity =  _settings.ZernikeWeight * zernikeSimilarity + 
                                  (1f - _settings.ZernikeWeight) * angularSimilarity;
                
                if (similarity > bestSimilarity)
                {
                    bestSimilarity = similarity;
                    bestMatch = template;
                }
                
                Debug.Log($"Name: <b>{template.name}</b>, Similarity: <b>{similarity:F4}</b>, " +
                          $"Zernike: <b>{zernikeSimilarity:F4}</b>, Angular: <b>{angularSimilarity:F4}</b>");
            }

            if (bestSimilarity >= _settings.RecognitionThreshold)
            {
                ZernikeStatic.LogFeatures(bestMatch.Data);
                Debug.Log($"<color=green>Recognized</color>: <color=green>{bestMatch.name}</color> (similarity: <b>{bestSimilarity:F4}</b>)");
                return bestMatch;
            }
            
            Debug.Log($"<color=yellow>Not recognized</color> (best: <b>{bestSimilarity:F4}</b>)");
            return null;
        }

        /// <summary>
        /// Нормализация моментов Цернике к диапазону [0, 1]
        /// </summary>
        private float[] NormalizeZernikeMoments(float[] moments)
        {
            if (moments.Length == 0) return moments;
            
            // Находим максимальное значение
            var maxMoment = moments.Max();
            if (maxMoment < 0.0001f) return moments; // Предотвращаем деление на ноль
            
            // Нормализуем к [0, 1]
            var normalized = new float[moments.Length];
            for (var i = 0; i < moments.Length; i++)
            {
                normalized[i] = moments[i] / maxMoment;
            }
            
            return normalized;
        }

        private static List<Vector2> NormalizePoints(List<Vector2> points, int targetCount)
        {
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
            points = ZernikeStatic.ResamplePoints(points, targetCount);
            
            return points;
        }

        private float[] CalculateZernikeMoments(List<Vector2> points, int order)
        {
            var moments = new List<float>();
            float pointCount = points.Count;
            
            for (var n = 0; n <= order; n++)
            {
                for (var m = -n; m <= n; m += 2)
                {
                    if (Mathf.Abs(m) > n || (n - Mathf.Abs(m)) % 2 != 0)
                        continue;
                    
                    var momentReal = 0f;
                    var momentImag = 0f; // Для полноты, хотя мы используем амплитуду
                    
                    foreach (var p in points)
                    {
                        var radius = p.magnitude;
                        var theta = Mathf.Atan2(p.y, p.x);
                        
                        if (radius <= 1f)
                        {
                            var radial = ZernikeRadialPolynomial(n, Mathf.Abs(m), radius);
                            momentReal += radial * Mathf.Cos(m * theta);
                            momentImag += radial * Mathf.Sin(m * theta);
                        }
                    }
                    
                    // Нормализация
                    momentReal *= (n + 1) / Mathf.PI;
                    momentImag *= (n + 1) / Mathf.PI;
                    
                    // Используем амплитуду (инвариантность к повороту)
                    float amplitude = Mathf.Sqrt(momentReal * momentReal + momentImag * momentImag);
                    
                    // Дополнительная нормализация по количеству точек
                    amplitude /= pointCount;
                    
                    moments.Add(amplitude);
                }
            }
            
            return moments.ToArray();
        }

        private static float ZernikeRadialPolynomial(int n, int m, float r)
        {
            var sum = 0f;
            var k = (n - m) / 2;
            
            for (var s = 0; s <= k; s++)
            {
                if (n - 2 * s >= 0)
                {
                    var sign = (s % 2 == 0) ? 1f : -1f;
                    var numerator = ZernikeStatic.Factorial(n - s);
                    var denominator = ZernikeStatic.Factorial(s) 
                                      * ZernikeStatic.Factorial(k - s) 
                                      * ZernikeStatic.Factorial((n - m) / 2 - s);
                    
                    if (denominator != 0)
                    {
                        var coefficient = sign * numerator / denominator;
                        sum += coefficient * Mathf.Pow(r, n - 2 * s);
                    }
                }
            }
            
            return sum;
        }

        private float[] CalculateAngularHistogram(List<Vector2> points, int bins)
        {
            var histogram = new float[bins];
            
            foreach (var p in points)
            {
                if (p.magnitude < 0.05f) continue; // Игнорируем точки в центре
                
                var theta = Mathf.Atan2(p.y, p.x);
                if (theta < 0) theta += 2 * Mathf.PI;
                
                var bin = Mathf.FloorToInt(theta / (2 * Mathf.PI / bins));
                bin = Mathf.Clamp(bin, 0, bins - 1);
                
                histogram[bin]++;
            }
            
            // Нормализация
            var total = histogram.Sum();
            if (total > 0.001f)
            {
                for (var i = 0; i < bins; i++)
                    histogram[i] /= total;
            }
            
            return histogram;
        }

        private float CompareAngular(SymbolFeatures a, SymbolFeatures b)
        {
            // Сравнение гистограмм (пересечение)
            var histogramSimilarity = 0f;
            for (var i = 0; i < _settings.AngularCount; i++)
            {
                histogramSimilarity += Mathf.Min(a.AngularHistogram[i], b.AngularHistogram[i]);
            }

            return histogramSimilarity;
        }

        private static float CompareZernike(SymbolFeatures a, SymbolFeatures b)
        {
            // Сравнение моментов Цернике (косинусное сходство - лучше для нормализованных векторов)
            var zernikeDot = 0f;
            var zernikeMagA = 0f;
            var zernikeMagB = 0f;

            for (var i = 0; i < a.ZernikeMoments.Length; i++)
            {
                zernikeDot += a.ZernikeMoments[i] * b.ZernikeMoments[i];
                zernikeMagA += a.ZernikeMoments[i] * a.ZernikeMoments[i];
                zernikeMagB += b.ZernikeMoments[i] * b.ZernikeMoments[i];
            }

            var zernikeSimilarity = 0f;
            if (zernikeMagA > 0.0001f && zernikeMagB > 0.0001f)
            {
                zernikeSimilarity = zernikeDot / (Mathf.Sqrt(zernikeMagA) * Mathf.Sqrt(zernikeMagB));
            }

            return zernikeSimilarity;
        }
    }
}