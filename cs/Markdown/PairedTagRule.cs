﻿using System;

namespace Markdown
{
    internal class PairedTagRule : ILexerRule
    {
        private readonly string fullTag;

        public PairedTagRule(char tag, int length)
        {
            Tag = tag;
            Length = length > 0 ? length : throw new ArgumentException("Length must be positive");
            fullTag = new string(tag, length);
        }

        public int Length { get; set; }

        public char Tag { get; set; }

        public Delimiter ProcessIncomingChar(int position, string text, out int amountOfSymbolsToSkip)
        {
            amountOfSymbolsToSkip = Length - 1;
            return new Delimiter(fullTag, position);
        }

        public bool Check(int position, string text)
        {
            if (Length == 1)
            {
                if (position < text.Length - 1 && text[position + 1] == Tag)
                    return false;
                return text[position] == Tag;
            }

            if (position < text.Length - 1)
                return text.Substring(position, Length) == fullTag;
            return false;

        }

        public bool Check(Delimiter delimiter) => delimiter.Value == fullTag;

        public Delimiter Escape(Delimiter delimiter, string text)
        {
            if (delimiter.Position == 0)
                return delimiter;
            return text[delimiter.Position - 1] != '\\' ? delimiter : Length == 1 ? null : new Delimiter(fullTag.Substring(1), delimiter.Position + 1);
        }

        public bool IsValid(Delimiter delimiter, string text)
        {
            var delimiterPosition = delimiter.Position;
            var isLast = delimiterPosition == text.Length - Length;
            var isFirst = delimiterPosition == 0;
            if (isLast || isFirst)
                return true;
            var next = text[delimiterPosition + Length];
            var previous = text[delimiterPosition - 1];
            return !(next.IsLetterOrDigitOrSpecifiedChar('_') && previous.IsLetterOrDigitOrSpecifiedChar('_'));
        }

        public Token GetToken(Delimiter delimiter, string text)
        {
            if (delimiter.IsLast)
                return null;
            var second = delimiter.Partner;
            var length = second.Position - delimiter.Position + Length;

            return new UnderscoreToken(delimiter.Position, length, text.Substring(delimiter.Position, length));
        }

        public bool IsValidFirst(Delimiter delimiter, string text)
        {
            var position = delimiter.Position;
            var isLast = position == text.Length - Length;
            var isFirst = position == 0;
            if (isFirst)
                return !char.IsWhiteSpace(text[position + Length]);
            if (isLast)
                return false;
            return char.IsWhiteSpace(text[position - 1]) && !char.IsWhiteSpace(text[position + Length]);

        }

        public bool IsValidSecond(Delimiter delimiter, string text)
        {
            var position = delimiter.Position;
            var isLast = position == text.Length - Length;
            var isFirst = position == 0;
            if (isFirst)
                return false;
            if (isLast)
                return !char.IsWhiteSpace(text[position - 1]);
            return !char.IsWhiteSpace(text[position - 1]) && char.IsWhiteSpace(text[position + Length]);
        }
    }
}