using CommunityToolkit.Maui.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pluto.Helpers
{
    public class IsNot0Converter :  BaseConverterOneWay<int?, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; }

        public override bool ConvertFrom(int? value, CultureInfo? culture)
        {
            if (value is int)
            {
                if (value == 0)
                    return false;

                return true;
            }

            return false;
        }
    }
}
