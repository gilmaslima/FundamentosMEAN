using System;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UsuarioRetorno
    {
        /// <summary>
        /// Login de usuário não Estabelecimento
        /// </summary>
        [DataMember(Name = "codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "tipo")]
        public string TipoUsuario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "senha")]
        public string Senha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "senha_temporaria")]
        public string SenhaTemporaria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_expiracao_senha")]
        public string DataExpiracaoSenha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_ultimo_acesso")]
        public string DataUltimoAcesso { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "qtd_login_incorreto")]
        public int QtdLoginIncorreto { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "qtd_confirmacao_positiva")]
        public int QtdConfirmacaoPositiva { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "senha_migrada")]
        public bool SenhaMigrada { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "codigo_id")]
        public int CodigoId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "email")]
        public String Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "email_secundario")]
        public String EmailSecundario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "email_temporário")]
        public String EmailTemporario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "cpf")]
        public String Cpf { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ddd")]
        public String Ddd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "celular")]
        public String Celular { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_ultima_atualizacao_senha")]
        public string DataUltimaAtualizacaoSenha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "data_inclusao")]
        public string DataInclusao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "mensagem_liberacao")]
        public bool IndicadorMensagemLiberacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "status_codigo")]
        public int StatusCodigo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "status_descricao")]
        public string StatusDescricao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "legado")]
        public bool IndicadorLegado { get; set; }
    }
}
