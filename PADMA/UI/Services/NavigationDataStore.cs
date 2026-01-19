using System.Collections.Concurrent;

namespace PADMA.UI.Services
{
    /// <summary>
    /// In-memory store for navigation payloads (avoid passing heavy objects via Shell query).
    /// </summary>
    public sealed class NavigationDataStore
    {
        private readonly ConcurrentDictionary<string, object> _items = new();

        public string Put<T>(T value) where T : notnull
        {
            var token = Guid.NewGuid().ToString("N");
            _items[token] = value;
            return token;
        }

        public bool TryGet<T>(string token, out T? value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(token)) return false;

            if (_items.TryGetValue(token, out var obj) && obj is T typed)
            {
                value = typed;
                return true;
            }
            return false;
        }

        public T GetRequired<T>(string token) where T : notnull
        {
            if (!TryGet<T>(token, out var value) || value is null)
                throw new KeyNotFoundException($"Navigation payload not found or wrong type. Token='{token}'");

            return value;
        }

        public bool Remove(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            return _items.TryRemove(token, out _);
        }

        public void Clear() => _items.Clear();
    }
}
