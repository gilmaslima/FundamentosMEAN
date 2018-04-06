using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{

    [DataContract]
    [Serializable]
    public class DadosCampanha
    {
        [DataMember]
        public List<ParametrosCampanha> Produtos { get; set; }

        [DataMember]
        public List<ParametrosCampanha> TaxaAdesao { get; set; }

        [DataMember]
        public List<ParametrosCampanha> CenarioAluguel { get; set; }

        [DataMember]
        public List<ParametrosCampanha> Servicos { get; set; }

        [DataMember]
        public DetalheCampanha DetalheCampanha { get; set; }

    }
}
