using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LibICAP.Utilities
{
    static class Extensions
    {
        /// <summary>
        /// Creates a subarray
        /// </summary>
        /// <typeparam name="T">Any type of array</typeparam>
        /// <param name="data">Array filled with content</param>
        /// <param name="index">Positive number from where to start the split operation</param>
        /// <param name="length">Length of the resulting array</param>
        /// <returns>Splitted array</returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Writes an array to file
        /// </summary>
        /// <param name="data">Array filled with content</param>
        /// <param name="path">Path where file should get saved to</param>
        /// <param name="delete">Delete the file prior saving it (if exist, delete)</param>
        public static void WriteToFile(this byte[] data, string path, bool delete = true)
        {
            if (File.Exists(path) && delete) File.Delete(path);
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

            fileStream.Write(data, 0, data.Length);
            fileStream.Flush();
            fileStream.Close();
        }

        /// <summary>
        /// Decompresses a gzip binary array
        /// </summary>
        /// <param name="data">Array filled with binary content</param>
        /// <returns>Decompressed bytearray</returns>
        public static byte[] DecompressGzipStream(this byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();

            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }

        /// <summary>
        /// Get text with the UTF8 encoding
        /// </summary>
        /// <param name="data">Bytearray containing UTF8 text</param>
        /// <returns>Decoded text</returns>
        public static string GetUTF8TextFromByteArray(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// Get text with the ASCII encoding
        /// </summary>
        /// <param name="data">Bytearray containing ASCII text</param>
        /// <returns>Decoded text</returns>
        public static string GetASCIITextFromByteArray(this byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
        /// <summary>
        /// Searches a needle in a binary byte haystack
        /// </summary>
        /// <param name="haystack">Bytearray / Haystack</param>
        /// <param name="needle">Search pattern / Needle</param>
        /// <returns>Position of the search pattern or -1 if not found</returns>
        public static int Search(this byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (Match(haystack, needle, i))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Searches a needle in a binary byte haystack
        /// </summary>
        /// <param name="haystack">Bytearray / Haystack</param>
        /// <param name="needle">Search pattern / Needle</param>
        /// <returns>True if found or false if search pattern is not in the haystack</returns>
        public static bool SearchBool(this byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (Match(haystack, needle, i))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Match(byte[] haystack, byte[] needle, int start)
        {
            if (needle.Length + start > haystack.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (needle[i] != haystack[i + start])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Removes first n lines from a string. Removes one line from the beginning by default
        /// </summary>
        /// <param name="text">Text from which the first lines should get removed</param>
        /// <param name="linesCount">Quantity of lines that should get removed</param>
        /// <returns>Reduced string</returns>
        public static string RemoveFirstLines(this string text, int linesCount = 1)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }


        private static bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Separates a byte array into smaller byte arrays by a defined seperator
        /// </summary>
        /// <param name="source">source byte array to split</param>
        /// <param name="separator">needle to search for</param>
        /// <returns>multidimensional, splitted byte array</returns>
        public static byte[][] SeparateBefore(this byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        /// <summary>
        /// Separates a byte array into smaller byte arrays by a defined seperator
        /// </summary>
        /// <param name="source">source byte array to split</param>
        /// <param name="separator">needle to search for</param>
        /// <returns>multidimensional, splitted byte array</returns>
        public static byte[][] SeparateAfter(this byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;

            for (var i = 0; i < source.Length; ++i)
            {
                if (Equals(source, separator, i))
                {
                    Part = new byte[i - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = i + separator.Length;
                    i += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        public static byte[] CombineWith(this byte[] source1, byte[] source2)
        {
            return source1.Union(source2).ToArray();
        }

        public static byte[] ConcatArrays(params byte[][] args)
        {
            if (args == null)
                throw new ArgumentNullException();

            var offset = 0;
            var newLength = args.Sum(arr => arr.Length);
            var newArray = new byte[newLength];

            foreach (var arr in args)
            {
                Buffer.BlockCopy(arr, 0, newArray, offset, arr.Length);
                offset += arr.Length;
            }

            return newArray;
        }
        /// <summary>
        /// Removes trailing zeros from a byte array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static byte[] TrimTailingZeros(this byte[] arr)
        {
            if (arr == null || arr.Length == 0)
                return arr;
            return arr.Reverse().SkipWhile(x => x == 0).Reverse().ToArray();
        }
    }
}
