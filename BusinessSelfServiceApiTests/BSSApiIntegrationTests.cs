using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusinessSelfServiceApiTests
{
    public class BSSApiIntegrationTests
    {
        static readonly HttpClient _httpClient = new HttpClient();

        public BSSApiIntegrationTests()
        {
            _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task HttpGetReturnsResponse200Async()
        {
            var httpResponse = await _httpClient.GetAsync(@"http://54.210.38.124/service/api/superstudentparmsdiscount/OH");
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task HttpGetReturnsCollectionAsync()
        {
            var httpResponse = await _httpClient.GetAsync(@"http://54.210.38.124/service/api/superstudentparmsdiscount");
            var businessData = JsonConvert.DeserializeObject<BusinessData>(await httpResponse.Content.ReadAsStringAsync());
            Assert.NotNull(businessData.Discounts);
        }

        private class BusinessData
        {
            [JsonPropertyName("discounts")]
            public List<SuperStudentParmsDTO> Discounts { get; set; }
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
