using System;

namespace DialogueSystem
{
    [Serializable]
    public class KeyValueSerializable<T, K>
    {
        public T Key;
        public K Value;

        public KeyValueSerializable() {}

        public KeyValueSerializable(T key, K value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
