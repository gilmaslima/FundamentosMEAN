using Redecard.PN.Comum;
using Redecard.PN.GerencieExtrato.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.GerencieExtrato.SharePoint.WebParts.ExtratoInibicao
{

    public partial class ExtratoInibicaoUserControl : UserControlBase
    {
        public class FilialInibicao : Comum.SharePoint.EntidadeServico.Filial
        {
            public String Status { get; set; }
        }
        public class Inibicao
        {
            public int NumeroPV { get; set; }
            public String RetornoServico { get; set; }
            public int CodigoErro { get; set; }
        }
        #region [ Constantes / Propriedades da página ]
        private const string _cPaginaGerenciaExtrato = "/Paginas/pn_GerencieExtratoDefault.aspx";

        public List<FilialInibicao> Filiais
        {
            get
            {
                var ret = ViewState["Filiais"] != null ? (List<FilialInibicao>)ViewState["Filiais"] : new List<FilialInibicao>();
                return ret;
            }
            set
            {
                ViewState["Filiais"] = value;
            }
        }
        #endregion

        #region Eventos da Página
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ReiniciaTela();
                this.ValidaPVLogado();
            }
        }

        protected void btnInibir_Click(object sender, EventArgs e)
        {
            lblMensagem.Text = string.Empty;
            ConsultaPv consultaPv = (ConsultaPv)ucConsultaPV;
            if (consultaPv.PVsSelecionados.Count > 0)
            {
                InibirExtratoPapel();
            }
            else
            {
                if (consultaPv.TipoAssociacao == ConsultaPvTipoAssociacao.Proprio)
                {
                    lblMensagem.Text = "estabelecimento com extrato cancelado.";
                }
                else
                {
                    lblMensagem.Text = "selecione o(s) estabelecimento(s) para cancelar o recebimento de extrato.";
                }
            }
        }

        #endregion

        #region Métodos auxiliares
        void InibirExtratoPapel()
        {
            using (Logger Log = Logger.IniciarLog("Inibição do Extrato Papel"))
            {
                try
                {
                    List<GerencieExtratoServico.DadosPV> lstEntidades = new List<GerencieExtratoServico.DadosPV>();
                    GerencieExtratoServico.DadosPV item;

                    ConsultaPv consultaPV = ucConsultaPV as ConsultaPv;
                    // carrega lista de entidades que terão os recebimentos dos extratos cancelados
                    foreach (Int32 valor in consultaPV.PVsSelecionados)
                    {
                        item = new GerencieExtratoServico.DadosPV();

                        item.PontoVenda = valor;
                        item.Status = "I";
                        item.CodigoOperador = "Internet";
                        item.DDD = 0;
                        item.Telefone = 0;
                        item.Solicitante = SessaoAtual.NomeUsuario;
                        item.IndicadorSituacaoCobranca = "N";
                        item.TextoIsencaoCobranca = string.Empty;
                        item.CodigoErro = "0";
                        item.MensagemErro = string.Empty;
                        lstEntidades.Add(item);
                    }

                    Log.GravarLog(EventoLog.ChamadaServico, lstEntidades);

                    using (GerencieExtratoServico.GerencieExtratoClient client = new GerencieExtratoServico.GerencieExtratoClient())
                    {
                        client.InibirExtPapel(ref lstEntidades);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { lstEntidades });

                    List<Inibicao> listaInibicao = new List<Inibicao>();
                    Inibicao entInibicao;
                    foreach (GerencieExtratoServico.DadosPV dados in lstEntidades)
                    {
                        entInibicao = new Inibicao();
                        entInibicao.NumeroPV = dados.PontoVenda;
                        entInibicao.CodigoErro = dados.CodigoErro.ToInt32();
                        //entInibicao.RetornoServico = dados.MensagemErro;
                        switch (entInibicao.CodigoErro)
                        {
                            case 0:
                                entInibicao.RetornoServico = "extrato papel cancelado com sucesso.";
                                break;
                            case 10:
                                entInibicao.RetornoServico = "extrato papel já cancelado.";
                                break;
                            case 23:
                                entInibicao.RetornoServico = "serviço não cadastrado para o estabelecimento.";
                                break;
                            default:
                                entInibicao.RetornoServico = "erro ao cancelar o extrato papel. acione o escalonamento de contatos...";
                                break;
                        }
                        listaInibicao.Add(entInibicao);

                    }

                    Logger.GravarLog("Entidades listadas para inibição", new { listaInibicao });

                    rptResultado.DataSource = listaInibicao;
                    rptResultado.DataBind();

                    pnlResultado.Visible = (listaInibicao.Count > 0);
                    btnInibir.Visible = !(pnlResultado.Visible);
                    pnlConteudo.Visible = !(pnlResultado.Visible);
                    btnInibir.Enabled = false;

                    //Inclusão no Histórico de Atividades
                    Historico.RealizacaoServico(SessaoAtual, "Cancelamento de Recebimento de Extrato Papel");
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void ReiniciaTela()
        {
            this.pnlQuadroAviso.Visible = false;
            this.pnlResultado.Visible = false;
            this.pnlConteudo.Visible = true;
            this.lblMensagem.Text = string.Empty;
            this.lblCancelado.Text = string.Empty;
            this.pnlLista.Attributes.Add("style", "display:none;");
        }

        void VerificaStatusInibicao(ref List<FilialInibicao> filiais)
        {
            using (Logger Log = Logger.IniciarLog("Verificando status inibição"))
            {
                try
                {
                    List<GerencieExtratoServico.StatusEmissao> lstSolicita = new List<GerencieExtratoServico.StatusEmissao>();
                    GerencieExtratoServico.StatusEmissao item;

                    foreach (FilialInibicao filial in filiais)
                    {
                        item = new GerencieExtratoServico.StatusEmissao();
                        item.PontoVenda = filial.PontoVenda.ToString();
                        item.SituacaoCobranca = " ";
                        item.Status = " ";
                        item.CodigoRetornoPV = 0;
                        lstSolicita.Add(item);
                    }
                    Int16 codigoRetorno = 0;
                    String mensagemRetorno = string.Empty;

                    Log.GravarLog(EventoLog.ChamadaServico, new { lstSolicita });

                    using (GerencieExtratoServico.GerencieExtratoClient client = new GerencieExtratoServico.GerencieExtratoClient())
                    {
                        client.ObterExtratoPapel(ref lstSolicita, ref codigoRetorno, ref mensagemRetorno);
                        if (codigoRetorno > 0)
                        {
                            Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, mensagemRetorno, lstSolicita });
                            this.ExibirPainelExcecao("GerencieExtratoServico.ObterExtratoPapel", codigoRetorno);
                            return;
                        }
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { codigoRetorno, mensagemRetorno, lstSolicita });

                    foreach (GerencieExtratoServico.StatusEmissao itemSolicita in lstSolicita)
                    {
                        FilialInibicao itemFilial = filiais.Where(x => x.PontoVenda == itemSolicita.PontoVenda.ToInt32()).First();
                        if (itemFilial != null)
                        {
                            itemFilial.Status = itemSolicita.Status == String.Empty ? "X" : itemSolicita.Status;
                        }
                    }

                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        bool ValidaPVLogado()
        {
            using (Logger Log = Logger.IniciarLog("Validação do PV logado"))
            {
                int intCentralizador = 0;
                try
                {
                    ConsultaPv consultaPV = ucConsultaPV as ConsultaPv;

                    int codigoRetorno = 0;
                    EntidadeServico.InformacaoPV[] infoPV;

                    Log.GravarLog(EventoLog.ChamadaServico, new { SessaoAtual.CodigoEntidade });

                    using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                    {
                        infoPV = client.ConsultarInformacoesPV(out codigoRetorno, SessaoAtual.CodigoEntidade);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { infoPV, codigoRetorno });

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarInformacoesPV", codigoRetorno);
                        return false;
                    }

                    if (infoPV.Length > 0)
                    {
                        if (infoPV[0].Centralizacao == "1")
                        {
                            ViewState["PVLogadoCentralizador"] = "S";
                        }
                        else
                        {
                            ViewState["PVLogadoCentralizador"] = "N";
                        }

                        switch (infoPV[0].CodigoTipoConsigancao)
                        {
                            case "2":
                                //Consignador
                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                this.SetConsultaPvItemEnabled(3, true);//consignados
                                break;
                            case "1":
                                //Consignatário
                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                this.SetConsultaPvItemEnabled(3, false);//consignados

                                if (this.ProprioCancelado())
                                {
                                    this.pnlConteudo.Visible = false;
                                    this.ExibirQuadroAviso("estabelecimento com extrato cancelado.", TipoQuadroAviso.Erro);
                                    this.SetConsultaPvItemEnabled(0, false);
                                    this.btnInibir.Visible = false;
                                }

                                break;

                            case "N":
                                //Quando não for nem consignatário e nem consignador
                                switch (infoPV[0].CodigoTipoEstabelecimento.ToString())
                                {
                                    case "0":
                                        //Autonomo
                                        this.SetConsultaPvItemEnabled(0, true); //próprio
                                        this.SetConsultaPvItemEnabled(2, false);//centralizado
                                        this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                        this.SetConsultaPvItemEnabled(1, false);//filiais
                                        this.SetConsultaPvItemEnabled(3, false);//consignados

                                        if (this.ProprioCancelado())
                                        {
                                            this.pnlConteudo.Visible = false;
                                            this.ExibirQuadroAviso("estabelecimento com extrato cancelado.", TipoQuadroAviso.Erro);
                                            this.SetConsultaPvItemEnabled(0, false);
                                            this.btnInibir.Visible = false;
                                        }

                                        break;
                                    case "1":
                                        //Filial
                                        switch (infoPV[0].Centralizacao)
                                        {
                                            case "2":
                                                //Centralizado
                                                //----
                                                //Busca do número do estab centralizador para enviar mensagem
                                                //----
                                                intCentralizador = infoPV[0].Centralizador;
                                                infoPV[0] = null;
                                                //----

                                                this.SetConsultaPvItemEnabled(0, false); //próprio
                                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                                this.SetConsultaPvItemEnabled(3, false);//consignados

                                                this.btnInibir.Visible = false;
                                                this.pnlConteudo.Visible = false;
                                                this.ExibirQuadroAviso("acesso não permitido. fazer o cancelamento do extrato por meio do seu estabelecimento centralizador (" + intCentralizador + ").", TipoQuadroAviso.Erro);

                                                break;
                                            //return true;
                                            case "1":
                                                //Centralizadora
                                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                                this.SetConsultaPvItemEnabled(2, true);//centralizado
                                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                                this.SetConsultaPvItemEnabled(3, false);//consignados

                                                break;
                                            default:
                                                //Apenas Filial
                                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                                this.SetConsultaPvItemEnabled(3, false);//consignados

                                                if (this.ProprioCancelado())
                                                {
                                                    this.pnlConteudo.Visible = false;
                                                    this.ExibirQuadroAviso("estabelecimento com extrato cancelado.", TipoQuadroAviso.Erro);
                                                    this.SetConsultaPvItemEnabled(0, false);
                                                    this.btnInibir.Enabled = false;
                                                }

                                                break;
                                        }

                                        break;
                                    case "2":
                                        //Matriz
                                        switch (infoPV[0].Centralizacao)
                                        {
                                            case "1":
                                                //Centralizadora
                                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                                this.SetConsultaPvItemEnabled(2, true);//centralizado
                                                this.SetConsultaPvItemEnabled(4, true);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, true);//filiais
                                                this.SetConsultaPvItemEnabled(3, true);//consignados
                                                break;
                                            case "N":
                                                //Simplesmente Matriz
                                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                                this.SetConsultaPvItemEnabled(4, true);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, true);//filiais
                                                this.SetConsultaPvItemEnabled(3, true);//consignados

                                                break;
                                            case "2":
                                                //Centralizada
                                                this.SetConsultaPvItemEnabled(0, true); //próprio
                                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                                this.SetConsultaPvItemEnabled(4, true);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, true);//filiais
                                                this.SetConsultaPvItemEnabled(3, true);//consignados
                                                break;
                                            default:
                                                //Não identificado
                                                this.SetConsultaPvItemEnabled(0, false); //próprio
                                                this.SetConsultaPvItemEnabled(2, false);//centralizado
                                                this.SetConsultaPvItemEnabled(4, false);//mesmo cnpj
                                                this.SetConsultaPvItemEnabled(1, false);//filiais
                                                this.SetConsultaPvItemEnabled(3, false);//consignados

                                                this.btnInibir.Enabled = false;
                                                this.pnlConteudo.Visible = false;
                                                this.ExibirQuadroAviso("estabelecimento inválido.", TipoQuadroAviso.Erro);

                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    pnlFiltroPvs.Visible = this.GetConsultaPvItemEnabled(0).GetValueOrDefault(false);

                    return true;
                }
                catch (FaultException<GerencieExtratoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        private bool ProprioCancelado()
        {
            List<FilialInibicao> FilialVerificacao = new List<FilialInibicao>();
            FilialVerificacao.Add(new FilialInibicao()
            {
                PontoVenda = SessaoAtual.CodigoEntidade,
                NomeComerc = SessaoAtual.CodigoEntidade + " - O PRÓPRIO"
            });

            VerificaStatusInibicao(ref FilialVerificacao);

            if (FilialVerificacao.Where(x => x.Status == "E").Count() > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Método auxiliar para exibição de mensagem na tela
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        /// <param name="tipoAviso"></param>
        private void ExibirQuadroAviso(string mensagem, TipoQuadroAviso tipoAviso)
        {
            this.pnlQuadroAviso.Visible = true;
            this.quadroAviso.Titulo = string.Empty; ;
            this.quadroAviso.Mensagem = mensagem;
            this.quadroAviso.TipoQuadro = tipoAviso;
        }

        /// <summary>
        /// Atribui enabled (true/false) para algum item do DropDownList do ConsultaPv
        /// </summary>
        /// <param name="value">Valor para localização do item no DropDownList do ConsultaPv</param>
        /// <param name="enabled">Valor (true/false) para o estado de enabled do item</param>
        private void SetConsultaPvItemEnabled(int value, bool enabled)
        {
            ConsultaPv consultaPv = (ConsultaPv)ucConsultaPV;
            if (consultaPv == null)
                return;

            var item = consultaPv.DropDownList.Items.FindByValue(value.ToString());
            if (item == null)
                return;

            item.Enabled = enabled;
        }

        /// <summary>
        /// Obtém o estado de enabled de algum item do DropDownList do ConsultaPv
        /// </summary>
        /// <param name="value">Valor para localização do item no DropDownList do ConsultaPv</param>
        /// <returns>Valor (true/false) em que se encontra o estado do item</returns>
        private bool? GetConsultaPvItemEnabled(int value)
        {
            ConsultaPv consultaPv = (ConsultaPv)ucConsultaPV;
            if (consultaPv == null)
                return null;

            var item = consultaPv.DropDownList.Items.FindByValue(value.ToString());
            if (item == null)
                return null;

            return item.Enabled;
        }

        #endregion
    }
}
