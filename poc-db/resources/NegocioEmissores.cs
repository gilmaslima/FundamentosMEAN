/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/29 - Tiago Barbosa dos Santos - Versão Inicial
- 2012/10/31 - Tiago Barbosa dos Santos - Trava de Domicilio
*/
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Dados;
using Redecard.PN.Emissores.Modelos;
using Redecard.PN.Emissores.Agentes;
using System;

namespace Redecard.PN.Emissores.Negocio
{

    public class NegocioEmissores : RegraDeNegocioBase
    {
        #region Constantes
        private const int COD_ERRO = 0;
        //private const string FONTE = "NegocioEmissores.cs";
        #endregion

        #region Obtem PV
        /// <summary>
        /// Obtem as Informações do PV
        /// </summary>
        /// <param name="numPV">Número PV</param>
        /// <returns>PV</returns>
        public PontoVenda ObtemPV(int numPV, out int codigoRetorno)
        {
            try
            {
                return new DadosEmissores().ObtemPV(numPV, out codigoRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        #endregion


        #region Solicitacao Tecnologia
        /// <summary>
        /// Lista os dados de Solicitação de Tecnologia a partir do número do PV
        /// </summary>
        /// <param name="numPV"></param>
        /// <returns></returns>
        public List<DadosSolicitacaoTecnologia> ConsultarTecnologia(int numPV)
        {
            try
            {
                if (numPV == 0)
                    throw new PortalRedecardException(70012, FONTE);
                return new DadosEmissores().ConsultarTecnologia(numPV);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Lista os Integradores
        /// </summary>
        /// <returns></returns>
        public List<Integrador> ConsultarIntegrador(string codIntegrador, string situacao)
        {
            try
            {
                return new DadosEmissores().ConsultarIntegrador(codIntegrador, situacao);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Lista os Equipamentos (Maquinetas)
        /// </summary>
        /// <returns></returns>
        public List<Equipamento> ConsultarEquipamento(out int codigoRetorno, out string mensagemRetorno)
        {
            try
            {
                return new DadosEmissores().ConsultarEquipamento(out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        #endregion


        public byte[] DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo)
        {
            try
            {

                if ((string.IsNullOrEmpty(codEmissor) && string.IsNullOrEmpty(mesArquivo) && string.IsNullOrEmpty(anoArquivo)))
                {
                    throw new PortalRedecardException(70011, FONTE);
                }
                return new DadosEmissores().DownloadArquivo(codEmissor, mesArquivo, anoArquivo);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        /// <summary>
        /// Obtém arquivos disponíveis no período informado
        /// </summary>
        /// <param name="codEmissor"></param>
        /// <param name="anoInicial"></param>
        /// <param name="anoFinal"></param>
        /// <returns></returns>
        public List<DownloadMes> ObterPeriodosDisponiveis(string codEmissor, string anoInicial, string anoFinal)
        {

            try
            {
                return new DadosEmissores().ObterPeriodosDisponiveis(codEmissor, anoInicial, anoFinal);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        public bool SalvarArquivo(byte[] arquivo, string nomeArquivo, string codEmissor, string mesArquivo, string anoArquivo, DateTime dataCriacao)
        {
            try
            {

                if ((string.IsNullOrEmpty(codEmissor) && string.IsNullOrEmpty(mesArquivo) && string.IsNullOrEmpty(anoArquivo)))
                {
                    throw new PortalRedecardException(70011, FONTE);
                }
                return new DadosEmissores().SalvarArquivo(arquivo, nomeArquivo, codEmissor, mesArquivo, anoArquivo, dataCriacao);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        #region Trava Domicilio

        /// <summary>
        /// Consulta de Limite do Emissor
        /// </summary>
        /// <param name="numEmissor">Numero do Emissor</param>
        /// <returns>Valor Limite</returns>
        public decimal ConsultaLimite(int numEmissor, out int codigoRetorno, out string mensagemRetorno)
        {
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);

                return new DadosEmissores().ConsultaLimite(numEmissor, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);
                if (cnpj == 0)
                    throw new PortalRedecardException(70012, FONTE);

                return new DadosEmissores().ConsultaPVTravado(numEmissor, cnpj);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);
                if (cnpj == 0)
                    throw new PortalRedecardException(70012, FONTE);

                return new DadosEmissores().ConsultaPVNaoTravado(numEmissor, cnpj);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }


        public TotaisPV ConsultaTotaisPV(int numEmissor, int cnpj)
        {
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);
                if (cnpj == 0)
                    throw new PortalRedecardException(70012, FONTE);

                return new DadosEmissores().ConsultaTotaisPV(numEmissor, cnpj);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }


        }
        public List<InformacaoPVCobrada> ConsultarInformacoesPVCobranca(
          Int16 funcao, Int32 numeroPv, Decimal cnpj,
       String datade, String datate, Int16 codBanco, Int32 codProduto, Int16 anoComp,
       Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Decimal fatorMultiplicador,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada, out Int16 codigoRetorno, out String mensagemRetorno)
        {
            try
            {
                return new AgentesEmissores().ConsultarInformacoesPVCobranca(funcao, numeroPv, cnpj,
                   datade, datate, codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento, fatorMultiplicador,
                   ref rechamada, out indicadorRechamada, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        public List<InformacaoDetalhada> ConsultarInformacoesDetalhadas(Int16 codBanco, Int32 codProduto, Int16 anoComp,
        Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento,
            ref Dictionary<String, Object> rechamada, out Boolean indicadorRechamada,
            out Int16 codigoRetorno, out String mensagemRetorno)
        {
            try
            {
                return new AgentesEmissores().ConsultarInformacoesDetalhadas(codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento,
                    ref rechamada, out indicadorRechamada, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        #endregion

        #region Efetua Solicitação
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="dadosEntrada"></param>
        /// <returns></returns>
        public bool EfetivarSolicitacao(int numEmissor, DadosEmissao dadosEntrada, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(COD_ERRO, FONTE, "Emissor inválido", new Exception());
                if (dadosEntrada == null)
                    throw new PortalRedecardException(COD_ERRO, FONTE, "Dados de Emissão inválidos", new Exception());

                return new AgentesEmissores().EfetivarSolicitacao(numEmissor, dadosEntrada, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        #endregion

        #region Consulta Vendedor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <param name="CpfCnpj"></param>
        /// <returns></returns>
        public DadosVendedor ConsultaVendedor(string tipoPessoa, string CpfCnpj)
        {
            return new DadosEmissores().ConsultaVendedor(tipoPessoa, CpfCnpj);
        }
        #endregion

        #region Consulta Periodo Trava Domicilio
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <returns></returns>
        public EntidadeConsultaTrava ConsultaPeriodo(Int16 numEmissor, Int32 codigoProduto, Int16 mes, Int16 ano, out Int16 codRetorno)
        {
            try
            {
                return new AgentesEmissores().ConsultaPeriodo(numEmissor, codigoProduto, mes, ano, out codRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="funcao"></param>
        /// <param name="numPv"></param>
        /// <param name="cnpj"></param>
        /// <param name="dataDe"></param>
        /// <param name="dataAte"></param>
        /// <param name="codigoBanco"></param>
        /// <param name="codigoProduto"></param>
        /// <param name="anoCompetencia"></param>
        /// <param name="mesCompetencia"></param>
        /// <param name="precoMedioReferencia"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        public EntidadeConsultaTrava ConsultarTotaisCobranca(Int16 funcao,
           Int32 numPv,
           decimal cnpj,
           String dataDe,
           String dataAte,
           Int16 codigoBanco,
           Int32 codigoProduto,
           Int16 anoCompetencia,
           Int16 mesCompetencia,
           decimal precoMedioReferencia,
           out Int16 codigoRetorno,
           out String mensagemRetorno)
        {
            try
            {

                return new AgentesEmissores().ConsultarTotaisCobranca(funcao, numPv, cnpj, dataDe, dataAte,
                    codigoBanco, codigoProduto, anoCompetencia, mesCompetencia, precoMedioReferencia, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="codProduto"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <param name="PV"></param>
        /// <param name="cod_erro"></param>
        /// <returns></returns>
        public List<InformacaoCobranca> ConsultaInformacaoCobranca(Int16 numEmissor, Int16 codProduto, Int16 mes, Int16 ano, out Int16 cod_erro)
        {
            try
            {
                return new AgentesEmissores().ConsultaInformacaoCobranca(numEmissor, codProduto, mes, ano, out cod_erro);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        public List<DetalheFatura> ConsultarDetalheFatura(Int16 codBanco, Int32 codProduto, Int16 anoComp,
            Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Int32 pvOriginal,
                ref Dictionary<String, Object> rechamada, out Boolean indicadorRechamada, out Int16 codigoRetorno, out String mensagemRetorno)
        {
            try
            {
                return new AgentesEmissores().ConsultarDetalheFatura(codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento, pvOriginal,
                    ref rechamada, out indicadorRechamada, out codigoRetorno, out mensagemRetorno);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        #endregion

        #region PrePagamento
        /// <summary>
        /// Pré-Pagamento Detalhado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <returns></returns>
        public List<SaldoPrePagamento> SaldoDetalhadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);

                return new DadosEmissores().SaldoDetalhadoPrePagamento(numEmissor, dataInicial, dataFinal);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Pré-Pagamento Detalhado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <returns></returns>
        public SaldoPrePagamento SaldoConsolidadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                if (numEmissor == 0)
                    throw new PortalRedecardException(70011, FONTE);

                return new DadosEmissores().SaldoConsolidadoPrePagamento(numEmissor, dataInicial, dataFinal);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Executa a procedure que ajusta a carga de confirmados
        /// </summary>
        /// <returns></returns>
        public Boolean AjustarCargaConfirmados()
        { 
            try
            {
                return new DadosEmissores().AjustarCargaConfirmados();
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
        public List<Modelos.PrePagamento> ConsultarPrePagamento(Int32 codigoBacen, DateTime dataInicial, DateTime dataFinal, List<Bandeira> bandeiras)
        {
            try
            {
                return new DadosEmissores().ConsultarPrePagamento(codigoBacen, dataInicial, dataFinal, bandeiras);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados
        /// </summary>
        /// <returns>List of Modelos.Banco Listagem Bancos(Emissores) com Pré-Pagamentos</returns>
        public List<Modelos.Emissor> ConsultarEmissores()
        {
            try
            {
                return new DadosEmissores().ConsultarEmissores();
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor a filtrar as bandeiras</param>
        /// <returns>List of Modelos.Bandeira Listagem de Emissor-Bandeira com Pré-Pagamentos</returns>
        public List<Modelos.Bandeira> ConsultarEmissoresBandeiras(Int32 codigoBacen)
        {
            try
            {
                return new DadosEmissores().ConsultarEmissoresBandeiras(codigoBacen);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consulta todas as Bandeiras cadastradas no Oracle DR
        /// </summary>
        /// <returns>List of Modelos.Bandeira </returns>
        public List<Modelos.Bandeira> ConsultarBandeiras()
        {
            try
            {
                return new DadosEmissores().ConsultarBandeiras();
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR
        /// </summary>
        /// <returns>List of Modelos.Banco </returns>
        public List<Modelos.Emissor> ConsultarBancos()
        {
            try
            {
                return new DadosEmissores().ConsultarBancos();
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
            try
            {
                return new DadosEmissores().CarregarPrePagamentos(prePagamentos, confirmados);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Exclui todos os pré-pagamentos carregados na base afim de realizar uma nova.
        /// </summary>
        /// <param name="tabela">Nome da tabela a ser excluída</param>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        public Boolean ExcluirPrePagamentos(String tabela)
        {
            try
            {
                return new DadosEmissores().ExcluirPrePagamentos(tabela);
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        #endregion
    }
}

