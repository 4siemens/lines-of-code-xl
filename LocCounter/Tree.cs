namespace LocCounter
{
    abstract class Tree<T> : Node<T>
    {
        public void Add(string fullName, T data)
        {

            var parts = fullName.Split('\\');
            var lastindex = parts.Length-1;
            parts[lastindex] = GetLeafName(parts[lastindex], data);
            
            Node<T> current = this;
            for (var i = 0; i < parts.Length; i++)
            {
                var key = parts[i];
                current.AddOrUpdate(
                    key,
                    s => new Node<T> { Name = s, Depth = i + 1, Data = data },
                    (s, n) =>
                    {
                        n.Data = Merge(n.Data, data);
                        return n;
                    });
                current.TryGet(key, out current);
            }
            Data = Merge(Data, data);
        }

        public bool TryGetByPath(string[] path, out Node<T>  node)
        {
            node = this;
            foreach (var part in path)
            {
                var found = node.TryGet(part, out node);
                if (!found) return false;
            }
            return true;
        }

        protected virtual string GetLeafName(string leafName, T data)
        {
            return leafName;
        }


        protected abstract T Merge(T prev, T add);
    }
}