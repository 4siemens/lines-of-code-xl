// // This code is distributed under MIT license. 
// // Copyright (c) 2015-2016 George Mamaladze
// // See license.txt or http://opensource.org/licenses/mit-license.php

using System.Linq;
using Gma.CodeCloud.Base.TextAnalyses.Blacklist;
using Gma.CodeCloud.Base.TextAnalyses.Processing;
using Gma.CodeCloud.Base.TextAnalyses.Stemmers;

namespace LocCounter
{
    internal class WordsTree : Tree<WordDo[]>
    {
        protected override WordDo[] Merge(WordDo[] data, WordDo[] words)
        {
            return null;
        }

        protected override string GetLeafName(string leafName, WordDo[] data)
        {
            return leafName;
        }

        public WordDo[] GetWords(string[] path)
        {
            var stemmer = new LowerCaseStemmer();
            var blacklist = CommonBlacklist.CreateFromTextFile("CSharpBlacklist.txt");

            Node<WordDo[]> node;
            var found = TryGetByPath(path, out node);
            if (!found) return new WordDo[] { };

            return node
                .Subtree()
                .Where(n => n.Data != null)
                .SelectMany(n => n.Data)
                .GroupByStem(stemmer)
                .OrderByDescending(w=>w.size)
                .Where(w=> !blacklist.Countains(w.text))
                .Take(100)
                .ToArray();
        }
    }
}