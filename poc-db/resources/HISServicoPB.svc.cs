using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades.Servicos
{
    /// <summary>
    /// Serviço para acesso ao serviço PB do módulo Outras Entidades
    /// </summary>
    public class HISServicoPB : ServicoBase, IHISServicoPB
    {
        /// <summary>
        /// PV764CB - Consulta os dados de quem(Bancos) consultou a Grade(SPB) no Portal Redecard.    
        /// </summary>
        /// <param name="mensagem">Mensagem de retorno da consulta </param>
        /// <param name="codRetorno">Código de retorno da consulta</param>
        /// <remarks>Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book:PV764CB, Programa: PB043</remarks>
        /// <returns>Lista dos bancos/usuários que consultaram a grade</returns>
        public List<Servicos.BancoGrade> ConsultarBanco(out string mensagem, out string codRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os dados de quem(Bancos) consultou a Grade(SPB) no Portal Redecard.  [PV764CB]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente);

                    List<Modelo.BancoGrade> lst = new Negocio.GradeSPB().ConsultarBanco(out mensagem, out codRetorno);

                    Log.GravarLog(EventoLog.RetornoAgente, new { mensagem, codRetorno });
                    List<Servicos.BancoGrade> result = PreencherModelo(lst).ToList();
                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;

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
        /// Consulta os dados da Grade para liquidação financeira(SPB) aos bancos.
        /// </summary>
        /// <param name="ispb">Codigo de ispb do banco no banco central.</param>
        /// <param name="usuario">Codigo do usuario(banco) que efetuou a pesquisa.</param>
        /// <param name="codRetorno">Código de retorno da consulta</param>
        /// <param name="mensagem">Mensagem de retorno da consulta </param>
        /// <param name="retorno">Cod de retorno (api)</param>
        /// <param name="dataContabil">Data contabil</param>
        /// <remarks>Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book:PV763CB, Programa: PB042</remarks>
        /// <returns>Lista de grade para liquidação financeira aos bancos.</returns>
        public List<Servicos.GradeLiquidacao> ExtrairDadosSPB(string ispb, string usuario, out int codRetorno, out string mensagem, out string retorno, out String dataContabil)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os dados da Grade para liquidação financeira(SPB) aos bancos.[PV763CB]"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {

                    Log.GravarLog(EventoLog.ChamadaAgente, new { ispb, usuario });

                    string dataRet = string.Empty;
                    List<Modelo.GradeLiquidacao> lst = new Negocio.GradeSPB().ExtrairDadosSPB(ispb, usuario, out codRetorno, out mensagem, out retorno, out dataContabil);
                    Log.GravarLog(EventoLog.RetornoAgente, new { codRetorno, mensagem, retorno, dataRet });

                    List<Servicos.GradeLiquidacao> result = PreencherModelo(lst).ToList();
                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;
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
        /// Dados da grade para liquidacao financeira (spb) ao bancos valores analitico por bandeira  
        /// </summary>
        /// <param name="ispb">Codigo de ispb do banco no banco central.</param>
        /// <param name="usuario">Codigo do usuario(banco) que efetuou a pesquisa.</param>
        /// <param name="codRetorno">Código de retorno da consulta</param>
        /// <param name="mensagem">Mensagem de retorno da consulta </param>
        /// <param name="retorno">Cod de retorno (api)</param>
        /// <param name="dataContabil">Data contabil</param>
        /// <remarks>Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book:PV761CB, Programa: PB041</remarks>
        /// <returns>Lista com grade para liquidacao financeira (spb) ao bancos valores analitico por bandeira.</returns>
        public Servicos.GradeLiquidacaoBandeira ExtrairDetalhesSPB(string ispb, string usuario, out int codRetorno, out string mensagem, out string retorno, out String dataContabil)
        {
            using (Logger Log = Logger.IniciarLog("Dados da grade para liquidacao financeira (spb) ao bancos valores analitico por bandeira [PV761CB]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    string dataRet = string.Empty;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { ispb, usuario });

                    Modelo.GradeLiquidacaoBandeira item = new Negocio.GradeSPB().ExtrairDetalhesSPB(ispb, usuario, out codRetorno, out mensagem, out retorno, out dataContabil);

                    Log.GravarLog(EventoLog.RetornoAgente, new { codRetorno, mensagem, retorno, dataRet });

                    Servicos.GradeLiquidacaoBandeira result = PreencherModelo(item);
                    Log.GravarLog(EventoLog.FimAgente, new { result });
                    return result; 

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
        #region Metodos auxiliares
        protected List<Servicos.BancoGrade> PreencherModelo(List<Modelo.BancoGrade> itens)
        {
            List<Servicos.BancoGrade> lstRetorno = new List<BancoGrade>();
            Servicos.BancoGrade modelo;
            foreach (Modelo.BancoGrade item in itens)
            {
                modelo = new BancoGrade();
                modelo.Ispb = item.Ispb;
                modelo.Descricao = item.Descricao;
                modelo.Banco = item.Banco;
                modelo.DataPesuisa = item.DataPesuisa;
                modelo.HoraPesuisa = item.HoraPesuisa;
                lstRetorno.Add(modelo);
            }
            return lstRetorno;
        }
        protected List<Servicos.GradeLiquidacao> PreencherModelo(List<Modelo.GradeLiquidacao> itens)
        {
            List<Servicos.GradeLiquidacao> lstRetorno = new List<GradeLiquidacao>();
            Servicos.GradeLiquidacao modelo;
            foreach (Modelo.GradeLiquidacao item in itens)
            {
                modelo = new GradeLiquidacao();
                modelo.Ispb = item.Ispb;
                modelo.Descricao = item.Descricao;
                modelo.Banco = item.Banco;
                modelo.Agencia = item.Agencia;
                modelo.ContaCorrente = item.ContaCorrente;
                modelo.Tipo = item.Tipo;
                modelo.TipoMovimentacao = item.TipoMovimentacao;
                modelo.TipoSolicitacao = item.TipoSolicitacao;
                modelo.ValorSaldoLiquidacao = item.ValorSaldoLiquidacao;
                lstRetorno.Add(modelo);
            }
            return lstRetorno;
        }
        protected Servicos.GradeLiquidacaoBandeira PreencherModelo(Modelo.GradeLiquidacaoBandeira item)
        {
            Servicos.GradeLiquidacaoBandeira modelo = new GradeLiquidacaoBandeira();
            modelo.Ispb = item.Ispb;
            modelo.Banco = item.Banco;
            modelo.Agencia = item.Agencia;
            modelo.ContaCorrente = item.ContaCorrente;
            modelo.Descricao = item.Descricao;
            modelo.TipoRegistro = item.TipoRegistro;
            modelo.TipoMovimentacao = item.TipoMovimentacao;
            modelo.TipoSolicitacao = item.TipoSolicitacao;
            modelo.ValorSaldoLiquidacao = item.ValorSaldoLiquidacao;

            modelo.ValorDebitoCabal = item.ValorDebitoCabal;
            modelo.ValorDebitoConstrucard = item.ValorDebitoConstrucard;
            modelo.ValorDebitoHipercard = item.ValorDebitoHipercard;
            modelo.ValorDebitoInstituicaoX = item.ValorDebitoInstituicaoX;
            modelo.ValorDebitoMaster = item.ValorDebitoMaster;
            modelo.ValorDebitoVisa = item.ValorDebitoVisa;
            modelo.ValorDebitoSicredi = item.ValorDebitoSicredi;
            modelo.ValorDebitoSaldo = item.ValorDebitoSaldo;
            modelo.ValorDebitoBanescard = item.ValorDebitoBanescard;
            modelo.ValorDebitoElo = item.ValorDebitoElo;

            modelo.ValorCreditoCabal = item.ValorCreditoCabal;
            modelo.ValorCreditoConstrucard = item.ValorCreditoConstrucard;
            modelo.ValorCreditoHipercard = item.ValorCreditoHipercard;
            modelo.ValorCreditoInstituicaoX = item.ValorCreditoInstituicaoX;
            modelo.ValorCreditoMaster = item.ValorCreditoMaster;
            modelo.ValorCreditoVisa = item.ValorCreditoVisa;
            modelo.ValorCreditoSicredi = item.ValorCreditoSicredi;
            modelo.ValorCreditoSaldo = item.ValorCreditoSaldo;
            modelo.ValorCreditoBanescard = item.ValorCreditoBanescard;
            modelo.ValorCreditoElo = item.ValorCreditoElo;

            modelo.SinalCreditoCabal = item.SinalCreditoCabal;
            modelo.SinalCreditoConstrucard = item.SinalCreditoConstrucard;
            modelo.SinalCreditoHipercard = item.SinalCreditoHipercard;
            modelo.SinalCreditoInstituicaoX = item.SinalCreditoInstituicaoX;
            modelo.SinalCreditoMaster = item.SinalCreditoMaster;
            modelo.SinalCreditoSaldo = item.SinalCreditoSaldo;
            modelo.SinalCreditoSicredi = item.SinalCreditoSicredi;
            modelo.SinalCreditoVisa = item.SinalCreditoVisa;
            modelo.SinalCreditoBanescard = item.SinalCreditoBanescard;
            modelo.SinalCreditoElo = item.SinalCreditoElo;

            modelo.SinalDebitoCabal = item.SinalDebitoCabal;
            modelo.SinalDebitoConstrucard = item.SinalDebitoConstrucard;
            modelo.SinalDebitoHipercard = item.SinalDebitoHipercard;
            modelo.SinalDebitoInstituicaoX = item.SinalDebitoInstituicaoX;
            modelo.SinalDebitoMaster = item.SinalDebitoMaster;
            modelo.SinalDebitoSaldo = item.SinalDebitoSaldo;
            modelo.SinalDebitoSicredi = item.SinalDebitoSicredi;
            modelo.SinalDebitoVisa = item.SinalDebitoVisa;
            modelo.SinalDebitoBanescard = item.SinalDebitoBanescard;
            modelo.SinalDebitoElo = item.SinalDebitoElo;
            return modelo;

        }

        #endregion
    }
}
