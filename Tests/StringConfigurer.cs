using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaturalConfiguration.Tests
{
    public class StringHolder
    {
        public string Value { get; set; }
    }

    public class StringConfigurer : ConfigurationParser<StringHolder>
    {
        protected override IEnumerable<SentenceParser<StringHolder>> CreateSentenceParsers()
        {
            yield return new Replace();
            yield return new Case();
            yield return new Remove();
        }

        class Replace : SentenceParser<StringHolder>
        {
            protected override string ExpressionText => "Replace \"(.*)\" with \"(.*)\"";

            public override string[] Examples => new[] {
                "Replace \"x\" with \"y\"",
                "Replace \"something\" with \"\""
            };

            protected override void ParseMatch(Match match, Action<Action<StringHolder>> action, Action<ParserError> error)
            {
                var find = match.Groups[1];

                if (find.Value.Length == 0)
                {
                    error(new ParserError("Match text cannot be empty.", find.Index - 1, 2));
                    return;
                }

                var replace = match.Groups[2];

                action(modify => modify.Value = modify.Value.Replace(find.Value, replace.Value));
            }
        }

        class Case : SentenceParser<StringHolder>
        {
            protected override string ExpressionText => "Convert to (.+) case";

            public override string[] Examples => new[] {
                "Convert to upper case",
                "Convert to lower case"
            };

            protected override void ParseMatch(Match match, Action<Action<StringHolder>> action, Action<ParserError> error)
            {
                var group = match.Groups[1];

                if (group.Value == "upper")
                {
                    action(modify => modify.Value = modify.Value.ToUpper());
                }
                else if (group.Value == "lower")
                {
                    action(modify => modify.Value = modify.Value.ToLower());
                }
                else
                {
                    error(new ParserError("Unexpected case value: " + group.Value, group));
                }
            }
        }

        class Remove : ListParser<StringHolder>
        {
            public override string[] Examples => new[] {
                "Remove these words: fish, chips",
                "Remove these words: red, white and blue"
            };

            protected override string ExpressionPrefix => "Remove these words: ";

            protected override void ParseMatch(Match match, List<Capture> listValues, Action<Action<StringHolder>> action, Action<ParserError> error)
            {
                action(modify => {
                    foreach (var word in match.Groups.Skip(1)) {
                        modify.Value = modify.Value.Replace(word.Value, string.Empty);
                    }
                });
            }
        }
    }
}
