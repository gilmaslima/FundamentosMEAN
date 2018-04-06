#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Contrato do serviço de Material de Venda
    /// </summary>
    [ServiceContract]
    public interface IEmissorServico
    {
        /// <summary>
        /// Consulta os dados de um cartão
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.Cartao> ConsultarBancoEmissor(Int32 codigoBin, out Int32 codigoRetorno);
    }
}