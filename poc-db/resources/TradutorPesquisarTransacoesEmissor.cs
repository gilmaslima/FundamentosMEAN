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


namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorTransacoesEmissorRetornoComQuantidade, que expõe métodos para manipular o retorno das transações por emissor com quantidade.
    /// </summary>
    public static class TradutorTransacoesEmissorRetornoComQuantidade
    {
        /// <summary>
        /// Este método é utilizado para traduzir o modelo para o retorno do serviço de transações por emissor com quantidade.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        public static TransacoesEmissorRetornoComQuantidade TraduzirModeloToServicoTransacoesEmissorRetornoComQuantidade(Redecard.PN.FMS.Modelo.RespostaListaTransacoes de)
        {
            TransacoesEmissorRetornoComQuantidade para = new TransacoesEmissorRetornoComQuantidade();

            para.QuantidadeRegistros = de.QuantidadeRegistros;
            para.ListaTransacoes = new List<Modelo.TransacaoEmissor>();
            para.ListaTransacoes.AddRange(de.ListaTransacoes.ConvertAll<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor>(TradutorTransacaoEmissor.TraduzirTransacaoEmissor));

            return para;
        }

    }
}