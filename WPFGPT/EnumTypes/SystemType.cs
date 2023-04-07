using System.ComponentModel;
using WPFGPT.Tools;

namespace WPFGPT.EnumTypes;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]

public enum SystemType
{
    [Description("翻译官")]
    Type = 0,
    [Description("脱口秀演员")]
    Type1 = 1,
    [Description("IT专家")]
    Type2 = 2
}