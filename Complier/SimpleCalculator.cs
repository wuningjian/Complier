using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complier
{
    // 实现一个计算器
    class SimpleCalculator
    {
        public static void Main(string[] args)
        {
            SimpleCalculator calculator = new SimpleCalculator();

            String script = "int a = b + 3;";
            Console.WriteLine("解析变量声明语句：" + script);
            SimpleLexer lexer = new SimpleLexer();
            ITokenReader tokens = lexer.tokenize(script);
            try
            {
                SimpleASTNode node = calculator.intDeclare(tokens);
                calculator.dumpAST(node, "");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            script = "2+3*5";
            Console.WriteLine("\n计算：{0}", script);
            calculator.evaluate(script);

            script = "2+";
            Console.WriteLine("\n计算：{0}", script);
            calculator.evaluate(script);

            script = "2+3+4*2+10+10/5";
            Console.WriteLine("\n计算：{0}", script);
            calculator.evaluate(script);

            Console.ReadKey();
        }

        private class SimpleASTNode:IASTNode
        {
            SimpleASTNode parent = null;
            List<IASTNode> children = new List<IASTNode>();
            ASTNodeType nodeType;
            string text = null;

            public SimpleASTNode(ASTNodeType nodeType, String text)
            {
                this.nodeType = nodeType;
                this.text = text;
            }

            public IASTNode getParent()
            {
                return parent;
            }

            public IList<IASTNode> getChildren()
            {
                IList<IASTNode> readonlyChildren = children.AsReadOnly();
                return readonlyChildren;
            }

            public ASTNodeType getType()
            {
                return nodeType;
            }

            public string getText()
            {
                return text;
            }

            public void addChild(SimpleASTNode child)
            {
                children.Add(child);
                child.parent = this;
            }
        }

        /**
         * 执行脚本，并打印输出AST和求值过程。
         * @param script
         */
        public void evaluate(string script)
        {
            try
            {
                IASTNode tree = parse(script);
                dumpAST(tree, "");
                evaluate(tree, "");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /**
         * 对某个AST节点求值，并打印求值过程。
         * @param node
         * @param indent  打印输出时的缩进量，用tab控制
         * @return
         */
        private int evaluate(IASTNode node, string indent)
        {
            int result = 0;
            Console.WriteLine(indent + "Calculating: " + node.getType());
            switch (node.getType())
            {
                case ASTNodeType.Programm:
                    foreach(IASTNode child in node.getChildren())
                    {
                        result = evaluate(child, indent + "\t");
                    }
                    break;
                case ASTNodeType.Additive:
                    IASTNode child1 = node.getChildren()[0];
                    int value1 = evaluate(child1, indent + "\t");
                    IASTNode child2 = node.getChildren()[1];
                    int value2 = evaluate(child2, indent + "\t");
                    if (node.getText() == "+")
                    {
                        result = value1 + value2;
                    }
                    else
                    {
                        result = value1 - value2;
                    }
                    break;
                case ASTNodeType.Multiplicative:
                    child1 = node.getChildren()[0];
                    value1 = evaluate(child1, indent + "\t");
                    child2 = node.getChildren()[1];
                    value2 = evaluate(child2, indent + "\t");
                    if (node.getText() == "*")
                    {
                        result = value1 * value2;
                    }
                    else
                    {
                        result = value1 / value2;
                    }
                    break;
                case ASTNodeType.IntLiteral:
                    result = Convert.ToInt32(node.getText());
                    break;
                default:
                    break;
            }
            Console.WriteLine(indent + "结果：" + result);
            return result;
        }

        /*
         * 整型变量声明语句，如：int a; int b = 2*3;
         * **/
        private SimpleASTNode intDeclare(ITokenReader tokens)
        {
            SimpleASTNode node = null;
            IToken token = tokens.peek();
            if (token.getType() == TokenType.Int)
            {
                tokens.read();
                token = tokens.peek();
                if (token.getType() == TokenType.Identifier)
                {
                    token = tokens.read();
                    node = new SimpleASTNode(ASTNodeType.IntDeclaration, token.getText());
                    token = tokens.peek();
                    if (token.getType() == TokenType.Assignment)
                    {
                        tokens.read();
                        SimpleASTNode child = additive(tokens);
                        if (child != null)
                        {
                            node.addChild(child);
                        }
                        else
                        {
                            throw (new FormatException("'='赋值表达式错误"));
                        }
                    }
                }
                else
                {
                    throw (new FormatException("需要一个变量名"));
                }

                if (node != null)
                {
                    token = tokens.peek();
                    if (token != null && token.getType() == TokenType.SemiColon)
                    {
                        tokens.read();
                    }
                    else
                    {
                        throw (new FormatException("表达式缺少分号"));
                    }
                }
            }
            return node;
        }

        /*
         * 语法解析：根节点
         * **/
        private SimpleASTNode prog(ITokenReader tokens)
        {
            SimpleASTNode node = new SimpleASTNode(ASTNodeType.Programm, "Calculator");
            SimpleASTNode child = additive(tokens);
            if (child != null)
            {
                node.addChild(child);
            }
            return node;
        }

        /*
         * 解析脚本，并返回根节点
         * @param code
         */
        public IASTNode parse(string code)
        {
            SimpleLexer lexer = new SimpleLexer();
            ITokenReader tokens = lexer.tokenize(code);

            IASTNode rootNode = prog(tokens);

            return rootNode;
        }


        /*
         * 语法解析：加法表达式
         * **/
        private SimpleASTNode additive(ITokenReader tokens)
        {
            SimpleASTNode child1 = multiplicative(tokens);
            SimpleASTNode node = child1;

            IToken token = tokens.peek();
            if(child1 != null && token != null)
            {
                if(token.getType() == TokenType.Plus || token.getType() == TokenType.Minus)
                {
                    token = tokens.read();
                    SimpleASTNode child2 = additive(tokens);
                    if (child2 != null)
                    {
                        node = new SimpleASTNode(ASTNodeType.Additive, token.getText());
                        node.addChild(child1);
                        node.addChild(child2);
                    }
                    else
                    {
                        throw (new FormatException("无效的加法表达式!"));
                    }
                }
            }
            return node;
        }

        /*
         * 语法解析：乘法表达式
         * **/
        private SimpleASTNode multiplicative(ITokenReader tokens)
        {
            SimpleASTNode child1 = primary(tokens);
            SimpleASTNode node = child1;

            IToken token = tokens.peek();
            if(child1 != null && token != null)
            {
                if(token.getType() == TokenType.Star || token.getType() == TokenType.Slash)
                {
                    token = tokens.read();
                    SimpleASTNode child2 = primary(tokens);
                    if(child2 != null)
                    {
                        node = new SimpleASTNode(ASTNodeType.Multiplicative, token.getText());
                        node.addChild(child1);
                        node.addChild(child2);
                    }
                    else
                    {
                        throw (new FormatException("无效的乘法表达式!"));
                    }
                }
            }
            return node;
        }

        /*
         * 语法解析：基础表达式
         * **/
        private SimpleASTNode primary(ITokenReader tokens)
        {
            SimpleASTNode node = null;
            IToken token = tokens.peek();
            if(token != null)
            {
                if(token.getType() == TokenType.IntLiteral)
                {
                    token = tokens.read();
                    node = new SimpleASTNode(ASTNodeType.IntLiteral, token.getText());
                }else if (token.getType() == TokenType.Identifier)
                {
                    token = tokens.read();
                    node = new SimpleASTNode(ASTNodeType.Identifier, token.getText());
                }else if (token.getType() == TokenType.LeftParen)
                {
                    token = tokens.read();
                    node = additive(tokens);
                    if (node != null)
                    {
                        token = tokens.peek();
                        if(token!=null && token.getType() == TokenType.RightParen)
                        {
                            tokens.read();
                        }
                        else
                        {
                            throw (new FormatException("缺少右括号"));
                        }
                    }
                    else
                    {
                        throw (new FormatException("无效的表达式"));
                    }
                }
            }
            return node;
        }

        /*
         * 打印输出AST的树状结构
         * @param node
         * @param indent 缩进字符，由tab组成，每一级多一个tab
         */
        private int recurCount = 0;
        private void dumpAST(IASTNode node, string indent)
        {
            Console.WriteLine(indent + node.getType() + " " + node.getText());
            recurCount++;
            if (recurCount < 9999) // 递归次数限制，防止过深递归
            {
                foreach(IASTNode childNode in node.getChildren())
                {
                    dumpAST(childNode, indent + "\t");
                }
            }
            else
            {
                Console.WriteLine("递归层数超过9999");
            }
        }
    }
}
