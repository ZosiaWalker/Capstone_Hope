using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class DictionarySerializable<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        private List<KeyValueSerializable<TKey, TValue>> items = new List<KeyValueSerializable<TKey, TValue>>();

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> Dictionary
        {
            get { return dictionary; }
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                    var item = items.FirstOrDefault(pair => pair.Key.Equals(key));
                    if (item != null)
                    {
                        items.Remove(item);
                    }
                }
                else
                {
                    dictionary.Add(key, value);
                }
                items.Add(new KeyValueSerializable<TKey, TValue>(key, value));
            }
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;

        public DictionarySerializable<TKey, TValue> Clone()
        {
            DictionarySerializable<TKey, TValue> clone = new DictionarySerializable<TKey, TValue>();

            foreach (var kvp in this)
                clone[kvp.Key] = kvp.Value;

            return clone;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Key != null)
                {
                    dictionary[items[i].Key] = items[i].Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
