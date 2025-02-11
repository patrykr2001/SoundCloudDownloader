using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Enums
{
    public static class EnumExtension
    {
        public static TEnum[] GetValuesArray<TEnum>(this TEnum enumType) where TEnum : Enum
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }
    }
}
