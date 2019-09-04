using System;
using System.Collections.Generic;
using System.Text;

namespace SemiExpression
{
    using Token = String;
    using TokenCollection = List<String>;

    public interface ITokenCollection : IEnumerable<Token>
    {
        bool Open(string source);
        void Close();
        TokenCollection GetTokens();
        int Size();
        Token this[int i] { get;set; }
        ITokenCollection Add(Token token);
        bool Insert(int n, Token token);
        void Clear();
        bool Contains(Token token);
        bool Find(Token token, out int index);
        Token Predecessor(Token token);
        bool HasSequence(params Token[] tokenSequence);
        bool HasTerminator();
        bool IsDone();
        int LineCount();
        string ToString();
        void Show();
    }
}
