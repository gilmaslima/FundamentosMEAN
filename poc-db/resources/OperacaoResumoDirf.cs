using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Negocio
{
    public class OperacaoResumoDirf
    {
        public static decimal ObterValorTotalConsolidadoAnoDirf(ConsultarExtratoDirfRetorno item){

            return item != null & item.Estabelecimentos.Any() ? item.Estabelecimentos.Sum(s => s.ValorCobrado) : 0.0M;
        }
    }
}

