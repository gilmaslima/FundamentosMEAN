using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHISServicoWA_DadosCadastrais" in both code and config file together.
    [ServiceContract]
    public interface IHISServicoWA_DadosCadastrais
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void GerarCarta(Int32 numeroPV,
            String codigoUsuario,
            String senha,
            String nomeUsuario,
            Int16 codigoCarta,
            String tipoEnvio,
            String endereco,
            out Int16 codigoRetorno);
    }
}
