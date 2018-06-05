using System.Collections.Generic;
using System.Linq;

namespace NaturalConfiguration
{
    public abstract class ConfigurationParser<TConfiguring>
    {
        public List<ParserError> Parse(TConfiguring configuring, string configurationText)
        {
            if (SentenceParsers == null)
            {
                SentenceParsers = CreateSentenceParsers().ToArray();
            }

            var errors = new List<ParserError>();

            IEnumerable<string> sentences = SplitSentences(configurationText);

            foreach (var sentence in sentences)
            {
                var error = ParseSentence(configuring, sentence);
                if (error != null)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }

        private IEnumerable<string> SplitSentences(string configurationText)
        {
            // TODO: if sentences can contain a . inside a quote (or as a number separator?), don't split on those.
            return configurationText.Split('.')
                .Select(sentence => sentence.Trim(' ', '\t', '\n', '\r'))
                .Where(sentence => sentence.Length > 0);
        }

        private SentenceParser<TConfiguring>[] SentenceParsers { get; set; }

        private ParserError ParseSentence(TConfiguring configuring, string sentence)
        {
            foreach (var parser in SentenceParsers)
            {
                if (parser.Parse(configuring, sentence, out string errorMsg))
                {
                    return errorMsg == null ? null : new ParserError(parser, sentence, errorMsg);
                }
            }

            return new ParserError(null, sentence, "Sentence doesn't match any known rules.");
        }

        protected abstract IEnumerable<SentenceParser<TConfiguring>> CreateSentenceParsers();
    }
}
