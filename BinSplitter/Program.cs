using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinSplitter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // 分割するファイルのパスです。対象ファイルが存在しない場合は以降の処理は行いません。
            var fileInfo = new FileInfo("target");
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"{fileInfo.FullName} は存在しません");
                return;
            }

            new DirectoryInfo("output").Create();

            var targetFilePath = fileInfo.FullName;
            var targetBytes = ReadFileAsBytes(targetFilePath);

            // 分割するパターンです。このパターンが先頭に来るように配列が分割されます。
            var pattern = new byte[] { 0x7B, };
            var splitAddresses = FindPatternAddresses(targetBytes, pattern);

            var sp = SplitBytesByHeaders(targetBytes, splitAddresses);
            for (var i = 0; i < sp.Count; i++)
            {
                var p = $@"output\{i:D5}.temp";
                WriteBytesToFile(p, sp[i]);
            }
        }

        public static long[] FindPatternAddresses(IReadOnlyList<byte> bytes, IReadOnlyCollection<byte> pattern)
        {
            var addresses = new List<long>();

            for (var i = 0; i <= bytes.Count - pattern.Count; i++)
            {
                var found = !pattern.Where((t, j) => bytes[i + j] != t).Any();

                if (found)
                {
                    addresses.Add(i);
                }
            }

            return addresses.ToArray();
        }

        public static List<byte[]> SplitBytesByHeaders(byte[] bytes, IReadOnlyList<long> headers)
        {
            var resultList = new List<byte[]>();

            for (var i = 0; i < headers.Count; i++)
            {
                var headerPos = headers[i];
                var length = i == headers.Count - 1 ? bytes.Length - (int)headerPos : (int)(headers[i + 1] - headerPos);
                var section = new byte[length];
                Array.Copy(bytes, headerPos, section, 0, length);
                resultList.Add(section);
            }

            return resultList;
        }

        private static void WriteBytesToFile(string path, byte[] bytes)
        {
            try
            {
                // 指定されたパスにバイナリファイルを作成し、バイトデータを書き込む
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ファイルの書き込み中にエラーが発生しました: " + ex.Message);
            }
        }

        private static byte[] ReadFileAsBytes(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllBytes(filePath); // 指定されたファイルをバイト配列として読み込む
                }

                Console.WriteLine("ファイルが存在しません。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ファイルの読み込み中にエラーが発生しました: " + ex.Message);
            }

            return null;
        }
    }
}