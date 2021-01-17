using System;

namespace _9cc
{
    public class Compiler
    {
        public string compile(string input){
            string objctCode = "";
            
            objctCode += ".intel_syntax noprefix\n";
            objctCode += ".globl main\n";
            objctCode += "main:\n";
            objctCode += $"\tmov rax, {input}\n";
            objctCode += "\tret\n";
            
            return objctCode;
        }
    }
}