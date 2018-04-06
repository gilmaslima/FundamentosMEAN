/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/25 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Collections.Generic;
using Redecard.PN.Emissores.Modelos;
using Redecard.PN.Emissores.Negocio;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Servicos.Erro;
using System.ServiceModel;
using AutoMapper;
using System.IO;

namespace Redecard.PN.Emissores.Servicos
{
    /// <summary>
    /// 
    /// </summary>
    public class ServicoPortalEmissores : ServicoBase, IServicoPortalEmissores
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codEmissor"></param>
        /// <param name="mesArquivo"></param>
        /// <param name="anoArquivo"></param>
        /// <returns></returns>
        public byte[] DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo)
        {
            using (Logger Log = Logger.IniciarLog("Serviço DownloadArquivo"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codEmissor, mesArquivo, anoArquivo });
                    return new NegocioEmissores().DownloadArquivo(codEmissor, mesArquivo, anoArquivo);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }

                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DownloadAnual> ListarDownload()
        {
            List<DownloadAnual> anos = new List<DownloadAnual>();
            for (int i = (DateTime.Today.Year - 6); i <= DateTime.Today.Year; i++)
            {
                DownloadAnual d = new DownloadAnual();
                d.Ano = i;
                //d.Meses = new List<DownloadMes>();
                //d.Meses.Add(new DownloadMes { Mes = "Janeiro" });
                //d.Meses.Add(new DownloadMes { Mes = "Fevereiro" });
                //d.Meses.Add(new DownloadMes { Mes = "Março" });
                //d.Meses.Add(new DownloadMes { Mes = "Abril" });
                //d.Meses.Add(new DownloadMes { Mes = "Maio" });
                //d.Meses.Add(new DownloadMes { Mes = "Junho" });
                //d.Meses.Add(new DownloadMes { Mes = "Julho" });
                //d.Meses.Add(new DownloadMes { Mes = "Agosto" });
                //d.Meses.Add(new DownloadMes { Mes = "Setembro" });
                //d.Meses.Add(new DownloadMes { Mes = "Outubro" });
                //d.Meses.Add(new DownloadMes { Mes = "Novembro" });
                //d.Meses.Add(new DownloadMes { Mes = "Dezembro" });
                anos.Add(d);
            }

            return anos;
        }

        /// <summary>
        /// Obtem as Informações do PV
        /// </summary>
        /// <param name="numPV">Número PV</param>
        /// <returns>PV</returns>
        public PontoVenda ObtemPV(int numPV, out int codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ObtemPV"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numPV });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numPV });

                    Modelos.PontoVenda pv = new NegocioEmissores().ObtemPV(numPV, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { pv });

                    Mapper.CreateMap<Modelos.PontoVenda, PontoVenda>();
                    Mapper.CreateMap<Modelos.DadosTelefone, DadosTelefone>();
                    Mapper.CreateMap<Modelos.EnderecoPadrao, EnderecoPadrao>();
                    Mapper.CreateMap<Modelos.DadosProprietario, DadosProprietario>();
                    Mapper.CreateMap<Modelos.DadosBancarios, DadosBancarios>();
                    

                    PontoVenda retorno = Mapper.Map<Modelos.PontoVenda, PontoVenda>(pv);

                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        #region Solicitacao Tecnologia
        /// <summary>
        /// Lista os dados de Solicitação de Tecnologia a partir do número do PV
        /// </summary>
        /// <param name="numPV"></param>
        /// <returns></returns>
        public List<DadosSolicitacaoTecnologia> ConsultarTecnologia(int numPV)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarTecnologia"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numPV });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numPV });

                    List<Modelos.DadosSolicitacaoTecnologia> listaIntegradores = new NegocioEmissores().ConsultarTecnologia(numPV);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaIntegradores });

                    Mapper.CreateMap<Modelos.DadosSolicitacaoTecnologia, DadosSolicitacaoTecnologia>();

                    List<DadosSolicitacaoTecnologia> retorno = Mapper.Map<List<Modelos.DadosSolicitacaoTecnologia>, List<DadosSolicitacaoTecnologia>>(listaIntegradores);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        /// <summary>
        /// Lista os Integradores
        /// </summary>
        /// <returns></returns>
        public List<Integrador> ConsultarIntegrador(string codIntegrador, string situacao)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarIntegrador"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codIntegrador, situacao });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codIntegrador, situacao });

                    List<Modelos.Integrador> listaIntegradores = new NegocioEmissores().ConsultarIntegrador(codIntegrador, situacao);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaIntegradores });

                    Mapper.CreateMap<Modelos.Integrador, Integrador>();

                    List<Integrador> retorno = Mapper.Map<List<Modelos.Integrador>, List<Integrador>>(listaIntegradores);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }

                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }


        }

        /// <summary>
        /// Lista os Equipamentos (Maquinetas)
        /// </summary>
        /// <returns></returns>
        public List<Equipamento> ConsultarEquipamento(out int codigoRetorno, out string mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultarEquipamento"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { });

                    List<Modelos.Equipamento> dadosPV = new NegocioEmissores().ConsultarEquipamento(out codigoRetorno, out mensagemRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { dadosPV });

                    Mapper.CreateMap<Modelos.Equipamento, Equipamento>();

                    List<Equipamento> retorno = Mapper.Map<List<Modelos.Equipamento>, List<Equipamento>>(dadosPV);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }

                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }

        }

        #endregion

        #region Trava Domicilio

        /// <summary>
        /// Consulta de Limite do Emissor
        /// </summary>
        /// <param name="numEmissor">Numero do Emissor</param>
        /// <returns>Valor Limite</returns>
        public decimal ConsultaLimite(int numEmissor, out int codigoRetorno, out string mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaLimite"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor });

                    decimal limite = new NegocioEmissores().ConsultaLimite(numEmissor, out codigoRetorno, out mensagemRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { limite, codigoRetorno, mensagemRetorno });

                    Log.GravarLog(EventoLog.RetornoServico, new { limite, codigoRetorno, mensagemRetorno });
                    return limite;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        /// <summary>
        /// Consulta PV Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        public List<DadosPV> ConsultaPVTravado(int numEmissor, int cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaPVTravado"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, cnpj });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, cnpj });

                    List<Modelos.DadosPV> dadosPV = new NegocioEmissores().ConsultaPVTravado(numEmissor, cnpj);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { dadosPV });

                    Mapper.CreateMap<Modelos.DadosPV, DadosPV>();

                    List<DadosPV> retorno = Mapper.Map<List<Modelos.DadosPV>, List<DadosPV>>(dadosPV);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }

        }

        /// <summary>
        /// Consulta PV Não Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        public List<DadosPV> ConsultaPVNaoTravado(int numEmissor, int cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaPVNaoTravado"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, cnpj });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, cnpj });

                    List<Modelos.DadosPV> dadosPV = new NegocioEmissores().ConsultaPVNaoTravado(numEmissor, cnpj);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { dadosPV });

                    Mapper.CreateMap<Modelos.DadosPV, DadosPV>();

                    List<DadosPV> retorno = Mapper.Map<List<Modelos.DadosPV>, List<DadosPV>>(dadosPV);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }

        }

        /// <summary>
        /// Retorno o totais do PV 
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns></returns>
        public TotaisPV ConsultaTotaisPV(int numEmissor, int cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaTotaisPV"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, cnpj });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, cnpj });

                    Modelos.TotaisPV totaisPV= new NegocioEmissores().ConsultaTotaisPV(numEmissor, cnpj);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { totaisPV });

                    Mapper.CreateMap<Modelos.TotaisPV, TotaisPV>();

                    TotaisPV retorno = Mapper.Map<Modelos.TotaisPV, Servicos.TotaisPV>(totaisPV);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }
        #endregion

        #region Pré-Pagamento
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public List<SaldoPrePagamento> SaldoDetalhadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Serviço SaldoDetalhadoPrePagamento"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, dataInicial, dataFinal });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, dataInicial, dataFinal });

                    List<Modelos.SaldoPrePagamento> listaSaldos = new NegocioEmissores().SaldoDetalhadoPrePagamento(numEmissor, dataInicial, dataFinal);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaSaldos });

                    Mapper.CreateMap<Modelos.SaldoPrePagamento, SaldoPrePagamento>();

                    List<SaldoPrePagamento> retorno = Mapper.Map<List<Modelos.SaldoPrePagamento>, List<SaldoPrePagamento>>(listaSaldos);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public SaldoPrePagamento SaldoConsolidadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Serviço SaldoConsolidadoPrePagamento"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, dataInicial, dataFinal });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, dataInicial, dataFinal });

                    Modelos.SaldoPrePagamento saldo = new NegocioEmissores().SaldoConsolidadoPrePagamento(numEmissor, dataInicial, dataFinal);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { saldo });

                    Mapper.CreateMap<Modelos.SaldoPrePagamento, SaldoPrePagamento>();

                    SaldoPrePagamento retorno = Mapper.Map<Modelos.SaldoPrePagamento, SaldoPrePagamento>(saldo);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));

                }
            }
        }

        /// <summary>
        /// Executa a procedure que ajusta a carga de confirmados
        /// </summary>
        /// <returns></returns>
        public Boolean AjustarCargaConfirmados()
        {
            using (Logger Log = Logger.IniciarLog("Serviço Executa a procedure que ajusta a carga de confirmados"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);
                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    Boolean confirmacao = new NegocioEmissores().AjustarCargaConfirmados();
                    Log.GravarLog(EventoLog.RetornoNegocio, new { confirmacao });

                    Log.GravarLog(EventoLog.RetornoServico, new { confirmacao });
                    return confirmacao;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta os Pré-Pagamentos carregados do KN de acordo com os parâmetros filtrados
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor do Pré-Pagamento. 
        /// Passar como 0 para retornar dos os dados.</param>
        /// <param name="dataInicial">Período de Vencimento inicial dos Pré-Pagamentos</param>
        /// <param name="dataFinal">Período de Vencimento final dos Pré-Pagamentos</param>
        /// <param name="bandeiras">Listagem de bandeiras e código EmissorBandeira a serem filtradas</param>
        /// <returns>Listagem de Pré-Pagamentos retornados</returns>
        public List<PrePagamento> ConsultarPrePagamento(Int32 codigoBacen, DateTime dataInicial, DateTime dataFinal, List<Bandeira> bandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta os Pré-Pagamentos carregados do KN de acordo com os parâmetros filtrados"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Mapper.CreateMap<Modelos.PrePagamento, PrePagamento>();
                    Mapper.CreateMap<Modelos.Emissor, Emissor>();
                    Mapper.CreateMap<Modelos.Bandeira, Bandeira>();
                    Mapper.CreateMap<Modelos.EmissorBandeira, EmissorBandeira>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoBancen = codigoBacen, dataInicial, dataFinal, bandeiras });

                    List<Modelos.Bandeira> bandeirasModelo = new List<Modelos.Bandeira>();
                    Modelos.Bandeira bandeiraModelo;
                    foreach (Bandeira bandeiraServ in bandeiras)
                    {
                        bandeiraModelo = new Modelos.Bandeira();
                        Mapper.CreateMap<EmissorBandeira, Modelos.EmissorBandeira>();
                        Mapper.CreateMap<Bandeira, Modelos.Bandeira>();
                        bandeiraModelo = Mapper.Map<Bandeira, Modelos.Bandeira>(bandeiraServ);
                        bandeirasModelo.Add(bandeiraModelo);
                    }

                    List<Modelos.PrePagamento> prepagamentosModelo = new NegocioEmissores().ConsultarPrePagamento(codigoBacen, dataInicial, dataFinal, bandeirasModelo);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { prepagamentosModelo });

                    List<PrePagamento> prePagamentosServico = new List<PrePagamento>();
                    PrePagamento prePagamentoServico;
                    foreach (Modelos.PrePagamento prePagamentoModelo in prepagamentosModelo)
                    {
                        prePagamentoServico = new PrePagamento();
                        prePagamentoServico = Mapper.Map<Modelos.PrePagamento, PrePagamento>(prePagamentoModelo);
                        prePagamentosServico.Add(prePagamentoServico);
                    }


                    Log.GravarLog(EventoLog.RetornoServico, new { prePagamentosServico });
                    return prePagamentosServico;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados
        /// </summary>
        /// <returns>List of Modelos.Banco Listagem Bancos(Emissores) com Pré-Pagamentos</returns>
        public List<Emissor> ConsultarEmissores()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Mapper.CreateMap<Modelos.Emissor, Emissor>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    List<Modelos.Emissor> emissoresModelo = new List<Modelos.Emissor>();

                    emissoresModelo = new NegocioEmissores().ConsultarEmissores();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { emissoresModelo });

                    List<Emissor> emissoresServico = new List<Emissor>();
                    Emissor emissorServico;
                    foreach (Modelos.Emissor emissorModelo in emissoresModelo)
                    {
                        emissorServico = new Emissor();
                        emissorServico = Mapper.Map<Modelos.Emissor, Emissor>(emissorModelo);
                        emissoresServico.Add(emissorServico);
                    }


                    Log.GravarLog(EventoLog.RetornoServico, new { emissoresServico });
                    return emissoresServico;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor a filtrar as bandeiras</param>
        /// <returns>List of Modelos.Bandeira Listagem de Emissor-Bandeira com Pré-Pagamentos</returns>
        public List<Bandeira> ConsultarEmissoresBandeiras(Int32 codigoBacen)
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Mapper.CreateMap<Modelos.Bandeira, Bandeira>();
                    Mapper.CreateMap<Modelos.EmissorBandeira, EmissorBandeira>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    List<Modelos.Bandeira> emissoresBandeirasModelo = new List<Modelos.Bandeira>();

                    emissoresBandeirasModelo = new NegocioEmissores().ConsultarEmissoresBandeiras(codigoBacen);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { emissoresBandeirasModelo });

                    List<Bandeira> bandeirasServico = new List<Bandeira>();
                    Bandeira emissorBandeiraServico;
                    foreach (Modelos.Bandeira emissorModelo in emissoresBandeirasModelo)
                    {
                        emissorBandeiraServico = new Bandeira();
                        emissorBandeiraServico = Mapper.Map<Modelos.Bandeira, Bandeira>(emissorModelo);
                        bandeirasServico.Add(emissorBandeiraServico);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { bandeirasServico });
                    return bandeirasServico;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta todas as Bandeiras cadastradas no Oracle DR
        /// </summary>
        /// <returns>List of Bandeira </returns>
        public List<Bandeira> ConsultarBandeiras()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todas as Bandeiras cadastradas no Oracle DR"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Mapper.CreateMap<Modelos.Bandeira, Bandeira>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    List<Modelos.Bandeira> bandeirasModelo = new List<Modelos.Bandeira>();

                    bandeirasModelo = new NegocioEmissores().ConsultarBandeiras();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { bandeirasModelo });

                    List<Bandeira> bandeirasServico = new List<Bandeira>();
                    Bandeira bandeiraServico;
                    foreach (Modelos.Bandeira bandeiraModelo in bandeirasModelo)
                    {
                        bandeiraServico = new Bandeira();
                        bandeiraServico = Mapper.Map<Modelos.Bandeira, Bandeira>(bandeiraModelo);
                        bandeirasServico.Add(bandeiraServico);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { bandeirasServico });
                    return bandeirasServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR
        /// </summary>
        /// <returns>List of Banco </returns>
        public List<Emissor> ConsultarBancos()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Mapper.CreateMap<Modelos.Emissor, Emissor>();

                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    List<Modelos.Emissor> bancosModelo = new List<Modelos.Emissor>();

                    bancosModelo = new NegocioEmissores().ConsultarBancos();

                    Log.GravarLog(EventoLog.RetornoNegocio, new { bancosModelo });

                    List<Emissor> bancosServico = new List<Emissor>();
                    Emissor bandeiraServico;
                    foreach (Modelos.Emissor bancoModelo in bancosModelo)
                    {
                        bandeiraServico = new Emissor();
                        bandeiraServico = Mapper.Map<Modelos.Emissor, Emissor>(bancoModelo);
                        bancosServico.Add(bandeiraServico);
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { bancosServico });
                    return bancosServico;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Realiza a carga de Pré-Pagamentos na base do RQ
        /// </summary>
        /// <param name="prePagamentos">Listagem de Pré-Pagamentos a carregar</param>
        /// <param name="confirmados">Indica se os Pré-Pagamentos são do tipo Confirmados ou Agendados/Parcelados.
        /// True - Grava na tabela TBRQ0006; False - Grava na tabela TBRQ0008 </param>
        /// <returns>List of PrePagamento - Listagem de pré-pagamentos que retornaram erro</returns>
        public List<PrePagamento> CarregarPrePagamentos(List<PrePagamento> prePagamentos, Boolean confirmados)
        {
            using (Logger Log = Logger.IniciarLog("Início Carga de Pré-Pagamentos na base do RQ"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);
                    
                    Mapper.CreateMap<PrePagamento, Modelos.PrePagamento>();
                    Mapper.CreateMap<Emissor, Modelos.Emissor>();
                    Mapper.CreateMap<Bandeira, Modelos.Bandeira>();
                    Mapper.CreateMap<EmissorBandeira, Modelos.EmissorBandeira>();
                    Mapper.CreateMap<Modelos.PrePagamento, PrePagamento>();
                    Mapper.CreateMap<Modelos.Emissor, Emissor>();
                    Mapper.CreateMap<Modelos.Bandeira, Bandeira>();
                    Mapper.CreateMap<Modelos.EmissorBandeira, EmissorBandeira>();
                    
                    Mapper.CreateMap<Bandeira, Modelos.Bandeira>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { prePagamentos, confirmados });

                    List<Modelos.PrePagamento> prePagamentosModelo = new List<Modelos.PrePagamento>();
                    Modelos.PrePagamento prePagamentoModelo;
                    foreach (PrePagamento prePagamentoServico in prePagamentos)
                    {
                        prePagamentoModelo = new Modelos.PrePagamento();

                        prePagamentoModelo = Mapper.Map<PrePagamento, Modelos.PrePagamento>(prePagamentoServico);
                        prePagamentosModelo.Add(prePagamentoModelo);
                    }

                    //Carrega os dados e retorna a lista de pré-pagamentos que houveram erro
                    List<Modelos.PrePagamento> prePagamentosErroModelo = new NegocioEmissores().CarregarPrePagamentos(prePagamentosModelo, confirmados);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { prePagamentosErroModelo });

                    List<PrePagamento> prePagamentosErroServico = new List<PrePagamento>();
                    PrePagamento prePagamentoErroServico;
                    foreach (Modelos.PrePagamento prePagamentoErroModelo in prePagamentosErroModelo)
                    {
                        prePagamentoErroServico = new PrePagamento();
                        prePagamentoErroServico = Mapper.Map<Modelos.PrePagamento, PrePagamento>(prePagamentoErroModelo);
                        prePagamentosErroServico.Add(prePagamentoErroServico);
                    }


                    Log.GravarLog(EventoLog.RetornoServico, new { prePagamentosErroServico });
                    return prePagamentosErroServico;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui todos os pré-pagamentos confirmados na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        public Boolean ExcluirPrePagamentosConfirmados()
        {
            using (Logger Log = Logger.IniciarLog("Exclui todos os pré-pagamentos confirmados na base afim de realizar uma nova."))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Boolean confirmacao = new NegocioEmissores().ExcluirPrePagamentos("rq.tbrq0006");

                    Log.GravarLog(EventoLog.RetornoNegocio, new { confirmacao });
                    Log.GravarLog(EventoLog.FimServico, new { confirmacao });

                    return confirmacao;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui todos os pré-pagamentos temporários na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        public Boolean ExcluirPrePagamentosTemporarios()
        {
            using (Logger Log = Logger.IniciarLog("Exclui todos os pré-pagamentos temporários na base afim de realizar uma nova."))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Boolean confirmacao = new NegocioEmissores().ExcluirPrePagamentos("rq.tbrq0007");

                    Log.GravarLog(EventoLog.RetornoNegocio, new { confirmacao });
                    Log.GravarLog(EventoLog.FimServico, new { confirmacao });

                    return confirmacao;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Exclui todos os pré-pagamentos parcelados na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        public Boolean ExcluirPrePagamentosParcelados()
        {
            using (Logger Log = Logger.IniciarLog("Exclui todos os pré-pagamentos parcelados na base afim de realizar uma nova."))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    Log.GravarLog(EventoLog.ChamadaNegocio);
                    Boolean confirmacao = new NegocioEmissores().ExcluirPrePagamentos("rq.tbrq0008");

                    Log.GravarLog(EventoLog.RetornoNegocio, new { confirmacao });
                    Log.GravarLog(EventoLog.FimServico, new { confirmacao });

                    return confirmacao;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <param name="CpfCnpj"></param>
        /// <returns></returns>
        public DadosVendedor ConsultaVendedor(string tipoPessoa, string CpfCnpj)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ConsultaVendedor"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { tipoPessoa, CpfCnpj });
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { tipoPessoa, CpfCnpj });

                    Modelos.DadosVendedor dadosVendedor = new NegocioEmissores().ConsultaVendedor(tipoPessoa, CpfCnpj);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { dadosVendedor });

                    Mapper.CreateMap<Modelos.DadosVendedor, DadosVendedor>();

                    DadosVendedor retorno = Mapper.Map<Modelos.DadosVendedor, DadosVendedor>(dadosVendedor);
                    Log.GravarLog(EventoLog.RetornoServico, new { retorno });
                    return retorno;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


    }
}
