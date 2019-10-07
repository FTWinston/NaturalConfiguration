using System;
using Xunit;

namespace NaturalConfiguration.Tests
{
    public class StringTests
    {
        [Theory]
        [InlineData("Replace \"o\" with \"ó\". Convert to upper case.")]
        [InlineData("Convert to upper case. Replace \"o\" with \"ó\".")]
        [InlineData("Remove these words: red, green and blue.")]
        public void Validate(string configuration)
        {
            var parser = new StringConfigurer();

            var errors = parser.Validate(configuration);

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("Replace \"o\" with \"ó\". Convert to upper case.", "Hello world", "HELLÓ WÓRLD")]
        [InlineData("Convert to upper case. Replace \"o\" with \"ó\".", "Hello world", "HELLO WORLD")]
        [InlineData("Remove these words: red, green and blue.", "red orange yellow green blue black white brown purple", " orange yellow   black white brown purple")]
        public void ModifyText(string configuration, string input, string output)
        {
            var parser = new StringConfigurer();

            var data = new StringHolder
            {
                Value = input,
            };

            var errors = parser.Configure(configuration, data);

            Assert.Empty(errors);

            Assert.Equal(output, data.Value);
        }

        [Fact]
        public void ReportUnrecognisedSentence()
        {
            var parser = new StringConfigurer();

            var errors = parser.Validate("This sentence matches no parser.");

            Assert.Single(errors);

            var error = errors[0];

            Assert.Equal(0, error.StartIndex);
            Assert.Equal(31, error.Length);
            Assert.Equal("Sentence doesn't match any known rules.", error.Message);
        }

        [Fact]
        public void IdentifySingleSentenceError()
        {
            var parser = new StringConfigurer();

            var errors = parser.Validate("Convert to nonsense case.");

            Assert.Single(errors);

            var error = errors[0];

            Assert.Equal(11, error.StartIndex);
            Assert.Equal(8, error.Length);
            Assert.Equal("Unexpected case value: nonsense", error.Message);
        }

        [Fact]
        public void IdentifySecondSentenceError()
        {
            var parser = new StringConfigurer();

            var errors = parser.Validate("Replace \"o\" with \"ó\". Convert to nonsense case.");

            Assert.Single(errors);

            var error = errors[0];

            Assert.Equal(33, error.StartIndex);
            Assert.Equal(8, error.Length);
            Assert.Equal("Unexpected case value: nonsense", error.Message);
        }

        [Fact]
        public void IdentifyMultipleErrors()
        {
            var parser = new StringConfigurer();

            var errors = parser.Validate("Replace \"\" with \"ó\". Convert to nonsense case.");

            Assert.Equal(2, errors.Count);

            var error = errors[0];
            Assert.Equal(8, error.StartIndex);
            Assert.Equal(2, error.Length);
            Assert.Equal("Match text cannot be empty.", error.Message);

            error = errors[1];
            Assert.Equal(32, error.StartIndex);
            Assert.Equal(8, error.Length);
            Assert.Equal("Unexpected case value: nonsense", error.Message);
        }
    }
}
