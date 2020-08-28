using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseGenerator;

namespace TestRunnerApi.Models
{
    public class ComboTestRunResults<T> where T : ITestCase, new()
    {
        public bool HasAllTestRunsPass { get; set; }
        public IEnumerable<ComboTestRun<T>> TestRuns { get; set; }
    }
}
