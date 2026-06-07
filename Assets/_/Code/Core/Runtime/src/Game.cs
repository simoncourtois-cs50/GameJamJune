using System.Collections.Generic;

namespace Core.Runtime
{
    public static class Game
    {
        private readonly static Dictionary<string, object> _facts =  new();

        public static void SetFact<T>(string key, T value) => _facts[key] = value;
        public static T GetFact<T>(string key, T fallback = default) => 
            _facts.TryGetValue(key, out var value)  && value is T typedValue ? typedValue : fallback;

        public static bool HasFact(string key) => _facts.ContainsKey(key);
        public static bool RemoveFact(string key) => _facts.Remove(key);
        public static void ClearFacts() => _facts.Clear();
    }
}
