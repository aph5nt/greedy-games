using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Infrastructure
{
    public class AdapterHub<TAdapter, TKey> where TAdapter : IAdapter<TKey>
    {
        public IEnumerable<TAdapter> AllRegisteredAdapters { get; set; }

        public TAdapter GetAdapter(TKey key)
        {
            var adapter = GetAdapterOrDefault(key);
            if (adapter == null)
                throw new InvalidOperationException($"No {typeof(TAdapter).Name} adapter supports key {key}.");

            return adapter;
        }

        public TAdapter GetAdapterOrDefault(TKey key)
        {
            var matchingAdapters = GetSupportedAdapters(key);
            if (matchingAdapters.Count > 1)
            {
                var adapterNames = string.Join(", ", matchingAdapters.Select(a => a.GetType().FullName));
                throw new InvalidOperationException(
                    $"More than one {typeof(TAdapter).Name} adapter supports key {key}: {adapterNames}.");
            }

            return matchingAdapters.SingleOrDefault();
        }

        public List<TAdapter> GetSupportedAdapters(TKey key)
        {
            if (AllRegisteredAdapters == null)
                throw new InvalidOperationException(
                    $"No adapters for interface {typeof(TAdapter).Name} were injected.");

            return AllRegisteredAdapters
                .Where(a => a.IsSupported(key))
                .ToList();
        }
    }
}