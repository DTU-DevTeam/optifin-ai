using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using AIFinanceAdvisor.Core.Entities;
using OpenAI.Chat;
using OpenAI.Assistants;
using AIFinanceAdvisor.Infrastructure.DatabaseContext;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class HomeController : Controller
{
    private readonly ChatClient _chatClient;
    private readonly string _apiKey;
    private readonly string _systemInstructions;

    // Lưu lịch sử hội thoại theo từng connectionId
    private static readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();
    private List<ChatMessageHistoryForUser> _ChatMessageHistoryForUserList;
    private List<ChatMessageHistoryForAssitant> _ChatMessageHistoryForAssitantList;
    private List<ChatHistoryForUser> _ChatHistoryForUserList;

    private readonly ApplicationDbContext _chatMessageHistoryRepository;

    private string fullMessage = ""; // Biến toàn cục để lưu trữ tin nhắn đầy đủ
    private Guid idMessage;
    private static string connectionId;
    private string userMessage;

    public HomeController(IConfiguration configuration, ApplicationDbContext chatMessageHistoryRepository)
    {
        var openAIOptions = configuration.GetSection("OpenAI");
        _apiKey = openAIOptions["ApiKey"];
        _systemInstructions = openAIOptions["SystemInstructions"];
        _chatClient = new(model: "gpt-4o-mini", apiKey: _apiKey);

        _ChatMessageHistoryForUserList = new List<ChatMessageHistoryForUser>();
        _ChatMessageHistoryForAssitantList = new List<ChatMessageHistoryForAssitant>();
        _ChatHistoryForUserList = new List<ChatHistoryForUser>();

        _chatMessageHistoryRepository = chatMessageHistoryRepository;

        connectionId = Guid.NewGuid().ToString();
    }

    [Route("newchat")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
           List<ChatHistoryForUser> chatHistoryForUsers = await _chatMessageHistoryRepository.ChatHistoryForUsers.ToListAsync();

            var groupedChatHistories = chatHistoryForUsers
            .GroupBy(c => c.IdConversation)
            .Select(group => new
            {
              ConversationId = group.Key,
              Messages = group.ToList()
            })
            .ToList();

            var chatHistory = groupedChatHistories.Select(g => g.Messages.First()).ToList();


            ViewBag.ChatBoxHistory = chatHistory;
        }
       

        return View();
    }

    [Route("historyMessage/{id}")]
    [HttpGet]
    public async Task<IActionResult> HistoryChat(string id)
    {
        if (User.Identity.IsAuthenticated)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            // Lấy lịch sử hội thoại cho người dùng
            var chatMessageHistoryForUser = await _chatMessageHistoryRepository.ChatMessageHistoryForUsers
                .Where(c => c.IdConverstation.ToString() == id)
                .ToListAsync();

            // Lấy lịch sử hội thoại cho trợ lý
            var chatMessageHistoryForAssitant = await _chatMessageHistoryRepository.ChatMessageHistoryForAssistants
                .Where(c => c.IdConverstation.ToString() == id)
                .ToListAsync();


            List<ChatHistoryForUser> chatHistoryForUsers = await _chatMessageHistoryRepository.ChatHistoryForUsers.ToListAsync();

            var groupedChatHistories = chatHistoryForUsers
            .GroupBy(c => c.IdConversation)
            .Select(group => new
            {
                ConversationId = group.Key,
                Messages = group.ToList()
            })
            .ToList();

            var chatHistory = groupedChatHistories.Select(g => g.Messages.First()).ToList();


            ViewBag.ChatBoxHistory = chatHistory;




            ViewBag.ChatMessageHistoryForUser = chatMessageHistoryForUser;
            ViewBag.ChatMessageHistoryForAssitant = chatMessageHistoryForAssitant;

            //var mergedMessages = chatMessageHistoryForUser.Concat(chatMessageHistoryForAssitant).OrderBy(m => m.IdMessager).ToList();
        }


        return View();
    }



    [Route("ws/{id?}")]
    public async Task StreamChat(string? id)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            Response.StatusCode = 400;
            return;
        }

        if(!string.IsNullOrEmpty(id))
        {
            connectionId = id;
        }
        

        // Tạo kết nối WebSocket và lưu trữ lịch sử hội thoại
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024];
        using var memoryStream = new MemoryStream();

        // Tạo lịch sử hội thoại mới cho kết nối này
        var chatHistory = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(_systemInstructions)
        };
        _conversations[connectionId] = chatHistory;

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

                if (result.EndOfMessage)
                {
                    userMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                    memoryStream.SetLength(0); // Reset bộ nhớ đệm

                    if (userMessage == "ping")
                    {
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes("pong"), WebSocketMessageType.Text, true, CancellationToken.None);
                        continue;
                    }

                    idMessage = Guid.NewGuid(); // Tạo ID cho tin nhắn người dùng

                    var jsonObject = JsonSerializer.Deserialize<JsonElement>(userMessage);

                    // Lấy giá trị của "content"
                    userMessage = jsonObject.GetProperty("content").GetString();

                    // Lưu tin nhắn người dùng vào lịch sử
                    _ChatMessageHistoryForUserList.Add(new ChatMessageHistoryForUser
                    {
                        IdMessage = idMessage,
                        IdConverstation = Guid.Parse(connectionId),
                        ContentMessage = userMessage
                    });

                    // Thêm tin nhắn người dùng vào lịch sử hội thoại
                    chatHistory.Add(ChatMessage.CreateUserMessage(userMessage));

                    // Gọi API với toàn bộ lịch sử hội thoại
                    await foreach (var update in _chatClient.CompleteChatStreamingAsync(chatHistory))
                    {
                        string reply = update.ContentUpdate[0].Text;

                        // Gửi phản hồi về client
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(reply), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Lưu phản hồi của trợ lý vào lịch sử
                        chatHistory.Add(ChatMessage.CreateAssistantMessage(reply));

                        fullMessage += reply;
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
            // Lưu tin nhắn lịch sử cho người dùng và trợ lý
            _ChatMessageHistoryForAssitantList.Add(new ChatMessageHistoryForAssitant
            {
                IdMessage = idMessage,
                IdConverstation = Guid.Parse(connectionId),
                ContentMessage = fullMessage
            });

            


            if(User.Identity.IsAuthenticated)
            {

                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                _ChatHistoryForUserList.Add( new ChatHistoryForUser 
                {
                    UserId = userId,
                    IdConversation = Guid.Parse(connectionId),
                    TopicMessage = userMessage,
                });

               
               await _chatMessageHistoryRepository.ChatMessageHistoryForUsers.AddRangeAsync(_ChatMessageHistoryForUserList);
               _chatMessageHistoryRepository.SaveChanges();

                await _chatMessageHistoryRepository.ChatMessageHistoryForAssistants.AddRangeAsync(_ChatMessageHistoryForAssitantList);
                _chatMessageHistoryRepository.SaveChanges();

                await _chatMessageHistoryRepository.ChatHistoryForUsers.AddRangeAsync(_ChatHistoryForUserList);
                 _chatMessageHistoryRepository.SaveChanges();

            }

               
            
            // Lưu vào database
            

            // Xóa lịch sử hội thoại cho kết nối này
            _conversations.TryRemove(connectionId, out _);

            if (webSocket.State != WebSocketState.Closed)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Lỗi trong quá trình xử lý", CancellationToken.None);
            }
        }
    }
}
