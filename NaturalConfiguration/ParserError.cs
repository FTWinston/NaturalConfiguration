using System.Text.RegularExpressions;

namespace NaturalConfiguration
{
    public class ParserError
    {
        public ParserError(string message, Capture capture)
            : this(message, capture.Index, capture.Length)
        { }

        public ParserError(string message)
            : this(message, 0, -1)
        { }

        public ParserError(string message, int startIndex, int length)
        {
            Message = message;
            StartIndex = startIndex;
            Length = length;
        }

        public string Message { get; }
        public int StartIndex { get; internal set; }
        public int Length { get; internal set; }
    }
}
