using System;
using System.Collections.Generic;

namespace WorldInterface
{
    public class Blackboard
    {
        private Dictionary<string, object> _database = new();

        public void AddItem<T>(string key, T item)
        {
            if (_database.ContainsKey(key) && _database[key].GetType() != item.GetType())
            {
                throw new Exception();
            }

            _database[key] = item;
        }

        public void ClearItem(string key)
        {
            if (!_database.ContainsKey(key))
            {
                return;
            }

            _database.Remove(key);
        }

        public T GetItem<T>(string key)
        {
            if (!_database.ContainsKey(key))
            {
                throw new Exception();
            }

            if (_database[key].GetType() != typeof(T))
            {
                throw new Exception();
            }

            return (T)_database[key];
        }
        
        public bool HasValue(string key)
        {
            return _database.ContainsKey(key);
        }

        public bool HasValue<T>(string key)
        {
            return _database.ContainsKey(key) && _database[key].GetType() == typeof(T);
        }
    }
}