using System;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Generic;
using Redecard.PN.Comum;

namespace Redecard.PN.Comum1
{
    public class Sessao
    {
        
        /// <summary>
        /// Identificar da sessão do usuário
        /// </summary>
        public Int32 IDSessao { get; set; }

        /// <summary>
        /// Verifica se existe uma sessão criada para o contexto web atual
        /// </summary>
        /// <returns>TRUE: existe uma sessão criada para o contexto web atual</returns>
        public static bool Contem()
        {
            // verifica se existe uma sessão web atual
            if (object.ReferenceEquals(HttpContext.Current, null))
                return false;

            HttpContext context = HttpContext.Current;
            return (!object.ReferenceEquals(context.Session[ChaveSessao], null) ? true : false);
        }

        /// <summary>
        /// Obtem a sessão atual do usuário, retorna nulo caso a sessão 
        /// ainda não exista
        /// </summary>
        /// <returns>Sessão atual do usuário</returns>
        public static Sessao Obtem()
        {
                return new Sessao();

            return null;
        }

        /// <summary>
        /// Chave da sessão do usuário, deve ser usada em cada chamada a session
        /// </summary>
        public const String ChaveSessao = "__key__SessaoUsuario";

        /// <summary>
        /// 
        /// </summary>
        private Int32 _codigoEntidade;

        /// <summary>
        /// Código/Número da Entidade
        /// </summary>
        [XmlAttribute("CodigoEntidade")]
        public Int32 CodigoEntidade
        {
            get
            {
                return 33;
            }
            set
            {
                _codigoEntidade = value;
            }
        }


        /// <summary>
        /// Código/Número da Matriz
        /// </summary>
        [XmlAttribute("CodigoEntidadeMatriz")]
        public Int32 CodigoEntidadeMatriz
        {
            get
            {
                return 3;
            }
        }

        /// <summary> 
        /// Membro do menu do usuário logado no portal de serviços
        /// </summary>
        private List<Menu> _menu = null;

        /// <summary>
        /// Lista de páginas que o usuário possui acesso.
        /// </summary>
        private List<Pagina> _paginas = null;

        /// <summary>
        /// Menu do usuário logado no portal de serviços
        /// </summary>
        [XmlElement("Menu")]
        public List<Menu> Menu
        {
            get
            {
                if (object.ReferenceEquals(_menu, null))
                {
                    _menu = new List<Menu>();
                }
                return _menu;
            }
        }

        /// <summary>
        /// Páginas do usuário logado no portal de serviços
        /// </summary>
        [XmlElement("Paginas")]
        public List<Pagina> Paginas
        {
            get
            {
                if (object.ReferenceEquals(_paginas, null))
                {
                    _paginas = new List<Pagina>();
                }
                return _paginas;
            }
        }

        /// <summary>
        /// Verificar se o usuário possui acesso a página especificada
        /// </summary>
        public Boolean VerificarAcessoPagina(String relativeUrl)
        {
            if (this.UsuarioMaster())
                return true;
            Pagina p = Paginas.FirstOrDefault(x => x.Url.ToLowerInvariant() == relativeUrl.ToLowerInvariant());
            return !object.ReferenceEquals(p, null);
        }

        /// <summary>
        /// Membro Status do PV
        /// </summary>
        private String _statusPV = null;

        /// <summary>
        /// Define o Status do PV Ativo = A ou Cancelado = C
        /// </summary>
        [XmlElement("StatusPV")]
        public String StatusPV
        {
            set
            {
                _statusPV = value;
            }
        }

        /// <summary>
        /// Retorna Status do PV
        /// </summary>
        /// <returns>Status do PV</returns>
        public Boolean StatusPVCancelado()
        {
            // A = Ativo
            // C = Cancelado
            if (this.AcessoFilial)
                return _statusPVFilial.ToLowerInvariant() == "a" ? false : true;
            else
                return _statusPV.ToLowerInvariant() == "a" ? false : true;
        }

        /// <summary>
        /// Verifica o se o tipo do usuário é Master
        /// </summary>
        /// <returns>Se o usuário é Master</returns>
        public Boolean UsuarioMaster()
        {
            // M = Master
            // P = Padão
            return true;
        }

        #region Dados/Métodos Filial

        [XmlAttribute("NomeEntidadeFilial")]
        private String _nomeEntidadeFilial;

        [XmlAttribute("CNPJFilial")]
        private String _cnpjEntidadeFilial;

        [XmlAttribute("CodigoEntidadeFilial")]
        private Int32 _codigoEntidadeFilial;

        [XmlAttribute("NomeUsuarioFilial")]
        private String _nomeUsuarioFilial;

        [XmlAttribute("LoginUsuarioFilial")]
        private String _loginUsuarioFilial;

        [XmlAttribute("StatusPVFilial")]
        private String _statusPVFilial;

        [XmlAttribute("EmailFilial")]
        private String _emailFilial;


