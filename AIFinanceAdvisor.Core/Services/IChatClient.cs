using AIFinanceAdvisor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Services
{
    public  interface IChatClient
    {
        IAsyncEnumerable<ChatUpdate> CompleteChatStreamingAsync(string userMessage);
    }
}
