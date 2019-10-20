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
            ParseConfiguration(configurationText, out _, out List<ParserError> errors);

            return errors;
        }

        public List<ParserError> Configure(string configurationText, TConfiguring configuring)
        {
            ParseConfiguration(configurationText, out List<Action<TConfiguring>> actions, out List<ParserError> errors);

            if (errors.Count > 0)
            {
                return errors;
            }

            foreach (var action in actions)
            {
                action(configuring);
            }

            return errors;
        }

        private void ParseConfiguration(string configurationText, out List<Action<TConfiguring>> actions, out List<ParserError> errors)
        {
            actions = new List<Action<TConfiguring>>();
            errors = new List<ParserError>();

            IEnumerable<SentenceData> sentences = SplitSentences(configurationText, out bool hasUnfinishedSentence);

            foreach (var sentence in sentences)
            {
                var sentenceErrors = ParseSentence(actions, sentence.Text);
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

            if (hasUnfinishedSentence)
            {
                errors.Add(new ParserError("The last sentence is unfinished.", configurationText.Length - 1, 1));
            }
        }

        private List<SentenceData> SplitSentences(string configurationText, out bool hasUnfinishedSentence)
        {
            var sentences = new List<SentenceData>();
            int startPos, endPos = -1;

            while (true)
            {
                startPos = endPos + 1;
                endPos = configurationText.IndexOf('.', startPos);

                if (endPos == -1)
                {
                    hasUnfinishedSentence = startPos < configurationText.Length;
                    return sentences;
                }

                string text = configurationText.Substring(startPos, endPos - startPos);

                int origLength = text.Length;
                text = text.TrimStart();

                if (text.Length == 0)
                {
                    continue;
                }

                startPos += origLength - text.Length;

                text = text.TrimEnd();

                sentences.Add(new SentenceData
                {
                    Text = text,
                    StartIndex = startPos,
                    Length = endPos - startPos,
                });
            }
        }

        private SentenceParser<TConfiguring>[] SentenceParsers { get; }

        public Lazy<string[]> Examples { get; }

        private IList<ParserError> ParseSentence(List<Action<TConfiguring>> actions, string sentence)
        {
            List<ParserError> errors = new List<ParserError>();

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
