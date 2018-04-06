using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// EntidadeServicoModel
    /// </summary>
    public class EntidadeServicoModel
    {
        /// <summary>
        /// Cto
        /// </summary>
        public EntidadeServicoModel()
        {
            PossuiUsuario = null;
            PossuiMaster = null;
            PossuiSenhaTemporaria = null;
        }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public string EmailEstabelecimento { get; set; }

        /// <summary>
        /// Senha criptografada do usuário
        /// </summary>
        public string SenhaCriptografada { get; set; }

        /// <summary>
        /// Cpf usuário
        /// </summary>
        public long CpfUsuario { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public String NomeUsuario { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public String EmailsMaster { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// E-mail secundário do usuário
        /// </summary>
        public String EmailSecundario { get; set; }

        /// <summary>
        /// DDD - Celular - Novo Acesso
        /// </summary>
        public Int32? DDDCelular { get; set; }

        /// <summary>
        /// Celular - Novo Acesso
        /// </summary>
        public Int32? Celular { get; set; }

        /// <summary>
        /// Numero do PV
        /// </summary>
        public int NumeroPV { get; set; }

        /// <summary>
        /// Razao social do PV
        /// </summary>
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Grupo entidade
        /// </summary>
        public int GrupoPV { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        public int IdUsuarioMaster { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Status do PV
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string NomeCompletoUsuario { get; set; }

        /// <summary>
        /// Informo que a entidade possui 
        /// </summary>
        public bool? PossuiUsuario  { get; set; }

        /// <summary>
        /// Informa que a entidade possui usuário master
        /// </summary>
        public bool? PossuiMaster  { get; set; }

        /// <summary>
        /// Informa que a entidade possui algum usuário que possui senha temp
        /// </summary>
        public bool? PossuiSenhaTemporaria { get; set; }
    }
}
