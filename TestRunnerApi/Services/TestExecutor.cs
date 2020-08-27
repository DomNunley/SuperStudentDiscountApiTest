using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestRunnerApi.Models;
using Xunit;
using Xunit.Sdk;

namespace TestRunnerApi.Services
{
    public static class TestExecutor
    {
        public static async Task<TestRunResults> ExecuteTests(Type testClassType)
        {
            var testMethods = testClassType.GetMethods();
            List<TestRun> runs = new List<TestRun>();
            bool allTestRunsPass = true;

            foreach(MethodInfo methodInfo in testMethods)
            {
                var attribute = methodInfo.GetCustomAttribute<FactAttribute>();
                if(attribute != null)
                {
                    var obj = Activator.CreateInstance(testClassType);
                    TestRun testRun = new TestRun();
                    testRun.TestName = methodInfo.Name;

                    try
                    {
                        Task result = (Task)methodInfo.Invoke(obj, null);
                        await result;
                        testRun.HasPassed = true;
                    }
                    catch(Exception e)
                    {
                        testRun.HasPassed = false;
                        allTestRunsPass = false;
                        testRun.Message = e.Message;
                    }
                    finally
                    {
                        runs.Add(testRun);
                    }
                }
            }

            return new TestRunResults { TestRuns = runs, HasAllTestRunsPass = allTestRunsPass };
        }
    }
}
