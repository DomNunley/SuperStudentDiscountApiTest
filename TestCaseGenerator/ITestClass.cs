using System;
using System.Collections.Generic;
using System.Text;

namespace TestCaseGenerator
{
    public interface ITestClass
    {
        IEnumerable<ITestCase> FailedTestCases { get; }
    }
}
