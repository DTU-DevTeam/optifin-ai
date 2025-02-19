using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Entities
{
    public class ChatUpdate
    {
        public List<MessageContent> ContentUpdate { get; set; }
    }

    public class MessageContent
    {
        public string Text { get; set; }
    }
}
