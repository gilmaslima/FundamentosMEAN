using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Boston.Servicos
{
    [ServiceContract]
    public interface ITransicoesBoston
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RetornoGravarAtualizarPasso1 GravarAtualizarPasso1(Char tipoPessoa, Int64 cnpjCpf, String codEquipamento, Int32 codCanal, Int32 codGrupoRamo, Int32 codRamoAtividade, String cnpjCpfProprietario);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 GravarAtualizarPasso2(Proposta proposta, List<Proprietario> proprietarios, Endereco endComercial, Endereco endCorrespondencia, Endereco endInstalacao, Tecnologia tecnologia);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 GravarAtualizarPasso3(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSeqProp, DomicilioBancario domBancarioCredito);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 GravarAtualizarPasso4(Char codTipoPessoa, Int64 cnpjCpf, Int32 numSequencia, ref Int32 numSolicitacao, String usuario, String tipoEquipamento, Int32 codBanco, Endereco endereco, out String descRetorno, out Int32 numPdv);
    }
}