        /// <summary>
        /// Alterar objetos de sessão para os novos dados da filial
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="loginUsuario">Login do usuário</param>
        /// <param name="nomeEntidade">Nome da entidade</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="statusPV">Status do PV</param>        
        public void AcessarFilial(String nomeEntidade, Int32 codigoEntidade, Int32 tipoTecnologia, String cnpj, String nomeUsuario, String loginUsuario, String statusPV, String emailEntidade)
        {
            if (!this.AcessoFilial)
            {
                // setar variaveis da filial
                _nomeEntidadeFilial = nomeEntidade;
                _codigoEntidadeFilial = codigoEntidade;
                _tecnologiaFilial = tipoTecnologia;
                _cnpjEntidadeFilial = cnpj;
                _nomeUsuarioFilial = nomeUsuario;
                _loginUsuarioFilial = loginUsuario;
                _statusPVFilial = statusPV;
                _emailEntidade = emailEntidade;
                //_nomeMatriz = this.no
            }
        }

        /// <summary>
        /// Voltar para os dados de acesso a Matriz
        /// </summary>
        public void RetornarMatriz()
        {
            // Verificar se é filial
            if (this.AcessoFilial)
            {
                // igualar as propriedades das filiais a nulo
                _nomeEntidadeFilial = String.Empty;
                _codigoEntidadeFilial = 0;
                _tecnologiaFilial = 0;
                _nomeUsuarioFilial = String.Empty;
                _loginUsuarioFilial = String.Empty;
                _cnpjEntidadeFilial = String.Empty;
                _statusPVFilial = String.Empty;
                _emailFilial = String.Empty;
            }
        }

        /// <summary>
        /// Verificar se o acesso é de uma filial. Para essa propriedade retornar verdadeiro é necessário
        /// ter realizado a operação de acesso a filial a partir da Matriz.
        /// </summary>
        public bool AcessoFilial
        {
            get
            {
                return !String.IsNullOrEmpty(_nomeEntidadeFilial);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private String _nomeEntidade;

        /// <summary>
        /// Nome/Descrição da Entidade
        /// </summary>
        [XmlAttribute("NomeEntidade")]
        public String NomeEntidade
        {
            get
            {
                if (this.AcessoFilial)
                    return _nomeEntidadeFilial;
                else
                    return _nomeEntidade;
            }
            set
            {
                _nomeEntidade = value;
            }
        }

        /// <summary>
        /// Nome/Descrição da Matriz da Filial
        /// </summary>
        [XmlAttribute("NomeEntidadeMatriz")]
        public String NomeEntidadeMatriz
        {
            get
            {
                return _nomeEntidade;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private String _loginUsuario;

        /// <summary>
        /// Login do Usuário Autenticado
        /// </summary>
        [XmlAttribute("LoginUsuario")]
        public String LoginUsuario
        {
            get
            {
                if (this.AcessoFilial)
                    return _loginUsuarioFilial;
                else
                    return _loginUsuario;
            }
            set
            {
                _loginUsuario = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private String _nomeUsuario;

        /// <summary>
        /// Nome do Usuário Autenticado
        /// </summary>
        [XmlAttribute("NomeUsuario")]
        public String NomeUsuario
        {
            get
            {
                return "teste";
            }
            set
            {
                _nomeUsuario = value;
            }
        }

        /// <summary>
        /// Tipo do Usuário Autenticado
        /// </summary>
        [XmlAttribute("TipoUsuario")]
        public String TipoUsuario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("CNPJ")]
        private String _cnpj;

        /// <summary>
        /// CNPJ do Estabelecimento
        /// </summary>
        [XmlAttribute("CNPJEntidade")]
        public String CNPJEntidade
        {
            get
            {
                if (this.AcessoFilial)
                    return _cnpjEntidadeFilial;
                else
                    return _cnpj;
            }
            set
            {
                _cnpj = value;
            }
        }

        [XmlAttribute("TipoTecnologia")]
        Int32 _tecnologia = 0;

        [XmlAttribute("TipoTecnologiaFilial")]
        Int32 _tecnologiaFilial = 0;

        /// <summary>
        /// Tipo de tecnologia do Estabelecimento
        /// </summary>
        [XmlAttribute("Tecnologia")]
        public Int32 Tecnologia
        {
            get
            {
                if (this.AcessoFilial)
                    return _tecnologiaFilial;
                else
                    return _tecnologia;
            }
            set
            {
                _tecnologia = value;
            }
        }

        /// <summary>
        /// Código do Grupo da Entidade (Estabelecimento, Emissores e etc....)
        /// </summary>
        [XmlAttribute("GrupoEntidade")]
        public Int32 GrupoEntidade { get { return 3; } }

        /// <summary>
        /// Data do último acesso do usuário
        /// </summary>
        [XmlAttribute("UltimoAcesso")]
        public DateTime UltimoAcesso { get; set; }

        /// <summary>
        /// Indica se o usuário é Atendimento
        /// </summary>
        [XmlAttribute("UsuarioAtendimento")]
        public Boolean UsuarioAtendimento { get; set; }


        /// <summary>
        /// Email do estabelecimento do Usuário Autenticado
        /// </summary>
        [XmlAttribute("EmailEntidade")]
        private String _emailEntidade;

        /// <summary>
        /// Email do estabelecimento do Usuário Autenticado
        /// </summary>
        [XmlAttribute("EmailEntidade")]
        public String EmailEntidade
        {
            get
            {
                if (this.AcessoFilial)
                    return _emailFilial;
                else
                    return _emailEntidade;
            }
            set
            {
                _emailEntidade = value;
            }
        }

        /// <summary>
        /// Indica se a entidade transanciona em dolar
        /// </summary>
        [XmlAttribute("TransacionaDolar")]
        public Boolean TransacionaDolar { get; set; }
    


    }
}






