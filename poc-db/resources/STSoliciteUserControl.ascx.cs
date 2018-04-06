using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Comum.SharePoint.EntidadeServico;
using Redecard.PN.OutrasEntidades.SharePoint.ServicoPortalEmissores;
using Redecard.PN.OutrasEntidades.SharePoint.HisServicoWfEmissores;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.FMS.STSolicite
{
    public partial class STSoliciteUserControl : UserControlBase
    {
        #region .Page_Load.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessaoAtual != null && SessaoAtual.GrupoEntidade == 12)
            {
                ViewState["GrupoEntidade"] = "PARCEIROS";
            }
            else
            {
                ViewState["GrupoEntidade"] = "EMISSORES";
            }

            lblTitulo.Text = string.Format("SOLICITAÇÃO DE TECNOLOGIA - {0}", ViewState["GrupoEntidade"]);
        }
        #endregion

        #region .mvwPropCredConsulte_ActiveViewChanged.
        /// <summary>
        /// Evento de mudança da view
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void mvwPropCredConsulte_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwPropCredConsulte.GetActiveView();

            if (activeView == vwRetornoDadosConsulte)
            {
                lblTitulo2.Text = string.Format("SOLICITAÇÃO DE TECNOLOGIA - {0}", ViewState["GrupoEntidade"]);
            }
        }
        #endregion

        #region .btnContinuarRetornoConsulta_Click.
        protected void btnContinuarRetornoConsulta_Click(object sender, EventArgs e)
        {
            Services servicos = new Services(this);

            Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade entidade = servicos.ConsultarDadosCompletos(int.Parse(txtPv.Text.Trim()));

            if (entidade != null && !string.IsNullOrEmpty(entidade.CNPJEntidade))
            {
                ltlTipoPessoa.Text = (entidade.CNPJEntidade.Length > 10 ? "Juridico" : "Fisica");
                ltlCnpj.Text = entidade.CNPJEntidade;
                ltlRazaoSocial.Text = entidade.RazaoSocial;
                ltlNomeFantasia.Text = entidade.NomeEntidade;
                ltlEndereco.Text = entidade.Endereco;
                ltlCep.Text = entidade.CEP;
                ltlBairro.Text = entidade.Bairro;
                ltlCidade.Text = entidade.Cidade;
                hdnRamo.Value = entidade.RamoAtividade;

                ChangeView(vwRetornoDadosConsulte);
            }
            else
            {
                MostrarMensagem("PV não encontrado");
            }
        }
        #endregion

        #region .MostrarMensagem.
        /// <summary>
        /// Mostra a mensagem no quadro aviso
        /// </summary>
        /// <param name="Mensagem"></param>
        private void MostrarMensagem(string Mensagem)
        {
            ((QuadroAviso)quadroAviso).ExibirPainelMensagem(Mensagem);
        }
        #endregion

        #region .btnContinuarImprimir_Click.
        protected void btnContinuarImprimir_Click(object sender, EventArgs e)
        {
            Services servicos = new Services(this);

            Int32 codigo = 0;
            string mensagem = string.Empty;

            Equipamento[] equipamentos = servicos.ConsultarEquipamentos(out codigo, out mensagem);
            #region .Debug.
            //equipamentos.SetValue(new Equipamento
            //{
            //    Codigo = "1",
            //    CodigoRegimeAlguel = "PJ",
            //    DataUltAtualizacao = DateTime.Now,
            //    Descricao = "Descrição",
            //    IndTecnologia = "Sim",
            //    Quantidade = 10,
            //    Situacao = "Regularizado",
            //    ValorMaximo = 1000,
            //    ValorMinimo = 100,
            //    ValorVenda = 2000
            //}, 0);
            //equipamentos.SetValue(new Equipamento
            //{
            //    Codigo = "2",
            //    CodigoRegimeAlguel = "CLT",
            //    DataUltAtualizacao = DateTime.Now,
            //    Descricao = "Descrição 2",
            //    IndTecnologia = "Sim",
            //    Quantidade = 11,
            //    Situacao = "Regularizado 2",
            //    ValorMaximo = 1000,
            //    ValorMinimo = 100,
            //    ValorVenda = 2000
            //}, 1);
            //equipamentos.SetValue(new Equipamento
            //{
            //    Codigo = "3",
            //    CodigoRegimeAlguel = "PJ",
            //    DataUltAtualizacao = DateTime.Now,
            //    Descricao = "Descrição 3",
            //    IndTecnologia = "Sim",
            //    Quantidade = 13,
            //    Situacao = "Regularizado 3",
            //    ValorMaximo = 1000,
            //    ValorMinimo = 100,
            //    ValorVenda = 2000
            //}, 2);
            //equipamentos.SetValue(new Equipamento
            //{
            //    Codigo = "4",
            //    CodigoRegimeAlguel = "PJ",
            //    DataUltAtualizacao = DateTime.Now,
            //    Descricao = "Descrição 4",
            //    IndTecnologia = "Sim",
            //    Quantidade = 14,
            //    Situacao = "Regularizado 4",
            //    ValorMaximo = 1000,
            //    ValorMinimo = 100,
            //    ValorVenda = 2000
            //}, 3);
            #endregion
            if (equipamentos != null && equipamentos.Length > 0)
            {
                rptEquipamentos.DataSource = equipamentos;
                rptEquipamentos.DataBind();

                Integrador[] integradores = servicos.ConsultarIntegradores(string.Empty, "A");
                //integradores.SetValue(new Integrador { Codigo = "1", Descricao = "Selecione" }, 0);
                //integradores.SetValue(new Integrador { Codigo = "2", Descricao = "Descrição 2" }, 1);
                //integradores.SetValue(new Integrador { Codigo = "3", Descricao = "Descrição 3" }, 2);
                //integradores.SetValue(new Integrador { Codigo = "4", Descricao = "Descrição 4" }, 3);
                drpIntegrador.DataValueField = "Codigo";
                drpIntegrador.DataTextField = "Descricao";
                drpIntegrador.DataSource = integradores;
                drpIntegrador.DataBind();

                ChangeView(vwImprimir);
            }
            else
            {
                MostrarMensagem("Equipamentos não encontrado");
            }
        }
        #endregion

        #region .btnVoltarFiltro_Click.
        protected void btnVoltarFiltro_Click(object sender, EventArgs e)
        {
            ChangeView(vwPreencherFiltrosConsulte);
        }
        #endregion

        #region .btnVoltarRetornoDados_Click.
        protected void btnVoltarRetornoDados_Click(object sender, EventArgs e)
        {
            ChangeView(vwRetornoDadosConsulte);
        }
        #endregion

        #region .ChangeView.
        /// <summary>
        /// Altera a visualização na tela
        /// </summary>
        /// <param name="view">ID da view</param>
        private void ChangeView(View view)
        {
            mvwPropCredConsulte.SetActiveView(view);
        }
        #endregion

        #region .rptEquipamentos_ItemDataBound.
        protected void rptEquipamentos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Equipamento equipamento = e.Item.DataItem as Equipamento;

                RepeaterItem item = e.Item;

                Label litTipoEquipamento = (Label)item.FindControl("litTipoEquipamento");
                Label litPreco = (Label)item.FindControl("litPreco");
                HtmlInputRadioButton radio = (HtmlInputRadioButton)item.FindControl("rdbEquipamento");

                litTipoEquipamento.Text = equipamento.Descricao;
                litPreco.Text = equipamento.ValorVenda.ToString();
                radio.Attributes.Add("value", equipamento.Descricao);
            }
        }
        #endregion

        #region .btnAvancarConfirmacao_Click.
        protected void btnAvancarConfirmacao_Click(object sender, EventArgs e)
        {
            if (hdnQtd.Value.ToInt32() == 0)
            {
                MostrarMensagem("Digite a quantidade");
                return;
            }
            if (string.IsNullOrEmpty(hdnTpEquipamento.Value))
            {
                MostrarMensagem("Selecione um equipamento");
                return;
            }

            litTipoPessoa.Text = ltlTipoPessoa.Text;
            litCNPJ_CPF.Text = ltlCnpj.Text;
            litRazaoSocial.Text = ltlRazaoSocial.Text;
            litNomeFantasia.Text = ltlNomeFantasia.Text;
            litEndereco.Text = ltlEndereco.Text;
            litCEP.Text = ltlCep.Text;
            litBairro.Text = ltlBairro.Text;
            litCidade.Text = ltlCidade.Text;
            litQtd.Text = hdnQtd.Value;
            litTipoTerminal.Text = hdnTpEquipamento.Value;
            litPrecoUnitario.Text = hdnPreco.Value;

            if (drpIntegrador.SelectedIndex > 0)
            {
                litIntegrador.Text = drpIntegrador.SelectedItem.Value;
            }

            Services service = new Services(this);
            Int32 codigoRetorno = 0;
            Endereco[] enderecos = service.ConsultarEnderecos(out codigoRetorno, 1250191, "C");

            foreach (Endereco item in enderecos)
            {
                litEnderecoCorrespondencia.Text = item.EnderecoEstabelecimento;
                litNumeroCorrespondencia.Text = item.Numero;
                litComplementoCorrespondencia.Text = item.Complemento;
                litBairroCorrespondencia.Text = item.Bairro;
                litCEPCorrespondencia.Text = item.CEP;
                litCidadeCorrespondencia.Text = item.Cidade;
                litUfCorrespondencia.Text = item.UF;
            }

            ChangeView(vwConfirmacao);
        }
        #endregion

        #region .btnVoltarImprimir_Click.
        protected void btnVoltarImprimir_Click(object sender, EventArgs e)
        {
            ChangeView(vwImprimir);
        }
        #endregion

        #region .btnGravar_Click.
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            var service = new Services(this);
            Int32 pv = int.Parse(txtPv.Text.Trim());
            Int32 codigoRetorno = 0;
            string mensagemRetorno = string.Empty;
            HisServicoWfEmissores.PontoVenda pontoVenda = new HisServicoWfEmissores.PontoVenda();

            pontoVenda.TipoPessoa = ltlTipoPessoa.Text.Substring(0, 1);
            pontoVenda.Cnpj = ltlCnpj.Text;
            pontoVenda.CodigoRamoAtividade = (hdnRamo.Value.Contains('-') ? hdnRamo.Value.Split('-').FirstOrDefault().PadLeft(5, '0') : hdnRamo.Value);
            pontoVenda.RazaoSocial = ltlRazaoSocial.Text;
            pontoVenda.Codigo = pv;

            EntidadeServico.DadosBancarios dados = service.ConsultarDadosBancarios(pv);

            if (!string.IsNullOrEmpty(dados.NomeBanco) &&
                !string.IsNullOrEmpty(dados.NomeAgencia) &&
                !string.IsNullOrEmpty(dados.ContaAtualizada))
            {
                HisServicoWfEmissores.DadosBancarios dadosBancariosCredito = new HisServicoWfEmissores.DadosBancarios();
                dadosBancariosCredito.CodBanco = int.Parse(dados.NomeBanco);
                dadosBancariosCredito.NumAgencia = int.Parse(dados.NomeAgencia);
                dadosBancariosCredito.NumContaCorrente = int.Parse(dados.ContaAtualizada);

                pontoVenda.DadosBancarioCredito = dadosBancariosCredito;
            }
            else
            {
                pontoVenda.DadosBancarioCredito = null;
            }

            if (!string.IsNullOrEmpty(dados.BancoDebito) &&
                !string.IsNullOrEmpty(dados.AgenciaDebito) &&
                !string.IsNullOrEmpty(dados.NumeroContaDebito))
            {
                HisServicoWfEmissores.DadosBancarios dadosBancariosDebito = new HisServicoWfEmissores.DadosBancarios();
                dadosBancariosDebito.CodBanco = int.Parse(dados.BancoDebito);
                dadosBancariosDebito.NumAgencia = int.Parse(dados.AgenciaDebito);
                dadosBancariosDebito.NumContaCorrente = int.Parse(dados.NumeroContaDebito);

                pontoVenda.DadosBancarioDebito = dadosBancariosDebito;
            }
            else
            {
                pontoVenda.DadosBancarioDebito = null;
            }

            HisServicoWfEmissores.EnderecoPadrao enderecoPadrao = new HisServicoWfEmissores.EnderecoPadrao();
            enderecoPadrao.Endereco = litEndereco.Text;
            enderecoPadrao.Complemento = string.Empty;
            enderecoPadrao.NumeroEndereco = string.Empty;
            enderecoPadrao.Bairro = litBairro.Text;
            enderecoPadrao.Cidade = litCidade.Text;
            enderecoPadrao.Uf = string.Empty;
            enderecoPadrao.Cep = litCEP.Text;
            pontoVenda.Endereco = enderecoPadrao;

            HisServicoWfEmissores.EnderecoPadrao enderecoEntrega = new HisServicoWfEmissores.EnderecoPadrao();
            enderecoEntrega.Endereco = litEnderecoCorrespondencia.Text;
            enderecoEntrega.Complemento = litComplementoCorrespondencia.Text;
            enderecoEntrega.NumeroEndereco = litNumeroCorrespondencia.Text;
            enderecoEntrega.Bairro = litBairroCorrespondencia.Text;
            enderecoEntrega.Cidade = litCidadeCorrespondencia.Text;
            enderecoEntrega.Uf = litUfCorrespondencia.Text;
            enderecoEntrega.Cep = string.Empty;
            pontoVenda.EnderecoEntrega = enderecoEntrega;

            pontoVenda.PessoaContato = string.Empty;//NOM_PES_CTTO_PDV_EMI
            pontoVenda.Email = string.Empty;//COD_INDC_ACES_INTN_EMI 
            pontoVenda.Telefone = null;/*new HisServicoWfEmissores.DadosTelefone
                        {
                            DDD = "",//NUM_DDD_PDV_EMI
                            Telefone = ""//NUM_TEL_PDV_EMI
                        }*/
            pontoVenda.NomePlaqueta2 = string.Empty;//NOM_PLQT_2_PDV_EMI
            pontoVenda.TipoEstabelecimento = string.Empty;//COD_TIP_ESTB_PDV_EMI
            pontoVenda.CodigoCentral = 0;//R1_NUM_PDV_EMI
            pontoVenda.NomeComercial = litNomeFantasia.Text;//NOM_FAT_PDV_EMI
            pontoVenda.ListaProprietarios = null; /*new HisServicoWfEmissores.DadosProprietario
            {
                Nome = "",//NOM-PRPT-EMI
                DataNascimento = DateTime.Now,//DAT-NASC-PRPT-EMI
                CPF = 0,//NUM-CPF-PRPT-EMI
                TipoPessoa = ""//COD-TIP-P-PRPT-EMI
            };*/
            pontoVenda.Codigo = 0;

            DadosEmissao dadosEmissao = new DadosEmissao();
            dadosEmissao.DadosPV = pontoVenda;
            dadosEmissao.CodTipoEquipamento = litTipoTerminal.Text;
            dadosEmissao.QtdeEquipamento = (short)int.Parse(litQtd.Text);
            dadosEmissao.ValorEquipamento = decimal.Parse(litPrecoUnitario.Text);
            dadosEmissao.CodAgenciaFilia = 0;
            dadosEmissao.CPFVendedor = 0;

            dadosEmissao.CodIntegrador = (litIntegrador.Text == string.Empty ? 0 : int.Parse(litIntegrador.Text));

            service.EfetuarSolicitacao(out codigoRetorno, out mensagemRetorno, 0/*COD_CEL_EMI*/, dadosEmissao);
        } 
        #endregion
    }
}