/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Redecard.PN.FMS.Sharepoint.Modelo;
using System.Collections.Generic;
using Redecard.PN.FMS.Sharepoint.Interfaces;


namespace Redecard.PN.FMS.Sharepoint.WebParts.ProdutividadeConsolidada
{
    /// <summary>
    /// Consulta produtividade consolidada por data ou analista.
    /// </summary>
    public partial class ProdutividadeConsolidadaUserControl : RelatorioGridBaseUserControl<CriterioOrdemProdutividade>, IPossuiExportacao
    {
        #region Atributos e propriedades
        private ItemTabelaProdutividadeConsolidada TotalGeral { get; set; }
        #endregion

        #region Eventos
        /// <summary>
        /// Evento que irá ocorrer ao carregar a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaListaUsuarios();
            }
        }
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão buscar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                base.MontaGridInicial();
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        /// <summary>
        /// Evento que irá ocorrer ao gravar dados no data row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ItemTabelaProdutividadeConsolidada item = (ItemTabelaProdutividadeConsolidada)e.Row.DataItem;

                if (item.RowSpan > 1)
                {
                    e.Row.Cells[0].RowSpan = item.RowSpan;
                }
                else
                {
                    e.Row.Cells[0].Visible = false;
                }

                switch (item.TipoItem)
                {
                    case ItemTabelaProdutividadeConsolidada.TipoItemTabela.SubTotal:
                        {
                            e.Row.Cells[1].Text = "Total";
                            break;
                        }
                }

                e.Row.CssClass += "linha_" + item.TipoItem.ToString();
            }
            else if (e.Row.RowType == DataControlRowType.Footer && this.TotalGeral != null)
            {
                e.Row.Cells[0].Text = "Total Geral";
                e.Row.Cells[0].ColumnSpan = 2;
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Text = this.TotalGeral.QuantidadeCartoesAnalisados.ToString();
                e.Row.Cells[3].Text = this.TotalGeral.QuantidadeCartoesFraudulentos.ToString();
                e.Row.Cells[4].Text = this.TotalGeral.QuantidadeTransacoesFraudulentas.ToString();
                e.Row.Cells[5].Text = this.TotalGeral.ValorFraude.ToString("C");
                e.Row.Cells[6].Text = this.TotalGeral.QuantidadeCartoesNaoFraudulentos.ToString();
                e.Row.Cells[7].Text = this.TotalGeral.QuantidadeTransacoesNaoFraudulentas.ToString();
                e.Row.Cells[8].Text = this.TotalGeral.ValorNaoFraude.ToString("C");
            }

            e.Row.Cells[e.Row.Cells.Count - 1].CssClass += "last";
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Carrega o combo de usuários da página.
        /// </summary>
        protected void CarregaListaUsuarios()
        {
            try
            {
                ddlUsuario.Items.Add(new ListItem() { Text = "Todos", Value = "" });

                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    string[] lisatUsuarios = objClient.PesquisarUsuariosPorEmissor(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);

                    foreach (string usu in lisatUsuarios)
                    {
                        ddlUsuario.Items.Add(new ListItem()
                        {
                            Text = usu,
                            Value = usu
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        /// <summary>
        /// Cria o grid agrupado por data ou analista.
        /// </summary>
        private void PrepararColunasIniciais()
        {
            grvDados.Columns.Clear();

            BoundField colunaAnalista = new BoundField();
            colunaAnalista.HeaderText = "Analista";
            colunaAnalista.DataField = "UsuarioLogin";
            colunaAnalista.HeaderStyle.Width = new Unit("54px");
            colunaAnalista.SortExpression = CriterioOrdemProdutividade.LoginUsuario.ToString();

            BoundField colunaData = new BoundField();
            colunaData.HeaderText = "Data";
            colunaData.DataField = "ObterDataFormatadaRegistro";
            colunaData.HeaderStyle.Width = new Unit("70px");
            colunaData.SortExpression = CriterioOrdemProdutividade.Data.ToString();

            BoundField colunaQtdCartoesAnalisados = new BoundField();
            colunaQtdCartoesAnalisados.HeaderText = "Qtde cartões analisados";
            colunaQtdCartoesAnalisados.DataField = "QuantidadeCartoesAnalisados";
            colunaQtdCartoesAnalisados.HeaderStyle.Width = new Unit("78px");
            colunaQtdCartoesAnalisados.SortExpression = CriterioOrdemProdutividade.QuantidadeCartoesAnalisados.ToString();
            
            BoundField colunaQtdCartoesFraudulentos = new BoundField();
            colunaQtdCartoesFraudulentos.HeaderText = "Qtde cartões fraudulentos";
            colunaQtdCartoesFraudulentos.DataField = "QuantidadeCartoesFraudulentos";
            colunaQtdCartoesFraudulentos.HeaderStyle.Width = new Unit("78px");
            colunaQtdCartoesFraudulentos.SortExpression = CriterioOrdemProdutividade.QuantidadeCartoesFraude.ToString();

            BoundField colunaQtdTransacoesFraudulentas = new BoundField();
            colunaQtdTransacoesFraudulentas.HeaderText = "Qtde transacões fraudulentas";
            colunaQtdTransacoesFraudulentas.DataField = "QuantidadeTransacoesFraudulentas";
            colunaQtdTransacoesFraudulentas.HeaderStyle.Width = new Unit("76px");
            colunaQtdTransacoesFraudulentas.SortExpression = CriterioOrdemProdutividade.QuantidadeTransacoesFraude.ToString();

            BoundField colunaValorTransacoesFraudulentas = new BoundField();
            colunaValorTransacoesFraudulentas.HeaderText = "Valor total fraude";
            colunaValorTransacoesFraudulentas.DataField = "ValorFraude";
            colunaValorTransacoesFraudulentas.HeaderStyle.Width = new Unit("101px");
            colunaValorTransacoesFraudulentas.DataFormatString = "{0:C}";
            colunaValorTransacoesFraudulentas.SortExpression = CriterioOrdemProdutividade.ValorTotalFraude.ToString();
            
            BoundField colunaQtdeCartoesNaoFraudulentos = new BoundField();
            colunaQtdeCartoesNaoFraudulentos.HeaderText = "Qtde cartões não-fraudulentos";
            colunaQtdeCartoesNaoFraudulentos.DataField = "QuantidadeCartoesNaoFraudulentos";
            colunaQtdeCartoesNaoFraudulentos.HeaderStyle.Width = new Unit("76px");
            colunaQtdeCartoesNaoFraudulentos.SortExpression = CriterioOrdemProdutividade.QuantidadeCartoesNaoFraude.ToString();
            
            BoundField colunaTransacoesNaoFraudulentos = new BoundField();
            colunaTransacoesNaoFraudulentos.HeaderText = "Qtde transações não-fraudulentas";
            colunaTransacoesNaoFraudulentos.DataField = "QuantidadeTransacoesNaoFraudulentas";
            colunaTransacoesNaoFraudulentos.HeaderStyle.Width = new Unit("89px");
            colunaTransacoesNaoFraudulentos.SortExpression = CriterioOrdemProdutividade.QuantidadeTransacoesNaoFraude.ToString();
            
            BoundField colunaValorTransacoesNaoFraudulentos = new BoundField();
            colunaValorTransacoesNaoFraudulentos.HeaderText = "Valor total não-fraude";
            colunaValorTransacoesNaoFraudulentos.DataField = "ValorNaoFraude";
            colunaValorTransacoesNaoFraudulentos.HeaderStyle.Width = new Unit("99px");
            colunaValorTransacoesNaoFraudulentos.DataFormatString = "{0:C}";
            colunaValorTransacoesNaoFraudulentos.SortExpression = CriterioOrdemProdutividade.ValorTotalNaoFraude.ToString();
            
            if (rdAnalista.Checked)
            {
                grvDados.Columns.Add(colunaAnalista);
                colunaData.SortExpression = null;
                grvDados.Columns.Add(colunaData);
            }
            else
            {
                grvDados.Columns.Add(colunaData);
                colunaAnalista.SortExpression = null;
                grvDados.Columns.Add(colunaAnalista);
            }

            grvDados.Columns.Add(colunaQtdCartoesAnalisados);
            grvDados.Columns.Add(colunaQtdCartoesFraudulentos);
            grvDados.Columns.Add(colunaQtdTransacoesFraudulentas);
            grvDados.Columns.Add(colunaValorTransacoesFraudulentas);
            grvDados.Columns.Add(colunaQtdeCartoesNaoFraudulentos);
            grvDados.Columns.Add(colunaTransacoesNaoFraudulentos);
            grvDados.Columns.Add(colunaValorTransacoesNaoFraudulentos);
        }

        protected override long MontaGrid(FMSClient objClient, CriterioOrdemProdutividade criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina)
        {
            long returnValue;

            List<ItemTabelaProdutividadeConsolidada> listaItensParaDataGrid = Consultar(objClient, criterioOrdem, ordemClassificacao, out returnValue);

            grvDados.DataSource = listaItensParaDataGrid;
            grvDados.DataBind();

            return returnValue;
        }

        private List<ItemTabelaProdutividadeConsolidada> Consultar(FMSClient objClient, CriterioOrdemProdutividade criterioOrdem, OrdemClassificacao ordemClassificacao, out long returnValue)
        {
            List<ItemTabelaProdutividadeConsolidada> listaItensParaDataGrid = new List<ItemTabelaProdutividadeConsolidada>();

            IntervaloData intervaloData = ValidarData(txtPeridoEspecificoDe, txtPeridoEspecificoAte);

            PesquisarRelatorioProdutividadeEnvio envio = new PesquisarRelatorioProdutividadeEnvio()
            {
                Criterio = criterioOrdem,
                DataFinal = intervaloData.dataFinal,
                DataInicial = intervaloData.dataInicial,
                GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                Ordem = ordemClassificacao,
                Usuario = ddlUsuario.SelectedValue,
                UsuarioLogin = GetSessaoAtual.LoginUsuario
            };

            PrepararColunasIniciais();

            if (rdAnalista.Checked)
            {
                RelatorioProdutividadePorAnalista retornoAnalista = objClient.RelatorioProdutividadePorAnalista(envio);

                MontarItensTabelaProdutividadeConsAnalista(retornoAnalista, listaItensParaDataGrid);
                returnValue = retornoAnalista.QuantidadeTotalRegistros;
            }
            else
            {
                RelatorioProdutividadePorData retornoData = objClient.PesquisarRelatorioProdutividadePorData(envio);

                MontarItensTabelaProdutividadeConsData(retornoData, listaItensParaDataGrid);
                returnValue = listaItensParaDataGrid.Count;
            }

            return listaItensParaDataGrid;
        }

        private void MontarItensTabelaProdutividadeConsAnalista(RelatorioProdutividadePorAnalista retornoAnalista, List<ItemTabelaProdutividadeConsolidada> listaItensParaDataGrid)
        {
            int identificadorAgrupamento = 0;

            foreach (AgrupamentoProdutividadePorAnalista agrupamento in retornoAnalista.AgrupamentoProdutividadePorAnalista)
            {
                bool isFirst = true;

                //DETALHE
                foreach (DetalheProdutividadePorAnalista detalhe in agrupamento.ProdutividadePorAnalista)
                {
                    listaItensParaDataGrid.Add(new ItemTabelaProdutividadeConsolidada()
                    {
                        TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.Detalhe,
                        IdentificadorAgrupamento = identificadorAgrupamento,
                        UsuarioLogin = agrupamento.UsuarioLogin,
                        Data = detalhe.Data,
                        QuantidadeCartoesAnalisados = detalhe.QuantidadeCartoesAnalisados,
                        QuantidadeCartoesFraudulentos = detalhe.QuantidadeCartoesFraudulentos,
                        QuantidadeTransacoesFraudulentas = detalhe.QuantidadeTransacoesFraudulentas,
                        ValorFraude = detalhe.ValorFraude,
                        QuantidadeCartoesNaoFraudulentos = detalhe.QuantidadeCartoesNaoFraudulentos,
                        QuantidadeTransacoesNaoFraudulentas = detalhe.QuantidadeTransacoesNaoFraudulentas,
                        ValorNaoFraude = detalhe.ValorNaoFraude,
                        RowSpan = isFirst ? agrupamento.ProdutividadePorAnalista.Length + 1 : 1
                    });
                    isFirst = false;
                }

                //SUBTOTAL
                listaItensParaDataGrid.Add(new ItemTabelaProdutividadeConsolidada()
                {
                    TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.SubTotal,
                    IdentificadorAgrupamento = identificadorAgrupamento,
                    UsuarioLogin = agrupamento.UsuarioLogin,
                    Data = agrupamento.ProdutividadePorAnalista[0].Data,
                    QuantidadeCartoesAnalisados = agrupamento.QuantidadeTotalCartoesAnalisados,
                    QuantidadeCartoesFraudulentos = agrupamento.QuantidadeTotalCartoesFraudulentos,
                    QuantidadeTransacoesFraudulentas = agrupamento.QuantidadeTotalTransacoesFraudulentas,
                    ValorFraude = agrupamento.ValorTotalFraude,
                    QuantidadeCartoesNaoFraudulentos = agrupamento.QuantidadeTotalCartoesNaoFraudulentos,
                    QuantidadeTransacoesNaoFraudulentas = agrupamento.QuantidadeTotalTransacoesNaoFraudulentas,
                    ValorNaoFraude = agrupamento.ValorTotalNaoFraude,
                    RowSpan = 1
                });

                identificadorAgrupamento++;
            }

            //TOTAL
            ItemTabelaProdutividadeConsolidada total = new ItemTabelaProdutividadeConsolidada()
            {
                TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.Total,
                IdentificadorAgrupamento = retornoAnalista.AgrupamentoProdutividadePorAnalista.Length + 1,
                UsuarioLogin = null,
                Data = DateTime.MinValue,
                QuantidadeCartoesAnalisados = retornoAnalista.QuantidadeTotalCartoesAnalisados,
                QuantidadeCartoesFraudulentos = retornoAnalista.QuantidadeTotalCartoesFraudulentos,
                QuantidadeTransacoesFraudulentas = retornoAnalista.QuantidadeTotalTransacoesFraudulentas,
                ValorFraude = retornoAnalista.ValorTotalFraude,
                QuantidadeCartoesNaoFraudulentos = retornoAnalista.QuantidadeTotalCartoesNaoFraudulentos,
                QuantidadeTransacoesNaoFraudulentas = retornoAnalista.QuantidadeTotalTransacoesNaoFraudulentas,
                ValorNaoFraude = retornoAnalista.ValorTotalNaoFraude,
                RowSpan = 1,
            };
            this.TotalGeral = total;
        }

        private void MontarItensTabelaProdutividadeConsData(RelatorioProdutividadePorData retornoData, List<ItemTabelaProdutividadeConsolidada> listaItensParaDataGrid)
        {
            int identificadorAgrupamento = 0;

            foreach (AgrupamentoProdutividadePorData agrupamento in retornoData.ProdutividadePorData)
            {
                bool isFirst = true;

                //DETALHE
                foreach (ProdutividadePorData detalhe in agrupamento.ProdutividadePorData)
                {
                    listaItensParaDataGrid.Add(new ItemTabelaProdutividadeConsolidada()
                    {
                        TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.Detalhe,
                        IdentificadorAgrupamento = identificadorAgrupamento,
                        UsuarioLogin = detalhe.UsuarioLogin,
                        Data = agrupamento.Data,
                        QuantidadeCartoesAnalisados = detalhe.QuantidadeCartoesAnalisados,
                        QuantidadeCartoesFraudulentos = detalhe.QuantidadeCartoesFraudulentos,
                        QuantidadeTransacoesFraudulentas = detalhe.QuantidadeTransacoesFraudulentas,
                        ValorFraude = detalhe.ValorFraude,
                        QuantidadeCartoesNaoFraudulentos = detalhe.QuantidadeCartoesNaoFraudulentos,
                        QuantidadeTransacoesNaoFraudulentas = detalhe.QuantidadeTransacoesNaoFraudulentas,
                        ValorNaoFraude = detalhe.ValorNaoFraude,
                        RowSpan = isFirst ? agrupamento.ProdutividadePorData.Length + 1 : 1
                    });
                    isFirst = false;
                }

                //SUBTOTAL
                listaItensParaDataGrid.Add(new ItemTabelaProdutividadeConsolidada()
                {
                    TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.SubTotal,
                    IdentificadorAgrupamento = identificadorAgrupamento,
                    UsuarioLogin = agrupamento.ProdutividadePorData[0].UsuarioLogin,
                    Data = agrupamento.Data,
                    QuantidadeCartoesAnalisados = agrupamento.QuantidadeCartoesAnalisados,
                    QuantidadeCartoesFraudulentos = agrupamento.QuantidadeTotalCartoesFraudulentos,
                    QuantidadeTransacoesFraudulentas = agrupamento.QuantidadeTotalTransacoesFraudulentas,
                    ValorFraude = agrupamento.ValorTotalFraude,
                    QuantidadeCartoesNaoFraudulentos = agrupamento.QuantidadeTotalCartoesNaoFraudulentos,
                    QuantidadeTransacoesNaoFraudulentas = agrupamento.QuantidadeTotalTransacoesNaoFraudulentas,
                    ValorNaoFraude = agrupamento.ValorTotalNaoFraude,
                    RowSpan = 1
                });

                identificadorAgrupamento++;
            }

            //TOTAL
            ItemTabelaProdutividadeConsolidada total = new ItemTabelaProdutividadeConsolidada()
            {
                TipoItem = ItemTabelaProdutividadeConsolidada.TipoItemTabela.Total,
                IdentificadorAgrupamento = retornoData.ProdutividadePorData.Length + 1,
                UsuarioLogin = null,
                Data = DateTime.MinValue,
                QuantidadeCartoesAnalisados = retornoData.QuantidadeCartoesAnalisados,
                QuantidadeCartoesFraudulentos = retornoData.QuantidadeTotalCartoesFraudulentos,
                QuantidadeTransacoesFraudulentas = retornoData.QuantidadeTotalTransacoesFraudulentas,
                ValorFraude = retornoData.ValorTotalFraude,
                QuantidadeCartoesNaoFraudulentos = retornoData.QuantidadeTotalCartoesNaoFraudulentos,
                QuantidadeTransacoesNaoFraudulentas = retornoData.QuantidadeTotalTransacoesNaoFraudulentas,
                ValorNaoFraude = retornoData.ValorTotalNaoFraude,
                RowSpan = 1,
            };
            this.TotalGeral = total;
        }

        /// <summary>
        /// Obtém a coluna inicial da ordenação.
        /// </summary>
        /// <returns></returns>
        protected override CriterioOrdemProdutividade ObterCriterioOrdemInicial()
        {
            if (rdAnalista.Checked)
                return CriterioOrdemProdutividade.LoginUsuario;
            else
                return CriterioOrdemProdutividade.Data;
        }

        /// <summary>
        /// Utilizado para identificar onde deverá ser incluído o controle de paginação.
        /// </summary>
        /// <returns></returns>
        protected override Control GetControleOndeColocarPaginacao()
        {
            return divPaginacao;
        }

        /// <summary>
        /// Utilizado para identificar se os parâmetros do sistema deverão ser carregados.
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return true;
        }

        /// <summary>
        /// Download do arquivo excel, contendo os dados exibidos no grid.
        /// </summary>
        public void Exportar()
        {
            long returnValue;

            using (Servico.FMS.FMSClient client = new Servico.FMS.FMSClient())
            {
                List<ItemTabelaProdutividadeConsolidada> registros = Consultar(client, ObterCriterioOrdemInicial(), Servico.FMS.OrdemClassificacao.Ascendente, out returnValue);

                grvDados.AllowSorting = false;
                grvDados.DataSource = registros;
                grvDados.DataBind();

                ExportadorHelper.GerarExcelGridView(grvDados, Response);

                grvDados.AllowSorting = true;
            }
        }
        #endregion
    }
}
