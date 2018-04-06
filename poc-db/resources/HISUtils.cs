using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Cancelamento.Agentes
{
    public class HISUtils
    {
        public static DateTime ConvertToDate(string dateString)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                DateTime result;

                DateTime.TryParse(dateString.Replace('.', '/'), out result);

                return result;
            }

            return DateTime.Now;
        }

        public static Decimal ConvertToDecimal(string decimalString)
        {
            if (!string.IsNullOrEmpty(decimalString))
            {
                decimal result;

                decimal.TryParse(decimalString, System.Globalization.NumberStyles.Any, null, out result);

                return result;
            }

            return 0;
        }

        public static Int32 ConvertToInt32(string int32String)
        {
            if (!String.IsNullOrEmpty(int32String))
            {
                Int32 result;

                Int32.TryParse(int32String, out result);

                return result;
            }

            return 0;
        }

        public static int ConvertToInt(string intString)
        {
            if (!String.IsNullOrEmpty(intString))
            {
                int result;

                int.TryParse(intString, out result);

                return result;
            }

            return 0;
        }
    }
}
