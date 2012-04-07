using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Shock.Logger;

namespace TheZhazha.Models
{
    public class TextGenerator
    {
        private MarkovChain _words;
        private List<MarkovChainDeep> _wordsDeep;

        public bool IsReady { get; private set; }

        public TextGenerator()
        {
            var fileName = string.Format(@"{0}\dictionary.txt", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            try
            {
                using (var sr = new StreamReader(fileName))
                {
                    // IsReady = TrainModel(sr);
                    IsReady = TrainModelDeep(sr);
                    LoggerFacade.Log(string.Format("TextGenerator.IsReady = {0}", IsReady));
                }
            }
            catch (Exception)
            {
                LoggerFacade.Log("Text dictionary not found, disabling text generation");
            }
        }

        private bool TrainModel(TextReader reader)
        {
            bool firstLine = true;
            string line;
            int level;
            _words = new MarkovChain();
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if(firstLine)
                {
                    if(!int.TryParse(line, out level))
                    {
                        return false;
                    }
                    firstLine = false;
                    continue;
                }
                line = Regex.Replace(line, @"\s{2,}", " ");
                var rawWords = Regex.Matches(line, @"\b(?<word>[\p{L}0-9'\.\-]+)\b(?<ends>[\.\?\!;:$])?");

                for (var i = 1; i < rawWords.Count; i++)
                {
                    var word = rawWords[i].Groups["word"].Value.ToLower();

                    if (!_words[rawWords[i - 1].Groups["word"].Value.ToLower()].Next.Contains(_words[word]))
                    {
                        _words[rawWords[i - 1].Groups["word"].Value.ToLower()].Next.Add(_words[word]);
                    }

                    if (rawWords[i - 1].Groups["ends"].Success)
                    {
                        _words[word].Starts = true;
                    }
                    /*
                    The original purpose of this is to capitalize the word in the dictionary
                    if it was capitalized in the text, therefore names will be written correctly,
                    however the tokenizer needs to be able to correctly find a sentence starter,
                    otherwise bunch of words will be capitalized.
                    Until then, this code is commented out.
                
                    else if (char.IsUpper(rawWords[i].Groups["word"].Value[0]))
                    {
                        words[word].Value = rawWords[i].Groups["word"].Value;
                    }
                    */

                    if (rawWords[i].Groups["ends"].Success)
                    {
                        _words[word].Ends = true;
                    }
                }
            }

            return true;
        }

        private bool TrainModelDeep(TextReader reader)
        {
            bool firstLine = true;
            string line;
            int level = 1;
            _wordsDeep = new List<MarkovChainDeep>();
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (firstLine)
                {
                    if (!int.TryParse(line, out level))
                    {
                        return false;
                    }
                    firstLine = false;
                    continue;
                }
                
                line = Regex.Replace(line, @"\s{2,}", " ");
                var rawWords = Regex.Matches(line, @"\b(?<word>[\p{L}0-9'\.\-]+)\b(?<ends>[\.\?\!;:$])?");

                for (var i = 0; i < rawWords.Count; i++)
                {
                    var word = rawWords[i].Groups["word"].Value;
                    var chain = new List<string>();
                    for (var j = 1; j <= level && j < rawWords.Count - i; j++)
                    {
                        chain.Add(rawWords[i + j].Groups["word"].Value.ToLower());
                    }
                    var mChain = new MarkovChainDeep(word, chain);

                    if (i == 0
                        || rawWords[i - 1].Groups["ends"].Success)
                    {
                        mChain.IsStart = true;
                    }

                    if (i == rawWords.Count - 1
                        || rawWords[i].Groups["ends"].Success)
                    {
                        mChain.IsEnd = true;
                    }

                    _wordsDeep.Add(mChain);
                }
            }

            return true;
        }

        public string Generate()
        {
            var sb = new StringBuilder();

                var wordCount = Zhazha.Rnd.Next(5, 30);

                var word = StartSentence();
                sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.Value));

                for (var z = 0; z != wordCount; z++)
                {
                    try
                    {
                        word = word.GetNext();
                        sb.Append(" " + word.Value);
                    }
                    catch
                    {
                        break;
                    }
                }

                sb.Append(EndSentence(word) + " ");

            return sb.ToString();
        }

        public string GenerateDeep()
        {
            var sb = new StringBuilder();
            var wordCount = Zhazha.Rnd.Next(5, 30);
            var wordsReady = 0;
            var startWord = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(StartSentenceDeep().Word);
            sb.Append(startWord);

            while (wordsReady < wordCount)
            {
                var newList = GetAllChainsStartingWith(startWord);
                if (newList.Count == 0)
                    break;
                //get a random word from the list
                var randomChain = newList[Zhazha.Rnd.Next(newList.Count)];
                foreach (var w in randomChain.WordChain.Where(w => !string.IsNullOrEmpty(w)))
                {
                    sb.Append(" ");
                    sb.Append(w);
                    startWord = w;
                }
                wordsReady += randomChain.WordChain.Count;
            }

            sb.Append(EndSentenceDeep());
            return sb.ToString();
        }

        private Word StartSentence()
        {
            var startingWords = _words.Where(word => word.Value.Starts);

            if (startingWords.Count() != 0)
            {
                return startingWords.ElementAt(Zhazha.Rnd.Next(startingWords.Count())).Value;
            }
            else
            {
                return _words.ElementAt(Zhazha.Rnd.Next(_words.Count)).Value;
            }
        }

        private MarkovChainDeep StartSentenceDeep()
        {
            var startingWords = _wordsDeep.Where(word => word.IsStart).ToList();
            if (startingWords.Count > 0)
            {
                return startingWords.ElementAt(Zhazha.Rnd.Next(startingWords.Count));
            }
            return _wordsDeep.ElementAt(Zhazha.Rnd.Next(_wordsDeep.Count));
        }

        private List<MarkovChainDeep> GetAllChainsStartingWith(string word)
        {
            return _wordsDeep.Where(mc => (mc.Word.ToLowerInvariant() == word.ToLowerInvariant()) && mc.WordChain.Count > 0).ToList();
        }

        private string EndSentence(Word lastWord)
        {
            var words = new List<Word>();
            var result = FindEndWord(lastWord, ref words);

            if (result)
            {
                var sb = new StringBuilder();

                words.Reverse();

                foreach (var word in words)
                {
                    sb.Append(" " + word.Value);
                }

                return sb + ".";
            }

            return "...";
        }

        private string EndSentenceDeep()
        {
            var endSigns = new[] {".", "!", "?", "..."};
            return endSigns[Zhazha.Rnd.Next(endSigns.Length)];
        }

        private static bool FindEndWord(Word lastWord, ref List<Word> words)
        {
            if (lastWord.Next.Count == 0)
            {
                return false;
            }

            var ends = lastWord.Next.Where(word => word.Ends);

            if (ends.Count() != 0)
            {
                words.Add(ends.OrderBy(word => Zhazha.Rnd.Next()).First());

                return true;
            }

            foreach (var word in lastWord.Next)
            {
                var result = FindEndWord(word, ref words);

                if (result)
                {
                    words.Add(word);
                    return true;
                }
            }

            return false;
        }
    }
}
