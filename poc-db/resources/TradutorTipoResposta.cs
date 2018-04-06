/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.FMS.Servico.Modelo;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorTipoResposta, que expõe métodos para manipular os tipos de resposta.
    /// </summary>
    public static class TradutorTipoResposta
    {
        /// <summary>
        /// Este método é utilizado para
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static Redecard.PN.FMS.Servico.Modelo.TipoResposta TraduzirTipoResposta(Redecard.PN.FMS.Modelo.TipoResposta de)
        {

            TipoResposta para = new TipoResposta()
            {
                CodigoResposta = de.CodigoResposta,
                DescricaoResposta = de.DescricaoResposta,
                NomeResposta = de.NomeResposta,
                SituacaoFraude = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.SituacaoFraude, Redecard.PN.FMS.Servico.Modelo.SituacaoFraude>(de.SituacaoFraude)
            };

            return para;

        }

        /// <summary>
        /// Este método é utilizado para traduzir o tipo de resposta.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static Redecard.PN.FMS.Modelo.TipoResposta TraduzirTipoResposta(Redecard.PN.FMS.Servico.Modelo.TipoResposta de)
        {

            Redecard.PN.FMS.Modelo.TipoResposta para = new Redecard.PN.FMS.Modelo.TipoResposta()
            {
                CodigoResposta = de.CodigoResposta,
                DescricaoResposta = de.DescricaoResposta,
                NomeResposta = de.NomeResposta,
                SituacaoFraude = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.SituacaoFraude, Redecard.PN.FMS.Modelo.SituacaoFraude>(de.SituacaoFraude)
            };

            return para;

        }
    }
}