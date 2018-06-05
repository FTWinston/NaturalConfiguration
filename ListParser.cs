using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public abstract class ListParser<TConfiguring>: SentenceParser<TConfiguring>
    {
        protected override string Expression => $"{ExpressionPrefix}({ElementExpression})(?:, ({ElementExpression}))*(?: and ({ElementExpression}))?{ExpressionSuffix}";
        protected abstract string ExpressionPrefix { get; }
        protected virtual string ExpressionSuffix { get; } = string.Empty;
        protected virtual string ElementExpression { get; } = "\\w+";

        protected override string ParseMatch(TConfiguring configuring, Match match)
        {
            var values = new List<string>();
            values.Add(match.Groups[1].Value);
            if (match.Groups[2].Success)
            {
                foreach (Capture capture in match.Groups[2].Captures)
                {
                    values.Add(capture.Value);
                }
            }

            if (match.Groups[3].Success)
            {
                values.Add(match.Groups[3].Value);
            }

            return ParseValues(configuring, values);
        }

        protected abstract string ParseValues(TConfiguring configuring, List<string> values);
    }
}
