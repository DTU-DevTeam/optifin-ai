using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Entities
{
    public class ChatHistoryForUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        public string TopicMessage { get; set; }
        public Guid IdConversation { get; set; }


    }
}
