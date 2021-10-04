using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CoreLib
{
    public static class UnityRandom
    {
        public static ShuffleBag<T> ToShuffleBag<T>(this IEnumerable<T> values)
        {
            var result = new ShuffleBag<T>();
            foreach (var x in values) 
                result.Add(x);

            return result;
        }

        // 
        public static WeightedSet<T> ToWeightedBag<T>(this IEnumerable<T> values, float initalWeight)
        {
            var result = new WeightedSet<T>();
            foreach (var n in values)
                result.Add(n, initalWeight);

            return result;
        }

        public static WeightedSet<T> WeightedBag<T>(T[] values, float[] initalWeights)
        {
            var result = new WeightedSet<T>();
            for (var n = 0; n < values.Length; ++n)
                result.Add(values[n], initalWeights[n]);

            return result;
        }

        // 
        public static WeightedSet<T> CreateWeightedSet<T>(IEnumerable<T> values, Func<T, float> initalWeights)
        {
            var result = new WeightedSet<T>();
            foreach (var value in values)
                result.Add(value, initalWeights(value));

            return result;
        }

        public static WeightedSet<T> CreateWeightedSet<T>(IEnumerable<KeyValuePair<T, float>> data)
        {
            var result = new WeightedSet<T>();
            foreach (var value in data)
                result.Add(value.Key, value.Value);

            return result;
        }

        // uses unity random function, moves every member to random position
        public static void RandomizeList<TList>(TList source)
            where TList : IList
        {
            var n = source.Count;
            while (n > 1)
            {
                var k     = Random.Range(0, n--);
                var value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        // uses unity random function, random pos to random location
        public static void RandomizeList<T>(IList<T> source, int swapsCount)
        {
            for (var n = 0; n < swapsCount; ++n)
            {
                var c = Random.Range(0, source.Count);
                var t = Random.Range(0, source.Count);

                var value = source[c];
                source[c] = source[t];
                source[t] = value;
            }
        }

        public static T RandomFromArray<T>(params T[] source)
        {
            return source[Random.Range(0, source.GetLength(0))];
        }

        public static T RandomFromArray<T>(T[] source, T noOptionsValue)
        {
            if (source.Length == 0)
                return noOptionsValue;

            return source[Random.Range(0, source.GetLength(0))];
        }

        // get random element from list
        public static T RandomFromList<T>(IList<T> source)
        {
            if (source.Count == 0)
                return default;

            return source[Random.Range(0, source.Count)];
        }

        // get random element from list
        public static T RandomFromList<T>(IList<T> source, T noOptionsValue)
        {
            if (source.Count == 0)
                return noOptionsValue;

            return source[Random.Range(0, source.Count)];
        }

        // get random element from list, except some values
        public static T RandomFromList<T>(IList<T> source, T noOptionsValue, params T[] exept)
        {
            var result = new List<T>(source.Count);

            foreach (var n in source)
                if (Array.IndexOf(exept, n) == -1)
                    result.Add(n);

            if (result.Count == 0)
                return noOptionsValue;

            return RandomFromList(result);
        }

        // angle in degrees
        public static Vector2 NormalDeviation(Vector2 normal, float angleMin, float angleMax)
        {
            var angle = Mathf.Atan2(normal.y, normal.x) +
                        Random.Range(angleMin * Mathf.Deg2Rad, angleMax * Mathf.Deg2Rad);

            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Quaternion Rotation(float minDeg, float maxDeg, Vector3 axis)
        {
            return Quaternion.AngleAxis(Random.Range(minDeg, maxDeg), axis);
        }

        public static bool Bool(float trueProbability = 0.5f)
        {
            return Random.value < trueProbability ? true : false;
        }

        public static bool Bool(int trueProbability)
        {
            return Random.Range(0, 99) < trueProbability ? true : false;
        }

        public static Stack<bool> RandomStack(float chance, int distance = 100)
        {
            return RandomStack(Mathf.RoundToInt(distance * chance), distance);
        }

        public static Stack<bool> RandomStack(int count, int distance)
        {
            // fill first part of the list with false values and second part with true values, randomize list, copy to stack
            // slow but works
            count = Mathf.Clamp(count, 0, distance);

            var stack  = new Stack<bool>(distance);
            var values = new List<bool>(distance);
            for (var n = 0; n < count; n++)
                values.Add(true);
            for (var n = count; n < distance; n++)
                values.Add(false);

            RandomizeList(values);
            foreach (var n in values)
                stack.Push(n);

            return stack;
        }

        public static Color Color()
        {
            return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                             Random.Range(0.0f, 1.0f), 1.0f);
        }

        public static Color Color(float alpha)
        {
            return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                             Random.Range(0.0f, 1.0f), alpha);
        }

        public static Color Color(bool randomAlpha)
        {
            return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                             Random.Range(0.0f, 1.0f),
                             randomAlpha ? Random.Range(0.0f, 1.0f) : 1.0f);
        }

        public static Color Color(Vector2 rRange, Vector2 gRange, Vector2 bRange, Vector2 aRange)
        {
            return new Color(Random.Range(rRange.x, rRange.y), Random.Range(gRange.x, gRange.y),
                             Random.Range(bRange.x, bRange.y),
                             Random.Range(aRange.x, aRange.y));
        }


        public static Vector2Int Vector2Int(Vector2Int xRange, Vector2Int yRange)
        {
            return new Vector2Int(Random.Range(xRange.x, xRange.y),
                                  Random.Range(yRange.x, yRange.y));
        }

        public static Vector2Int Vector2Int(int xMagnitude, int yMagnitude)
        {
            var xHalf = (int)(xMagnitude * 0.5f);
            var yHalf = (int)(yMagnitude * 0.5f);
            return new Vector2Int(Random.Range(-xHalf, xHalf), Random.Range(-yHalf, yHalf));
        }

        public static Vector2Int Vector2Int(int xMin, int xMax, int yMin, int yMax)
        {
            return new Vector2Int(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }

        public static Vector2 Vector2(Vector2 xRange, Vector2 yRange)
        {
            return new Vector2(Random.Range(xRange.x, xRange.y),
                               Random.Range(yRange.x, yRange.y));
        }

        public static Vector2 Vector2(float magnitude)
        {
            var half = magnitude * 0.5f;
            return new Vector2(Random.Range(-half, half), Random.Range(-half, half));
        }

        public static Vector2 Vector2(float xMagnitude, float yMagnitude)
        {
            var xHalf = xMagnitude * 0.5f;
            var yHalf = yMagnitude * 0.5f;
            return new Vector2(Random.Range(-xHalf, xHalf), Random.Range(-yHalf, yHalf));
        }

        public static Vector2 Vector2(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }

        public static Vector3 Vector3(float magnitude)
        {
            var half = magnitude * 0.5f;
            return new Vector3(Random.Range(-half, half), Random.Range(-half, half),
                               Random.Range(-half, half));
        }

        public static Vector3 Vector3(float xMagnitude, float yMagnitude, float zMagnitude)
        {
            var xHalf = xMagnitude * 0.5f;
            var yHalf = yMagnitude * 0.5f;
            var zHalf = zMagnitude * 0.5f;
            return new Vector3(Random.Range(-xHalf, xHalf), Random.Range(-yHalf, yHalf),
                               Random.Range(-zHalf, zHalf));
        }

        public static Vector3 Vector3(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        {
            return new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax),
                               Random.Range(zMin, zMax));
        }

        public static Vector3 BoundPoint(Bounds bound)
        {
            return new Vector3(
                Random.Range(bound.min.x, bound.max.x),
                Random.Range(bound.min.y, bound.max.y),
                Random.Range(bound.min.z, bound.max.z));
        }

        public static Vector2 Normal2D(float lenght)
        {
            return Normal2D() * lenght;
        }

        public static Vector2 Normal2D()
        {
            var angle = Random.Range(0f, Mathf.PI * 2f);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector3 Normal3D()
        {
            return Random.onUnitSphere;
        }
    }
}