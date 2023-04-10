using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Models;

namespace WPFGPT.Models;

public class ChatGpt
{
    public OpenAIClient? Api { get; set; }
    public int MaxToken { get; set; }
    private readonly Model[] _models = { Model.GPT3_5_Turbo, Model.GPT4, Model.GPT4_32K  };
    private List<ChatPrompt> ChatPrompts { get; set; }
    public ChatGpt(string? system)
    {
        system = $"Please use the markdown grammar to reply me.{system}";
        ChatPrompts = new List<ChatPrompt>
        {
            new("system", system)
        };
    }
    public async Task Chat(ObservableCollection<ChatMessage> chatObservableCollection,string? prompt, int modelType)
    {
        
        chatObservableCollection.Add(new ChatMessage{IsSend = false, Message = "Is thinking..." });
        
        ChatPrompts.Add(new("user", prompt));
        var chatRequest = new ChatRequest(ChatPrompts, maxTokens: this.MaxToken, model: this._models[modelType]);
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
        ChatPrompts.Add(new("assistant", response));
        
    }

    public List<ChatPrompt> GetPrompts()
    {
        return ChatPrompts;
    }
    public async Task<string> Whisper(string language)
    {
        var audioPath = "Audio.wav";
        var request = new AudioTranscriptionRequest(Path.GetFullPath(audioPath), language: language);
        var result = await Api!.AudioEndpoint.CreateTranscriptionAsync(request);
        return result;
    }
}