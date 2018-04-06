using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICaptchaServico" in both code and config file together.
    [ServiceContract]
    public interface ICaptchaServico
    {
        [OperationContract]
        bool ValidarCaptcha(string sharedKey, string captchaResponse);
    }
}
