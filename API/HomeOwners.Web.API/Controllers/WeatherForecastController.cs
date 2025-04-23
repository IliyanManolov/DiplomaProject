using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Base;
using HomeOwners.Web.API.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace HomeOwners.Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    _logger.LogInformation("B");
        //    var test = "BLAH";

        //    _logger.LogWarning("I wonder what this value is - {val}", test);
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //return NotFound(new NotFoundResponseModel(HttpContext.TraceIdentifier));
                throw new IdentifierInUseValidationError("MyProp");

                return Ok();
            }
            catch (BaseValidationError err)
            {
                var badModel = new BadRequestResponseModel(HttpContext.TraceIdentifier);
                badModel.AddError(err);
                return BadRequest(badModel);
            }
        }
    }
}
