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
        public static async Task<ComboTestRunResults<T>> ComboExecuteTests<T>(Type testClassType) where T : ITestCase, new()
        {
            var testMethods = testClassType.GetMethods();
            List<ComboTestRun<T>> runs = new List<ComboTestRun<T>>();
            bool allTestRunsPass = true;

            foreach(MethodInfo methodInfo in testMethods)
            {
                var attribute = methodInfo.GetCustomAttribute<FactAttribute>();
                if(attribute != null)
                {
                    var obj = (ITestClass)Activator.CreateInstance(testClassType);
                    ComboTestRun<T> testRun = new ComboTestRun<T>();
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

            return new ComboTestRunResults<T> { TestRuns = runs, HasAllTestRunsPass = allTestRunsPass };
        }

        public static async Task<TestRunResults> ExecuteTests(Type testClassType)
        {
            var testMethods = testClassType.GetMethods();
            List<TestRun> runs = new List<TestRun>();
            bool allTestRunsPass = true;

            foreach (MethodInfo methodInfo in testMethods)
            {
                var attribute = methodInfo.GetCustomAttribute<FactAttribute>();
                if (attribute != null)
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
                    catch (Exception e)
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
