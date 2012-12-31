using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace PAS.AutoTest.Automan
{
    public static class Utility
    {
        public static ArrayList GenerateMethodList()
        {
            //string[] methods = new string[];
            ArrayList methodList = new ArrayList();
            Type t = typeof(PAS.AutoTest.TestCase.Runner);
            string className = t.Name;

            MethodInfo[] methods = t.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.Name.Contains("Run"))
                {
                    methodList.Add(method.Name);
                }
            }

            return methodList;
        }
    }
}