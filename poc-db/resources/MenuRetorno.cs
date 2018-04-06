using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class MenuRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_servico")]
        public int CodigoServico { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_servico_pai")]
        public Int32? CodigoServicoPai { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "nome ")]
        public string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "indicador_menu")]
        public bool IndicadorMenu { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "basico")]
        public bool IndicadorServicoBasico { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "url")]
        public String Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "descricao_botao")]
        public String DescricaoBotao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "submenu")]
        public List<MenuRetorno> SubMenu { get; set; }
    }
}
