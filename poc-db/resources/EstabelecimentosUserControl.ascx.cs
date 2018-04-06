/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Estabelecimentos
{
    /// <summary>
    /// Administração de Usuários - Criação/Edição de Usuário - Passo 2 - Estabelecimentos Permitidos
    /// </summary>
    public partial class EstabelecimentosUserControl : UsuariosUserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Lista de todos os Estabelecimentos do PV
        /// Armazena dados em cache (por PV)
        /// </summary>
        public List<Estabelecimento> Estabelecimentos
        {
            get
            {
                //Consulta todos os estabelecimentos associados ao PV da sessão (utilizando cache)
                String chave = String.Format("ConsultaEstabelecimentos_{0}", SessaoAtual.CodigoEntidade);
                List<Estabelecimento> pvs = null;
#if !DEBUG
                pvs = CacheAdmin.Recuperar<List<Estabelecimento>>(Comum.Cache.Filiais, chave);
#endif
                if (pvs == null)
                {
                    pvs = ConsultarEstabelecimentos(this.SessaoAtual.CodigoEntidade);
#if !DEBUG
                    CacheAdmin.Adicionar(Comum.Cache.Filiais, chave, pvs);
#endif
                }

                if (pvs != null)
                {
                    Estabelecimento proprio = pvs.FirstOrDefault(e => e.Pv == SessaoAtual.CodigoEntidade);
                    if (proprio != null)
                        pvs.Remove(proprio);

                    //Inclui o próprio
                    pvs.Insert(0, new Estabelecimento
                    {
                        Status = "A",
                        Nome = String.Format("{0} - {1}", SessaoAtual.CodigoEntidade, SessaoAtual.NomeEntidade),
                        Pv = SessaoAtual.CodigoEntidade,
                        Tipo = SessaoAtual.PVMatriz ? TipoEstabelecimento.Proprio : TipoEstabelecimento.Filiais
                    });

                    {
                        //Verifica se o e-mail foi alterado
                        String emailOriginal = this.UsuarioSelecionadoOriginal != null ?
                            this.UsuarioSelecionadoOriginal.Email : String.Empty;
                        Boolean emailAlterado = String.Compare(this.UsuarioSelecionado.Email, emailOriginal, true) != 0;

                        //Consulta os PVs associados ao e-mail do usuário, pois não pode existir
                        //mais de um usuário com o mesmo e-mail associado a um mesmo PV
                        List<Int32> pvsEmail = ConsultarEstabelecimentosEmail(this.UsuarioSelecionado.Email);

                        //Se e-mail não foi alterado, exclui, dos PVs associados ao e-mail, 
                        //os PVs do próprio usuário (quando em modo de edição)
                        if (!emailAlterado)
                            pvsEmail = pvsEmail.Except(this.UsuarioSelecionado.Estabelecimentos).ToList();

                        //Remove da coleção de todos os estabelecimentos associados ao PV da sessão
                        //os PVs que já estão associados ao mesmo e-mail (deve desconsiderar os PVs do e-mail do usuário atual)
                        if (pvsEmail != null && pvsEmail.Count > 0)
                        {
                            pvs.RemoveAll(pv => pvsEmail.Contains(pv.Pv));
                            this.PvsSelecionados.RemoveAll(pv => pvsEmail.Contains(pv));

                            if (!this.SessaoAtual.PVMatriz && this.PvsSelecionados.Count == 0)
                            {
                                base.ExibirPainelExcecao(this.ModoCriacao ?
                                    "UsuarioServico.InserirUsuario2" : "UsuarioServico.Atualizar2", 1);
                            }
                        }
                    }

                    {
                        //Verifica se o CPF foi alterado
                        Int64? cpfOriginal = this.UsuarioSelecionadoOriginal != null ?
                            this.UsuarioSelecionadoOriginal.Cpf : null;
                        Boolean cpfAlterado = this.UsuarioSelecionado.Cpf != cpfOriginal;

                        //Consulta os PVs associados ao CPF do usuário, pois não pode existir
                        //mais de um usuário com o mesmo CPF associado a um mesmo PV
                        List<Int32> pvsCpf = ConsultarEstabelecimentosCpf(this.UsuarioSelecionado.Cpf);

                        //Se CPF não foi alterado, excluir, dos PVs associados ao CPF,
                        //os PVs do próprio usuário (quando em modo de edição)
                        if (!cpfAlterado)
                            pvsCpf = pvsCpf.Except(this.UsuarioSelecionado.Estabelecimentos).ToList();

                        //Remove da coleção de todos os estabelecimentos associados ao PV da sessão
                        //os PVs que já estão associados ao mesmo CPF (deve desconsiderar os PVs do CPF do usuário atual)
                        if (pvsCpf != null && pvsCpf.Count > 0)
                        {
                            pvs.RemoveAll(pv => pvsCpf.Contains(pv.Pv));
                            this.PvsSelecionados.RemoveAll(pv => pvsCpf.Contains(pv));

                            if (!this.SessaoAtual.PVMatriz && this.PvsSelecionados.Count == 0)
                            {
                                base.ExibirPainelExcecao(this.ModoCriacao ?
                                    "UsuarioServico.InserirUsuario2" : "UsuarioServico.Atualizar2", 15);
                            }
                        }
                    }
                }

                return pvs;
            }
        }

        /// <summary>
        /// Lista todos os Estabelecimentos associados à Matriz do PV atual
        /// Armazena dados em cache (por PV)
        /// </summary>
        public List<Estabelecimento> EstabelecimentosMatriz
        {
            get
            {
                var pvs = new List<Estabelecimento>();

                if (SessaoAtual.CodigoMatriz != 0)
                {
                    //Consulta todos os estabelecimentos associados ao PV da sessão (utilizando cache)
                    String chave = String.Format("ConsultaEstabelecimentos_{0}", SessaoAtual.CodigoMatriz);
#if !DEBUG
                    pvs = CacheAdmin.Recuperar<List<Estabelecimento>>(Comum.Cache.Filiais, chave);
#endif

                    if (pvs == null || pvs.Count == 0)
                    {
                        pvs = ConsultarEstabelecimentos(SessaoAtual.CodigoMatriz);
#if !DEBUG
                        CacheAdmin.Adicionar(Comum.Cache.Filiais, chave, pvs);
#endif
                    }
                }
                //Se não possui Matriz, inclui o próprio apenas
                else
                    pvs.Insert(0, this.EstabelecimentoProprio);

                return pvs;
            }
        }

        /// <summary>
        /// Estabelecimento Próprio
        /// </summary>
        public Estabelecimento EstabelecimentoProprio
        {
            get
            {
                return new Estabelecimento
                {
                    Status = "A",
                    Nome = String.Format("{0} - {1}", SessaoAtual.CodigoEntidade, SessaoAtual.NomeEntidade),
                    Pv = SessaoAtual.CodigoEntidade,
                    Tipo = TipoEstabelecimento.Proprio,
                };
            }
        }

        /// <summary>
        /// Lista de PVs selecionados na tela
        /// </summary>
        public List<Int32> PvsSelecionados
        {
            get
            {
                if (ViewState["PvsSelecionados"] == null)
                    ViewState["PvsSelecionados"] = new List<Int32>();
                return (List<Int32>)ViewState["PvsSelecionados"];
            }
            set
            {
                ViewState["PvsSelecionados"] = value;
            }
        }

        /// <summary>
        /// Se for Filial editando usuário, controles devem vir
        /// no modo Somente Leitura
        /// </summary>
        private Boolean EdicaoSomenteLeitura
        {
            get { return this.ModoEdicao && !this.SessaoAtual.PVMatriz; }
        }

        /// <summary>
        /// Se for Filial criando usuário, o PV da Filial 
        /// deve ser o único e estar obrigatoriamente selecionado
        /// </summary>
        private Boolean CriacaoSomenteLeitura
        {
            get { return this.ModoCriacao && !this.SessaoAtual.PVMatriz; }
        }

        /// <summary>
        /// Se for Filial aprovando usuário, controles devem vir
        /// no modo Somente Leitura
        /// </summary>
        private Boolean AprovacaoSomenteLeitura
        {
            get { return this.ModoAprovacao && !this.SessaoAtual.PVMatriz; }
        }

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - Page_Load"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    if (!base.PossuiPermissao)
                    {
                        pnlAcessoNegado.Visible = true;
                        pnlDados.Visible = false;
                        return;
                    }

                    //Botão default da página
                    this.Page.Form.DefaultButton = btnBuscar.UniqueID;
                    //Manter posição da página
                    this.Page.MaintainScrollPositionOnPostBack = true;

                    if (!IsPostBack)
                    {
                        //Carregamento inicial dos dados
                        CarregarDadosUsuario();

                        if (this.ModoEdicao) {
                            divTabsEdicao.Visible = true;
                            btnContinuar.Text = "salvar";
                        }

                        //Estabelecimentos ficam em Modo exibição quando é Edição de Usuário ou Aprovação e 
                        //usuário está logado em uma Filial
                        if (this.EdicaoSomenteLeitura || this.CriacaoSomenteLeitura || this.AprovacaoSomenteLeitura)
                        {
                            mvwEstabelecimentos.SetActiveView(pnlExibicao);
                            CarregarEstabelecimentosUsuarioModoLeitura();

                            this.btnContinuar.ValidationGroup = String.Empty;
                        }
                        else
                        {
                            mvwEstabelecimentos.SetActiveView(pnlEdicao);
                            CarregarGridEstabelecimentos();
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Continua para o próximo passo
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - Continua para o próximo passo"))
            {
                try
                {
                    if (this.SalvarEstabelecimentosTemporariosUsuario())
                    {

                        // No modo de edicao, salva os itens e mantem na tela
                        if (this.ModoEdicao)
                        {
                            log.GravarMensagem("Modo edição");

                            Boolean emailAtualizado;
                            base.AlterarUsuario("Estabelecimentos", out emailAtualizado);

                            hdfFormModificado.Value = String.Empty;

                            //Abrir lightbox para indicar que foi atualizado
                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { estabelecimentosOpenModal('#lbxModalConfirmacao'); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                        }
                        else
                        {
                            //Redireciona para o próximo passo
                            base.RedirecionarPassoPermissoes();
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Busca dos estabelecimentos
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - btnBuscar_Click"))
            {
                try
                {
                    grvTodosEstabelecimentos.Marcados.Clear();
                    grvTodosEstabelecimentos.PageIndex = 0;
                    grvExibicao.PageIndex = 0;

                    if (this.EdicaoSomenteLeitura || this.CriacaoSomenteLeitura || this.AprovacaoSomenteLeitura)
                    {
                        CarregarEstabelecimentosUsuarioModoLeitura();
                    }
                    else
                    {
                        CarregarGridEstabelecimentos();
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Cancela e volta para a tela inicial de Usuários
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - btnCancelar_Click"))
            {
                try
                {
                    //Limpa variável de sessão que contém os dados do usuário em edição
                    base.Encerrar();

                    if (ModoAprovacao)
                        this.RedirecionarParaAprovacao();
                    else //Retorna para a tela principal de Usuários
                        this.RedirecionarParaUsuarios();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Tratamento linha a linha
        /// </summary>
        protected void grvTodosEstabelecimentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as Estabelecimento;
                String status = String.Empty;
                String permissao = "não";

                //Marca qual linha corresponde ao "Próprio", seja Matriz ou Filial
                if (item.Pv == this.SessaoAtual.CodigoEntidade)
                    e.Row.Attributes["proprio"] = "true";

                if (String.Compare(item.Status, "A", true) == 0)
                {
                    status = "ativo";
                }
                else if (String.Compare(item.Status, "C", true) == 0)
                {
                    status = "cancelado";
                }
                else
                {
                    status = item.Status;
                }

                //Se o pv estiver como selecionado na coluna permissão fica 'sim'
                if (this.PvsSelecionados.Exists(pv => pv.Equals(item.Pv)))
                {
                    permissao = "sim";
                }

                e.Row.Cells[2].Text = e.Row.Cells[2].Text.ToLower();
                e.Row.Cells[3].Text = status;
                e.Row.Cells[4].Text = permissao;
            }
        }

        /// <summary>
        /// Atualização da página selecionada na grid de estabelecimentos não selecionados
        /// </summary>
        protected void grvTodosEstabelecimentos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - grvTodosEstabelecimentos_PageIndexChanging"))
            {
                try
                {
                    grvTodosEstabelecimentos.PageIndex = e.NewPageIndex;
                    CarregarGridEstabelecimentos();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Tratamento linha a linha
        /// </summary>
        protected void grvExibicao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as Estabelecimento;
                String status = String.Empty;
                String permissao = "sim";

                if (String.Compare(item.Status, "A", true) == 0)
                {
                    status = "ativo";
                }
                else if (String.Compare(item.Status, "C", true) == 0)
                {
                    status = "cancelado";
                }
                else
                {
                    status = item.Status;
                }

                e.Row.Cells[1].Text = e.Row.Cells[1].Text.ToLower();
                e.Row.Cells[2].Text = status;
                e.Row.Cells[3].Text = permissao;
            }
        }

        /// <summary>
        /// Atualização da página selecionada na grid de estabelecimentos do usuário (modo exibição)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvExibicao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - grvExibicao_PageIndexChanging"))
            {
                try
                {
                    grvExibicao.PageIndex = e.NewPageIndex;
                    CarregarEstabelecimentosUsuarioModoLeitura();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Método implementado para extração das chaves dos items das grids.
        /// Utilizado para a MultiSelectGridView
        /// </summary>
        protected List<Object> grv_GetItemsKeyValue(Object sender, List<Object> items)
        {
            return items.Cast<Estabelecimento>()
                .Select(estabelecimento => estabelecimento.Pv)
                .Where(pv => pv != 0)
                .Cast<Object>().ToList();
        }

        /// <summary>
        /// Botao que desassocia os pvs do usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtnBloquear_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - lnkBtnBloquear_Click"))
            {
                try
                {
                    hdfFormModificado.Value = "true";

                    List<Int32> marcados = grvTodosEstabelecimentos.Marcados.Cast<Int32>().ToList();
                    grvTodosEstabelecimentos.Marcados.Clear();

                    if (marcados.Count > 0)
                    {
                        this.PvsSelecionados = this.PvsSelecionados.Except(marcados).ToList();
                    }

                    //Recarrega as grids com os estabelecimentos selecionados/não selecionados
                    CarregarGridEstabelecimentos();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Botao que associa os pvs do usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtnPermitir_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - lnkBtnPermitir_Click"))
            {
                try
                {
                    hdfFormModificado.Value = "true";

                    List<Int32> marcados = grvTodosEstabelecimentos.Marcados.Cast<Int32>().ToList();
                    grvTodosEstabelecimentos.Marcados.Clear();

                    if (marcados.Count > 0)
                    {
                        this.PvsSelecionados.AddRange(marcados);
                        this.PvsSelecionados = this.PvsSelecionados.Distinct().ToList(); // para remover itens duplicados
                    }

                    //Recarrega as grids com os estabelecimentos selecionados/não selecionados
                    CarregarGridEstabelecimentos();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Descarta as alterações e redireciona o usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDescartar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - btnDescartar_Click"))
            {
                try
                {
                    // Redireciona o usuario para a pagina da aba clicada
                    RedirecionarPassoAba();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Salva as informacoes no banco e redireciona o usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - btnAtualizar_Click"))
            {
                try
                {
                    //Armazena em sessão os dados digitados
                    if (this.SalvarEstabelecimentosTemporariosUsuario())
                    {
                        Boolean emailAtualizado;
                        base.AlterarUsuario("Estabelecimentos", out emailAtualizado);

                        // Redireciona o usuario para a pagina da aba clicada
                        RedirecionarPassoAba(); 
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Retorna para a tela de adm de usuarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetornarAdm_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("EstabelecimentosUserControl.ascx - btnRetornarAdm_Click"))
            {
                try
                {
                    //Limpa variável de sessão que contém os dados do usuário em edição
                    base.Encerrar();

                    //Retorna para a tela principal de Usuários
                    this.RedirecionarParaUsuarios();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Carrega os dados do usuário
        /// </summary>
        private void CarregarDadosUsuario()
        {
            if (ModoCriacao)
            {
                //Se é modo criação, por padrão, o usuário possui permissão para o PV da sessão
                //Exceção: se o PV da sessão já estiver associado a outro usuário de mesmo e-mail
                if (!this.ConsultarEstabelecimentosEmail(this.UsuarioSelecionado.Email)
                        .Any(pv => pv == this.SessaoAtual.CodigoEntidade))
                    this.UsuarioSelecionado.Estabelecimentos = new List<Int32>(new[] { this.SessaoAtual.CodigoEntidade });
            }

            if (ModoAprovacao)
            {
                pnlDadosUsuario.Visible = true;
                lblEmail.Text = this.UsuarioSelecionado.Email;
                lblNomeUsuario.Text = this.UsuarioSelecionado.Nome;
            }

            this.PvsSelecionados = this.UsuarioSelecionado.Estabelecimentos;

            ltrMatriz.Text = this.SessaoAtual.CodigoEntidade.ToString();
        }

        /// <summary>
        /// Atualiza grid com os PVs não selecionados
        /// </summary>
        private void CarregarGridEstabelecimentos()
        {
            this.ltrTotalEstabelecimentos.Visible = false;
            this.divRegistroNaoEncontrado.Visible = false;

            List<Estabelecimento> estabelecimentos = new List<Estabelecimento>();
            estabelecimentos = this.FiltrarEstabelecimentos(this.Estabelecimentos).ToList();

            //Verifica se PV já está selecionado
            //var estabelecimentoJaSelecionado = this.PvsSelecionados.Contains(txtEstabelecimento.Text.ToInt32(0));

            //if (estabelecimentoJaSelecionado)
            //{
            //    // indica se o pv ja esta selecionado
            //    this.ltrTotalEstabelecimentos.Text = "estabelecimento já selecionado - ";
            //    this.ltrTotalEstabelecimentos.Visible = true;
            //}

            if (estabelecimentos.Count == 0)
            {
                this.divRegistroNaoEncontrado.Visible = true;
            }
            //else
            //{
            //    this.ltrTotalEstabelecimentos.Text = String.Format("Total de {0} Estabelecimento{1}:",
            //        estabelecimentos.Count, estabelecimentos.Count == 1 ? String.Empty : "s");
            //}

            //Atualiza info de quantidade de estabelecimentos selecionados
            this.ltrEstabelecimentosSelecionados.Text = String.Format("{0} estabelecimento{1} com permissão",
                this.PvsSelecionados.Count, this.PvsSelecionados.Count == 1 ? String.Empty : "s");

            //Bind dos estabelecimentos
            grvTodosEstabelecimentos.DataSource = estabelecimentos;
            grvTodosEstabelecimentos.DataBind();
        }

        /// <summary>
        /// Atualiza grid com os Estabelecimentos do Usuário (modo Exibição)
        /// </summary>
        private void CarregarEstabelecimentosUsuarioModoLeitura()
        {
            var estabelecimentosMatriz = default(List<Estabelecimento>);

            if (this.EdicaoSomenteLeitura)
                estabelecimentosMatriz = this.FiltrarEstabelecimentos(this.EstabelecimentosMatriz)
                    .Where(pv => this.UsuarioSelecionado.Estabelecimentos.Contains(pv.Pv)).ToList();
            else if (this.CriacaoSomenteLeitura || this.AprovacaoSomenteLeitura)
                estabelecimentosMatriz = new[] { this.EstabelecimentoProprio }.ToList();

            //Atualiza info. de quantidade de estabelecimentos encontrados pelo Filtro
            if (estabelecimentosMatriz.Count == 0)
            {
                this.ltrTotalEstabelecimentosExibicao.Text = "<span class='validacoes'>estabelecimento não encontrado</span>";
            }
            else
            {
                this.ltrTotalEstabelecimentosExibicao.Text = String.Format("total de {0} estabelecimento{1}",
                    estabelecimentosMatriz.Count, estabelecimentosMatriz.Count == 1 ? String.Empty : "s");
            }

            grvExibicao.DataSource = estabelecimentosMatriz;
            grvExibicao.DataBind();
        }

        /// <summary>
        /// Retorna os estabelecimentos aplicando os filtros
        /// </summary>
        private List<Estabelecimento> FiltrarEstabelecimentos(List<Estabelecimento> estabelecimentos)
        {
            //Recupera os dados informados nos filtros
            Int32? numeroPv = txtEstabelecimento.Text.ToInt32Null();
            String situacao = ddlSituacao.SelectedValue;
            Boolean proprio = String.Compare(ddlTipoEstabelecimento.SelectedValue, "0") == 0 || String.Compare(ddlTipoEstabelecimento.SelectedValue, "1") == 0;
            Boolean filiais = String.Compare(ddlTipoEstabelecimento.SelectedValue, "0") == 0 || String.Compare(ddlTipoEstabelecimento.SelectedValue, "2") == 0;
            Boolean centralizados = String.Compare(ddlTipoEstabelecimento.SelectedValue, "0") == 0 || String.Compare(ddlTipoEstabelecimento.SelectedValue, "3") == 0;
            Boolean consignados = String.Compare(ddlTipoEstabelecimento.SelectedValue, "0") == 0 || String.Compare(ddlTipoEstabelecimento.SelectedValue, "4") == 0;
            Boolean mesmoCnpj = String.Compare(ddlTipoEstabelecimento.SelectedValue, "0") == 0 || String.Compare(ddlTipoEstabelecimento.SelectedValue, "5") == 0;

            //A partir da lista de estabelecimentos, aplica filtros
            return estabelecimentos.Where(pv =>
                (!numeroPv.HasValue || numeroPv.Value == pv.Pv) &&
                (String.Compare("T", situacao, true) == 0 || String.Compare(situacao, pv.Status, true) == 0) &&
                (centralizados || pv.Tipo != TipoEstabelecimento.Centralizados) &&
                (consignados || pv.Tipo != TipoEstabelecimento.Consignados) &&
                (filiais || pv.Tipo != TipoEstabelecimento.Filiais) &&
                (mesmoCnpj || pv.Tipo != TipoEstabelecimento.MesmoCnpj) &&
                (proprio || pv.Tipo != TipoEstabelecimento.Proprio)).ToList();
        }

        /// <summary>
        /// Retorna os estabelecimentos concatenados formato CSV
        /// </summary>
        /// <param name="estabelecimentosSelecionados">Listagem de estabelecimentos</param>
        /// <returns>
        /// <para>Estabelecimentos elementos do conjunto separados por ';'</para>
        /// <para>Nome e Número do Estabelecimento separados por ','</para>
        /// </returns>
        private String ObterEstabelecimentosConcatenados(List<Estabelecimento> estabelecimentosSelecionados)
        {
            String estabelecimentos = String.Empty;
            StringBuilder stb = new StringBuilder();

            foreach (Estabelecimento pv in estabelecimentosSelecionados)
            {
                stb.Append(String.Format("{0},{1};", pv.Pv.ToString(), pv.Nome.Trim()));
            }

            if (stb.Length > 0)
                estabelecimentos = stb.ToString();

            return estabelecimentos;
        }

        /// <summary>
        /// Redireciona o usuario para a tela da aba que foi clicada
        /// </summary>
        private void RedirecionarPassoAba()
        {
            String abaClicada = hdfAbaClicada.Value;
            if (String.Compare(abaClicada, "dadosCadastroTab", true) == 0)
            {
                base.RedirecionarPassoDadosCadastro();
            }
            else if (String.Compare(abaClicada, "permissoesTab", true) == 0)
            {
                base.RedirecionarPassoPermissoes();
            }
        }
        #endregion

        #region [ Consultas ]

        /// <summary>
        /// Consulta os estabelecimentos do PV (filiais, consignados, mesmo CNPJ, etc...)
        /// </summary>
        private List<Estabelecimento> ConsultarEstabelecimentos(Int32 codigoEntidade)
        {
            using (Logger log = Logger.IniciarLog("Consulta estabelecimentos do PV"))
            {
                var estabelecimentos = new List<Estabelecimento>();

                Int32 codigoRetorno;
                foreach (TipoEstabelecimento tipoAssociacao in Enum.GetValues(typeof(TipoEstabelecimento)))
                {
                    if (tipoAssociacao != TipoEstabelecimento.Proprio)
                    {
                        try
                        {
                            var consulta = default(List<Filial>);

                            //Chama o serviço entidade, buscando todos os estabelecimentos de acordo com o tipo de associação
                            using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                                consulta = ctx.Cliente.ConsultarFiliais(out codigoRetorno,
                                    SessaoAtual.CodigoEntidade, (Int32)tipoAssociacao).ToList();

                            if (codigoRetorno != 0)
                            {
                                log.GravarMensagem("Retorno EntidadeServico.ConsultarFiliais",
                                    new { SessaoAtual.CodigoEntidade, tipoAssociacao, codigoRetorno });
                            }
                            else if (consulta != default(List<Filial>) && consulta.Count > 0)
                            {
                                //Converte lista de Filiais em lista de Estabelecimentos
                                estabelecimentos.AddRange(consulta.Select(estabelecimento => new Estabelecimento
                                {
                                    //Se categoria "X" ou "E", é "Cancelado" ("C"). Caso contrário, "Ativo" ("A")
                                    Status = String.Compare("X", estabelecimento.Categoria, true) == 0
                                        || String.Compare("E", estabelecimento.Categoria, true) == 0 ? "C" : "A",
                                    Nome = String.Format("{0} - {1}", estabelecimento.PontoVenda, estabelecimento.NomeComerc),
                                    Pv = estabelecimento.PontoVenda,
                                    Tipo = tipoAssociacao
                                }).ToArray());
                            }
                        }
                        catch (FaultException<UsuarioServico.GeneralFault> ex)
                        {
                            log.GravarErro(ex);
                            base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                        }
                        catch (Exception ex)
                        {
                            log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                            base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                        }
                    }
                    else
                    {
                        using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                        {
                            Entidade entidade = ctx.Cliente.ConsultarDadosPV(out codigoRetorno, codigoEntidade);
                            if (codigoRetorno != 0)
                            {
                                log.GravarMensagem("Retorno EntidadeServico.ConsultarFiliais",
                                    new { SessaoAtual.CodigoEntidade, tipoAssociacao, codigoRetorno });
                            }
                            else if (entidade != null)
                            {
                                estabelecimentos.Add(new Estabelecimento
                                {
                                    //Se categoria "X" ou "E", é "Cancelado" ("C"). Caso contrário, "Ativo" ("A")
                                    Status = String.Compare("X", entidade.Status, true) == 0
                                        || String.Compare("E", entidade.Status, true) == 0 ? "C" : "A",
                                    Nome = String.Format("{0} - {1}", entidade.Codigo, entidade.NomeEntidade),
                                    Pv = entidade.Codigo,
                                    Tipo = tipoAssociacao
                                });
                            }
                        }
                    }
                }

                //Garante apenas um registro do mesmo PV
                estabelecimentos = estabelecimentos.GroupBy(e => e.Pv).Select(grupo => grupo.First()).OrderBy(e => e.Pv).ToList();

                //Garante que o PV atual é do tipo Próprio
                estabelecimentos.Where(e => e.Pv == SessaoAtual.CodigoEntidade).ToList().ForEach((e) =>
                {
                    e.Tipo = TipoEstabelecimento.Proprio;
                });

                return estabelecimentos;
            }
        }

        /// <summary>
        /// Consulta todos os estabelecimentos associados ao e-mail
        /// </summary>
        /// <param name="email">E-mail</param>
        private List<Int32> ConsultarEstabelecimentosEmail(String email)
        {
            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail"))
            {
                var codigoRetorno = default(Int32);
                var pvs = new List<Int32>();

                try
                {
                    var entidades = new Entidade1[0];

                    using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                        entidades = ctx.Cliente.ConsultarPorEmail(out codigoRetorno, email);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);

                    if (entidades != null && entidades.Length > 0)
                        pvs = entidades.Select(entidade => entidade.Codigo).ToList();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return pvs;
            }
        }

        /// <summary>
        /// Consulta todos os estabelecimentos associados ao CPF
        /// </summary>
        /// <param name="cpf">CPF</param>
        private List<Int32> ConsultarEstabelecimentosCpf(Int64? cpf)
        {
            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por CPF"))
            {
                var codigoRetorno = default(Int32);
                var pvs = new List<Int32>();

                if (cpf.HasValue)
                {
                    try
                    {
                        var entidades = new Entidade1[0];
                        using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                            entidades = ctx.Cliente.ConsultarPorCpf(out codigoRetorno, cpf.Value);

                        if (codigoRetorno != 0)
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarPorCpf", codigoRetorno);

                        if (entidades != null && entidades.Length > 0)
                            pvs = entidades.Select(entidade => entidade.Codigo).ToList();
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }

                return pvs;
            }
        }

        #endregion

        /// <summary>
        /// Armazena os dados temporários do usuário em sessão
        /// </summary>
        private Boolean SalvarEstabelecimentosTemporariosUsuario()
        {
            //Garante que existem PVs selecionados
            if (this.PvsSelecionados.Count <= 0)
                return false;

            //Garante que PVs sejam válidos e não estejam repetidos
            this.PvsSelecionados = this.PvsSelecionados.Distinct().Where(pv => pv != 0).ToList();
            this.UsuarioSelecionado.Estabelecimentos = this.PvsSelecionados;

            //Validar se os Estabelecimentos selecionados existem no PN
            List<Estabelecimento> estabelecimentosSelecionados = this.Estabelecimentos.Where(pv => PvsSelecionados.Contains(pv.Pv)).ToList();
            String estabelecimentosContatenados = string.Join("|", estabelecimentosSelecionados.Select(est => est.Pv).ToList());

            int codigoRetorno = 0;

            using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                codigoRetorno = contexto.Cliente.ValidarPvsExistentes(estabelecimentosContatenados);

            if (codigoRetorno != 0)
            {
                base.ExibirPainelExcecao("EntidadeServico.ValidarPvsExistentes", codigoRetorno);
                return false;
            }

            return true;
        }
    }
}