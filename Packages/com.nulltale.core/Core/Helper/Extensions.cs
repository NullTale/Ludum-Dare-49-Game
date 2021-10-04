using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using CompressionLevel = System.IO.Compression.CompressionLevel;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreLib
{
    public static class Extensions
    {
        private static Dictionary<(Type, Type), MethodInfo> s_DynamicCastCache = new Dictionary<(Type, Type), MethodInfo>();

        // =======================================================================
        private class CircularEnumarator<T> : IEnumerator<T>
        {
            private readonly IEnumerator _wrapedEnumerator;

            public CircularEnumarator(IEnumerator wrapedEnumerator)
            {
                _wrapedEnumerator = wrapedEnumerator;
            }

            public object Current => _wrapedEnumerator.Current;

            T IEnumerator<T>.Current =>  (T)Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (!_wrapedEnumerator.MoveNext())
                {
                    _wrapedEnumerator.Reset();
                    return _wrapedEnumerator.MoveNext();
                }
                return true;
            }

            public void Reset()
            {
                _wrapedEnumerator.Reset();
            }
        }

        // =======================================================================
        #region Comparison

        public static bool Check(this Core.ComparisonOperation comparison, float a, float b)
        {
            switch (comparison)
            {
                case Core.ComparisonOperation.Less:
                    return a < b;
                case Core.ComparisonOperation.Greater:
                    return a > b;
                case Core.ComparisonOperation.Equal:
                    return a == b;
                case Core.ComparisonOperation.NotEqual:
                    return a != b;
                case Core.ComparisonOperation.LessOrEqual:
                    return a <= b;
                case Core.ComparisonOperation.GreaterOrEqual:
                    return a >= b;
                case Core.ComparisonOperation.Any:
                    return true;
                case Core.ComparisonOperation.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }
    
        public static bool Check<T>(this Core.ComparisonOperation comparison, T a, T b) where T : IComparable
        {
            switch (comparison)
            {
                case Core.ComparisonOperation.Less:
                    return a.CompareTo(b) < 0;
                case Core.ComparisonOperation.Greater:
                    return a.CompareTo(b) > 0;
                case Core.ComparisonOperation.Equal:
                    return a.CompareTo(b) == 0;
                case Core.ComparisonOperation.NotEqual:
                    return a.CompareTo(b) != 0;
                case Core.ComparisonOperation.LessOrEqual:
                    return a.CompareTo(b) <= 0;
                case Core.ComparisonOperation.GreaterOrEqual:
                    return a.CompareTo(b) >= 0;
                case Core.ComparisonOperation.Any:
                    return true;
                case Core.ComparisonOperation.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }
    
        public static bool Check(this Core.ComparisonOperation comparison, int a, int b)
        {
            switch (comparison)
            {
                case Core.ComparisonOperation.Less:
                    return a < b;
                case Core.ComparisonOperation.Greater:
                    return a > b;
                case Core.ComparisonOperation.Equal:
                    return a == b;
                case Core.ComparisonOperation.NotEqual:
                    return a != b;
                case Core.ComparisonOperation.LessOrEqual:
                    return a <= b;
                case Core.ComparisonOperation.GreaterOrEqual:
                    return a >= b;
                case Core.ComparisonOperation.Any:
                    return true;
                case Core.ComparisonOperation.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }

        #endregion

        #region Coroutines

        // =======================================================================
        public static Coroutine Delayed(this MonoBehaviour obj, Action action, float timeDelay)
        {
            return obj.StartCoroutine(DelayRun(timeDelay, action));

            static IEnumerator DelayRun(float delay, Action action) 
            {
                yield return new WaitForSeconds(delay);
                action();
            }
        }

        public static Coroutine Delayed(this MonoBehaviour obj, Action action, int frameDelay = 1)
        {
            return obj.StartCoroutine(_run(frameDelay, action));
            
            static IEnumerator _run(int frameCount, Action action) 
            {
                while (frameCount-- > 0)
                    yield return null;

                action();
            }
        }

        public static Coroutine LateUpdate(this MonoBehaviour obj, Action action)
        {
            return obj.StartCoroutine(_run(action));
            
            static IEnumerator _run( Action action) 
            {
                yield return Core.k_WaitForEndOfFrame;

                action();
            }
        }

        public static Coroutine Repeat(this MonoBehaviour obj, Action action, int repeat, float repeatInterval) 
        {
            return obj.StartCoroutine(_run(repeat, action, repeatInterval));

            static IEnumerator _run(int repeat, Action action, float repeatInterval) 
            {
                if (repeat <= 0)	yield break;
            
                var interval = new WaitForSeconds(repeatInterval);

                // repeat with interval
                do
                {
                    action();
                    yield return interval;
                }   
                while (repeat-- >= 0);
            }
        }

        public static Coroutine Forever(this MonoBehaviour obj, Action action) 
        {
            return obj.StartCoroutine(_run(action));

            static IEnumerator _run(Action action) 
            {
                while (true)
                {
                    action();
                    yield return null;
                }
            }
        }

        public static Coroutine Forever(this MonoBehaviour obj, Func<object> action) 
        {
            return obj.StartCoroutine(_run(action));

            static IEnumerator _run(Func<object> action) 
            {
                while (true)
                    yield return action();
            }
        }
        
        public static Coroutine Forever(this MonoBehaviour obj, Func<object> action, float interval) 
        {
            return obj.StartCoroutine(_run(action, interval));

            static IEnumerator _run(Func<object> action, float interval) 
            {
                var wait = new WaitForSeconds(interval);
                yield return wait;

                while (true)
                {
                    yield return action();
                    yield return wait;
                }
            }
        }
        
        public static Coroutine While(this MonoBehaviour obj, Func<bool> condition, Action action)
        {
            return obj.StartCoroutine(WhileRun(condition, action));

            static IEnumerator WhileRun(Func<bool> condition, Action action) 
            {
                // while run
                while (condition())
                {
                    action.Invoke();
                    yield return null;
                }
            }
        }

        #endregion

        #region External

        public static void JsSyncFiles()
        {
            if (Application.isEditor == false)
                SyncFiles();
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        internal static extern void SyncFiles();

        [DllImport("__Internal")]
        internal static extern void WindowAlert(string message);
#else
        public static void SyncFiles(){ }

        public static void WindowAlert(string message){ }
#endif

        #endregion

        #region Array2D

        public static T[,] Take<T>(this T[,] array, in RectInt square)
        {
            return Take(array, square.xMin, square.yMin, square.xMax, square.yMax);
        }

        public static T[,] Take<T>(this T[,] array, Vector2Int at, Vector2Int to)
        {
            return Take(array, at.x, to.x, at.y, to.y);
        }

        public static T[,] Take<T>(this T[,] array, int xMin, int yMin, int xMax, int yMax)
        {
            var takeWidth = xMax - xMin;
            var takeHeight = yMax - yMin;

            var result = new T[takeWidth, takeHeight];

            for (var x = 0; x < takeWidth; x++)
            for (var y = 0; y < takeHeight; y++)
                result[x, y] =  array[xMin + x, yMin + y];

            return result;
        }

        public static T GetValue<T>(this T[,] array, in Vector2Int index)
        {
            return array[index.x, index.y];
        }

        public static T GetValueSafe<T>(this T[,] array, in Vector2Int index)
        {
            return array.GetValueSafe(index.x, index.y);
        }
        
        public static T GetValueSafe<T>(this T[,] array, int x, int y)
        {
            return InBounds(array, x, y) ? array[x, y] : default;
        }

        public static void SetValue<T>(this T[,] array, T value, in Vector2Int index)
        {
            array[index.x, index.y] = value;
        }

        public static void SetValueSafe<T>(this T[,] array, in Vector2Int index, T value)
        {
            SetValueSafe(array, index.x, index.y, value);
        }

        public static void SetValueSafe<T>(this T[,] array, int x, int y, T value)
        {
            if (InBounds(array, x, y))
                array[x, y] = value;
        }

        public static bool InBounds<T>(this T[,] array, in Vector2Int index)
        {
            return InBounds(array, index.x, index.y);
        }

        public static bool InBounds<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1);
        }

        public static bool TrySetValue<T>(this T[,] array, in Vector2Int index, T value)
        {
            return TrySetValue(array, index.x, index.y, value);
        }

        public static bool TrySetValue<T>(this T[,] array, int x, int y, T value)
        {
            if (InBounds(array, x, y))
            {
                array[x, y] = value;
                return true;
            }
            
            return false;
        }

        public static bool TryGetValue<T>(this T[,] array, in Vector2Int index, out T value)
        {
            return TryGetValue(array, index.x, index.y, out value);
        }

        public static bool TryGetValue<T>(this T[,] array, int x, int y, out T value)
        {
            if (InBounds(array, x, y))
            {
                value = array[x, y];
                return true;
            }

            value = default;
            return false;
        }
        
        public static int GetVolume<T>(this T[,] array)
        {
            return array.GetLength(0) * array.GetLength(1);
        }
        
        public static T Random<T>(this T[,] array)
        {
            var w = array.GetLength(0);
            var h = array.GetLength(1);
            return array[UnityEngine.Random.Range(0, w - 1), UnityEngine.Random.Range(0, h - 1)];
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] array)
        {
            for (var y = 0; y < array.GetLength(1); y++)
            for (var x = 0; x < array.GetLength(0); x++)
                yield return array[x, y];
        }
        public static IEnumerable<T> ToEnumerable<T>(this T[,] array, int xMin, int yMin, int xMax, int yMax)
        {
            for (var y = yMin; y <= yMax; y++)
            for (var x = xMin; x <= xMax; x++)
                yield return array[x, y];
        }
        public static IEnumerable<(int x, int y, T value)> Enumerate<T>(this T[,] array)
        {
            for (var y = 0; y < array.GetLength(1); y++)
            for (var x = 0; x < array.GetLength(0); x++)
                yield return (x, y, array[x, y]);
        }

        public static List<T> ToList<T>(this T[,] array)
        {
            var result = new List<T>(array.GetLength(0) * array.GetLength(1));

            foreach (var element in ToEnumerable(array))
                result.Add(element);

            return result;
        }

        public static T[] ToArray<T>(this T[,] array)
        {
            var result = new T[array.GetLength(0) * array.GetLength(1)];

            for (var y = 0; y < array.GetLength(1); y++)
            for (var x = 0; x < array.GetLength(0); x++)
                result[y * x + x] = array[x, y];

            return result;
        }
        
        public static T[,] Initialize<T>(this T[,] array, Action<int, int, T[,]> action)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            for (var y = 0; y < array.GetLength(1); y++)
                action(x, y, array);

            return array;
        }

        public static T[,] Initialize<T>(this T[,] array, Func<int, int, T> action)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            for (var y = 0; y < array.GetLength(1); y++)
                array[x, y] = action(x, y);

            return array;
        }

        public static T[,] Clear<T>(this T[,] array)
        {
            Array.Clear(array, 0, array.GetLength(0) * array.GetLength(1));
            return array;
        }

        #endregion

        #region Zip

        public static byte[] Zip(this string str, CompressionLevel compression = CompressionLevel.Optimal) 
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) 
            {
                using (var gs = new GZipStream(mso, compression)) 
                    msi.CopyTo(gs);

                return mso.ToArray();
            }
        }

        public static string UnzipString(this byte[] data)
        {
            return Encoding.UTF8.GetString(data.Unzip());
        }
        
        public static byte[] Zip(this byte[] data) 
        {
            using (var msi = new MemoryStream(data))
            using (var mso = new MemoryStream()) 
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress)) 
                    msi.CopyTo(gs);

                return mso.ToArray();
            }
        }

        public static byte[] Unzip(this byte[] bytes) 
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    gs.CopyTo(mso);

                return mso.ToArray();
            }
        }

        #endregion

        #region Rect

        public static RectInt ToRectInt(this Rect rect)
        {
            return new RectInt((int)rect.xMin, (int)rect.yMin, Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height));
        }
        
        public static Rect ToRect(this RectInt rect)
        {
            return new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
        }
        
        public static Vector2Int RandomPos(this RectInt rect)
        {
            return new Vector2Int(UnityEngine.Random.Range(rect.xMin, rect.xMax), UnityEngine.Random.Range(rect.yMin, rect.yMax));
        }

        public static Rect WithXY(this Rect rect, float xMin, float yMin)
        {
            return new Rect(xMin, yMin, rect.width, rect.height);
        }

        public static Rect WithX(this Rect rect, float xMin)
        {
            return new Rect(xMin, rect.yMin, rect.width, rect.height);
        }
        
        public static Rect WithY(this Rect rect, float yMin)
        {
            return new Rect(rect.xMin, yMin, rect.width, rect.height);
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            return new Rect(rect.xMin, rect.yMin, width, rect.height);
        }
        public static Rect WithHeight(this Rect rect, float height)
        {
            return new Rect(rect.xMin, rect.yMin, rect.width, height);
        }
        public static Rect IncHeight(this Rect rect, float addHeight)
        {
            return new Rect(rect.x, rect.y, rect.width, rect.height + addHeight);
        }
        public static Rect IncHeight(this Rect rect, float addHeight, Vector2 pivot)
        {
            return new Rect(rect.x, rect.y + pivot.y * addHeight, rect.width, rect.height + addHeight);
        }

        public static Rect IncXY(this Rect rect, float addX, float addY)
        {
            return new Rect(rect.xMin + addX, rect.yMin + addY, rect.width, rect.height);
        }
        
        public static Rect IncX(this Rect rect, float addX )
        {
            return new Rect(rect.xMin + addX, rect.yMin, rect.width, rect.height);
        }
        public static Rect IncY(this Rect rect, float addY)
        {
            return new Rect(rect.xMin, rect.yMin + addY, rect.width, rect.height);
        }

        public static Rect IncWidth(this Rect rect, float addWidth)
        {
            return new Rect(rect.x, rect.y, rect.width + addWidth, rect.height);
        }
        public static Rect IncWidth(this Rect rect, float addWidth, Vector2 pivot)
        {
            return new Rect(rect.x + pivot.x * addWidth, rect.y, rect.width + addWidth, rect.height);
        }
        
        public static Vector2 Clamp(this Rect rect, Vector2 v)
        {
            return new Vector2(Mathf.Clamp(v.x, rect.xMin, rect.xMax), Mathf.Clamp(v.y, rect.yMin, rect.yMax));
        }

        public static RectInt FrameCrop(this RectInt rect, RectInt region)
        {
            if (region.xMin < rect.xMin)
                region.xMin = rect.xMin;
            
            if (region.xMax > rect.xMax)
                region.xMax = rect.xMax;
            
            if (region.yMin < rect.yMin)
                region.yMin = rect.yMin;

            if (region.yMax > rect.yMax)
                region.yMax = rect.yMax;

            return region;
        }

        public static RectInt FrameOf(this RectInt rect, RectInt region)
        {
            if (rect.width < region.width)
                region.width = rect.width;
            
            if (rect.height < region.height)
                region.height = rect.height;
            
            if (region.x < rect.xMin)
                region.x = rect.xMin;
            
            if (region.xMax > rect.xMax)
                region.x = rect.xMax - region.width;
            
            if (region.y < rect.yMin)
                region.y = rect.yMin;

            if (region.yMax > rect.yMax)
                region.y = rect.yMax - region.height;

            return region;
        }

        public static RectInt WithXY(this RectInt rect, int xMin, int yMin)
        {
            return new RectInt(xMin, yMin, rect.width, rect.height);
        }

        public static RectInt WithWH(this RectInt rect, int width, int height)
        {
            return new RectInt(rect.xMin, rect.yMin, width, height);
        }

        public static Rect ToRectXY(this Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        #endregion

        #region Texture

        public static RectInt Rect(this Texture2D texture)
        {
            return new RectInt(0, 0, texture.width, texture.height);
        }

        public static Texture2D Copy(this Texture2D texture, TextureFormat format = TextureFormat.RGBA32)
        {
            return texture.Copy(new RectInt(0, 0, texture.width, texture.height), format);
        }

        public static Texture2D Copy(this Texture2D texture, RectInt rect, TextureFormat format = TextureFormat.RGBA32)
        {
            var dst = new Texture2D(rect.width, rect.height, format, false, false);
            try
            {
                dst.filterMode = texture.filterMode;
                dst.SetPixels(0, 0, rect.width, rect.height, texture.GetPixels(rect.x, rect.y, rect.width, rect.height), 0);
                dst.Apply();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Can't copy texture {e}");
            }
            /*try
            {
            }
            catch 
            {
            }*/

            return dst;
        }

        #endregion

        // =======================================================================
        public static bool IsNull(this object obj)
        {
            return ReferenceEquals(obj, null);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        
        public static TEnum ToEnum<TEnum>(this string str)
            where TEnum : unmanaged, Enum
        {
            Enum.TryParse(str, out TEnum result);
            return result;
        }

        public static bool IsOnScreen(this Transform t) 
        {
            var onScreen = Camera.current.WorldToViewportPoint(t.position);
            return onScreen.z > 0 && onScreen.x > 0 && onScreen.y > 0 && onScreen.x < 1 && onScreen.y < 1;
        }

        /*public static bool CompareTagSafe(this GameObject go, string tag)
        {
            go.CompareTag(string.IsNullOrEmpty(tag)
        }*/

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator) 
        {
            while (enumerator.MoveNext()) 
                yield return enumerator.Current;
        }

        public static IEnumerator<T> ToCircular<T>(this IEnumerable<T> t) 
        {
            return new CircularEnumarator<T>(t.GetEnumerator());
        }
        
        public static bool TryGetValue<T>(this IEnumerable<T> t, Func<T, bool> check, out T value) 
            where T : class
        {
            value = t.FirstOrDefault(check);
            return !value.IsNull();
        }

        public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> t) 
            where T : class
        {
            var index = 0;
            var enumerator = t.GetEnumerator();
            while (enumerator.MoveNext())
                yield return (index ++, enumerator.Current);
            enumerator.Dispose();
        }

        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator.MoveNext())
                return enumerator.Current;

            return default;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T item)
        {
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
                if (Equals(enumerator.Current, item) == false)
                    yield return enumerator.Current;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            return GetPermutations(itemsArray, itemsArray.Length);
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items, int count)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            return GetPermutations(itemsArray, count);
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(T[] items, int count)
        {
            if (count == 1)
            {
                foreach (var item in items)
                    yield return new T[] {item};

                yield break;
            }

            foreach(var item in items)
            {
                foreach (var result in GetPermutations(items.Except(new [] { item }).ToArray(), count - 1))
                    yield return new T[] { item }.Concat(result);
            }
        }

        public static string FullActionName(this InputAction inputAction)
        {
            if (inputAction.actionMap == null)
                return inputAction.name;

            return inputAction.actionMap.name + '/' + inputAction.name;
        }


        private class GeneralPropertyComparer<T,TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> expr { get; }

            public GeneralPropertyComparer (Func<T, TKey> expr)
            {
                this.expr = expr;
            }

            public bool Equals(T left, T right)
            {
                var leftProp  = expr.Invoke(left);
                var rightProp = expr.Invoke(right);

                if (leftProp == null && rightProp == null)
                    return true;

                if (leftProp == null ^ rightProp == null)
                    return false;

                return leftProp.Equals(rightProp);
            }
            public int GetHashCode(T obj)
            {
                var prop = expr.Invoke(obj);
                return (prop==null)? 0:prop.GetHashCode();
            }
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.Distinct(new GeneralPropertyComparer<T,TKey>(property));
        }

        public static LinkedListNode<T> FirstOrDefault<T>(this LinkedList<T> source, Func<LinkedListNode<T>, bool> predicate)
        {
            for (var current = source.First; current != null;  current = current.Next)
                if (predicate(current))
                    return current;

            return null;
        }
        
        public static bool Implements<T>(this Type source) where T : class
        {
            return typeof(T).IsAssignableFrom(source);
        }

        public static bool DynamicCast<TType>(this object source, out TType result)
        {
            var srcType = source.GetType();
            var destType = typeof(TType);

            if (srcType == destType)
            {
                result = (TType)source;
                return true;
            }
            
            if (destType.IsEnum)
            {
                result = (TType)Enum.ToObject(destType, srcType);
                return true;
            }

            if (s_DynamicCastCache.TryGetValue((srcType, destType), out var cast) == false)
            {
                cast = GetCastOperator<TType>(source);
                s_DynamicCastCache.Add((srcType, destType), cast);
            }

            if (cast == null)
            {
                result = default;
                return false;
            }

            result = (TType)cast.Invoke(null, new object[] { source });

            return true;

        }

        public static MethodInfo GetCastOperator<TType>(this object source)
        {
            var srcType = source.GetType();
            var destType = typeof(TType);

            while (srcType != null)
            {
                var cast = srcType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                  .Where(mi =>
                                  {
                                      if ((mi.Name == "op_Explicit" || mi.Name == "op_Implicit") == false)
                                          return false;

                                      if (mi.ReturnType != destType)
                                          return false;

                                      var pars = mi.GetParameters();
                                      //if (pars.Length != 1 || pars[0].ParameterType != srcType)
                                      if (pars.Length != 1 || pars[0].ParameterType.IsAssignableFrom(srcType) == false)
                                          return false;

                                      return true;
                                  })
                                  .FirstOrDefault();

				if (cast != null)
					return cast;
				
                srcType = srcType.BaseType;
            }

            return null;
        }

        public static IEnumerable<Type> GetBaseTypes(this Type source)
        {
            var current = source;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<T> GetEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>();
        }

        public static void DrawCircle(Vector3 pos, float radius, Vector3 up, Color color, int segments = 20, float duration = 0)
        {
            DrawEllipse(pos, Quaternion.LookRotation(up), radius, radius, color, segments, duration);
        }

        public static void DrawEllipse(Vector3 pos, float radius, Color color, int segments = 20, float duration = 0)
        {
            DrawEllipse(pos, Quaternion.identity, radius, radius, color, segments, duration);
        }

        public static void DrawEllipse(Vector3 pos, Quaternion rotation, float radiusX, float radiusY, Color color, int segments, float duration = 0)
        {
            var angle     = 0f;
            var rot       = rotation;
            var lastPoint = Vector3.zero;
            var thisPoint = Vector3.zero;
 
            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;
 
                if (i > 0)
                {
                    Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }
 
                lastPoint =  thisPoint;
                angle     += 360f / segments;
            }
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return GetEnum<T>();
        }

        public static IEnumerable<T> GetValues<T>(this T en) where T : Enum
        {
            return GetEnum<T>();
        }

        public static IEnumerable<T> GetFlags<T>(this T en) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
                if (en.HasFlag(value))
                    yield return value;
        }

        public static T NextEnum<T>(this T en) where T : Enum
        {
            // get values
            var valueList = GetEnum<T>().ToList();

            // get value index
            var index = valueList.IndexOf(en);

            // reset or increment index
            if (index < 0 || (index + 1) >= valueList.Count)
                index = 0;
            else
                index ++;

            return valueList[index];
        }

        public static void RemoveAllBut<T>(this List<T> source, Predicate<T> predicate)
        {
            source.RemoveAll(inverse);

            bool inverse(T item) => !predicate(item);
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        /*public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            //nothing to add
            if (other == null)
                return;

            foreach (var obj in other)
            {
                collection.Add(obj);
            }
        }*/
        [Conditional("UNITY_EDITOR")]
        public static void SetDirty(this Object obj)
        {
#if  UNITY_EDITOR
            EditorUtility.SetDirty(obj);
#endif
        }
    
        public static IEnumerable<Transform> GetChildren(this GameObject obj)
        {
            for (var n = 0; n < obj.transform.childCount; n++)
                yield return obj.transform.GetChild(n);
        }

        public static TComponent AddChild<TComponent>(this Transform transform, Action<TComponent> setup = null, string name = "") 
            where TComponent : Component
        {
            var go = new GameObject(name.IsNullOrEmpty() ? nameof(TComponent): name);
            go.SetActive(false);
            go.transform.SetParent(transform);
            
            var mb = go.AddComponent<TComponent>();
            setup?.Invoke(mb);

            go.SetActive(true);

            return mb;
        }

        public static void DestroyChildren(this GameObject obj)
        {
            var childList = obj.GetChildren().ToArray();

#if UNITY_EDITOR
            if (Application.isPlaying)
                foreach (var child in childList)
                    Object.Destroy(child.gameObject);
            else
                foreach (var child in childList)
                    Object.DestroyImmediate(child.gameObject);
#else
        foreach (var child in childList)
            UnityEngine.Object.Destroy(child.gameObject);
#endif
        }

        public static bool Has<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Any(predicate);
        }
    
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TSource noOptionsValue = default)
        {
            return source.MinBy(selector, Comparer<TKey>.Default, noOptionsValue);
        }
    
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, TSource noOptionsValue = default)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (sourceIterator.MoveNext() == false)
                    return noOptionsValue;

                var min = sourceIterator.Current;
                var minKey = selector(min);
	
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TSource noOptionsValue = default)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default, noOptionsValue);
        }
    
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, TSource noOptionsValue = default)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (sourceIterator.MoveNext() == false)
                    return noOptionsValue;

                var max = sourceIterator.Current;
                var maxKey = selector(max);
	
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        public static List<Transform> GetChildren(this Transform transform)
        {
            var result = new List<Transform>(transform.childCount);
            for (var n = 0; n < transform.childCount; n++)
                result.Add(transform.GetChild(n));

            return result;
        }

        public static void AddUnique<T>(this IList list, T item)
        {
            if (list.Contains(item))
                return;

            list.Add(item);
        }

        public static T RandomItem<T>(this IEnumerable<T> list)
        {
            return UnityRandom.RandomFromList(list.ToArray());
        }

        public static T RandomItem<T>(this IEnumerable<T> list, T noOptionsValue)
        {
            return UnityRandom.RandomFromList(list.ToArray(), noOptionsValue);
        }

        public static T RandomItem<T>(this IEnumerable<T> list, T noOptionsValue, params T[] except)
        {
            return UnityRandom.RandomFromList(list.ToArray(), noOptionsValue, except);
        }

        public static T RandomItem<T>(this IList<T> list)
        {
            return UnityRandom.RandomFromList(list);
        }

        public static T RandomItem<T>(this IList<T> list, T noOptionsValue)
        {
            return UnityRandom.RandomFromList(list, noOptionsValue);
        }

        public static T RandomItem<T>(this IList<T> list, T noOptionsValue, params T[] except)
        {
            return UnityRandom.RandomFromList(list, noOptionsValue, except);
        }
        
        public static T RandomItem<T>(this IList<T> list, params T[] except)
        {
            return UnityRandom.RandomFromList(list, default, except);
        }

        public static TList Randomize<TList>(this TList list)
            where TList : IList
        {
            UnityRandom.RandomizeList(list);
            return list;
        }
        
        public static List<T> Randomize<T>(this IEnumerable<T> enumerable)
        {
            var result = enumerable.ToList();
            UnityRandom.RandomizeList(result);
            return result;
        }

        public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
        {
            if (index < 0 || index >= list.Count)
            {
                value = default;
                return false;
            }

            value = list[index];
            return true;
        }
        
        public static T PrevItem<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index == -1 || index - 1 < 0)
                return default;

            return list[index - 1];
        }

        public static T NextItem<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index == -1 || list.Count <= index + 1)
                return default;

            return list[index + 1];
        }

        public static T NextItem<T>(this IList<T> list, T item, out int index)
        {
            index = list.IndexOf(item);
            if (index == -1 || list.Count <= ++index)
                return default;

            return list[index];
        }

        public static T NextItem<T>(this IList<T> list, ref int index)
        {
            if (list.Count <= ++index)
            {
                index = -1;
                return default;
            }

            return list[index];
        }
        
        public static IEnumerable<Vector2Int> ToEnumerableSquare(this Vector2Int vec)
        {
            for (var x = 0; x < vec.x; x++)
            for (var y = 0; y < vec.y; y++)
                yield return new Vector2Int(x, y);
        }

        public static Vector2Int RandomPoint(this Vector2Int vec)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, vec.x), UnityEngine.Random.Range(0, vec.y));
        }

        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector2 WithIncX(this Vector2 vector, float xInc)
        {
            return new Vector2(vector.x + xInc, vector.y);
        }

        public static Vector2 WithIncY(this Vector2 vector, float yInc)
        {
            return new Vector2(vector.x, vector.y + yInc);
        }
        
        public static Vector2 WithMulX(this Vector2 vector, float xMul)
        {
            return new Vector2(vector.x * xMul, vector.y);
        }

        public static Vector2 WithMulY(this Vector2 vector, float yMul)
        {
            return new Vector2(vector.x, vector.y * yMul);
        }

        public static Vector3 WithIncX(this Vector3 vector, float xInc)
        {
            return new Vector3(vector.x + xInc, vector.y, vector.z);
        }

        public static Vector3 WithIncY(this Vector3 vector, float yInc)
        {
            return new Vector3(vector.x, vector.y + yInc, vector.z);
        }

        public static Vector3 WithIncZ(this Vector3 vector, float zInc)
        {
            return new Vector3(vector.x, vector.y, vector.z + zInc);
        }
        
        public static Vector3 WithMulX(this Vector3 vector, float xMul)
        {
            return new Vector3(vector.x * xMul, vector.y, vector.z);
        }

        public static Vector3 WithMulY(this Vector3 vector, float yMul)
        {
            return new Vector3(vector.x, vector.y * yMul, vector.z);
        }

        public static Vector3 WithMulZ(this Vector3 vector, float zMul)
        {
            return new Vector3(vector.x, vector.y, vector.z * zMul);
        }

        public static Vector3 To3DXZ(this Vector2 vector, float y)
        {
            return new Vector3(vector.x, y, vector.y);
        }

        public static Vector3 To3DXZ(this Vector2 vector)
        {
            return vector.To3DXZ(0);
        }

        public static Vector3 To3DXY(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 To3DXY(this Vector2 vector)
        {
            return vector.To3DXY(0);
        }

        public static Vector3 To3DYZ(this Vector2 vector, float x)
        {
            return new Vector3(x, vector.x, vector.y);
        }

        public static Vector3 To3DYZ(this Vector2 vector)
        {
            return vector.To3DYZ(0);
        }

        public static Vector2 To2DXZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 To2DXY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 To2DYZ(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }

        public static Vector2 YX(this Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        public static Vector4 WithX(this Vector4 vector, float x)
        {
            return new Vector4(x, vector.y, vector.z, vector.w);
        }

        public static Vector4 WithY(this Vector4 vector, float y)
        {
            return new Vector4(vector.x, y, vector.z, vector.w);
        }
        
        public static Vector4 WithZ(this Vector4 vector, float z)
        {
            return new Vector4(vector.x, vector.y, z, vector.w);
        }
        
        public static Vector4 WithW(this Vector4 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }

        public static float Sum(this Vector2 vector)
        {
            return vector.x + vector.y;
        }

        public static float Sum(this Vector3 vector)
        {
            return vector.x + vector.y + vector.z;
        }
        public static bool IsZero(this Vector3 vector)
        {
            return vector == Vector3.zero;
        }

        public static Vector3 YZX(this Vector3 vector)
        {
            return new Vector3(vector.y, vector.z, vector.x);
        }

        public static Vector3 XZY(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.z, vector.y);
        }

        public static Vector3 ZXY(this Vector3 vector)
        {
            return new Vector3(vector.z, vector.x, vector.y);
        }

        public static Vector3 YXZ(this Vector3 vector)
        {
            return new Vector3(vector.y, vector.x, vector.z);
        }

        public static Vector3 ZYX(this Vector3 vector)
        {
            return new Vector3(vector.z, vector.y, vector.x);
        }

        public static Vector2 ReflectAboutX(this Vector2 vector)
        {
            return new Vector2(vector.x, -vector.y);
        }

        public static Vector2 ReflectAboutY(this Vector2 vector)
        {
            return new Vector2(-vector.x, vector.y);
        }
	
        public static Vector2 Rotate(this Vector2 vector, float angleInDeg)
        {
            float angleInRad = Mathf.Deg2Rad * angleInDeg;
            float cosAngle = Mathf.Cos(angleInRad);
            float sinAngle = Mathf.Sin(angleInRad);

            float x = vector.x * cosAngle - vector.y * sinAngle;
            float y = vector.x * sinAngle + vector.y * cosAngle;

            return new Vector2(x, y);
        }

        public static Vector2 RotateAround(this Vector2 vector, float angleInDeg, Vector2 axisPosition)
        {
            return (vector - axisPosition).Rotate(angleInDeg) + axisPosition;
        }

        public static Vector2 Rotate90(this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        public static Vector2 Rotate180(this Vector2 vector)
        {
            return new Vector2(-vector.x, -vector.y);
        }

        public static Vector2 Rotate270(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }

        /// <summary>
        /// Returns the vector rotated 90 degrees counter-clockwise.
        /// </summary>
        /// <remarks>
        /// 	<para>The returned vector is always perpendicular to the given vector. </para>
        /// 	<para>The perp dot product can be calculated using this: <c>var perpDotPorpduct = Vector2.Dot(v1.Perp(), v2);</c></para>
        /// </remarks>
        /// <param name="vector"></param>
        public static Vector2 Perp(this Vector2 vector)
        {
            return vector.Rotate90();
        }

        public static float Dot(this Vector2 vector1, Vector2 vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y;
        }

        public static float Dot(this Vector3 vector1, Vector3 vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
        }

        public static float Dot(this Vector4 vector1, Vector4 vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z + vector1.w * vector2.w;
        }

        /// <summary> Returns the projection of this vector onto the given base. </summary>
        public static Vector2 Proj(this Vector2 vector, Vector2 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary> Returns the rejection of this vector onto the given base. </summary>
        public static Vector2 Rej(this Vector2 vector, Vector2 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        public static Vector3 Proj(this Vector3 vector, Vector3 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector3.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary> Returns the rejection of this vector onto the given base. </summary>
        public static Vector3 Rej(this Vector3 vector, Vector3 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        /// <summary>
        /// Returns the projection of this vector onto the given base.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="baseVector"></param>
        public static Vector4 Proj(this Vector4 vector, Vector4 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }

        /// <summary>
        /// Returns the rejection of this vector onto the given base.
        /// The sum of a vector's projection and rejection on a base is
        /// equal to the original vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="baseVector"></param>
        public static Vector4 Rej(this Vector4 vector, Vector4 baseVector)
        {
            return vector - vector.Proj(baseVector);
        }

        /// <summary>
        /// Turns the vector 90 degrees anticlockwise as viewed from the top (keeping the y coordinate intact).
        /// Equivalent to <code>v.To2DXZ().Perp().To3DXZ(v.y);</code>
        /// </summary>
        public static Vector3 PerpXZ(this Vector3 v)
        {
            return new Vector3(-v.z, v.y, v.x);
        }

        /// <summary>
        /// Turns the vector 90 degrees anticlockwise as viewed from the front (keeping the z coordinate intact).
        /// Equivalent to <code>v.To2DXY().Perp().To3DXY(v.z);</code>
        /// </summary>
        public static Vector3 PerpXY(this Vector3 v)
        {
            return new Vector3(-v.y, v.x, v.z);
        }
    
        public static Vector2 HadamardMul(this Vector2 thisVector, Vector2 otherVector)
        {
            return new Vector2(thisVector.x * otherVector.x, thisVector.y * otherVector.y);
        }

        /// <summary>
        /// Divides one vector component by component by another.
        /// </summary>
        public static Vector2 HadamardDiv(this Vector2 thisVector, Vector2 otherVector)
        {
            return new Vector2(thisVector.x / otherVector.x, thisVector.y / otherVector.y);
        }
    
        public static Vector3 HadamardMul(this Vector3 thisVector, Vector3 otherVector)
        {
            return new Vector3(
                thisVector.x * otherVector.x, 
                thisVector.y * otherVector.y,
                thisVector.z * otherVector.z);
        }
    
        public static Vector3 HadamardDiv(this Vector3 thisVector, Vector3 otherVector)
        {
            return new Vector3(
                thisVector.x / otherVector.x, 
                thisVector.y / otherVector.y,
                thisVector.z / otherVector.z);
        }
    
        public static Vector4 HadamardMul(this Vector4 thisVector, Vector4 otherVector)
        {
            return new Vector4(
                thisVector.x * otherVector.x,
                thisVector.y * otherVector.y,
                thisVector.z * otherVector.z,
                thisVector.w * otherVector.w);
        }
        public static Vector4 HadamardDiv(this Vector4 thisVector, Vector4 otherVector)
        {
            return new Vector4(
                thisVector.x / otherVector.x,
                thisVector.y / otherVector.y,
                thisVector.z / otherVector.z,
                thisVector.w / otherVector.w);
        }

			
        public static Vector2Int To2DXY(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.y);
        }
        public static Vector3 ToVector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }


        public static Vector2 ToVector2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3Int To3DXY(this Vector2Int v)
        {
            return new Vector3Int(v.x, v.y, 0);
        }
        public static Vector3Int To3DXZ(this Vector2Int v)
        {
            return new Vector3Int(v.x, 0, v.y);
        }
        
        public static int Hash(this Vector2Int v)
        {
            return (v.x << 16) | v.y;
        }


        public static void SetMax(this Vector2Int v, Vector2Int max)
        {
            v.Set(v.x > max.x ? max.x : v.x,
                v.y > max.y ? max.y : v.y);
        }

        public static void SetMin(this Vector2Int v, Vector2Int min)
        {
            v.Set(v.x < min.x ? min.x : v.x, 
                v.y < min.y ? min.y : v.y);
        }

        public static Vector2Int Center(this Vector2Int v)
        {
            return new Vector2Int(v.x / 2, v.y / 2);
        }

        public static Vector2Int CenterRound(this Vector2Int v)
        {
            return new Vector2Int(Mathf.RoundToInt((float)v.x / 2.0f), Mathf.RoundToInt((float)v.y / 2.0f));
        }

        /// <summary>From min inclusive, to max exclusive </summary>
        public static bool InRange(this Vector2Int v, Vector2Int min, Vector2Int max)
        {
            return v.x >= min.x && v.y >= min.y 
                                 && v.x < max.x && v.y < max.y;
        }

        /// <summary>From 0 to max exclusive </summary>
        public static bool InRange(this Vector2Int v, Vector2Int max)
        {
            return v.x >= 0 && v.y >= 0 
                            && v.x < max.x && v.y < max.y;
        }
        /// <summary>From 0 to max exclusive </summary>
        public static bool InRange(this Vector2Int v, int maxX, int maxY)
        {
            return v.x >=0 && v.x < maxX 
                           && v.y >=0 && v.y < maxY;
        }

        /// <summary>From 0 to max exclusive </summary>
        public static bool InRange(this Vector2Int v, int max)
        {
            return v.x >=0 && v.x < max 
                           && v.y >=0 && v.y < max;
        }

        /// <summary>From zero to max, exclusive </summary>
        public static bool Contains(this Vector2Int v, Vector2Int point)
        {
            return point.x >= 0 && point.y >= 0
                                && point.x < v.x && point.y < v.y;
        }
	
        /// <summary>Max value </summary>
        public static int Max(this Vector2Int v)
        {
            return v.x > v.y ? v.x : v.y;
        }

        // min value
        public static int Min(this Vector2Int v)
        {
            return v.x < v.y ? v.x : v.y;
        }
	
        // true if this vector in bounds(inclusive min max) of argument vector, none argument check
        public static bool InRangeOfInc(this Vector2 v, Vector2 range)
        {
            return v.x >= range.x && v.y <= range.y;
            // && v.x <= range.y && v.y >= range.x
        }

        public static bool InRangeOfInc(this Vector2 v, float pos)
        {
            return v.x <= pos && pos <= v.y;
        }

        public static float ClosesdValue(this Vector2 v, float pos)
        {
            return Mathf.Abs(v.x - pos) < Mathf.Abs(v.y - pos) ? v.x : v.y;
        }

        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y);
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(v.x < 0.0f ? -v.x : v.x, v.y < 0.0f ? -v.y : v.y, v.z < 0.0f ? -v.z : v.z);
        }

        public static Vector2 ClampLenght(this Vector2 v, float lenghtAbs)
        {
            var lenght = v.magnitude;
            if (lenght > lenghtAbs)
                v *= lenghtAbs / lenght;

            return v;
        }
        public static Vector2 ToVector2X(this float value)
        {
            return new Vector2(value, 0);
        }
        public static Vector2 ToVector2Y(this float value)
        {
            return new Vector2(0, value);
        }
        public static Vector2 ToVector2(this float value)
        {
            return new Vector2(value, value);
        }
    
        public static Vector3 ToVector3X(this float value)
        {
            return new Vector3(value, 0, 0);
        }
        public static Vector3 ToVector3Y(this float value)
        {
            return new Vector3(0, value, 0);
        }
        public static Vector3 ToVector3Z(this float value)
        {
            return new Vector3(0, 0, value);
        }
        public static Vector3 ToVector3(this float value)
        {
            return new Vector3(value, value, value);
        }
        
        public static float AngleDeg(this Vector2 v)
        {
            return (Mathf.Atan2(v.y, v.x)) * Mathf.Rad2Deg;
        }
        public static float AngleRad(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x);
        }

        public static Vector2Int Round(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        public static Vector2Int Ceil(this Vector2 v)
        {
            return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }
        public static Vector2Int Floor(this Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }

        /// <summary> 8 king neighborhood </summary>
        public static IEnumerable<Vector2Int> MooreNeighborhood(this Vector2Int cell)
        {
            foreach (var neighbour in VonNeumannNeighborhood(cell))
                yield return neighbour;

            yield return new Vector2Int(cell.x - 1, cell.y + 1);
            yield return new Vector2Int(cell.x - 1, cell.y - 1);
            yield return new Vector2Int(cell.x + 1, cell.y + 1);
            yield return new Vector2Int(cell.x + 1, cell.y - 1);
        }

        /// <summary> 4 cross neighbors </summary>
        public static IEnumerable<Vector2Int> VonNeumannNeighborhood(this Vector2Int cell)
        {
            yield return new Vector2Int(cell.x - 1, cell.y);
            yield return new Vector2Int(cell.x + 1, cell.y);
            yield return new Vector2Int(cell.x, cell.y + 1);
            yield return new Vector2Int(cell.x, cell.y - 1);
        }

        public static Vector2 Clamp(this Vector2 v, float min, float max)
        {
            return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        public static Vector3 Clamp(this Vector3 v, float min, float max)
        {
            return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }

        public static Vector2Int Clamp(this Vector2Int v, int min, int max)
        {
            return new Vector2Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
        }

        public static Vector3Int Clamp(this Vector3Int v, int min, int max)
        {
            return new Vector3Int(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }
	
        public static Vector2 Normal(this float rad)
        {
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public static bool IsAproximatlyZero(this float val)
        {
            return Mathf.Approximately(val, 0.0f);
        }
        public static bool Aproximatly(this float val, float equal)
        {
            return Mathf.Approximately(val, equal);
        }

        public static IEnumerable<int> BitPositions(this int val, bool positiveBits = true)
        {
            var bit = 1;
            for (var n = 0; n < 32; n++)
            {
                if (((val & bit) != 0) == positiveBits)
                    yield return n;

                bit <<= 1;
            }
        }

        public static int ToInt(this Vector2Int v)
        {
            //return unchecked(v.x | (v.y << 15));
            return v.x | (v.y << 16);
        }

        public static Vector2Int ToVector2Int(this int v)
        {
            return new Vector2Int(v & 0b0000_0000_0000_0000_1111_1111_1111_1111, v >> 16);
        }

        public static int Sum(this Vector2Int v)
        {
            return v.x + v.y;
        }

        public static int SumAbs(this Vector2Int v)
        {
            return Mathf.Abs(v.x) + Mathf.Abs(v.y);
        }

        public static float Evaluate(this ParticleSystem.MinMaxCurve curve)
        {
            return curve.Evaluate(UnityEngine.Random.value, UnityEngine.Random.value);
        }

        public static float FinishTime(this AnimationCurve curve)
        {
            return curve.keys[curve.length - 1].time;
        }

        public static float StartTime(this AnimationCurve curve)
        {
            return curve.keys[0].time;
        }

        public static float Duration(this AnimationCurve curve)
        {
            return curve.FinishTime() - curve.StartTime();
        }

        public static Color WithA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color IncA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, color.a + a);
        }

        public static Color MulA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, color.a * a);
        }
        
        public static double Clamp01(this double d)
        {
            return d.Clamp(0d, 1d);
        }

        public static double Clamp(this double d, double min, double max)
        {
            if (d < min)
                return min;
            if (d > max)
                return max;

            return d;
        }

        public static float Clamp01(this float f)
        {
            return Mathf.Clamp01(f);
        }

        public static float OneMinus(this float f)
        {
            return 1f - f;
        }
        
        public static float Clamp(this float f, float min, float max)
        {
            return Mathf.Clamp(f, min, max);
        }

        public static float ClampMax(this float f, float max)
        {
            return f > max ? max : f;
        }
        
        public static float ClampMin(this float f, float min)
        {
            return f < min ? min : f;
        }
        
        public static float Max(this float f, float max)
        {
            return f > max ? f : max;
        }

        public static float Positive(this float f)
        {
            return f.Max(0f);
        }
        public static float Negative(this float f)
        {
            return f.Min(0f);
        }

        public static float Min(this float f, float min)
        {
            return f < min ? f : min;
        }

        public static float Abs(this float f)
        {
            return f < 0 ? -f : f;
        }
        
        public static float Round(this float f)
        {
            return Mathf.Round(f);
        }

        public static int RoundToInt(this float f)
        {
            return Mathf.RoundToInt(f);
        }
        
        public static float Floor(this float f)
        {
            return Mathf.Floor(f);
        }

        public static int FloorToInt(this float f)
        {
            return Mathf.FloorToInt(f);
        }
        
        public static float Ceil(this float f)
        {
            return Mathf.Ceil(f);
        }

        public static int CeilToInt(this float f)
        {
            return Mathf.CeilToInt(f);
        }

        public static bool Chance(this float f)
        {
            return UnityRandom.Bool(f);
        }

        public static float Amplitude(this float f)
        {
            var half = f * 0.5f;
            return UnityEngine.Random.Range(-half, half);
        }
        
        public static float Range(this float f)
        {
            return f > 0f ? UnityEngine.Random.Range(0f, f) : UnityEngine.Random.Range(f, 0f);
        }

        public static bool IsEven(this int number)
        {
            return (number & 0x1) == 0x0;
        }

        public static bool IsOdd(this int number)
        {
            return (number & 0x1) == 0x1;
        }

        public static int Clamp(this int number, int max, int min)
        {
            return Mathf.Clamp(number, min, max);
        }
        
        public static int ClampMin(this int number, int min)
        {
            return number < min ? min : number;
        }

        public static int ClampMax(this int number, int max)
        {
            return number > max ? max : number;
        }
    }

    public static class Actions
    {
        public static void Empty() { }
        public static void Empty<T>(T value) { }
        public static void Empty<T1, T2>(T1 value1, T2 value2) { }
    }

    public static class Functions
    {
        public static T Identity<T>(T value) { return value; }

        public static T Default<T>() { return default; }

        public static bool IsNull<T>(T entity) where T : class { return entity == null; }
        public static bool IsNonNull<T>(T entity) where T : class { return entity != null; }

        public static bool True<T>(T entity) { return true; }
        public static bool False<T>(T entity) { return false; }
    }
}