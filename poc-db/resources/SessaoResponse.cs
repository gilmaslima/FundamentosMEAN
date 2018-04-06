using System.Runtime.Serialization;

namespace Rede.PN.SessaoPortal.SharePoint.SessaoPortal.Modelos
{
    [DataContract]
    public class SessaoResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string TipoTokenApi { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string TokenApi { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int CodigoEntidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string NomeEntidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LoginUsuario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string UltimoAcesso { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string StatusPV { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool TransacionaDolar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool PVFisico { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool PVLogico { get; set; }

        /// <summary>
        /// Código do segmento do estabelecimento.
        /// </summary>
        [DataMember]
        public char CodigoSegmento { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [DataMember]
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Indica se é um usuario da Central
        /// </summary>
        [DataMember]
        public bool UsuarioAtendimento { get; set; }

        /// <summary>
        /// Indica o id do usuario logado
        /// </summary>
        [DataMember]
        public int CodigoIdUsuario { get; set; }

        /// <summary>
        /// Indica se o PV possui filiais
        /// </summary>
        [DataMember]
        public bool AcessoFilial { get; set; }

        /// <summary>
        /// Indicador de usuário legado - Novo Acesso.
        /// </summary>
        [DataMember]
        public bool Legado { get; set; }

        /// <summary>
        /// Código do Grupo da Entidade (Estabelecimento, Emissores e etc....)
        /// </summary>
        [DataMember]
        public int GrupoEntidade { get; set; }
    }
}
