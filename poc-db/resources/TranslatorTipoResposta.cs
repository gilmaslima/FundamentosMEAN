/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – Renao Cara – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorTipoResposta, e expõe métodos para traduzir os dados de tipo de resposta, oriundos do webservice
    /// </summary>
    public class TranslatorTipoResposta
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de tipo de resposta oriundos do webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static TipoResposta TranslateIssuerResponseTypeToTipoRespostaBusiness(issuerResponseType from)
        {
            TipoResposta to = new TipoResposta();
            to.CodigoResposta = from.responseTypeId;
            to.DescricaoResposta = from.responseTypeDescripion;
            to.NomeResposta = from.name;
            to.SituacaoFraude = (SituacaoFraude) from.fraudSituation;
            return to;
        }

        /// <summary>
        /// Este método é utilizado para traduzir os dados de tipo de resposta para o webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static issuerResponseType TranslateTipoRespostaBusinessToIssuerResponseType(TipoResposta from)
        {
            issuerResponseType to = new issuerResponseType();
            to.responseTypeId = from.CodigoResposta;
            to.responseTypeDescripion = from.DescricaoResposta;
            to.name = from.NomeResposta;
            to.fraudSituation = (int) from.SituacaoFraude;
            return to;
        }
    }
}
