using System.Collections.Generic;
using System.Linq;

namespace MarvellousMarkovModels
{
    public class ModelBuilder
    {
        private readonly int _order;

        public readonly Dictionary<string, int> _startingStrings = new Dictionary<string, int>();
        public readonly Dictionary<string, Dictionary<string, int>> _productions = new Dictionary<string, Dictionary<string, int>>();

        public ModelBuilder(int order)
        {
            _order = order;
        }

        public ModelBuilder Teach(IEnumerable<string> examples)
        {
            foreach (var example in examples)
                Teach(example);

            return this;
        }

        public ModelBuilder Teach(string example)
        {
            example = example.ToLowerInvariant();

            //if the example is shorter than the order, just add a production that this example instantly leads to null
            if (example.Length <= _order)
            {
                _startingStrings.AddOrUpdate(example, 1, a => a + 1);
                Increment(example, string.Empty);

                return this;
            }

            //Chomp string into "order" length parts, and the single letter which follows it
            for (int i = 0; i < example.Length - _order + 1; i++)
            {
                var key = example.Substring(i, _order);
                if (i == 0)
                    _startingStrings.AddOrUpdate(key, 1, a => a + 1);

                var sub = i + _order == example.Length ? string.Empty : example.Substring(i + _order, 1);
                Increment(key, sub);
            }

            return this;
        }

        public Model ToModel()
        {
            return new Model(
                _order,
                Normalize(_startingStrings),
                _productions.ToDictionary(a => a.Key, a => Normalize(a.Value))
            );
        }

        private static KeyValuePair<string, float>[] Normalize(Dictionary<string, int> stringCounts)
        {
            var total = (float)stringCounts.Select(a => a.Value).Sum();

            return stringCounts.Select(a => new KeyValuePair<string, float>(a.Key, a.Value / total)).ToArray();
        }

        private void Increment(string key, string value)
        {
            Dictionary<string, int> set;
            if (!_productions.TryGetValue(key, out set))
            {
                set = new Dictionary<string, int>();
                _productions.Add(key, set);
            }

            set.AddOrUpdate(value, 1, a => a + 1);
        }
    }
}
