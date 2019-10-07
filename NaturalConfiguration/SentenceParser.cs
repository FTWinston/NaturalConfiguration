using System;
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

        public bool Parse(string sentence, Action<Action<TConfiguring>> action, Action<ParserError> error)
        {
            var match = Expression.Match(sentence);
            if (!match.Success)
            {
                return false;
            }
            
            ParseMatch(match, action, error);
            return true;
        }

        protected abstract IEnumerable<ParserError> ParseMatch(Match match, Action<Action<TConfiguring>> action, Action<ParserError> error);
    }
}
