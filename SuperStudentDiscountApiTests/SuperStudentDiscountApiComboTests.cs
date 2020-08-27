using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TestCaseGenerator;
using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;

namespace SuperStudentDiscountApiTests
{
    public class SuperStudentDiscountApiComboTests
    {
        [Fact]
        public async Task SuperStudentQualifiesForDiscountAsync()
        {
            #region Define Inputs
            InputDefinition state = new InputDefinition("State", "OH", new object[] { "OH", "AZ", "CO" });
            InputDefinition age = new InputDefinition("DriverAge", 16, new object[] { 16, 17, 18, 19, 29, 30, 31 });
            InputDefinition gpa = new InputDefinition("DriverGPA", 3.50, new object[] { 3.50, 3.49, 3.51, 3.79, 3.80, 3.81, 4.0 });
            InputDefinition studentStatus = new InputDefinition("StudentStatus", "College-enrolled", new object[] { "College-enrolled", "HighSchool-graduated", "None" });
            InputDefinition maritialStatus = new InputDefinition("MaritalStatus", "Single", new object[] { "Single", "Married", "Divorced", "Separated" });
            InputDefinition violation = new InputDefinition("Violations", new List<string> { }, new List<List<string>> { new List<string> { }, new List<string> { "Speeding", "AtFault" }, new List<string> { "Comp" } });
            InputDefinition relationship = new InputDefinition("Relationship", "Child", new object[] { "Child", "NamedInsured", "Spouse", "Other" });
            List<InputDefinition> inputs = new List<InputDefinition> { age, gpa, studentStatus, maritialStatus, violation, relationship };
            #endregion

            #region Setup Combination Generator and Generate Test Cases
            //Pass inputs to combination generator and select technique
            TestComboGen testComboGen = new TestComboGen(inputs);
            var testCases = testComboGen.GenerateNfatTestCases<SuperStudentDiscountApiTestCase>(1).ToList();
            #endregion

            bool hasFailures = false;

            #region Execute Test Cases and Log Results
            for(int i = 0; i < testCases.Count(); i++)
            {
                #region Get Expected Results From Oracle and Get Actual Results From SUT
                SuperStudentDiscountOracle myOracle = new SuperStudentDiscountOracle(testCases[i]);
                var responseMessage = await TestCaseToHttpConverter.ConvertTestCaseToHttpPOST<SuperStudentDiscountApiTestCase>("http://54.210.38.124/service/superstudentdiscount", testCases[i]);
                SuperStudentDiscountResult discountResult = JsonConvert.DeserializeObject<SuperStudentDiscountResult>(await responseMessage.Content.ReadAsStringAsync());
                bool qualifiesForDiscountActual = discountResult.DiscountGranted;
                bool qualifiesForDiscountExpected = await myOracle.QualifiesForDiscountAsync();
                #endregion

                #region Compare Expected VS Actual and Log Result Differences
                if (!qualifiesForDiscountExpected.Equals(qualifiesForDiscountActual))
                {
                    testCases[i].ResultMessage = $"Expected <{qualifiesForDiscountExpected}> but was <{qualifiesForDiscountActual}>";
                    hasFailures = true;
                }
                #endregion
            }

            var failedTests = testCases.Where(x => x.ResultMessage != null);
            string jsonTestCases = JsonConvert.SerializeObject(failedTests);
            #endregion

            Assert.True(!hasFailures, hasFailures ? jsonTestCases : "");
        }

