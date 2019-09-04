using System;
using System.Collections.Generic;
using System.Text;

namespace Toker
{
    using Token = String;

    public interface ITokenSource
    {
        int LineCount { get; set; }

        bool Open(string path);
        void Close();
        int Next();
        int Peek(int n = 0);
        bool End();
    }

    public interface ITokenState
    {
        Token GetToken();
        bool IsDone();
    }

    public class Toker
    {
        private TokenContext context;
        public bool doReturnComments { get; set; } = false;

        public Toker()
        {
            context = new TokenContext();
        }

        public bool Open(string path)
        {
            TokenSourceFile source = new TokenSourceFile(context);
            context.source = source;
            return source.Open(path);
        }

        public void Close()
        {
            context.source.Close();
        }

        public bool ExtractSingleToken(Token tokenString)
        {
            if (isWhiteSpaceToken(tokenString))
                return true;
            if (doReturnComments && (isSingleLineCommentToken(tokenString) || isMultipleLineCommentToken(tokenString)))
                return true;
            return false;
        }


        public Token GetToken()
        {
            Token token = null;

            while (!IsDone())
            {
                token = context.currentTokenState.GetToken();
                context.currentTokenState = context.currentTokenState.NextTokenState();
                if (!ExtractSingleToken(token))
                    break;
            }
            return token;
        }

        public bool IsDone()
        {
            if (context.currentTokenState == null)
                return true;
            return context.currentTokenState.IsDone();
        }

        public int LineCount()
        {
            return context.source.LineCount;
        }

        private static bool isWhiteSpaceToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            return (Char.IsWhiteSpace(tokenString[0]) && tokenString[0] != '\n');
        }

