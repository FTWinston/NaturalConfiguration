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

        public bool Parse(string sentence, ICollection<Action<TConfiguring>> actions, ICollection<ParserError> errors)
        {
            var match = Expression.Match(sentence);
            if (!match.Success)
            {
                return false;
            }
            
            ParseMatch(match, action => actions.Add(action), error => errors.Add(error));
            return true;
        }

        protected abstract void ParseMatch(Match match, Action<Action<TConfiguring>> action, Action<ParserError> error);
    }
}
