using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Array = System.Array;

namespace SubstitutionCipher
{
    class Program
    {
        
        static void Main()
        {
            var text = TextReader.ReadWithoutSpecials("war.txt");
            var bigramms = new Bigram();
            var code = TextReader.ReadWithoutSpecials("text.txt");

            bigramms.Fill(text);
            bigramms.Transform(text.Length);
            var res = Solve(code, bigramms);

            Console.WriteLine(res);
        }

        private static string Solve(string text, Bigram standard)
        {
            var alphabet = Utils.GetAlphabet();
            var freq = new double[32];
            foreach (var ch in text)
            {
                freq[ch - 'а']++;
            }

            for (var i = 0; i < freq.Length; i++)
            {
                freq[i] = (freq[i] / text.Length) * 100;
            }

            var sortedFreq = new double[32];
            Array.Copy(freq, sortedFreq, freq.Length);
            Array.Sort(sortedFreq);

            var key = new List<char>();

            foreach (var t in sortedFreq)
            {
                var symb = alphabet[GetIndexOf(freq, t)];

                if (!key.Contains(symb))
                {
                    key.Add(symb);
                }
                else
                {
                    for (var j = 0; j < freq.Length; j++)
                    {
                        if (Math.Abs(freq[j] - t) < 1e-7)
                        {
                            var s = alphabet[j];
                            if (!key.Contains(s))
                            {
                                key.Add(s);
                                break;
                            }
                        }
                    }
                }
            }

            key.Reverse();
            Console.WriteLine($"Key: {string.Join(string.Empty, key)}");
            var txt = Decrypt(text, key);
            var bigram = new Bigram();

            bigram.Fill(txt);
            bigram.Transform(txt.Length);

            var bigramRating = bigram.Sum(b => 
                Math.Pow(b.Value - standard.Get(b.Key), 2));

            var prevBigramRating = bigramRating;
            var step = 1;

            while (true)
            {
                var goodBigramFound = false;

                for (var i = 0; i < alphabet.Length; i++)
                {
                    if (i + step >= alphabet.Length)
                        break;

                    var temp = key[i];
                    key[i] = key[i + step];
                    key[i + step] = temp;

                    var copyBigramFreq = bigram.Clone() as Dictionary<string, double>;

                    txt = Decrypt(text, key);
                    bigram = new Bigram();

                    bigram.Fill(txt);
                    bigram.Transform(txt.Length);

                    bigramRating = bigram.Sum(b => 
                        Math.Pow(b.Value - standard.Get(b.Key), 2));

                    if (bigramRating < prevBigramRating)
                    {
                        goodBigramFound = true;
                        prevBigramRating = bigramRating;
                        step = 1;
                        break;
                    }

                    temp = key[i];
                    key[i] = key[i + step];
                    key[i + step] = temp;

                    bigram.Set(copyBigramFreq);
                }

                if (!goodBigramFound)
                {
                    step++;
                    if (step >= alphabet.Length - 1)
                        break;
                }
            }

            Console.WriteLine($"Key: {string.Join(string.Empty, key)}");
            var res = Decrypt(text, key);

            return res;
        }

        private static string Decrypt(string text, IList<char> key)
        {
            var result = new StringBuilder();
            var alphabet = Utils.GetAlphabet();

            foreach (var ch in text)
            {
                result.Append(alphabet[key.IndexOf(ch)]);
            }

            return result.ToString();
        }

        private static int GetIndexOf(IReadOnlyList<double> arr, double elem)
        {
            int i;
            for (i = 0; i < arr.Count; i++)
            {
                if (Math.Abs(arr[i] - elem) < 1e-7)
                    return i;
            }

            return -1;
        }
    }
}
