/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorFaixaBin, e expõe métodos para traduzir os dados de faixa bon, oriundos do webservice
    /// </summary>
    public class TranslatorFaixaBin
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de faixa bon oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static FaixaBin TranslateFaixaBinWSFaixaBinBusiness(issuerRangeBin from)
        {
            FaixaBin to = new FaixaBin();

            to.ValorInicial = from.initialBinRange;
            to.ValorFinal = from.finalBinRange;

            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de faixa bon oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static issuerRangeBin TranslateFaixaBinBusinessFaixaBinWS(FaixaBin from)
        {
            issuerRangeBin to = new issuerRangeBin();

            to.initialBinRange = from.ValorInicial;
            to.finalBinRange = from.ValorFinal;

            return to;
        }
    }
}
