#region Histórico do Arquivo
/*
© Copyright 2016 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
Histórico   : Criação da Classe
- 20/03/2012 – Criação - Mario Neto
*/

#endregion

using System;
using System.Web.UI;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SharePoint.Utilities;
using System.Web;

using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Text.RegularExpressions;


namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.SolicitarMateriais
{
    /// <summary>
    /// Web part de Solicitação de Material de Venda
    /// </summary>
    public partial class SolicitarMateriaisUserControl : UserControlBase
    {
        #region [Variáveis]
        /// <summary>
        /// Responsável pelo Serviço a ser consumido para as regras da tela
        /// </summary>
        private const String NomeServico = "Redecard.PN.OutrosServicos.Servicos.MaterialVendaServico";

        /// <summary>
        /// Responsável pelo Serviço que presta a manutembilidade da tela
        /// </summary>
        private const String NomeServicoIncluir = "MaterialVenda.IncluirKit";

        /// <summary>
        /// Flag para indicar se houve erro durante a execução da página
        /// </summary>
        private Boolean IndicaErro = false;
        #endregion

        /// <summary>
        /// Carregamento da página, realizar buscas de Remessas enviadas e Remessas baixadas.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sessao.Contem())
            {
                if (!Page.IsPostBack && !IsPostBack && Util.UsuarioLogadoFBA())
                {
                    // carregar listas de estados
                    this.CarregarSugestaoEndereco();

                    // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                    if (SessaoAtual.UsuarioAtendimento)
                    {
                        btnEnviar.Visible = false;
                    }

                    this.hdnSuccessPanel.Value = false.ToString();

                    //Validar: Possui Terminal??? Existe materiais Cadastrado???
                    this.ValidarPossibilidadePedido();
                }

               //CarregaTela
                this.ReloadTela();
            }
        }

        /// <summary>
        /// Carregamento dos dados "dinamicos" da Tela.
        /// </summary>
        private void ReloadTela() {
            //carregar Itens para remessas solicitadas (Abertas e Baixadas)
            this.CarregarRemessasBaixadas();
            this.CarregarRemessasAbertas();

            //checkar existencia de itens...
            this.CheckItems();
        }

        /// <summary>
        /// Indica comportamento das mensagens/grid de consulta da tela.
        /// </summary>
        private void CheckItems()
        {
            if (pnlRemessasAbertasSemItens.Visible && pnlRemessasBaixadasSemItens.Visible)
            {
                pnlRemessasAbertasSemItens.Visible = false;
                pnlRemessasBaixadasSemItens.Visible = false;
                pnlSemItemNenhum.Visible = true;

                //esconder todos os componentes da tela.
                pnlLabelAberto.Visible = false;
                pnlLabelEntregue.Visible = false;
            }
            else {
                pnlSemItemNenhum.Visible = false;

                //esconder todos os componentes da tela.
                pnlLabelAberto.Visible = true;
                pnlLabelEntregue.Visible = true;
            }
        }

        /// <summary>
        /// Efetua o carregamento do endereço padrão (cadastro) do cliente perante ao GE.
        /// </summary>
        private void CarregarSugestaoEndereco()
        {
            //Carregar o endereço a ser sugerido no formulario de solicitação de materiais.
            using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                Int32 codigoRetorno;
                EntidadeServico.Endereco[] enderecos = client.Cliente.ConsultarEndereco(out codigoRetorno, base.SessaoAtual.CodigoEntidade, "I");

                if (enderecos.Length > 0)
                {
                    EntidadeServico.Endereco endereco = enderecos[0];
                    txtContato.Text = RemoverEspacos(SessaoAtual.NomeUsuario);
                    txtEmail.Text = RemoverEspacos(SessaoAtual.Email);
                    txtEndereco.Text = RemoverEspacos(endereco.EnderecoEstabelecimento);
                    txtNumero.Text = RemoverEspacos(endereco.Numero);
                    txtComplemento.Text = RemoverEspacos(endereco.Complemento);
                    txtBairro.Text = RemoverEspacos(endereco.Bairro);
                    txtCEP.Text = RemoverEspacos(endereco.CEP);
                    txtCidade.Text = RemoverEspacos(endereco.Cidade);
                    txtTelefone.Text = FormatarTelefone(endereco.Telefone);
                    txtEstado.Text = RemoverEspacos(endereco.UF);
                }
            }

        }

        /// <summary>
        /// Verifica se há solicitações para Kits de determinado tipo.
        /// </summary>
        private void CheckarKitsPedidos(MaterialVendaServico.Remessa[] remessas)
        {
            MaterialVendaServico.Kit[] kitsVendas = CarregarMateriaisVenda();
            MaterialVendaServico.Kit[] kitsSinalizacao = CarregarKitsSinalizacao();

            this.hdnSolicitacaoKitBobina.Value = remessas.Any(a => a.Kit != null && kitsVendas.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).Trim()).Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).Trim())).ToString();
            this.hdnSolicitacaoKitSinalizacao.Value = remessas.Any(a => a.Kit != null && kitsSinalizacao.Select(s => (String.IsNullOrEmpty(s.DescricaoKit) ? "" : s.DescricaoKit).Trim()).Contains((String.IsNullOrEmpty(a.Kit.DescricaoKit) ? "" : a.Kit.DescricaoKit).Trim())).ToString();
        }

        /// <summary>
        /// Carrega os materiais disponíveis para o estabelecimento
        /// </summary>
        private MaterialVendaServico.Kit[] CarregarMateriaisVenda()
        {
            using (Logger Log = Logger.IniciarLog("Carregando materias de venda"))
            {
                try
                {
                    //Int32 codigoRetorno;
                    if (!IndicaErro)
                    {
                        using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
                        {
                            MaterialVendaServico.Kit[] remessas = client.Cliente.ConsultarKitsVendas(this.SessaoAtual.CodigoEntidade);

                            return remessas != null && remessas.Length > 0 ? remessas : null;

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
                return null;
            }
        }

        /// <summary>
        /// Carrega os materiais de sinalização disponíveis para o estabelecimento
        /// </summary>
        private MaterialVendaServico.Kit[] CarregarKitsSinalizacao()
        {
            using (Logger Log = Logger.IniciarLog("Carregando kits de sinalização"))
            {
                try
                {
                    if (!IndicaErro)
                    {
                        using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
                        {
                            MaterialVendaServico.Kit[] remessas = client.Cliente.ConsultarKitsSinalizacao(this.SessaoAtual.CodigoEntidade);

                            return remessas != null && remessas.Length > 0 ? remessas : null;
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
            return null;
        }

        /// <summary>
        /// Carrega as remessas (solicitações) abertas (próximas remessas) do estabelecimento
        /// </summary>
        private void CarregarRemessasAbertas()
        {
            using (Logger Log = Logger.IniciarLog("Carregando remessas abertas"))
            {
                try
                {
                    if (!IndicaErro)
                    {
                        using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
                        {
                            MaterialVendaServico.Remessa[] remessas = client.Cliente.ConsultarProximasRemessas(this.SessaoAtual.CodigoEntidade);

                            if (remessas.Length > 0)
                            {
                                remessas = remessas.OrderByDescending(o => o.DataRemessa).ToArray();
                                this.CheckarKitsPedidos(remessas);
                                rptRemessasAbertas.DataSource = remessas.Length > 3 ? remessas.Take(3).ToArray() : remessas;
                                rptRemessasAbertas.DataBind();

                                pnlRemessasAbertas.Visible = true;
                                pnlRemessasAbertasSemItens.Visible = false;
                            }
                            else
                            {
                                pnlRemessasAbertas.Visible = false;
                                pnlRemessasAbertasSemItens.Visible = true;
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
        /// Carrega as remessas (solicitações finalizadas) baixadas do estabelecimento
        /// </summary>
        private void CarregarRemessasBaixadas(Boolean moreThanThree = false)
        {
            using (Logger Log = Logger.IniciarLog("Carregando remessas baixadas"))
            {
                try
                {
                    if (!IndicaErro)
                    {
                        using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
                        {
                            MaterialVendaServico.Remessa[] remessas = client.Cliente.ConsultarUltimasRemessas(this.SessaoAtual.CodigoEntidade);

                            if (remessas != null && remessas.Length > 0)
                            {
                                pnlShowMoreOrLess.Visible = remessas.Length > 3;

                                rptRemessasBaixadas.DataSource = remessas.OrderByDescending(o => o.DataRemessa).ToArray();
                                rptRemessasBaixadas.DataBind();

                                pnlRemessasBaixadas.Visible = true;
                                pnlRemessasBaixadasSemItens.Visible = false;
                            }
                            else
                            {
                                pnlRemessasBaixadas.Visible = false;
                                pnlRemessasBaixadasSemItens.Visible = true;
                                pnlShowMoreOrLess.Visible = false;

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
        /// Verificar se o Estabelecimento atual possui terminal com "necessidade" de solicitar bobina. e Se tem Material cadastrado.
        /// </summary>
        private void ValidarPossibilidadePedido()
        {
            using (Logger Log = Logger.IniciarLog("Verificando se o PV possui Terminal que necessita de bobina."))
            {
                try
                {
                    if (!IndicaErro)
                    {
                        using (var client = new ContextoWCF<MaximoServico.MaximoServicoClient>())
                        {
                            pnlSolicitarKitBobina.Visible = client.Cliente.PossuiTerminalComBobina(this.SessaoAtual.CodigoEntidade.ToString());
                        }

                        if(pnlSolicitarKitBobina.Visible){
                            MaterialVendaServico.Kit[] kitsVendas = this.CarregarMateriaisVenda();
                            pnlSolicitarKitBobina.Visible = kitsVendas != null && kitsVendas.Length > 0;
                        }

                        if (pnlSolicitarkitSinalizacao.Visible)
                        {
                            MaterialVendaServico.Kit[] kitsSinalizacao = this.CarregarKitsSinalizacao();
                            pnlSolicitarkitSinalizacao.Visible = kitsSinalizacao != null && kitsSinalizacao.Length > 0;
                        }
                    }
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    pnlSolicitarKitBobina.Visible = false;
                    pnlSolicitarkitSinalizacao.Visible = false;
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    pnlSolicitarKitBobina.Visible = false;
                    pnlSolicitarkitSinalizacao.Visible = false;
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
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
                    endereco = new MaterialVendaServico.Endereco()
                    {
                        Bairro = txtBairro.Text,
                        CEP = txtCEP.Text.Replace("-", String.Empty),
                        Cidade = txtCidade.Text,
                        Complemento = txtComplemento.Text,
                        Contato = txtContato.Text,
                        DDDFax = RemoverLetras(txtTelefone.Text).Substring(0, 2),
                        DDDTelefone = RemoverLetras(txtTelefone.Text).Substring(0, 2),
                        DescricaoEndereco = txtEndereco.Text,
                        Email = txtEmail.Text,
                        Fax = txtTelefone.Text,
                        Numero = Int32.Parse(txtNumero.Text),
                        Ramal = "0",
                        Site = ".",
                        Telefone = txtTelefone.Text,
                        UF = txtEstado.Text
                    };

                    List<MaterialVendaServico.Kit> kits = new List<MaterialVendaServico.Kit>();

                    //Os Kits serão incluídos conforme Cadastro do estabelecimento no GS...
                    //Para Cada Solicitação de materialm serão efetuados pedidos de todos os meteriais daquele tipo(s) escolhido(s) na tela.
                    //151 - Kit 6 bobinas
                    //143 - Solicitacao

                    if (chkKitBobina.Checked)
                    {
                        MaterialVendaServico.Kit[] kitsVendas = CarregarMateriaisVenda();

                        if (kitsVendas != null)
                        {
                            kits.AddRange(kitsVendas);
                        }
                    }

                    if (chkKitSinalizacao.Checked)
                    {
                        MaterialVendaServico.Kit[] kitsSinalizacao = CarregarKitsSinalizacao();
                        if (kitsSinalizacao != null)
                        {
                            kits.AddRange(kitsSinalizacao);
                        }
                    }

                    MaterialVendaServico.Motivo motivo = ConsultarMotivo();

                    kits.ForEach(f =>
                    {
                        f.Quantidade = 1;
                        f.Motivo = motivo;
                    });

                    Int32 codigoRetorno;
                    using (var _client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
                    {
                        codigoRetorno = _client.Cliente.IncluirKit(kits.ToArray(), this.SessaoAtual.CodigoEntidade, this.SessaoAtual.NomeEntidade, this.SessaoAtual.LoginUsuario, this.SessaoAtual.NomeUsuario, true, endereco);
                        if (codigoRetorno == 0)
                        {
                            ExibirPainelSucesso();
                            this.ReloadTela();
                            this.CarregarSugestaoEndereco();
                        }
                        else
                        {
                            this.ExibirErro(NomeServicoIncluir, codigoRetorno);
                        }

                        //Registro no histórico/log de atividades
                        Historico.RealizacaoServico(SessaoAtual, "Solicitação de Material de Venda");
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
        /// Exibir painel de erro no controle
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="Codigo"></param>
        private void ExibirErro(String fonte, Int32 Codigo)
        {
            if (!IndicaErro)
            {
                IndicaErro = true;
                base.ExibirPainelExcecao(fonte, Codigo);
            }
        }

        /// <summary>
        /// Exibir painel de sucesso da inclusão para uma dada solicitação de material.
        /// </summary>
        private void ExibirPainelSucesso()
        {
            this.chkKitBobina.Checked = false;
            this.chkKitSinalizacao.Checked = false;
            this.pnlSolicitarMaterial.Attributes.Add("style", "display: none;");
            this.pnlSucessoSolicitacao.Visible = true;
            if (this.pnlSucessoSolicitacao.HasAttributes && this.pnlSucessoSolicitacao.Attributes["style"] != null)
            {
                this.pnlSucessoSolicitacao.Attributes.Remove("style");
            }

            this.hdnSuccessPanel.Value = true.ToString();
        }

        /// <summary>
        /// Consulta motivo da solicitação material
        /// </summary>
        private MaterialVendaServico.Motivo ConsultarMotivo() {
            MaterialVendaServico.Motivo[] motivos;

            using (var client = new ContextoWCF<MaterialVendaServico.MaterialVendaServicoClient>())
            {
                motivos = client.Cliente.ConsultarMotivos();
            }

            return motivos != null && motivos.Any() ? motivos.First() : null;
        }

        #region [Utils]
        /// <summary>
        /// Remove espaços para determinada string.
        /// </summary>
        private String RemoverEspacos(String valor)
        {
            if (String.IsNullOrWhiteSpace(valor))
                valor = String.Empty;

            return valor.Trim();
        }

        /// <summary>
        /// Formata o telefone para carregamento.
        /// </summary>
        private String FormatarTelefone(String numero)
        {
            //limpa a string, mantendo apenas os números
            numero = RemoverLetras(numero);

            if (numero.Length > 11)
            {
                numero = String.Compare(numero.Substring(2, 1), "9", true) == 0 ? numero.Substring(0, 11) : numero.Substring(0, 10);
            }

            //trata as 4 situações, baseando-se na quantidade de caracteres na string:
            // 10 caracteres:   DDD + 8 dígitos
            // 11 caracteres:   DDD + 9 dígitos
            // 9 caracteres:    9 dígitos
            // 8 caracteres:    8 dígitos

            switch (numero.Length)
            {
                case 8:     // sem DDD, 8 dígitos
                    numero = Regex.Replace(numero, @"(\d{4})(\d{4})", "(00) $1-$2");
                    break;

                case 9:     // sem DDD, 9 dígitos
                    numero = Regex.Replace(numero, @"(\d{5})(\d{4})", "(00) $1-$2");
                    break;

                case 10:    // com DDD, 8 dígitos
                    numero = Regex.Replace(numero, @"(\d{2})(\d{4})(\d{4})", "($1) $2-$3");
                    break;

                case 11:    // com DDD, 9 dígitos
                    numero = Regex.Replace(numero, @"(\d{2})(\d{5})(\d{4})", "($1) $2-$3");
                    break;

                default:
                    numero = String.Empty;
                    break;
            }

            return numero;
        }

        /// <summary>
        /// Remove letras para possível string a ser convertida para inteiro.
        /// </summary>
        private String RemoverLetras(String valor)
        {
            if (String.IsNullOrWhiteSpace(valor))
                valor = String.Empty;

            return new String(valor.Where(c => Char.IsNumber(c)).ToArray());
        }
        #endregion
    }
}