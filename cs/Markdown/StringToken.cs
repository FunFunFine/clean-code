﻿namespace Markdown
{
    using System;
    using System.Collections.Generic;

    internal sealed class StringToken : Token
    {
        public StringToken(int position, int length, string value)
        {
            Position = position;
            Length = length;
            Value = value;
        }

        public override List<Token> InnerTokens { get => null; set => throw new NotSupportedException(); }

        public override int Length { get; set; }

        public override Token ParentToken { get; set; }

        public override int Position { get; set; }

        public override string Value { get; set; }
    }
}
