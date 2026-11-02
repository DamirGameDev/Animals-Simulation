using System;
using System.Collections.Generic;

namespace Modules.SharedModule.Runtime.Scripts.Repository
{
    public abstract class IndexRepository<TKey, TValue> : IDisposable
    {
        public IReadOnlyDictionary<TKey, TValue> ValueByKey => _valueByKey;

        public event Action<TKey, TValue> Added, Removed;

        private readonly Dictionary<TKey, TValue> _valueByKey = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TValue, TKey> _keyByValue = new Dictionary<TValue, TKey>();

        public virtual void Dispose()
        {
            _valueByKey.Clear();
            _keyByValue.Clear();
        }

        public void Add(TKey key, TValue value)
        {
            if (!_valueByKey.TryAdd(key, value))
            {
                throw new ArgumentException(
                    $"An item with the same key has already been added. key = {key} value = {value}");
            }

            _keyByValue.Add(value, key);

            Added?.Invoke(key, value);
        }

        public bool RemoveByValue(TValue value)
        {
            if (!_keyByValue.Remove(value, out var key))
                return false;

            _valueByKey.Remove(key);
            Removed?.Invoke(key, value);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value) => _valueByKey.TryGetValue(key, out value);
    }
}
