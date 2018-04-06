using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DownloadMes
    {
        [DataMember]
        public string Mes { get; set; }

        [DataMember]
        public string Caminho { get; set; }

        [DataMember]
        public int MesId { get; set; }

        [DataMember]
        public int Ano { get; set; }
    }
}
