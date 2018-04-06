using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.ServiceModel;
using Redecard.PN.Comum;
using System.Linq.Expressions;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class SelecaoPV : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        #region [ Atributos / Propriedades ]

        //Modo de seleção
        public enum ModoSelecao
        {
            RegistroUnico, //apenas um registro selecionável
            MultiplosRegistros //vários registros selecionáveis
        }

        public const Int32 EstabelecimentoNaoCentralizador = 60007;
        public const Int32 EstabelecimentoNaoMatriz = 60008;
        public const Int32 EstabelecimentoNaoConsignador = 60009;
        public const Int32 EstabelecimentoNaoFilial = 60005;
        public const Int32 EstabelecimentoNaoEncontrado = 60040;
        public const String ErroMensagemServicoRetornoHIS = "Redecard.PN.Extrato.Servicos.HIS.Retorno";

        public Int32[] PVsSelecionados
        {
            get { return ViewState["PVsSelecionados"] == null ? new Int32[0] : (Int32[])ViewState["PVsSelecionados"]; }
            set { ViewState["PVsSelecionados"] = value; }
        }

        /// <summary>
        /// Todas as filiais do PV logado
        /// </summary>
        public EntidadeServico.Filial[] Filiais
        {
            get { return (EntidadeServico.Filial[])ViewState["Filiais"]; }
            set { ViewState["Filiais"] = value; }
        }

        /// <summary>
        /// Filiais selecionadas do PV logado
        /// </summary>
        public EntidadeServico.Filial[] FiliaisSelecionadas
        {
            get
            {
                if (PVsSelecionados != null && PVsSelecionados.Length > 0 && Filiais != null && Filiais.Length > 0)
                    return Filiais.Where(filial => PVsSelecionados.Contains(filial.PontoVenda)).ToArray();
                else
                    return new EntidadeServico.Filial[0];
            }
        }

        /// <summary>
        /// Tipo de associação
        /// </summary>
        public Int32 TipoAssociacao
        {
            get { return (Int32)ViewState["TipoAssociacao"]; }
            private set { ViewState["TipoAssociacao"] = value; }
        }
     
        /// <summary>
        /// Modo de seleção
        /// </summary>
        public ModoSelecao Modo
        {
            get { return (ModoSelecao?)ViewState["Modo"] ?? ModoSelecao.MultiplosRegistros; }
            set { ViewState["Modo"] = value; }
        }

        #endregion

        #region [ Events ]
        
        public delegate void SelectedItemsChangedEventHandler(EntidadeServico.Filial[] filiais);
        /// <summary>
        /// Evento chamado após selecionar as entidades (botão OK)
        /// </summary>
        public event SelectedItemsChangedEventHandler SelectedItemsChanged;

        public delegate List<EntidadeServico.Filial> PosConsultaEventHandler(List<EntidadeServico.Filial> filiais);
        public event PosConsultaEventHandler PosConsulta;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ExibirPopup(false);
        }

        /// <summary>
        /// Método de carga dos PVs associados ao estabelecimento
        /// </summary>
        /// <param name="pFiltro"></param>
        /// <returns>Quantidade de estabelecimentos associados ao PV logado</returns>
        public Int32 CarregarPvs(Int32 tipoAssociacao)
        {
            Int32 codigoRetorno;
            return CarregarPvs(tipoAssociacao, this.SessaoAtual.CodigoEntidade, null, out codigoRetorno);
        }

        /// <summary>
        /// Método de carga dos PVs associados ao estabelecimento
        /// </summary>
        /// <param name="pFiltro">Filtro</param>
        /// <returns>Quantidade de estabelecimentos associados ao PV logado</returns>
        public Int32 CarregarPvs(Int32 tipoAssociacao, Func<EntidadeServico.Filial, bool> filtro)
        {
            Int32 codigoRetorno;
            return CarregarPvs(tipoAssociacao, this.SessaoAtual.CodigoEntidade, filtro, out codigoRetorno);
        }

        /// <summary>
        /// Método de carga dos PVs associados ao estabelecimento
        /// </summary>
        /// <param name="pFiltro">Filtro</param>
        /// <param name="codigoRetorno">Código de retorno da consulta</param>
        /// <param name="filtro">Filtro</param>
        /// <param name="tipoAssociacao">Tipo de Entidade</param>
        /// <returns>Quantidade de estabelecimentos associados ao PV logado</returns>
        public Int32 CarregarPvs(Int32 tipoAssociacao, Func<EntidadeServico.Filial, bool> filtro, out Int32 codigoRetorno)
        {            
            return CarregarPvs(tipoAssociacao, this.SessaoAtual.CodigoEntidade, filtro, out codigoRetorno);
        }

        /// <summary>
        /// Método de carga dos PVs associados ao estabelecimento
        /// </summary>
        /// <param name="tipoAssociacao">Tipo de associacao com o PV</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <returns>Quantidade de estabelecimentos associados ao PV especificado</returns>
        private Int32 CarregarPvs(Int32 tipoAssociacao, Int32 codigoEntidade, Func<EntidadeServico.Filial, bool> filtro, out Int32 codigoRetorno)
        {            
            this.TipoAssociacao = tipoAssociacao;
            codigoRetorno = 999;
            Int32 qtdeFiliais = 0;

            try
            {
                using (EntidadeServico.EntidadeServicoClient objEntidadeClient = new EntidadeServico.EntidadeServicoClient())
                {
                    List<EntidadeServico.Filial> entidades = new List<EntidadeServico.Filial>(objEntidadeClient.ConsultarFiliais(out codigoRetorno, codigoEntidade, this.TipoAssociacao));                    

                    // Códigos de sucesso ou código de retorno válido
                    List<int> codigosNaoEncotrado = new List<Int32>(
                        new Int32[]
                        {
                            EstabelecimentoNaoCentralizador,
                            EstabelecimentoNaoConsignador,
                            EstabelecimentoNaoFilial,
                            EstabelecimentoNaoMatriz,
                            EstabelecimentoNaoEncontrado
                        }
                        );

                    if (codigoRetorno != 0)
                    {
                        // Se for um dos códigos esperandos NAO EXIBE painel de excecao
                        if (!codigosNaoEncotrado.Contains(codigoRetorno))
                            base.ExibirPainelExcecao(ErroMensagemServicoRetornoHIS, codigoRetorno);                       
                        return 0;
                    }

                    //Aplica filtro por expression
                    if (filtro != null)
                        entidades = entidades.Where(filtro).ToList();

                    //Chama evento para manipulação geral da consulta de filiais
                    if (PosConsulta != null)
                        entidades = PosConsulta(entidades);
                    
                    // Insere o PV atual na lista
                    // FIXME DAVID - 20/10/2012 - Corrigir a moeda. Deve vir da session de algum parametro que eu ainda nao sei
                    // objListEntidadeTodas.Insert(0, new EntidadeServico.Filial() { PontoVenda = codigoEntidade, NomeComerc = "PRÓPRIO", Categoria = "A", Moeda = "R" });
                    entidades = entidades.OrderBy(p => p.PontoVenda).ToList();

                    //Pega a contagem da lista completa
                    qtdeFiliais = entidades.Count;

                    List<int> filiaisSelecionadas = this.PVsSelecionados.ToList();
                    
                    // Gera os SPAN com checkboxes
                    if (entidades.Count != 0)
                    {
                        Filiais = entidades.ToArray();

                        foreach (EntidadeServico.Filial filial in entidades)
                        {
                            Boolean bDolar = "D".Equals(filial.Moeda, StringComparison.CurrentCultureIgnoreCase);
                            Boolean bAtivo = "A".Equals(filial.Categoria, StringComparison.CurrentCultureIgnoreCase);
                            String id = "filial_" + filial.PontoVenda.ToString();
                            
                            HtmlGenericControl label = new HtmlGenericControl("label");
                            label.Attributes["for"] = id;
                            label.InnerText = filial.PontoVenda.ToString() + " - " + filial.NomeComerc;

                            HtmlControl ctrlChk = null;
                            if (Modo == ModoSelecao.MultiplosRegistros)
                            {
                                HtmlInputCheckBox chk = new HtmlInputCheckBox();
                                chk.Value = filial.PontoVenda.ToString();
                                chk.Checked = filiaisSelecionadas.Contains(filial.PontoVenda);
                                chk.Attributes["id"] = id;
                                chk.Attributes["class"] = "chk_val";
                                chk.Attributes["filial"] = filial.PontoVenda.ToString();
                                chk.Attributes["moeda"] = bDolar ? "dolar" : "real";
                                chk.Attributes["categoria"] = bAtivo ? "ativo" : "cancelado";
                                ctrlChk = chk;
                            }
                            else
                            {
                                HtmlInputRadioButton rbtn = new HtmlInputRadioButton();
                                rbtn.Value = filial.PontoVenda.ToString();
                                rbtn.Checked = filiaisSelecionadas.Contains(filial.PontoVenda);
                                rbtn.Attributes["id"] = id;
                                rbtn.Attributes["class"] = "chk_val";
                                rbtn.Attributes["filial"] = filial.PontoVenda.ToString();
                                rbtn.Attributes["moeda"] = bDolar ? "dolar" : "real";
                                rbtn.Attributes["categoria"] = bAtivo ? "ativo" : "cancelado";
                                rbtn.Name = "chk_val";
                                ctrlChk = rbtn;
                            }                                                        

                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.Controls.Add(ctrlChk);
                            span.Controls.Add(label);

                            if (SessaoAtual.CodigoEntidade == filial.PontoVenda)
                                label.InnerText += " - Próprio";

                            if (bDolar)
                                divPvs_dolar.Controls.Add(span);
                            else
                                divPvs_real.Controls.Add(span);
                        }

                        this.ExibirPopup(true);
                    }
                    else
                        this.ExibirPopup(false);
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {                
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
            }
            catch (Exception ex)
            {                
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }            
            
            return qtdeFiliais;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            //Nenhum PV selecionado
            if (hdnPVsSelecionados.Value == string.Empty)
            {
                this.ExibirPopup(false);
                return;
            }

            //Recupera os PVs selecionados
            PVsSelecionados = hdnPVsSelecionados.Value.Split(',').Select(item => item.ToInt32()).ToArray();
            
            //Invoca o evento após seleção dos PVs, caso o método esteja implementado
            if (SelectedItemsChanged != null)
                SelectedItemsChanged(FiliaisSelecionadas);
            
            //Oculta o popup de seleção de PVs
            this.ExibirPopup(false);            
        }

        private void ExibirPopup(Boolean exibir)
        {
            if (exibir)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ProtecaoExibir", "$('#bgProtecao').show();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelecaoPVExibir", "$('#divTelaSelecionarPV').show();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ProtecaoEsconder", "$('#bgProtecao').hide();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelecaoPVEsconder", "$('#divTelaSelecionarPV').hide();", true);
            }
        }
    }
}
