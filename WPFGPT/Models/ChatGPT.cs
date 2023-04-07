using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace WPFGPT.Models;

public class ChatGpt
{
    private OpenAIClient? Api { get; set; }
    public int MaxToken { get; set; }
    private readonly string[] _systemSetting = {"我希望你能担任英语翻译、拼写校对和修辞改进的角色。我会用任何语言和你交流，你会识别语言，将其翻译并用更为优美和精炼的英语回答我。请将我简单的词汇和句子替换成更为优美和高雅的表达方式，确保意思不变，但使其更具文学性。请仅回答更正和改进的部分，不要写解释。","我想让你扮演一个脱口秀喜剧演员。我将为您提供一些与时事相关的话题，您将运用您的智慧、创造力和观察能力，根据这些话题创建一个例程。您还应该确保将个人轶事或经历融入日常活动中，以使其对观众更具相关性和吸引力。","我希望你担任 IT 架构师。我将提供有关应用程序或其他数字产品功能的一些详细信息，而您的工作是想出将其集成到 IT 环境中的方法。这可能涉及分析业务需求、执行差距分析以及将新系统的功能映射到现有 IT 环境。接下来的步骤是创建解决方案设计、物理网络蓝图、系统集成接口定义和部署环境蓝图。"};
    private readonly Model[] _models = { Model.GPT3_5_Turbo, Model.GPT4, Model.GPT4_32K  };
    public ChatGpt(string api, int maxToken)
    {
        this.MaxToken = maxToken;
        this.Api = new OpenAIClient(new OpenAIAuthentication(api));
    }
    public async Task Chat(ObservableCollection<ChatMessage> chatObservableCollection,string? prompt, int systemType, int modelType)
    {
        chatObservableCollection.Add(new ChatMessage{IsSend = false, Message = "Is thinking..." });
        var chatPrompts = new List<ChatPrompt>
        {
            new("system", this._systemSetting[systemType]),
            new("user", prompt),
        };
        var chatRequest = new ChatRequest(chatPrompts, maxTokens: this.MaxToken, model: this._models[modelType]);
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