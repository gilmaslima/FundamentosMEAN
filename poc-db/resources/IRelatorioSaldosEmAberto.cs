using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    [ServiceContract]
    public interface IRelatorioSaldosEmAberto
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Modelo.PeriodoDisponivel> ConsultarPeriodosDisponiveis(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RetornoConsultaSaldosEmAberto ConsultarSaldosEmAbertoOnline(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RetornoConsultaSaldosEmAberto ConsultarSaldosEmAbertoVSAM(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato);
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 IncluirSolicitacao(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio);
    }
}
