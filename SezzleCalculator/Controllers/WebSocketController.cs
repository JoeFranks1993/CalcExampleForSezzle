using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SezzleCalculator.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SezzleCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : Controller
    {
        private readonly ILogger<CalculatorController> logger;
        private readonly ICalculationManager calcManager;

        public WebSocketController(ILogger<CalculatorController> logger, ICalculationManager calcManager)
        {
            this.logger = logger;
            this.calcManager = calcManager;
        }

        [HttpGet]
        public async Task Index()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await ListenForEvents(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task ListenForEvents(WebSocket webSocket)
        {
            var lastIndexSentToClient = 0; // represents that highest calculation index we have sent to the client

            while(webSocket.State == WebSocketState.Open)
            {
                var latestCalculations = calcManager.GetCalulations(10); // Get the 10 most recent calculations
                if (latestCalculations.Count != 0) // if the count is 0, the repo is empty, we don't need to do anything
                {
                    var highestIndexInRepo = latestCalculations.Max(x => x.Index);

                    if (lastIndexSentToClient < highestIndexInRepo) // If we have an entry in the repo with a higher index then what was last sent to the client, we need to send the data.
                    {
                        lastIndexSentToClient = highestIndexInRepo;
                        var calcAsJson = JsonConvert.SerializeObject(latestCalculations);
                        var bytes = Encoding.ASCII.GetBytes(calcAsJson);

                        var arraySegment = new ArraySegment<byte>(bytes);
                        await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                await Task.Delay(500); 
            }
        }
    }
}
