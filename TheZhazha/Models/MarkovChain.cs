using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TheZhazha.Models
{
    [Serializable]
    public class MarkovChain : Dictionary<string, Word>
    {
        #region Constructors

        public MarkovChain()
        {

        }

        public MarkovChain(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
        
        public new Word this[string key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return base[key];
                }
                return base[key] = new Word(key);
            }
            set
            {
                base[key] = value;
            }
        }
    }

    [Serializable]
    public class Word
    {
        public string Value;
        public bool Starts;     // was it seen to start a sentence?
        public bool Ends;       // ...or to end one?
        public List<Word> Next;

        public Word(string value, List<Word> next)
        {
            Value = value;
            Starts = false;
            Ends = false;
            Next = next ?? new List<Word>();
        }

        public Word(string value, bool starts = false, bool ends = false, List<Word> next = null)
        {
            Value = value;
            Starts = starts;
            Ends = ends;
            Next = next ?? new List<Word>();
        }

        public Word GetNext()
        {
            if (Next.Count == 0)
            {
                throw new Exception("Fun's over. Out of words.");
            }

            return Next[Zhazha.Rnd.Next(Next.Count)];
        }
    }
}
