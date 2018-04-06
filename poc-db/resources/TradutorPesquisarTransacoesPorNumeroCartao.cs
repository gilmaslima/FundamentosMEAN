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
using Redecard.PN.FMS.Servico.Modelo.Transacoes;
using Redecard.PN.FMS.Comum;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorPesquisarTransacoesPorNumeroCartao, que expõe métodos para manipular as pesquisas de transações por numero cartão.
    /// </summary>
    public class TradutorPesquisarTransacoesPorNumeroCartao
    {
        /// <summary>
        /// Este método é utilizado para traduzir o modelo para a resposta do servicço de lista de transações por emissor.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Redecard.PN.FMS.Servico.Modelo.Transacoes.PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno TraduzirModeloToServicoRespostaListaTransacoesEmissor(Redecard.PN.FMS.Modelo.RespostaListaTransacoesEmissor from)
        {
            PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno to = new PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno();
            //to = new PesquisarTransacoesPorNumeroCartaoRetorno();

            to.ListaTransacoesEmissor = new List<TransacaoEmissor>();
            if (from.ListaTransacoesEmissor != null)
            {
                foreach (Redecard.PN.FMS.Modelo.TransacaoEmissor tran in from.ListaTransacoesEmissor)
                {
                    to.ListaTransacoesEmissor.Add(Tradutor.TradutorTransacaoEmissor.TraduzirTransacaoEmissor(tran));
                }
            }

            to.SegundosRestanteBloqueio = from.SegundosRestanteBloqueio;
            to.TipoRespostaListaEmissor = EnumHelper.EnumToEnum<Redecard.PN.FMS.Modelo.TipoRespostaListaEmissor, TipoRespostaListaEmissor>(from.TipoRespostaListaEmissor);

            return to;
        }

     
    }
}