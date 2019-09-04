using System;
using System.Collections.Generic;
using System.Text;

namespace SemiExpression
{
    using System.Collections;
    using Token = String;
    using TokenCollection = List<String>;

    static public class Factory
    {
        static public ITokenCollection Create()
        {
            SemiExpression semiExp = new SemiExpression();
            semiExp.toker = new Toker.Toker();
            return semiExp;
        }
        
    }

    public class SemiExpression : ITokenCollection
    {
        TokenCollection tokenCollection = new TokenCollection();

        public Toker.Toker toker { get; set; } = new Toker.Toker();
        
        public SemiExpression(){ }

        public SemiExpression(List<Token> list)
        {
            tokenCollection.AddRange(list);
        }

        public void AddRange(ITokenCollection collection)
        {
            foreach (var token in collection)
                tokenCollection.Add(token);
        }

        public bool Open(string source)
        {
            return toker.Open(source);
        }

        public void Close()
        {
            toker.Close();
        }

        public int Size()
        {
            return tokenCollection.Count;
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            foreach (Token token in tokenCollection)
            {
                temp.Append(token).Append(" ");
            }
            return temp.ToString();
        }

        public int LineCount()
        {
            return toker.LineCount();
        }

        public bool IsTerminator(Token token)
        {
            if (token == ";" || token == "{" || token == "}")
                return true;
            if (token.Length > 0 && token[0] == '\n')
            {
                Trim();
                if (tokenCollection.Count > 0 && tokenCollection[0] == "#")
                    return true;
            }
            return false;
        }

        public void Trim()
        {
            int count = 0;
            for (count = 0; count < tokenCollection.Count; ++count)
            {
                if (tokenCollection[count] != "\n")
                    break;
            }

            if (count == 0)
                return;
            for (int i = 0; i < count; ++i)
                tokenCollection.RemoveAt(0);
        }

        public bool Contains(Token token)
        {
            if (tokenCollection.Contains(token))
                return true;
            return false;
        }

        public bool Find(Token token, out int index)
        {
            for (index = 0; index < Size(); ++index)
            {
                if (tokenCollection[index] == token)
                    return true;
            }
            index = -1;
            return false;
        }

        public string Predecessor(Token token)
        {
            int index;
            if (Find(token, out index) && index > 0)
            {
                return tokenCollection[index - 1];
            }
            return "";
        }

        public Token LastToken()
        {
            return tokenCollection[tokenCollection.Count - 1];
        }

        public bool HasSequence(params Token[] tokenSequence)
        {
            int position = 0;
            foreach (var token in tokenCollection)
            {
                if (position == tokenSequence.Length - 1)
                    return true;
                if (token == tokenSequence[position])
                    ++position;
            }
            return (position == tokenSequence.Length - 1);
        }

        public void FoldForFor()
        {
            if (HasSequence("for", "(", ";"))
            {
                SemiExpression temp = new SemiExpression(tokenCollection);
                GetTokens();
                temp.AddRange(this);
                GetTokens();
                temp.AddRange(this);
                tokenCollection = temp.tokenCollection;
            }
        }
        
        public TokenCollection GetTokens()
        {
            tokenCollection.Clear();

            while (!toker.IsDone())
            {
                Token token = toker.GetToken();
                if (token != "\n")
                    tokenCollection.Add(token);
                if (IsTerminator(token))
                {
                    FoldForFor();
                    return tokenCollection;
                }
            }
            return tokenCollection;
        }

        public Token this[int i]
        {
            get { return tokenCollection[i]; }
            set { tokenCollection[i] = value; }
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return tokenCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tokenCollection.GetEnumerator();
        }

        public ITokenCollection Add(Token token)
        {
            tokenCollection.Add(token);
            return this;
        }

        public void Clear()
        {
            tokenCollection.Clear();
        }

        public bool IsDone()
        {
            return toker.IsDone();
        }

        public bool Insert(int n, Token token)
        {
            if (n < 0 || n >= token.Length)
                return false;
            tokenCollection.Insert(n, token);
            return true;
        }
        
        public bool HasTerminator()
        {
            if (tokenCollection.Count <= 0)
                return false;
            if (IsTerminator(tokenCollection[tokenCollection.Count - 1]))
                return true;
            return false;
        }
        
        public void Show()
        {
            Console.Write("\n--  ");
            foreach (Token token in tokenCollection)
            {
                if (token != "\n")
                    Console.Write("{0} ", token);
            }
        }
    }

    public class SemiExpressionTestStub
    {
        public static void FindClasses()
        {
            string[] classSignature = { "class", "{" };
            ITokenCollection semiExp = Factory.Create();

            //string source = "../../../../Toker/Toker.cs";
            string source = "../../../SemiExpression.cs";
            if (!semiExp.Open(source))
            {
                Console.Write("\n Cant't open");
                return;
            }
            
            while (!semiExp.IsDone())
            {
                semiExp.GetTokens();
                if (semiExp.HasSequence(classSignature))
                {
                    semiExp.Show();
                }
            }
            Console.Write("\n-------------------------------------------------------------------\n");
            
        }
        public static void Main(string[] args)
        {
            Console.Write("\n Testing Semi");
            Console.Write("\n ============");

            ITokenCollection semiExp = Factory.Create();

            string source = "../../../SemiExpression.cs";
            if (!semiExp.Open(source))
            {
                Console.Write("\n Cant't open");
                return;
            }

            FindClasses();

            //while (!semiExp.IsDone())
            //{
            //    semiExp.GetTokens();
            //    semiExp.Show();
            //}
        }
    }

}
