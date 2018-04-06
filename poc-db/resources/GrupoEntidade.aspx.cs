using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    public partial class GrupoEntidade : LayoutsPageBase
    {
        #region [ Propriedades da Página ]
        /// <summary>Entidades que foram alteradas</summary>
        private List<DadosEntidade> ListaDadosAlterados
        {
            get
            {
                if (ViewState["ListaDadosAlterados"] == null)
                    ViewState["ListaDadosAlterados"] = new List<DadosEntidade>();
                return (List<DadosEntidade>)ViewState["ListaDadosAlterados"];
            }
            set { ViewState["ListaDadosAlterados"] = value; }
        }
        #endregion


        #region [ Eventos da Página ]
        protected void Page_Load(object sender, EventArgs e)
        {
            //Carrega o Grid
            if (!IsPostBack)
                carregarGrid();
        }

        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAtualizar.Text.Equals("Atualizar"))
                {
                    //Atualiza todas as entidades
                    atualizarEntidades();

                    //Mosta mensagem de OK para o usuário
                    criarMensagem("Dados atualizados com sucesso..", false);

                }
                else
                {
                    insereGrupoEntidade();
                    //Mosta mensagem de OK para o usuário
                    criarMensagem("Grupo entidade salvo com sucesso..", false);
                    ModoTela(false);
                }
                //Recarrega o Grid
                carregarGrid();
            }
            catch (Exception ex)
            {
                //Mosta mensagem de erro para o usuário
                criarMensagem(string.Format("Erro ao atualizar dados: {0}", ex.Message), true);
                SharePointUlsLog.LogErro(ex);
            }
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!btnAtualizar.Text.Equals("Atualizar"))
            {
                ModoTela(false);
            }
            carregarGrid();
        }
        private void insereGrupoEntidade()
        {

            //Abre a conexão com o WCF e atualiza todos os itens alterados
            using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
            {
                EntidadeServico.GrupoEntidade item = new EntidadeServico.GrupoEntidade()
                {
                    Codigo = txtCodigo.Text.ToInt32(),
                    Ativo = chkAtivo.Checked,
                    Descricao = txtDescricao.Text,
                    NomeResponsavel = "IS"
                };
                //Insere os itens alterados
                client.InserirGrupo(item);
            }

        }
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ModoTela(true);

        }
        void ModoTela(bool edicao)
        {
            btnNovo.Visible = !edicao;
            pnlEdicao.Visible = edicao;
            pnlLista.Visible = !edicao;
            btnAtualizar.Text = edicao ? "Salvar" : "Atualizar";
        }
        /// <summary>Atualiza a lista de itens alterados</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chk_CheckedChanged(object sender, EventArgs e)
        {
            //Busca os dados da entidade atualizada
            CheckBox chk = ((CheckBox)sender);
            int codigo = Convert.ToInt32(chk.Attributes["codigo"]);

            //Verifica se a entidade já existe
            DadosEntidade entidade = ListaDadosAlterados.Find(i => i.CodigoEntidade == codigo);

            //Se existir atualiza, caso contrário, cria
            if (entidade != null)
                entidade.Ativo = chk.Checked;
            else
                ListaDadosAlterados.Add(new DadosEntidade() { CodigoEntidade = Convert.ToInt32(chk.Attributes["codigo"]), Ativo = chk.Checked });
        }
        #endregion

        #region [ Métodos Auxiliares ]
        /// <summary>Carrega os Grupos de Entidades</summary>
        private void carregarGrid()
        {
            try
            {
                //Carrega o Grid
                using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                {
                    int codigoErro = 0;

                    //Carrega o Grid
                    EntidadeServico.GrupoEntidade[] dados = client.ConsultarTodosGrupos(out codigoErro);
                    gdvGrupos.DataSource = dados;
                    gdvGrupos.DataBind();

                    //Verifica se houve erro
                    if (codigoErro > 0)
                        criarMensagem(string.Format("Erro ao buscar dados.. {0}", codigoErro.ToString()), true);
                }
            }
            catch (Exception ex)
            {
                //Mosta mensagem de erro para o usuário
                criarMensagem(string.Format("Erro ao buscar dados: {0}", ex.Message), true);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>Atualiza as entidades</summary>
        private void atualizarEntidades()
        {
            //Verifica se possui item a ser atualizado
            if (ListaDadosAlterados.Count == 0)
                return;

            //Abre a conexão com o WCF e atualiza todos os itens alterados
            using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
            {
                //Atualiza todos os itens alterados
                foreach (DadosEntidade item in ListaDadosAlterados)
                    client.AtualizarStatusGrupo(item.ConverterParaEntidade());
            }

            //Limpa a lista de entidades que devem ser atualizadas
            ListaDadosAlterados = null;
        }

        /// <summary>Emite um altera para o usuário</summary>
        /// <param name="mensagem">Mensagem para o alerta</param>
        private void criarMensagem(string mensagem, bool erro)
        {
            string script = "";

            if (erro)
                script = String.Format("window.setTimeout('emiteAlertaErro(\"{0}\");', 500);", mensagem);
            else
                script = String.Format("window.setTimeout('emiteAlerta(\"{0}\");', 500);", mensagem);

            //Registra o script na página
            //this.Page.ClientScript.RegisterStartupScript(typeof(string), "alerta", script, true);
            System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(string), "alerta", script, true);
        }
        #endregion

        #region [ Classes Auxiliares ]
        /// <summary>Dados da entidade alterada</summary>
        [Serializable]
        private class DadosEntidade
        {
            public int CodigoEntidade { get; set; }
            public bool Ativo { get; set; }

            /// <summary>Converte para a entidade o WCF</summary>
            /// <returns>Entidade WCF</returns>
            public EntidadeServico.GrupoEntidade ConverterParaEntidade()
            {
                return new EntidadeServico.GrupoEntidade { Codigo = CodigoEntidade, Ativo = Ativo };
            }
        }
        #endregion
    }
}
