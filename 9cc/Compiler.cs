using System;
using System.Collections.Generic;

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

            s = s.Replace(" ", "");

            for (; rightIdx < s.Length; rightIdx++)
            {
                if (!Char.IsDigit(s[rightIdx]))
                {
                    splited.Add(s.Substring(leftIdx, rightIdx - leftIdx));
                    splited.Add(s[rightIdx].ToString());
                    leftIdx = rightIdx + 1;
                }
                if (Char.IsWhiteSpace(s, rightIdx))
                {
                    rightIdx++;
                    leftIdx = rightIdx;
                }
            }
            splited.Add(s.Substring(leftIdx, rightIdx - leftIdx));
            return splited;
        }


        void tokenize(string p)
        {
            int splitedIndex = 0;
            var splited = split_expr(p);

            while (splitedIndex < splited.Count)
            {

                if (splited[splitedIndex] == "+" || splited[splitedIndex] == "-")
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

                Console.Error.WriteLine(string.Join("",splited));
                Console.Error.WriteLine($"{new String(' ', splitedIndex-1)} ^");
                Console.Error.WriteLine("Can't tokenize.");
                Environment.Exit(-1);
            }

            TokenList.Add(new_token(TokenKind.TK_EOF, ""));
        }


        public string compile(string input)
        {
            string objctCode = "";

            tokenize(input);

            objctCode += ".intel_syntax noprefix\n";
            objctCode += ".globl main\n";
            objctCode += "main:\n";

            objctCode += $"\tmov rax, {expect_number()}\n";

            while (!at_eof())
            {
                if (consume('+'))
                {
                    objctCode += $"\tadd rax, {expect_number()}\n";
                    continue;
                }

                expect('-');
                objctCode += $"\tsub rax, {expect_number()}\n";
            }

            objctCode += "\tret\n";

            return objctCode;
        }
    }
}