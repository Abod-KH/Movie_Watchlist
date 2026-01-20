using Microsoft.AspNetCore.Mvc;


namespace apiTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static List<WeatherForecast> Forecasts = new List<WeatherForecast>();

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

            // Initialize some dummy data if empty
            if (!Forecasts.Any())
            {
                var rng = new Random();
                for (int i = 0; i < 5; i++)
                {
                    Forecasts.Add(new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(i),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    });
                }
            }
        }

        // GET: /WeatherForecast
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Forecasts;
        }

        // GET: /WeatherForecast/5
        [HttpGet("{id}")]
        public ActionResult<WeatherForecast> Get(int id)
        {
            if (id < 0 || id >= Forecasts.Count)
                return NotFound();

            return Forecasts[id];
        }

        // POST: /WeatherForecast
        [HttpPost]
        public ActionResult<WeatherForecast> Post([FromBody] WeatherForecast forecast)
        {
            Forecasts.Add(forecast);
            return CreatedAtAction(nameof(Get), new { id = Forecasts.Count - 1 }, forecast);
        }

        // PUT: /WeatherForecast/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WeatherForecast updatedForecast)
        {
            if (id < 0 || id >= Forecasts.Count)
                return NotFound();

            Forecasts[id] = updatedForecast;
            return NoContent();
        }

        // DELETE: /WeatherForecast/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0 || id >= Forecasts.Count)
                return NotFound();

            Forecasts.RemoveAt(id);
            return NoContent();
        }
    }

    // Model
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

