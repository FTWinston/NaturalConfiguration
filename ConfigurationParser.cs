using System.Collections.Generic;
using System.Linq;

namespace NaturalConfiguration
{
    public abstract class ConfigurationParser<TConfiguring>
    {
        public List<ParserError> Parse(TConfiguring configuring, string configurationText)
        {
            var errors = new List<ParserError>();

            IEnumerable<SentenceData> sentences = SplitSentences(configurationText);

            foreach (var sentence in sentences)
            {
                var sentenceErrors = ParseSentence(configuring, sentence.Text);
                if (sentenceErrors == null)
                    continue;
                    
                foreach (var error in sentenceErrors)
                {
                    error.StartIndex += sentence.StartIndex;
                    if (error.Length == -1)
                    {
                        error.Length = sentence.Length;
                    }

                    errors.Add(error);
                }
            }

            return errors;
        }

        private IEnumerable<SentenceData> SplitSentences(string configurationText)
        {
            int startPos = -1, endPos = -1;

            while (true)
            {
                startPos = endPos + 1;
                endPos = configurationText.IndexOf('.', startPos);

                if (endPos == -1)
                    break;

                string text = configurationText.Substring(startPos, endPos - startPos);

                int numTrimStart = text.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                text = text.Substring(numTrimStart);

                if (text.Length == 0)
                {
                    continue;
                }

                startPos += numTrimStart;

                int numTrimEnd = text.Reverse().TakeWhile(c => char.IsWhiteSpace(c)).Count();
                text = text.Substring(0, text.Length - numTrimEnd);

                yield return new SentenceData()
                {
                    Text = text,
                    StartIndex = startPos,
                    Length = endPos - startPos,
                };
            }
        }

        private SentenceParser<TConfiguring>[] _sentenceParsers = null;
        public SentenceParser<TConfiguring>[] SentenceParsers
        {
            get
            {
                if (_sentenceParsers == null)
                    _sentenceParsers = CreateSentenceParsers().ToArray();

                return _sentenceParsers;
            }
        }

        private ParserError[] ParseSentence(TConfiguring configuring, string sentence)
        {
            foreach (var parser in SentenceParsers)
            {
                if (parser.Parse(configuring, sentence, out ParserError[] errors))
                {
                    return errors;
                }
            }

            return new[] { new ParserError("Sentence doesn't match any known rules.") };
        }

        protected abstract IEnumerable<SentenceParser<TConfiguring>> CreateSentenceParsers();

        struct SentenceData
        {
            public string Text;
            public int StartIndex;
            public int Length;
        }
    }
}
