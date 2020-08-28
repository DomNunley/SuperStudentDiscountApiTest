using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using SuperStudentDiscountApiTests;
using TestRunnerApi.Models;
using TestRunnerApi.Services;

namespace TestRunnerApi.Controllers
{
    [ApiController]
    public class TestRunnerController : ControllerBase
    {
        [HttpGet]
        [Route("superstudentdiscounttestrunner")]
        public async Task<TestRunResults<SuperStudentDiscountApiTestCase>> GetAsync()
        {
            return await TestExecutor.ExecuteTests<SuperStudentDiscountApiTestCase>(typeof(SuperStudentDiscountApiComboTests));
        }
    }
}