        [Fact]
        public async Task SuperStudentDiscountAmountAsync()
        {
            #region Define Inputs
            InputDefinition gpa = new InputDefinition("DriverGPA", 3.50, new object[] { 3.49, 3.50, 3.51, 3.79, 3.80, 3.81, 4.0 });
            List<InputDefinition> inputs = new List<InputDefinition> { gpa };
            #endregion

            #region Setup Combination Generator and Generate Test Cases
            //Pass inputs to combination generator and select technique
            TestComboGen testComboGen = new TestComboGen(inputs);
            var testCases = testComboGen.GenerateNfatTestCases<SuperStudentDiscountApiTestCase>(1).ToList();
            #endregion

            bool hasFailures = false;

            #region Execute Test Cases and Log Results
            for(int i = 0; i < testCases.Count; i++)
            {
                #region Get Expected Results From Oracle and Get Actual Results From SUT
                SuperStudentDiscountOracle myOracle = new SuperStudentDiscountOracle(testCases[i]);
                var responseMessage = await TestCaseToHttpConverter.ConvertTestCaseToHttpPOST<SuperStudentDiscountApiTestCase>("http://54.210.38.124/service/superstudentdiscount", testCases[i]);
                SuperStudentDiscountResult discountResult = JsonConvert.DeserializeObject<SuperStudentDiscountResult>(await responseMessage.Content.ReadAsStringAsync());
                double discountAmountActual = discountResult.DiscountAmount;
                double discountAmountExpected = await myOracle.DiscountAmountAsync();
                #endregion

                #region Compare Expected VS Actual and Log Result Differences
                if (!discountAmountExpected.Equals(discountAmountActual))
                {
                    testCases[i].ResultMessage = $"Expected <{discountAmountExpected}> but was <{discountAmountActual}>";
                    hasFailures = true;
                }
                #endregion
            }

            var failedTests = testCases.Where(x => x.ResultMessage != null);
            string jsonTestCases = JsonConvert.SerializeObject(failedTests);
            #endregion

            Assert.True(!hasFailures, hasFailures ? jsonTestCases : "");
}
    }

    public class SuperStudentDiscountResult
    {
        public bool DiscountGranted { get; set; }

        public double DiscountAmount { get; set; }
    }

    public class SuperStudentDiscountApiTestCase : ITestCase
    {
        public string State { get; set; } = "OH";
        public int DriverAge { get; set; } = 29;
        public string Relationship { get; set; } = "Child";
        public string StudentStatus { get; set; } = "college-enrolled";
        public List<string> Violations { get; set; } = new List<string> { };
        public double DriverGPA { get; set; } = 3.5;
        public string MaritalStatus { get; set; } = "Single";
        public string ResultMessage { get; set; }

        public SuperStudentDiscountApiTestCase() { }

        #region Interface overrides
        public void SetPropertyOnTestCase(string propertyName, object value)
        {
            PropertyInfo[] properties = typeof(SuperStudentDiscountApiTestCase).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == propertyName)
                {
                    property.SetValue(this, value);
                    return;
                }
            }
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public T ConstrainTestCase<T>(T testCase) where T : ITestCase, new()
        {
            return testCase;
        }
        #endregion

        #region Equality overloads for distinct test cases
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return IsEqual((SuperStudentDiscountApiTestCase)obj);
        }

        public bool Equal(SuperStudentDiscountApiTestCase testCase)
        {
            if (object.ReferenceEquals(null, testCase))
            {
                return false;
            }

            if (object.ReferenceEquals(this, testCase))
            {
                return true;
            }

            return IsEqual(testCase);
        }

        private bool IsEqual(SuperStudentDiscountApiTestCase testCase)
        {
            return testCase.DriverAge.Equals(this.DriverAge) &&
                testCase.DriverGPA.Equals(this.DriverGPA) &&
                testCase.MaritalStatus.Equals(this.MaritalStatus) &&
                testCase.Relationship.Equals(this.Relationship) &&
                testCase.StudentStatus.Equals(this.StudentStatus) &&
                testCase.Violations.Equals(this.Violations);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, DriverAge) ? DriverAge.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Relationship) ? Relationship.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, StudentStatus) ? StudentStatus.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Violations) ? Violations.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, DriverGPA) ? DriverGPA.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, MaritalStatus) ? MaritalStatus.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion
    }
}
