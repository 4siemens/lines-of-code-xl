using System;

namespace LocCounter
{
    class SizeTree : Tree<int>
    {
        private readonly int m_Threshold;

        public SizeTree(int threshold)
        {
            m_Threshold = threshold;
        }

        protected override int Merge(int prev, int add)
        {
            return prev + add;
        }

        protected override string GetLeafName(string leafName, int size)
        {
            return size < m_Threshold ? "other" : leafName;
        }
    }
}