using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MethodSourceCodeParser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the test case cs files list
            string currentPath = Directory.GetCurrentDirectory(); // e.g.: E:\Trunk\Test_Pas\Autotest\Build\Release or E:\Trunk\Test_Pas\Autotest\Build\Debug
            string sourceFielPath = string.Empty;

            if (currentPath.Contains(@"Build\Release"))
            {
                sourceFielPath = currentPath.Replace(@"Build\Release", "") + @"\PAS.AutoTest\PAS.AutoTest.TestCase\";
            }
            else
            {
                sourceFielPath = currentPath.Replace(@"Build\Debug", "")+ @"\PAS.AutoTest\PAS.AutoTest.TestCase\";
            }

            if (!Directory.Exists(sourceFielPath))
            {
                return;
            }

            string[] sourceFiles = Directory.GetFiles(sourceFielPath, "*.cs");

            foreach (string sourceFile in sourceFiles)
            {
                string sourceCode = File.ReadAllText(sourceFile);
                HandleSourceFileForMothodContent(sourceCode);
            }
        }

        private static void HandleSourceFileForMothodContent(string sourceFile)
        {
            // Please refer to below link for more info about how this works
            // http://stackoverflow.com/questions/7455329/regex-to-match-method-content
            // http://www.codeproject.com/Articles/21080/In-Depth-with-RegEx-Matching-Nested-Constructions
            string methodReg = @"
                                                (?<body>
                                                public.*void.*Case.*\s*
                                                \{(?<DEPTH>)
                                                (?>
                                                (?<DEPTH>)\{
                                                |
                                                \}(?<-DEPTH>)  
                                                 |
                                                (?(DEPTH)[^\{\}]* | )
                                                )*
                                                \}(?<-DEPTH>)
                                                (?(DEPTH)(?!))
                                                )";

            foreach (Match m in Regex.Matches(sourceFile, methodReg, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace))
            {
                string code = m.Groups["body"].Value;

                string methodNameReg = @"Run.*Case.*\(\)";

                string methodName = Regex.Match(code, methodNameReg, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace).Value.Replace("()", "");

                if (methodName.ToLower().Contains("case"))
                {
                    string path = "TestData\\" + methodName + ".cod";

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    //Replace the method name to Run
                    code = HandleMethodName(code);

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(code);
                    }
                }
            }
        }

        private static string HandleMethodName(string code)
        {
            int start = code.ToLower().IndexOf("run");
            int end = code.ToLower().IndexOf("()");

            if (end<=start)
            {
                code = "public void Run(){}";
            }

            string subString = code.Substring(start, end-start);
            code = code.Replace(subString,"Run");

            return code;
        }
    }
}