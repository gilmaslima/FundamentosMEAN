#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Web.UI;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Microsoft.SharePoint.Utilities;
using System.Web;

using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;


namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.SolicitarMaterialVendas
{
    /// <summary>
    /// Web part de Solicitação de Material de Venda
    /// </summary>
    public partial class SolicitarMaterialVendasUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        private const String NomeServico = "Redecard.PN.OutrosServicos.Servicos.MaterialVendaServico";

        /// <summary>
        /// 
        /// </summary>
        private const String NomeServicoIncluir = "MaterialVenda.IncluirKit";

        /// <summary>
        /// 
        /// </summary>
        private Int32 _QuantidadeMaximaKitsApoio = -1;

        /// <summary>
        /// Flag para indicar se houve erro durante a execução da página
        /// </summary>
        private Boolean _indicaErro = false;

        /// <summary>
        /// Exibir painel de erro no controle
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="Codigo"></param>
        private void ExibirErro(String fonte, Int32 Codigo)
        {
            if (!_indicaErro)
            {
                _indicaErro = true;
                base.ExibirPainelExcecao(fonte, Codigo);
            }
        }

        /// <summary>
        /// Incluir nova solicitação de material
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IncluirSolicitacao(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Incluindo nova solicitação de material"))
            {
                try
                {
                    MaterialVendaServico.Endereco endereco = null;
                    if (chkEnderecoTemporario.Checked)
                    {
                        endereco = new MaterialVendaServico.Endereco()
                        {
                            Bairro = txtBairro.Text,
                            CEP = txtCEP.Text.Replace("-", String.Empty),
                            Cidade = txtCidade.Text,
                            Complemento = txtComplemento.Text,
                            Contato = txtContato.Text,
                            DDDFax = txtDDDFax.Text,
                            DDDTelefone = txtDDDTelefone.Text,
                            DescricaoEndereco = txtEndereco.Text,
                            Email = txtEmail.Text,
                            Fax = txtFax.Text,
                            Numero = Int32.Parse(txtNumero.Text),
                            Ramal = txtRamal.Text,
                            Site = txtSite.Text,
                            Telefone = txtTelefone.Text,
                            UF = ddlEstados.SelectedValue
                        };
                    }

                    List<MaterialVendaServico.Kit> kits = new List<MaterialVendaServico.Kit>();
                    // recuperar materiais de venda
                    this.RecuperarItemMateriaisVendas(kits);
                    // recuperar materiais de sinalização
                    this.RecuperarItemMateriaisSinalizacao(kits);
                    // recuperar materiais de apoio
                    this.RecuperarItemsMateriaisApoio(kits);
                    // recuperar materiais de tecnologia
                    this.RecuperarItemsMateriaisTecnologia(kits);

                    Int32 codigoRetorno;
                    using (var _client = new MaterialVendaServico.MaterialVendaServicoClient())
                    {
                        codigoRetorno = _client.IncluirKit(kits.ToArray(), this.SessaoAtual.CodigoEntidade, this.SessaoAtual.NomeEntidade, this.SessaoAtual.LoginUsuario, this.SessaoAtual.NomeUsuario, chkEnderecoTemporario.Checked, endereco);
                        if (codigoRetorno == 0)
                        {
                            Panel[] paineis = new Panel[1]{
                            pnlSolicitacaoMaterial
                        };

                            //pnlSucesso.Visible = true;
                            base.ExibirPainelConfirmacaoAcao("Concluído com Sucesso", "O Prazo para análise desta solicitação é de 2 dias úteis.<br /><br />Solicitação(ões) incluída(s) com sucesso.", SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
                            pnlSolicitacaoMaterial.Visible = false;

                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Solicitação de Material de Venda");
                        }
                        else
                        {
                            //pnlSucesso.Visible = false; ;
                            pnlSolicitacaoMaterial.Visible = true;
                            this.ExibirErro(NomeServicoIncluir, codigoRetorno);
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Recuperar items marcados de materiais de tecnologia
        /// </summary>
        /// <param name="kits"></param>
        private void RecuperarItemsMateriaisTecnologia(List<MaterialVendaServico.Kit> kits)
        {
            foreach (RepeaterItem item in rptKitsApoio.Items)
            {
                CheckBox chkTecno = item.FindControl("chkTecno") as CheckBox;
                HiddenField hdfTecno = item.FindControl("hdfTecno") as HiddenField;
                TextBox txtQuantidadeItensTecnologia = item.FindControl("txtQuantidadeItensTecnologia") as TextBox;
                DropDownList cboMotivoTecno = item.FindControl("cboMotivoTecno") as DropDownList;

                if (!object.ReferenceEquals(chkTecno, null))
                {
                    if (chkTecno.Checked)
                    {
                        kits.Add(new MaterialVendaServico.Kit()
                        {
                            CodigoKit = Int32.Parse(hdfTecno.Value),
                            Quantidade = Int32.Parse(txtQuantidadeItensTecnologia.Text),
                            Motivo = new MaterialVendaServico.Motivo()
                            {
                                CodigoMotivo = Int32.Parse(cboMotivoTecno.Text)
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Recuperar items marcados de materiais de apoio
        /// </summary>
        /// <param name="kits"></param>
        private void RecuperarItemsMateriaisApoio(List<MaterialVendaServico.Kit> kits)
        {
            foreach (RepeaterItem item in rptKitsApoio.Items)
            {
                CheckBox chkKitsApoio = item.FindControl("chkKitsApoio") as CheckBox;
                HiddenField hdfKitsApoio = item.FindControl("hdfKitsApoio") as HiddenField;
                if (!object.ReferenceEquals(chkKitsApoio, null))
                {
                    if (chkKitsApoio.Checked)
                    {
                        kits.Add(new MaterialVendaServico.Kit()
                        {
                            CodigoKit = Int32.Parse(hdfKitsApoio.Value),
                            Quantidade = 0,
                            Motivo = new MaterialVendaServico.Motivo()
                            {
                                CodigoMotivo = 0
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Recuperar items marcados de materiais de sinalização
        /// </summary>
        /// <param name="kits"></param>
        private void RecuperarItemMateriaisSinalizacao(List<MaterialVendaServico.Kit> kits)
        {
            foreach (RepeaterItem item in rptKitsSinalizacao.Items)
            {
                CheckBox chkKitSinalizacao = item.FindControl("chkKitSinalizacao") as CheckBox;
                HiddenField hdfKitSinalizacao = item.FindControl("hdfKitSinalizacao") as HiddenField;
                if (!object.ReferenceEquals(chkKitSinalizacao, null))
                {
                    if (chkKitSinalizacao.Checked)
                    {
                        kits.Add(new MaterialVendaServico.Kit()
                        {
                            CodigoKit = Int32.Parse(hdfKitSinalizacao.Value),
                            Quantidade = 0,
                            Motivo = new MaterialVendaServico.Motivo()
                            {
                                CodigoMotivo = 0
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Recuperar items marcados de materiais de vendas
        /// </summary>
        /// <param name="kits"></param>
        private void RecuperarItemMateriaisVendas(List<MaterialVendaServico.Kit> kits)
        {
            foreach (RepeaterItem item in rptMateriaisVendas.Items)
            {
                CheckBox chkVendas = item.FindControl("chkVendas") as CheckBox;
                HiddenField hdfVendas = item.FindControl("hdfVendas") as HiddenField;
                DropDownList cboMotivo = item.FindControl("cboMotivo") as DropDownList;
                if (!object.ReferenceEquals(chkVendas, null))
                {
                    if (chkVendas.Checked)
                    {
                        kits.Add(new MaterialVendaServico.Kit()
                        {
                            CodigoKit = Int32.Parse(hdfVendas.Value),
                            Quantidade = 0,
                            Motivo = new MaterialVendaServico.Motivo()
                            {
                                CodigoMotivo = Int32.Parse(cboMotivo.SelectedValue)
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Quantidade máxima de itens de apoio que podem ser solicitadas em um requisição de material
        /// </summary>
        public Int32 QuantidadeMaximaKitsApoio
        {
            get
            {
                using (Logger Log = Logger.IniciarLog("Quantidade máxima de itens de apoio"))
                {
                    if (_QuantidadeMaximaKitsApoio < 0 && !_indicaErro)
                    {
                        try
                        {
                            Int32 codigoRetorno;
                            using (var _client = new MaterialVendaServico.MaterialVendaServicoClient())
                            {
                                String valor = _client.ConsultarQuantidadeMaximaKitsApoio(out codigoRetorno);

                                if (String.IsNullOrEmpty(valor))
                                    valor = "0";

                                if (codigoRetorno == 0)
                                {
                                    _QuantidadeMaximaKitsApoio = Int32.Parse(valor);
                                }
                                else
                                {
                                    this.ExibirErro(NomeServico, codigoRetorno);
                                }
                            }
                        }
                        catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                        {
                            Log.GravarErro(ex);
                            this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                            this.ExibirErro(FONTE, CODIGO_ERRO);
                        }
                    }
                    return _QuantidadeMaximaKitsApoio;
                }
            }
        }

        private Int32 _QuantidadeMaximaKitsSinalizacao = -1;

        /// <summary>
        /// Quantidade máxima de itens de sinalização que podem ser solicitadas em um requisição de material
        /// </summary>
        public Int32 QuantidadeMaximaKitsSinalizacao
        {
            get
            {
                using (Logger Log = Logger.IniciarLog("Quantidade máxima de itens de sinalização"))
                {
                    if (_QuantidadeMaximaKitsSinalizacao < 0 && !_indicaErro)
                    {
                        try
                        {
                            Int32 codigoRetorno;
                            using (var _client = new MaterialVendaServico.MaterialVendaServicoClient())
                            {
                                String valor = _client.ConsultarQuantidadeMaximaKitsSinalizacao(out codigoRetorno);

                                if (String.IsNullOrEmpty(valor))
                                    valor = "0";

                                if (codigoRetorno == 0)
                                {
                                    _QuantidadeMaximaKitsSinalizacao = Int32.Parse(valor);
                                }
                                else
                                {
                                    this.ExibirErro(NomeServico, codigoRetorno);
                                }
                            }
                        }
                        catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                        {
                            Log.GravarErro(ex);
                            this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                            this.ExibirErro(FONTE, CODIGO_ERRO);
                        }
                    }
                    return _QuantidadeMaximaKitsSinalizacao;
                }
            }
        }

        /// <summary>
        /// Carregamento da página, realizar buscas de Remessas enviadas e Remessas baixadas.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && Util.UsuarioLogadoFBA())
            {
                this.CarregarRemessasBaixadas();
                this.CarregarRemessasAbertas();
                this.CarregarMateriaisVenda();
                this.CarregarKitsSinalizacao();
                this.CarregarKitsApoio();
                this.CarregarKitsTecnologia();
                // carregar listas de estados
                this.CarregarEstados();

                // carregar quantidade maxima de kits
                qtdKitsApoio.Text = this.QuantidadeMaximaKitsApoio.ToString();
                qtdKitsSinalizacao.Text = this.QuantidadeMaximaKitsSinalizacao.ToString();

                // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                if (SessaoAtual.UsuarioAtendimento)
                {
                    btnEnviar.Visible = false;
                }
            }
        }

        /// <summary>
        /// Carrega a lista de estados para o controle no endereço temporário
        /// </summary>
        protected void CarregarEstados()
        {
            using (Logger Log = Logger.IniciarLog("Carregando estados"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new EntidadeServico.EntidadeServicoClient())
                        {
                            EntidadeServico.Estados[] estados = client.ConsultarEstados();
                            if (estados.Length > 0)
                            {
                                ddlEstados.DataTextField = "NomeUF";
                                ddlEstados.DataValueField = "CodigoUF";
                                ddlEstados.DataSource = estados;
                                ddlEstados.DataBind();
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected object _motivos = null;

        /// <summary>
        /// 
        /// </summary>
        protected object MotivosMateriaisVendas
        {
            get
            {
                using (Logger Log = Logger.IniciarLog("Motivos materias vendas"))
                {
                    if (object.ReferenceEquals(_motivos, null))
                    {
                        using (var _client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Motivo[] motivos = _client.ConsultarMotivos();

                            if (motivos.Length > 0)
                                _motivos = motivos;
                            else
                                _motivos = new List<MaterialVendaServico.Motivo>();
                        }
                    }
                    return _motivos;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 intQuantidadeItensTecnologia = 0;

        /// <summary>
        /// Evento de databound dos Kits de Tecnologia
        /// </summary>
        protected void KitsTecnologiaDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Bind dos Kits de tecnologia"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                        {
                            DropDownList cboMotivo = e.Item.FindControl("cboMotivoTecno") as DropDownList;
                            if (!object.ReferenceEquals(cboMotivo, null))
                            {
                                cboMotivo.DataSource = MotivosMateriaisVendas;
                                cboMotivo.DataTextField = "DescricaoMotivo";
                                cboMotivo.DataValueField = "CodigoMotivo";
                                cboMotivo.DataBind();
                            }

                            TextBox txtQuantidadeItensTecnologia = e.Item.FindControl("txtQuantidadeItensTecnologia") as TextBox;
                            if (!object.ReferenceEquals(txtQuantidadeItensTecnologia, null))
                            {
                                txtQuantidadeItensTecnologia.Attributes.Add("Name", "txtQuantidadeItensTecnologia" + intQuantidadeItensTecnologia.ToString());
                            }
                            intQuantidadeItensTecnologia++;
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento de databound dos Materiais de venda, cadastra os motivos de solicitação
        /// </summary>
        protected void MateriaisVendaDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Bind dos materias de venda"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                        {
                            DropDownList cboMotivo = e.Item.FindControl("cboMotivo") as DropDownList;
                            if (!object.ReferenceEquals(cboMotivo, null))
                            {
                                cboMotivo.DataSource = MotivosMateriaisVendas;
                                cboMotivo.DataTextField = "DescricaoMotivo";
                                cboMotivo.DataValueField = "CodigoMotivo";
                                cboMotivo.DataBind();
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega os materiais de tecnologia disponíveis para o estabelecimento
        /// </summary>
        private void CarregarKitsTecnologia()
        {
            using (Logger Log = Logger.IniciarLog("Carregando kits de tecnologia"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Kit[] remessas = client.ConsultarKitsTecnologia(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptKitsTecnologia.DataSource = remessas;
                                rptKitsTecnologia.DataBind();
                            }
                            else
                            {
                                pnlKitsTecnologia.Visible = false;
                                pnlVazioKitsTecnologia.Visible = true;
                                ((QuadroAviso)qdKitsTecnologia).CarregarMensagem("Aviso", "Nenhum Kit de Tecnologia Cadastrado.");
                                ((QuadroAviso)qdKitsTecnologia).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega os materiais de apoio disponíveis para o estabelecimento
        /// </summary>
        private void CarregarKitsApoio()
        {
            using (Logger Log = Logger.IniciarLog("Carregando kits de apoio"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Kit[] remessas = client.ConsultarKitsApoio(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptKitsApoio.DataSource = remessas;
                                rptKitsApoio.DataBind();
                            }
                            else
                            {
                                pnlKitsApoio.Visible = false;
                                pnlVazioKitsApoio.Visible = true;
                                ((QuadroAviso)qdKitsApoio).CarregarMensagem("Aviso", "Nenhum Kit de Apoio Cadastrado.");
                                ((QuadroAviso)qdKitsApoio).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega os materiais de sinalização disponíveis para o estabelecimento
        /// </summary>
        private void CarregarKitsSinalizacao()
        {
            using (Logger Log = Logger.IniciarLog("Carregando kits de sinalização"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Kit[] remessas = client.ConsultarKitsSinalizacao(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptKitsSinalizacao.DataSource = remessas;
                                rptKitsSinalizacao.DataBind();
                            }
                            else
                            {
                                pnlKitsSinalizacao.Visible = false;
                                pnlVazioKitsSinalizacao.Visible = true;
                                ((QuadroAviso)qdKitsSinalizacao).CarregarMensagem("Aviso", "Nenhum Kit de Sinalização Cadastrado.");
                                ((QuadroAviso)qdKitsSinalizacao).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega os materiais disponíveis para o estabelecimento
        /// </summary>
        private void CarregarMateriaisVenda()
        {
            using (Logger Log = Logger.IniciarLog("Carregando materias de venda"))
            {
                try
                {
                    //Int32 codigoRetorno;
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Kit[] remessas = client.ConsultarKitsVendas(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptMateriaisVendas.DataSource = remessas;
                                rptMateriaisVendas.DataBind();
                            }
                            else
                            {
                                pnlMateriaisVendas.Visible = false;
                                pnlVazioMateriaisVendas.Visible = true;
                                ((QuadroAviso)qdMateriaisVendas).CarregarMensagem("Aviso", "Nenhum Material de Venda Cadastrado.");
                                ((QuadroAviso)qdMateriaisVendas).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega as remessas abertas (próximas remessas) do estabelecimento
        /// </summary>
        private void CarregarRemessasAbertas()
        {
            using (Logger Log = Logger.IniciarLog("Carregando remessas abertas"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Remessa[] remessas = client.ConsultarProximasRemessas(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptRemessasAbertas.DataSource = remessas;
                                rptRemessasAbertas.DataBind();
                            }
                            else
                            {
                                pnlProximasRemessas.Visible = false;
                                pnlVazioRemessasAbertas.Visible = true;
                                ((QuadroAviso)qdRemessasAbertas).CarregarMensagem("Aviso", "Não Existem Remessas Cadastradas.");
                                ((QuadroAviso)qdRemessasAbertas).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega as remessas baixadas do estabelecimento
        /// </summary>
        private void CarregarRemessasBaixadas()
        {
            using (Logger Log = Logger.IniciarLog("Carregando remessas baixadas"))
            {
                try
                {
                    if (!_indicaErro)
                    {
                        using (var client = new MaterialVendaServico.MaterialVendaServicoClient())
                        {
                            MaterialVendaServico.Remessa[] remessas = client.ConsultarUltimasRemessas(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                rptRemessasBaixadas.DataSource = remessas;
                                rptRemessasBaixadas.DataBind();
                            }
                            else
                            {
                                pnlUltimasRemessas.Visible = false;
                                pnlVazioRemessasBaixadas.Visible = true;
                                ((QuadroAviso)qdRemessasAbertas).CarregarMensagem("Aviso", "Não Existem Remessas Cadastradas.");
                                ((QuadroAviso)qdRemessasAbertas).ClasseImagem = "icone-aviso";
                            }
                        }
                    }
                }
                catch (FaultException<MaterialVendaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}