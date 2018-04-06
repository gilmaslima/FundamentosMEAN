using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class EntidadeRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo")]
        public int Codigo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "tipo_entidade")]
        public int CodigoGrupoEntidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "nome")]
        public string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_alteracao")]
        public string DataAlteracao { get; set; }

        /// <summary>
        /// Data de último login realizado com o PV
        /// </summary>
        public DateTime DataAlteracaoDt { get; set; }
    }
}
