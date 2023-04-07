using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace WPFGPT.Models;

public class ChatMessage
{
    public bool IsSend { get; set; }
    public string? Message { get; set; }


}