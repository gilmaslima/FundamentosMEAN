using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response
{
    [DataContract]
    public class ItemEntidadeResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Categoria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Centralizador { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Matriz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Moeda { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string NomeComercial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PontoVenda { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TipoEstabelecimento { get; set; }
    }
}
