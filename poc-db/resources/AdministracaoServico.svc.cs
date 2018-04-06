#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [24/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Collections.Generic;
using System;
using System.ServiceModel;
using Redecard.PN.Comum;
using System.Data;

namespace Redecard.PN.Sustentacao.Servicos
{

    /// <summary>
    /// Serviço para gerenciamento da administração do Portal de Serviços
    /// </summary>
    public class AdministracaoServico : ServicoBase, IAdministracaoServico
    {
        /// <summary>
        /// 
        /// </summary>
        public DataTable[] ConsultarSQL(String bancoDados, String script)
        {
            try
            {
                Negocio.Administracao adm = new Negocio.Administracao();
                return adm.ConsultarSQL(bancoDados, script);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }
    }
}
