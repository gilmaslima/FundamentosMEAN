using Redecard.PN.Comum;
using System;
using System.Text.RegularExpressions;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Class modelo para o cadastro de usuário
    /// </summary>
    public class UsuarioCriacaoModel
    {
        /// <summary>
        /// Regex padrão do campo de celular
        /// </summary>
        private static Regex RegexCelular
        {
            get
            {
                return new Regex(@"^\((?<DDD>[0-9]{2})\)\s(?<Parte1>[0-9]{4,5})\-(?<Parte2>[0-9]{4})$");
            }
        }

        public Int32 IdUsuario { get; set; }

        public String TipoUsuario { get; set; }

        /// <summary>
        /// Tipo de pessoa
        /// </summary>
        public String TipoPessoa { get; set; }

        /// <summary>
        /// Retorna se o cadastro é para pessoa jurídica
        /// </summary>
        public Boolean IsPJ
        {
            get
            {
                return String.Compare(this.TipoPessoa, "J", true) == 0;
            }
        }

        /// <summary>
        /// Retorna se o cadastro é para pessoa física
        /// </summary>
        public Boolean IsPF
        {
            get
            {
                return String.Compare(this.TipoPessoa, "F", true) == 0;
            }
        }

        /// <summary>
        /// CNPJ do estabelecimento
        /// </summary>
        public long CpfCnpjEstabelecimento { get; set; }

        /// <summary>
        /// CPF do usuároi
        /// </summary>
        public long CpfUsuario { get; set; }

        /// <summary>
        /// CPF/CNPJ do sócio
        /// </summary>
        public long CpfCnpjSocio { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// Número do celular completo
        /// </summary>
        public String CelularCompleto { get; set; }

        /// <summary>
        /// DDD do celular
        /// </summary>
        public Int32? CelularDdd
        {
            get
            {
                Match match = RegexCelular.Match(this.CelularCompleto);
                if (match.Success)
                    return match.Groups["DDD"].Value.ToInt32Null();
                else
                    return null;
            }
        }

        /// <summary>
        /// Celular parcial: sem o DDD
        /// </summary>
        public Int32? CelularNumero
        {
            get
            {
                Match match = RegexCelular.Match(this.CelularCompleto);
                if (match.Success)
                    return String.Concat(match.Groups["Parte1"], match.Groups["Parte2"]).ToInt32Null();
                else
                    return null;
            }
        }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Senha para login no portal
        /// </summary>
        public String Senha { get; set; }

        /// <summary>
        /// Senha criptografada para login
        /// </summary>
        public String SenhaCriptografada
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Senha))
                    return String.Empty;

                return EncriptadorSHA1.EncryptString(Senha);
            }
        }

        /// <summary>
        /// Banco para confirmação positiva
        /// </summary>
        public Int32 Banco { get; set; }

        /// <summary>
        /// Agência para confirmação positiva
        /// </summary>
        public String Agencia { get; set; }

        /// <summary>
        /// Conta corrente para confirmação positiva
        /// </summary>
        public String Conta { get; set; }

        /// <summary>
        /// Lista com os PVs selecionados
        /// </summary>
        public Int32[] PvsSelecionados { get; set; }

        /// <summary>
        /// Identifica se a entidade possui master
        /// </summary>
        public Boolean EntidadePossuiMaster { get; set; }

        /// <summary>
        /// Identifica se a entidade possui usuário
        /// </summary>
        public Boolean EntidadePossuiUsuario { get; set; }

        /// <summary>
        /// Hash de envio de e-mail
        /// </summary>
        public Guid HashEmail { get; set; }
    }
}
