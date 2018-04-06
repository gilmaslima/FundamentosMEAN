using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Boston.Servicos
{
    [ServiceContract]
    public interface IToken
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String GetTokenAnaliseRisco(String cpfCnpj, String nome, String sobrenome, DateTime dataFundacao, String email, String telefone1, String telefone2, String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos, Endereco enderecoPrincipal, Endereco enderecoEntrega, Endereco enderecoCobranca);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String GetToken(String numPdv, Decimal valorTransacao, String numPedido, Int32 qtdParcela, String urlRetorno, IEnumerable<Int32> codServicos);
    }
}
