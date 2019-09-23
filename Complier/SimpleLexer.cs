using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://github.com/RichardGong/PlayWithCompiler/blob/master/lab/craft
namespace Complier
{
    // 词法分析器
    public class SimpleLexer
    {
        //public static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello SimpleLexer");
        //    SimpleLexer lexer = new SimpleLexer();

        //    //StringReader reader = new StringReader(script);
        //    //while (reader.Peek() != -1)
        //    //{
        //    //    int temp = reader.Read();
        //    //    Console.WriteLine(temp);
        //    //    Console.WriteLine(Convert.ToChar(temp));
        //    //}

        //    string script = "int age = 45;";
        //    Console.WriteLine("parse : " + script);
        //    SimpleTokenReader tokenReader = lexer.tokenize(script);
        //    dump(tokenReader);

        //    script = "inta age = 45";
        //    Console.WriteLine("parse : " + script);
        //    tokenReader = lexer.tokenize(script);
        //    dump(tokenReader);

        //    script = "in age = 45";
        //    Console.WriteLine("parse : " + script);
        //    tokenReader = lexer.tokenize(script);
        //    dump(tokenReader);

        //    script = "age >= 45";
        //    Console.WriteLine("parse : " + script);
        //    tokenReader = lexer.tokenize(script);
        //    dump(tokenReader);

        //    script = "age > 45";
        //    Console.WriteLine("parse : " + script);
        //    tokenReader = lexer.tokenize(script);
        //    dump(tokenReader);

        //    Console.ReadKey();
        //}

        //下面几个变量是在解析过程中用到的临时变量,如果要优化的话，可以塞到方法里隐藏起来
        private StringWriter tokenText = null;   //临时保存token的文本
        private List<IToken> tokens = null;       //保存解析出来的Token
        private SimpleToken token = null;        //当前正在解析的Token

