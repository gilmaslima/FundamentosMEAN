using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Sharepoint.Servicos;
using System.Collections.Generic;
using System.Linq;


namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class DadosOperacionais : UserControlCredenciamentoBase
    {

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Carregar valores dos campos iniciais
        /// </summary>
        public void CarregarCamposIniciais()
        {
            using (var log = Logger.IniciarLog("Dados Operacionais - Page Load"))
            {
                try
                {


                    ((ResumoProposta)resumoProposta).CarregaResumoProposta();

                    //Carregar DropDown Gerencias e setar Valor
                    CarregaGerencias();
                    ddlGerencia.SelectedValue = Credenciamento.Proposta.CodigoGerencia != null && !Char.IsWhiteSpace((Char)Credenciamento.Proposta.CodigoGerencia) ? Credenciamento.Proposta.CodigoGerencia.ToString() : "V";

                    //Carregar DropDown Carteiras utilizando valor do ddlGerencia
                    CarregaCarteiras();

                    //Carregar Regra Tipo Estabelecimento (Preenche valor campo estabelecimento)
                    RegraCampoEstabelecimento();
                    RegraCampoParcelamentoIATA();

                    // Carrega dados já salvos
                    txtNomeFatura.Text = !String.IsNullOrEmpty(Credenciamento.Proposta.NomeFatura) ? Credenciamento.Proposta.NomeFatura.Trim() : txtNomeFatura.Text;
                    txtNroGrupoGerencial.Text = Credenciamento.Proposta.NumeroGrupoGerencial != null && Credenciamento.Proposta.NumeroGrupoGerencial != 0 ? Credenciamento.Proposta.NumeroGrupoGerencial.ToString() : txtNroGrupoGerencial.Text;
                    txtNroGrupoComercial.Text = Credenciamento.Proposta.NumeroGrupoComercial != null && Credenciamento.Proposta.NumeroGrupoComercial != 0 ? Credenciamento.Proposta.NumeroGrupoComercial.ToString() : txtNroGrupoComercial.Text;
                    //txtDataAssProposta.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txtCPFVendedor.Text = Credenciamento.Proposta.NumeroCPFVendedor != null && Credenciamento.Proposta.NumeroCPFVendedor != 0 ? String.Format(@"{0:000\.000\.000\-00}", Credenciamento.Proposta.NumeroCPFVendedor) : txtCPFVendedor.Text;
                    ddlCarteira.SelectedValue = Credenciamento.Proposta.CodigoCarteira != null ? Credenciamento.Proposta.CodigoCarteira.ToString() : ddlCarteira.SelectedValue;

                    //Regra do valor CodigoCanal
                    RegraValorCanal();

                    //Regra do BNDES
                    RegraBNDES();

                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);

                }
            }
        }

        /// <summary>
        /// Verifica se "Grupo Ramo" e "Ramo Atividade" pertence ao BNDES, se sim efetua controle de tela específico
        /// </summary>
        private void RegraBNDES()
        {
            if (Credenciamento.Proposta.CodigoGrupoRamo == 8 && Credenciamento.Proposta.CodigoRamoAtividade == 105)
            {
                int grupoGerencial = ServicosGE.RetornaGrupoGerencialBNDES();
                txtNroGrupoGerencial.Text = grupoGerencial.ToString();
                txtNroGrupoGerencial.Enabled = false;
            }
            else
            {
                txtNroGrupoGerencial.Text = String.Empty;
                txtNroGrupoGerencial.Enabled = true;
            }
        }

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de Carteiras e preenche o dropdown
        /// </summary>
        private void CarregaCarteiras()
        {

            char codigoGerencia = ddlGerencia.SelectedValue.ToCharArray()[0];

            ddlCarteira.Items.Clear();

            ServicosGE.ConsultaListaCarteiras(codigoGerencia, null, string.Empty).ForEach(c =>
            {
                ddlCarteira.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodCarteira, c.NomeCarteira), c.CodCarteira.ToString()));
            });
        }

        /// <summary>
        /// Busca lista de Gerências e preenche o dropdown
        /// </summary>
        private void CarregaGerencias()
        {
            ServicosGE.ConsultaListaGerencia().ForEach(c =>
            {
                ddlGerencia.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodGerencia, c.NomeGerencia), c.CodGerencia.ToString()));
            });
        }

        /// <summary>
        /// Regra de tela Campo Estabelecimento
        /// </summary>
        private void RegraCampoEstabelecimento()
        {
            if (Credenciamento.Proposta.CodigoTipoPessoa == 'F')
                ddlTipoEstabelecimento.SelectedValue = "0"; //Física
            else
                if (Credenciamento.Proposta.CodigoTipoEstabelecimento == null)
                    ddlTipoEstabelecimento.SelectedValue = "2"; //Matriz
                else
                    ddlTipoEstabelecimento.SelectedValue = Credenciamento.Proposta.CodigoTipoEstabelecimento.ToString(); //Filial
        }

        /// <summary>
        /// Regra de tela baseada no valor Código Canal
        /// </summary>
        private void RegraValorCanal()
        {
            if (Credenciamento.Proposta.CodigoCanal == 4)
                ltlCPFVendedor.Text = "CPF do Promotor*:";
        }

        /// <summary>
        /// Regra de tela do controle ddlParcelamentoIATA baseada no campo "CodigoEquipamento" e "IndicadorIATA"
        /// </summary>
        private void RegraCampoParcelamentoIATA()
        {
            Boolean isBndes = Credenciamento.Proposta.CodigoGrupoRamo == 8 && Credenciamento.Proposta.CodigoRamoAtividade == 105;

            var tecnologia = ServicosWF.CarregarDadosTecnologia(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf, Credenciamento.Proposta.IndicadorSequenciaProposta);

            //Regra para Código Equipamento == TOL, SNT ou TOF
            if (String.Compare(tecnologia.CodTipoEquipamento, "TOL") == 0 ||
                String.Compare(tecnologia.CodTipoEquipamento, "SNT") == 0 ||
                String.Compare(tecnologia.CodTipoEquipamento, "TOF") == 0 && !isBndes)
            {
                ddlParcelamentoIATA.SelectedValue = "Sim";
                Credenciamento.Proposta.IndicadorIATA = 'S';
                ddlParcelamentoIATA.Enabled = false;
            }
            else
            {
                //Regra para IndicadorIATA == S
                if (Credenciamento.Proposta.IndicadorIATA != null && Credenciamento.Proposta.IndicadorIATA == 'S')
                    ddlParcelamentoIATA.Enabled = true;
                else
                {
                    ddlParcelamentoIATA.Enabled = false;
                    ddlParcelamentoIATA.SelectedValue = "Não";
                }
            }

        }

        #endregion


        #region [ EVENTOS CONTROLES ]

        /// <summary>
        /// Carrega dropdown de Carteiras ao selecionar uma Gerência
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlGerencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregaCarteiras();
        }

        /// <summary>
        /// Valida Nro do Grupo Comercial
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvNroGrupoComercial_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            GEGrpComGer.CodErroDescricaoErro retorno = ServicosGE.ValidaGrupoComercial(txtNroGrupoComercial.Text.ToInt32());
            if (retorno.CodErro == 0)
                args.IsValid = true;
        }

        /// <summary>
        /// Valida Nro do Grupo Gerencial
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvNroGrupoGerencial_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            GEGrpComGer.CodErroDescricaoErro retorno = ServicosGE.ValidaGrupoGerencial(txtNroGrupoGerencial.Text.ToInt32());
            if (retorno.CodErro == 0)
                args.IsValid = true;
        }

        /// <summary>
        /// Validar CPF
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCPFVendedor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCPFVendedor.Text.Replace(".", "").Replace("-", "").IsValidCPF();
        }

        /// <summary>
        /// Evento de salvar dados e parar tela neste formulário
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Operacionais - Salvar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        SalvarDados();
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                finally
                {
                    Credenciamento = null;
                    NovaProposta(sender, e);
                }
            }

        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Método de salvar dados no BD
        /// </summary>
        private void SalvarDados()
        {
            CarregarDadosTelaParaViewState();
            ServicosWF.SalvarDadosOperacionais(Credenciamento.Proposta, Credenciamento.Tecnologia);
        }

        /// <summary>
        /// Carrega dados da tela para objeto na viewState
        /// </summary>
        private void CarregarDadosTelaParaViewState()
        {
            Credenciamento.Proposta.NomeFatura = txtNomeFatura.Text.RemoverAcentos().RemoverCaracteresEspeciais().DeleteExtraWhitespaces().ToUpper();
            Credenciamento.Proposta.NumeroGrupoComercial = txtNroGrupoComercial.Text.ToInt32();
            Credenciamento.Proposta.NumeroGrupoGerencial = txtNroGrupoGerencial.Text.ToInt32();
            Credenciamento.Proposta.DataCadastroProposta = DateTime.Now;
            Credenciamento.Proposta.NumeroCPFVendedor = !String.IsNullOrEmpty(txtCPFVendedor.Text) ? Int64.Parse(txtCPFVendedor.Text.Replace("-", "").Replace(".", "")) : 0;
            Credenciamento.Proposta.CodigoGerencia = ddlGerencia.SelectedValue.ToCharArray()[0];
            Credenciamento.Proposta.CodigoCarteira = ddlCarteira.SelectedValue.ToInt32();
            Credenciamento.Proposta.CodigoTipoEstabelecimento = ddlTipoEstabelecimento.SelectedValue.ToInt32();

            if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 5)
                Credenciamento.Proposta.CodigoFaseFiliacao = 5;
        }

        public event EventHandler Salvar;

        /// <summary>
        /// Salvar e continuar para próximo formulário
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Operacionais - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        SalvarDados();
                        Continuar(sender, e);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        public event EventHandler Continuar;

        /// <summary>
        /// Retorna para formulário anterior
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Operacionais - Voltar"))
            {
                try
                {
                    Voltar(sender, e);
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        public event EventHandler Voltar;

        #endregion
    }
}
