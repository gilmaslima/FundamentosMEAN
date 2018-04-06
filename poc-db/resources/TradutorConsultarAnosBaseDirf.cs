using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarAnosBaseDirf
    {
        public static ConsultarAnosBaseDirfRetorno TraduzirRetornoConsultarAnosBaseDirf(ConsultarAnosBaseDirfRetornoDTO de)
        {
            ConsultarAnosBaseDirfRetorno para = new ConsultarAnosBaseDirfRetorno();
            para.AnosBase = de.AnosBase;

            return para;
        }
    }
}