using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Sharepoint.Servicos;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class DadosContato : UserControlCredenciamentoBase
    {

        /// <summary>
        /// Regular expression para E-mails
        /// </summary>
        public static Regex RegexEmail
        {
            get { return new Regex(@"^(?<Conta>[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&''*+/=?^_`{|}~-]+)*)@((?<Dominio>(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?))\.)+(?<Dominio>[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9]))?$", RegexOptions.IgnoreCase); }
        }

        /// <summary>
        /// Retorna bool para tipo de equipamentos específicos que tornam obrigatórios os preenchimentos de certos campos na tela
        /// </summary>
        private Boolean CamposTipoEquipamentoObrigatorios
        {
            get
            {
                if (ViewState["CamposTipoEquipamentoObrigatorios"] != null)
                    return (Boolean)ViewState["CamposTipoEquipamentoObrigatorios"];
                return false;
            }
            set
            {
                ViewState["CamposTipoEquipamentoObrigatorios"] = value;
            }
        }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Carrega campos iniciais na tela
        /// </summary>
        public void CarregarCamposIniciais()
        {
            CarregarDropDownEstados();
            ((ResumoProposta)resumoProposta).CarregaResumoProposta();
            CarregarValoresDefault();
            AplicarRegrasNegocioPrevias();
            CarregarPropostaAndamento();
        }

        private void CarregarDropDownEstados()
        {
            ddlUfComercial.DataSource = ListaEstados();
            ddlUfComercial.DataBind();
            ddlUfCorrespondencia.DataSource = ListaEstados();
            ddlUfCorrespondencia.DataBind();
            ddlUfInstalacao.DataSource = ListaEstados();
            ddlUfInstalacao.DataBind();
        }

        /// <summary>
        /// Retorna lista de siglas de estados do Brasil
        /// </summary>
        /// <returns></returns>
        private ListItem[] ListaEstados()
        {
            ListItem[] listaEstados = new ListItem[]{ new ListItem(String.Empty),
                        new ListItem("AC"),
                        new ListItem("AL"),
                        new ListItem("AP"),
                        new ListItem("AM"),
                        new ListItem("BA"),
                        new ListItem("CE"),
                        new ListItem("DF"),
                        new ListItem("ES"),
                        new ListItem("GO"),
                        new ListItem("MA"),
                        new ListItem("MT"),
                        new ListItem("MS"),
                        new ListItem("MG"),
                        new ListItem("PA"),
                        new ListItem("PB"),
                        new ListItem("PR"),
                        new ListItem("PE"),
                        new ListItem("PI"),
                        new ListItem("RJ"),
                        new ListItem("RN"),
                        new ListItem("RS"),
                        new ListItem("RO"),
                        new ListItem("RR"),
                        new ListItem("SC"),
                        new ListItem("SP"),
                        new ListItem("SE"),
                        new ListItem("TO")
                    };

            return listaEstados;
        }

        /// <summary>
        /// Carrega valores da proposta em andamento recuperada
        /// </summary>
        private void CarregarPropostaAndamento()
        {
            Modelo.Endereco enderecoComercial = Credenciamento.Enderecos.FirstOrDefault(e => e.IndicadorTipoEndereco.Equals('1'));
            Modelo.Endereco enderecoCorrespondecia = Credenciamento.Enderecos.FirstOrDefault(e => e.IndicadorTipoEndereco.Equals('2'));
            Modelo.Endereco enderecoInstalacao = Credenciamento.Enderecos.FirstOrDefault(e => e.IndicadorTipoEndereco.Equals('4'));

            //Endereço Comercial
            if (enderecoComercial != null)
            {
                if (!String.IsNullOrEmpty(enderecoComercial.NumeroEndereco))
                    txtNumeroComercial.Text = enderecoComercial.NumeroEndereco.Trim();

                if (enderecoComercial.ComplementoEndereco != null && !String.IsNullOrEmpty(enderecoComercial.ComplementoEndereco.Trim()))
                    txtComplementoComercial.Text = enderecoComercial.ComplementoEndereco.Trim();
            }

            //Endereço Corresponsdência
            if (enderecoCorrespondecia != null)
            {
                txtCepCorrespondencia.Text = String.Format("{0}-{1}", enderecoCorrespondecia.CodigoCep.Trim(), enderecoCorrespondecia.CodigoComplementoCep.Trim());
                txtEnderecoCorrespondencia.Text = enderecoCorrespondecia.Logradouro.Trim();
                txtBairroCorrespondencia.Text = enderecoCorrespondecia.Bairro.Trim();
                txtCidadeCorrespondencia.Text = enderecoCorrespondecia.Cidade.Trim();
                ddlUfCorrespondencia.SelectedValue = enderecoCorrespondecia.Estado;
                //txtUfCorrespondencia.Text = enderecoCorrespondecia.Estado;
                txtNumeroCorrespondencia.Text = enderecoCorrespondecia.NumeroEndereco.Trim();
                txtComplementoCorrespondencia.Text = enderecoCorrespondecia.ComplementoEndereco.Trim();
            }

            //Endereço Corresponsdência
            if (enderecoInstalacao != null)
            {
                txtCepInstalacao.Text = String.Format("{0}-{1}", enderecoInstalacao.CodigoCep.Trim(), enderecoInstalacao.CodigoComplementoCep.Trim());
                txtEnderecoInstalacao.Text = enderecoInstalacao.Logradouro.Trim();
                txtBairroInstalacao.Text = enderecoInstalacao.Bairro.Trim();
                txtCidadeInstalacao.Text = enderecoInstalacao.Cidade.Trim();
                ddlUfInstalacao.SelectedValue = enderecoInstalacao.Estado;
                //txtUfInstalacao.Text = enderecoInstalacao.Estado;
                txtNumeroInstalacao.Text = enderecoInstalacao.NumeroEndereco.Trim();
                txtComplementoInstalacao.Text = enderecoInstalacao.ComplementoEndereco.Trim();

            }

            //Preenchimento dos Checkbox
            if (Credenciamento.Proposta.IndicadorEnderecoIgualComercial == 'N')
            {
                cbEnderecoCorrespondenciaIgualComercial.Checked = false;
                ExibirBlocoEnderecoCorrespondencia(true);
            }

            if (Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial == 'N')
            {
                cbEnderecoInstalacaoIgualComercial.Checked = false;
                ExibirBlocoEnderecoInstalacao(true);
            }

            //Contato para Instalação
            if (!String.IsNullOrEmpty(Credenciamento.Tecnologia.NomeContato))
                txtNomeContato.Text = Credenciamento.Tecnologia.NomeContato.Trim();

            if (!String.IsNullOrEmpty(Credenciamento.Tecnologia.NumeroDDD))
                txtDddTelefoneContato.Text = Credenciamento.Tecnologia.NumeroDDD.Trim();

            if (Credenciamento.Tecnologia.NumeroTelefone != 0)
                txtTelefoneContato.Text = Credenciamento.Tecnologia.NumeroTelefone.ToString();

            if (Credenciamento.Tecnologia.NumeroRamal != 0)
                txtRamalContato.Text = Credenciamento.Tecnologia.NumeroRamal.ToString();

            if (!String.IsNullOrEmpty(Credenciamento.Tecnologia.Observacao))
                txtObservacaoContato.Text = Credenciamento.Tecnologia.Observacao.Trim();

            if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD1))
                txtDddTelefone1.Text = Credenciamento.Proposta.NumeroDDD1.Trim();

            if (Credenciamento.Proposta.NumeroTelefone1 != null && Credenciamento.Proposta.NumeroTelefone1 != 0)
                txtTelefone1.Text = Credenciamento.Proposta.NumeroTelefone1.Value.ToString();

            if (Credenciamento.Proposta.Ramal1 != null && Credenciamento.Proposta.Ramal1 != 0)
                txtRamal1.Text = Credenciamento.Proposta.Ramal1.Value.ToString();

            if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD2))
                txtDdTelefone2.Text = Credenciamento.Proposta.NumeroDDD2.Trim();

            if (Credenciamento.Proposta.NumeroTelefone2 != null && Credenciamento.Proposta.NumeroTelefone2 != 0)
                txtTelefone2.Text = Credenciamento.Proposta.NumeroTelefone2.Value.ToString();

            if (Credenciamento.Proposta.Ramal2 != null && Credenciamento.Proposta.Ramal2 != 0)
                txtRamal2.Text = Credenciamento.Proposta.Ramal2.Value.ToString();

            if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDDFax))
                ddlDddTelefone3.Text = Credenciamento.Proposta.NumeroDDDFax.Trim();

            if (Credenciamento.Proposta.NumeroTelefoneFax != null && Credenciamento.Proposta.NumeroTelefoneFax != 0)
                txtTelefone3.Text = Credenciamento.Proposta.NumeroTelefoneFax.Value.ToString();

            //email e site
            if (Credenciamento.Proposta.NomeEmail != null && !String.IsNullOrEmpty(Credenciamento.Proposta.NomeEmail.Trim()))
            {
                txtEmail.Text = Credenciamento.Proposta.NomeEmail.Trim();
                cbAtivarExtratoEmail.Checked = true;
            }

            if (Credenciamento.Proposta.NomeHomePage != null && !String.IsNullOrEmpty(Credenciamento.Proposta.NomeHomePage.Trim()))
                txtSite.Text = Credenciamento.Proposta.NomeHomePage;
        }

        /// <summary>
        /// Carregar valores de endereço comercial, habilita edição caso o CEP não tenha retornado endereço
        /// </summary>
        private void CarregarValoresDefault()
        {
            if (Credenciamento.OfertasPrecoUnico.Count > 0)
            {
                var tecnologia = ServicosWF.CarregarDadosTecnologia(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf, Credenciamento.Proposta.IndicadorSequenciaProposta);
                Credenciamento.Tecnologia = new Modelo.Tecnologia()
                {
                    CodigoTipoEquipamento = tecnologia.CodTipoEquipamento,
                    HoraInicioFuncionamento = tecnologia.HoraInicioFuncionamento.GetValueOrDefault(),
                    HoraFimFuncionamento = tecnologia.HoraFimFuncionamento.GetValueOrDefault(),
                    DiaInicioFuncionamento = tecnologia.DiaInicioFuncionamento.GetValueOrDefault(),
                    DiaFimFuncionamento = tecnologia.DiaFimFuncionamento.GetValueOrDefault(),
                    NomeContato = tecnologia.NomeContato,
                    NumeroDDD = tecnologia.NumDDD,
                    NumeroTelefone = tecnologia.NumTelefone.GetValueOrDefault(),
                    NumeroRamal = tecnologia.NumRamal.GetValueOrDefault(),
                    Observacao = tecnologia.Observacao,
                    IndicadorEnderecoIgualComercial = tecnologia.IndEnderecoIgualComercial.GetValueOrDefault(),
                };
            }

            var enderecoComercial = Credenciamento.Enderecos.FirstOrDefault(e => e.IndicadorTipoEndereco.Equals('1'));
            if (enderecoComercial != null)
            {
                txtCepComercial.Text = String.Format("{0}-{1}", enderecoComercial.CodigoCep, enderecoComercial.CodigoComplementoCep);
                txtCepComercial.Enabled = false;

                txtEnderecoComercial.Text = enderecoComercial.Logradouro.Trim();
                txtEnderecoComercial.Enabled = String.IsNullOrEmpty(enderecoComercial.Logradouro.Trim());

                txtBairroComercial.Text = enderecoComercial.Bairro.Trim();
                txtBairroComercial.Enabled = String.IsNullOrEmpty(enderecoComercial.Bairro.Trim());

                txtCidadeComercial.Text = enderecoComercial.Cidade.Trim();
                txtCidadeComercial.Enabled = String.IsNullOrEmpty(enderecoComercial.Cidade.Trim());

                ddlUfComercial.SelectedValue = enderecoComercial.Estado;
                ddlUfComercial.Enabled = String.IsNullOrEmpty(enderecoComercial.Estado);
                //txtUfComercial.Text = enderecoComercial.Estado;
                //txtUfComercial.Enabled = String.IsNullOrEmpty(enderecoComercial.Estado);
            }

            ddlDiasFuncionamentoInicio.SelectedValue = "2";
            ddlDiasFuncionamentoFim.SelectedValue = "6";

            ddlHorarioFuncionamentoInicio.SelectedValue = "800";
            ddlHorarioFuncionamentoFim.SelectedValue = "1800";
        }

        /// <summary>
        /// Captura dados da tela e salva no BD
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Contato - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        ValidaTelefonesIguais();
                        CapturarInformacoesPreenchidas();

                        Int32? codigoFilial = null;
                        Int32? codigoZonaVenda = null;
                        ServicosWF.SalvarDadosContato(Credenciamento.Proposta, Credenciamento.Enderecos.ToList(), Credenciamento.Tecnologia, ref codigoFilial, ref codigoZonaVenda);

                        Credenciamento.Proposta.CodigoFilial = codigoFilial;
                        Credenciamento.Proposta.CodigoZona = codigoZonaVenda;

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

        /// <summary>
        /// Evento disparado ao clicar no botão Salvar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    CapturarInformacoesPreenchidas();

                    //Salvar dados
                    Int32? codigoFilial = null;
                    Int32? codigoZonaVenda = null;
                    ServicosWF.SalvarDadosContato(Credenciamento.Proposta, Credenciamento.Enderecos.ToList(), Credenciamento.Tecnologia, ref codigoFilial, ref codigoZonaVenda);

                    Credenciamento.Proposta.CodigoFilial = codigoFilial;
                    Credenciamento.Proposta.CodigoZona = codigoZonaVenda;
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
                //Redirect
                Credenciamento = null;
                NovaProposta(sender, e);
            }

        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Valida se telefones 1, 2 e 3 preenchidos são iguais
        /// </summary>
        private void ValidaTelefonesIguais()
        {
            if (!String.IsNullOrEmpty(txtTelefone2.Text))
            {
                if (txtDddTelefone1.Text.Equals(txtDdTelefone2.Text) &&
                    txtTelefone1.Text.Equals(txtTelefone2.Text) &&
                    txtRamal1.Text.Equals(txtRamal2.Text))
                    throw new PortalRedecardException(312, FONTE);
            }

            if (!String.IsNullOrEmpty(txtTelefone3.Text))
            {
                if (txtDddTelefone1.Text.Equals(ddlDddTelefone3.Text) &&
                    txtTelefone1.Text.Equals(txtTelefone3.Text))
                    throw new PortalRedecardException(313, FONTE);
            }

            if (!String.IsNullOrEmpty(txtTelefone2.Text) && !String.IsNullOrEmpty(txtTelefone3.Text))
            {
                if (txtDdTelefone2.Text.Equals(ddlDddTelefone3.Text) &&
                    txtTelefone2.Text.Equals(txtTelefone3.Text))
                    throw new PortalRedecardException(314, FONTE);
            }
        }

        public event EventHandler Continuar;

        /// <summary>
        /// Evento que volta para o formulário anterior
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Contato - Voltar"))
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

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCepComercial_TextChanged(object sender, EventArgs e)
        {
            EventoBuscaCEP(txtCepComercial, txtEnderecoComercial, txtBairroComercial, txtCidadeComercial, ddlUfComercial);
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCepCorrespondencia_TextChanged(object sender, EventArgs e)
        {
            EventoBuscaCEP(txtCepCorrespondencia, txtEnderecoCorrespondencia, txtBairroCorrespondencia, txtCidadeCorrespondencia, ddlUfCorrespondencia);
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCepInstalacao_TextChanged(object sender, EventArgs e)
        {
            EventoBuscaCEP(txtCepInstalacao, txtEnderecoInstalacao, txtBairroInstalacao, txtCidadeInstalacao, ddlUfInstalacao);
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox cbEnderecoCorrespondenciaIgualComercial
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbEnderecoCorrespondenciaIgualComercial_CheckedChanged(object sender, EventArgs e)
        {
            ExibirBlocoEnderecoCorrespondencia(!((CheckBox)sender).Checked);
        }

        /// <summary>
        /// Evento ao marcar, ou desmarcar o checkbox cbEnderecoInstalacaoIgualComercial
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbEnderecoInstalacaoIgualComercial_CheckedChanged(object sender, EventArgs e)
        {
            ExibirBlocoEnderecoInstalacao(!((CheckBox)sender).Checked);
        }

        /// <summary>
        /// Exibe/Não Exibe correspondência
        /// </summary>
        /// <param name="visible">Visible</param>
        private void ExibirBlocoEnderecoCorrespondencia(Boolean visible)
        {
            phEnderecoCorrespondencia.Visible = visible;
        }

        /// <summary>
        /// Exibe/Não Exibe bloco instalação
        /// </summary>
        /// <param name="visible">Visible</param>
        private void ExibirBlocoEnderecoInstalacao(Boolean visible)
        {
            phEnderecoInstalacao.Visible = visible;
        }

        /// <summary>
        /// Aplica regras de negocio, de acordo com dados preenchidos na Sessao nos passos anteriores.
        /// </summary>
        private void AplicarRegrasNegocioPrevias()
        {


            if (Credenciamento.Tecnologia != null)
            {
                CamposTipoEquipamentoObrigatorios = (String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "SNT") == 0
                                                  || String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "TOF") == 0
                                                  || String.Compare(Credenciamento.Tecnologia.CodigoTipoEquipamento, "TOL") == 0);

                CampoEmailSiteObrigatorio(CamposTipoEquipamentoObrigatorios);
            }
        }

        /// <summary>
        /// Torna campo e-mail, e Site Obrigatorio/Não Obrigatorio.
        /// </summary>
        /// <param name="obrigatorio">Obrigatório</param>
        private void CampoEmailSiteObrigatorio(Boolean obrigatorio)
        {
            lblObrigatoriedadeEmail.Visible = obrigatorio;
            rfvObrigatoriedadeEmail.Visible = obrigatorio;

            lblObrigatoriedadeConfirmaEmail.Visible = obrigatorio;
            rfvObrigatoriedadeConfirmacaoEmail.Visible = obrigatorio;

            lblObrigatoriedadeSite.Visible = obrigatorio;
            rfvObrigatoriedadeSite.Visible = obrigatorio;
            revEmail.Visible = obrigatorio;
        }

        /// <summary>
        /// Regra de negócio ao mudar checkbox Extrato Email
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void cbAtivarExtratoEmail_CheckedChanged(object sender, EventArgs e)
        {
            rfvObrigatoriedadeEmail.Visible = cbAtivarExtratoEmail.Checked;
        }

        /// <summary>
        /// Operação realizada quando o campo texto CEP é modificado.
        /// </summary>
        /// <param name="txtCEP">CEP</param>
        /// <param name="txtEndereco">Endereço</param>
        /// <param name="txtBairro">Bairro</param>
        /// <param name="txtCidade">Cidade</param>
        /// <param name="txtUf">UF</param>
        private void EventoBuscaCEP(TextBox txtCEP, TextBox txtEndereco, TextBox txtBairro, TextBox txtCidade, DropDownList ddlUf)
        {
            if (Request.Form.Get("__EVENTTARGET") == txtCEP.UniqueID)
            {
                if (!String.IsNullOrEmpty(txtCEP.Text) && txtCEP.Text.Length == 9)
                {
                    CarregarLogradouro(txtCEP, txtEndereco, txtBairro, txtCidade, ddlUf);
                }
                else
                    txtEndereco.Text = String.Empty;
            }
        }

        /// <summary>
        /// Carrega endereço e acordo com o CEP
        /// </summary>
        /// <param name="txtCEP">CEP</param>
        /// <param name="txtEndereco">Endereço</param>
        /// <param name="txtBairro">Bairro</param>
        /// <param name="txtCidade">Cidade</param>
        /// <param name="txtUf">UF</param>
        private void CarregarLogradouro(TextBox txtCEP, TextBox txtEndereco, TextBox txtBairro, TextBox txtCidade, DropDownList ddlUf)
        {
            String cep = txtCEP.Text.Replace("-", "");
            String endereco = String.Empty;
            String bairro = String.Empty;
            String cidade = String.Empty;
            String uf = String.Empty;

            Int32 codigoRetorno = 0;
            codigoRetorno = ServicosDR.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);

            if (codigoRetorno == 38)
            {
                HabilitarCamposEndereco(txtEndereco, txtBairro, txtCidade, ddlUf, false);
                txtEndereco.Text = String.Format("{0}", endereco);
                txtBairro.Text = String.Format("{0}", bairro);
                txtCidade.Text = String.Format("{0}", cidade);
                ddlUf.SelectedValue = String.Format("{0}", uf);
                //txtUf.Text = String.Format("{0}", uf);
            }
            else if (codigoRetorno == 1)
            {
                HabilitarCamposEndereco(txtEndereco, txtBairro, txtCidade, ddlUf, false);
                txtCidade.Text = String.Format("{0}", cidade);
                ddlUf.SelectedValue = String.Format("{0}", uf);
                //txtUf.Text = String.Format("{0}", uf);
            }
            else
            {
                HabilitarCamposEndereco(txtEndereco, txtBairro, txtCidade, ddlUf, true);
                txtEndereco.Text = String.Empty;
            }
        }

        /// <summary>
        /// Habilita/Desabilita os campos de endereço, cidade, e UF.
        /// </summary>
        /// <param name="txtEndereco">Endereço</param>
        /// <param name="txtBairro">Bairro</param>
        /// <param name="txtCidade">Cidade</param>
        /// <param name="txtUf">UF</param>
        /// <param name="enabled">Habilitado</param>
        private void HabilitarCamposEndereco(TextBox txtEndereco, TextBox txtBairro, TextBox txtCidade, DropDownList ddlUf, Boolean enabled)
        {
            txtEndereco.Enabled = enabled;
            txtCidade.Enabled = enabled;
            ddlUf.Enabled = enabled;
            txtBairro.Enabled = enabled;
        }

        /// <summary>
        /// Validação de campos Site e email quando obrigatórios
        /// </summary>
        /// <returns>retorna boolean site e email preenchidos</returns>
        private Boolean CamposSiteEmailPreenchidos()
        {
            Boolean retorno = true;
            if (CamposTipoEquipamentoObrigatorios)
            {
                if (String.IsNullOrEmpty(txtSite.Text) || String.IsNullOrEmpty(txtEmail.Text))
                {
                    ExibirPainelExcecao(FONTE, 0030);
                    retorno = false;
                }
                else if (Regex.IsMatch(txtEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                    retorno = true;
            }
            else
                retorno = true;

            return retorno;
        }

        /// <summary>
        /// Preenche objeto endereço com valores dos parâmetros
        /// </summary>
        /// <param name="cep">CEP</param>
        /// <param name="logradouro">Logradouro</param>
        /// <param name="numero">Numero</param>
        /// <param name="complemento">Complemento</param>
        /// <param name="bairro">Bairro</param>
        /// <param name="cidade">Cidade</param>
        /// <param name="estado">Estado</param>
        /// <param name="indicadorTipoEndereco">Indicador do tipo de endereço</param>
        /// <returns>retorna objeto Endereço</returns>
        private Modelo.Endereco PreencherObjetoEndereco(String cep, String logradouro, String numero, String complemento, String bairro, String cidade, String estado, Char indicadorTipoEndereco)
        {
            Modelo.Endereco endereco = new Modelo.Endereco();

            endereco.CodigoTipoPessoa = (Modelo.TipoPessoa)Credenciamento.Proposta.CodigoTipoPessoa;
            endereco.NumeroCNPJ = Credenciamento.Proposta.NumeroCnpjCpf;
            endereco.NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta;
            endereco.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
            endereco.DataHoraUltimaAtualizacao = DateTime.Now;
            endereco.CodigoCep = cep.Substring(0, 5);
            endereco.CodigoComplementoCep = cep.Substring(6, 3);
            endereco.Logradouro = logradouro;
            endereco.NumeroEndereco = numero;
            endereco.ComplementoEndereco = complemento;
            endereco.Bairro = bairro;
            endereco.Cidade = cidade;
            endereco.Estado = estado;
            endereco.IndicadorTipoEndereco = indicadorTipoEndereco;

            return endereco;
        }

        /// <summary>
        /// Metodo que adiciona endereco, com as informações do endereco comercial
        /// </summary>
        /// <param name="tipoEndereco">Tipo de Endereço</param>
        public void PreencherEnderecoGenerico(Char tipoEndereco)
        {
            Modelo.Endereco endereco = PreencherObjetoEndereco(
                    txtCepComercial.Text.Trim(),
                    txtEnderecoComercial.Text.ToUpper().Trim(),
                    txtNumeroComercial.Text.Trim(),
                    txtComplementoComercial.Text.ToUpper().Trim(),
                    txtBairroComercial.Text.ToUpper().Trim(),
                    txtCidadeComercial.Text.ToUpper().Trim(),
                    ddlUfComercial.SelectedValue.ToUpper(),
                //txtUfComercial.Text.ToUpper(),
                    tipoEndereco);

            if (Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).Count() > 0)
                Credenciamento.Enderecos.Remove(Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).FirstOrDefault());

            Credenciamento.Enderecos.Add(endereco);
        }

        /// <summary>
        /// Metodo que adiciona endereco de correspondencia a sessao
        /// </summary>
        private void PreencherEnderecoCorrespondencia()
        {
            Char tipoEndereco = '2';

            if (phEnderecoCorrespondencia.Visible)
            {
                Modelo.Endereco endereco = PreencherObjetoEndereco(
                        txtCepCorrespondencia.Text.Trim(),
                        txtEnderecoCorrespondencia.Text.ToUpper().Trim(),
                        txtNumeroCorrespondencia.Text.Trim(),
                        txtComplementoCorrespondencia.Text.ToUpper().Trim(),
                        txtBairroCorrespondencia.Text.ToUpper().Trim(),
                        txtCidadeCorrespondencia.Text.ToUpper().Trim(),
                        ddlUfCorrespondencia.SelectedValue.ToUpper(),
                    //txtUfCorrespondencia.Text.ToUpper(),
                        tipoEndereco);

                if (Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).Count() > 0)
                    Credenciamento.Enderecos.Remove(Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).FirstOrDefault());

                Credenciamento.Enderecos.Add(endereco);
            }
            else
                PreencherEnderecoGenerico(tipoEndereco);
        }

        /// <summary>
        /// Metodo que adiciona endereco de instalacao a sessao
        /// </summary>
        private void PreencherEnderecoInstalacao()
        {
            Char tipoEndereco = '4';

            if (phEnderecoInstalacao.Visible)
            {
                Modelo.Endereco endereco = PreencherObjetoEndereco(
                        txtCepInstalacao.Text.Trim(),
                        txtEnderecoInstalacao.Text.ToUpper().Trim(),
                        txtNumeroInstalacao.Text.Trim(),
                        txtComplementoInstalacao.Text.ToUpper().Trim(),
                        txtBairroInstalacao.Text.ToUpper().Trim(),
                        txtCidadeInstalacao.Text.ToUpper().Trim(),
                        ddlUfInstalacao.SelectedValue.ToUpper(),
                    //txtUfInstalacao.Text.ToUpper(),
                        tipoEndereco);

                if (Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).Count() > 0)
                    Credenciamento.Enderecos.Remove(Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == tipoEndereco).FirstOrDefault());

                Credenciamento.Enderecos.Add(endereco);
            }
            else
                PreencherEnderecoGenerico(tipoEndereco);
        }

        /// <summary>
        /// Carrega informações preenchidas na tela para objeto
        /// </summary>
        public void CapturarInformacoesPreenchidas()
        {
            //Tipo 2 = Comercial
            PreencherEnderecoGenerico('1');

            //Tipo 2 = Correspondencia
            PreencherEnderecoCorrespondencia();

            //Tipo 4 = Instalacao
            PreencherEnderecoInstalacao();

            Credenciamento.Tecnologia.HoraInicioFuncionamento = ddlHorarioFuncionamentoInicio.SelectedValue.ToInt32();
            Credenciamento.Tecnologia.HoraFimFuncionamento = ddlHorarioFuncionamentoFim.SelectedValue.ToInt32();

            Credenciamento.Tecnologia.DiaInicioFuncionamento = ddlDiasFuncionamentoInicio.SelectedValue.ToInt32();
            Credenciamento.Tecnologia.DiaFimFuncionamento = ddlDiasFuncionamentoFim.SelectedValue.ToInt32();

            Credenciamento.Tecnologia.NomeContato = txtNomeContato.Text.ToUpper().Trim();
            Credenciamento.Tecnologia.NumeroDDD = txtDddTelefoneContato.Text.Trim();
            Credenciamento.Tecnologia.NumeroTelefone = txtTelefoneContato.Text.Replace("-", "").ToInt32();
            Credenciamento.Tecnologia.NumeroRamal = txtRamalContato.Text.ToInt32();
            Credenciamento.Tecnologia.Observacao = txtObservacaoContato.Text.ToUpper().Trim();

            Credenciamento.Proposta.NumeroDDD1 = txtDddTelefone1.Text.Trim();
            Credenciamento.Proposta.NumeroTelefone1 = txtTelefone1.Text.ToInt32();
            Credenciamento.Proposta.Ramal1 = txtRamal1.Text.ToInt32();

            Credenciamento.Proposta.NumeroDDD2 = txtDdTelefone2.Text.Trim();
            Credenciamento.Proposta.NumeroTelefone2 = txtTelefone2.Text.ToInt32();
            Credenciamento.Proposta.Ramal2 = txtRamal2.Text.ToInt32();

            Credenciamento.Proposta.NumeroDDDFax = ddlDddTelefone3.Text.Trim();
            Credenciamento.Proposta.NumeroTelefoneFax = txtTelefone3.Text.ToInt32();

            Credenciamento.Proposta.NomeEmail = txtEmail.Text.ToUpper() ?? String.Empty;
            Credenciamento.Proposta.NomeHomePage = txtSite.Text.ToUpper() ?? String.Empty;

            char? indicadorEnvioExtratoEmail = 'N';
            if (cbAtivarExtratoEmail.Checked)
                indicadorEnvioExtratoEmail = 'S';

            Credenciamento.Proposta.IndicadorEnvioExtratoEmail = indicadorEnvioExtratoEmail;

            if (cbEnderecoCorrespondenciaIgualComercial.Checked)
                Credenciamento.Proposta.IndicadorEnderecoIgualComercial = 'S';
            else
                Credenciamento.Proposta.IndicadorEnderecoIgualComercial = 'N';

            if (cbEnderecoInstalacaoIgualComercial.Checked)
                Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial = 'S';
            else
                Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial = 'N';

            if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 4)
                Credenciamento.Proposta.CodigoFaseFiliacao = 4;
        }

        /// <summary>
        /// Valida Telefone 
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvTelefone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                String telefone = args.Value;
                args.IsValid = ServicosGE.ValidaTelefone(telefone);
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

        /// <summary>
        /// Valida confirmação de e-mail igual ao email digitado
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvConfirmaEmail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                String strConfirmaEmail = args.Value;
                String strEmail = txtEmail.Text;

                if (String.Compare(strConfirmaEmail.ToLower(), strEmail.ToLower()) == 0)
                    args.IsValid = true;
                else
                    args.IsValid = false;
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

        /// <summary>
        /// Valida domínio fora da blackList
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvDominioEmail_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (ValidarDominio())
                    args.IsValid = true;
                else
                    args.IsValid = false;
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

        /// <summary>
        /// Valida se o e-mail informado é válido e se o domínio do e-mail é permitido.
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se e-mail é válido e permitido</returns>
        public bool ValidarDominio()
        {
            String email = txtEmail.Text;
            String dominio = String.Empty;

            //Verifica se o domínio do e-mail é permitido
            Boolean dominioValido = VerificarDominioEmailPermitido(email);

            Match match = RegexEmail.Match(email);
            if (match.Success)
                dominio = email.Split('@')[1];

            if (!dominioValido)
            {

                String msgDominioInvalido = String.Format(
                    "Domínio inválido. Por favor, insira outro e-mail", dominio);

                cvDominioEmail.Text = msgDominioInvalido;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se o domínio do e-mail é válido
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <returns></returns>
        public static Boolean VerificarDominioEmailPermitido(String email)
        {
            try
            {
                String[] dominiosBloqueados = DominiosBloqueados;

                //Verifica se o domínio do e-mail está na banlist dos domínios não permitidos
                foreach (String dominio in dominiosBloqueados)
                {
                    Boolean dominioBloqueado = VerificarDominioEmail(dominio, email);
                    if (dominioBloqueado)
                        return false;
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Erro ao validar domínio do e-mail", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao validar domínio do e-mail", ex);
            }

            return true;
        }

        /// <summary>
        /// Domínios bloqueados/não permitidos para cadastro de usuários
        /// </summary>
        private static String[] DominiosBloqueados
        {
            get
            {
                try
                {
                    //Query para recuperar apenas os registros ativos
                    var query = new SPQuery();
                    query.Query = String.Concat(
                        "<Where>",
                            "<Eq>",
                                "<FieldRef Name=\"Ativo\" />",
                                "<Value Type=\"Boolean\">1</Value>",
                            "</Eq>",
                        "</Where>");

                    //Preparação do objeto de retorno contendo a lista de domínios bloqueados
                    if (ListaDominiosBloqueados != null)
                        return ListaDominiosBloqueados.GetItems(query)
                            .Cast<SPListItem>()
                            .Select(spItem => (String)spItem["Dominio"]).ToArray();
                    else
                        return new String[0];
                }
                catch (SPException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    return new String[0];
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    return new String[0];
                }
            }
        }

        /// <summary>
        /// Verifica se determinado domínio do e-mail é aceito por determinada expressão de busca.<br/>
        /// O parâmetro "pattern" aceita os wildcards "*" e "?".<br/>
        /// Apenas o que sucede o '@' no e-mail é considerado.<br/>               
        /// </summary>
        /// <param name="pattern">Pattern</param>
        /// <param name="email">E-mail a ser verificado</param>
        /// <returns>TRUE: se e-mail atende o padrão informado</returns>
        private static Boolean VerificarDominioEmail(String pattern, String email)
        {
            Match match = RegexEmail.Match(email);
            if (match.Success)
            {
                //Extrai o domínio e subdomínios do e-mail
                String[] subdominios = match.Groups["Dominio"].Captures.Cast<Capture>()
                    .Select(capture => capture.Value).ToArray();
                String dominio = String.Join(".", subdominios);

                //Converte para Regex
                pattern = String.Concat("^", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."), "$");
                return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(dominio);
            }
            else
                return false;
        }

        #region [ Listas ]

        /// <summary>
        /// Lista de domínios bloqueados
        /// </summary>
        private static SPList ListaDominiosBloqueados
        {
            get
            {
                SPList lista = null;

                SPUtility.ValidateFormDigest();

                //Recupera a lista de "Domínios Bloqueados" em sites/fechado/minhaconta
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                    using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    {
                        //spWeb.AllowUnsafeUpdates = true;
                        lista = spWeb.Lists.TryGetList("Domínios Bloqueados");
                    }
                });

                return lista;
            }
        }

        #endregion

        /// <summary>
        /// Valida Telefone 
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvTelefone2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (String.IsNullOrEmpty(txtDdTelefone2.Text) && !String.IsNullOrEmpty(txtTelefone2.Text) ||
                    !String.IsNullOrEmpty(txtDdTelefone2.Text) && String.IsNullOrEmpty(txtTelefone2.Text))
                {
                    args.IsValid = false;
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

        /// <summary>
        /// Valida Telefone 
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvTelefone3_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (String.IsNullOrEmpty(ddlDddTelefone3.Text) && !String.IsNullOrEmpty(txtTelefone3.Text) ||
                    !String.IsNullOrEmpty(ddlDddTelefone3.Text) && String.IsNullOrEmpty(txtTelefone3.Text))
                {
                    args.IsValid = false;
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
}