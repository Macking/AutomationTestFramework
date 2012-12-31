using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.Generator
{
    public static partial class PasATCoreV2Generator
    {
        public static void Initialize()
        {
            GenarateClassFile();

            GeneratePasATCoreV2Lib();
        }
    }
}
