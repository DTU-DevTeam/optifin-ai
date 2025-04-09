using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFinanceAdvisor.Core.Entities
{
    public class ChatMessageHistoryForUser
    {
        [Key]
        public Guid IdMessage { get; set; }
        public Guid IdConverstation { get; set; }
        
        public string ContentMessage { get; set; }
    }
}
