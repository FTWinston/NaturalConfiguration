using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public abstract class SentenceParser
    {

    }

    public abstract class SentenceParser<TConfiguring> : SentenceParser
    {
        protected SentenceParser()
        {
            Regex = new Regex($"^{Expression}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
        
        protected abstract string Expression { get; }
        private Regex Regex { get; }

        public bool Parse(TConfiguring configuring, string sentence, out string error)
        {
            var match = Regex.Match(sentence);
            if (!match.Success)
            {
                error = null;
                return false;
            }
            
            error = ParseMatch(configuring, match);
            return true;
        }

        protected abstract string ParseMatch(TConfiguring configuring, Match match);
    }
}
