using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System
{
    public static partial class Properties
    {
        private static readonly Dictionary<int, MatchProperty[]> _matchesDictionary = new Dictionary<int, MatchProperty[]>();
        private static MatchProperty[] JoinMatches<T>(PropertyDescriptorsInfo left, PropertyDescriptorsInfo right, Action<IEnumerable<T>> load, IEnumerable<T> items)
        {
            if (left.Kind == PropertyDescriptorKind.Class && right.Kind == PropertyDescriptorKind.Class)
            {
                int key = GetFullNameHashCode(left.Type, right.Type);
                if (!_matchesDictionary.TryGetValue(key, out MatchProperty[] matches))
                {
                    lock (_matchesDictionary)
                    {
                        if (!_matchesDictionary.TryGetValue(key, out matches))
                        {
                            load(items);
                            matches = JoinMatches(left, right, false, false).ToArray();
                            _matchesDictionary.Add(key, matches);
                        }
                    }
                }
                return matches;
            }
            else
            {
                load(items);
                if (left.Kind == PropertyDescriptorKind.Dictionary)
                {
                    left.ConcatDictionaryProperties(right.Properties);
                }
                bool ignoreCaseName = right.Kind == PropertyDescriptorKind.DataRow;
                bool valueType = left.Kind != PropertyDescriptorKind.DataRow && right.Kind == PropertyDescriptorKind.DataRow;
                return JoinMatches(left, right, ignoreCaseName, valueType).ToArray();
            }
        }
        private static IEnumerable<MatchProperty> JoinMatches(PropertyDescriptorsInfo left, PropertyDescriptorsInfo right, bool ignoreCaseName, bool valueType)
        {
            PropertyKey[] rightKeys = new PropertyKey[right.Properties.Length];
            List<int> list = new List<int>();
            for (int i = 0; i < rightKeys.Length; i++)
            {
                rightKeys[i] = GetPropertyKey(right.Properties[i]);
                list.Add(i);
            }
            if (!ignoreCaseName)
            {
                foreach (PropertyDescriptor property in left.Properties)
                {
                    if (!property.IsReadOnly)
                    {
                        PropertyKey key = GetPropertyKey(property);
                        for (int i = 0; i < list.Count; i++)
                        {
                            int index = list[i];
                            if (ComparePropertyKey(rightKeys[index], key))
                            {
                                list.Remove(index);
                                yield return GetMatchProperty(property, right.Properties[index], valueType);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (PropertyDescriptor property in left.Properties)
                {
                    if (!property.IsReadOnly)
                    {
                        PropertyKey key = GetPropertyKey(property);
                        int ignoreCaseIndex = -1;
                        for (int i = 0; i < list.Count; i++)
                        {
                            int index = list[i];
                            if (ComparePropertyKey(rightKeys[index], key))
                            {
                                ignoreCaseIndex = -1;
                                list.Remove(index);
                                yield return GetMatchProperty(property, right.Properties[index], valueType);
                                break;
                            }
                            if (ignoreCaseIndex == -1 && ComparePropertyKeyIgnoreCase(rightKeys[index], key))
                            {
                                ignoreCaseIndex = index;
                            }
                        }
                        if (ignoreCaseIndex != -1)
                        {
                            list.Remove(ignoreCaseIndex);
                            yield return GetMatchProperty(property, right.Properties[ignoreCaseIndex], valueType);
                        }
                    }
                }
            }
        }
        private static PropertyKey GetPropertyKey(PropertyDescriptor property)
        {
            return new PropertyKey() { Name = property.Name, Type = GetMatchType(property.PropertyType) };
        }
        private static bool ComparePropertyKey(PropertyKey x, PropertyKey y)
        {
            return x.Name == y.Name && (x.Type == y.Type || y.Type == typeof(object) || y.Type.IsAssignableFrom(x.Type));
        }
        private static bool ComparePropertyKeyIgnoreCase(PropertyKey x, PropertyKey y)
        {
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && (x.Type == y.Type || y.Type == typeof(object) || y.Type.IsAssignableFrom(x.Type));
        }
        private class PropertyKey
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }
    }
}
