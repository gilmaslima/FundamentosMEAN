/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe ProdutividadeBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Produtividade para o FMS.
    /// </summary>
    public class ProdutividadeBLL
    {
        /// <summary>
        /// Este método é utilizado para pesquisar dados do relatório de produtividade por data.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            RelatorioProdutividadePorData relatorio = fmsClient.PesquisarRelatorioProdutividadePorData(numeroEmissor,
                grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, criterio, ordem);

            return relatorio;

        }
        /// <summary>
        /// Este método é utilizado para pesquisar dados do relatório de produtividade por analista.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="criterio"></param>
        /// <param name="ordem"></param>
        /// <returns></returns>
        public RelatorioProdutividadePorAnalista PesquisarRelatorioProdutividadePorAnalista(int numeroEmissor,
            int grupoEntidade, string usuarioLogin, string usuario, DateTime dataInicial, DateTime dataFinal, CriterioOrdemProdutividade criterio, OrdemClassificacao ordem)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            RelatorioProdutividadePorAnalista relatorio = fmsClient.PesquisarRelatorioProdutividadePorAnalista(numeroEmissor,
                grupoEntidade, usuarioLogin, usuario, dataInicial, dataFinal, criterio, ordem);

            return relatorio;

        }


    }
}
