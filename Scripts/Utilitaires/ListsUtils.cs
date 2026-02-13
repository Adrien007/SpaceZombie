//ListsUtils.cs
using System;
using System.Collections.Generic;

namespace SpaceZombie.Utilitaires
{
    public static class ListsUtils
    {
        /// <summary>
        /// swap-remove (no shifting) O(1)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index">indexToRemove</param>
        public static void RemoveAtSwapBack<T>(List<T> list, int index)
        {
            int last = list.Count - 1;
            list[index] = list[last];
            list.RemoveAt(last);
        }

        /// <summary>
        /// Fisher-Yates partial shuffle (O(X), no full shuffle needed):
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="count">Number of element to select</param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static List<T> PickRandom<T>(List<T> list, int count, Random rng)
        {
            int n = list.Count;
            count = Math.Min(count, n);
            for (int i = 0; i < count; i++)
            {
                int j = rng.Next(i, n);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list.GetRange(0, count);
        }
    }
}