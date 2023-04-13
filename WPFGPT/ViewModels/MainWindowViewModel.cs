using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using WPFGPT.EnumTypes;
using WPFGPT.Models;

namespace WPFGPT.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<ChatMessage> ChatObservableCollection { set; get; } = new();
    private readonly ChatGpt _chatGpt;
    private readonly Config _config;
    private readonly Audio _audio;
    private readonly SystemSetting _systemSetting;
    private string? _keyWords;
    private bool _isRecording;
    private string[] _systemArray;
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _messageInput;
    [ObservableProperty] private string? _apiKey;
    [ObservableProperty] private int _modelType;
    [ObservableProperty] private int _maxToken = 100;
    [ObservableProperty] private string? _system;
    [ObservableProperty] private int _systemType;
    [ObservableProperty] private bool? _isEnabled;
    [ObservableProperty] private string? _recordImg = "Icons/Record.png";
    [ObservableProperty] private int _languageType;
    [ObservableProperty] private string _soundContent = "Close Sound";
    [ObservableProperty] private bool _soundState = true;

    public MainWindowViewModel()
    {
        this._chatGpt = new ChatGpt();
        this._config = new Config();
        this._audio = new Audio();
        this._systemSetting = new SystemSetting();
        this.Initialize();
        
    }

    private void Initialize()
    {
        this._config.GetConfig();
        this.SetSettings();
        this._systemSetting.GenerateSystemFile();
        this.GetSystemSettings();
        if (this.ApiKey == "")
        {
            MessageBox.Show("Please Set The Api Key!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void SetSettings()
    {
        this.ApiKey = this._config.Api;
    }

    private void GetSystemSettings()
    {
        string txtString;
        using (StreamReader reader = new StreamReader(this._systemSetting.SystemSettingTxt))
        {
            txtString = reader.ReadToEnd();
        }

        if (!string.IsNullOrEmpty(txtString))
        {
            this._systemArray = txtString.Split('\n');
            this.System = this._systemArray[0];
        }
    }

    private void CheckApi()
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
        }
    }

    [RelayCommand(CanExecute = nameof(CanClick))]
    private async Task Click()
    {
        this._keyWords += $" {this.MessageInput}";
        this.CheckApi();
        this._chatGpt.System = this.System;
        this._chatGpt.MaxToken = this.MaxToken;
        var chatMessage = new ChatMessage { IsSend = true, Message = MessageInput };
        this.ChatObservableCollection.Add(chatMessage);
        this.MessageInput = "";
        this.IsEnabled = false;
        await this._chatGpt.Chat(this.ChatObservableCollection, chatMessage.Message,  this.ModelType, this.SoundState);
        this.IsEnabled = true;
    }

    bool CanClick() => !string.IsNullOrEmpty(MessageInput);

    [RelayCommand]
    private void ClearClick()
    {
        this.ChatObservableCollection.Clear();
    }

    [RelayCommand]
    private void SaveReadingChatsClick()
    {
        var pdf = new Pdf();
        pdf.GeneratePdf(this._keyWords!, this._chatGpt.GetPrompts());
    }
    

    [RelayCommand]
    private void SaveClick()
    {
        var configJson = "config.json";
        var config = new Config
        {
            Api = this.ApiKey,
        };
        var configString = JsonConvert.SerializeObject(config);
        File.Create(configJson).Close();
        File.WriteAllText(configJson, configString);
    }
    
    
    [RelayCommand]
    private async Task RecordClick()
    {
        this.CheckApi();
        var language =Enum.Parse(typeof(LanguageType), this.LanguageType.ToString());
        if (_isRecording == false)
        {
            _isRecording = true;
            this.RecordImg = "Icons/Stop.png";
            this._audio.StartRecording();
        }
        else
        {
            _isRecording = false;
            this.RecordImg = "Icons/Record.png";
            this._audio.StopRecording();
            this.MessageInput = await this._chatGpt.Whisper(language.ToString()!);
        }
    }

    [RelayCommand]
    private void SoundClick()
    {
        this.SoundContent = this.SoundState ? "Open Sound" : "Close Sound";
        this.SoundState = !this.SoundState;

    }

    [RelayCommand]
    private void SystemChanged()
    {
        var type = this.SystemType;
        if (this._systemArray.Length < type + 1)
        {
            MessageBox.Show("Please Add New System!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.SystemType = 0;
            return;
        }
        this.System = this._systemArray[type];
    }
    

}