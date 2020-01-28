using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class Properties
    {
        private static readonly Dictionary<string, MatchProperty[]> _matchesDictionary = new Dictionary<string, MatchProperty[]>();
        private static MatchProperty[] JoinMatches<T>(PropertyDescriptorsInfo left, PropertyDescriptorsInfo right, MatchesNecessary<T> necessary, IEnumerable<T> items)
        {
            if (left.Kind == PropertyDescriptorKind.Class && right.Kind == PropertyDescriptorKind.Class)
            {
                string key = left.Type.GetFullName() + "#" + right.Type.GetFullName();
                if (!_matchesDictionary.TryGetValue(key, out MatchProperty[] matches))
                {
                    lock (_matchesDictionary)
                    {
                        if (!_matchesDictionary.TryGetValue(key, out matches))
                        {
                            necessary.LoadProperties(items);
                            matches = JoinMatches(left, right).ToArray();
                            _matchesDictionary.Add(key, matches);
                        }
                    }
                }
                return matches;
            }
            else
            {
                necessary.LoadProperties(items);
                if (left.Kind == PropertyDescriptorKind.Dictionary)
                {
                    left.ConcatDictionaryProperties(right.Properties);
                }
                return JoinMatches(left, right).ToArray();
            }
        }
        private static IEnumerable<MatchProperty> JoinMatches(PropertyDescriptorsInfo left, PropertyDescriptorsInfo right)
        {
            PropertyKey[] rightKeys = new PropertyKey[right.Properties.Length];
            List<int> list = new List<int>();
            for (int i = 0; i < rightKeys.Length; i++)
            {
                rightKeys[i] = GetPropertyKey(right.Properties[i]);
                list.Add(i);
            }
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
                            yield return GetMatchProperty(property, right.Properties[index]);
                            break;
                        }
                    }
                }
            }
        }
        private static PropertyKey GetPropertyKey(PropertyDescriptor property)
        {
            return new PropertyKey() { Name = property.Name, Type = GetMatchType(property.PropertyType) };
        }
        private static MatchProperty GetMatchProperty(PropertyDescriptor left, PropertyDescriptor right)
        {
            return new MatchProperty() { Left = left, Right = right };
        }
        private static bool ComparePropertyKey(PropertyKey x, PropertyKey y)
        {
            return x.Name == y.Name && (x.Type == y.Type || y.Type == typeof(object) || y.Type.IsAssignableFrom(x.Type));
        }
        private class MatchProperty
        {
            public PropertyDescriptor Left { get; set; }
            public PropertyDescriptor Right { get; set; }
        }
        private class PropertyKey
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }
    }
}
