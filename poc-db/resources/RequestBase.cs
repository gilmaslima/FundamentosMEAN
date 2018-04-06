using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.Request.Agentes
{
    public class RequestBase : AgentesBase
    {
        protected DateTime ParseDate(object value, string format)
        {
            DateTime date;

            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(value.ToString(), format, provider, System.Globalization.DateTimeStyles.None, out date))
                return date;
            else
                return default(DateTime);
        }

        protected DateTime? ParseDateNull(object value, string format)
        {
            try
            {
                DateTime date;

                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

                if (DateTime.TryParseExact(value.ToString(), format, provider, System.Globalization.DateTimeStyles.None, out date))
                    return date;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
