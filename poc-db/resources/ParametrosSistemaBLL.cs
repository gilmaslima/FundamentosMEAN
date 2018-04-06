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
using Redecard.PN.FMS.Modelo;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe ParametrosSistemaBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Parâmetros do Sistema para o FMS.
    /// </summary>
    public class ParametrosSistemaBLL
    {
        /// <summary>
        /// Este método é utilizado para listar parâmetros de sistema.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <returns></returns>
        public ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade, string usuarioLogin)
        {
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            ParametrosSistema parametros = fmsClient.ListaParametrosSistema(numeroEmissor, grupoEntidade, usuarioLogin);

            return parametros;
        }

    }
}