        /**
        * 解析字符串，形成Token。（就是把字符串，按照token的定义，翻译成一个个token）
        * 这是一个有限状态自动机，在不同的状态中迁移。
        * @param code
        * @return
        */
        public SimpleTokenReader tokenize(string code)
        {
            tokens = new List<IToken>();
            StringReader code_reader = new StringReader(code);
            tokenText = new StringWriter();
            token = new SimpleToken();
            int ich = 0;
            char ch = Convert.ToChar(ich);
            DfaState state = DfaState.Initial;
            try
            {
                while ((ich = code_reader.Read()) != -1)
                {
                    ch = Convert.ToChar(ich);
                    switch (state)
                    {
                        case DfaState.Initial:
                            state = initToken(ch);
                            break;
                        case DfaState.Id:
                            if (isAlpha(ch) || (isDigit(ch)))
                            {
                                tokenText.Write(ch);
                            }
                            else
                            {
                                state = initToken(ch);
                            }
                            break;
                        case DfaState.GT:
                            if (ch == '=')
                            {
                                token.type = TokenType.GE;  //转换成GE
                                state = DfaState.GE;
                                tokenText.Write(ch);
                            }
                            else
                            {
                                state = initToken(ch);      //退出GT状态，并保存Token
                            }
                            break;
                        case DfaState.GE:
                        case DfaState.Assignment:
                        case DfaState.Plus:
                        case DfaState.Minus:
                        case DfaState.Star:
                        case DfaState.Slash:
                        case DfaState.SemiColon:
                        case DfaState.LeftParen:
                        case DfaState.RightParen:
                            state = initToken(ch);          //退出当前状态，并保存Token
                            break;
                        case DfaState.IntLiteral:
                            if (isDigit(ch))
                            {
                                tokenText.Write(ch);       //继续保持在数字字面量状态
                            }
                            else
                            {
                                state = initToken(ch);      //退出当前状态，并保存Token
                            }
                            break;
                        case DfaState.Id_int1:
                            if (ch == 'n')
                            {
                                state = DfaState.Id_int2;
                                tokenText.Write(ch);
                            }
                            else if (isDigit(ch) || isAlpha(ch))
                            {
                                state = DfaState.Id;    //切换回Id状态
                                tokenText.Write(ch);
                            }
                            else
                            {
                                state = initToken(ch);
                            }
                            break;
                        case DfaState.Id_int2:
                            if (ch == 't')
                            {
                                state = DfaState.Id_int3;
                                tokenText.Write(ch);
                            }
                            else if (isDigit(ch) || isAlpha(ch))
                            {
                                state = DfaState.Id;    //切换回id状态
                                tokenText.Write(ch);
                            }
                            else
                            {
                                state = initToken(ch);
                            }
                            break;
                        case DfaState.Id_int3:
                            if (isBlank(ch))
                            {
                                token.type = TokenType.Int;
                                state = initToken(ch);
                            }
                            else
                            {
                                state = DfaState.Id;    //切换回Id状态
                                tokenText.Write(ch);
                            }
                            break;
                        default:
                            Console.WriteLine("Unexpected state: {0}", state);
                            break;
                  
                    }
                }

                if(tokenText.ToString() != "")
                {
                    initToken(ch);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(
                "{0}: The write operation could not be performed ",
                e.GetType().Name);
            }

            return new SimpleTokenReader(tokens);
        }

        /**
        * 有限状态机进入初始状态。
        * 这个初始状态其实并不做停留，它马上进入其他状态。
        * 开始解析的时候，进入初始状态；某个Token解析完毕，也进入初始状态，在这里把Token记下来，然后建立一个新的Token。
        * @param ch
        * @return
        */
        private DfaState initToken(char ch)
        {
            if(tokenText.ToString() != "")
            {
                token.text = tokenText.ToString();
                tokens.Add(token);

                tokenText.Close();
                tokenText = new StringWriter();
                token = new SimpleToken();
            }

            DfaState newState = DfaState.Initial;
            if (isAlpha(ch)){
                if(ch == 'i')
                {
                    newState = DfaState.Id_int1;
                }
                else
                {
                    newState = DfaState.Id;
                }
                token.type = TokenType.Identifier;
                tokenText.Write(ch);
            }else if (isDigit(ch))
            {
                newState = DfaState.IntLiteral;
                token.type = TokenType.IntLiteral;
                tokenText.Write(ch);
            }else if (ch == '>')
            {
                newState = DfaState.GT;
                token.type = TokenType.GT;
                tokenText.Write(ch);
            }
            else if (ch == '+')
            {
                newState = DfaState.Plus;
                token.type = TokenType.Plus;
                tokenText.Write(ch);
            }
            else if (ch == '-')
            {
                newState = DfaState.Minus;
                token.type = TokenType.Minus;
                tokenText.Write(ch);
            }
            else if (ch == '*')
            {
                newState = DfaState.Star;
                token.type = TokenType.Star;
                tokenText.Write(ch);
            }
            else if (ch == '/')
            {
                newState = DfaState.Slash;
                token.type = TokenType.Slash;
                tokenText.Write(ch);
            }
            else if (ch == ';')
            {
                newState = DfaState.SemiColon;
                token.type = TokenType.SemiColon;
                tokenText.Write(ch);
            }
            else if (ch == '(')
            {
                newState = DfaState.LeftParen;
                token.type = TokenType.LeftParen;
                tokenText.Write(ch);
            }
            else if (ch == ')')
            {
                newState = DfaState.RightParen;
                token.type = TokenType.RightParen;
                tokenText.Write(ch);
            }
            else if (ch == '=')
            {
                newState = DfaState.Assignment;
                token.type = TokenType.Assignment;
                tokenText.Write(ch);
            }
            else
            {
                newState = DfaState.Initial;
            }
            return newState;
        }

        /**
         * 打印所有的Token
         * @param tokenReader
         */
        public static void dump(SimpleTokenReader tokenReader)
        {
            Console.WriteLine("text\ttype");
            IToken token = null;
            while ((token = tokenReader.read()) != null)
            {
                Console.WriteLine(token.getText() + "\t\t" + token.getType());
            }
        }

        //是否是字母
        private bool isAlpha(int ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
        }

        //是否是数字
        private bool isDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        //是否是空白字符
        private bool isBlank(int ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n';
        }

        private class SimpleToken : IToken
        {
            public TokenType type = TokenType.Empty;
            public string text = null;
            public TokenType getType()
            {
                return type;
            }
            public string getText()
            {
                return text;
            }

        }

        public class SimpleTokenReader : ITokenReader
        {
            List<IToken> tokens = null;
            int pos = 0;

            public SimpleTokenReader(List<IToken> tokens)
            {
                this.tokens = tokens;
            }

            public IToken read()
            {
                if (pos < tokens.Count)
                {
                    return tokens[pos++];
                }
                return null;
            }

            public IToken peek()
            {
                if (pos < tokens.Count)
                {
                    return tokens[pos];
                }
                return null;
            }

            public void unread()
            {
                if (pos > 0)
                {
                    pos--;
                }
            }

            public int getPosition()
            {
                return pos;
            }

            public void setPosition(int position)
            {
                if (position >= 0 && position < tokens.Count)
                {
                    pos = position;
                }
            }
        }

        private enum DfaState{
            Initial,

            If, Id_if1, Id_if2, Else, Id_else1, Id_else2, Id_else3, Id_else4, Int, Id_int1, Id_int2, Id_int3, Id, GT, GE,

            Assignment,

            Plus, Minus, Star, Slash,

            SemiColon,
            LeftParen,
            RightParen,

            IntLiteral
        }
    }
}
