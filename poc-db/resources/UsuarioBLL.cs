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
using Redecard.PN.FMS.Agente;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe UsuarioBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Usuário para o FMS.
    /// </summary>
    public class UsuarioBLL
    {
        /// <summary>
        /// Este método é utilizado para pesquisar usuários por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntididade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntididade, string usuarioLogin)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            string[] resposta = fmsClient.PesquisarUsuariosPorEmissor(numeroEmissor, grupoEntididade, usuarioLogin);

            return resposta;

        }
    }
}
