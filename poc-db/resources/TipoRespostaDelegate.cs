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
using System.Text;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;

namespace Redecard.PN.FMS.Sharepoint.Delegate
{
    /// <summary>
    /// Publica o serviço 'Tipos de Resposta Delegate' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public class TipoRespostaDelegate
    {
        /// <summary>
        /// Este método é utilizado para  pesquisar o tipo de resposta.
        /// </summary>
        /// <param name="grupoEntidade"></param>
        /// <param name="numeroEmissor"></param>
        /// <param name="usuariologin"></param>
        /// <returns></returns>
        public List<Servico.FMS.TipoResposta> PesquisaTipoResposta(int grupoEntidade, int numeroEmissor, string usuariologin)
        {
            using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
            {
                Servico.FMS.PesquisaTipoRespostaEnvio envio = new Servico.FMS.PesquisaTipoRespostaEnvio()
                {
                    GrupoEntidade = grupoEntidade,
                    NumeroEmissor = numeroEmissor,//TODO:WILL Verificar
                    UsuarioLogin = usuariologin
                };

                PesquisaTipoRespostaRetorno objRetorno = objClient.PesquisarTiposResposta(envio);

                return objRetorno.ListaTipoResposta == null ? null : objRetorno.ListaTipoResposta.ToList();
            }
        }

    }
}
