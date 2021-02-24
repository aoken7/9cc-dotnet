using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _9cc
{
    public class Compiler
    {
        enum TokenKind
        {
            TK_RESERVED,
            TK_NUM,
            TK_EOF,
        }

        struct Token
        {
            public TokenKind kind;
            public int val;
            public string str;
        }

        List<Token> TokenList = new List<Token>();
        int tokenIndex = 0;

        bool consume(char op)
        {
            if (TokenList[tokenIndex].kind != TokenKind.TK_RESERVED || TokenList[tokenIndex].str[0] != op)
            {
                return false;
            }
            tokenIndex++;
            return true;
        }

        void expect(char op)
        {
            if (TokenList[tokenIndex].kind != TokenKind.TK_RESERVED || TokenList[tokenIndex].str[0] != op)
            {
                Console.Error.WriteLine($"{op}ではありません");
                Environment.Exit(-1);
            }
            tokenIndex++;
        }

        int expect_number()
        {
            if (TokenList[tokenIndex].kind != TokenKind.TK_NUM)
            {
                Console.Error.WriteLine("数ではありません");
                Environment.Exit(-1);
            }
            //tokenIndex++;
            return TokenList[tokenIndex++].val;
        }

        bool at_eof()
        {
            return TokenList[tokenIndex].kind == TokenKind.TK_EOF;
        }

        Token new_token(TokenKind kind, string str)
        {
            Token tok;
            tok.kind = kind;
            tok.str = str;
            tok.val = 0;
            //TokenList.Add(tok);
            return tok;
        }

        List<string> split_expr(string s)
        {
            var splited = new List<string>();
            int leftIdx = 0, rightIdx = 0;

            s = Regex.Replace(s,@"\s+"," ");

            if (!Char.IsDigit(s[0]))
            {
                rightIdx++;
                leftIdx++;
                if(s[0].ToString() != " ") splited.Add(s[0].ToString());
            }

            for (; rightIdx < s.Length; rightIdx++)
            {
                if (!Char.IsDigit(s[rightIdx]))
                {
                    if (leftIdx < rightIdx)
                    {
                        splited.Add(s.Substring(leftIdx, rightIdx - leftIdx));
                    }
                    if(s[rightIdx].ToString() != " ") splited.Add(s[rightIdx].ToString());
                    leftIdx = rightIdx + 1;
                }

            }

            if (Char.IsDigit(s[s.Length - 1]))
            {
                splited.Add(s.Substring(leftIdx, rightIdx - leftIdx));
            }
            return splited;
        }


        void tokenize(string p)
        {
            int splitedIndex = 0;
            var splited = split_expr(p);

            while (splitedIndex < splited.Count)
            {
                if (Regex.IsMatch(splited[splitedIndex], "[+-/()*]"))
                {
                    var token = new_token(TokenKind.TK_RESERVED, splited[splitedIndex++]);
                    TokenList.Add(token);
                    continue;
                }

                int val;
                if (int.TryParse(splited[splitedIndex], out val))
                {
                    var token = new_token(TokenKind.TK_NUM, p);
                    token.val = val;
                    TokenList.Add(token);
                    splitedIndex++;
                    continue;
                }

                Console.Error.WriteLine(string.Join("", splited));
                //Console.Error.WriteLine($"{new String(' ', splitedIndex - 1)} ^");
                Console.Error.WriteLine("Can't tokenize.");
                Environment.Exit(-1);
            }

            TokenList.Add(new_token(TokenKind.TK_EOF, ""));
        }

        enum NodeKind
        {
            ND_ADD,
            ND_SUB,
            ND_MUL,
            ND_DIV,
            ND_NUM,
        }

        struct Node
        {
            public NodeKind kind;
            public int lhs;
            public int rhs;
            public int val;
        }

        List<Node> absTree = new List<Node>();

        Node new_node(NodeKind kind, int lhs, int rhs)
        {
            Node node = new Node();
            node.kind = kind;
            node.lhs = lhs;
            node.rhs = rhs;
            return node;
        }

        Node new_node_num(int val)
        {
            Node node = new Node();
            node.kind = NodeKind.ND_NUM;
            node.val = val;
            return node;
        }

        int primary()
        {
            if (consume('('))
            {
                int index = expr();
                expect(')');
                return index;
            }
            var nodeNum = new_node_num(expect_number());
            absTree.Add(nodeNum);
            return absTree.Count - 1;
        }

        int mul()
        {
            Node node = new Node();
            node.lhs = primary();
            for (; ; )
            {
                if (consume('*'))
                {
                    node = new_node(NodeKind.ND_MUL, node.lhs, primary());
                    absTree.Add(node);
                    node.lhs = absTree.Count - 1;
                }
                else if (consume('/'))
                {
                    node = new_node(NodeKind.ND_DIV, node.lhs, primary());
                    absTree.Add(node);
                    node.lhs = absTree.Count - 1;
                }
                else
                {
                    return absTree.Count - 1;
                }
            }
        }

        int expr()
        {
            Node node = new Node();
            node.lhs = mul();
            for (; ; )
            {
                if (consume('+'))
                {
                    node = new_node(NodeKind.ND_ADD, node.lhs, mul());
                    absTree.Add(node);
                    node.lhs = absTree.Count - 1;
                }
                else if (consume('-'))
                {
                    node = new_node(NodeKind.ND_SUB, node.lhs, mul());
                    absTree.Add(node);
                    node.lhs = absTree.Count - 1;
                }
                else
                {
                    return absTree.Count - 1;
                }
            }
        }

        string objctCode = "";

        void gen(Node node)
        {
            if (node.kind == NodeKind.ND_NUM)
            {
                objctCode += $"\tpush {node.val}\n";
                return;
            }

            gen(absTree[node.lhs]);
            gen(absTree[node.rhs]);

            objctCode += $"\tpop rdi\n";
            objctCode += $"\tpop rax\n";

            switch (node.kind)
            {
                case NodeKind.ND_ADD:
                    objctCode += $"\tadd rax, rdi\n";
                    break;
                case NodeKind.ND_SUB:
                    objctCode += $"\tsub rax, rdi\n";
                    break;
                case NodeKind.ND_MUL:
                    objctCode += $"\timul rax, rdi\n";
                    break;
                case NodeKind.ND_DIV:
                    objctCode += $"\tcqo\n";
                    objctCode += $"\tidiv rdi\n";
                    break;
            }
            objctCode += $"\tpush rax\n";
        }

        public string compile(string input)
        {

            tokenize(input);

            int index = expr();

            objctCode += ".intel_syntax noprefix\n";
            objctCode += ".globl main\n";
            objctCode += "main:\n";

            gen(absTree[index]);

            objctCode += "\tpop rax\n";
            objctCode += "\tret\n";

            return objctCode;
        }
    }
}