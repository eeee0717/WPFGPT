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
    private readonly ChatGpt _chatGpt;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _messageInput;

    [ObservableProperty] private string? _apiKey;
    private readonly Config _config;
    [ObservableProperty] private int _systemType;
    [ObservableProperty] private int _modelType;

    [ObservableProperty] private int _maxToken = 100;

    [ObservableProperty] private string? _system;
    [ObservableProperty] private bool? _isEnabled;

    public MainWindowViewModel()
    {
        this._chatGpt = new ChatGpt();
        this._config = new Config();
        this.Initialize();
    }

    private void Initialize()
    {
        this._config.GetConfig();
        this.SetSettings();
        if (this.ApiKey == "")
        {
            MessageBox.Show("Please Set The Api Key!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void SetSettings()
    {
        this.ApiKey = this._config.Api;
        this.System = this._config.System;
    }

    [RelayCommand(CanExecute = nameof(CanClick))]
    private async Task Click()
    {
        if (this.ApiKey == "")
        {
            return;
        }

        if (this.ApiKey!.Contains("sk"))
        {
            this._chatGpt.Api = new OpenAIClient(new OpenAIAuthentication(this.ApiKey));
        }
        else
        {
            MessageBox.Show("Please Set The Correct Api Key!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        this._chatGpt.MaxToken = this.MaxToken;
        var chatMessage = new ChatMessage { IsSend = true, Message = MessageInput };
        this.ChatObservableCollection.Add(chatMessage);
        this.MessageInput = "";
        this.IsEnabled = false;
        await this._chatGpt.Chat(this.ChatObservableCollection, chatMessage.Message, this.System, this.ModelType);
        this.IsEnabled = true;
    }

    bool CanClick() => !string.IsNullOrEmpty(MessageInput);

    [RelayCommand]
    private void ClearClick()
    {
        this.ChatObservableCollection.Clear();
    }

    [RelayCommand]
    private void SaveClick()
    {
        var configJson = "config.json";
        var config = new Config
        {
            Api = this.ApiKey,
            System = this.System
        };
        var configString = JsonConvert.SerializeObject(config);
        File.Create(configJson).Close();
        File.WriteAllText(configJson, configString);
    }
}