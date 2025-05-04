using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace OneDay.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetRandomExcluding<T>(this IEnumerable<T> source, T except)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var filteredList = source.Where(item => !except.Equals(item)).ToList();

            if (!filteredList.Any())
                throw new InvalidOperationException("No valid items to select from.");

            return filteredList[UnityEngine.Random.Range(0, filteredList.Count)];
        }
        
        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var random = new Random();
            int count = source.Count();
            return source.ElementAt(random.Next(count));
        }
        
        public static T GetRandomWithProbabilities<T>(this IEnumerable<T> source, List<float> probabilities)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var random = new Random();
            var sourceList = source.ToList();
            if (sourceList.Count != probabilities.Count)
                throw new ArgumentException("Count of probabilities must match count of items in the list");

            float total = probabilities.Sum();
            float rnd = (float)(random.NextDouble() * total);

            float cumulative = 0f;
            for (int i = 0; i < probabilities.Count; i++)
            {
                cumulative += probabilities[i];
                if (rnd < cumulative)
                    return sourceList[i];
            }

            throw new InvalidOperationException("Random selection failed due to probability mismatch.");
        }
        
        public static List<T> GetRandomized<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var sourceList = source.ToList();
            var randomizedList = new List<T>();

            while (sourceList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, sourceList.Count);
                randomizedList.Add(sourceList[index]);
                sourceList.RemoveAt(index);
            }

            return randomizedList;
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Func<T, UniTask> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            int index = 0;
            foreach (var item in source)
            {
                action(index,item);
                index++;
            }
        }
    }
}