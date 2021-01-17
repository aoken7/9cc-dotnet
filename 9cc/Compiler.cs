using System;
using System.Collections.Generic;

namespace _9cc
{
    public class Compiler
    {
        public string compile(string input){
            string objctCode = "";
            
            objctCode += ".intel_syntax noprefix\n";
            objctCode += ".globl main\n";
            objctCode += "main:\n";

            var splited = new List<string>();

            int leftIdx = 0, rightIdx = 0;
            for (; rightIdx < input.Length; rightIdx++)
            {
                if(!Char.IsDigit(input[rightIdx]))
                {
                    splited.Add(input.Substring(leftIdx,rightIdx-leftIdx));
                    splited.Add(input[rightIdx].ToString());
                    leftIdx = rightIdx + 1;
                }
            }
            splited.Add(input.Substring(leftIdx,rightIdx-leftIdx));
        
            objctCode += $"\tmov rax, {splited[0]}\n";
            
            for (int i = 1; i < splited.Count; i++)
            {
                if(splited[i] == "+")
                {
                    objctCode += $"\tadd rax, {splited[++i]}\n";
                }

                if(splited[i] == "-")
                {
                    objctCode += $"\tsub rax, {splited[++i]}\n";
                }
            }

            objctCode += "\tret\n";
            
            return objctCode;
        }
    }
}