#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [04/06/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Redecard.PN.Servicos
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface ITrataErroServico
    {
        /// <summary>
        /// Consulta mensagem de erro
        /// </summary>
        /// <param name="fonte">Nome do serviço e do método que gerou o erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        [OperationContract]
        Servicos.TrataErro Consultar(String fonte, Int32 codigo);

        /// <summary>
        /// Consultar mensagem de erro
        /// </summary>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Mensagem de erro</returns>
        [OperationContract(Name="ConsultarPorCodigo")]
        Servicos.TrataErro Consultar(Int32 codigo);

        /// <summary>
        /// Atualizar mensagem de erro
        /// </summary>
        /// <param name="mensagem">Objeto modelo com a nova mensagem e seu código de erro</param>
        /// <returns>0</returns>
        [OperationContract]
        Int16 AtualizarMensagem(Servicos.TrataErro mensagem);
    }
}
