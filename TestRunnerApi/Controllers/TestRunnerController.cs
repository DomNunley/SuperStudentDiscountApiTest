using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperStudentDiscountApiTests;
using BusinessSelfServiceApiTests;
using TestRunnerApi.Services;

namespace TestRunnerApi.Controllers
{
    [ApiController]
    public class TestRunnerController : ControllerBase
    {
        [HttpGet]
        [Route("api/superstudentdiscounttestrunner")]
        public async Task<ActionResult> GetDiscountResultsAsync()
        {
            var testRunResults = await TestExecutor.ComboExecuteTests<SuperStudentDiscountApiTestCase>(typeof(SuperStudentDiscountApiComboTests));
            return Ok(testRunResults);
        }

        [HttpGet]
        [Route("api/bizintegrationtestrunner")]
        public async Task<ActionResult> GetIntegrationResultsAsync()
        {
            var testRunResults = await TestExecutor.ExecuteTests(typeof(BSSApiIntegrationTests));
            return Ok(testRunResults);
        }
    }
}
