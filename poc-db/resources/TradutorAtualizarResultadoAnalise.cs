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
using Redecard.PN.FMS.Servico.Modelo.Transacoes;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Modelo;


namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorAtualizarResultadoAnalise, que expõe métodos para atualizar o resultado da análise do tradutor.
    /// </summary>
    public class TradutorAtualizarResultadoAnalise
    {
        /// <summary>
        /// Este método é utilizado para traduzir a lista de resposta da análise.
        /// </summary>
        /// <param name="deList"></param>
        /// <returns></returns>
        public static List<RespostaAnalise> TraduzirListaRespostaAnalise(List<RespostaAnaliseItem> deList)
        {
            List<RespostaAnalise> para = new List<RespostaAnalise>();

            foreach (RespostaAnaliseItem de in deList)
            {
                para.Add(new RespostaAnalise()
                {
                    Comentario = de.Comentario,
                    EhFraude = de.EhFraude,
                    GrupoEntidade = de.GrupoEntidade,
                    IdentificadorTransacao = de.IdentificadorTransacao,
                    NumeroEmissor = int.Parse(de.NumeroEmissor),
                    TipoAlarme = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.TipoAlarme, Redecard.PN.FMS.Modelo.TipoAlarme>(de.TipoAlarme),
                    TipoResposta = new TipoResposta(){CodigoResposta = de.TipoResposta.CodigoResposta,
                        DescricaoResposta=de.TipoResposta.DescricaoResposta,
                        NomeResposta=de.TipoResposta.NomeResposta,
                        SituacaoFraude = EnumHelper.EnumToEnum<Redecard.PN.FMS.Servico.Modelo.SituacaoFraude, Redecard.PN.FMS.Modelo.SituacaoFraude>(de.TipoResposta.SituacaoFraude)
                    },
                    UsuarioLogin = de.UsuarioLogin
                });
            }
            return para;
        }
    }
}