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
using Redecard.PN.FMS.Servico.Modelo.Transacoes;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorPesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin, que expõe métodos para manipular as pesquisas de transações suspeitas por emissor e usuário logado.
    /// </summary>
    public class TradutorPesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin
    {
        /// <summary>
        /// Este método é utilizado para traduzir o modelo para o serviço de resposta da lista de transações por emissor.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno TraduzirModeloToServicoRespostaListaTransacoesEmissor(Redecard.PN.FMS.Modelo.TransacoesEmissor from)
        {
            PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno to = new PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno();
                        
            if (from.ListaTransacoesEmissor != null)
            {
                to.ListaTransacoesEmissor = new List<TransacaoEmissor>();
                to.ListaTransacoesEmissor.AddRange(from.ListaTransacoesEmissor.ConvertAll<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor>(TradutorTransacaoEmissor.TraduzirTransacaoEmissor));
            }

            to.QuantidadeHorasRecuperadas = from.QuantidadeHorasRecuperadas;
            to.QuantidadeHorasTotalPeriodo = from.QuantidadeHorasTotalPeriodo;
            to.QuantidadeTotalRegistros = from.QuantidadeTotalRegistros;

            return to;
        }
        
    }
}