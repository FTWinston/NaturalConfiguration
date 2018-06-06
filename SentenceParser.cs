using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public abstract class SentenceParser<TConfiguring>
    {
        protected SentenceParser()
        {
            Regex = new Regex($"^{Expression}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
        
        protected abstract string Expression { get; }
        private Regex Regex { get; }

        public bool Parse(TConfiguring configuring, string sentence, out ParserError[] errors)
        {
            var match = Regex.Match(sentence);
            if (!match.Success)
            {
                errors = null;
                return false;
            }
            
            errors = ParseMatch(configuring, match).ToArray();
            return true;
        }

        protected abstract IEnumerable<ParserError> ParseMatch(TConfiguring configuring, Match match);
    }
}
