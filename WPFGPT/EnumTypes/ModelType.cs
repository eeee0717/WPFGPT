using System.ComponentModel;
using WPFGPT.Tools;

namespace WPFGPT.EnumTypes;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]

public enum ModelType
{
    [Description("gpt-3.5-turbo")]
    Type = 0,
}