
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AI
{

    public static class AIExtensions
    {

        public static Vector3 GetCenter(this IEnumerable<Transform> transforms)
        {
            return transforms.Select(t => t.position).GetCenter();
        }

        public static Vector3 GetCenter(this IEnumerable<Vector3> vecs)
        {
            Vector3 posSum = Vector3.zero;
            int count = 0;
            foreach (var v in vecs)
            {
                posSum += v;
                count++;
            }
            return posSum / count;
        }
		
        public static bool HasInRange(this Vector3 pos, Vector3 targetPos, float range)
        {
            return (pos - targetPos).sqrMagnitude < range * range;
        }
		
        static public T GetRandom<T>(this IEnumerable<T> self)
        {
            return self.ElementAtOrDefault(UnityEngine.Random.Range(0, self.Count()));
        }
		
        static public IEnumerable<List<T>> GetAllSubsets<T>(this IList<T> all)
        {
            int allCount = all.Count();
            int subsetCount = 1 << allCount;
            for (int i = subsetCount-1; i>=0; i--) //roughly getting answers starting from bigger sets then smaller sets
            {
                var subset = new List<T>();
                for (int bitIndex = 0; bitIndex < allCount; bitIndex++)
                {
                    if (GetBit(i, bitIndex) == 1)
                    {
                        subset.Add(all[bitIndex]);
                    }
                }
                yield return subset;
            }
        }

        private static int GetBit(int value, int bitIndex)
        {
            int bit = value & 1 << bitIndex;
            return bit > 0 ? 1 : 0;
        }
    }
}

public static class GlobalExtensions
{
    public static Boolean IsEmptyOrNull<T>(this IEnumerable<T> source)
    {
        if (source == null)
            return true;
        return !source.Any();
    }

    public static T[] RemoveNulls<T>(this T[] source) where T : class 
    {
        int nulls = 0;
        foreach (var VARIABLE in source)
            if (VARIABLE == null)
                nulls++;
        if (nulls == 0) return source;
        return source.Where(t => t != null).ToArray();
    }
}