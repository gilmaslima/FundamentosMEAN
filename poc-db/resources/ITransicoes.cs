using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Credenciamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITransicoes" in both code and config file together.
    [ServiceContract]
    public interface ITransicoes
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso1(Proposta proposta, Endereco endereco, Endereco enderecoCorrespondencia, DomicilioBancario domCredito, List<Proprietario> proprietarios, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso2(Proposta proposta, List<Proprietario> proprietarios, Endereco enderecoComercial,
            Endereco enderecoCorrespondencia, Endereco enderecoInstalacao, DomicilioBancario domCredito, List<Produto> produtosCredito, List<Produto> produtosDebito, 
            List<Produto> produtosConstrucard, List<Patamar> patamares, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso3(Proposta proposta, List<Produto> produtosCredito, List<Produto> produtosDebito,
            List<Produto> produtosConstrucard, List<Patamar> patamares, Tecnologia tecnologia, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso4(Proposta proposta, Tecnologia tecnologia, Endereco endComercial,
            Endereco endCorrespondencia, Endereco endInstalacao, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso5(Proposta proposta, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso6(Proposta proposta, DomicilioBancario domBancarioCredito, DomicilioBancario domBancarioDebito,
            DomicilioBancario domBancarioConstrucard, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso7(Proposta proposta, Tecnologia tecnologia, out Int32 codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GravarAtualizarPasso8(Proposta proposta, List<Servico> servicos, List<ProdutoVan> produtosVan, out Int32 codRetorno);
    }
}
