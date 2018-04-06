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
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Modelos;
using Redecard.PN.RAV.Agentes;

namespace Redecard.PN.RAV.Negocios
{
    public class NegPortalRAV : RegraDeNegocioBase
    {
        #region Variáveis e Construtor
        private AgPortalRAV agentesPortalRAV = null;
        
        public NegPortalRAV()
        {
            if (agentesPortalRAV == null)
            { agentesPortalRAV = AgPortalRAV.GetInstance(); }
        }
        #endregion

        #region RAV Avulso

        public Modelos.MA30 ExecutarMA30(Modelos.MA30 chamadaMA30)
        {
            using (Logger Log = Logger.IniciarLog("Execução do RAV Avulso"))
            {
                try
                {
                    Modelos.MA30 retornoMA30 = new MA30();

                    retornoMA30 = agentesPortalRAV.ExecutarMA30(chamadaMA30);

                    return retornoMA30;

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Verifica o RAV Avulso disponível.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida VerificarRAVDisponivel(Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV });

                ModRAVAvulsoSaida ravSaida = null;
                try
                {
                    if (numeroPDV != 0)
                    {
                        ravSaida = agentesPortalRAV.VerificarRAVDisponivel(numeroPDV);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new Exception("Número PDV inválido.");
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível através da URA (tipo do crédito identificado no método).
        /// </summary>
        /// <param name="numeroPDV">Número da Entidade</param>
        /// <returns>Modelo com os dados de saída do RAV Avulso</returns>
        public ModRAVAvulsoSaida VerificarRAVDisponivelURA(Int32 numeroPDV)
        {
            short tipoCredito = 0;
            ModRAVAvulsoSaida ravSaida = null;

            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível através da URA - Chamada sem o tipo de Crédito"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV });

                try
                {
                    if ((DateTime.Now.Hour == 14 && DateTime.Now.Minute >= 30) || DateTime.Now.Hour > 14)
                        tipoCredito = 1;

                    ravSaida = this.VerificarRAVDisponivelURA(numeroPDV, tipoCredito);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                Log.GravarLog(EventoLog.FimNegocio, new { ravSaida });
                return ravSaida;
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
        public ModRAVAvulsoSaida VerificarRAVDisponivelURA(Int32 numeroPDV, short tipoCredito)
        {
            using (Logger Log = Logger.IniciarLog("Verifica o RAV Avulso disponível através da URA"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoCredito });

                ModRAVAvulsoSaida ravSaida = null;
                try
                {
                    if (numeroPDV != 0)
                    {
                        ravSaida = agentesPortalRAV.VerificarRAVDisponivelURA(numeroPDV, tipoCredito);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new Exception("Número PDV inválido.");
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravSaida });
                return ravSaida;
            }
        }


        /// <summary>
        /// Consulta o RAV Avulso.
        /// </summary>
        /// <param name="entradaRAV">Dados sobre o RAV, todos os campos precisam ser preenchidos.</param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorAntecipado"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida ConsultarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorAntecipado)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Avulso"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { entradaRAV, numeroPDV, tipoCredito, valorAntecipado });

                ModRAVAvulsoSaida ravSaida = null;

                try
                {
                    if (entradaRAV != null && numeroPDV != 0 && VerificarCamposEntradaRAV(entradaRAV))
                    {
                        ravSaida = agentesPortalRAV.ConsultarRAVAvulso(entradaRAV, numeroPDV, tipoCredito, valorAntecipado);
                    }
                    else
                    {
                        Logger.GravarErro("Dados de Entrada e/ou Número PDV inválidos.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Dados de Entrada e/ou Número PDV inválidos."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Efetua o RAV Avulso.
        /// </summary>
        /// <param name="entradaRAV">Dados sobre o RAV, todos os campos precisam ser preenchidos.</param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorSolicitado"></param>
        /// <returns></returns>
        public Int32 EfetuarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorSolicitado)
        {
            using (Logger Log = Logger.IniciarLog("Efetua o RAV Avulso"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { entradaRAV, numeroPDV, tipoCredito, valorAntecipado = valorSolicitado });

                int codigoRetorno = 0;

                try
                {
                    if (entradaRAV != null && numeroPDV != 0 && VerificarCamposEntradaRAV(entradaRAV))
                    {
                        codigoRetorno = agentesPortalRAV.EfetuarRAVAvulso(entradaRAV, numeroPDV, tipoCredito, valorSolicitado);
                    }
                    else
                    {
                        Logger.GravarErro("Dados de Entrada e/ou Número PDV inválidos.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Dados de Entrada e/ou Número PDV inválidos."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { codigoRetorno});
                return codigoRetorno;
            }
        }
        
        //Verificar se todos os automáticos também são preenchidos?
        /// <summary>
        /// Método que verifica se os campos da entidade RavAvulsoEntrada foram todos preenchidos.
        /// </summary>
        /// <param name="entradaRAV"></param>
        /// <returns></returns>
        private Boolean VerificarCamposEntradaRAV(ModRAVAvulsoEntrada entradaRAV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica se os campos da entidade RavAvulsoEntrada foram todos preenchidos."))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { entradaRAV });

                Boolean status = true;

                //if (entradaRAV.DiasCredito == 0)
                //{ status = false; }

                if (entradaRAV.NumeroPDV == 0)
                { status = false; }

                //if (entradaRAV.ValorAntecipado == 0)
                //{ status = false; }

                Log.GravarLog(EventoLog.FimNegocio, new { status });
                return status;
            }
        }
        #endregion

        #region RAV Automático
        /// <summary>
        /// Consulta o RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAutomatico ConsultarRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoVenda, periodicidade });

                ModRAVAutomatico ravDados = null;

                try
                {
                    if (numeroPDV != 0)
                    {
                        ravDados = agentesPortalRAV.ConsultarRAVAutomatico(numeroPDV, tipoVenda, periodicidade);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravDados });
                return ravDados;
            }
        }

        /// <summary>
        /// Consulta o RAV Automático personalizado.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        public ModRAVAutomatico ConsultarRAVAutomaticoPersonalizado(Int32 numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String _diaSemana, String _diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Consulta o RAV Automático personalizado"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim });
                ModRAVAutomatico ravDados = null;

                try
                {
                    if (numeroPDV != 0)
                    {
                        ravDados = agentesPortalRAV.ConsultarRAVAutomaticoPersonalizado(numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim, _diaSemana, _diaAntecipacao, sBandeiras);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravDados });
                return ravDados;
            }
        }

        /// <summary>
        /// Efetiva o RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public int EfetuarRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade, string sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetiva o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoVenda, periodicidade });
                try
                {
                    if (numeroPDV != 0)
                    {
                        return agentesPortalRAV.EfetuarRAVAutomatico(numeroPDV, tipoVenda, periodicidade, sBandeiras);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Efetiva o RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Int32 EfetuarRAVAutomaticoPersonalizado(Int32 numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String diaSemana, String diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetiva o RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim});
                Int32 status = 0;

                try
                {
                    if (numeroPDV != 0)
                    {
                        status = agentesPortalRAV.EfetuarRAVAutomaticoPersonalizado(numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim, diaSemana, diaAntecipacao, sBandeiras);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { status });
                return status;
            }
        }

        /// <summary>
        /// Simula um RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAutomatico SimularRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Simula um RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV, tipoVenda, periodicidade });

                ModRAVAutomatico ravDados = null;

                try
                {
                    if (numeroPDV != 0)
                    {
                        ravDados = agentesPortalRAV.SimularRAVAutomatico(numeroPDV, tipoVenda, periodicidade);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravDados });
                return ravDados;
            }
        }
        #endregion

        #region RAV Email
        /// <summary>
        /// Consulta e-mails do PDV passado de parâmetro
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVEmailEntradaSaida ConsultarEmails(Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de e-mails"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV });

                ModRAVEmailEntradaSaida ravSaida = null;

                try
                {
                    if (numeroPDV != 0)
                    {
                        ravSaida = agentesPortalRAV.ConsultarEmails(numeroPDV);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido.");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Salva os e-mails
        /// </summary>
        /// <param name="dadosEmail"></param>
        /// <returns></returns>
        public Boolean SalvarEmails(ModRAVEmailEntradaSaida dadosEmail)
        {
            using (Logger Log = Logger.IniciarLog("Salva e-mails"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { dadosEmail });

                Boolean status = false;

                try
                {
                    if (dadosEmail.NumeroPDV != 0)
                    {
                        status = agentesPortalRAV.SalvarEmails(dadosEmail);
                    }
                    else
                    {
                        Logger.GravarErro("Número PDV inválido");
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE, new Exception("Número PDV inválido."));
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { status });
                return status;
            }
        }
        #endregion

        #region RAV Senha
        /// <summary>
        /// Valida a senha de um PDV.
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean ValidarSenha(String senha, Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Validação de senha"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV });

                Boolean status = false;

                try
                {
                    status = agentesPortalRAV.ValidarSenha(senha, numeroPDV);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { status });
                return status;
            }
        }
        
        /// <summary>
        /// Verifica o acesso de um PDV.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean VerificarAcesso(Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verifica acesso"))
            {
                Log.GravarLog(EventoLog.InicioNegocio, new { numeroPDV });

                Boolean status = false;

                try
                {
                    status = agentesPortalRAV.VerificarAcesso(numeroPDV);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimNegocio, new { status });
                return status;
            }
        }
        #endregion
    }
}
