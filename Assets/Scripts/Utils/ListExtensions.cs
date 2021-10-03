using System;
using System.Collections.Generic;
using System.Linq;

namespace Wizard.Utils
{
    public static class ListExtensions
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        private static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }

        /// <summary>
        /// Shuffles the list using random permutations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int index = list.Count;
            while (index > 0)
            {
                index--;
                int randomIndex = RandomNumber(0, list.Count);
                (list[randomIndex], list[index]) = (list[index], list[randomIndex]);
            }

            return list;
        }

        public static List<T> ShufflePartly<T>(this List<T> list, int min, int max)
        {
            int index = max;
            while (index > min)
            {
                index--;
                int randomIndex = RandomNumber(min, max);
                (list[randomIndex], list[index]) = (list[index], list[randomIndex]);
            }

            return list;
        }

        /// <summary>
        /// Adds the same value multpile times
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="times"></param>
        public static void AddMany<T>(this List<T> list, T item, int times)
        {
            for (int i = 0; i < times; i++)
            {
                list.Add(item);
            }
        }

        public static List<T> TakeRandom<T>(this List<T> list, int count)
        {
            list.Shuffle();
            return list.Take(count).ToList();
        }

        public static T TakeRandom<T>(this List<T> list, params T[] exclude)
        {
            List<T> copy = new List<T>(list);

            foreach (var e in exclude)
            {
                copy.RemoveAll(c => c.Equals(e));
            }

            copy.Shuffle();
            copy.Shuffle();
            return copy[0];
        }

        public static List<T> TakeRandom<T>(this List<T> list, int amount, params T[] exclude)
        {
            List<T> copy = new List<T>(list);

            foreach (var e in exclude)
            {
                copy.RemoveAll(c => c.Equals(e));
            }

            copy.Shuffle();

            return copy.Take(amount).ToList();
        }
    }
}