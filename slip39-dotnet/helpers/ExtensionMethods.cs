using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static slip39_dotnet.helpers.BitArrayOperations;

namespace slip39_dotnet.helpers
{
    static internal class ExtensionMethods
    {
        public static T[] TakeLast<T>(this T[] source, int n)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (n > source.Length)
                throw new ArgumentOutOfRangeException(nameof(n), "Can't be bigger than the array");
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), "Can't be negative");

            var target = new T[n];
            Array.Copy(source, source.Length - n, target, 0, n);
            return target;
        }


        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static byte[] XOR(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("arr1 and arr2 are not the same length");

            byte[] result = new byte[arr1.Length];

            for (int i = 0; i < arr1.Length; ++i)
                result[i] = (byte)(arr1[i] ^ arr2[i]);

            return result;
        }

        public static byte[] Append(this byte[] arr1, byte[] arr2)
        {
            byte[] result = new byte[arr1.Length + arr2.Length];

            for (int i = 0; i < arr1.Length; ++i)
            {
                result[i] = (arr1[i]);
            }
            for (int j = arr1.Length; j < arr1.Length + arr2.Length; j++)
            {
                result[j] = arr2[j - arr1.Length];
            }

            return result;
        }

        public static IEnumerable<int> Append(this List<int> list1, IEnumerable<int> list2)
        {
            foreach(var item in list2)
            {
                list1.Add(item);
            }
            return list1;
        }

        public static BitArray Prepend(this BitArray current, BitArray before)
        {
            var bools = new bool[current.Count + before.Count];
            before.CopyTo(bools, 0);
            current.CopyTo(bools, before.Count);
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static void Reverse(this BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public static IEnumerable<int> Split(this BitArray array, int size = 10)
        {
            if (array.Count % size != 0) throw new ArgumentException(nameof(array), $"Length of variable {nameof(array)} is {array.Count}, it should be a multiple of {size}.");
            var result = new List<BitArray>();
            for(int i = 0; i < array.Count / size; i++) 
            {
                result.Add(new BitArray(size));
            }
            for(int j = 0; j < array.Count; j++)
            {
                result[j / size][j % size] = array[j];
            }
            foreach(var element in result)
            {
                yield return getIntFromBitArray(element);
            }
            
        }
    }
}
