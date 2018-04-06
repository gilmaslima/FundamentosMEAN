#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [04/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Web;
using System;
using System.Xml.Serialization;
using Redecard.PN.Comum;
using System.Linq;

namespace Rede.PN.DadosCadastraisMobile.SharePoint
{
    /// <summary>
    /// Classe armazenada na sessão para guardar dados de login do usuário somente para confirmação positiva.
    /// </summary>
    [Serializable]
    public class InformacaoUsuario
    {
        /// <summary>
        /// Informação do Usuário
        /// </summary>
        private InformacaoUsuario() { }

        /// <summary>
        /// Chave de sessão
        /// </summary>
        [XmlAttribute]
        private const String ChaveDadosUsuario = "__keyDadosUsuario";

        /// <summary>
        /// Limpar o conteúdo da Sessão Aberta do usuário
        /// </summary>
        public static void Limpar()
        {
            if (!object.ReferenceEquals(HttpContext.Current.Session[ChaveDadosUsuario], null))
                HttpContext.Current.Session.Remove(ChaveDadosUsuario);
        }

        /// <summary>
        /// Valida se existem dados do usuário na Sessão
        /// </summary>
        /// <returns></returns>
        public static Boolean Existe()
        {
            if (!object.ReferenceEquals(HttpContext.Current.Session[ChaveDadosUsuario], null))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cria o objeto na sessão do usuário
        /// </summary>
        /// <param name="grupoEntidade">Grupo da entidade do PV</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="usuario">Usuário do PV</param>
        public static void Criar(Int32 grupoEntidade, Int32 numeroPV, String usuario)
        {
            Criar(grupoEntidade, numeroPV, usuario, "");
        }

        /// <summary>
        /// Cria o objeto na sessão do usuário
        /// </summary>
        /// <param name="grupoEntidade">Grupo da entidade do PV</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="usuario">Usuário do PV</param>
        /// <param name="tipoUsuario">Tipo de usuário</param>
        public static void Criar(Int32 grupoEntidade, Int32 numeroPV, String usuario, String tipoUsuario)
        {
            Criar(grupoEntidade, numeroPV, usuario, tipoUsuario, 0);

        }

        /// <summary>
        /// Cria o objeto na sessão do usuário
        /// </summary>
        /// <param name="grupoEntidade">Grupo da entidade do PV</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="usuario">Usuário do PV</param>
        public static void Criar(Int32 grupoEntidade, Int32 numeroPV, String usuario, Int32 _codigoRetorno)
        {
            Criar(grupoEntidade, numeroPV, usuario, "", 0);
        }

        /// <summary>
        /// Cria o objeto na sessão do usuário
        /// </summary>
        /// <param name="grupoEntidade">Grupo da entidade do PV</param>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="usuario">Usuário do PV</param>
        /// <param name="tipoUsuario">Tipo de usuário</param>
        public static void Criar(Int32 grupoEntidade, Int32 numeroPV, String usuario, String tipoUsuario, Int32 _codigoRetorno)
        {
            Limpar();

            InformacaoUsuario dados = new InformacaoUsuario()
            {
                GrupoEntidade = grupoEntidade,
                NumeroPV = numeroPV,
                EmailUsuario = usuario,
                CodigoRetorno = _codigoRetorno,
                TipoUsuario = tipoUsuario,
                CriacaoAcessoLegado = false
            };

            HttpContext.Current.Session.Add(ChaveDadosUsuario, dados);
        }

        /// <summary>
        /// Recupera da Sessão os dados do Usuário caso existam dados na Sessão
        /// </summary>
        /// <returns></returns>
        public static InformacaoUsuario Recuperar()
        {
            if (!object.ReferenceEquals(HttpContext.Current.Session[ChaveDadosUsuario], null))
                return HttpContext.Current.Session[ChaveDadosUsuario] as InformacaoUsuario;
            return null;
        }

        /// <summary>
        /// Salvar os dados de um objeto de sessão
        /// </summary>
        public static void Salvar(InformacaoUsuario usuario)
        {
            if (!object.ReferenceEquals(HttpContext.Current.Session[ChaveDadosUsuario], null))
                HttpContext.Current.Session[ChaveDadosUsuario] = usuario;
        }

        /// <summary>
        /// Salvar os dados da sessão
        /// </summary>
        public static void Salvar()
        {
            if (!object.ReferenceEquals(HttpContext.Current.Session[ChaveDadosUsuario], null))
            {
                InformacaoUsuario usuario = HttpContext.Current.Session[ChaveDadosUsuario] as InformacaoUsuario;
                HttpContext.Current.Session[ChaveDadosUsuario] = usuario;
            }
        }

        /// <summary>
        /// xml atributo confirmação booleano
        /// </summary>
        [XmlAttribute]
        public Boolean Confirmado { get; set; }

        /// <summary>
        /// xml atributo grupo entidade
        /// </summary>
        [XmlAttribute]
        public Int32 GrupoEntidade { get; set; }

        /// <summary>
        /// xml atributo cpf cnpj
        /// </summary>
        [XmlAttribute]
        private Decimal CpfCnpj = 0;

        /// <summary>
        /// xml atributo cpf cnpj Estabelecimento
        /// </summary>
        [XmlAttribute]
        public Decimal CpfCnpjEstabelecimento
        {
            get
            {
                return CpfCnpj;
            }
            set
            {
                CpfCnpj = value;
            }
        }

        /// <summary>
        /// Campo CPF do proprietário
        /// </summary>
        [XmlAttribute]
        public long? CpfProprietario { get; set; }

        /// <summary>
        /// Campo CNPJ do estabelecimento
        /// </summary>
        [XmlAttribute]
        public long? CnpjEstabelecimento { get; set; }

        /// <summary>
        /// CPF/CNPJ do sócio
        /// </summary>
        [XmlAttribute]
        public long? CpfCnpjSocio { get; set; }

        /// <summary>
        /// Valida se o PV é uma Empresa ou Usuário
        /// </summary>
        [XmlAttribute]
        public Boolean Empresa { get; set; }

        /// <summary>
        /// Número do PV
        /// </summary>
        [XmlAttribute]
        public Int32 NumeroPV { get; set; }

        /// <summary>
        /// Código ID do Usuário
        /// </summary>
        [XmlAttribute]
        public Int32 IdUsuario { get; set; }

        /// <summary>
        /// Hash de envio de e-mail do usuário
        /// </summary>
        [XmlAttribute]
        public Guid HashEmail { get; set; }

        /// <summary>
        /// Indicador se o estabelecimento do usuário possui Usuário Master
        /// </summary>
        [XmlAttribute]
        public Boolean EntidadePossuiMaster { get; set; }

        /// <summary>
        /// xml atributo usuario
        /// </summary>
        [XmlAttribute]
        public String Usuario { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [XmlAttribute]
        public String Senha { get; set; }

        /// <summary>
        /// Indicador se a Senha do usuário está expirada
        /// </summary>
        [XmlAttribute]
        public Boolean SenhaExpirada { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [XmlAttribute]
        public String NomeCompleto { get; set; }

        /// <summary>
        /// E-mail da Entidade
        /// </summary>
        [XmlAttribute]
        public String EmailEntidade { get; set; }

        /// <summary>
        /// E-mail do Usuario
        /// </summary>
        [XmlAttribute]
        public String EmailUsuario { get; set; }

        /// <summary>
        /// E-mail secundário do usuário
        /// </summary>
        [XmlAttribute]
        public String EmailSecundario { get; set; }

        /// <summary>
        /// DDD Celular do usuário
        /// </summary>
        [XmlAttribute]
        public Int32 DddCelularUsuario { get; set; }

        /// <summary>
        /// Número Celular do usuário
        /// </summary>
        [XmlAttribute]
        public Int32 CelularUsuario { get; set; }

        /// <summary>
        /// CPF do usuário
        /// </summary>
        [XmlAttribute]
        public long CpfUsuario { get; set; }

        /// <summary>
        /// Indica se trata-se de uma Recuperação de Usuário
        /// </summary>
        [XmlAttribute]
        public Boolean EsqueciUsuario { get; set; }

        /// <summary>
        /// Hash com a senha do usuário para gravar na base de dados ao realizar a Confirmação Positiva
        /// </summary>
        [XmlAttribute]
        public String SenhaUsuario { get; set; }

        /// <summary>
        /// Indica se o Estabelecimento é EMP/IBBA
        /// </summary>
        [XmlAttribute]
        public Boolean EstabelecimentoEmpIbba { get; set; }

        /// <summary>
        /// Indica se o PV possui Komerci
        /// </summary>
        [XmlAttribute]
        public Boolean PossuiKomerci { get; set; }

        /// <summary>
        /// Indica o tipo do Usuário (Básico, Personalisado, Completo)
        /// </summary>
        [XmlAttribute]
        public String TipoUsuario { get; set; }

        /// <summary>
        /// Indicador se o usuário pode ler o seu e-mail recuperado 
        /// ou cadastrar nova senha na recuperação, 
        /// caso precise realizar a confirmação positiva na Recuperação de Usuário
        /// </summary>
        [XmlAttribute]
        public Boolean PodeRecuperarCriarAcesso { get; set; }

        /// <summary>
        /// Indicador se o usuário é Legado criando seu novo acesso
        /// </summary>
        [XmlAttribute]
        public Boolean CriacaoAcessoLegado { get; set; }

        /// <summary>
        /// Código de Recuperação de Senha por SMS
        /// </summary>
        [XmlAttribute]
        public String CodigoRecuperacaoSMS { get; set; }

        /// <summary>
        /// Código de retorno de validações
        /// </summary>
        [XmlAttribute]
        public Int32 CodigoRetorno { get; set; }

        /// <summary>Forma de recuperação da senha</summary>
        public FormaRecuperacao FormaRecuperacaoSenha { get; set; }

        /// <summary>
        /// Pvs selecionados caso existe mais de um pv relacionado ao usuário
        /// </summary>
        [XmlAttribute]
        public int[] PvsSelecionados { get; set; }

        /// <summary>
        /// Pvs selecionados caso existe mais de um pv relacionado ao usuário
        /// </summary>
        [XmlAttribute]
        public EntidadeServico.EntidadeServicoModel[] EstabelecimentosRelacinados { get; set; }

        /// <summary>
        /// Retorna uma string de pvs selecionados
        /// </summary>
        public string ObterPvsSelecionadosToString()
        {
            if (PvsSelecionados != null && PvsSelecionados.Any())
            {
                return string.Join("|", PvsSelecionados);
            }

            return null;
        }

        /// <summary>
        /// Status do usuário
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// PV com senhas iguais.
        /// </summary>
        public bool PvSenhasIguais { get; set; }

        /// <summary>
        /// Quantidade de e-mails diferentes na recuperação de usuário;
        /// </summary>
        public int QuantidadeEmaislDiferentes { get; set; }

        /// <summary>
        /// Retorna uma string de pvs selecionados
        /// </summary>
        public string GetPvsSelecionados()
        {
            if (PvsSelecionados != null && PvsSelecionados.Any())
            {
                return string.Join("|", PvsSelecionados);
            }

            return null;
        }

        /// <summary>
        /// Indica se a entidade possui usuários.
        /// </summary>
        public bool EntidadePossuiUsuario { get; set; }
    }
}
