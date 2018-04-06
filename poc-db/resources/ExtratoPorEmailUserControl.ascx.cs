using Redecard.PN.Comum;
using Redecard.PN.GerencieExtrato.Core.Web.Controles.Portal;
using Redecard.PN.GerencieExtrato.SharePoint.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.GerencieExtrato.SharePoint.WebParts.ExtratoPorEmail
{
    public partial class ExtratoPorEmailUserControl : UserControlBase
    {
        #region [ Propriedades da página ]
        /// <summary>Ação da página</summary>
        public enum Acao
        {
            Consultar,
            Excluir,
            Atualizar
        }

        /// <summary>Ação retornada</summary>
        public String AcaoEmail
        {
            get
            {
                if (ViewState["AcaoEmail"] == null)
                    ViewState["AcaoEmail"] = "";
                return ViewState["AcaoEmail"].ToString();
            }
            set { ViewState["AcaoEmail"] = value; }
        }

        /// <summary>Frase retornada</summary>
        public String FraseCriptografada
        {
            get
            {
                if (ViewState["FraseCriptografada"] == null)
                    ViewState["FraseCriptografada"] = "";
                return ViewState["FraseCriptografada"].ToString();
            }
            set { ViewState["FraseCriptografada"] = value; }
        }

        /// <summary>Quantidade de PVs</summary>
        public Int32 QuantidadePVs
        {
            get
            {
                if (ViewState["QuantidadePVs"] == null)
                    ViewState["QuantidadePVs"] = 0;
                return (int)ViewState["QuantidadePVs"];
            }
            set { ViewState["QuantidadePVs"] = value; }
        }
        #endregion
        private const string _cPaginaGerenciaExtrato = "/Paginas/pn_GerencieExtratoDefault.aspx";

        #region [ Eventos da Página ]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Busca os dados da página
                ExecutarDadosEmail(Acao.Consultar);
            }
        }

        /// <summary>Seleciona os PVs</summary>
        /// <param name="filiais">Filiais</param>
        //protected void SelecaoPV_SelectedItemsChanged(Comum.SharePoint.EntidadeServico.Filial[] filiais)
        //{
        //    if (filiais.Length > 0)
        //    {
        //        listaFiliais.Items.Clear();

        //        listaFiliais.DataSource = filiais;
        //        listaFiliais.DataTextField = "NomeComerc";
        //        listaFiliais.DataValueField = "PontoVenda";
        //        listaFiliais.DataBind();
        //    }
        //}

        //protected void ddlConjuntoEstabelecimento_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    lblMensagemSelecionados.Text = string.Empty;

        //    Int32 tipoEntidade = ddlConjuntoEstabelecimento.SelectedValue.ToInt32();
        //    if (tipoEntidade > 0)
        //    {
        //        Int32 qtdFiliais = (ucSelecaoPV as SelecaoPV).CarregarPvs(tipoEntidade);

        //        //Somente mostra se possuir filiais
        //        divFiliais.Visible = qtdFiliais > 0;

        //        if (qtdFiliais == 0)
        //        {
        //            switch (tipoEntidade)
        //            {
        //                case 1: //Centralizados
        //                    lblMensagemSelecionados.Text = "Estabelecimento não possui Centralizados";
        //                    break;
        //                case 2: //Filiais
        //                    lblMensagemSelecionados.Text = "Estabelecimento não possui Filiais";
        //                    break;
        //                case 3: //Consignados
        //                    lblMensagemSelecionados.Text = "Estabelecimento não possui Consignados";
        //                    break;
        //                case 4: //Mesmo CNPJ
        //                    lblMensagemSelecionados.Text = "Estabelecimento não possui outros PVs com mesmo CNPJ";
        //                    break;
        //            }
        //        }
        //    }
        //    if (tipoEntidade == 0)
        //    {
        //        listaFiliais.Items.Clear();
        //        divFiliais.Visible = false;

        //    }
        //}

        /// <summary>Handler do botão voltar, redireciona para a tela de Gerenciar Extratos</summary>        
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Atualização dos dados do e-mail"))
            {
                //Atualiza o item
                ExecutarDadosEmail(Acao.Atualizar);
            }
        }

        /// <summary>Handler do botão voltar, redireciona para a tela de Gerênciar Extratos</summary>        
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Exclusão dos dados do e-mail"))
            {
                //Exclui o item
                ExecutarDadosEmail(Acao.Excluir);
            }
        }
        #endregion

        #region [ Métodos Auxiliares ]
        /// <summary>Executa a ação de consulta/atualização/inclusão e exclusão dos dados de e-mail</summary>
        /// <param name="acao">Ação a ser executada</param>
        public void ExecutarDadosEmail(Acao acao)
        {
            using (Logger Log = Logger.IniciarLog("Ação de consulta/atualização/inclusão/exclusão dos dados de e-mail"))
            {
                try
                {
                    //Busca os dados do e-mail
                    List<GerencieExtratoServico.CadeiaPV> listaPVsSelecionados = new List<GerencieExtratoServico.CadeiaPV>();
                    String acaoExecucao = TipoOperacao(acao);
                    String acaoRetorno = "";
                    Int32 codigoEstabelecimento = base.SessaoAtual.CodigoEntidade;
                    Decimal cnpjSolicitante = base.SessaoAtual.CNPJEntidade.ToDecimal();
                    String periodicidadeEnvio = "";
                    String diaEnvio = "";
                    String tipoPVSolicitante = "";
                    String tipoSolicitacao = "";
                    String nomeUsuario = "";
                    String nomeEmailrecebimento = "";
                    String fraseCriptografada = "";
                    List<String> codigoBoxes = new List<String>();
                    String quantidadePvs = "0";
                    String identificadorContinuacao = "";
                    String mensagemErro = "";
                    Int32 codigoErro = 0;
                    ConsultaPv consultaPV = ucConsultaPV as ConsultaPv;
                    //Caso seja uma operação de Atualização/Inclusão ou Exclusão, preenche as variáveis para execução
                    if (acao != Acao.Consultar)
                    {
                        //Busca os dados para atualizar
                        periodicidadeEnvio = rblFrequencia.SelectedValue;
                        diaEnvio = rblFrequencia.SelectedValue == "S" ? ddlDiaSemana.SelectedValue : "";
                        nomeEmailrecebimento = rbpEmailReceb.Checked ? txtEmail.Text : SessaoAtual.EmailEntidade;
                        nomeUsuario = SessaoAtual.NomeUsuario;
                        tipoPVSolicitante = ((Int32)consultaPV.TipoAssociacao).ToString();   //ddlConjuntoEstabelecimento.SelectedValue;                                  //Estabelecimento Selecionado
                        tipoSolicitacao = listaFiliais.Items.Count >= QuantidadePVs ? "T" : "P";                     //Itens Selecionados T- Todos / P Parcial
                        fraseCriptografada = txtSenha.Text == "**************************************************" || txtSenha.Text.Trim() == "" ?  /*Define a Senha atual*/
                                                        FraseCriptografada :
                                                        txtSenha.Text;

                        ConsultaPv _controleConsulta = ucConsultaPV as ConsultaPv;
                        if (!object.ReferenceEquals(_controleConsulta, null))
                        {
                            if (_controleConsulta.TipoAssociacao == ConsultaPvTipoAssociacao.Proprio)
                                listaPVsSelecionados.Add(new GerencieExtratoServico.CadeiaPV() { NumeroPV = this.SessaoAtual.CodigoEntidade.ToString() });
                            else
                            {
                                //Busca a lista de itens selecionados
                                foreach (Int32 numeroPV in _controleConsulta.PVsSelecionados)
                                    listaPVsSelecionados.Add(new GerencieExtratoServico.CadeiaPV() { NumeroPV = numeroPV.ToString() });
                            }
                        }
                    }

                    //Completa a lista de itens para enviar para o servidor (Caso contrário, irá retornar erro)
                    while (listaPVsSelecionados.Count < 1200)
                        listaPVsSelecionados.Add(new GerencieExtratoServico.CadeiaPV() { NumeroPV = "0" });

                    //Completa a lista de boxes para enviar para o servidor (Caso contrário, irá retornar erro)
                    while (codigoBoxes.Count < 30)
                        codigoBoxes.Add("0");

                    Log.GravarLog(EventoLog.ChamadaServico, new
                    {
                        acaoExecucao,
                        codigoEstabelecimento,
                        cnpjSolicitante,
                        periodicidadeEnvio,
                        diaEnvio,
                        tipoPVSolicitante,
                        tipoSolicitacao,
                        nomeUsuario,
                        nomeEmailrecebimento,
                        fraseCriptografada,
                        listaPVsSelecionados,
                        codigoBoxes,
                        codigoErro,
                        mensagemErro,
                        quantidadePvs,
                        identificadorContinuacao,
                        acaoRetorno
                    });

                    //Executa a operação no extrato
                    using (GerencieExtratoServico.GerencieExtratoClient extrato = new GerencieExtratoServico.GerencieExtratoClient())
                    {
                        extrato.Extrato_Email(ref acaoExecucao,
                                                ref codigoEstabelecimento,
                                                ref cnpjSolicitante,
                                                ref periodicidadeEnvio,
                                                ref diaEnvio,
                                                ref tipoPVSolicitante,
                                                ref tipoSolicitacao,
                                                ref nomeUsuario,
                                                ref nomeEmailrecebimento,
                                                ref fraseCriptografada,
                                                ref listaPVsSelecionados,
                                                ref codigoBoxes,
                                                ref codigoErro,
                                                ref mensagemErro,
                                                ref quantidadePvs,
                                                ref identificadorContinuacao,
                                                ref acaoRetorno);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new
                    {
                        acaoExecucao,
                        codigoEstabelecimento,
                        cnpjSolicitante,
                        periodicidadeEnvio,
                        diaEnvio,
                        tipoPVSolicitante,
                        tipoSolicitacao,
                        nomeUsuario,
                        nomeEmailrecebimento,
                        fraseCriptografada,
                        listaPVsSelecionados,
                        codigoBoxes,
                        codigoErro,
                        mensagemErro,
                        quantidadePvs,
                        identificadorContinuacao,
                        acaoRetorno
                    });

                    //Caso tenha ocorrido erro na chamada, dispara uma exceção(22 = novo registro, não é erro)
                    if (codigoErro > 0 && codigoErro != 22)
                        throw new ExcecaoWs(codigoErro, "GerencieExtratoClient.Extrato_Email", mensagemErro);

                    //Define a ação de retorno atual (se deve ser Inclusão ou Atualização)
                    AcaoEmail = acaoRetorno;

                    //Caso seja operação de Consulta, preenche os dados da página
                    if (acao == Acao.Consultar)
                    {
                        //Verifica se possui e-mail
                        bool possuiEmailReceb = !string.IsNullOrEmpty(nomeEmailrecebimento) && nomeEmailrecebimento.Trim() != "";

                        //Define as informações de e-mail
                        if (possuiEmailReceb)
                            txtEmail.Text = nomeEmailrecebimento;

                        if (!string.IsNullOrEmpty(SessaoAtual.EmailEntidade))
                        {
                            lblEmail.Text = SessaoAtual.EmailEntidade;
                            divEmailPadrao.Visible = true;
                        }
                        else
                            divEmailPadrao.Visible = false;

                        //Verifica qual e-mail deve estar selecionado
                        rbpEmailReceb.Checked = possuiEmailReceb;
                        rbtEmailPadrao.Checked = !possuiEmailReceb && !string.IsNullOrEmpty(SessaoAtual.EmailEntidade);

                        //Define o valor da senha
                        if (!string.IsNullOrEmpty(fraseCriptografada))
                            FraseCriptografada = fraseCriptografada;

                        //Seleciona os valores 
                        SelecionarItem(rblFrequencia, periodicidadeEnvio);              //Frequencia
                        SelecionarItem(ddlDiaSemana, diaEnvio);                         //Dia da semana

                        //No código original, o tipo de PV não é selecionado por padrão. Está sendo mantido igual.
                        //SelecionarItem(ddlConjuntoEstabelecimento, tipoPVSolicitante);  //Tipo de PV

                        //Busca a quantidade de pvs atuais
                        QuantidadePVs = listaPVsSelecionados != null ? listaPVsSelecionados.Count : 0;

                        //Esconde as filiais por padrão
                        divFiliais.Attributes.Add("style", "display:none;");

                        //Define os PVs retornados
                        //SelecaoPV selecaoPvs = ucSelecaoPV as SelecaoPV;


                        var pvsSelecionados = from t0 in listaPVsSelecionados where !string.IsNullOrEmpty(t0.NumeroPV) select RetornaNumero(t0.NumeroPV);
                        consultaPV.PVsSelecionados = (from t0 in pvsSelecionados where t0 != 0 select t0).ToList();
                    }
                    else
                    {
                        //Exibi a mensagem de aviso da confirmação da execução
                        ExibirAviso(@"O email foi cadastrado com sucesso!", true);

                        //Inclusão no Histórico de Atividades
                        Historico.RealizacaoServico(SessaoAtual, "Cancelamento de Recebimento de Extrato Papel");
                        if (acao == Acao.Atualizar)
                            Historico.RealizacaoServico(SessaoAtual, "Extrato por E-mail - Atualização");
                        else if (acao == Acao.Excluir)
                            Historico.RealizacaoServico(SessaoAtual, "Extrato por E-mail - Cadastro");
                    }
                }
                catch (ExcecaoWs ex)
                {
                    Log.GravarErro(ex);
                    ExibirDadosEmail(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Fonte, ex.CodigoErro);
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    ExibirDadosEmail(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    ExibirDadosEmail(false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Converte o valor para número</summary>
        /// <param name="valor">valor</param>
        /// <returns>Número</returns>
        private int RetornaNumero(string valor)
        {
            Int32 retorno;
            if (Int32.TryParse(valor, out retorno))
                return retorno;
            else
                return 0;
        }

        /// <summary>Busca o texto da operação</summary>
        /// <param name="acao">Ação</param>
        /// <returns>Operação</returns>
        private String TipoOperacao(Acao acao)
        {
            switch (acao)
            {
                case Acao.Consultar:
                    return "C";
                case Acao.Excluir:
                    return "E";
                case Acao.Atualizar:
                    return AcaoEmail;
            }
            return "";
        }

        /// <summary>Se deve exibir os dados de e-mail</summary>
        /// <param name="exibir">Exibir</param>
        private void ExibirDadosEmail(bool exibir)
        {
            pnlConteudo.Visible = exibir;
            pnlConteudoExtrato.Visible = exibir;
            pnlBotoes.Visible = exibir;
        }

        /// <summary>Exibe a mensagem do quadro de aviso</summary>
        /// <param name="titulo">Título</param>
        /// <param name="mensagem">Mensagem</param>
        private void ExibirAviso(String mensagem, bool esconderDadosEmail, TipoQuadroAviso tipoAviso = TipoQuadroAviso.Sucesso)
        {
            //Mostra o quadro de aviso
            pnlQuadroAviso.Visible = true;

            //Define a mensagem do Quadro de aviso
            this.qdAvisoSemExtrato.Visible = true;
            this.qdAvisoSemExtrato.Mensagem = mensagem;
            this.qdAvisoSemExtrato.TipoQuadro = tipoAviso;

            //Esconde os dados do extrato
            if (esconderDadosEmail)
                ExibirDadosEmail(false);
        }

        /// <summary>Seleciona o item do controle</summary>
        /// <param name="controle">Controle</param>
        /// <param name="valor">Valor</param>
        private void SelecionarItem(ListControl controle, String valor)
        {
            controle.ClearSelection();
            if (!string.IsNullOrEmpty(valor) && valor.Trim() != "" && controle.Items.FindByValue(valor) != null)
                controle.Items.FindByValue(valor).Selected = true;
        }
        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Concat(base.web.ServerRelativeUrl, _cPaginaGerenciaExtrato));
        }
    }
}
