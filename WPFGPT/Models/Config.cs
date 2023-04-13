using System.IO;
using Newtonsoft.Json;

namespace WPFGPT.Models;

public class Config
{ 
    public string? Api { get; set; }

    public void GetConfig()
    {
        var configJson = "config.json";
        if (!File.Exists(configJson))
        {
            var config = new Config
            {
                Api = "",
            };
            var configString = JsonConvert.SerializeObject(config);
            File.Create(configJson).Close();
            File.WriteAllText(configJson, configString);
            
        }

        string jsonString;
        using (StreamReader reader = new StreamReader(configJson))
        {
            jsonString = reader.ReadToEnd();
        }

        if (!string.IsNullOrEmpty(jsonString))
        {
            var config = JsonConvert.DeserializeObject<Config>(jsonString);
            this.Api = config!.Api;
        }
    }
}