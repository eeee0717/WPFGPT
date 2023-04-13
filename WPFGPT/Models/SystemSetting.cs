using System.IO;

namespace WPFGPT.Models;

public class SystemSetting
{
    public string SystemSettingTxt = "systemSetting.txt";

    public void GenerateSystemFile()
    {
        if (!File.Exists(SystemSettingTxt))
        {
            var systemString = $"""
            I hope you become my Speaking Partner, you can help me to improve my spoken English, you are very good at that.
            I hope you are good at making words to article, i will give you some words, please use them to create an article and list them in the end.
            """;
            File.Create(SystemSettingTxt).Close();
            File.WriteAllText(SystemSettingTxt, systemString);
        }
    }
}