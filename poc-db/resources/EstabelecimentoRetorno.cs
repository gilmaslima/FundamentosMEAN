using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// Classe de Serialização do retorno do Login com Sucesso
    /// </summary>
    [DataContract]
    public class EstabelecimentoRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo")]
        public Int32 CodigoEntidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "tipo_entidade")]
        public int TipoEntidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "razao_social")]
        public string RazaoSocial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "nome")]
        public string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "uf")]
        public string Uf { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "transaciona_dolar")]
        public bool TransacionaDolar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_matriz")]
        public int CodigoMatriz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_alteracao")]
        public string DataAlteracao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_cash")]
        public string IndicadorDataCash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_ativacao_data_cash")]
        public string DataAtivacaoDataCash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_segmento")]
        public string CodigoSegmento { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_ramo")]
        public int CodigoRamo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_grupo_ramo")]
        public int CodigoGrupoRamo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_canal")]
        public int CodigoCanal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_celula")]
        public int CodigoCelula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "recarga")]
        public string Recarga { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_tecnologia")]
        public int CodigoTecnologia { get; set; }
    }
}
