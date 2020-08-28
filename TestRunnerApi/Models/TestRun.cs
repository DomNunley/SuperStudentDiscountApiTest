using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseGenerator;

namespace TestRunnerApi.Models
{
    public class TestRun<T> where T : ITestCase, new()
    {
        public string TestName { get; set; }

        public bool HasPassed { get; set; }

        public IEnumerable<T> FailedTestCases { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
