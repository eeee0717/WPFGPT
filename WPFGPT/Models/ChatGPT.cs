using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace WPFGPT.Models;

public class ChatGpt
{
    // private readonly OpenAIClient _api = new(new OpenAIAuthentication("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"));
    public OpenAIClient? Api { get; set; }
    private readonly string _systemSetting = "you are a helpful homeassistant";
    public async Task Chat(ObservableCollection<ChatMessage> chatObservableCollection,string? prompt)
    {
        chatObservableCollection.Add(new ChatMessage{IsSend = false, Message = "Is thinking..." });
        var chatPrompts = new List<ChatPrompt>
        {
            new("system", this._systemSetting),
            new("user", prompt),
        };
        var chatRequest = new ChatRequest(chatPrompts, maxTokens: 3800, model: "gpt-3.5-turbo");
        var response = "";
        await Api!.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
        {
            response += result.FirstChoice;
            ChatMessage message = new ChatMessage { IsSend = false, Message = response };
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                chatObservableCollection.Remove(chatObservableCollection.Last());
                chatObservableCollection.Add(message);
            }));
        });
    }
}