using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OpenAI;
using WPFGPT.Models;

namespace WPFGPT.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<ChatMessage> ChatObservableCollection { set; get; } = new();
    private ChatGpt _chatGpt;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _messageInput;

    [ObservableProperty] private object? _apiKey;
    private Config? _config;
    [ObservableProperty] private int _systemType;
    [ObservableProperty] private int _modelType;

    [ObservableProperty] private int _maxToken = 100;

    public MainWindowViewModel()
    {
        this.Initialize();
    }

    private void Initialize()
    {
        this.GetConfig();
    }

    private void GetConfig()
    {
        var configJson = "config.json";
        if (!File.Exists(configJson))
        {
            File.Create(configJson).Close();
        }

        string jsonString;
        using (StreamReader reader = new StreamReader(configJson))
        {
            jsonString = reader.ReadToEnd();
        }

        if (!string.IsNullOrEmpty(jsonString))
        {
            this._config = JsonConvert.DeserializeObject<Config>(jsonString);
            this.ApiKey = this._config!.Api;
            this._chatGpt = new ChatGpt(this.ApiKey!.ToString()!, this.MaxToken);
        }
        // else
        // {
        //     MessageBox.Show("Please Set The Api Key!", "Warning",MessageBoxButton.OK, MessageBoxImage.Warning);
        // }
    }

    [RelayCommand(CanExecute = nameof(CanClick))]
    private async Task Click()
    {
        this._chatGpt.MaxToken = this.MaxToken;
        var chatMessage = new ChatMessage { IsSend = true, Message = MessageInput };
        this.ChatObservableCollection.Add(chatMessage);
        this.MessageInput = "";

        await this._chatGpt.Chat(this.ChatObservableCollection, chatMessage.Message, this.SystemType, this.ModelType);
    }

    bool CanClick() => !string.IsNullOrEmpty(MessageInput);

    [RelayCommand]
    private void ClearClick()
    {
        this.ChatObservableCollection.Clear();
    }
}