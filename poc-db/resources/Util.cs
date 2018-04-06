/*
© Copyright 2017 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redecard.PN.Sustentacao.SharePoint.Helpers
{
    public static class Util
    {
        public static String FormatExceptionMessage(Exception ex)
        {
            var sbError = new StringBuilder();
            String exceptionMessage = null;

            sbError.Append(Environment.NewLine);
            sbError.AppendLine("Error Message:");
            sbError.AppendLine(ex.Message);

            if (ex.InnerException != null)
            {
                sbError.Append(Environment.NewLine);
                sbError.AppendLine("InnerException:");
                sbError.Append(ex.InnerException.Message);

                sbError.Append(Environment.NewLine);
                sbError.AppendLine("InnerException - Source: ");
                sbError.AppendLine(ex.InnerException.Source);

                sbError.Append(Environment.NewLine);
                sbError.AppendLine("InnerException: - Stack:");
                sbError.AppendLine(ex.InnerException.StackTrace);
            }

            sbError.Append(Environment.NewLine);
            sbError.AppendLine("Source: ");
            sbError.AppendLine(ex.Source);

            sbError.Append(Environment.NewLine);
            sbError.AppendLine("Stack:");
            sbError.AppendLine(ex.StackTrace);

            exceptionMessage = sbError.ToString();

            return exceptionMessage;
        }

    }
}