        private static bool isNewLineToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            return (tokenString[0] == '\n');
        }

        private static bool isAlphaNumericToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            return (Char.IsLetterOrDigit(tokenString[0]) || tokenString[0] == '_');
        }

        private static bool isPuntuationToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            bool test = isWhiteSpaceToken(tokenString) || isNewLineToken(tokenString) || isAlphaNumericToken(tokenString);

            test = test || isSingleLineCommentToken(tokenString) || isMultipleLineCommentToken(tokenString);
            test = test || isSingleQuoteToken(tokenString) || isDoubleQuoteToken(tokenString);
            return !test;
        }

        private static bool isSingleLineCommentToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length < 2)
                return false;
            if (tokenString[0] == '/' && tokenString[1] == '/')
                return true;
            return false;
        }

        private static bool isMultipleLineCommentToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length < 2)
                return false;
            if (tokenString[0] == '/' && tokenString[1] == '*')
                return true;
            return false;
        }

        private static bool isSingleQuoteToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            return (tokenString[0] == '\'');
        }

        private static bool isDoubleQuoteToken(Token tokenString)
        {
            if (tokenString == null || tokenString.Length == 0)
                return false;
            char ch = tokenString[0];

            if (ch == '@')
            {
                char next = tokenString[1];
                return (next == '\"');
            }
            return (ch == '\"');
        }

    }

    public class TokenContext
    {
        internal TokenContext()
        {
            whiteSpaceState = new WhiteSpaceState(this);
            punctuationState = new PunctuationState(this);
            alphaNumericState = new AlphaNumericState(this);
            newLineState = new NewLineState(this);
            singleLineCommentState = new SingleLineCommentState(this);
            multipleLineCommentState = new MultipleLineCommentState(this);
            singleQouteState = new SingleQouteState(this);
            doubleQuoteState = new DoubleQuoteState(this);
            //more to come
            currentTokenState = whiteSpaceState;

        }
        internal WhiteSpaceState whiteSpaceState { get; set; }
        internal PunctuationState punctuationState { get; set; }
        internal AlphaNumericState alphaNumericState { get; set; }
        internal NewLineState newLineState { get; set; }
        internal SingleLineCommentState singleLineCommentState { get; set; }
        internal MultipleLineCommentState multipleLineCommentState { get; set; }
        internal SingleQouteState singleQouteState { get; set; }
        internal DoubleQuoteState doubleQuoteState { get; set; }
        //more to come

        internal TokenState currentTokenState { get; set; }
        internal ITokenSource source { get; set; }

    }

    public abstract class TokenState : ITokenState
    {
        internal HashSet<string> oneCharacterTokens { get; set; }
        internal HashSet<string> twoCharacterTokens { get; set; }
        internal TokenContext context { get; set; }

        public TokenState()
        {
            oneCharacterTokens = new HashSet<string>
            {
                "<", ">", "[", "]", "(", ")", "{", "}", ".", ";", "=", "+", "-", "*"
            };

            twoCharacterTokens = new HashSet<string>
            {
                "<<", ">>", "::", "++", "--", "==", "+=", "-=", "*=", "/=", "&&", "||"
            };
        }

        internal bool AddOneCharacterToken(string oneCharToken)
        {
            if (oneCharToken.Length > 1)
                return false;
            oneCharacterTokens.Add(oneCharToken);
            return true;
        }

        internal bool RemoveOneCharacterToken(string oneCharToken)
        {
            return oneCharacterTokens.Remove(oneCharToken);
        }

        internal bool AddTwoCharacterToken(string twoCharToken)
        {
            if (twoCharToken.Length != 2)
                return false;
            twoCharacterTokens.Add(twoCharToken);
            return true;
        }

        internal bool RemoveTwoCharacterToken(string twoCharToken)
        {
            return twoCharacterTokens.Remove(twoCharToken);
        }

        public bool Open(string path)
        {
            return context.source.Open(path);
        }

        public abstract Token GetToken();

        public bool isWhiteSpace()
        {
            char ch = (char)context.source.Peek();
            return (Char.IsWhiteSpace(ch) && ch != '\n');
        }

        public bool isNewLine()
        {
            return ((char)context.source.Peek() == '\n');
        }

        public bool isAplhaNumeric()
        {
            char ch = (char)context.source.Peek();
            return (Char.IsLetterOrDigit(ch) || ch == '_');
        }

        public bool isPunctuation()
        {
            bool test = isWhiteSpace() || isNewLine() || isAplhaNumeric();
            test = test || isSingleLine() || isMultiLine();
            test = test || isSingleQuote() || isDoubleQuote();
            return !test;

        }

        public bool isSingleLine()
        {
            char first = (char)context.source.Peek();
            char second = (char)context.source.Peek(1);
            return (first == '/' && second == '/');
        }

        public bool isMultiLine()
        {
            char first = (char)context.source.Peek();
            char second = (char)context.source.Peek(1);
            return (first == '/' && second == '*');
        }

        public bool isSingleQuote()
        {
            char ch = (char)context.source.Peek();
            return (ch == '\'');
        }

        public bool isDoubleQuote()
        {
            char ch = (char)context.source.Peek();
            if (ch == '@')
            {
                char next = (char)context.source.Peek(1);
                return (next == '\"');
            }
            return (ch == '\"');
        }

        public TokenState NextTokenState()
        {
            int nextItem = context.source.Peek();

            if (nextItem < 0)
                return null;

            if (isWhiteSpace())
                return context.whiteSpaceState;

            if (isNewLine())
                return context.newLineState;

            if (isAplhaNumeric())
                return context.alphaNumericState;

            if (isSingleLine())
                return context.singleLineCommentState;

            if (isMultiLine())
                return context.multipleLineCommentState;

            if (isSingleQuote())
                return context.singleQouteState;

            if (isDoubleQuote())
                return context.doubleQuoteState;


            return context.punctuationState;
        }

        public bool IsDone()
        {
            if (context.source == null)
                return true;
            return context.source.End();
        }

        public bool isEscaped(Token tokenString)
        {
            int size = tokenString.Length;
            if (size < 2)
                return false;

            int count = 0;
            for (int i = 0; i < size - 1; ++i)
            {
                count = i % 2;
                if (tokenString[size - i - 2] != '\\')
                    break;
            }
            if (count == 0)
                return false;
            return true;
        }
    }

    public class WhiteSpaceState : TokenState
    {
        public WhiteSpaceState(TokenContext _context)
        {
            context = _context;
        }

        override public Token GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());

            while (context.currentTokenState.isWhiteSpace())
            {
                token.Append((char)context.source.Next());
            }
            return token.ToString();
        }
    }

    public class NewLineState : TokenState
    {
        public NewLineState(TokenContext _context)
        {
            context = _context;
        }

        override public string GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());
            return token.ToString();
        }
    }

    public class SingleLineCommentState : TokenState
    {
        public SingleLineCommentState(TokenContext _context)
        {
            context = _context;
        }

        override public string GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());
            token.Append((char)context.source.Next());

            char ch;
            while (true)
            {
                ch = (char)context.source.Peek();
                if (ch == '\n')
                    break;
                token.Append((char)context.source.Next());
            }
            return token.ToString();
        }
    }

    public class MultipleLineCommentState : TokenState
    {
        public MultipleLineCommentState(TokenContext _context)
        {
            context = _context;
        }

        override public string GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());
            token.Append((char)context.source.Next());

            char ch = ' ',prvch = ' ';
            while (true)
            {
                prvch = ch;
                ch = (char)context.source.Next();
                token.Append(ch);
                if (prvch == '*' && ch == '/')
                    break;
            }
            return token.ToString();
        }
    }

    public class SingleQouteState : TokenState
    {
        public SingleQouteState(TokenContext _context)
        {
            context = _context;
        }

        override public Token GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());

            while (true)
            {
                char ch = (char)context.source.Next();
                token.Append(ch);
                if (ch == '\'' && !isEscaped(token.ToString()))
                    break;
            }
            return token.ToString();
        }

    }

    public class DoubleQuoteState : TokenState
    {
        public DoubleQuoteState(TokenContext _context)
        {
            context = _context;
        }

        override public Token GetToken()
        {
            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());
            char next = (char)context.source.Peek();
            if (next == '\"' && token[0] == '@')
                token.Append((char)context.source.Next());

            while (true)
            {
                char ch = (char)context.source.Next();
                token.Append(ch);
                if (ch == '\"' && (!isEscaped(token.ToString()) || token[0] == '@'))
                    break;
            }
            return token.ToString();
        }
    }

    public class PunctuationState : TokenState
    {
        public PunctuationState(TokenContext _context)
        {
            context = _context;
        }
        
        override public Token GetToken()
        {
            StringBuilder test = new StringBuilder();

            test.Append((char)context.source.Peek());
            test.Append((char)context.source.Peek(1));

            if (twoCharacterTokens.Contains(test.ToString()))
            {
                context.source.Next();
                context.source.Next();
                return test.ToString();
            }

            StringBuilder token = new StringBuilder();
            token.Append((char)context.source.Next());
            if (oneCharacterTokens.Contains(token.ToString()))
            {
                return token.ToString();
            }

            while(context.currentTokenState.isPunctuation())
            {
                if (isMultiLine() || isSingleLine() || isSingleQuote() || isDoubleQuote())
                    break;
                token.Append((char)context.source.Next());
            }
            return token.ToString();
        }
    }

    public class AlphaNumericState : TokenState
    {
        public AlphaNumericState(TokenContext _context)
        {
            context = _context;
        }
        

        override public Token GetToken()
        {
            StringBuilder token = new StringBuilder();

            token.Append((char)context.source.Next());

            while (isAplhaNumeric())
            {
                token.Append((char)context.source.Next());
            }
            return token.ToString();
        }
    }

    public class TokenSourceFile : ITokenSource
    {
        public int LineCount { get; set; } = 1;
        public System.IO.StreamReader fileSource;
        private List<int> charQueue = new List<int>();
        public TokenContext context;

        public TokenSourceFile(TokenContext _context)
        {
            context = _context;
        }

        public bool Open(string path)
        {
            try
            {
                fileSource = new System.IO.StreamReader(path,true);
                context.currentTokenState = context.currentTokenState.NextTokenState();
            }
            catch(Exception ex)
            {
                Console.Write("\n {0}\n", ex.Message);
                return false;
            }
            return true;
        }

        public void Close()
        {
            fileSource.Close();   
        }

        public int Next()
        {
            int ch;

            if (charQueue.Count == 0)
            {
                if (End())
                    return -1;
                ch = fileSource.Read();
            }
            else
            {
                ch = charQueue[0];
                charQueue.Remove(ch);
            }
            if ((char)ch == '\n')
                ++LineCount;
            return ch;
        }

        public int Peek(int n = 0)
        {
            if (n < charQueue.Count)
            {
                return charQueue[n];
            }
            else
            {
                for (int i = charQueue.Count; i <= n; ++i) //i++
                {
                    if (End())
                        return -1;
                    charQueue.Add(fileSource.Read());
                }
                return charQueue[n];
            }
        }

        public bool End()
        {
            return fileSource.EndOfStream;
        }
        
    }


    class TokerStub
    {
        static bool testTokenizer(string path)
        {
            Toker toker = new Toker();

            string fileName = System.IO.Path.GetFullPath(path);

            if (!toker.Open(fileName))
            {
                Console.WriteLine("\n Can't open file {0}\n", fileName);
                return false;
            }
            else
            {
                Console.WriteLine("\n Processing file: {0}", fileName);
            }

            while (!toker.IsDone())
            {
                Token singleToken = toker.GetToken();
                Console.Write("\n --Line{0, 4} : {1}", toker.LineCount(), singleToken);
            }
            toker.Close();
            return true;

        }


        static void Main(string[] args)
        {
            Console.WriteLine("Trying to get tokens,\n\n");

            testTokenizer("../../Test.txt");
            //testTokenizer("../../Test2.txt");
            //testTokenizer("../../Toker.cs");

            Console.Write("\n\n");


        }
    }
}
