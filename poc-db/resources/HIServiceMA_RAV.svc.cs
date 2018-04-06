/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.RAV.Servicos;
using Redecard.PN.RAV.Negocios;
using Redecard.PN.RAV.Modelos;
using AutoMapper;
using Redecard.PN.Comum;

namespace Redecard.PN.RAV.Servicos
{
    //[ExceptionShielding("RedecardPortalException")]
    public class ServicoPortalRAV : ServicoBase, IServicoPortalRAV
    {
        #region RAV Avulso
        /// <summary>
        /// Verifica o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        public Servicos.ModRAVAvulsoSaida VerificarRAVDisponivel(int numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV });

                try
                {
                    Servicos.ModRAVAvulsoSaida svcSaidaRAV = new Servicos.ModRAVAvulsoSaida();
                    Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivel(numeroPDV);
                    /*
                    Mapper.CreateMap<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>();
                    svcSaidaRAV.DadosAntecipacao = Mapper.Map<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>(modSaidaRAV.DadosAntecipado);

                    Mapper.CreateMap<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>();
                    svcSaidaRAV = Mapper.Map<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>(modSaidaRAV);
                    */

                    //if (modSaidaRAV.Retorno == 70000 && modSaidaRAV.ValorDisponivel == 0)
                    //{
                    //    modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivelURA(numeroPDV);
                    //}

                    svcSaidaRAV.Agencia = modSaidaRAV.Agencia;
                    svcSaidaRAV.Banco = modSaidaRAV.Banco;
                    svcSaidaRAV.Conta = modSaidaRAV.Conta;

                    //Dados Antecipação                
                    svcSaidaRAV.DadosAntecipacao.DataAte = modSaidaRAV.DadosAntecipado.DataAte;
                    svcSaidaRAV.DadosAntecipacao.DataDe = modSaidaRAV.DadosAntecipado.DataDe;
                    svcSaidaRAV.DadosAntecipacao.NomeProdutoAntecipacao = modSaidaRAV.DadosAntecipado.NomeProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.DescricaoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.DescricaoProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.ValorMaxAntecUraSenha = modSaidaRAV.DadosAntecipado.ValorMaxAntecUraSenha;
                    svcSaidaRAV.DadosAntecipacao.ValorTarifa = modSaidaRAV.DadosAntecipado.ValorTarifa;
                    svcSaidaRAV.DadosAntecipacao.CodigoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.CodigoProdutoAntecipacao;

                    switch (modSaidaRAV.DadosAntecipado.Indicador)
                    {
                        case Modelos.ElndAntecipa.Parcial:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Parcial; break; }
                        case Modelos.ElndAntecipa.Total:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Total; break; }
                    }

