using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDirf" in both code and config file together.
    [ServiceContract]
    public interface IDirf
    {
        #region WACA075 - Dirf.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarExtratoDirfRetorno ConsultarExtratoDirf(out StatusRetorno statusRetorno, ConsultarExtratoDirfEnvio envio);
        #endregion

        #region WACA079 - Dirf.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ConsultarAnosBaseDirfRetorno ConsultarAnosBaseDirf(out StatusRetorno statusRetorno);
        #endregion
    }
}
