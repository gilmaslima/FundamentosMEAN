using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
namespace Redecard.PN.OutrasEntidades.Agentes
{
    public class PB : AgentesBase
    {
        // codigo de erro >= 50400
        /// <summary>
        /// Consultar Banco SPB [PV764CB]
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <returns></returns>
        public List<Modelo.BancoGrade> ConsultarBanco(out string mensagem, out string retorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Banco SPB [PV764CB]"))
            {
                Log.GravarLog(EventoLog.InicioAgente);

                try
                {
                    COMTIPB.REG_COMM_INTR_OCC[] itens;
                    Log.GravarLog(EventoLog.ChamadaHIS);

#if DEBUG
                    mensagem = default(string);
                    retorno = default(string);
                    itens = new COMTIPB.REG_COMM_INTR_OCC[10];

                    for (int i = 0; i < 10; i++)
                    {
                        itens[i].REGIN3_BANCO = "\034";
                        itens[i].REGIN3_DATA = "\0\0\0\0";
                        itens[i].REGIN3_DESC = "\0teste asd asd";
                        itens[i].REGIN3_HORA = "\0000000";
                        itens[i].REGIN3_ISPB = "\0123123";
                        itens[i].REGIN3_USUARIO = "\0asdas";
                    }
#else
                    using (COMTIPB.COMTIPBClient consulta = new COMTIPB.COMTIPBClient())
                    {
                        string ret = consulta.consultarBanco(out mensagem, out retorno, out itens);
                    }

#endif
                    Log.GravarLog(EventoLog.RetornoHIS, new { mensagem, retorno, itens });
                    List<Modelo.BancoGrade> result = PreencheModelo(itens).ToList();
                    Log.GravarLog(EventoLog.FimAgente, new { result });
                    return result;

                }
                catch (Exception ex)
                {

                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// PV761CB - Dados da grade para liquidacao financeira (spb) ao bancos  valores analitico por bandeira   
        /// </summary>
        /// <param name="ispb"></param>
        /// <param name="usuario"></param>
        /// <param name="codRetorno"></param>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <param name="dataContabil"></param>
        /// <returns></returns>
        public Modelo.GradeLiquidacaoBandeira ExtrairDetalhesSPB(string ispb, string usuario,
                out int codRetorno, out string mensagem, out string retorno, out String dataContabil)
        {
            short retornoTratado = 0;
            int dataContabilTratada = 0;

            using (Logger Log = Logger.IniciarLog("Dados da grade para liquidacao financeira (spb) ao bancos  valores analitico por bandeira   [PV761CB]"))
            {
                Log.GravarLog(EventoLog.InicioAgente);

                try
                {

                    COMTIPB.REG_COMM_INTE_OCC dados;
                    Log.GravarLog(EventoLog.ChamadaHIS, new { ispb, usuario });

#if DEBUG
                    codRetorno = 0;
                    mensagem = string.Empty;
                    retorno = "00";
                    dataContabil = "000000";
                    dados = new COMTIPB.REG_COMM_INTE_OCC();
                    dados.REGIN3_AGENC = string.Empty;
                    dados.REGIN3_BANCO = 0;
                    dados.REGIN3_CONTA = string.Empty;
                    dados.REGIN3_DESC = string.Empty;
                    dados.REGIN3_ISPB = 0;
                    dados.REGIN3_MOVIM = string.Empty;
                    dados.REGIN3_SOLIC = string.Empty;
                    dados.REGIN3_TIPO = 0;
                    dados.REGIN3_VALOR = 0.00m;
                    dados.RG3_CRE_GRADE_CABAL = 7695.53m;
                    dados.RG3_CRE_GRADE_CABAL_SINAL = "C";
                    dados.RG3_CRE_GRADE_CONST = 0.00m;
                    dados.RG3_CRE_GRADE_CONST_SINAL = "D";
                    dados.RG3_CRE_GRADE_HIPER = 0.30m;
                    dados.RG3_CRE_GRADE_HIPER_SINAL = "D";
                    dados.RG3_CRE_GRADE_MCI = 8232781.35m;
                    dados.RG3_CRE_GRADE_MCI_SINAL = "C";
                    dados.RG3_CRE_GRADE_SICREDI = 37084.52m;
                    dados.RG3_CRE_GRADE_SICREDI_SINAL = "C";
                    dados.RG3_CRE_GRADE_SLDO = 21500889.49m;
                    dados.RG3_CRE_GRADE_SLDO_SINAL = "C";
                    dados.RG3_CRE_GRADE_VISA = 13223328.09m;
                    dados.RG3_CRE_GRADE_VISA_SINAL = "C";
                    dados.RG3_CRE_GRADE_X = 0;
                    dados.RG3_CRE_GRADE_X_SINAL = "D";
                    dados.RG3_CRE_GRADE_BANESCARD = 0;
                    dados.RG3_CRE_GRADE_BANES_SINAL = "D";
                    dados.RG3_CRE_GRADE_ELO = 0;
                    dados.RG3_CRE_GRADE_ELO_SINAL = "C";
                    dados.RG3_DEB_GRADE_CABAL = 0;
                    dados.RG3_DEB_GRADE_CABAL_SINAL = "D";
                    dados.RG3_DEB_GRADE_CONST = 0;
                    dados.RG3_DEB_GRADE_CONST_SINAL = "D";
                    dados.RG3_DEB_GRADE_HIPER = 109.76m;
                    dados.RG3_DEB_GRADE_HIPER_SINAL = "D";
                    dados.RG3_DEB_GRADE_MCI = 12611046.47m;
                    dados.RG3_DEB_GRADE_MCI_SINAL = "D";
                    dados.RG3_DEB_GRADE_SICREDI = 0;
                    dados.RG3_DEB_GRADE_SICREDI_SINAL = "D";
                    dados.RG3_DEB_GRADE_SLDO = 12611046.47m;
                    dados.RG3_DEB_GRADE_SLDO_SINAL = "D";
                    dados.RG3_DEB_GRADE_VISA = 0;
                    dados.RG3_DEB_GRADE_VISA_SINAL = "D";
                    dados.RG3_DEB_GRADE_X = 0;
                    dados.RG3_DEB_GRADE_X_SINAL = "D";
                    dados.RG3_DEB_GRADE_BANESCARD = 0;
                    dados.RG3_DEB_GRADE_BANES_SINAL = "D";
                    dados.RG3_DEB_GRADE_ELO = 0;
                    dados.RG3_DEB_GRADE_ELO_SINAL = "D";
#else
                using (COMTIPB.COMTIPBClient client = new COMTIPB.COMTIPBClient())
                {
                    short ret = client.DETgradeSPB(out mensagem, out retornoTratado, out dataContabilTratada, out dados, ispb, usuario);
                    codRetorno = ret;
                }
#endif
                    Log.GravarLog(EventoLog.RetornoHIS, new { mensagem, retornoTratado, dataContabilTratada, dados });
                    retorno = retornoTratado.ToString();
                    dataContabil = dataContabilTratada.ToString();

                    Modelo.GradeLiquidacaoBandeira result = PreencheModelo(dados);

                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// PV763CB - Grade para liquidacao financeira (spb) ao bancos 
        /// </summary>
        /// <param name="ispb"></param>
        /// <param name="usuario"></param>
        /// <param name="codRetorno"></param>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <param name="dataContabil"></param>
        /// <returns></returns>
        public List<Modelo.GradeLiquidacao> ExtrairDadosSPB(string ispb, string usuario,
                out int codRetorno, out string mensagem, out string retorno, out string dataContabil)
        {
            using (Logger Log = Logger.IniciarLog("Grade para liquidacao financeira (spb) ao bancos  [PV763CB]"))
            {
                Log.GravarLog(EventoLog.InicioAgente);

                try
                {
                    List<Modelo.GradeLiquidacao> grade = new List<Modelo.GradeLiquidacao>();

                    COMTIPB.REG_COMM_INTE_OCC_PV763CB[] dados = new COMTIPB.REG_COMM_INTE_OCC_PV763CB[5];
                    Log.GravarLog(EventoLog.ChamadaHIS, new { ispb, usuario });

#if DEBUG

                    codRetorno = 0;
                    mensagem = string.Empty;
                    retorno = "00";
                    dataContabil = "130827";
                    dados[0] = new COMTIPB.REG_COMM_INTE_OCC_PV763CB
                        {
                            REGIN2_AGENC = string.Empty,
                            REGIN2_BANCO = "00001",
                            REGIN2_CONTA = string.Empty,
                            REGIN2_DESC = "BRASIL",
                            REGIN2_ISPB = "00000000",
                            REGIN2_MOVIM = "C",
                            REGIN2_SOLIC = "B",
                            REGIN2_TIPO = "01",
                            REGIN2_VALOR = "000000888984302"
                        };
                    dados[1] = new COMTIPB.REG_COMM_INTE_OCC_PV763CB
                    {
                        REGIN2_AGENC = string.Empty,
                        REGIN2_BANCO = "00409",
                        REGIN2_CONTA = string.Empty,
                        REGIN2_DESC = "UNIBANCO",
                        REGIN2_ISPB = "33700394",
                        REGIN2_MOVIM = string.Empty,
                        REGIN2_SOLIC = string.Empty,
                        REGIN2_TIPO = "02",
                        REGIN2_VALOR = "000000888984302"
                    };
                    dados[2] = new COMTIPB.REG_COMM_INTE_OCC_PV763CB
                    {
                        REGIN2_AGENC = string.Empty,
                        REGIN2_BANCO = string.Empty,
                        REGIN2_CONTA = string.Empty,
                        REGIN2_DESC = string.Empty,
                        REGIN2_ISPB = string.Empty,
                        REGIN2_MOVIM = string.Empty,
                        REGIN2_SOLIC = string.Empty,
                        REGIN2_TIPO = "00",
                        REGIN2_VALOR = "000000000000000"
                    };
                    dados[3] = new COMTIPB.REG_COMM_INTE_OCC_PV763CB
                    {
                        REGIN2_AGENC = string.Empty,
                        REGIN2_BANCO = string.Empty,
                        REGIN2_CONTA = string.Empty,
                        REGIN2_DESC = string.Empty,
                        REGIN2_ISPB = string.Empty,
                        REGIN2_MOVIM = string.Empty,
                        REGIN2_SOLIC = string.Empty,
                        REGIN2_TIPO = "00",
                        REGIN2_VALOR = "000000000000000"
                    };
                    dados[4] = new COMTIPB.REG_COMM_INTE_OCC_PV763CB
                    {
                        REGIN2_AGENC = string.Empty,
                        REGIN2_BANCO = string.Empty,
                        REGIN2_CONTA = string.Empty,
                        REGIN2_DESC = string.Empty,
                        REGIN2_ISPB = string.Empty,
                        REGIN2_MOVIM = string.Empty,
                        REGIN2_SOLIC = string.Empty,
                        REGIN2_TIPO = "00",
                        REGIN2_VALOR = "000000000000000"
                    };
#else

                using (COMTIPB.COMTIPBClient client = new COMTIPB.COMTIPBClient())
                {
                    string ret = client.gradeSPB(out mensagem, out retorno, out dataContabil, out dados, ispb, usuario);
                    codRetorno = ret.ToInt32(0);
                }
#endif
                    Log.GravarLog(EventoLog.RetornoHIS, new { mensagem, retorno, dataContabil, dados });
                    List<Modelo.GradeLiquidacao> result = PreencheModelo(dados).ToList();
                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #region Preenche Modelo

        private List<Modelo.BancoGrade> PreencheModelo(COMTIPB.REG_COMM_INTR_OCC[] itens)
        {
            List<Modelo.BancoGrade> lstRetorno = new List<Modelo.BancoGrade>();
            if (itens != null)
            {
                Modelo.BancoGrade modelo;
                foreach (COMTIPB.REG_COMM_INTR_OCC item in itens)
                {
                    modelo = new Modelo.BancoGrade();
                    modelo.Banco = item.REGIN3_BANCO.ToInt32(0);
                    modelo.Descricao = item.REGIN3_DESC;
                    modelo.DataPesuisa = ParseDate(item.REGIN3_DATA, "yyyyMMdd");
                    modelo.HoraPesuisa = ParseTime(item.REGIN3_HORA, "hhmmss");
                    modelo.Ispb = item.REGIN3_ISPB.ToInt32(0);
                    modelo.Usuario = item.REGIN3_USUARIO;
                    lstRetorno.Add(modelo);
                }

            }
            return lstRetorno;
        }
        private Modelo.GradeLiquidacaoBandeira PreencheModelo(COMTIPB.REG_COMM_INTE_OCC item)
        {
            Modelo.GradeLiquidacaoBandeira modelo = new Modelo.GradeLiquidacaoBandeira();

            modelo.Ispb = item.REGIN3_ISPB;
            modelo.Banco = item.REGIN3_BANCO;
            modelo.Agencia = item.REGIN3_AGENC;
            modelo.ContaCorrente = item.REGIN3_CONTA.ToInt32(0).ToString();
            modelo.Descricao = item.REGIN3_DESC;
            modelo.TipoRegistro = item.REGIN3_TIPO;
            modelo.TipoMovimentacao = item.REGIN3_MOVIM.Replace("\0", "");
            modelo.TipoSolicitacao = item.REGIN3_SOLIC;
            modelo.ValorSaldoLiquidacao = item.RG3_DEB_GRADE_SLDO;

            modelo.ValorDebitoCabal = item.RG3_DEB_GRADE_CABAL;
            modelo.ValorDebitoConstrucard = item.RG3_DEB_GRADE_CONST;
            modelo.ValorDebitoHipercard = item.RG3_DEB_GRADE_HIPER;
            modelo.ValorDebitoInstituicaoX = item.RG3_DEB_GRADE_X;
            modelo.ValorDebitoMaster = item.RG3_DEB_GRADE_MCI;
            modelo.ValorDebitoVisa = item.RG3_DEB_GRADE_VISA;
            modelo.ValorDebitoSicredi = item.RG3_DEB_GRADE_SICREDI;
            modelo.ValorDebitoSaldo = item.RG3_DEB_GRADE_SLDO;
            modelo.ValorDebitoBanescard = item.RG3_DEB_GRADE_BANESCARD;
            modelo.ValorDebitoElo = item.RG3_DEB_GRADE_ELO;

            modelo.ValorCreditoCabal = item.RG3_CRE_GRADE_CABAL;
            modelo.ValorCreditoConstrucard = item.RG3_CRE_GRADE_CONST;
            modelo.ValorCreditoHipercard = item.RG3_CRE_GRADE_HIPER;
            modelo.ValorCreditoInstituicaoX = item.RG3_CRE_GRADE_X;
            modelo.ValorCreditoMaster = item.RG3_CRE_GRADE_MCI;
            modelo.ValorCreditoVisa = item.RG3_CRE_GRADE_VISA;
            modelo.ValorCreditoSicredi = item.RG3_CRE_GRADE_SICREDI;
            modelo.ValorCreditoSaldo = item.RG3_CRE_GRADE_SLDO;
            modelo.ValorCreditoBanescard = item.RG3_CRE_GRADE_BANESCARD;
            modelo.ValorCreditoElo = item.RG3_CRE_GRADE_ELO;

            modelo.SinalCreditoCabal = item.RG3_CRE_GRADE_CABAL_SINAL.Replace("\0", "");
            modelo.SinalCreditoConstrucard = item.RG3_CRE_GRADE_CONST_SINAL.Replace("\0", "");
            modelo.SinalCreditoHipercard = item.RG3_CRE_GRADE_HIPER_SINAL.Replace("\0", "");
            modelo.SinalCreditoInstituicaoX = item.RG3_CRE_GRADE_X_SINAL.Replace("\0", "");
            modelo.SinalCreditoMaster = item.RG3_CRE_GRADE_MCI_SINAL.Replace("\0", "");
            modelo.SinalCreditoSaldo = item.RG3_CRE_GRADE_SLDO_SINAL.Replace("\0", "");
            modelo.SinalCreditoSicredi = item.RG3_CRE_GRADE_SICREDI_SINAL.Replace("\0", "");
            modelo.SinalCreditoVisa = item.RG3_CRE_GRADE_VISA_SINAL.Replace("\0", "");
            modelo.SinalCreditoBanescard = item.RG3_CRE_GRADE_BANES_SINAL.Replace("\0", "");
            modelo.SinalCreditoElo = item.RG3_CRE_GRADE_ELO_SINAL.Replace("\0", "");

            modelo.SinalDebitoCabal = item.RG3_DEB_GRADE_CABAL_SINAL.Replace("\0", "");
            modelo.SinalDebitoConstrucard = item.RG3_DEB_GRADE_CONST_SINAL.Replace("\0", "");
            modelo.SinalDebitoHipercard = item.RG3_DEB_GRADE_HIPER_SINAL.Replace("\0", "");
            modelo.SinalDebitoInstituicaoX = item.RG3_DEB_GRADE_X_SINAL.Replace("\0", "");
            modelo.SinalDebitoMaster = item.RG3_DEB_GRADE_MCI_SINAL.Replace("\0", "");
            modelo.SinalDebitoSaldo = item.RG3_DEB_GRADE_SLDO_SINAL.Replace("\0", "");
            modelo.SinalDebitoSicredi = item.RG3_DEB_GRADE_SICREDI_SINAL.Replace("\0", "");
            modelo.SinalDebitoVisa = item.RG3_DEB_GRADE_VISA_SINAL.Replace("\0", "");
            modelo.SinalDebitoBanescard = item.RG3_DEB_GRADE_BANES_SINAL.Replace("\0", "");
            modelo.SinalDebitoElo = item.RG3_DEB_GRADE_ELO_SINAL.Replace("\0", "");

            return modelo;

        }
        private List<Modelo.GradeLiquidacao> PreencheModelo(COMTIPB.REG_COMM_INTE_OCC_PV763CB[] itens)
        {
            List<Modelo.GradeLiquidacao> lstRetorno = new List<Modelo.GradeLiquidacao>();
            if (itens != null)
            {
                Modelo.GradeLiquidacao modelo;
                foreach (COMTIPB.REG_COMM_INTE_OCC_PV763CB item in itens)
                {
                    modelo = new Modelo.GradeLiquidacao();
                    modelo.Agencia = item.REGIN2_AGENC;
                    modelo.Banco = item.REGIN2_BANCO.ToInt32(0);
                    modelo.ContaCorrente = item.REGIN2_CONTA;
                    modelo.Descricao = item.REGIN2_DESC;
                    modelo.Ispb = item.REGIN2_ISPB.ToInt32(0);
                    modelo.Tipo = item.REGIN2_TIPO.ToInt32(0);
                    modelo.TipoMovimentacao = item.REGIN2_MOVIM;
                    modelo.TipoSolicitacao = item.REGIN2_SOLIC;
                    modelo.ValorSaldoLiquidacao = item.REGIN2_VALOR.ToDecimalNull(0).Value / 100;
                    lstRetorno.Add(modelo);
                }

            }
            return lstRetorno;
        }
        #endregion
        #region Metodos auxiliares
        protected TimeSpan ParseTime(object value, string format)
        {
            TimeSpan time;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

            if (TimeSpan.TryParseExact(value.ToString(), format, provider, out time))
            {

                return time;
            }
            else
            {
                return default(TimeSpan);
            }
        }
        protected DateTime ParseDate(object value, string format)
        {
            DateTime date;

            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(value.ToString(), format, provider, System.Globalization.DateTimeStyles.None, out date))
                return date;
            else
                return default(DateTime);
        }
        #endregion
    }
}
