using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Rede.PN.Eadquirencia.Sharepoint.SvcConfigCallback;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.ConfigurarCallback
{
    public partial class ConfigurarCallbackUserControl : WebpartBase
    {
        protected int NumeroPv
        {
            get { return SessaoAtual.CodigoEntidade; }
        }
		
		private List<TipoEvento> Eventos
        {
            get { return ViewState["EventosCallback"] as List<TipoEvento>; }
            set { ViewState["EventosCallback"] = value; }
        }

        /// <summary>
        /// Page_Load 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ExecucaoTratada("Page Load - configuracao callback", () =>
            {
                if (!IsPostBack)
                {
                    if (base.VerificarConfirmacaoPositia())
                    {
                        base.RedirecionarConfirmacaoPositiva();
                        return;
                    }

					Eventos = ListarEventos();
                    ListarUrls();
					
                }
            });
        }

        /// <summary>
        /// Exibe a div para incluir uma nova url.
        /// </summary>
        protected void lkbExibirDadosInclusao_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Exbir panel inclusão - configuracao callback", () =>
            {
                ExibirPanelIncluirUrl(true);
            });
        }

        /// <summary>
        /// Inclui a URL no sistema.
        /// </summary>
        protected void btnIncluirUrl_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Incluir URL - configuracao callback", () =>
            {
                UrlCallback url       = new UrlCallback();
                url.Eventos           = new List<TipoEvento>();
                url.Situacao          = 'A';
                url.Url               = txtUrl.Text;
                url.NumeroPV          = NumeroPv;
                url.NumeroFuncionalPv = !String.IsNullOrEmpty(SessaoAtual.Funcional) ? SessaoAtual.Funcional : SessaoAtual.CodigoEntidade.ToString();

                foreach (ListItem item in cblEventos.Items)
                {
                    if (item.Selected)
                        url.Eventos.Add(new TipoEvento { Codigo = Convert.ToInt32(item.Value) });
                }

                if (ValidarUrlInclusaoEdicao(url))
                {
                    using (var ctx = new ContextoWCF<ServicoConfigCallbackClient>())
                    {
                        ctx.Cliente.SalvarUrl(url);
                    }

                    base.ExibirPainelMensagem("Url cadastrada com sucesso.");
                    ExibirPanelIncluirUrl(false);
                    ListarUrls();
                }
            });
        }

        /// <summary>
        /// Esconde a div de inclusão de url.
        /// </summary>
        protected void btnCancelarInclusaoUrl_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Cancelar inclusão - configuracao callback", () =>
            {
                ExibirPanelIncluirUrl(false);
            });
        }

        /// <summary>
        /// Método que coloca o datalist em modo de edição.
        /// </summary>
        protected void dtlUrls_EditCommand(object source, DataListCommandEventArgs e)
        {
            ExecucaoTratada("Exbir dados edição - configuracao callback", () =>
            {
                dtlUrls.EditItemIndex = e.Item.ItemIndex;
                ListarUrls();
            });
        }

        /// <summary>
        /// Atualiza a url de negocio.
        /// </summary>
        protected void dtlUrls_UpdateCommand(object source, DataListCommandEventArgs e)
        {
            ExecucaoTratada("edição edição - configuracao callback", () =>
            {
                TextBox txt          = e.Item.FindControl("txtUrl") as TextBox;
                CheckBoxList cbk     = e.Item.FindControl("cblEventos") as CheckBoxList;

                UrlCallback url         = new UrlCallback();
                url.Eventos             = new List<TipoEvento>();
                url.Situacao            = 'A';
                url.Url                 = txt.Text;
                url.NumeroPV            = NumeroPv;
                url.NumeroFuncionalPv   = !String.IsNullOrEmpty(SessaoAtual.Funcional) ? SessaoAtual.Funcional : NumeroPv.ToString();
                url.NumeroSequencialUrl = Convert.ToInt32(dtlUrls.DataKeys[e.Item.ItemIndex]);

                foreach (ListItem item in cbk.Items)
                {
                    if (item.Selected)
                        url.Eventos.Add(new TipoEvento { Codigo = Convert.ToInt32(item.Value) });
                }

                if (ValidarUrlInclusaoEdicao(url))
                {
                    using (var ctx = new ContextoWCF<ServicoConfigCallbackClient>())
                    {
                        ctx.Cliente.SalvarUrl(url);
                    }

                    dtlUrls.SelectedIndex = -1;
                    dtlUrls.EditItemIndex = -1;
                    ListarUrls();
                    ExibirPainelMensagem("URL editada com sucesso.");
                }
            });
        }

        /// <summary>
        /// Método que exibe a popup de confirmação de exclusão.
        /// </summary>
        protected void dtlUrls_DeleteCommand(object source, DataListCommandEventArgs e)
        {
            ExecucaoTratada("Exibir popup confirmacao exclusão - configuracao callback", () =>
            {
                hdfNumeroSequecialUrlParaExclusao.Value = e.CommandArgument.ToString();
                divConfirmacao.Visible = true;
                StringBuilder sb = new StringBuilder();
                this.ExibirPainelHtml(sb.ToString());
            });
        }

        /// <summary>
        /// método que cancela a inclusão.
        /// </summary>
        protected void dtlUrls_CancelCommand(object source, DataListCommandEventArgs e)
        {
            ExecucaoTratada("Cancelar edição - configuracao callback", () =>
            {
                dtlUrls.SelectedIndex = -1;
                dtlUrls.EditItemIndex = -1;
                ListarUrls();
            });
        }

        /// <summary>
        /// Confirma a exclusão a url da lista.
        /// </summary>
        protected void btnConfirmarExclusaoSim_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Confirmar exclusão - configuracao callback", () =>
            {
                string numeroFuncionalPv = !String.IsNullOrEmpty(SessaoAtual.Funcional) ? SessaoAtual.Funcional : SessaoAtual.CodigoEntidade.ToString();

                using (var ctx = new ContextoWCF<ServicoConfigCallbackClient>())
                {
                    ctx.Cliente.ExcluirUrl(NumeroPv, Convert.ToInt32(hdfNumeroSequecialUrlParaExclusao.Value), Convert.ToInt32(numeroFuncionalPv));
                }

                dtlUrls.SelectedIndex = -1;
                dtlUrls.EditItemIndex = -1;
                divConfirmacao.Visible = false;
                ListarUrls();
                ExibirPainelMensagem("URL excluída com sucesso.");
            });
        }

        /// <summary>
        /// Cancela a exclusão.
        /// </summary>
        protected void btnConfirmarExclusaoNao_Click(object sender, EventArgs e)
        {
            ExecucaoTratada("Cancelar exclusão - configuracao callback", () =>
            {
                divConfirmacao.Visible = false;
            });
        }

        /// <summary>
        /// Evento auxiliar para preencher os checklist de eventos.
        /// </summary>
        protected void dtlUrls_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ExecucaoTratada("ItemDataBound edição - configuracao callback", () =>
            {
                CheckBoxList checklist = e.Item.FindControl("cblEventos") as CheckBoxList;
                UrlCallback url        = e.Item.DataItem as UrlCallback;

                if (checklist != null && url != null && dtlUrls.EditItemIndex >= 0)
                {
                    checklist.DataSource = ListarEventosDisponiveis(url);
                    checklist.DataBind();

                    foreach (TipoEvento evento in url.Eventos)
                    {
                        ListItem i = checklist.Items.FindByValue(evento.Codigo.ToString());
                        if (i != null)
                            i.Selected = true;
                    }
                }
            });
        }

        /// <summary>
        /// Exibe ou esconde a div para incluir o novo registro.
        /// </summary>
        /// <param name="exibir">True para incluir, false para esconder.</param>
        private void ExibirPanelIncluirUrl(bool exibir)
        {
            if (exibir)
            {
                dtlUrls.EditItemIndex    = -1;
                dtlUrls.SelectedIndex    = -1;
                ListarUrls();

                txtUrl.Text              = String.Empty;

                cblEventos.SelectedIndex = -1;
                cblEventos.DataSource    = ListarEventosDisponiveis(null);
                cblEventos.DataBind();
            }

            pnlBotaoAdicionar.Visible = !exibir;
            pnlDadosInclusao.Visible  = exibir;
        }

        /// <summary>
        /// Método de auxilio para converter a lista de Eventos para string separada por vírgula.
        /// </summary>
        /// <param name="o">Lista de evento.</param>
        /// <returns>eventos em forma de string: "Mpi, Refund, ..."</returns>
        protected static string ConverterEventosParaString(object o)
        {
            List<TipoEvento> lista = o as List<TipoEvento>;

            string eventosEmTexto = String.Empty;
            if (lista != null)
            {
                eventosEmTexto = string.Join(", ", lista.Select(x => x.Descricao));
            }

            return eventosEmTexto;
        }

        /// <summary>
        /// Carrega as urls na tela.
        /// </summary>
        private void ListarUrls()
        {
            Int32 numeroPv = NumeroPv;
            List<UrlCallback> urls = null;

            using (var ctx = new ContextoWCF<ServicoConfigCallbackClient>())
            {
                urls = ctx.Cliente.ListarUrls(numeroPv);
            }
			
			pnlBotaoAdicionar.Visible = urls == null || !Eventos.All(e => urls.SelectMany(u => u.Eventos).Any(ue => ue.Codigo == e.Codigo));

            dtlUrls.DataSource = urls;
            dtlUrls.DataBind();
        }

        /// <summary>
        /// Lista os eventos cadastrados na base.
        /// </summary>
        /// <returns>Lista de evento.</returns>
        private List<TipoEvento> ListarEventos()
        {
            List<TipoEvento> eventos = null;
            using (var ctx = new ContextoWCF<ServicoConfigCallbackClient>())
            {
                eventos = ctx.Cliente.ListarEventos();
            }

            return eventos;
        }

        /// <summary>
        /// Valida se a URL tá ok antes de enviar para inclusao/edição.
        /// Se a url está válida retorna true.
        /// </summary>
        /// <param name="urlNotificacao">Objeto URL que será validado.</param>
        private bool ValidarUrlInclusaoEdicao(UrlCallback urlNotificacao)
        {
            if (String.IsNullOrWhiteSpace(urlNotificacao.Url.Trim()))
            {
                ExibirPainelMensagem("Campo URL obrigatório.");
                return false;
            }

            if (urlNotificacao.Url.Length > 500)
            {
                ExibirPainelMensagem("O campo url excede o limite permitido (500 caracteres).");
                return false;
            }

            if (!Regex.IsMatch(urlNotificacao.Url, @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"))
            {
                ExibirPainelMensagem("O campo url está no formato inválido (Obrigatório: http ou https).");
                return false;
            }

            if (urlNotificacao.Eventos.Count == 0)
            {
                ExibirPainelMensagem("Ao menos um evento tem que ser selecionado.");
                return false;
            }

            return true;
        }
		
		/// <summary>
        /// Lista os eventos disponíveis para o cadastro da url.
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Lista contendo os eventos disponíveis</returns>
        private List<TipoEvento> ListarEventosDisponiveis(UrlCallback url)
        {
            var eventosDisponiveis = new List<TipoEvento>();
            eventosDisponiveis.AddRange(Eventos);

            var listaUrls = new List<UrlCallback>();
            var listaUrlsCadastradas = dtlUrls.DataSource as List<UrlCallback>;

            // Adicionando as urls já cadastradas
            if (listaUrlsCadastradas != null)
                listaUrls.AddRange(listaUrlsCadastradas);

            // Removendo a url editada
            // Na edição da url, deve aparecer os eventos disponíveis + o evento cadastrado para a URL
            if (url != null)
                listaUrls.RemoveAll(u => u.NumeroSequencialUrl == url.NumeroSequencialUrl);

            // Removendos os eventos que possuem uma url cadastrada
            eventosDisponiveis.RemoveAll(e => listaUrls.SelectMany(u => u.Eventos).Any(ue => ue.Codigo == e.Codigo));

            return eventosDisponiveis;
        }
    }
}