using System;
using System.Collections.Generic;
using System.Linq;

namespace NaturalConfiguration
{
    public abstract class ConfigurationParser<TConfiguring>
    {
        protected ConfigurationParser()
        {
            SentenceParsers = CreateSentenceParsers().ToArray();

            Examples = new Lazy<string[]>(() => SentenceParsers.SelectMany(s => s.Examples).ToArray());
        }

        public List<ParserError> Validate(string configurationText)
        {

        }

        public List<ParserError> Configure(string configurationText, TConfiguring configuring)
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

                int origLength = text.Length;
                text = text.TrimStart();

                if (text.Length == 0)
                {
                    continue;
                }

                startPos += text.Length - origLength;

                text = text.TrimEnd();

                yield return new SentenceData()
                {
                    Text = text,
                    StartIndex = startPos,
                    Length = endPos - startPos,
                };
            }
        }

        private SentenceParser<TConfiguring>[] SentenceParsers { get; }

        public Lazy<string[]> Examples { get; }

        private ParserError[] ParseSentence(List<Action<TConfiguring>> actions, string sentence)
        {
            foreach (var parser in SentenceParsers)
            {
                if (parser.Parse(sentence, actions, errors))
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
