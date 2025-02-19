using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using System.Text;
using System.Net.WebSockets;

namespace AIFinanceAdvisor.UI.Controllers
{
    //[ApiController]
   // [Route("ws")]
    public class HomeController : Controller
    {
        private readonly ChatClient _chatClient;
        private readonly string _apiKey;
        private readonly string _systemInstructions;

        public HomeController(IConfiguration configuration)
        {
            var openAIOptions = configuration.GetSection("OpenAI");
            _apiKey = openAIOptions["ApiKey"];
            _systemInstructions = openAIOptions["SystemInstructions"];
            _chatClient = new(model: "gpt-4o-mini", apiKey: _apiKey);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("ws")]
        public async Task StreamChat()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                Response.StatusCode = 400;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[1024]; // 1KB mỗi lần đọc
            using var memoryStream = new MemoryStream();

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    memoryStream.Write(buffer, 0, result.Count);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
                        break;
                    }

                    // Nếu nhận được toàn bộ tin nhắn
                    if (result.EndOfMessage)
                    {
                        string userMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                        memoryStream.SetLength(0); // Reset bộ nhớ đệm

                        if (userMessage == "ping")
                        {
                            await webSocket.SendAsync(Encoding.UTF8.GetBytes("pong"), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }

                       

                        // Xử lý phản hồi từ API AI (ChatGPT hoặc tương tự)
                        await foreach (var update in _chatClient.CompleteChatStreamingAsync(userMessage, _systemInstructions))
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(update.ContentUpdate[0].Text);
                            await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"⚠️ WebSocket lỗi: {ex.Message}");
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Lỗi trong quá trình xử lý", CancellationToken.None);
                }
            }
        }







    }
}
