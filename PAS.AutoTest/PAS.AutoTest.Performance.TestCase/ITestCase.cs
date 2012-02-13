using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.Performance.TestCase
{
    public delegate void SingleCallCompleteEventHandler(object sender, TestCaseResultArgs e);
    public delegate void TestCaseCompleteEventhandler(object  sender, System.EventArgs e);

    public interface ITestCase
    {
        int Repeat{set;get;}
        string FunctionName{set;get;}
        int Executed { set;get;}
        string Label { set;get;}
        int Failed { set;get;}
        double ExecutedTime { get;set;}
        int Average { get;}
        int Concurrent { set;get;}
        void Run();
        void Reset();
        void EndTestCase();
        void LoadConfig();

        event SingleCallCompleteEventHandler SingleCallCompleteEvent;
        event TestCaseCompleteEventhandler TestCaseCompleteEvent;
    }
}
