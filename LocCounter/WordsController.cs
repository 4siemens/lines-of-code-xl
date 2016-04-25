using System.IO;
using System.Linq;
using System.Web.Http;
using Gma.CodeCloud.Base.TextAnalyses.Processing;
using Newtonsoft.Json;

namespace LocCounter
{
    public class WordDo
    {
        public string text { get; set; }
        public int size { get; set; }
    }


    public static class DoConversionExtensions
    {
        public static WordDo ToDo(this IWord word)
        {
            return new WordDo {text = word.Text, size = word.Occurrences};
        }
    }

    public class WordsController : ApiController
    {
        private static WordsTree s_Cache;

        private static WordsTree GetTree()
        {
            return s_Cache ?? (s_Cache = Load());
        }

        private static WordsTree Load()
        {
            return JsonConvert.DeserializeObject<WordsTree>(File.ReadAllText(@"html\words.json"));
        }

        public WordDo[] Get(string id)
        {
            var path = id.Split(';').Where(part => !string.IsNullOrEmpty(part)).ToArray();
            return GetTree().GetWords(path);
        }
    }
}