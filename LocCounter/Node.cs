using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LocCounter
{
    [JsonObject(MemberSerialization.OptIn)]
    class Node<T>
    {
        private readonly ConcurrentDictionary<string, Node<T>> m_Nodes;

        public Node()
        {
            m_Nodes = new ConcurrentDictionary<string, Node<T>>();
        }

        [JsonProperty("name", Order = 1)]
        public string Name { get; set; }

        [JsonProperty("data", Order = 2)]
        public T Data { get; set; }

        [JsonProperty("depth", Order = 3)]
        public int Depth { get; set; }


        [JsonProperty("children", Order = 4)]
        public Node<T>[] Children
        {
            get { return m_Nodes.Values.ToArray(); }
            set
            {
                m_Nodes.Clear();
                foreach (var node in value)
                {
                    m_Nodes.AddOrUpdate(node.Name, node, (s, node1) => node);
                }
            }
        }

        public bool TryGet(string key, out Node<T> node)
        {
            return m_Nodes.TryGetValue(key, out node);
        }

        public void AddOrUpdate(string name, Func<string, Node<T>> addValueFactory, Func<string, Node<T>, Node<T>> updateValueFactory)
        {
            m_Nodes.AddOrUpdate(name, addValueFactory, updateValueFactory);
        }

        public bool IsLeaf()
        {
            return m_Nodes.IsEmpty;
        }

        public IEnumerable<Node<T>> Subtree()
        {
            yield return this;
            foreach (var child in Children)
            {
                var childSubtree = child.Subtree();
                foreach (var node in childSubtree)
                {
                    yield return node;
                }
            }
        }

        public bool ShouldSerializeChildren()
        {
            return m_Nodes.Count > 0;
        }


    }
}