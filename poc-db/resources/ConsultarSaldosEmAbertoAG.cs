using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes.ServicoSaldosEmAberto;
using Redecard.PN.Extrato.Comum.Helper;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarSaldosEmAbertoAG : AgentesBase
    {


        #region Consulta OnLine
        /// <summary>
        /// Retorna os totais por bandeira - WACA1300
        /// </summary>
        /// <param name="statusRetornoDTO">status do retorno</param>
        /// <param name="envio">filtro da consulta</param>
        /// <returns>retorno a classe Modelo.TotaisPorBandeiraSaldosEmAberto</returns>        
        public TotalizadorPorBandeiraSaldosEmAbertoDTO ConsultarTotaisPorBandeira_WACA1300(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarTotaisPorBandeira_WACA1300";

            using (Logger Log = Logger.IniciarLog("Consultar Totais por Bandeira - Saldos em Aberto[WACA1300]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {

                    using (ServicoSaldosEmAberto.SaldosEmAbertoClient client = new ServicoSaldosEmAberto.SaldosEmAbertoClient())
                    {
                        Int16 codigoRetorno = 0;
                        String programaChamador = "WACA1300";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        String reservaDados = string.Empty;
                        String dados = string.Empty;

                        // monta a string de dados para ser enviada ao HIS
                        dados = String.Empty;//envio.Estabelecimentos.Count.ToString().PadLeft(4, '0');
                        foreach (int estabelecimento in envio.Estabelecimentos)
                        {
                            dados += estabelecimento.ToString().PadLeft(9, '0');
                        }
                        dados = dados.PadRight(30000, ' ');

                        Modelo.TotalizadorPorBandeiraSaldosEmAbertoDTO retorno = new Modelo.TotalizadorPorBandeiraSaldosEmAbertoDTO();


                        //consulta o serviço do HIS
                        codigoRetorno = client.TotaisPorBandeira_WACA1300(programaChamador, sistema, ref usuario, dataInicial, dataFinal, ref mensagemRetorno, ref reservaDados, ref dados);


                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);


                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        // converte o resultado na classe de retorno do método
                        retorno = Tradutores.TradutorResultadoConsultaSaldosEmAberto.TraduzirResultadoTotalPorBandeira(dados);

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Efetua a consulta ao HIS no book  - WACA1301
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public ConsultarSaldosEmAbertoDTO ConsultarDetalheMensalSaldosEmAberto_WACA1301(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {

            string FONTE_METODO = this.GetType().Name + ".ConsultarDetalheMensalSaldosEmAberto";
            ConsultarSaldosEmAbertoDTO retorno = new ConsultarSaldosEmAbertoDTO();

            using (Logger Log = Logger.IniciarLog("consultar Detalhe Mensal - Saldos em Aberto[WACA1301]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {

                    using (ServicoSaldosEmAberto.SaldosEmAbertoClient client = new ServicoSaldosEmAberto.SaldosEmAbertoClient())
                    {
                        short codigoRetorno = 0;
                        String programaChamador = "WACA1301";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        short quantidadeTransacoes = 0;
                        short numeroTransacaoEnviada = 0;
                        String reservaDados = string.Empty;
                        String dados = string.Empty;

                        Decimal valorTotalLiquido = 0;


                        do
                        {
                            // monta a string de dados para ser enviada ao HIS
                            dados = String.Empty;// envio.Estabelecimentos.Count.ToString().PadLeft(4, '0');
                            foreach (int estabelecimento in envio.Estabelecimentos)
                            {
                                dados += estabelecimento.ToString().PadLeft(9, '0');
                            }

                            dados = dados.PadRight(30000, ' ');
                            //consulta o serviço do HIS
                            codigoRetorno = client.DetalhamentoPorBandeira_WACA1301(programaChamador, sistema, ref usuario, dataInicial, dataFinal,
                                 ref mensagemRetorno, ref indicadorReChamada, ref quantidadeTransacoes, ref numeroTransacaoEnviada,
                                 ref reservaDados, ref dados);

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);


                            if (codigoRetorno != 0)
                            {
                                return null;
                            }

                            if (object.ReferenceEquals(retorno.Registro, null))
                            {
                                retorno.Registro = new List<BasicDTO>();
                            }
                            // converte o resultado na classe de retorno do método
                            retorno.Registro.AddRange(Tradutores.TradutorResultadoConsultaSaldosEmAberto.TraduzirResultadoDetalhe(dados, ref valorTotalLiquido));
                        } while (indicadorReChamada == "S");

                        retorno.ValorLiquido = valorTotalLiquido;
                        retorno.QuantidadeRegistros = retorno.Registro.Count;

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

        }
        /// <summary>
        /// Efetua a consulta aos servicos WACA1300 e WACA1302
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public ConsultarSaldosEmAbertoDTO ConsultarSaldosemAbertoOnline(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarSaldosemAbertoOnline";
            ConsultarSaldosEmAbertoDTO retorno = new ConsultarSaldosEmAbertoDTO();

            using (Logger Log = Logger.IniciarLog("Consultar Saldos em Aberto Mensal On line - Saldos em Aberto"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    retorno = ConsultarDetalheMensalSaldosEmAberto_WACA1301(out statusRetornoDTO, envio);
                    if (statusRetornoDTO.CodigoRetorno == 0)
                        retorno.Totais = ConsultarTotaisPorBandeira_WACA1300(out statusRetornoDTO, envio);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

        }
        #endregion
        #region Consulta VSAM
        /// <summary>
        /// Efetua a consulta ao HIS no book  - WACA1303
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public TotalizadorPorBandeiraSaldosEmAbertoDTO ConsultarTotaisPorBandeira_WACA1303(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarTotaisPorBandeira_WACA1303";

            using (Logger Log = Logger.IniciarLog("Consultar Totais por Bandeira - Saldos em Aberto[WACA1303]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {

                    using (ServicoSaldosEmAberto.SaldosEmAbertoClient client = new ServicoSaldosEmAberto.SaldosEmAbertoClient())
                    {
                        Int16 codigoRetorno = 0;
                        String programaChamador = "WACA1303";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        String reservaDados = string.Empty;
                        String dados = string.Empty;

                        // monta a string de dados para ser enviada ao HIS
                        dados = String.Empty;// = envio.Estabelecimentos.Count.ToString().PadLeft(4, '0');
                        foreach (int estabelecimento in envio.Estabelecimentos)
                        {
                            dados += estabelecimento.ToString().PadLeft(9, '0') + envio.CodigoSolicitacao.PadLeft(17, '0');
                        }
                        dados = dados.PadRight(30000, ' ');

                        Modelo.TotalizadorPorBandeiraSaldosEmAbertoDTO retorno = new Modelo.TotalizadorPorBandeiraSaldosEmAbertoDTO();


                        //consulta o serviço do HIS
                        codigoRetorno = client.TotalizadoresPorBandeira_WACA1303(programaChamador, sistema, ref usuario, dataInicial, dataFinal, ref mensagemRetorno, ref reservaDados, ref dados);


                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);


                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        // converte o resultado na classe de retorno do método
                        retorno = Tradutores.TradutorResultadoConsultaSaldosEmAberto.TraduzirResultadoTotalPorBandeira(dados);

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Efetua a consulta ao HIS no book  - WACA1304
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public ConsultarSaldosEmAbertoDTO ConsultarDetalheMensalSaldosEmAberto_WACA1304(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {

            string FONTE_METODO = this.GetType().Name + ".ConsultarDetalheMensalSaldosEmAberto_WACA1304";
            ConsultarSaldosEmAbertoDTO retorno = new ConsultarSaldosEmAbertoDTO();

            using (Logger Log = Logger.IniciarLog("consultar Detalhe Mensal - Saldos em Aberto[WACA1304]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {

                    using (ServicoSaldosEmAberto.SaldosEmAbertoClient client = new ServicoSaldosEmAberto.SaldosEmAbertoClient())
                    {
                        short codigoRetorno = 0;
                        String programaChamador = "WACA1304";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        short quantidadeTransacoes = 0;
                        short numeroTransacaoEnviada = 0;
                        String reservaDados = string.Empty;
                        String dados = string.Empty;
                        Decimal valorLiquidoTotal = 0;

                        do
                        {
                            // monta a string de dados para ser enviada ao HIS
                            dados = String.Empty;//envio.Estabelecimentos.Count.ToString().PadLeft(4, '0');
                            foreach (int estabelecimento in envio.Estabelecimentos)
                            {
                                dados += estabelecimento.ToString().PadLeft(9, '0') + envio.CodigoSolicitacao.PadLeft(17, '0');
                            }

                            dados = dados.PadRight(30000, ' ');
                            //consulta o serviço do HIS
                            codigoRetorno = client.DetalhamentoPorBandeira_WACA1304(programaChamador, sistema, ref usuario, dataInicial, dataFinal,
                                 ref mensagemRetorno, ref indicadorReChamada, ref quantidadeTransacoes, ref numeroTransacaoEnviada,
                                 ref reservaDados, ref dados);

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);


                            if (codigoRetorno != 0)
                            {
                                return null;
                            }

                            if (object.ReferenceEquals(retorno.Registro, null))
                            {
                                retorno.Registro = new List<BasicDTO>();
                            }

                            // converte o resultado na classe de retorno do método
                            retorno.Registro.AddRange(Tradutores.TradutorResultadoConsultaSaldosEmAberto.TraduzirResultadoDetalhe(dados, ref valorLiquidoTotal));
                        } while (indicadorReChamada == "S");

                        retorno.ValorLiquido = valorLiquidoTotal;
                        retorno.QuantidadeRegistros = retorno.Registro.Count;

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

        }
        /// <summary>
        /// Efetua a consulta aos serviços WACA1303 e WACA1304
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public ConsultarSaldosEmAbertoDTO ConsultarSaldosemAbertoVsam(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarSaldosemAbertoVsam";
            ConsultarSaldosEmAbertoDTO retorno = new ConsultarSaldosEmAbertoDTO();

            using (Logger Log = Logger.IniciarLog("Consultar Saldos em Aberto Mensal VSAM - Saldos em Aberto"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    retorno = ConsultarDetalheMensalSaldosEmAberto_WACA1304(out statusRetornoDTO, envio);
                    if (statusRetornoDTO.CodigoRetorno == 0)
                        retorno.Totais = ConsultarTotaisPorBandeira_WACA1303(out statusRetornoDTO, envio);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

        }
        #endregion
        /// <summary>
        /// Efetua a consulta ao HIS no book  - WACA1302
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public List<PeriodoDisponivelDTO> ConsultarPeriodosDisponiveis(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarPeriodosDisponiveis";
            List<PeriodoDisponivelDTO> retorno = new List<PeriodoDisponivelDTO>();

            using (Logger Log = Logger.IniciarLog("Consultar Periodos Disponíveis - Saldos em Aberto"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    using (SaldosEmAbertoClient client = new SaldosEmAbertoClient())
                    {
                        Int16 codigoRetorno = 0;
                        String programaChamador = "WACA1302";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = string.Empty;// envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = string.Empty;//envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        String reservaDados = string.Empty;
                        String dados = string.Empty;

                        // monta a string de dados para ser enviada ao HIS
                        dados = String.Empty;// envio.Estabelecimentos.Count.ToString().PadLeft(4, '0');
                        foreach (int estabelecimento in envio.Estabelecimentos)
                        {
                            dados += estabelecimento.ToString().PadLeft(9, '0');
                        }
                        dados = dados.PadRight(30000, ' ');


                        //consulta o serviço do HIS
                        codigoRetorno = client.ConsultarPeriodoDisponivel(programaChamador, sistema, ref usuario, dataInicial, dataFinal, ref mensagemRetorno, ref reservaDados, ref dados);


                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);


                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        // converte o resultado na classe de retorno do método
                        retorno = Tradutores.TradutorResultadoConsultaSaldosEmAberto.TraduzirResultadoPeriodosDisponiveis(dados);

                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Efetua a consulta ao HIS no book  - WACA1305
        /// </summary>
        /// <param name="statusRetornoDTO">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns></returns>
        public Int16 IncluirSolicitacao(out Modelo.StatusRetornoDTO statusRetornoDTO, Modelo.DadosConsultaSaldosEmAbertoDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".IncluirSolicitacao";

            using (Logger Log = Logger.IniciarLog("Incluir solicitação - Saldos em Aberto"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    using (SaldosEmAbertoClient client = new SaldosEmAbertoClient())
                    {
                        Int16 codigoRetorno = 0;
                        String programaChamador = "WACA1305";
                        String sistema = "IS";
                        String usuario = "xxx";
                        String dataInicial = envio.DataInicial.ToString("MM/yyyy");
                        String dataFinal = envio.DataFinal.ToString("MM/yyyy");
                        String mensagemRetorno = string.Empty;
                        String indicadorReChamada = string.Empty;
                        String reservaDados = string.Empty;
                        Int32 estabelecimento = envio.Estabelecimentos[0];

                        //consulta o serviço do HIS
                        codigoRetorno = client.IncluirSolicitacao(programaChamador, sistema, ref usuario, estabelecimento, dataInicial, dataFinal, ref mensagemRetorno, ref reservaDados);


                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno });

                        return codigoRetorno;
                    }

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

    }
}
