using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Modelo para transportar os dados do estabelecimento para a camada de client side
    /// </summary>
    [Serializable]
    public class ConsultaPvsHandlerModel
    {
        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        public Int32 NumeroPv { get; set; }

        /// <summary>
        /// Id do Usuario
        /// </summary>
        public Int32 IdUsuario { get; set; }
        
        /// <summary>
        /// Número do estabelecimento
        /// </summary>
        public String NumeroPvMascarado { get; set; }

        /// <summary>
        /// Nome do estabelecimento
        /// </summary>
        public String NomeEstabelecimento { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public String EmailSecundario { get; set; }
        
        /// <summary>
        /// Celular do usuário.
        /// </summary>
        public Int32? Celular { get; set; }

        /// <summary>
        /// Celular do usuário.
        /// </summary>
        public Int32? DDDCelular { get; set; }

        /// <summary>
        /// Status do estabelecimento.
        /// </summary>
        public Int32? Status { get; set; }

        /// <summary>
        /// Identifica se o PV possui usuário master
        /// </summary>
        public Boolean? PossuiMaster { get; set; }

        /// <summary>
        /// Identifica se o PV possui usuário.
        /// </summary>
        public Boolean? PossuiUsuario { get; set; }
    }
}