                    switch (modSaidaRAV.DadosAntecipado.IndicadorData)
                    {
                        case Modelos.ElndDataAntecipa.Apresentacao:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Apresentacao; break; }
                        case Modelos.ElndDataAntecipa.Vencimento:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Vencimento; break; }
                    }
                    switch (modSaidaRAV.DadosAntecipado.IndicadorProduto)
                    {
                        case Modelos.ElndProdutoAntecipa.Ambos:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos; break; }
                        case Modelos.ElndProdutoAntecipa.Parcelado:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado; break; }
                        case Modelos.ElndProdutoAntecipa.Rotativo:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo; break; }
                    }

                    svcSaidaRAV.DadosAntecipacao.Valor = modSaidaRAV.DadosAntecipado.Valor;
                    //Fim Dados Antecipação

                    svcSaidaRAV.DadosParaCredito = new List<Servicos.ModRAVAvulsoCredito>();
                    for (int i = 0; i < modSaidaRAV.DadosParaCredito.Count; i++)
                    {
                        svcSaidaRAV.DadosParaCredito.Add(new Servicos.ModRAVAvulsoCredito()
                        {
                            DataCredito = modSaidaRAV.DadosParaCredito[i].DataCredito,
                            TaxaEfetiva = modSaidaRAV.DadosParaCredito[i].TaxaEfetiva,
                            TaxaPeriodo = modSaidaRAV.DadosParaCredito[i].TaxaPeriodo,
                            ValorLiquido = modSaidaRAV.DadosParaCredito[i].ValorLiquido,
                            ValorParcelado = modSaidaRAV.DadosParaCredito[i].ValorParcelado,
                            ValorRotativo = modSaidaRAV.DadosParaCredito[i].ValorRotativo
                        });
                    }

                    svcSaidaRAV.DataProcessamento = modSaidaRAV.DataProcessamento;
                    svcSaidaRAV.Desconto = modSaidaRAV.Desconto;
                    svcSaidaRAV.FimCarencia = modSaidaRAV.FimCarencia;
                    svcSaidaRAV.HoraFimD0 = modSaidaRAV.HoraFimD0;
                    svcSaidaRAV.HoraFimDn = modSaidaRAV.HoraFimDn;
                    svcSaidaRAV.HoraIniD0 = modSaidaRAV.HoraIniD0;
                    svcSaidaRAV.HoraIniDn = modSaidaRAV.HoraIniDn;
                    svcSaidaRAV.HoraProcessamento = modSaidaRAV.HoraProcessamento;
                    svcSaidaRAV.MsgErro = modSaidaRAV.MsgErro;
                    svcSaidaRAV.PeriodoAte = modSaidaRAV.PeriodoAte;
                    svcSaidaRAV.PeriodoDe = modSaidaRAV.PeriodoDe;
                    svcSaidaRAV.Retorno = modSaidaRAV.Retorno;

                    svcSaidaRAV.TabelaRAVs = new List<Servicos.ModRAVAvulsoRetorno>();
                    for (int i = 0; i < modSaidaRAV.TabelaRAVs.Count; i++)
                    {
                        svcSaidaRAV.TabelaRAVs.Add(new Servicos.ModRAVAvulsoRetorno()
                        {
                            DataApresentacao = modSaidaRAV.TabelaRAVs[i].DataApresentacao,
                            NumeroRAV = modSaidaRAV.TabelaRAVs[i].NumeroRAV,
                            QuantidadeOC = modSaidaRAV.TabelaRAVs[i].QuantidadeOC,
                            ValorBruto = modSaidaRAV.TabelaRAVs[i].ValorBruto,
                            ValorLiquido = modSaidaRAV.TabelaRAVs[i].ValorLiquido
                        });
                    }

                    svcSaidaRAV.ValorAntecipadoD0 = modSaidaRAV.ValorAntecipadoD0;
                    svcSaidaRAV.ValorAntecipadoD1 = modSaidaRAV.ValorAntecipadoD1;
                    svcSaidaRAV.ValorBruto = modSaidaRAV.ValorBruto;
                    svcSaidaRAV.ValorDisponivel = modSaidaRAV.ValorDisponivel;
                    svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;
                    svcSaidaRAV.ValorOriginal = modSaidaRAV.ValorOriginal;

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });

                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível através da URA.
        /// </summary>
        /// <param name="numeroPDV">Número da Entidade</param>
        /// <param name="tipoCredito">Tipo da Antecipação do Crédito
        ///     <example>0: Antecipação D+0;</example>
        ///     <example>1: Antecipação D+1</example>
        /// </param>
        /// <returns>Modelo com os dados de saída do RAV Avulso</returns>
        public Servicos.ModRAVAvulsoSaida VerificarRAVDisponivelURA(Int32 numeroPDV, short tipoCredito)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível através da URA"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoCredito });

                try
                {
                    Servicos.ModRAVAvulsoSaida svcSaidaRAV = new Servicos.ModRAVAvulsoSaida();
                    Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivelURA(numeroPDV, tipoCredito);
                    /*
                    Mapper.CreateMap<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>();
                    svcSaidaRAV.DadosAntecipacao = Mapper.Map<Modelos.ModRAVAntecipa, Servicos.ModRAVAntecipa>(modSaidaRAV.DadosAntecipado);

                    Mapper.CreateMap<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>();
                    svcSaidaRAV = Mapper.Map<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>(modSaidaRAV);
                    */

                    svcSaidaRAV.Agencia = modSaidaRAV.Agencia;
                    svcSaidaRAV.Banco = modSaidaRAV.Banco;
                    svcSaidaRAV.Conta = modSaidaRAV.Conta;

                    //Dados Antecipação                
                    svcSaidaRAV.DadosAntecipacao.DataAte = modSaidaRAV.DadosAntecipado.DataAte;
                    svcSaidaRAV.DadosAntecipacao.DataDe = modSaidaRAV.DadosAntecipado.DataDe;
                    svcSaidaRAV.DadosAntecipacao.NomeProdutoAntecipacao = modSaidaRAV.DadosAntecipado.NomeProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.DescricaoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.DescricaoProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.ValorMaxAntecUraSenha = modSaidaRAV.DadosAntecipado.ValorMaxAntecUraSenha;
                    svcSaidaRAV.DadosAntecipacao.ValorTarifa = modSaidaRAV.DadosAntecipado.ValorTarifa;
                    svcSaidaRAV.DadosAntecipacao.CodigoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.CodigoProdutoAntecipacao;

                    switch (modSaidaRAV.DadosAntecipado.Indicador)
                    {
                        case Modelos.ElndAntecipa.Parcial:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Parcial; break; }
                        case Modelos.ElndAntecipa.Total:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Total; break; }
                    }

                    switch (modSaidaRAV.DadosAntecipado.IndicadorData)
                    {
                        case Modelos.ElndDataAntecipa.Apresentacao:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Apresentacao; break; }
                        case Modelos.ElndDataAntecipa.Vencimento:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Vencimento; break; }
                    }
                    switch (modSaidaRAV.DadosAntecipado.IndicadorProduto)
                    {
                        case Modelos.ElndProdutoAntecipa.Ambos:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos; break; }
                        case Modelos.ElndProdutoAntecipa.Parcelado:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado; break; }
                        case Modelos.ElndProdutoAntecipa.Rotativo:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo; break; }
                    }

                    svcSaidaRAV.DadosAntecipacao.Valor = modSaidaRAV.DadosAntecipado.Valor;
                    //Fim Dados Antecipação

                    svcSaidaRAV.DadosParaCredito = new List<Servicos.ModRAVAvulsoCredito>();
                    for (int i = 0; i < modSaidaRAV.DadosParaCredito.Count; i++)
                    {
                        svcSaidaRAV.DadosParaCredito.Add(new Servicos.ModRAVAvulsoCredito()
                        {
                            DataCredito = modSaidaRAV.DadosParaCredito[i].DataCredito,
                            TaxaEfetiva = modSaidaRAV.DadosParaCredito[i].TaxaEfetiva,
                            TaxaPeriodo = modSaidaRAV.DadosParaCredito[i].TaxaPeriodo,
                            ValorLiquido = modSaidaRAV.DadosParaCredito[i].ValorLiquido,
                            ValorParcelado = modSaidaRAV.DadosParaCredito[i].ValorParcelado,
                            ValorRotativo = modSaidaRAV.DadosParaCredito[i].ValorRotativo
                        });
                    }

                    svcSaidaRAV.DataProcessamento = modSaidaRAV.DataProcessamento;
                    svcSaidaRAV.Desconto = modSaidaRAV.Desconto;
                    svcSaidaRAV.FimCarencia = modSaidaRAV.FimCarencia;
                    svcSaidaRAV.HoraFimD0 = modSaidaRAV.HoraFimD0;
                    svcSaidaRAV.HoraFimDn = modSaidaRAV.HoraFimDn;
                    svcSaidaRAV.HoraIniD0 = modSaidaRAV.HoraIniD0;
                    svcSaidaRAV.HoraIniDn = modSaidaRAV.HoraIniDn;
                    svcSaidaRAV.HoraProcessamento = modSaidaRAV.HoraProcessamento;
                    svcSaidaRAV.MsgErro = modSaidaRAV.MsgErro;
                    svcSaidaRAV.PeriodoAte = modSaidaRAV.PeriodoAte;
                    svcSaidaRAV.PeriodoDe = modSaidaRAV.PeriodoDe;
                    svcSaidaRAV.Retorno = modSaidaRAV.Retorno;

                    svcSaidaRAV.TabelaRAVs = new List<Servicos.ModRAVAvulsoRetorno>();
                    for (int i = 0; i < modSaidaRAV.TabelaRAVs.Count; i++)
                    {
                        svcSaidaRAV.TabelaRAVs.Add(new Servicos.ModRAVAvulsoRetorno()
                        {
                            DataApresentacao = modSaidaRAV.TabelaRAVs[i].DataApresentacao,
                            NumeroRAV = modSaidaRAV.TabelaRAVs[i].NumeroRAV,
                            QuantidadeOC = modSaidaRAV.TabelaRAVs[i].QuantidadeOC,
                            ValorBruto = modSaidaRAV.TabelaRAVs[i].ValorBruto,
                            ValorLiquido = modSaidaRAV.TabelaRAVs[i].ValorLiquido
                        });
                    }

                    svcSaidaRAV.ValorAntecipadoD0 = modSaidaRAV.ValorAntecipadoD0;
                    svcSaidaRAV.ValorAntecipadoD1 = modSaidaRAV.ValorAntecipadoD1;
                    svcSaidaRAV.ValorBruto = modSaidaRAV.ValorBruto;
                    svcSaidaRAV.ValorDisponivel = modSaidaRAV.ValorDisponivel;
                    svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;
                    svcSaidaRAV.ValorOriginal = modSaidaRAV.ValorOriginal;

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });

                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Verifica o RAV Avulso disponível de um PDV específico.
        /// Equivalente ao método VerificarRAVDisponivel, porém utiliza cacheamento dos dados.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        public Servicos.ModRAVAvulsoSaida VerificarRAVDisponivel_Cache(int numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível - Cache"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV });

                try
                {
                    //Objeto de retorno
                    Servicos.ModRAVAvulsoSaida svcSaidaRAV = null;

                    //Chave do objeto no cache (valor disponível por PV)
                    String chaveCache = String.Format("RAV_{0}_{1}", DateTime.Now.ToString("ddMMyyHH"), numeroPDV);

                    Log.GravarMensagem("Chave cache", new { chaveCache });
#if !DEBUG
                    //Retorna objeto do cache
                    svcSaidaRAV = CacheAdmin.Recuperar<Servicos.ModRAVAvulsoSaida>(Cache.Home, chaveCache);
#endif

                    //Se objeto não está em cache, busca no mainframe e armazena em cache
                    if (svcSaidaRAV == null)
                    {
                        svcSaidaRAV = new Servicos.ModRAVAvulsoSaida();
                        Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivel(numeroPDV);

                        //if (modSaidaRAV.Retorno == 70000 && modSaidaRAV.ValorDisponivel == 0)
                        //{
                        //    modSaidaRAV = new NegPortalRAV().VerificarRAVDisponivelURA(numeroPDV);
                        //}

                        svcSaidaRAV.Agencia = modSaidaRAV.Agencia;
                        svcSaidaRAV.Banco = modSaidaRAV.Banco;
                        svcSaidaRAV.Conta = modSaidaRAV.Conta;

                        //Dados Antecipação                
                        svcSaidaRAV.DadosAntecipacao.DataAte = modSaidaRAV.DadosAntecipado.DataAte;
                        svcSaidaRAV.DadosAntecipacao.DataDe = modSaidaRAV.DadosAntecipado.DataDe;
                        svcSaidaRAV.DadosAntecipacao.NomeProdutoAntecipacao = modSaidaRAV.DadosAntecipado.NomeProdutoAntecipacao;
                        svcSaidaRAV.DadosAntecipacao.DescricaoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.DescricaoProdutoAntecipacao;
                        svcSaidaRAV.DadosAntecipacao.ValorMaxAntecUraSenha = modSaidaRAV.DadosAntecipado.ValorMaxAntecUraSenha;
                        svcSaidaRAV.DadosAntecipacao.ValorTarifa = modSaidaRAV.DadosAntecipado.ValorTarifa;
                        svcSaidaRAV.DadosAntecipacao.CodigoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.CodigoProdutoAntecipacao;

                        switch (modSaidaRAV.DadosAntecipado.Indicador)
                        {
                            case Modelos.ElndAntecipa.Parcial:
                                { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Parcial; break; }
                            case Modelos.ElndAntecipa.Total:
                                { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Total; break; }
                        }

                        switch (modSaidaRAV.DadosAntecipado.IndicadorData)
                        {
                            case Modelos.ElndDataAntecipa.Apresentacao:
                                { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Apresentacao; break; }
                            case Modelos.ElndDataAntecipa.Vencimento:
                                { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Vencimento; break; }
                        }
                        switch (modSaidaRAV.DadosAntecipado.IndicadorProduto)
                        {
                            case Modelos.ElndProdutoAntecipa.Ambos:
                                { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos; break; }
                            case Modelos.ElndProdutoAntecipa.Parcelado:
                                { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado; break; }
                            case Modelos.ElndProdutoAntecipa.Rotativo:
                                { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo; break; }
                        }

                        svcSaidaRAV.DadosAntecipacao.Valor = modSaidaRAV.DadosAntecipado.Valor;
                        //Fim Dados Antecipação

                        svcSaidaRAV.DadosParaCredito = new List<Servicos.ModRAVAvulsoCredito>();
                        for (int i = 0; i < modSaidaRAV.DadosParaCredito.Count; i++)
                        {
                            svcSaidaRAV.DadosParaCredito.Add(new Servicos.ModRAVAvulsoCredito()
                            {
                                DataCredito = modSaidaRAV.DadosParaCredito[i].DataCredito,
                                TaxaEfetiva = modSaidaRAV.DadosParaCredito[i].TaxaEfetiva,
                                TaxaPeriodo = modSaidaRAV.DadosParaCredito[i].TaxaPeriodo,
                                ValorLiquido = modSaidaRAV.DadosParaCredito[i].ValorLiquido,
                                ValorParcelado = modSaidaRAV.DadosParaCredito[i].ValorParcelado,
                                ValorRotativo = modSaidaRAV.DadosParaCredito[i].ValorRotativo
                            });
                        }

                        svcSaidaRAV.DataProcessamento = modSaidaRAV.DataProcessamento;
                        svcSaidaRAV.Desconto = modSaidaRAV.Desconto;
                        svcSaidaRAV.FimCarencia = modSaidaRAV.FimCarencia;
                        svcSaidaRAV.HoraFimD0 = modSaidaRAV.HoraFimD0;
                        svcSaidaRAV.HoraFimDn = modSaidaRAV.HoraFimDn;
                        svcSaidaRAV.HoraIniD0 = modSaidaRAV.HoraIniD0;
                        svcSaidaRAV.HoraIniDn = modSaidaRAV.HoraIniDn;
                        svcSaidaRAV.HoraProcessamento = modSaidaRAV.HoraProcessamento;
                        svcSaidaRAV.MsgErro = modSaidaRAV.MsgErro;
                        svcSaidaRAV.PeriodoAte = modSaidaRAV.PeriodoAte;
                        svcSaidaRAV.PeriodoDe = modSaidaRAV.PeriodoDe;
                        svcSaidaRAV.Retorno = modSaidaRAV.Retorno;

                        svcSaidaRAV.TabelaRAVs = new List<Servicos.ModRAVAvulsoRetorno>();
                        for (int i = 0; i < modSaidaRAV.TabelaRAVs.Count; i++)
                        {
                            svcSaidaRAV.TabelaRAVs.Add(new Servicos.ModRAVAvulsoRetorno()
                            {
                                DataApresentacao = modSaidaRAV.TabelaRAVs[i].DataApresentacao,
                                NumeroRAV = modSaidaRAV.TabelaRAVs[i].NumeroRAV,
                                QuantidadeOC = modSaidaRAV.TabelaRAVs[i].QuantidadeOC,
                                ValorBruto = modSaidaRAV.TabelaRAVs[i].ValorBruto,
                                ValorLiquido = modSaidaRAV.TabelaRAVs[i].ValorLiquido
                            });
                        }

                        svcSaidaRAV.ValorAntecipadoD0 = modSaidaRAV.ValorAntecipadoD0;
                        svcSaidaRAV.ValorAntecipadoD1 = modSaidaRAV.ValorAntecipadoD1;
                        svcSaidaRAV.ValorBruto = modSaidaRAV.ValorBruto;
                        svcSaidaRAV.ValorDisponivel = modSaidaRAV.ValorDisponivel;
                        svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;
                        svcSaidaRAV.ValorOriginal = modSaidaRAV.ValorOriginal;
#if !DEBUG
                        CacheAdmin.Adicionar(Cache.Home, chaveCache, svcSaidaRAV);
#endif
                    }
                    else
                    {
                        Logger.GravarLog("Dados obtidos do cache", new { chaveCache });
                    }

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });

                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da pesquisa. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorAntecipado">O valor a ser antecipado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        public Servicos.ModRAVAvulsoSaida ConsultarRAVAvulso(Servicos.ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorAntecipado)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { entradaRAV, numeroPDV, tipoCredito, valorAntecipado });

                try
                {
                    Modelos.ModRAVAvulsoEntrada modEntradaRAV = new Modelos.ModRAVAvulsoEntrada();
                    Servicos.ModRAVAvulsoSaida svcSaidaRAV = new Servicos.ModRAVAvulsoSaida();

                    Mapper.CreateMap<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>();
                    modEntradaRAV = Mapper.Map<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>(entradaRAV);

                    switch (entradaRAV.DadosAntecipacao.IndicadorProduto)
                    {
                        case ElndProdutoAntecipa.Rotativo:
                            modEntradaRAV.IndProduto = "R";
                            break;
                        case ElndProdutoAntecipa.Parcelado:
                            modEntradaRAV.IndProduto = "P";
                            break;
                        case ElndProdutoAntecipa.Ambos:
                            modEntradaRAV.IndProduto = "A";
                            break;
                    }

                    Modelos.ModRAVAvulsoSaida modSaidaRAV = new NegPortalRAV().ConsultarRAVAvulso(modEntradaRAV, numeroPDV, tipoCredito, valorAntecipado);

                    Mapper.CreateMap<Modelos.ModRAVAvulsoSaida, Servicos.ModRAVAvulsoSaida>();

                    //MAPPING
                    svcSaidaRAV.Agencia = modSaidaRAV.Agencia;
                    svcSaidaRAV.Banco = modSaidaRAV.Banco;
                    svcSaidaRAV.Conta = modSaidaRAV.Conta;

                    //Dados Antecipação                
                    svcSaidaRAV.DadosAntecipacao.DataAte = modSaidaRAV.DadosAntecipado.DataAte;
                    svcSaidaRAV.DadosAntecipacao.DataDe = modSaidaRAV.DadosAntecipado.DataDe;
                    svcSaidaRAV.DadosAntecipacao.NomeProdutoAntecipacao = modSaidaRAV.DadosAntecipado.NomeProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.DescricaoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.DescricaoProdutoAntecipacao;
                    svcSaidaRAV.DadosAntecipacao.ValorMaxAntecUraSenha = modSaidaRAV.DadosAntecipado.ValorMaxAntecUraSenha;
                    svcSaidaRAV.DadosAntecipacao.ValorTarifa = modSaidaRAV.DadosAntecipado.ValorTarifa;
                    svcSaidaRAV.DadosAntecipacao.CodigoProdutoAntecipacao = modSaidaRAV.DadosAntecipado.CodigoProdutoAntecipacao;

                    switch (modSaidaRAV.DadosAntecipado.Indicador)
                    {
                        case Modelos.ElndAntecipa.Parcial:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Parcial; break; }
                        case Modelos.ElndAntecipa.Total:
                            { svcSaidaRAV.DadosAntecipacao.Indicador = ElndAntecipa.Total; break; }
                    }

                    switch (modSaidaRAV.DadosAntecipado.IndicadorData)
                    {
                        case Modelos.ElndDataAntecipa.Apresentacao:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Apresentacao; break; }
                        case Modelos.ElndDataAntecipa.Vencimento:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorData = ElndDataAntecipa.Vencimento; break; }
                    }
                    switch (modSaidaRAV.DadosAntecipado.IndicadorProduto)
                    {
                        case Modelos.ElndProdutoAntecipa.Ambos:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos; break; }
                        case Modelos.ElndProdutoAntecipa.Parcelado:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado; break; }
                        case Modelos.ElndProdutoAntecipa.Rotativo:
                            { svcSaidaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo; break; }
                    }

                    svcSaidaRAV.DadosAntecipacao.Valor = modSaidaRAV.DadosAntecipado.Valor;
                    //Fim Dados Antecipação

                    svcSaidaRAV.DadosParaCredito = new List<Servicos.ModRAVAvulsoCredito>();
                    for (int i = 0; i < modSaidaRAV.DadosParaCredito.Count; i++)
                    {
                        svcSaidaRAV.DadosParaCredito.Add(new Servicos.ModRAVAvulsoCredito()
                        {
                            DataCredito = modSaidaRAV.DadosParaCredito[i].DataCredito,
                            TaxaEfetiva = modSaidaRAV.DadosParaCredito[i].TaxaEfetiva,
                            TaxaPeriodo = modSaidaRAV.DadosParaCredito[i].TaxaPeriodo,
                            ValorLiquido = modSaidaRAV.DadosParaCredito[i].ValorLiquido,
                            ValorParcelado = modSaidaRAV.DadosParaCredito[i].ValorParcelado,
                            ValorRotativo = modSaidaRAV.DadosParaCredito[i].ValorRotativo
                        });
                    }

                    svcSaidaRAV.DataProcessamento = modSaidaRAV.DataProcessamento;
                    svcSaidaRAV.Desconto = modSaidaRAV.Desconto;
                    svcSaidaRAV.FimCarencia = modSaidaRAV.FimCarencia;
                    svcSaidaRAV.HoraFimD0 = modSaidaRAV.HoraFimD0;
                    svcSaidaRAV.HoraFimDn = modSaidaRAV.HoraFimDn;
                    svcSaidaRAV.HoraIniD0 = modSaidaRAV.HoraIniD0;
                    svcSaidaRAV.HoraIniDn = modSaidaRAV.HoraIniDn;
                    svcSaidaRAV.HoraProcessamento = modSaidaRAV.HoraProcessamento;
                    svcSaidaRAV.MsgErro = modSaidaRAV.MsgErro;
                    svcSaidaRAV.PeriodoAte = modSaidaRAV.PeriodoAte;
                    svcSaidaRAV.PeriodoDe = modSaidaRAV.PeriodoDe;
                    svcSaidaRAV.Retorno = modSaidaRAV.Retorno;

                    svcSaidaRAV.TabelaRAVs = new List<Servicos.ModRAVAvulsoRetorno>();
                    for (int i = 0; i < modSaidaRAV.TabelaRAVs.Count; i++)
                    {
                        svcSaidaRAV.TabelaRAVs.Add(new Servicos.ModRAVAvulsoRetorno()
                        {
                            DataApresentacao = modSaidaRAV.TabelaRAVs[i].DataApresentacao,
                            NumeroRAV = modSaidaRAV.TabelaRAVs[i].NumeroRAV,
                            QuantidadeOC = modSaidaRAV.TabelaRAVs[i].QuantidadeOC,
                            ValorBruto = modSaidaRAV.TabelaRAVs[i].ValorBruto,
                            ValorLiquido = modSaidaRAV.TabelaRAVs[i].ValorLiquido
                        });
                    }

                    svcSaidaRAV.ValorAntecipadoD0 = modSaidaRAV.ValorAntecipadoD0;
                    svcSaidaRAV.ValorAntecipadoD1 = modSaidaRAV.ValorAntecipadoD1;
                    svcSaidaRAV.ValorBruto = modSaidaRAV.ValorBruto;
                    svcSaidaRAV.ValorDisponivel = modSaidaRAV.ValorDisponivel;
                    svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;
                    svcSaidaRAV.ValorOriginal = modSaidaRAV.ValorOriginal;

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });
                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua o RAV Avulso de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da efetuação. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorSolicitado">O valor a ser antecipado.</param>
        /// <returns>O código de retorno da transação. Consultar documentação para lista de códigos de retorno.</returns>
        public int EfetuarRAVAvulso(Servicos.ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorSolicitado)
        {
            using (Logger Log = Logger.IniciarLog("Efetua o RAV Avulso"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { entradaRAV, numeroPDV, tipoCredito, valorAntecipado = valorSolicitado });

                try
                {
                    Modelos.ModRAVAvulsoEntrada modEntradaRAV = new Modelos.ModRAVAvulsoEntrada();

                    Mapper.CreateMap<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>();
                    modEntradaRAV = Mapper.Map<Servicos.ModRAVAvulsoEntrada, Modelos.ModRAVAvulsoEntrada>(entradaRAV);

                    switch (entradaRAV.DadosAntecipacao.IndicadorProduto)
                    {
                        case ElndProdutoAntecipa.Rotativo:
                            modEntradaRAV.IndProduto = "R";
                            break;
                        case ElndProdutoAntecipa.Parcelado:
                            modEntradaRAV.IndProduto = "P";
                            break;
                        case ElndProdutoAntecipa.Ambos:
                            modEntradaRAV.IndProduto = "A";
                            break;
                    }

                    var retorno = new NegPortalRAV().EfetuarRAVAvulso(modEntradaRAV, numeroPDV, tipoCredito, valorSolicitado);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        public Servicos.MA30 ExecutarMA30(Servicos.MA30 chamadaMA30)
        {
            using (Logger Log = Logger.IniciarLog("Execução RAV Avulso"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { chamadaMA30 });

                    Servicos.MA30 retornoServicoMA30 = new MA30();
                    Modelos.MA30 retornoModeloMA30 = new Modelos.MA30();
                    
                    Mapper.CreateMap<Servicos.MA30, Modelos.MA30>();
                    Mapper.CreateMap<Servicos.FILLER, Modelos.FILLER>();
                    Mapper.CreateMap<Servicos.FILLER1, Modelos.FILLER1>();
                    Modelos.MA30 chamadaModeloMA30 = Mapper.Map<Servicos.MA30,Modelos.MA30>(chamadaMA30);

                    NegPortalRAV ravNegocio = new NegPortalRAV();
                    retornoModeloMA30 = ravNegocio.ExecutarMA30(chamadaModeloMA30);

                    Mapper.CreateMap<Modelos.MA30, Servicos.MA30>();
                    Mapper.CreateMap<Modelos.FILLER, Servicos.FILLER>();
                    Mapper.CreateMap<Modelos.FILLER1, Servicos.FILLER1>();

                    retornoServicoMA30 = Mapper.Map<Modelos.MA30, Servicos.MA30>(retornoModeloMA30);

                    return retornoServicoMA30;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        #region RAV Automático
        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <param name="tipoVenda">Tipo de Venda (R).</param>
        /// <param name="periodicidade">Periodicidade (P).</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public bool VerificarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade });

                try
                {

                    Servicos.ModRAVAutomatico modSaidaRAV = this.ConsultarRAVAutomatico(numeroPDV, tipoVenda, periodicidade);

                    Boolean retorno = false;
                    if (modSaidaRAV != null && !String.IsNullOrEmpty(modSaidaRAV.DataContratoFormatada))
                    {
                        retorno = true;
                    }

                    Log.GravarLog(EventoLog.FimServico, new { retorno, modSaidaRAV });
                    return retorno;
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    throw ex;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public Servicos.ModRAVAutomatico ConsultarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade });

                try
                {
                    Servicos.ModRAVAutomatico svcSaidaRAV = null;
                    Modelos.ModRAVAutomatico modSaidaRAV = new NegPortalRAV().ConsultarRAVAutomatico(numeroPDV, tipoVenda, periodicidade);
                    /*
                    Mapper.CreateMap<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>();
                    svcSaidaRAV = Mapper.Map<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>(modSaidaRAV);
                    */

                    if (modSaidaRAV == null) { return null; }

                    svcSaidaRAV = new Servicos.ModRAVAutomatico();

                    svcSaidaRAV.CodigoProduto = modSaidaRAV.CodigoProduto;
                    svcSaidaRAV.CodMotivoExclusao = modSaidaRAV.CodMotivoExclusao;
                    svcSaidaRAV.CodSituacao = modSaidaRAV.CodSituacao;
                    svcSaidaRAV.CodVenda = modSaidaRAV.CodVenda;

                    //Dados Retorno
                    if (modSaidaRAV.DadosRetorno != null)
                    {
                        svcSaidaRAV.DadosRetorno.CodCategoria = modSaidaRAV.DadosRetorno.CodCategoria;
                        svcSaidaRAV.DadosRetorno.CodOpidAlteracao = modSaidaRAV.DadosRetorno.CodOpidAlteracao;
                        svcSaidaRAV.DadosRetorno.CodOpidAutorizacao = modSaidaRAV.DadosRetorno.CodOpidAutorizacao;
                        svcSaidaRAV.DadosRetorno.CodRetorno = modSaidaRAV.CodRetorno;
                        svcSaidaRAV.DadosRetorno.CodSituacaoPendente = modSaidaRAV.DadosRetorno.CodSituacaoPendente;
                        svcSaidaRAV.DadosRetorno.CPF_CNPJ = modSaidaRAV.DadosRetorno.CpfCnpj;
                        svcSaidaRAV.DadosRetorno.DataAgendaExclusao = modSaidaRAV.DadosRetorno.DataAgendaExclusao;
                        svcSaidaRAV.DadosRetorno.DataAlteracao = modSaidaRAV.DadosRetorno.DataAlteracao;
                        svcSaidaRAV.DadosRetorno.DataAutorizacao = modSaidaRAV.DadosRetorno.DataAutorizacao;
                        svcSaidaRAV.DadosRetorno.DataBaseAntecipacao = modSaidaRAV.DadosRetorno.DataBaseAntecipacao;
                        svcSaidaRAV.DadosRetorno.DataFimFidelizacao = modSaidaRAV.DadosRetorno.DataFimFidelizacao;
                        svcSaidaRAV.DadosRetorno.DataIniFidelizacao = modSaidaRAV.DadosRetorno.DataIniFidelizacao;
                        svcSaidaRAV.DadosRetorno.DataProximaAntecipacao = modSaidaRAV.DadosRetorno.DataProximaAntecipacao;
                        svcSaidaRAV.DadosRetorno.DescCategoria = modSaidaRAV.DadosRetorno.DescCategoria;
                        svcSaidaRAV.DadosRetorno.DescSituacaoCategoria = modSaidaRAV.DadosRetorno.DescSituacaoCategoria;
                        svcSaidaRAV.DadosRetorno.Estabelecimento = modSaidaRAV.DadosRetorno.Estabelecimento;
                        svcSaidaRAV.DadosRetorno.HoraAlteracao = modSaidaRAV.DadosRetorno.HoraAlteracao;
                        svcSaidaRAV.DadosRetorno.HoraAutorizacao = modSaidaRAV.DadosRetorno.HoraAutorizacao;
                        svcSaidaRAV.DadosRetorno.IndBloqueio = modSaidaRAV.DadosRetorno.IndBloqueio;
                        svcSaidaRAV.DadosRetorno.MsgRetorno = modSaidaRAV.MensagemRetorno;
                        svcSaidaRAV.DadosRetorno.NumMatrix = modSaidaRAV.DadosRetorno.NumMatrix;
                        svcSaidaRAV.DadosRetorno.TaxaCategoria = modSaidaRAV.DadosRetorno.TaxaCategoria;
                        svcSaidaRAV.DadosRetorno.TaxaFidelizacao = modSaidaRAV.DadosRetorno.TaxaFidelizacao;
                        
                        // ARE - 18/05 - INCLUSÃO DOS CAMPOS DE SESSÃO DE CRÉDITO PARA EXIBIÇÃO EM TELA
                        svcSaidaRAV.DadosRetorno.CodigoProdutoAntecipacao = modSaidaRAV.DadosRetorno.CodigoProdutoAntecipacao;
                        svcSaidaRAV.DadosRetorno.NomeProdutoAntecipacao = modSaidaRAV.DadosRetorno.NomeProdutoAntecipacao;
                    }

                    if (modSaidaRAV.DataContrato != null)
                    {
                        svcSaidaRAV.DataContrato = modSaidaRAV.DataContrato.Value;
                        svcSaidaRAV.DataContratoFormatada = modSaidaRAV.DataContrato.Value.ToShortDateString();
                    }
                    else
                    {
                        svcSaidaRAV.DataContrato = DateTime.MinValue;
                        svcSaidaRAV.DataContratoFormatada = string.Empty;
                    }
                    svcSaidaRAV.DataIniEstoq = modSaidaRAV.DataIniEstoq;
                    svcSaidaRAV.DataVigenciaFim = modSaidaRAV.DataVigenciaFim;
                    svcSaidaRAV.DataVigenciaIni = modSaidaRAV.DataVigenciaIni;
                    svcSaidaRAV.DescMotivoExclusao = modSaidaRAV.DescMotivoExclusao;
                    svcSaidaRAV.DiaAntecipacao = modSaidaRAV.DiaAntecipacao;

                    switch (modSaidaRAV.DiaSemana)
                    {
                        case Modelos.EDiaSemana.Segunda:
                            { svcSaidaRAV.DiaSemana = EDiaSemana.Segunda; break; }
                        case Modelos.EDiaSemana.Terca:
                            { svcSaidaRAV.DiaSemana = EDiaSemana.Terca; break; }
                        case Modelos.EDiaSemana.Quarta:
                            { svcSaidaRAV.DiaSemana = EDiaSemana.Quarta; break; }
                        case Modelos.EDiaSemana.Quinta:
                            { svcSaidaRAV.DiaSemana = EDiaSemana.Quinta; break; }
                        case Modelos.EDiaSemana.Sexta:
                            { svcSaidaRAV.DiaSemana = EDiaSemana.Sexta; break; }
                    }

                    switch (modSaidaRAV.Funcao)
                    {
                        case Modelos.ECodFuncao.Consultar:
                            { svcSaidaRAV.Funcao = ECodFuncao.Consultar; break; }
                        case Modelos.ECodFuncao.Efetivar:
                            { svcSaidaRAV.Funcao = ECodFuncao.Efetivar; break; }
                        case Modelos.ECodFuncao.Simulacao:
                            { svcSaidaRAV.Funcao = ECodFuncao.Simulacao; break; }
                    }

                    switch (modSaidaRAV.IndAnteEstoq)
                    {
                        case Modelos.ElndAntecEstoq.Nao:
                            { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Nao; break; }
                        case Modelos.ElndAntecEstoq.Sim:
                            { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Sim; break; }
                    }

                    svcSaidaRAV.IndContratoPortal = modSaidaRAV.IndContratoPortal;
                    svcSaidaRAV.IndPRFComercial = modSaidaRAV.IndPRFComercial;
                    svcSaidaRAV.NomeContrato = modSaidaRAV.NomeContato;
                    svcSaidaRAV.NumeroPDV = modSaidaRAV.NumeroPDV;
                    svcSaidaRAV.NumeroPDVRef = modSaidaRAV.NumeroPDVRef;
                    svcSaidaRAV.NumParcelaFim = modSaidaRAV.NumParcelaFim;
                    svcSaidaRAV.NumParcelaIni = modSaidaRAV.NumParcelaIni;

                    switch (modSaidaRAV.Periodicidade)
                    {
                        case Modelos.EPeriodicidade.Diario:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Diario; break; }
                        case Modelos.EPeriodicidade.Semanal:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Semanal; break; }
                        case Modelos.EPeriodicidade.Quinzenal:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Quinzenal; break; }
                    }

                    svcSaidaRAV.QtdeDiasCancelamento = modSaidaRAV.QtdeDiasCancelamento;

                    switch (modSaidaRAV.TipoRAV)
                    {
                        case Modelos.ElndProdutoAntecipa.Ambos:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Ambos; break; }
                        case Modelos.ElndProdutoAntecipa.Parcelado:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Parcelado; break; }
                        case Modelos.ElndProdutoAntecipa.Rotativo:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Rotativo; break; }
                    }

                    svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;

                    svcSaidaRAV.QtdeDiasCancelamento = modSaidaRAV.QtdeDiasCancelamento;
                    svcSaidaRAV.IndFull = modSaidaRAV.IndFull;
                    svcSaidaRAV.Bandeiras = MapearBandeiras(modSaidaRAV.Bandeiras);

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });
                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public Servicos.ModRAVAutomatico ConsultarRAVAutomaticoPersonalizado(int numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String _diaSemana, String _diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim });

                try
                {
                    Servicos.ModRAVAutomatico svcSaidaRAV = new Servicos.ModRAVAutomatico();
                    Modelos.ModRAVAutomatico modSaidaRAV = new NegPortalRAV().ConsultarRAVAutomaticoPersonalizado(numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim, _diaSemana, _diaAntecipacao, sBandeiras);
                    /*
                    Mapper.CreateMap<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>();
                    svcSaidaRAV = Mapper.Map<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>(modSaidaRAV);
                    */

                    switch (modSaidaRAV.Funcao)
                    {
                        case Modelos.ECodFuncao.Consultar:
                            { svcSaidaRAV.Funcao = ECodFuncao.Consultar; break; }
                        case Modelos.ECodFuncao.Efetivar:
                            { svcSaidaRAV.Funcao = ECodFuncao.Efetivar; break; }
                        case Modelos.ECodFuncao.Simulacao:
                            { svcSaidaRAV.Funcao = ECodFuncao.Simulacao; break; }
                    }

                    switch (modSaidaRAV.IndAnteEstoq)
                    {
                        case Modelos.ElndAntecEstoq.Nao:
                            { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Nao; break; }
                        case Modelos.ElndAntecEstoq.Sim:
                            { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Sim; break; }
                    }

                    svcSaidaRAV.IndContratoPortal = modSaidaRAV.IndContratoPortal;
                    svcSaidaRAV.IndPRFComercial = modSaidaRAV.IndPRFComercial;
                    svcSaidaRAV.NomeContrato = modSaidaRAV.NomeContato;
                    svcSaidaRAV.NumeroPDV = modSaidaRAV.NumeroPDV;
                    svcSaidaRAV.NumeroPDVRef = modSaidaRAV.NumeroPDVRef;
                    svcSaidaRAV.NumParcelaFim = modSaidaRAV.NumParcelaFim;
                    svcSaidaRAV.NumParcelaIni = modSaidaRAV.NumParcelaIni;

                    switch (modSaidaRAV.Periodicidade)
                    {
                        case Modelos.EPeriodicidade.Diario:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Diario; break; }
                        case Modelos.EPeriodicidade.Semanal:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Semanal; break; }
                        case Modelos.EPeriodicidade.Quinzenal:
                            { svcSaidaRAV.Periodicidade = EPeriodicidade.Quinzenal; break; }
                    }

                    svcSaidaRAV.QtdeDiasCancelamento = modSaidaRAV.QtdeDiasCancelamento;

                    switch (modSaidaRAV.TipoRAV)
                    {
                        case Modelos.ElndProdutoAntecipa.Ambos:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Ambos; break; }
                        case Modelos.ElndProdutoAntecipa.Parcelado:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Parcelado; break; }
                        case Modelos.ElndProdutoAntecipa.Rotativo:
                            { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Rotativo; break; }
                    }

                    svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;
                    if (modSaidaRAV.DadosRetorno != null)
                    {
                        svcSaidaRAV.DadosRetorno = new ModRAVAutomaticoDados();
                        svcSaidaRAV.DadosRetorno.TaxaCategoria = modSaidaRAV.DadosRetorno.TaxaCategoria;
                    }

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });
                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <param name="sBandeiras"></param>
        /// <returns>0 se foi realizado com sucesso, Código de retorno se não foi.</returns>
        public int EfetuarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade, string sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetua o RAV Automático"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade });
                    var retorno = new NegPortalRAV().EfetuarRAVAutomatico(numeroPDV, tipoVenda, periodicidade, sBandeiras);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Efetua o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <param name="sBandeiras"></param>
        /// <returns>0 se foi realizado com sucesso, Código de retorno se não foi.</returns>
        public Int32 EfetuarRAVAutomaticoPersonalizado(int numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String diaSemana, String diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetua o RAV Automático"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim });
                    Int32 retorno = new NegPortalRAV().EfetuarRAVAutomaticoPersonalizado(numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim, diaSemana, diaAntecipacao, sBandeiras);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Simula o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser simulado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        public Servicos.ModRAVAutomatico SimularRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Simula o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV, tipoVenda, periodicidade });

                try
                {
                    Servicos.ModRAVAutomatico svcSaidaRAV = null;
                    Modelos.ModRAVAutomatico modSaidaRAV = new NegPortalRAV().SimularRAVAutomatico(numeroPDV, tipoVenda, periodicidade);
                    /*
                    Mapper.CreateMap<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>();
                    svcSaidaRAV = Mapper.Map<Modelos.ModRAVAutomatico, Servicos.ModRAVAutomatico>(modSaidaRAV);
                    */
                    if (modSaidaRAV != null && modSaidaRAV.DataContrato != null)
                    {
                        svcSaidaRAV = new Servicos.ModRAVAutomatico();

                        svcSaidaRAV.CodigoProduto = modSaidaRAV.CodigoProduto;
                        svcSaidaRAV.CodMotivoExclusao = modSaidaRAV.CodMotivoExclusao;
                        svcSaidaRAV.CodSituacao = modSaidaRAV.CodSituacao;
                        svcSaidaRAV.CodVenda = modSaidaRAV.CodVenda;

                        //Dados Retorno
                        svcSaidaRAV.DadosRetorno.CodCategoria = modSaidaRAV.DadosRetorno.CodCategoria;
                        svcSaidaRAV.DadosRetorno.CodOpidAlteracao = modSaidaRAV.DadosRetorno.CodOpidAlteracao;
                        svcSaidaRAV.DadosRetorno.CodOpidAutorizacao = modSaidaRAV.DadosRetorno.CodOpidAutorizacao;
                        svcSaidaRAV.DadosRetorno.CodRetorno = modSaidaRAV.CodRetorno;
                        svcSaidaRAV.DadosRetorno.CodSituacaoPendente = modSaidaRAV.DadosRetorno.CodSituacaoPendente;
                        svcSaidaRAV.DadosRetorno.CPF_CNPJ = modSaidaRAV.DadosRetorno.CpfCnpj;
                        svcSaidaRAV.DadosRetorno.DataAgendaExclusao = modSaidaRAV.DadosRetorno.DataAgendaExclusao;
                        svcSaidaRAV.DadosRetorno.DataAlteracao = modSaidaRAV.DadosRetorno.DataAlteracao;
                        svcSaidaRAV.DadosRetorno.DataAutorizacao = modSaidaRAV.DadosRetorno.DataAutorizacao;
                        svcSaidaRAV.DadosRetorno.DataBaseAntecipacao = modSaidaRAV.DadosRetorno.DataBaseAntecipacao;
                        svcSaidaRAV.DadosRetorno.DataFimFidelizacao = modSaidaRAV.DadosRetorno.DataFimFidelizacao;
                        svcSaidaRAV.DadosRetorno.DataIniFidelizacao = modSaidaRAV.DadosRetorno.DataIniFidelizacao;
                        svcSaidaRAV.DadosRetorno.DataProximaAntecipacao = modSaidaRAV.DadosRetorno.DataProximaAntecipacao;
                        svcSaidaRAV.DadosRetorno.DescCategoria = modSaidaRAV.DadosRetorno.DescCategoria;
                        svcSaidaRAV.DadosRetorno.DescSituacaoCategoria = modSaidaRAV.DadosRetorno.DescSituacaoCategoria;
                        svcSaidaRAV.DadosRetorno.Estabelecimento = modSaidaRAV.DadosRetorno.Estabelecimento;
                        svcSaidaRAV.DadosRetorno.HoraAlteracao = modSaidaRAV.DadosRetorno.HoraAlteracao;
                        svcSaidaRAV.DadosRetorno.HoraAutorizacao = modSaidaRAV.DadosRetorno.HoraAutorizacao;
                        svcSaidaRAV.DadosRetorno.IndBloqueio = modSaidaRAV.DadosRetorno.IndBloqueio;
                        svcSaidaRAV.DadosRetorno.MsgRetorno = modSaidaRAV.DadosRetorno.MsgRetorno;
                        svcSaidaRAV.DadosRetorno.NumMatrix = modSaidaRAV.DadosRetorno.NumMatrix;
                        svcSaidaRAV.DadosRetorno.TaxaCategoria = modSaidaRAV.DadosRetorno.TaxaCategoria;
                        svcSaidaRAV.DadosRetorno.TaxaFidelizacao = modSaidaRAV.DadosRetorno.TaxaFidelizacao;
                        // CESSAÕ DE CRÉDITO
                        svcSaidaRAV.DadosRetorno.CodigoProdutoAntecipacao = modSaidaRAV.DadosRetorno.CodigoProdutoAntecipacao;
                        svcSaidaRAV.DadosRetorno.NomeProdutoAntecipacao = modSaidaRAV.DadosRetorno.NomeProdutoAntecipacao;
                        svcSaidaRAV.DadosRetorno.DescricaoProdutoAntecipacao = modSaidaRAV.DadosRetorno.DescricaoProdutoAntecipacao;

                        svcSaidaRAV.DataContrato = modSaidaRAV.DataContrato.Value;
                        svcSaidaRAV.DataIniEstoq = modSaidaRAV.DataIniEstoq;
                        svcSaidaRAV.DataVigenciaFim = modSaidaRAV.DataVigenciaFim;
                        svcSaidaRAV.DataVigenciaIni = modSaidaRAV.DataVigenciaIni;
                        svcSaidaRAV.DescMotivoExclusao = modSaidaRAV.DescMotivoExclusao;
                        svcSaidaRAV.DiaAntecipacao = modSaidaRAV.DiaAntecipacao;

                        switch (modSaidaRAV.DiaSemana)
                        {
                            case Modelos.EDiaSemana.Segunda:
                                { svcSaidaRAV.DiaSemana = EDiaSemana.Segunda; break; }
                            case Modelos.EDiaSemana.Terca:
                                { svcSaidaRAV.DiaSemana = EDiaSemana.Terca; break; }
                            case Modelos.EDiaSemana.Quarta:
                                { svcSaidaRAV.DiaSemana = EDiaSemana.Quarta; break; }
                            case Modelos.EDiaSemana.Quinta:
                                { svcSaidaRAV.DiaSemana = EDiaSemana.Quinta; break; }
                            case Modelos.EDiaSemana.Sexta:
                                { svcSaidaRAV.DiaSemana = EDiaSemana.Sexta; break; }
                        }

                        switch (modSaidaRAV.Funcao)
                        {
                            case Modelos.ECodFuncao.Consultar:
                                { svcSaidaRAV.Funcao = ECodFuncao.Consultar; break; }
                            case Modelos.ECodFuncao.Efetivar:
                                { svcSaidaRAV.Funcao = ECodFuncao.Efetivar; break; }
                            case Modelos.ECodFuncao.Simulacao:
                                { svcSaidaRAV.Funcao = ECodFuncao.Simulacao; break; }
                        }

                        switch (modSaidaRAV.IndAnteEstoq)
                        {
                            case Modelos.ElndAntecEstoq.Nao:
                                { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Nao; break; }
                            case Modelos.ElndAntecEstoq.Sim:
                                { svcSaidaRAV.IndAnteEstoq = ElndAntecEstoq.Sim; break; }
                        }

                        svcSaidaRAV.IndContratoPortal = modSaidaRAV.IndContratoPortal;
                        svcSaidaRAV.IndPRFComercial = modSaidaRAV.IndPRFComercial;
                        svcSaidaRAV.NomeContrato = modSaidaRAV.NomeContato;
                        svcSaidaRAV.NumeroPDV = modSaidaRAV.NumeroPDV;
                        svcSaidaRAV.NumeroPDVRef = modSaidaRAV.NumeroPDVRef;
                        svcSaidaRAV.NumParcelaFim = modSaidaRAV.NumParcelaFim;
                        svcSaidaRAV.NumParcelaIni = modSaidaRAV.NumParcelaIni;

                        switch (modSaidaRAV.Periodicidade)
                        {
                            case Modelos.EPeriodicidade.Diario:
                                { svcSaidaRAV.Periodicidade = EPeriodicidade.Diario; break; }
                            case Modelos.EPeriodicidade.Semanal:
                                { svcSaidaRAV.Periodicidade = EPeriodicidade.Semanal; break; }
                            case Modelos.EPeriodicidade.Quinzenal:
                                { svcSaidaRAV.Periodicidade = EPeriodicidade.Quinzenal; break; }
                        }

                        svcSaidaRAV.QtdeDiasCancelamento = modSaidaRAV.QtdeDiasCancelamento;

                        switch (modSaidaRAV.TipoRAV)
                        {
                            case Modelos.ElndProdutoAntecipa.Ambos:
                                { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Ambos; break; }
                            case Modelos.ElndProdutoAntecipa.Parcelado:
                                { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Parcelado; break; }
                            case Modelos.ElndProdutoAntecipa.Rotativo:
                                { svcSaidaRAV.TipoRAV = ElndProdutoAntecipa.Rotativo; break; }
                        }

                        svcSaidaRAV.ValorMinimo = modSaidaRAV.ValorMinimo;

                        svcSaidaRAV.IndFull = modSaidaRAV.IndFull;
                        svcSaidaRAV.Bandeiras = MapearBandeiras(modSaidaRAV.Bandeiras);
                    }
                    else
                    {
                        svcSaidaRAV = new ModRAVAutomatico();
                        svcSaidaRAV.DadosRetorno = new ModRAVAutomaticoDados() { CodRetorno = modSaidaRAV.CodRetorno, 
                                                                                 MsgRetorno = modSaidaRAV.MensagemRetorno};
                    }

                    Log.GravarLog(EventoLog.FimServico, new { svcSaidaRAV });
                    return svcSaidaRAV;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        #region RAV Email
        /// <summary>
        /// Salva os emails para as notificações de um RAV.
        /// </summary>
        /// <param name="dadosEmail">Classe de dados preenchida. Consultar documentação para informações.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool SalvarEmails(Servicos.ModRAVEmailEntradaSaida dadosemail)
        {
            using (Logger Log = Logger.IniciarLog("Salva os emails para as notificações de um RAV"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { dadosemail });

                    Modelos.ModRAVEmailEntradaSaida modDadosEmail = new Modelos.ModRAVEmailEntradaSaida();

                    modDadosEmail.IndEnviaEmail = dadosemail.IndEnviaEmail;
                    modDadosEmail.IndEnviaFluxoCaixa = dadosemail.IndEnviaFluxoCaixa;
                    modDadosEmail.IndEnviaResumoOperacao = dadosemail.IndEnviaResumoOperacao;
                    modDadosEmail.IndEnviaValoresPV = dadosemail.IndEnviaValoresPV;
                    modDadosEmail.IndRegistro = dadosemail.IndRegistro;
                    modDadosEmail.NumeroPDV = dadosemail.NumeroPDV;
                    modDadosEmail.ListaEmails = dadosemail.ListaEmails.Select(x => new Modelos.ModRAVEmail()
                    {
                        DataUltAlteracao = x.DataUltAlteracao,
                        DataUltInclusao = x.DataUltInclusao,
                        Email = x.Email,
                        Periodicidade = ConvertePeriodo(x.Periodicidade),
                        Sequencia = x.Sequencia,
                        Status = ConverteEmail(x.Status)
                    }).ToList();

                    var retorno = new NegPortalRAV().SalvarEmails(modDadosEmail);

                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Converte a status entre as entidades
        /// </summary>
        /// <param name="status">Status da entidade do serviço</param>
        /// <returns></returns>
        private Modelos.EStatusEmail ConverteEmail(Servicos.EStatusEmail status)
        {
            switch (status)
            {
                case EStatusEmail.Alterado:
                    return Modelos.EStatusEmail.Alterado;
                    break;
                case EStatusEmail.Incluso:
                    return Modelos.EStatusEmail.Incluso;
                    break;
                case EStatusEmail.Excluido:
                    return Modelos.EStatusEmail.Excluir;
                    break;
                default:
                    return Modelos.EStatusEmail.None;

            }
        }

        /// <summary>
        /// Converte o valor recebido pela tela para o enumerador relativo
        /// </summary>
        /// <param name="periodo">Flag do periodo</param>
        /// <returns></returns>
        private Modelos.EPeriodicidadeEmail ConvertePeriodo(Servicos.EPeriodicidadeEmail periodo)
        {
            switch (periodo)
            {
                case EPeriodicidadeEmail.Diario:
                    return Modelos.EPeriodicidadeEmail.Diario;
                    break;
                case EPeriodicidadeEmail.Mensal:
                    return Modelos.EPeriodicidadeEmail.Mensal;
                    break;
                case EPeriodicidadeEmail.Quinzenal:
                    return Modelos.EPeriodicidadeEmail.Quinzenal;
                    break;
                case EPeriodicidadeEmail.Semanal:
                    return Modelos.EPeriodicidadeEmail.Semanal;
                    break;
                default:
                    return Modelos.EPeriodicidadeEmail.Diario;
            }
        }

        /// <summary>
        /// Consulta os emails registrados no RAV de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados com a lista de emails no RAV.</returns>
        public Servicos.ModRAVEmailEntradaSaida ConsultarEmails(int numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os emails registrados no RAV"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPDV });

                try
                {
                    Modelos.ModRAVEmailEntradaSaida modEntradaSaida = new NegPortalRAV().ConsultarEmails(numeroPDV);

                    Servicos.ModRAVEmailEntradaSaida svcEmailEntradaSaida = new ModRAVEmailEntradaSaida();
                    /*
                    Mapper.CreateMap<Modelos.ModRAVEmailEntradaSaida, Servicos.ModRAVEmailEntradaSaida>();
                    svcEmailEntradaSaida = Mapper.Map<Modelos.ModRAVEmailEntradaSaida, Servicos.ModRAVEmailEntradaSaida>(modEntradaSaida);
                    */

                    svcEmailEntradaSaida.IndEnviaEmail = modEntradaSaida.IndEnviaEmail;
                    svcEmailEntradaSaida.IndEnviaFluxoCaixa = modEntradaSaida.IndEnviaFluxoCaixa;
                    svcEmailEntradaSaida.IndEnviaResumoOperacao = modEntradaSaida.IndEnviaResumoOperacao;
                    svcEmailEntradaSaida.IndEnviaValoresPV = modEntradaSaida.IndEnviaValoresPV;
                    svcEmailEntradaSaida.IndRegistro = modEntradaSaida.IndRegistro;

                    for (int i = 0; i < modEntradaSaida.ListaEmails.Count; i++)
                    {
                        Servicos.ModRAVEmail ravEmail = new ModRAVEmail();
                        ravEmail.DataUltAlteracao = modEntradaSaida.ListaEmails[i].DataUltAlteracao;
                        ravEmail.DataUltInclusao = modEntradaSaida.ListaEmails[i].DataUltInclusao;
                        ravEmail.Email = modEntradaSaida.ListaEmails[i].Email;

                        switch (modEntradaSaida.ListaEmails[i].Periodicidade)
                        {
                            case Modelos.EPeriodicidadeEmail.Diario:
                                { ravEmail.Periodicidade = EPeriodicidadeEmail.Diario; break; }
                            case Modelos.EPeriodicidadeEmail.Semanal:
                                { ravEmail.Periodicidade = EPeriodicidadeEmail.Semanal; break; }
                            case Modelos.EPeriodicidadeEmail.Quinzenal:
                                { ravEmail.Periodicidade = EPeriodicidadeEmail.Quinzenal; break; }
                            case Modelos.EPeriodicidadeEmail.Mensal:
                                { ravEmail.Periodicidade = EPeriodicidadeEmail.Mensal; break; }
                        }

                        ravEmail.Sequencia = modEntradaSaida.ListaEmails[i].Sequencia;

                        switch (modEntradaSaida.ListaEmails[i].Status)
                        {
                            case Modelos.EStatusEmail.Alterado:
                                { ravEmail.Status = EStatusEmail.Alterado; break; }
                            case Modelos.EStatusEmail.Incluso:
                                { ravEmail.Status = EStatusEmail.Incluso; break; }
                            case Modelos.EStatusEmail.None:
                                { ravEmail.Status = EStatusEmail.None; break; }
                        }

                        svcEmailEntradaSaida.ListaEmails.Add(ravEmail);
                    }

                    svcEmailEntradaSaida.NumeroPDV = modEntradaSaida.NumeroPDV;

                    Log.GravarLog(EventoLog.FimServico, new { svcEmailEntradaSaida });
                    return svcEmailEntradaSaida;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        #region RAV Senha
        /// <summary>
        /// Valida a senha de RAV de um PDV específico.
        /// </summary>
        /// <param name="senha">A senha.</param>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool ValidarSenha(string senha, int numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Valida a senha de RAV "))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numeroPDV });
                    var retorno = new NegPortalRAV().ValidarSenha(senha, numeroPDV);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Verifica o acesso ao RAV de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        public bool VerificarAcesso(int numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o acesso ao RAV"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numeroPDV });
                    var retorno = new NegPortalRAV().VerificarAcesso(numeroPDV);
                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = ex.Codigo, Fonte = ex.Fonte }, base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<ServicoRAVException>(
                        new ServicoRAVException() { Codigo = CODIGO_ERRO, Fonte = FONTE }, base.RecuperarExcecao(ex));
                }
            }
        }
        #endregion

        /// <summary>
        /// Mapeamento de bandeiras
        /// </summary>
        /// <param name="modBandeiras"></param>
        /// <returns></returns>
        public List<ModRAVAutomaticoBandeira> MapearBandeiras(List<Modelos.ModRAVAutomaticoBandeira> modBandeiras)
        {
            List<ModRAVAutomaticoBandeira> svcBandeiras = new List<ModRAVAutomaticoBandeira>();

            if (!object.Equals(modBandeiras, null))
            {
                foreach (Modelos.ModRAVAutomaticoBandeira modBandeira in modBandeiras)
                {
                    ModRAVAutomaticoBandeira svcBandeira = new ModRAVAutomaticoBandeira();

                    svcBandeira.CodBandeira = modBandeira.CodBandeira;
                    svcBandeira.DscBandeira = modBandeira.DscBandeira;
                    svcBandeira.IndSel = modBandeira.IndSel;
                    svcBandeiras.Add(svcBandeira);
                }
            }

            return svcBandeiras;
        }
    }
}
