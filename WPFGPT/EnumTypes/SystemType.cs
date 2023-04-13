using System.ComponentModel;
using WPFGPT.Tools;

namespace WPFGPT.EnumTypes;

[TypeConverter(typeof(EnumDescriptionTypeConverter))]

public enum SystemType
{
    [Description("Speaking Partner")]
    Type = 0,
    [Description("Article Assistant")]
    Type1 = 1,

}