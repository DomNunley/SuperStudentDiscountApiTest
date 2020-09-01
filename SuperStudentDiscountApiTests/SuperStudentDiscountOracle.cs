using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperStudentDiscountApiTests
{
    public class SuperStudentDiscountOracle
    {
        private static HttpClient _httpClient = new HttpClient();
        private readonly SuperStudentDiscountApiTestCase _testCase;

        public SuperStudentDiscountOracle(SuperStudentDiscountApiTestCase testCase)
        {
            _testCase = testCase;
            _httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> QualifiesForDiscountAsync()
        {
            var bizData = await GetBizDataFromDB();

            if(_testCase.DriverAge < bizData.DriverAge &&
                _testCase.DriverGPA >= bizData.DriverEligibleGPA && 
                (_testCase.MaritalStatus == "Single" || _testCase.MaritalStatus == "Divorced") &&
                _testCase.Relationship == "Child" &&
                _testCase.StudentStatus.ToLower().Contains("enrolled") &&
                _testCase.Violations.Count == 0)
            {
                return true;
            }

            return false;
        }

        public async Task<double> DiscountAmountAsync()
        {
            var bizData = await GetBizDataFromDB();

            if (await QualifiesForDiscountAsync() && _testCase.DriverGPA >= bizData.DriverHighGPA)
            {
                return bizData.DiscountHighAmount;
            }
            else if (await QualifiesForDiscountAsync() && _testCase.DriverGPA >= bizData.DriverMediumGPA)
            {
                return bizData.DiscountMediumAmount;
            }
            else
            {
                return 0;
            }
        }

        private async Task<SuperStudentParmsDTO> GetBizDataFromDB()
        {
            var httpResponse = await _httpClient.GetAsync(@"http://54.208.9.185/service/api/superstudentparmsdiscount/" + _testCase.State);
            return JsonConvert.DeserializeObject<SuperStudentParmsDTO>(await httpResponse.Content.ReadAsStringAsync());
        }

        private class SuperStudentParmsDTO
        {
            [JsonPropertyName("state")]
            public string State { get; set; }
            [JsonPropertyName("driverage")]
            public int DriverAge { get; set; }
            [JsonPropertyName("drivereligiblegpa")]
            public double DriverEligibleGPA { get; set; }
            [JsonPropertyName("drivermediumgpa")]
            public double DriverMediumGPA { get; set; }
            [JsonPropertyName("driverhighgpa")]
            public double DriverHighGPA { get; set; }
            [JsonPropertyName("discountmediumamount")]
            public double DiscountMediumAmount { get; set; }
            [JsonPropertyName("discounthighamount")]
            public double DiscountHighAmount { get; set; }
        }
    }
}
