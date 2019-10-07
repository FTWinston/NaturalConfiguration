using System;
using System.Collections.Generic;

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
            throw new NotImplementedException();
        }
    }
}
