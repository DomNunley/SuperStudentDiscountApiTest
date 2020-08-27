using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestRunnerApi.Models
{
    public class TestRunResults
    {
        public bool HasAllTestRunsPass { get; set; }
        public IEnumerable<TestRun> TestRuns { get; set; }
    }
}
