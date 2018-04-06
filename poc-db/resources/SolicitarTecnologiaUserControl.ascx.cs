/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/21 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.ServiceModel;
using System.Web.UI;
using System.Globalization;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.Emissores.Sharepoint.WebParts.SolicitarTecnologia
{
    public partial class SolicitarTecnologiaUserControl : UserControlBase
    {


        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ClickBuscar(object sender, EventArgs e)
        {
            Logger.IniciarLog("Início evento ClickBuscar ");
            try
            {
                PontoVenda pv = new PontoVenda();

                pnlSolicitacaoEmissores.Visible = false;
                pnlBuscar.Visible = true;
                Int32 codigoRetorno = 0;
                String mensagemRetorno = string.Empty;
                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    Logger.GravarLog("Chamada ao método ObtemPV ", new { NumeroEstabelecimento.Text });
                    pv = context.Cliente.ObtemPV(out codigoRetorno, NumeroEstabelecimento.Text.ToInt32());
                    Logger.GravarLog("Retorno chamada ao método ObtemPV ", new { pv , codigoRetorno});
                    if (codigoRetorno != 0 || object.Equals(pv, null))
                    {
                        List<Panel> lstPaineis = new List<Panel>();
                        lstPaineis.Add(pnlTudo);
                        ExibirPainelConfirmacaoAcao("Emissores", "Não há dados para este Nº de Estabelecimento", Request.Url.AbsolutePath, lstPaineis.ToArray(), "icone-aviso");
                        return;
                    }
                }
                

                pnlDados.Visible = true;
                pnlEnviar.Visible = false;

                lblBairro.Text = pv.Endereco.Bairro;
                lblCep.Text = pv.Endereco.Cep;
                lblCidade.Text = pv.Endereco.Cidade;
                //lblComplemento.Text = pv.Endereco.Complemento;
                lblEndereco.Text = pv.Endereco.Endereco;
                //lblNumero.Text = pv.Endereco.NumeroEndereco;
                lblestado.Text = pv.Endereco.Uf;

                lblCnpj.Text = pv.Cnpj;
                lblRazaoSocial.Text = pv.RazaoSocial;
                lblNomeFantasia.Text = pv.NomeComercial;


                //dados da entrega
                if (!object.Equals(pv.EnderecoEntrega, null))
                {
                    txtEntEndereco.Text = pv.EnderecoEntrega.Endereco;
                    txtEntNumero.Text = pv.EnderecoEntrega.NumeroEndereco;
                    txtEntBairro.Text = pv.EnderecoEntrega.Bairro;
                    txtEntCep.Text = pv.EnderecoEntrega.Cep;
                    txtEntCidade.Text = pv.EnderecoEntrega.Cidade;
                    txtEntComplemento.Text = pv.EnderecoEntrega.Complemento;
                }

                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    Logger.GravarLog("Chamada ao método ConsultarEquipamento ", new { NumeroEstabelecimento.Text });
                    List<Equipamento> lstEquipamentos = context.Cliente.ConsultarEquipamento(out codigoRetorno, out mensagemRetorno);
                    Logger.GravarLog("Retorno chamada ao método ConsultarEquipamento ", new { lstEquipamentos, codigoRetorno, mensagemRetorno }); 
                    
                    rptMaquina.DataSource = lstEquipamentos;
                    if (codigoRetorno != 0)
                    {
                        ExibirPainelExcecao("Erro ao consultar equipamentos", codigoRetorno.ToString());
                        return;
                    }

                    List<ListItem> integradores = new List<ListItem>();
                    Logger.GravarLog("Chamada ao método ConsultarIntegrador ", new {  });

                    List<Integrador> listaIntegradores = context.Cliente.ConsultarIntegrador(null, "A");
                    Logger.GravarLog("Retorno chamada ao método ConsultarIntegrador ", new { listaIntegradores}); 

                    integradores.Add(new ListItem { Text = "Selecione um Integrador", Value = "-1", Selected = true });

                    if (!object.Equals(listaIntegradores, null) && listaIntegradores.Count > 0)
                        integradores.AddRange(listaIntegradores.Select(x => new ListItem { Text = x.Descricao, Value = x.Codigo.ToString() }).ToList<ListItem>());

                    ddlIntegrador.DataSource = integradores;
                    ddlIntegrador.DataBind();
                    rptMaquina.DataBind();
                }
                Logger.GravarLog("Retorno do método ", new {  });
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("ClickBuscar - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ClickBuscar - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return;
            }

        }

        protected void OnclickEnviar(object sender, EventArgs e)
        {

            bool mostraEquipamentos = false;
            bool integra = false;
            int cont = 0;

            foreach (RepeaterItem item in rptMaquina.Items)
            {
                RadioButton rbtEquipamento = (RadioButton)rptMaquina.Items[cont].FindControl("rbtEquipamento");

                if (rbtEquipamento.Checked)
                {
                    HiddenField hidCodigoEquipamento = (HiddenField)item.FindControl("hidCodigoEquipamento");
                    integra = (hidCodigoEquipamento.Value == "PDV");
                    Literal ltlTipoEquipamento = (Literal)item.FindControl("ltlTipoEquipamento");
                    Literal ltlPreco = (Literal)item.FindControl("ltlPreco");

                    hidCodigoEquipamentoSelecionado.Value = hidCodigoEquipamento.Value;

                    DropDownList ddlQuantidade = (DropDownList)item.FindControl("ddlQuantidade");

                    if (ddlQuantidade.SelectedValue.Trim() == "" || Convert.ToInt32(ddlQuantidade.SelectedValue.Trim()) == 0)
                    {
                        ExibirPainelExcecao("Quantidade inválida.", "");
                        ddlQuantidade.Focus();
                        return;
                    }

                    mostraEquipamentos = true;
                    lblQtd.Text = ddlQuantidade.SelectedValue;
                    lblTipoTerminal.Text = ltlTipoEquipamento.Text;
                    lblPreco.Text = ltlPreco.Text;

                    if (integra)
                        lblIntegrador.Text = ddlIntegrador.SelectedItem.Text;


                }
                cont++;
            }
            pnlConfirmacaoEquipamentos.Visible = mostraEquipamentos;

            if (ddlIntegrador.SelectedIndex > 0)
            {
                pnlSolicitacaoEmissores.Visible = false;
                pnlEnviar.Visible = true;
                pnlDados.Visible = false;
            }
            else
            {
                lblErro.Visible = true;
                lblErro.Text = "*";
            }

        }

        protected void OnclickConfirmar(object sender, EventArgs e)
        {
            Logger.GravarLog("Início evento OnclickConfirmar");
            try
            {
                HisServicoWfEmissores.DadosEmissao entradaEmissao = new HisServicoWfEmissores.DadosEmissao();
                entradaEmissao.DadosPV = new HisServicoWfEmissores.PontoVenda();

                Int32 codigoRetorno = 0;
                String mensagemRetorno = string.Empty;

                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    Logger.GravarLog("Chamada ao método ObtemPV ", new { NumeroEstabelecimento.Text });
                    PontoVenda pv = context.Cliente.ObtemPV(out codigoRetorno, NumeroEstabelecimento.Text.ToInt32());
                    Logger.GravarLog("Retorno chamada ao método ObtemPV ", new { pv, codigoRetorno });
                    if (codigoRetorno != 0 || object.Equals(pv, null))
                    {
                        List<Panel> lstPaineis = new List<Panel>();
                        lstPaineis.Add(pnlTudo);
                        ExibirPainelConfirmacaoAcao("Emissores", "Erro ao confirmar dados", Request.Url.AbsolutePath, lstPaineis.ToArray(), "icone-aviso");
                        return;
                    }

                    entradaEmissao.DadosPV.TipoPessoa = pv.TipoPessoa;
                    entradaEmissao.DadosPV.RazaoSocial = pv.RazaoSocial;
                    entradaEmissao.DadosPV.CodigoRamoAtividade = pv.CodigoRamoAtividade;
                    entradaEmissao.DadosPV.DescricaoRamoAtividade = pv.DescricaoRamoAtividade;
                    entradaEmissao.DadosPV.PessoaContato = pv.PessoaContato;
                    entradaEmissao.DadosPV.NomeComercial = pv.NomeComercial;
                    entradaEmissao.DadosPV.Centralizadora = pv.Centralizadora;
                    entradaEmissao.DadosPV.Cnpj = pv.Cnpj;
                    entradaEmissao.DadosPV.Codigo = pv.Codigo;
                    entradaEmissao.DadosPV.DataFundacao = pv.DataFundacao;
                    entradaEmissao.DadosPV.Email = pv.Email;
                    entradaEmissao.DadosPV.TipoEstabelecimento = pv.TipoEstabelecimento;
                    entradaEmissao.DadosPV.TipoPessoa = pv.TipoPessoa;

                    entradaEmissao.DadosPV.ListaProprietarios = new List<HisServicoWfEmissores.DadosProprietario>();

                    foreach (DadosProprietario item in pv.ListaProprietarios)
                    {
                        entradaEmissao.DadosPV.ListaProprietarios.Add(new HisServicoWfEmissores.DadosProprietario()
                        {
                            Nome = item.Nome,
                            CPF = item.CPF,
                            DataNascimento = item.DataNascimento,
                            TipoPessoa = item.TipoPessoa,
                            Percetual = item.Percetual
                        });
                    }

                    if (!object.Equals(pv.DadosBancarioCredito, null))
                    {
                        entradaEmissao.DadosPV.DadosBancarioCredito = new HisServicoWfEmissores.DadosBancarios()
                        {
                            CodBanco = pv.DadosBancarioCredito.CodBanco,
                            NumAgencia = pv.DadosBancarioCredito.NumAgencia,
                            NumContaCorrente = pv.DadosBancarioCredito.NumContaCorrente
                        };
                    }

                    if (!object.Equals(pv.DadosBancarioDebito, null))
                    {
                        entradaEmissao.DadosPV.DadosBancarioDebito = new HisServicoWfEmissores.DadosBancarios()
                        {
                            CodBanco = pv.DadosBancarioDebito.CodBanco,
                            NumAgencia = pv.DadosBancarioDebito.NumAgencia,
                            NumContaCorrente = pv.DadosBancarioDebito.NumContaCorrente
                        };
                    }

                    if (!object.Equals(pv.DadosBancarioMaestro, null))
                    {
                        entradaEmissao.DadosPV.DadosBancarioMaestro = new HisServicoWfEmissores.DadosBancarios()
                        {
                            CodBanco = pv.DadosBancarioMaestro.CodBanco,
                            NumAgencia = pv.DadosBancarioMaestro.NumAgencia,
                            NumContaCorrente = pv.DadosBancarioMaestro.NumContaCorrente
                        };
                    }


                    if (!object.Equals(pv.DadosBancarioConstrucard, null))
                    {
                        entradaEmissao.DadosPV.DadosBancarioConstrucard = new HisServicoWfEmissores.DadosBancarios()
                        {
                            CodBanco = pv.DadosBancarioConstrucard.CodBanco,
                            NumAgencia = pv.DadosBancarioConstrucard.NumAgencia,
                            NumContaCorrente = pv.DadosBancarioConstrucard.NumContaCorrente
                        };
                    }


                    entradaEmissao.DadosPV.CodigoCentral = pv.CodigoCentral;

                    entradaEmissao.DadosPV.Endereco = new HisServicoWfEmissores.EnderecoPadrao()
                        {
                            Bairro = pv.Endereco.Bairro,
                            Cep = pv.Endereco.Cep,
                            Cidade = pv.Endereco.Cidade,
                            Complemento = pv.Endereco.Complemento,
                            Endereco = pv.Endereco.Endereco,
                            NumeroEndereco = pv.Endereco.NumeroEndereco,
                            Uf = pv.Endereco.Uf
                        };

                    entradaEmissao.DadosPV.EnderecoEntrega = new HisServicoWfEmissores.EnderecoPadrao()
                    {
                        Bairro = txtEntBairro.Text,
                        Cep = txtEntCep.Text,
                        Cidade = txtEntCidade.Text,
                        Complemento = txtEntComplemento.Text,
                        Endereco = txtEntEndereco.Text,
                        NumeroEndereco = txtEntNumero.Text
                    };

                    if (!object.Equals(pv.Telefone, null))
                    {
                        entradaEmissao.DadosPV.Telefone = new HisServicoWfEmissores.DadosTelefone() { DDD = pv.Telefone.DDD, Ramal = pv.Telefone.Ramal, Telefone = pv.Telefone.Telefone };
                    }
                    if (!object.Equals(pv.Telefone, null))
                    {
                        entradaEmissao.DadosPV.Telefone2 = new HisServicoWfEmissores.DadosTelefone() { DDD = pv.Telefone2.DDD, Ramal = pv.Telefone2.Ramal, Telefone = pv.Telefone2.Telefone };
                    }
                    if (!object.Equals(pv.Fax, null))
                    {
                        entradaEmissao.DadosPV.Fax = new HisServicoWfEmissores.DadosTelefone() { DDD = pv.Fax.DDD, Ramal = pv.Fax.Ramal, Telefone = pv.Fax.Telefone };
                    }

                    entradaEmissao.DadosPV.NomePlaqueta1 = pv.NomePlaqueta1;
                    entradaEmissao.DadosPV.NomePlaqueta2 = pv.NomePlaqueta2;

                    entradaEmissao.CodTipoEquipamento = hidCodigoEquipamentoSelecionado.Value;
                    entradaEmissao.QtdeEquipamento = lblQtd.Text.ToInt16(0);
                    entradaEmissao.ValorEquipamento = lblPreco.Text.ToDecimal();
                    entradaEmissao.CodIntegrador = ddlIntegrador.SelectedValue.ToInt16();

                    //obtém dados do vendedor
                    Logger.GravarLog("Chamda ao método ConsultaVendedor", new { entradaEmissao.DadosPV.TipoPessoa, entradaEmissao.DadosPV.Cnpj });
                    DadosVendedor vendedor = context.Cliente.ConsultaVendedor(entradaEmissao.DadosPV.TipoPessoa, entradaEmissao.DadosPV.Cnpj);
                    Logger.GravarLog("Retorno chamda ao método ConsultaVendedor", new { vendedor });
                    if (!object.Equals(vendedor, null))
                    {
                        entradaEmissao.CPFVendedor = vendedor.Cpf.ToInt16(0);
                        entradaEmissao.DadosPV.DataFundacao = vendedor.DataFundacao;
                    }
                }
                using (var context = new ContextoWCF<HisServicoWfEmissores.HisServicoWfEmissoresClient>())
                {
                    Logger.GravarLog("Chamda ao método EfetuarSolicitacao", new { SessaoAtual.CodigoEntidade, entradaEmissao });
                    bool retorno = context.Cliente.EfetuarSolicitacao(out codigoRetorno, out mensagemRetorno, SessaoAtual.CodigoEntidade, entradaEmissao);
                    Logger.GravarLog("Retorno chamda ao método ConsultaVendedor", new { retorno, codigoRetorno, mensagemRetorno });

                    if (codigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao("HisServicoWfEmissores.EfetuarSolicitacao", codigoRetorno);
                        return;
                    }
                    
                    if (!retorno)
                    {
                        List<Panel> lstPaineis = new List<Panel>();
                        lstPaineis.Add(pnlTudo);
                        base.ExibirPainelConfirmacaoAcao("Emissores", "Erro ao gravar solicitação", Request.Url.AbsolutePath, lstPaineis.ToArray(), "icone-red");
                        return;
                    }
                }

                pnlSolicitacaoEmissores.Visible = false;
                pnlBuscar.Visible = false;
                pnlEnviar.Visible = false;
                pnlSucesso.Visible = true;
                //((QuadroAviso)qdAvisoSucesso).ClasseImagem = "icone-green";
                //((QuadroAviso)qdAvisoSucesso).Mensagem = "Solictitação(ões) incluída(s) com sucesso.";
                //((QuadroAviso)qdAvisoSucesso).TituloMensagem = "Concuído com sucesso";
                //((QuadroAviso)qdAvisoSucesso).Visible = true;
                ((QuadroAviso)qdAvisoSucesso).CarregarMensagem();
                Logger.GravarLog("Fim evento OnclickConfirmar");
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("OnclickConfirmar - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("OnclickConfirmar - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void OnclickVoltarInicial(object sender, EventArgs e)
        {
            pnlBuscar.Visible = false;
            pnlEnviar.Visible = false;
            pnlSucesso.Visible = false;
            NumeroEstabelecimento.Text = "";
            pnlSolicitacaoEmissores.Visible = true;
        }

        protected void btnvoltarDadosEquipementos_Click(object sender, EventArgs e)
        {
            NumeroEstabelecimento.Text = "";
            pnlSolicitacaoEmissores.Visible = true;
            pnlBuscar.Visible = false;
        }

        protected void btnvoltarConfirmacao_Click(object sender, EventArgs e)
        {
            pnlEnviar.Visible = false;
            pnlTudo.Visible = true;
            pnlBuscar.Visible = true;
            pnlDados.Visible = true;
            pnlEnviar.Visible = false;
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {

        }

        protected void rptMaquina_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Equipamento item = (Equipamento)e.Item.DataItem;
                if (!object.Equals(item, null))
                {
                    Literal ltlPreco = (Literal)e.Item.FindControl("ltlPreco");
                    Literal ltlTipoEquipamento = (Literal)e.Item.FindControl("ltlTipoEquipamento");
                    ltlTipoEquipamento.Text = item.Descricao;
                    ltlPreco.Text = item.ValorVenda.ToString("N2");
                }

            }
        }

    }
}
