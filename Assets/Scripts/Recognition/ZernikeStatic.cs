using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Recognition
{
    public static class ZernikeStatic
    {
        public static List<Vector2> RemoveDuplicates(List<Vector2> points)
        {
            if (points.Count <= 1) return points;
            
            var result = new List<Vector2> { points[0] };
            const float minDist = 0.01f;

            for (var i = 1; i < points.Count; i++)
            {
                if (Vector2.Distance(points[i], result.Last()) > minDist)
                    result.Add(points[i]);
            }
            
            return result;
        }

        public static Vector2 ComputeCentroid(List<Vector2> points)
        {
            if (points.Count == 0) return Vector2.zero;
            
            var sum = Vector2.zero;
            foreach (var point in points)
                sum += point;
            return sum / points.Count;
        }

        public static List<Vector2> ResamplePoints(List<Vector2> points, int targetCount)
        {
            if (points.Count <= 1 || targetCount <= 1) return points;
            
            var result = new List<Vector2>();
            var totalLength = 0f;
            
            for (var i = 1; i < points.Count; i++)
                totalLength += Vector2.Distance(points[i - 1], points[i]);
            
            if (totalLength < 0.0001f) return points;
            
            var segmentLength = totalLength / (targetCount - 1);
            var currentDist = 0f;
            
            result.Add(points[0]);
            
            for (var i = 1; i < points.Count && result.Count < targetCount; i++)
            {
                var dist = Vector2.Distance(points[i - 1], points[i]);
                
                while (currentDist + dist >= segmentLength && result.Count < targetCount)
                {
                    var t = (segmentLength - currentDist) / dist;
                    var newPoint = Vector2.Lerp(points[i - 1], points[i], t);
                    result.Add(newPoint);
                    
                    dist -= (segmentLength - currentDist);
                    currentDist = 0f;
                }
                
                currentDist += dist;
            }
            
            // Добиваем последнюю точку
            while (result.Count < targetCount)
                result.Add(points.Last());
            
            return result;
        }

        public static float Factorial(int n)
        {
            if (n <= 1) return 1f;
            
            var result = 1f;
            for (var i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        public static void LogFeatures(SymbolFeatures features)
        {
            var zernikeString = string.Join(" ", features.ZernikeMoments.Select(f => f.ToString("F3")));
            var angularString = string.Join(" ", features.AngularHistogram.Select(f => f.ToString("F3")));
            Debug.Log($"Zernike moments: [{zernikeString}], Angular histogram: [{angularString}]");
        }
    }
}