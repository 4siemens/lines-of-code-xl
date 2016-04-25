using System;
using System.Collections.Generic;
using System.Linq;
using Gma.CodeCloud.Base.TextAnalyses.Blacklist;
using Gma.CodeCloud.Base.TextAnalyses.Stemmers;
using LocCounter;

namespace Gma.CodeCloud.Base.TextAnalyses.Processing
{
    public static class WordExtensions
    {

        public static IEnumerable<WordDo> GroupByStem(this IEnumerable<WordDo> words, IWordStemmer stemmer)
        {
            return
                words
                .GroupBy(word => stemmer.GetStem(word.text))
                .Select(group=>
                    {
                        var pairs = group.ToArray();
                        return new WordDo()
                        {
                            size = 
                                pairs
                                .Select(s=>s.size).Sum(),
                            text = 
                                pairs
                                .OrderByDescending(p => p.size)
                                .First().text
                        };
                    });

        }


        public static IEnumerable<WordGroup> GroupByStem(this IEnumerable<IWord> words, IWordStemmer stemmer)
        {
            return
                words.GroupBy(
                    word => stemmer.GetStem(word.Text), 
                    (stam, sameStamWords) => new WordGroup(stam, sameStamWords));
            
        }

        public static IOrderedEnumerable<T> SortByOccurences<T>(this IEnumerable<T> words) where T : IWord
        {
            return 
                words.OrderByDescending(
                    word => word.Occurrences);
        }

        public static IEnumerable<IWord> CountOccurences(this IEnumerable<string> terms)
        {
            return 
                terms.GroupBy(
                    term => term,
                    (term, equivalentTerms) => new Word(term, equivalentTerms.Count()), 
                    StringComparer.InvariantCultureIgnoreCase)
                    .Cast<IWord>();
        }

        public static IEnumerable<string> Filter(this IEnumerable<string> terms, IBlacklist blacklist)
        {
            return
                terms.Where(
                    term => !blacklist.Countains(term));
        }

        public static IEnumerable<IWord> Merge(this IEnumerable<IWord> words, IEnumerable<IWord> others)
        {
            return 
                words
                    .Concat(others)
                    .GroupBy(w => w.Text)
                    .Select(g => new Word(g.Key, g.Sum(w => w.Occurrences)))
                    .Cast<IWord>();
        }
    }
}