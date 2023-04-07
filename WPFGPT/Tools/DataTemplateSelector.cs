using System.Windows;
using System.Windows.Controls;
using WPFGPT.Models;

namespace WPFGPT.Tools;

public class DateTemplateSelector:DataTemplateSelector
{
    
    public DataTemplate? SendTemplate { get; set; }
    public DataTemplate? ReceiveTemplate { get; set; }
    
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        ChatMessage message = (ChatMessage)item;


        if (message.IsSend)
        {
            return SendTemplate;
        }
        else
        {
            return ReceiveTemplate;
        }
    }
}