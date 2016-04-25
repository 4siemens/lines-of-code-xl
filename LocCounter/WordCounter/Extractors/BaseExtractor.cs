using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gma.CodeCloud.Base.TextAnalyses.Extractors
{
    public abstract class BaseExtractor : IEnumerable<string>
    {
        protected virtual IEnumerable<string> GetWordsInLine(string line)
        {
            var word = new StringBuilder();
            foreach (var ch in line)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    word.Append(ch);
                }
                else
                {
                    if (word.Length > 1)
                    {
                        yield return word.ToString();
                    }
                    word.Clear();
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return GetWords().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract IEnumerable<string> GetWords();
    }
}