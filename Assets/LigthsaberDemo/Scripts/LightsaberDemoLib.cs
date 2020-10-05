using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LigthsaberDemo.Scripts
{
    public class LightsaberDemoLib
    {
        /// <summary>
        /// Calculate linear regression slope based on number of Vector2
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static float CalculateLinearRegressionSlope(IEnumerable<Vector2> vectors)
        {
            float sumOfX = 0;
            float sumOfY = 0;
            float sumOfXSq = 0;
            float sumCodeviates = 0;
        
            foreach (var v in vectors) {
                sumCodeviates += v.x * v.y;
                sumOfX += v.x;
                sumOfY += v.y;
                sumOfXSq += v.x * v.x;
            }

            var count = vectors.Count();
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);
        
            return sCo / ssX;
        }
        
        /// <summary>
        /// Calculate centroid of vertices
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 CalculateVectorsCentroid(IEnumerable<Vector3> input)
        {
            return input.Aggregate(Vector3.zero, (a, b) => a + b) / input.Count();
        }
        
        /// <summary>
        /// Calculate intersection point between two points (representing mesh edge) and plane.
        /// Edge **should** intersect plane, passing not-intersecting edge will result in undefined behavior.
        /// Points should be provided in order of `above`, `below`.
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="pointAbove">Point above the plane</param>
        /// <param name="pointBelow">Point below the plane</param>
        /// <returns></returns>
        public static Vector3 CalculateEdgePlaneIntersection(Plane plane, Vector3 pointAbove, Vector3 pointBelow)
        {
            var ray = new Ray(pointAbove, (pointAbove - pointBelow).normalized);
            plane.Raycast(ray, out var dist);
            return ray.origin + ray.direction * dist;
        }
    }
}