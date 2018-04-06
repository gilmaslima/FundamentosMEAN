#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [24/05/2012] – [André Garcia] – [Criação]
- [26/11/2015] – [Rodrigo Rodrigues] – Migração do método ConsultarSQL() para novo projeto (Redecard.PN.Sustentacao.Servicos)
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;

namespace Redecard.PN.Sustentacao.Servicos
{
    
    [ServiceContract]
    public interface IAdministracaoServico 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bancoDados"></param>
        /// <param name="script"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DataTable[] ConsultarSQL(String bancoDados, String script);
    }
}