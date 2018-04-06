#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [10/06/2012] – [André Rentes] – [Criação]
*/
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   : Criar métodos de menu/permissões/páginas
- [21/06/2012] – [André Rentes] – [Alteração]
*/
#endregion

using System;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe de atributos de sessão do usuário
    /// </summary>
    [Serializable]
    public class Sessao
    {
        #region Constantes

        /// <summary>
        /// Chave da sessão do usuário, deve ser usada em cada chamada a session
        /// </summary>
        public const String ChaveSessao = "__key__SessaoUsuario";

        #endregion

        #region Campos

        #region Entidade

        /// <summary>
        /// Código da entidade
        /// </summary>
        private Int32 _codigoEntidade;

        /// <summary>
        /// Nome da entidade
        /// </summary>
        private String _nomeEntidade;

        /// <summary>
        /// Membro Status do PV
        /// </summary>
        private String _statusPV = null;

        /// <summary>
        /// CNPJ do PV
        /// </summary>
        [XmlAttribute("CNPJ")]
        private String _cnpj;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("TipoTecnologia")]
        Int32 _tecnologia = 0;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("TipoTecnologiaDataCash")]
        String _tecnologiaDataCash = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("TipoTecnologiaDataCashDataAtivacao")]
        DateTime _tecnologiaDataCashDataAtivacao = DateTime.MinValue;

        /// <summary>
        /// Código do grupo do ramo de atividade
        /// </summary>
        [XmlAttribute("TipoCodigoGrupoRamo")]
        Int32 _codigoGrupoRamo = 0;

        /// <summary>
        /// Codigo do ramo de atividade
        /// </summary>
        [XmlAttribute("TipoCodigoRamoAtividade")]
        Int32 _codigoRamoAtividade = 0;

        /// <summary>
        /// Identifica se o PV possui recarga
        /// </summary>
        [XmlAttribute("Recarga")]
        String _recarga = "";

        /// <summary>
        /// Email do estabelecimento do Usuário Autenticado
        /// </summary>
        [XmlAttribute("EmailEntidade")]
        private String _emailEntidade;

        /// <summary>
        /// UF do PV
        /// </summary>
        [XmlAttribute("UF")]
        String _uf = "";

        /// <summary>
        /// Código do segmento do PV
        /// </summary>
        [XmlAttribute("TipoCodigoSegmento")]
        Char _codigoSegmento = Char.MinValue;

        /// <summary>
        /// Código da Matriz (GE)
        /// </summary>
        [XmlAttribute("TipoCodigoMatriz")]
        Int32 _codigoMatriz;

        #endregion

        #region Usuario

        /// <summary>
        /// A senha foi migrada?
        /// </summary>
        [XmlAttribute("SenhaMigrada")]
        private Boolean _senhaMigrada;

        /// <summary>
        /// Usuário escolheu migrar seus dados depois
        /// </summary>
        [XmlAttribute("MigrarDepois")]
        private Boolean _migrarDepois;

        /// <summary>
        /// Usuário deve ser direcionado para Liberação de Acesso Completo
        /// </summary>
        [XmlAttribute("LiberarAcessoCompleto")]
        private Boolean _liberarAcessoCompleto;

        /// <summary>
        /// Login do usuário
        /// </summary>
        private String _loginUsuario;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        private String _nomeUsuario;

        #endregion

        #region Menu

        /// <summary> 
        /// Membro do menu do usuário logado no portal de serviços
        /// </summary>
        private List<Menu> _menu = null;

        /// <summary> 
        /// Serviços do usuário logado no portal de serviços
        /// </summary>
        private List<Menu> _servicos = null;

        /// <summary>
        /// Lista de páginas que o usuário possui acesso.
        /// </summary>
        private List<Pagina> _paginas = null;

        #endregion

        #region Filial

        /// <summary>
        /// Nome da filial
        /// </summary>
        [XmlAttribute("NomeEntidadeFilial")]
        private String _nomeEntidadeFilial;

        /// <summary>
        /// CNPJ da filial
        /// </summary>
        [XmlAttribute("CNPJFilial")]
        private String _cnpjEntidadeFilial;

        /// <summary>
        /// Código da filial
        /// </summary>
        [XmlAttribute("CodigoEntidadeFilial")]
        private Int32 _codigoEntidadeFilial;

        /// <summary>
        /// Nome do usuário da filial
        /// </summary>
        [XmlAttribute("NomeUsuarioFilial")]
        private String _nomeUsuarioFilial;

        /// <summary>
        /// Login do usuário da filial
        /// </summary>
        [XmlAttribute("LoginUsuarioFilial")]
        private String _loginUsuarioFilial;

        /// <summary>
        /// Status da filial
        /// </summary>
        [XmlAttribute("StatusPVFilial")]
        private String _statusPVFilial;

        /// <summary>
        /// Email da filial
        /// </summary>
        [XmlAttribute("EmailFilial")]
        private String _emailFilial;

        /// <summary>
        /// Tipo de tecnologia da filial
        /// </summary>
        [XmlAttribute("TipoTecnologiaFilial")]
        Int32 _tecnologiaFilial = 0;

        /// <summary>
        /// Tipo de tecnologia datacash da filial
        /// </summary>
        [XmlAttribute("TipoTecnologiaDataCashFilial")]
        String _tecnologiaDataCashFilial = String.Empty;

        /// <summary>
        /// Data de ativação da datacash para filial
        /// </summary>
        [XmlAttribute("TipoTecnologiaDataCashDataAtivacaoFilial")]
        DateTime _tecnologiaDataCashDataAtivacaoFilial = DateTime.MinValue;

        /// <summary>
        /// Código do grupo do ramo de atividade da filial
        /// </summary>
        [XmlAttribute("TipoCodigoGrupoRamoFilial")]
        Int32 _codigoGrupoRamoFilial = 0;

        /// <summary>
        /// Código do ramo de atividade da filial
        /// </summary>
        [XmlAttribute("TipoCodigoRamoAtividadeFilial")]
        Int32 _codigoRamoAtividadeFilial = 0;

        /// <summary>
        /// Código do canal da filial
        /// </summary>
        [XmlAttribute("CodigoCanalFilial")]
        Int32 _codigoCanalFilial = 0;

        /// <summary>
        /// Código da célula da filial
        /// </summary>
        [XmlAttribute("CodigoCelulaFilial")]
        Int32 _codigoCelulaFilial = 0;

        /// <summary>
        /// Recarga da filial
        /// </summary>
        [XmlAttribute("recargaFilial")]
        String _recargaFilial = String.Empty;

        /// <summary>
        /// UF da filial
        /// </summary>
        [XmlAttribute("UFFilial")]
        String _ufFilial = String.Empty;

        /// <summary>
        /// Código do segmento da Filial
        /// </summary>
        [XmlAttribute("TipoCodigoSegmentoFilial")]
        Char _codigoSegmentoFilial = Char.MinValue;

        /// <summary>
        /// Código da Matriz da Filial (GE)
        /// </summary>
        [XmlAttribute("TipoCodigoMatrizFilial")]
        Int32 _codigoMatrizFilial;

        #endregion

        #endregion

        #region Propriedades

        #region Sessao

        /// <summary>
        /// Identificar da sessão do usuário
        /// </summary>
        public Int32 IDSessao { get; set; }

        /// <summary>
        /// Token de sessão no Portal Legado
        /// </summary>
        public String TokenLegado { get; set; }

        #endregion

        #region Entidade

        /// <summary>
        /// Código/Número da Entidade
        /// </summary>
        [XmlAttribute("CodigoEntidade")]
        public Int32 CodigoEntidade
        {
            get
            {
                if (this.AcessoFilial)
                    return _codigoEntidadeFilial;
                else
                    return _codigoEntidade;
            }
            set
            {
                _codigoEntidade = value;
            }
        }

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
        /// Código/Número da Matriz
        /// </summary>
        [XmlAttribute("CodigoEntidadeMatriz")]
        public Int32 CodigoEntidadeMatriz
        {
            get
            {
                return _codigoEntidade;
            }
        }

        /// <summary>
        /// Código do Grupo da Entidade (Estabelecimento, Emissores e etc....)
        /// </summary>
        [XmlAttribute("GrupoEntidade")]
        public Int32 GrupoEntidade { get; set; }

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

        /// <summary>
        /// Código do Ramo Atividade do Estabelecimento
        /// </summary>
        [XmlAttribute("CodigoRamoAtividade")]
        public Int32 CodigoRamoAtividade
        {
            get
            {
                if (this.AcessoFilial)
                    return _codigoRamoAtividadeFilial;
                else
                    return _codigoRamoAtividade;
            }
            set
            {
                _codigoRamoAtividade = value;
            }
        }

        /// <summary>
        /// Código do Grupo Ramo do Estabelecimento
        /// </summary>
        [XmlAttribute("CodigoGrupoRamo")]
        public Int32 CodigoGrupoRamo
        {
            get
            {
                if (this.AcessoFilial)
                    return _codigoGrupoRamoFilial;
                else
                    return _codigoGrupoRamo;
            }
            set
            {
                _codigoGrupoRamo = value;
            }
        }

        /// <summary>
        /// Indica se o PV é um PV Lógico
        /// </summary>
        /// <remarks>
        /// PV Lógico:<br/>
        ///     - Código Grupo Ramo:        8<br/>
        ///     - Código Ramo Atividade:    0108 
        /// </remarks>
        public Boolean PVLogico
        {
            get
            {
                return this.CodigoGrupoRamo == 8
                    && this.CodigoRamoAtividade == 0108;
            }
        }

        /// <summary>
        /// Indica se o PV é um PV Físico
        /// </summary>
        public Boolean PVFisico
        {
            get
            {
                return !this.PVLogico;
            }
        }

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
        /// Tipo de tecnologia DataCash do Estabelecimento
        /// </summary>
        [XmlAttribute("TecnologiaDataCash")]
        public String TecnologiaDataCash
        {
            get
            {
                if (this.AcessoFilial)
                    return _tecnologiaDataCashFilial;
                else
                    return _tecnologiaDataCash;
            }
            set
            {
                _tecnologiaDataCash = value;
            }
        }

        /// <summary>
        /// Tipo de tecnologia DataCash do Estabelecimento
        /// </summary>
        [XmlAttribute("TecnologiaDataCashDataAtivacao")]
        public DateTime TecnologiaDataCashDataAtivacao
        {
            get
            {
                if (this.AcessoFilial)
                    return _tecnologiaDataCashDataAtivacaoFilial;
                else
                    return _tecnologiaDataCashDataAtivacao;
            }
            set
            {
                _tecnologiaDataCashDataAtivacao = value;
            }
        }

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

        /// <summary>
        /// UF do Estabelecimento
        /// </summary>
        [XmlAttribute("UFentidade")]
        public String UFEntidade
        {
            get
            {
                if (this.AcessoFilial)
                    return _ufFilial;
                else
                    return _uf;
            }
            set
            {
                _uf = value;
            }
        }

        /// <summary>
        /// Código do canal da entidade
        /// </summary>
        [XmlAttribute("CodigoCanal")]
        public Int32 CodigoCanal { get; set; }

        /// <summary>
        /// Código do celula da entidade
        /// </summary>
        [XmlAttribute("CodigoCelula")]
        public Int32 CodigoCelula { get; set; }

        /// <summary>
        /// Código do celula da entidade
        /// </summary>
        [XmlAttribute("Recarga")]
        public String Recarga
        {
            get
            {
                if (this.AcessoFilial)
                    return _recargaFilial;
                else
                    return _recarga;
            }
            set
            {
                _recarga = value;
            }
        }

        /// <summary>
        /// Verificar se o PV atual possui recarga
        /// </summary>
        /// <returns>True: possui recaraga ou False: não possui</returns>
        public Boolean PossuiRecarga
        {
            get
            {
                return String.Compare(this.Recarga, "A", true) == 0 || 
                    String.Compare(this.Recarga, "R", true) == 0;
            }
        }

        /// <summary>
        /// Código do Segmento do estabelecimento
        /// </summary>
        [XmlAttribute("CodigoSegmento")]
        public Char CodigoSegmento
        {
            get
            {
                if (this.AcessoFilial)
                    return _codigoSegmentoFilial;
                else
                    return _codigoSegmento;
            }
            set
            {
                _codigoSegmento = value;
            }
        }

        /// <summary>
        /// Código da Matriz (GE)
        /// </summary>
        [XmlAttribute("CodigoMatriz")]
        public Int32 CodigoMatriz
        {
            get
            {
                if (this.AcessoFilial)
                    return _codigoMatrizFilial;
                else
                    return _codigoMatriz;
            }
            set
            {                
                _codigoMatriz = value;
            }
        }

        /// <summary>
        /// Indica se o PV é Matriz (GE)
        /// </summary>
        public Boolean PVMatriz
        {
            get
            {
                //Se estiver acessando Como Filial, não pode ser Matriz
                if (this.AcessoFilial)
                    return false;
                //Se não possui código da Matriz, ou o código da Matriz é igual ao PV, é Matriz
                else if (this.CodigoMatriz == 0 || this.CodigoMatriz == this.CodigoEntidade)
                    return true;
                //Se código da Matriz existe (diferente de 0), e não é igual ao PV, é Filial
                else
                    return false;
            }
        }

        /// <summary>
        /// Informa se o estabelecimento tem o serviço 600
        /// </summary>
        [XmlAttribute("PossuiAdquirencia")]
        public Boolean PossuiAdquirencia { get; set; }

        /// <summary>
        /// Indica se o PV é Carteira Digital
        /// </summary>
        public Boolean PVCarteiraDigital
        {
            get
            {
                Int32 codCanal = this.AcessoFilial ? this._codigoCanalFilial : this.CodigoCanal;
                Int32 codCelula = this.AcessoFilial ? this._codigoCelulaFilial : this.CodigoCelula;

                return codCanal == 15 && codCelula == 535;
            }
        }

        #endregion

        #region Usuario

        /// <summary>
        /// Indica se o usuário escolheu a opção de Migrar seus dados Depois
        /// </summary>
        /// <returns>False: Escolheu migrar agora ; True: Escolheu Migrar depois</returns>
        public Boolean MigrarDepois
        {
            get
            {
                return _migrarDepois;
            }
            set
            {
                _migrarDepois = value;
            }
        }

        /// <summary>
        /// Indica se a senha do usuário já foi migrada para o padrão PCI
        /// </summary>
        /// <returns>False: Senhão não migrada; True: Senha já migrada</returns>
        public Boolean SenhaMigrada
        {
            get
            {
                return _senhaMigrada;
            }
            set
            {
                _senhaMigrada = value;
            }
        }

        /// <summary>
        /// Indica se o usuário deve ser redirecionado para Liberação de Acesso Completo
        /// </summary>
        /// <returns>False: Não direcionar; True: Redirecionar</returns>
        public Boolean LiberarAcessoCompleto
        {
            get
            {
                return _liberarAcessoCompleto;
            }
            set
            {
                _liberarAcessoCompleto = value;
            }
        }

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
        /// Login do Usuário Autenticado na Matriz
        /// </summary>
        [XmlAttribute("LoginUsuarioMatriz")]
        public String LoginUsuarioMatriz
        {
            get
            {
                return _loginUsuario;
            }
            set
            {
                _loginUsuario = value;
            }
        }

        /// <summary>
        /// Nome do Usuário Autenticado
        /// </summary>
        [XmlAttribute("NomeUsuario")]
        public String NomeUsuario
        {
            get
            {
                if (this.AcessoFilial)
                    return _nomeUsuarioFilial;
                else
                    return _nomeUsuario;
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
        /// Código do Usuário - Novo Acesso
        /// </summary>
        [XmlAttribute("CodigoIdUsuario")]
        public Int32 CodigoIdUsuario { get; set; }

        /// <summary>
        /// E-mail - Novo Acesso
        /// </summary>
        [XmlAttribute("Email")]
        public String Email { get; set; }

        /// <summary>
        /// E-mail Secundario - Novo Acesso
        /// </summary>
        [XmlAttribute("EmailSecundario")]
        public String EmailSecundario { get; set; }

        /// <summary>
        /// E-mail Secundario - Novo Acesso
        /// </summary>
        [XmlAttribute("EmailTemporario")]
        public String EmailTemporario { get; set; }

        /// <summary>
        /// CPF - Novo Acesso
        /// </summary>
        [XmlAttribute("CPF")]
        public Int64? CPF { get; set; }

        /// <summary>
        /// DDD - Celular - Novo Acesso
        /// </summary>
        [XmlAttribute("DDDCelular")]
        public Int32? DDDCelular { get; set; }

        /// <summary>
        /// Celular - Novo Acesso
        /// </summary>
        [XmlAttribute("Celular")]
        public Int32? Celular { get; set; }

        /// <summary>
        /// Status do usuário - Novo Acesso
        /// </summary>
        [XmlAttribute("Status")]
        public Enumerador.Status CodigoStatus { get; set; }

        /// <summary>
        /// Indicador de usuário legado - Novo Acesso
        /// </summary>
        [XmlAttribute("Legado")]
        public Boolean Legado { get; set; }

        /// <summary>
        /// Código da Funcional, caso esteja autenticado como Usuário Atendimento
        /// </summary>
        [XmlAttribute("Funcional")]
        public String Funcional { get; set; }

        /// <summary>
        /// Flag de exibição de mensagem de liberação de acesso completo
        /// </summary>
        [XmlAttribute("ExibirMensagemLiberacaoAcessoCompleto")]
        public Boolean ExibirMensagemLiberacaoAcessoCompleto { get; set; }

        #endregion

        #region Menu

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
        /// Serviços do usuário logado no portal de serviços
        /// </summary>
        [XmlElement("Servicos")]
        public List<Menu> Servicos
        {
            get
            {
                if (object.ReferenceEquals(_servicos, null))
                {
                    _servicos = new List<Menu>();
                }
                return _servicos;
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

        #endregion

        #region DataCash/Komerci
        /// <summary>
        /// Verificar se o PV atual possui tecnologia Komerci/Datacash
        /// </summary>
        /// <returns></returns>
        public Boolean PossuiKomerci
        {
            get
            {
                return this.Tecnologia == 25 || this.Tecnologia == 26 || this.Tecnologia == 23;
            }
        }

        /// <summary>
        /// Verificar se o PV atual possui tecnologia Komerci/Datacash
        /// </summary>
        /// <returns></returns>
        public Boolean PossuiDataCash
        {
            get
            {
                return object.ReferenceEquals(this.TecnologiaDataCash, null) ? false : this.TecnologiaDataCash.Equals("S");
            }
        }

        /// <summary>
        /// Verificar se o PV atual possui tecnologia Komerci/Datacash ativado
        /// </summary>
        /// <returns></returns>
        public Boolean PossuiDataCashAtivado
        {
            get
            {
                return (
                    this.TecnologiaDataCash.Equals("S") &&
                    !this.TecnologiaDataCashDataAtivacao.Equals(DateTime.MinValue)
                );
            }
        }

        #endregion

        #region Filial

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

        #endregion

        #region Métodos

        #region Sessao

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
            if (Contem())
                return HttpContext.Current.Session[ChaveSessao] as Sessao;

            return null;
        }

        #endregion

        #region Menu

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

        #endregion

        #region Entidade

        /// <summary>
        /// Verifica se o Status do PV é cancelado
        /// </summary>
        /// <returns>Status do PV</returns>
        public Boolean StatusPVCancelado()
        {
            // A = Ativo
            // C = Cancelado
            if (this.AcessoFilial)
            {
                // Entidade não é um estabelecimento
                if (String.IsNullOrEmpty(_statusPVFilial))
                    return false;
                else
                    return _statusPVFilial.ToLowerInvariant() == "a" ? false : true;
            }
            else
            {
                // Entidade não é um estabelecimento
                if (String.IsNullOrEmpty(_statusPV))
                    return false;
                else
                    return _statusPV.ToLowerInvariant() == "a" ? false : true;
            }
        }

        #endregion

        #region Usuario

        /// <summary>
        /// Verifica o se o tipo do usuário é Master
        /// </summary>
        /// <returns>Se o usuário é Master</returns>
        public Boolean UsuarioMaster()
        {
            // M = Master
            // P = Personalizado (antigo Padrão)
            // B = Básico
            return this.TipoUsuario.ToLowerInvariant() == "m" ? true : false;
        }

        #endregion

        #region Filial

        /// <summary>
        /// Alterar objetos de sessão para os novos dados da filial
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="loginUsuario">Login do usuário</param>
        /// <param name="nomeEntidade">Nome da entidade</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="statusPV">Status do PV</param>     
        /// <param name="cnpj">CNPJ</param>
        /// <param name="codigoGrupoRamo">Código grupo ramo</param>
        /// <param name="codigoRamoAtividade">Código do ramo de atividade</param>
        /// <param name="emailEntidade">E-mail da entidade</param>
        /// <param name="ufEntidade">UF da entidade</param>
        /// <param name="tecnologiaDataCashDataAtivacao">Data de ativação Tecnologia Datacash</param>
        /// <param name="tipoTecnologia">Tipo de tecnologia</param>
        /// <param name="tipoTecnologiaDataCash">Tipo tecnologia datacash</param>
        /// <param name="codigoSegmento">Código do segmento</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="recarga">Recarga</param>
        /// <param name="codigoMatriz">Código da Matriz (GE)</param>
        public void AcessarFilial(
            String nomeEntidade, 
            Int32 codigoEntidade, 
            Int32 tipoTecnologia, 
            String cnpj,
            String nomeUsuario, 
            String loginUsuario, 
            String statusPV, 
            String emailEntidade, 
            String ufEntidade,
            String tipoTecnologiaDataCash, 
            DateTime tecnologiaDataCashDataAtivacao,
            Int32 codigoGrupoRamo, 
            Int32 codigoRamoAtividade, 
            Char codigoSegmento,
            Int32 codigoCanal, 
            Int32 codigoCelula, 
            String recarga,
            Int32 codigoMatriz)
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
                _ufFilial = ufEntidade;
                _tecnologiaDataCashFilial = tipoTecnologiaDataCash;
                _tecnologiaDataCashDataAtivacaoFilial = tecnologiaDataCashDataAtivacao;
                _codigoSegmentoFilial = codigoSegmento;
                _codigoRamoAtividadeFilial = codigoRamoAtividade;
                _codigoGrupoRamoFilial = codigoGrupoRamo;
                _codigoCanalFilial = codigoCanal;
                _codigoCelulaFilial = codigoCelula;
                _recargaFilial = recarga;
                _codigoMatrizFilial = codigoMatriz;
            }
        }

        // Compatibilidade com o sistema sem o projeto Novo Acesso
        /// <summary>
        /// Alterar objetos de sessão para os novos dados da filial
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="loginUsuario">Login do usuário</param>
        /// <param name="nomeEntidade">Nome da entidade</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="statusPV">Status do PV</param>     
        /// <param name="cnpj">CNPJ</param>
        /// <param name="codigoGrupoRamo">Código grupo ramo</param>
        /// <param name="codigoRamoAtividade">Código do ramo de atividade</param>
        /// <param name="emailEntidade">E-mail da entidade</param>
        /// <param name="ufEntidade">UF da entidade</param>
        /// <param name="tecnologiaDataCashDataAtivacao">Data de ativação Tecnologia Datacash</param>
        /// <param name="tipoTecnologia">Tipo de tecnologia</param>
        /// <param name="tipoTecnologiaDataCash">Tipo tecnologia datacash</param>
        /// <param name="codigoCanal">Código do canal</param>
        /// <param name="codigoCelula">Código da célula</param>
        /// <param name="recarga">Recarga</param>
        public void AcessarFilial(
            String nomeEntidade,
            Int32 codigoEntidade,
            Int32 tipoTecnologia,
            String cnpj,
            String nomeUsuario,
            String loginUsuario,
            String statusPV,
            String emailEntidade,
            String ufEntidade,
            String tipoTecnologiaDataCash,
            DateTime tecnologiaDataCashDataAtivacao,
            Int32 codigoGrupoRamo,
            Int32 codigoRamoAtividade,
            Int32 codigoCanal,
            Int32 codigoCelula,
            String recarga)
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
                _ufFilial = ufEntidade;
                _tecnologiaDataCashFilial = tipoTecnologiaDataCash;
                _tecnologiaDataCashDataAtivacaoFilial = tecnologiaDataCashDataAtivacao;
                _codigoRamoAtividadeFilial = codigoRamoAtividade;
                _codigoGrupoRamoFilial = codigoGrupoRamo;
                _codigoCanalFilial = codigoCanal;
                _codigoCelulaFilial = codigoCelula;
                _recargaFilial = recarga;
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
                _ufFilial = String.Empty;
                _tecnologiaDataCashFilial = String.Empty;
                _tecnologiaDataCashDataAtivacaoFilial = DateTime.MinValue;
                _codigoSegmento = Char.MinValue;
                _codigoGrupoRamoFilial = 0;
                _codigoRamoAtividadeFilial = 0;
                _codigoCanalFilial = 0;
                _codigoCelulaFilial = 0;
                _recargaFilial = String.Empty;
                _codigoMatrizFilial = 0;
            }
        }

        #endregion

        #endregion

    }
}
