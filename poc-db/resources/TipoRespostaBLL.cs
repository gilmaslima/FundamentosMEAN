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
    /// Este componente publica a classe TipoRespostaBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Tipo de Resposta para o FMS.
    /// </summary>
    public class TipoRespostaBLL
    {
        /// <summary>
        /// Este método é utilizado para pesquisar lista de tipos de resposta. 
        /// </summary>
        /// <param name="usuarioLogin"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <returns></returns>
        public List<TipoResposta> PesquisarListaTiposResposta(string usuarioLogin, int numeroEmissor, int grupoEntidade)
        {
            List<TipoResposta> tipoResposta;
            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            tipoResposta = fmsClient.PesquisarListaTiposResposta(usuarioLogin, numeroEmissor, grupoEntidade);

            return tipoResposta;
        }
    }
}
