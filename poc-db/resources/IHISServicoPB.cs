using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Interface de serviçopara acessoao componente PB de outras entidades
    /// </summary>
    [ServiceContract]
    public interface IHISServicoPB
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.BancoGrade> ConsultarBanco(out string mensagem, out string codRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Servicos.GradeLiquidacao> ExtrairDadosSPB(string ispb, string usuario, out int codRetorno,
           out string mensagem, out string retorno, out String dataContabil);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Servicos.GradeLiquidacaoBandeira ExtrairDetalhesSPB(string ispb, string usuario, out int codRetorno,
            out string mensagem, out string retorno, out String dataContabil);

    }
}
