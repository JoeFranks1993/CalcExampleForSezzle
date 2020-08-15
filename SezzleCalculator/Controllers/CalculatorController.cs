using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SezzleCalculator.Managers;
using SezzleCalculator.Models;

namespace SezzleCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : Controller
    {
        private readonly ILogger<CalculatorController> logger;
        private readonly ICalculationManager calcManager;

        public CalculatorController(ILogger<CalculatorController> logger, ICalculationManager calcManager)
        {
            this.logger = logger;
            this.calcManager = calcManager;
        }

        [HttpGet]
        public ActionResult GetCalculations()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(calcManager.GetCalulations(10)));
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex.Message);
                throw new Exception("Failed To Retrieve Calculations From The Underlying Repo");
            }
        }

        [HttpPost]
        public ActionResult Calculate([FromBody] string calculation)
        {
            try
            {
                var result = calcManager.AddCalculation(calculation);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex.Message);
                return BadRequest();
            }
        }
    }
}