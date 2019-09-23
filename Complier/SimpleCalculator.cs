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

        }

        /**
         * 对某个AST节点求值，并打印求值过程。
         * @param node
         * @param indent  打印输出时的缩进量，用tab控制
         * @return
         */
        private int evaluate(IASTNode node, string indent)
        {
            return 1;
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
