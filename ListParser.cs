using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public abstract class ListParser<TConfiguring>: SentenceParser<TConfiguring>
    {
        protected override string ExpressionText => $"{ExpressionPrefix}({ElementExpression})(?:, ({ElementExpression}))*(?: and ({ElementExpression}))?{ExpressionSuffix}";
        protected abstract string ExpressionPrefix { get; }
        protected virtual string ExpressionSuffix { get; } = string.Empty;
        protected virtual string ElementExpression { get; } = WordExpression;
        protected virtual int ListGroupOffset { get; } = 0;

        protected sealed override IEnumerable<ParserError> ParseMatch(TConfiguring configuring, Match match)
        {
            var values = new List<Capture>();
            values.Add(match.Groups[1 + ListGroupOffset]);
            if (match.Groups[2 + ListGroupOffset].Success)
            {
                foreach (Capture capture in match.Groups[2 + ListGroupOffset].Captures)
                {
                    values.Add(capture);
                }
            }

            if (match.Groups[3 + ListGroupOffset].Success)
            {
                values.Add(match.Groups[3 + ListGroupOffset]);
            }

            return ParseMatch(configuring, match, values);
        }

        protected abstract IEnumerable<ParserError> ParseMatch(TConfiguring configuring, Match match, List<Capture> listValues);
    }
}
