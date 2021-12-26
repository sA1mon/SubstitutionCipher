using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SubstitutionCipher
{
    public class Bigram : IEnumerable<KeyValuePair<string, double>>, ICloneable
    {
        private readonly Dictionary<string, double> _bigram;

        public Bigram()
        {
            _bigram = new Dictionary<string, double>();
        }

        public void Add(string bigram)
        {
            if (!_bigram.ContainsKey(bigram))
            {
                _bigram[bigram] = 0;
            }

            _bigram[bigram]++;
        }

        public void Fill(string text)
        {
            for (var i = 0; i < text.Length - 1; i++)
            {
                var j = i + 1;
                Add($"{text[i]}{text[j]}");
            }
        }

        public void Transform(int length)
        {
            var keys = _bigram.Keys.ToArray();

            foreach (var bigramKey in keys)
            {
                _bigram[bigramKey] = _bigram[bigramKey] / (length - 1) * 100;
            }
        }

        public double Get(string key)
        {
            return !_bigram.ContainsKey(key) ? 0.0 : _bigram[key];
        }

        public void Set(IDictionary<string, double> d)
        {
            foreach (var d1 in d)
            {
                _bigram[d1.Key] = d1.Value;
            }
        }

        public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, double>>) _bigram).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            return _bigram.ToDictionary(bigram => bigram.Key, bigram => bigram.Value);
        }
    }
}