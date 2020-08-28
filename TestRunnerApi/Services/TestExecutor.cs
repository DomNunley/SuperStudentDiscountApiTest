using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestCaseGenerator;
using TestRunnerApi.Models;
using Xunit;
using Xunit.Sdk;

namespace TestRunnerApi.Services
{
    public static class TestExecutor
    {
        public static async Task<TestRunResults<T>> ExecuteTests<T>(Type testClassType) where T : ITestCase, new()
        {
            var testMethods = testClassType.GetMethods();
            List<TestRun<T>> runs = new List<TestRun<T>>();
            bool allTestRunsPass = true;

            foreach(MethodInfo methodInfo in testMethods)
            {
                var attribute = methodInfo.GetCustomAttribute<FactAttribute>();
                if(attribute != null)
                {
                    var obj = (ITestClass)Activator.CreateInstance(testClassType);
                    TestRun<T> testRun = new TestRun<T>();
                    testRun.TestName = methodInfo.Name;

                    try
                    {
                        Task result = (Task)methodInfo.Invoke(obj, null);
                        await result;
                        testRun.HasPassed = true;
                    }
                    catch(TrueException)
                    {
                        testRun.HasPassed = false;
                        allTestRunsPass = false;
                        testRun.FailedTestCases = obj.FailedTestCases as IEnumerable<T>;
                    }
                    catch(Exception e)
                    {
                        testRun.HasPassed = false;
                        allTestRunsPass = false;
                        testRun.ExceptionMessage = e.Message;
                    }
                    finally
                    {
                        runs.Add(testRun);
                    }
                }
            }

            return new TestRunResults<T> { TestRuns = runs, HasAllTestRunsPass = allTestRunsPass };
        }
    }
}
