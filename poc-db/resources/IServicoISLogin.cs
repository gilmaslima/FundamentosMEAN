using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Interface de serviço para validar acesso de Login - Migração do wsLogin do IS
    /// </summary>
    [ServiceContract]
    public interface IServicoISLogin
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void AutenticarUsuarioKO(Int32 codigoEntidade, String codigoUsuario, String senhaUsuario, out Int32 codigoRetorno, out String mensagemRetorno, bool senhaCriptografada);

    }
}
