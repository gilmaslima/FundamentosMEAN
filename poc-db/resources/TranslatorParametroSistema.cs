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
    /// Este componente publica a classe TranslatorParametroSistema, e expõe métodos para traduzir os parâmetros de sistema, oriundos do webservice
    /// </summary>
    public class TranslatorParametroSistema
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de parãmetros de sistema, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static ParametrosSistema TranslateParametrosSistemaWSParametrosSistemaBusiness(systemParameterComposite from)
        {
            ParametrosSistema to = new ParametrosSistema();

            to.PossuiAcesso = from.hasAccess;
            to.QuantidadeMaximaIntervaloDiasPesquisas = from.intervalDaysMaxAmount;
            to.QuantidadeMaximaDiasRetroativosPesquisas = from.retroactiveDaysMaxAmount;
            to.TempoExpiracaoBloqueio = from.timeoutLock;

            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de composição de parãmetros de sistema, oriundos do webservice 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static systemParameterComposite TranslateParametrosSistemaBusinessParametrosSistemaWS(ParametrosSistema from)
        {
            systemParameterComposite to = new systemParameterComposite();

            to.hasAccess = from.PossuiAcesso;
            to.intervalDaysMaxAmount = from.QuantidadeMaximaIntervaloDiasPesquisas;
            to.retroactiveDaysMaxAmount = from.QuantidadeMaximaDiasRetroativosPesquisas;
            to.timeoutLock = from.TempoExpiracaoBloqueio;

            return to;
        }
    }
}
