using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Comum.Helper
{
    public static class TesteHelper
    {
        /// <summary>
        /// Verificar se o sistema está sendo executado em ambiente de desenvolvimento.
        /// </summary>
        /// <returns></returns>
        public static bool IsAmbienteDesenvolvimento()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
