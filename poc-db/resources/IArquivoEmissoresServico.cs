using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Emissores.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IArquivoEmissoresServico" in both code and config file together.
    [ServiceContract]
    public interface IArquivoEmissoresServico
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        System.IO.Stream DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void SalvarArquivo(DadosArquivo dadosArquivo);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DownloadMes>  ObterPeriodosDisponiveis(string codEmissor, string anoInicial, string anoFinal);
    }
}
