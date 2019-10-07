using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public abstract class SentenceParser<TConfiguring>
    {
        protected SentenceParser()
        {
            Expression = new Regex($"^{ExpressionText}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
     
        public abstract string[] Examples { get; }

        protected abstract string ExpressionText { get; }
        public Regex Expression { get; }

        protected const string WordExpression = "[^,\\s]+";

        public bool Parse(TConfiguring configuring, string sentence, out ParserError[] errors)
        {
            var match = Expression.Match(sentence);
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
