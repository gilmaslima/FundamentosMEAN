/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.OutrosServicos.Servicos.PlanoContas;

namespace Redecard.PN.OutrosServicos.Servicos
{
    [ServiceContract]
    public interface IHisServicoNkPlanoContas
    {
        /// <summary>
        /// Retorna as compensações de débitos de aluguel do PV incluídas no Mês/Ano informado.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0070 / Programa NKC007 / TranID NK07 / Método ConsultarCompensacoesDebitos
        /// </remarks>  
        /// <returns>Compensações Débitos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CompensacaoDebitoAluguel> ConsultarCompensacoesDebitos(
            Int32 numeroPv,
            DateTime mesAnoDebito,
            out Int16 codigoRetorno);

        /// <summary>
        /// Retorna a apuração de preço único dos 12 ultimos meses para o PV(Ponto de Venda) informado.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKNK0080 / Programa  / TranID  / Método ConsultarPlanosPrecoUnicoContratados
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKNK0080 / Programa  / TranID  / Método ConsultarPlanosPrecoUnicoContratados
        /// </remarks>  
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar os planos.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Planos Preço Único Contratados</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<PlanoPrecoUnico> ConsultarPlanosPrecoUnicoContratados(
            Int32 numeroPv,
            out Int16 codigoRetorno);
    }
}