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
using Redecard.PN.FMS.Agente;
using Redecard.PN.FMS.Modelo;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe CriterioSelecaoBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Criterio de Seleção para o FMS.
    /// </summary>
    public class CriterioSelecaoBLL
    {
        /// <summary>
        /// Este método é utilizado para atualizar os critérios de seleção na camada de negócios do FMS
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="criterio"></param>
        public void AtualizarCriteriosSelecao(int numeroEmissor, int grupoEntidade, string usuarioLogin, CriteriosSelecao criterio)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            fmsClient.AtualizarCriteriosSelecao(numeroEmissor, usuarioLogin, grupoEntidade, criterio);

        }
        /// <summary>
        /// Este método é utilizado para pesquisar os critérios de seleção por usuário na camada de negócios do FMS
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public CriteriosSelecao PesquisarCriteriosSelecaoPorUsuarioLogin(int numeroEmissor, int grupoEntidade, string usuarioLogin, string usuario)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            CriteriosSelecao criterioSelecao = fmsClient.PesquisarCriteriosSelecaoPorUsuarioLogin(numeroEmissor, grupoEntidade, usuarioLogin, usuario);

            return criterioSelecao;
        }

    }
}
