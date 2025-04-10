using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace WorkShop.ServiceBus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private readonly ILogger<ServiceBusController> _logger;
        private readonly IConfiguration _configuration;

        public ServiceBusController(ILogger<ServiceBusController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

        }


        [HttpPost("Send")]
        public async Task<IActionResult> SendMessageAsync([FromBody] string message)
        {
            try
            {
                var connectionString = _configuration["ServiceBus:ConnectionString"];
                var queueName = _configuration["ServiceBus:QueueName"];

                await using var client = new ServiceBusClient(connectionString);

                ServiceBusSender sender = client.CreateSender(queueName);

                var messageServiceBus = new ServiceBusMessage(message);

                await sender.SendMessageAsync(messageServiceBus);

                _logger.LogInformation($"Mensage enviada: {message}");

                return Ok("Mensage enviada!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no envio!");
                throw;
            }
        }
    }
}
