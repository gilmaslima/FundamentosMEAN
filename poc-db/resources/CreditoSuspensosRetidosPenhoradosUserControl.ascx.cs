using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.ControlTemplates;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.RelatorioCreditoSuspensosRetidosPenhorados.CreditoSuspensosRetidosPenhorados
{
    public partial class CreditoSuspensosRetidosPenhoradosUserControl : BaseUserControl
    {
        //#region Atributos
        //private SelecionarPVs objSelecionarPVs;
        //private BuscarRelatorioVendas objBuscar;
        //private Paginacao objPaginacaoSuspensoCredito;
        //private Paginacao objPaginacaoSuspensoDebito;
        //private Paginacao objPaginacaoRetido;
        //#endregion

        //#region Enum
        //private enum EnumTipoSuspensao
        //{
        //    Credito,
        //    Debito
        //}
        //#endregion

        //#region Eventos
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    /*Esconde os dados dos relatorios*/
        //    MostrarResultadoRelatorioSuspenso(false, EnumTipoSuspensao.Credito);
        //    MostrarResultadoRelatorioSuspenso(false, EnumTipoSuspensao.Debito);

        //    //recupera o controle de Selecionar PVs e seta seus eventos
        //    objSelecionarPVs = this.FindControl("SelecionarPVs1") as SelecionarPVs;

        //    //recupera o controle de Busca e seta seus eventos
        //    objBuscar = this.FindControl("BuscarRelatorioVendas1") as BuscarRelatorioVendas;
        //    objBuscar.onBuscar += new BuscarRelatorioVendas.Buscar(objBuscar_onBuscar);
        //    objBuscar.SelecionarPVs = objSelecionarPVs;
        //    objBuscar.RelatorioValorSelecionado = "";//TODO:WILL
        //    objBuscar.TipoVenda = false;

        //    //recupera o controle de Paginação e seta seus eventos e variaveis
        //    objPaginacaoSuspensoCredito = this.FindControl("PaginacaoSuspensoCredito") as Paginacao;
        //    objPaginacaoSuspensoCredito.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacaoSuspensoCredito_onPaginacaoChanged);
        //    objPaginacaoSuspensoCredito.RegistrosPorPagina = Convert.ToInt32(ddlRegistroPorPaginaSuspensoCredito.SelectedValue);

        //    objPaginacaoSuspensoDebito = this.FindControl("PaginacaoSuspensoDebito") as Paginacao;
        //    objPaginacaoSuspensoDebito.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacaoSuspensoDebito_onPaginacaoChanged);
        //    objPaginacaoSuspensoDebito.RegistrosPorPagina = Convert.ToInt32(ddlRegistroPorPaginaSuspensoCredito.SelectedValue);

        //    objPaginacaoRetido = this.FindControl("PaginacaoRetido") as Paginacao;
        //    objPaginacaoRetido.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacaoRetido_onPaginacaoChanged);
        //    objPaginacaoRetido.RegistrosPorPagina = Convert.ToInt32(ddlRegistroPorPaginaRetido.SelectedValue);
        //}

        //private void objPaginacaoRetido_onPaginacaoChanged(int UltimoRegistro, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void objPaginacaoSuspensoCredito_onPaginacaoChanged(int UltimoRegistro, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void objPaginacaoSuspensoDebito_onPaginacaoChanged(int UltimoRegistro, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void objBuscar_onBuscar(BuscarDados Dados, EventArgs e)
        //{
        //    ConsultarSuspensao(Dados, EnumTipoSuspensao.Credito, ObterQuantidadeRegistrosPagina());
        //    ConsultarSuspensao(Dados, EnumTipoSuspensao.Debito, ObterQuantidadeRegistrosPagina());
        //    ConsultarRetencao(Dados, ObterQuantidadeRegistrosPagina());
        //}

        //protected void ddlRegistroPorPaginaSuspensoCredito_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //protected void ddlRegistroPorPaginaSuspensoDebito_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //protected void ddlRegistroPorPaginaRetido_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //protected void ddlRegistroPorPaginaPenhorado_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //protected void grvDadosSuspensoCredito_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //protected void grvDadosSuspensoCredito_RowDataBound(object sender, GridViewRowEventArgs e)
        //{

        //}

        //protected void grvDadosSuspensoDebito_RowDataBound(object sender, GridViewRowEventArgs e)
        //{

        //}

        //#endregion

        //#region Métodos
        //private void ConsultarSuspensao(BuscarDados BuscarDados, EnumTipoSuspensao TipoSuspensao)
        //{
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.StatusRetorno objStatusRetorno;
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoRetorno objRetorno = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoRetorno();
        //    try
        //    {
        //        //cria ou recupera os guids de consulta
        //        if (ViewState["guidPesquisa"] == null)
        //        {
        //            ViewState["guidPesquisa"] = new Guid();
        //            ViewState["guidUsuarioCacheExtrato"] = new Guid();
        //        }

        //        Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract[] objRegistroRetorno;

        //        using (Servico.RelatorioCreditoSuspensosRetidosPenhorados.RelatorioCreditoSuspensosRetidosPenhoradosClient objClient = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.RelatorioCreditoSuspensosRetidosPenhoradosClient())
        //        {
        //            //verifica se a consulta é inicial ou se é paginação
        //            if (BuscarDados.UltimoRegistro >= 0)
        //            {
        //                //a consulta já foi efetuada, e é paginação
        //                objRegistroRetorno = objClient.ConsultarSuspensaoNovaPaginaPesquisa(BuscarDados.UltimoRegistro,
        //                                                                    Convert.ToInt32(ddlRegistroPorPaginaSuspensoCredito.SelectedValue),
        //                                                                    (Guid)ViewState["guidPesquisa"], (Guid)ViewState["guidUsuarioCacheExtrato"]);
        //            }
        //            else
        //            {
        //                objRetorno = objClient.ConsultarSuspensaoPesquisaInicial(out objStatusRetorno,
        //                                                                    TradudorEnvioSPParaServico(BuscarDados, TipoSuspensao),
        //                                                                    Convert.ToInt32(ddlRegistroPorPaginaSuspensoCredito.SelectedValue),
        //                                                                    (Guid)ViewState["guidPesquisa"], (Guid)ViewState["guidUsuarioCacheExtrato"]);

        //                objRegistroRetorno = objRetorno.Registros;

        //                if (EnumTipoSuspensao.Credito == TipoSuspensao)
        //                {
        //                    objPaginacaoSuspensoCredito.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
        //                    objPaginacaoSuspensoCredito.PaginaAtual = 1;
        //                }
        //                else if (EnumTipoSuspensao.Debito == TipoSuspensao)
        //                {
        //                    objPaginacaoSuspensoDebito.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
        //                    objPaginacaoSuspensoDebito.PaginaAtual = 1;
        //                }
        //                ViewState["Totais"] = objRetorno.Totais;
        //            }
        //        }

        //        //mostra os resultados
        //        MostrarResultadoRelatorioSuspenso(true, TipoSuspensao);

        //        //preenche a lista de bancos
        //        //objListBanco = base.GetListaSP(Constantes.Extrato_Lista_Banco);

        //        List<Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract> registros = new List<Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract>(objRegistroRetorno);

        //        if (EnumTipoSuspensao.Credito == TipoSuspensao)
        //        {
        //            grvDadosSuspensoCredito.DataSource = registros.FindAll(ObterRegistrosPorTipoRegistro("DT"));
        //            grvDadosSuspensoCredito.DataBind();

        //            spnTotalValorSuspensoCredito.InnerText = ((Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoTotaisRetorno)ViewState["Totais"]).TotalValorSuspencao.ToString();
        //        }
        //        else if (EnumTipoSuspensao.Debito == TipoSuspensao)
        //        {
        //            grvDadosSuspensoDebito.DataSource = registros.FindAll(ObterRegistrosPorTipoRegistro("DT"));
        //            grvDadosSuspensoDebito.DataBind();

        //            spnTotalValorSuspensoDebito.InnerText = ((Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoTotaisRetorno)ViewState["Totais"]).TotalValorSuspencao.ToString();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //private void ConsultarRetencao(BuscarDados BuscarDados)
        //{
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.StatusRetorno objStatusRetorno;
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoRetorno objRetorno = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoRetorno();
        //    try
        //    {
        //        //cria ou recupera os guids de consulta
        //        if (ViewState["guidPesquisa"] == null)
        //        {
        //            ViewState["guidPesquisa"] = new Guid();
        //            ViewState["guidUsuarioCacheExtrato"] = new Guid();
        //        }

        //        Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract[] objRegistroRetorno;

        //        using (Servico.RelatorioCreditoSuspensosRetidosPenhorados.RelatorioCreditoSuspensosRetidosPenhoradosClient objClient = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.RelatorioCreditoSuspensosRetidosPenhoradosClient())
        //        {
        //            //verifica se a consulta é inicial ou se é paginação
        //            if (BuscarDados.UltimoRegistro >= 0)
        //            {
        //                //a consulta já foi efetuada, e é paginação
        //                objRegistroRetorno = objClient.ConsultarRetencaoNovaPaginaPesquisa(BuscarDados.UltimoRegistro,
        //                                                                    Convert.ToInt32(ddlRegistroPorPaginaRetido.SelectedValue),
        //                                                                    (Guid)ViewState["guidPesquisa"], (Guid)ViewState["guidUsuarioCacheExtrato"]);
        //            }
        //            else
        //            {
        //                objRetorno = objClient.ConsultarRetencaoPesquisaInicial(out objStatusRetorno,
        //                                                                    TradudorEnvioSPParaServicoRetencao(BuscarDados),
        //                                                                    Convert.ToInt32(ddlRegistroPorPaginaRetido.SelectedValue),
        //                                                                    (Guid)ViewState["guidPesquisa"], (Guid)ViewState["guidUsuarioCacheExtrato"]);

        //                objRegistroRetorno = objRetorno.Registros;

        //                objPaginacaoRetido.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
        //                objPaginacaoRetido.PaginaAtual = 1;

        //                ViewState["Totais"] = objRetorno.Totais;
        //            }
        //        }

        //        //monta o grid com os resultados
        //        MontarGridResultadoRetencao(objRegistroRetorno);

        //        spnTotalValorReter.InnerText = ((Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoTotaisRetorno)ViewState["Totais"]).TotalValorRetencao.ToString();
        //        spnTotalValorRetido.InnerText = ((Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoTotaisRetorno)ViewState["Totais"]).TotalValorProcesso.ToString();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //private Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoEnvio TradudorEnvioSPParaServicoRetencao(BuscarDados BuscarDados)
        //{
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoEnvio objEnvio = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoEnvio();

        //    objEnvio.CodigoBandeira = BuscarDados.CodigoBandeira;
        //    objEnvio.DataFinal = BuscarDados.DataFinal;
        //    objEnvio.DataInicial = BuscarDados.DataInicial;
        //    objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;

        //    return objEnvio;
        //}

        //private void MontarGridResultadoRetencao(Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract[] basicContractList)
        //{
        //    //PR - ConsultarRetencaoNumeroProcessoRetorno
        //    //DC - ConsultarRetencaoDetalheProcessoCreditoRetorno
        //    //DD - ConsultarRetencaoDetalheProcessoDebitoRetorno

        //    //D1 - ConsultarRetencaoDescricaoComValorRetorno
        //    //D2 - ConsultarRetencaoDescricaoSemValorRetorno

        //    foreach (Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract basicContract in basicContractList)
        //    {
        //        string tipoRegistro = basicContract.TipoRegistro;

        //        if (tipoRegistro == "PR")
        //        {
        //            Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoNumeroProcessoRetorno processo = (Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarRetencaoNumeroProcessoRetorno)basicContract;

        //            Table table;
        //            TableRow row;
        //            TableCell cell;

        //            row = new TableHeaderRow();
        //            row.TableSection = TableRowSection.TableHeader;

        //            cell = new TableCell();
        //            cell.Text = "RETENÇÃO - Nº DO PROCESSO:";
        //            row.Cells.Add(cell);

        //            cell = new TableCell();
        //            cell.Text = processo.NumeroProcesso.ToString();
        //            row.Cells.Add(cell);

        //            cell = new TableCell();
        //            cell.Text = "Valor Total a Reter:";
        //            row.Cells.Add(cell);

        //            cell = new TableCell();
        //            cell.Text = processo.ValorTotalProcesso.ToString("0,00");
        //            row.Cells.Add(cell);

        //            table = new Table();
        //            table.CssClass = "dadosComum";
        //            table.Width = Unit.Percentage(100);
        //            table.Style.Add("border", "1px solid #cccccc");
        //            table.Rows.Add(row);

        //            tblRetidoCell.Controls.Add(table);
        //        }
        //    }
        //}

        //private Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoEnvio TradudorEnvioSPParaServico(BuscarDados BuscarDados, EnumTipoSuspensao TipoSuspensao)
        //{
        //    Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoEnvio objEnvio = new Servico.RelatorioCreditoSuspensosRetidosPenhorados.ConsultarSuspensaoEnvio();

        //    objEnvio.CodigoBandeira = BuscarDados.CodigoBandeira;
        //    objEnvio.DataFinal = BuscarDados.DataFinal;
        //    objEnvio.DataInicial = BuscarDados.DataInicial;
        //    objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;

        //    if (EnumTipoSuspensao.Credito == TipoSuspensao)
        //    {
        //        objEnvio.TipoSuspensao = "C";
        //    }
        //    else if (EnumTipoSuspensao.Debito == TipoSuspensao)
        //    {
        //        objEnvio.TipoSuspensao = "D";
        //    }

        //    return objEnvio;
        //}

        ///// <summary>
        ///// Mostra os resultados do relatório
        ///// </summary>
        ///// <param name="FlagMostrar">True = Mostrar, False = Não Mostrar</param>
        //private void MostrarResultadoRelatorioSuspenso(bool FlagMostrar, EnumTipoSuspensao TipoSuspensao)
        //{
        //    if (EnumTipoSuspensao.Credito == TipoSuspensao)
        //    {
        //        if (objPaginacaoSuspensoCredito != null)
        //        {
        //            objPaginacaoSuspensoCredito.Visible = FlagMostrar;
        //        }
        //        grvDadosSuspensoCredito.Visible = FlagMostrar;
        //    }
        //    else if (EnumTipoSuspensao.Debito == TipoSuspensao)
        //    {
        //        if (objPaginacaoSuspensoDebito != null)
        //        {
        //            objPaginacaoSuspensoDebito.Visible = FlagMostrar;
        //        }
        //        grvDadosSuspensoDebito.Visible = FlagMostrar;
        //    }

        //    tblRelatorioValores.Visible = FlagMostrar;
        //}

        //private Predicate<Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract> ObterRegistrosPorTipoRegistro(string tipoRegistro)
        //{
        //    return delegate(Servico.RelatorioCreditoSuspensosRetidosPenhorados.BasicContract obj)
        //    {
        //        return obj.TipoRegistro == tipoRegistro;
        //    };
        //}
        //#endregion
    }
}
