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
    /// Este componente publica a classe TranslatorMCC,  utilizados para traduzir os dados de Merchant Categoy Code oriundos do webservice 
    /// </summary>
    public class TranslatorMCC
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de Merchant Categoy Code oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static MCC TranslateMCCWSMCCBusiness(issuerMCC from)
        {
            MCC to = new MCC();
            to.CodigoMCC = from.code.ToString();
            to.DescricaoMCC = from.description;

            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de merchant categoy code oriundos do webservice para merchant categoy code issue
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static issuerMCC TranslateMCCBusinessMCCWS(MCC from)
        {
            issuerMCC to = new issuerMCC();
            to.code = long.Parse(from.CodigoMCC);
            to.codeSpecified = to.code != 0;
            to.description = from.DescricaoMCC;

            return to;
        }

    }
}
