using System;
using System.Collections.Generic;

namespace TheZhazha.Models
{
    public class MarkovChainDeep
    {
        public String Word { get; set; }
        public List<String> WordChain { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }

        public MarkovChainDeep()
        {
        }

        public MarkovChainDeep(string word, List<string> wordChain)
        {
            Word = word;
            WordChain = wordChain;
        }
    }

    [Serializable]
    public class WordDeep
    {
        public string Value;
        public bool Starts;     // was it seen to start a sentence?
        public bool Ends;       // ...or to end one?
        public List<List<Word>> Next;

        public WordDeep(string value, List<List<Word>> next)
        {
            Value = value;
            Starts = false;
            Ends = false;
            Next = next ?? new List<List<Word>>();
        }

        public WordDeep(string value, bool starts = false, bool ends = false, List<List<Word>> next = null)
        {
            Value = value;
            Starts = starts;
            Ends = ends;
            Next = next ?? new List<List<Word>>();
        }

        public List<Word> GetNext()
        {
            if (Next.Count == 0)
            {
                throw new Exception("Fun's over. Out of words.");
            }

            return Next[Zhazha.Rnd.Next(Next.Count)];
        }
    }
}