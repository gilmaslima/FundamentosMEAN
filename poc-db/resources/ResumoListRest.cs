using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos.Modelos
{
    [DataContract]
    public class ResumoListRest : ResponseBaseList<ResumoTaxa>
    {
    }
}