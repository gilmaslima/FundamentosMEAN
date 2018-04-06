using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoInformeDIRF;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class InformeIRAG : AgentesBase
    {
        #region WACA075 - Dirf.
        /// <summary>
        /// WACA075 - Dirf.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarExtratoDirfRetornoDTO ConsultarExtratoDirf(out StatusRetornoDTO statusRetornoDTO, ConsultarExtratoDirfEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarExtratoDirf";
            using (Logger Log = Logger.IniciarLog("Dirf [WACA075]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    using (DirfsClient cliente = new DirfsClient())
                    {
                        string razaoSocialEstabelecimento;
                        string comarcaEstabelecimento;
                        string enderecoEstabelecimento;
                        string bairroEstabelecimento;
                        string cidadeEstabelecimento;
                        string estadoEstabelecimento;
                        int cepEstabelecimento;
                        string codigoMalaDiretaEstabelecimento;
                        EstabelecimentoDirf[] estabelecimentos;
                        Emissor[] emissores;
                        string mensagemRetorno;
                        string reservaDados = default(string);
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        short anoBaseDirf = envio.AnoBaseDirf;
                        string cnpjEstabelecimento = string.Empty;

                        if (envio.CnpjEstabelecimento != null)
                        {
                            cnpjEstabelecimento = envio.CnpjEstabelecimento.PadLeft(14, '0');
                        }

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { numeroEstabelecimento, anoBaseDirf, cnpjEstabelecimento });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarExtratoDirf(out razaoSocialEstabelecimento, out comarcaEstabelecimento, out enderecoEstabelecimento, out bairroEstabelecimento, out cidadeEstabelecimento, out estadoEstabelecimento, out cepEstabelecimento, out codigoMalaDiretaEstabelecimento, out estabelecimentos, out emissores, out mensagemRetorno, out reservaDados, numeroEstabelecimento, anoBaseDirf, cnpjEstabelecimento);

                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            razaoSocialEstabelecimento = "Razão social";
                            comarcaEstabelecimento = "Comarca";
                            enderecoEstabelecimento = "Av. Paulista";
                            bairroEstabelecimento = "Cerqueira Cesar";
                            cidadeEstabelecimento = "São Paulo";
                            estadoEstabelecimento = "SP";
                            cepEstabelecimento = 05102012;
                            codigoMalaDiretaEstabelecimento = "Cód. mala direta";

                            estabelecimentos = new EstabelecimentoDirf[12];

                            for (int i = 0; i < 12; i++)
                            {

                                estabelecimentos[i] = new EstabelecimentoDirf();
                                estabelecimentos[i].ValorCobrado = 22222M;
                                estabelecimentos[i].ValorIrRecebido = 333333M;
                                estabelecimentos[i].ValorRecebido = 444444M;
                                estabelecimentos[i].ValorRepassadoEmissor = 777777M;
                                estabelecimentos[i].SinalValorCobrado = "+";
                                estabelecimentos[i].SinalValorIrRecebido = "+";
                                estabelecimentos[i].SinalValorRecebido = "+";
                                estabelecimentos[i].SinalValorRepassadoEmissor = "+";
                            }


                            emissores = new Emissor[3];

                            emissores[0] = new Emissor();
                            emissores[0].Cnpj = 301234560987612;
                            emissores[0].Nome = "Nome 1";
                            emissores[0].ValorIrEmissor1 = 28052012M;
                            emissores[0].ValorIrEmissor10 = 28052012M;
                            emissores[0].ValorIrEmissor11 = 28052012M;
                            emissores[0].ValorIrEmissor12 = 28052012M;
                            emissores[0].ValorIrEmissor2 = 28052012M;
                            emissores[0].ValorIrEmissor3 = 28052012M;
                            emissores[0].ValorIrEmissor4 = 28052012M;
                            emissores[0].ValorIrEmissor5 = 28052012M;
                            emissores[0].ValorIrEmissor6 = 28052012M;
                            emissores[0].ValorIrEmissor7 = 28052012M;
                            emissores[0].ValorIrEmissor8 = 28052012M;
                            emissores[0].ValorIrEmissor9 = 28052012M;
                            emissores[0].ValorRepassadoEmissor1 = 28052012M;
                            emissores[0].ValorRepassadoEmissor10 = 28052012M;
                            emissores[0].ValorRepassadoEmissor11 = 28052012M;
                            emissores[0].ValorRepassadoEmissor12 = 28052012M;
                            emissores[0].ValorRepassadoEmissor2 = 28052012M;
                            emissores[0].ValorRepassadoEmissor3 = 28052012M;
                            emissores[0].ValorRepassadoEmissor4 = 28052012M;
                            emissores[0].ValorRepassadoEmissor5 = 28052012M;
                            emissores[0].ValorRepassadoEmissor6 = 28052012M;
                            emissores[0].ValorRepassadoEmissor7 = 28052012M;
                            emissores[0].ValorRepassadoEmissor8 = 28052012M;
                            emissores[0].ValorRepassadoEmissor9 = 28052012M;
                            emissores[0].SinalValorIrEmissor1 = "+";
                            emissores[0].SinalValorIrEmissor10 = "+";
                            emissores[0].SinalValorIrEmissor11 = "+";
                            emissores[0].SinalValorIrEmissor12 = "+";
                            emissores[0].SinalValorIrEmissor2 = "+";
                            emissores[0].SinalValorIrEmissor3 = "+";
                            emissores[0].SinalValorIrEmissor4 = "+";
                            emissores[0].SinalValorIrEmissor5 = "+";
                            emissores[0].SinalValorIrEmissor6 = "+";
                            emissores[0].SinalValorIrEmissor7 = "+";
                            emissores[0].SinalValorIrEmissor8 = "+";
                            emissores[0].SinalValorIrEmissor9 = "+";
                            emissores[0].SinalValorRepassadoEmissor1 = "+";
                            emissores[0].SinalValorRepassadoEmissor10 = "+";
                            emissores[0].SinalValorRepassadoEmissor11 = "+";
                            emissores[0].SinalValorRepassadoEmissor12 = "+";
                            emissores[0].SinalValorRepassadoEmissor2 = "+";
                            emissores[0].SinalValorRepassadoEmissor3 = "+";
                            emissores[0].SinalValorRepassadoEmissor4 = "+";
                            emissores[0].SinalValorRepassadoEmissor5 = "+";
                            emissores[0].SinalValorRepassadoEmissor6 = "+";
                            emissores[0].SinalValorRepassadoEmissor7 = "+";
                            emissores[0].SinalValorRepassadoEmissor8 = "+";
                            emissores[0].SinalValorRepassadoEmissor9 = "+";

                            emissores[1] = new Emissor();
                            emissores[1].Cnpj = 301234560987612;
                            emissores[1].Nome = "Nome 1";
                            emissores[1].ValorIrEmissor1 = 28052012M;
                            emissores[1].ValorIrEmissor10 = 28052012M;
                            emissores[1].ValorIrEmissor11 = 28052012M;
                            emissores[1].ValorIrEmissor12 = 28052012M;
                            emissores[1].ValorIrEmissor2 = 28052012M;
                            emissores[1].ValorIrEmissor3 = 28052012M;
                            emissores[1].ValorIrEmissor4 = 28052012M;
                            emissores[1].ValorIrEmissor5 = 28052012M;
                            emissores[1].ValorIrEmissor6 = 28052012M;
                            emissores[1].ValorIrEmissor7 = 28052012M;
                            emissores[1].ValorIrEmissor8 = 28052012M;
                            emissores[1].ValorIrEmissor9 = 28052012M;
                            emissores[1].ValorRepassadoEmissor1 = 28052012M;
                            emissores[1].ValorRepassadoEmissor10 = 28052012M;
                            emissores[1].ValorRepassadoEmissor11 = 28052012M;
                            emissores[1].ValorRepassadoEmissor12 = 28052012M;
                            emissores[1].ValorRepassadoEmissor2 = 28052012M;
                            emissores[1].ValorRepassadoEmissor3 = 28052012M;
                            emissores[1].ValorRepassadoEmissor4 = 28052012M;
                            emissores[1].ValorRepassadoEmissor5 = 28052012M;
                            emissores[1].ValorRepassadoEmissor6 = 28052012M;
                            emissores[1].ValorRepassadoEmissor7 = 28052012M;
                            emissores[1].ValorRepassadoEmissor8 = 28052012M;
                            emissores[1].ValorRepassadoEmissor9 = 28052012M;
                            emissores[1].SinalValorIrEmissor1 = "+";
                            emissores[1].SinalValorIrEmissor10 = "+";
                            emissores[1].SinalValorIrEmissor11 = "+";
                            emissores[1].SinalValorIrEmissor12 = "+";
                            emissores[1].SinalValorIrEmissor2 = "+";
                            emissores[1].SinalValorIrEmissor3 = "+";
                            emissores[1].SinalValorIrEmissor4 = "+";
                            emissores[1].SinalValorIrEmissor5 = "+";
                            emissores[1].SinalValorIrEmissor6 = "+";
                            emissores[1].SinalValorIrEmissor7 = "+";
                            emissores[1].SinalValorIrEmissor8 = "+";
                            emissores[1].SinalValorIrEmissor9 = "+";
                            emissores[1].SinalValorRepassadoEmissor1 = "+";
                            emissores[1].SinalValorRepassadoEmissor10 = "+";
                            emissores[1].SinalValorRepassadoEmissor11 = "+";
                            emissores[1].SinalValorRepassadoEmissor12 = "+";
                            emissores[1].SinalValorRepassadoEmissor2 = "+";
                            emissores[1].SinalValorRepassadoEmissor3 = "+";
                            emissores[1].SinalValorRepassadoEmissor4 = "+";
                            emissores[1].SinalValorRepassadoEmissor5 = "+";
                            emissores[1].SinalValorRepassadoEmissor6 = "+";
                            emissores[1].SinalValorRepassadoEmissor7 = "+";
                            emissores[1].SinalValorRepassadoEmissor8 = "+";
                            emissores[1].SinalValorRepassadoEmissor9 = "+";

                            emissores[2] = new Emissor();
                            emissores[2].Cnpj = 301234560987612;
                            emissores[2].Nome = "Nome 1";
                            emissores[2].ValorIrEmissor1 = 28052012M;
                            emissores[2].ValorIrEmissor10 = 28052012M;
                            emissores[2].ValorIrEmissor11 = 28052012M;
                            emissores[2].ValorIrEmissor12 = 28052012M;
                            emissores[2].ValorIrEmissor2 = 28052012M;
                            emissores[2].ValorIrEmissor3 = 28052012M;
                            emissores[2].ValorIrEmissor4 = 28052012M;
                            emissores[2].ValorIrEmissor5 = 28052012M;
                            emissores[2].ValorIrEmissor6 = 28052012M;
                            emissores[2].ValorIrEmissor7 = 28052012M;
                            emissores[2].ValorIrEmissor8 = 28052012M;
                            emissores[2].ValorIrEmissor9 = 28052012M;
                            emissores[2].ValorRepassadoEmissor1 = 28052012M;
                            emissores[2].ValorRepassadoEmissor10 = 28052012M;
                            emissores[2].ValorRepassadoEmissor11 = 28052012M;
                            emissores[2].ValorRepassadoEmissor12 = 28052012M;
                            emissores[2].ValorRepassadoEmissor2 = 28052012M;
                            emissores[2].ValorRepassadoEmissor3 = 28052012M;
                            emissores[2].ValorRepassadoEmissor4 = 28052012M;
                            emissores[2].ValorRepassadoEmissor5 = 28052012M;
                            emissores[2].ValorRepassadoEmissor6 = 28052012M;
                            emissores[2].ValorRepassadoEmissor7 = 28052012M;
                            emissores[2].ValorRepassadoEmissor8 = 28052012M;
                            emissores[2].ValorRepassadoEmissor9 = 28052012M;
                            emissores[2].SinalValorIrEmissor1 = "+";
                            emissores[2].SinalValorIrEmissor10 = "+";
                            emissores[2].SinalValorIrEmissor11 = "+";
                            emissores[2].SinalValorIrEmissor12 = "+";
                            emissores[2].SinalValorIrEmissor2 = "+";
                            emissores[2].SinalValorIrEmissor3 = "+";
                            emissores[2].SinalValorIrEmissor4 = "+";
                            emissores[2].SinalValorIrEmissor5 = "+";
                            emissores[2].SinalValorIrEmissor6 = "+";
                            emissores[2].SinalValorIrEmissor7 = "+";
                            emissores[2].SinalValorIrEmissor8 = "+";
                            emissores[2].SinalValorIrEmissor9 = "+";
                            emissores[2].SinalValorRepassadoEmissor1 = "+";
                            emissores[2].SinalValorRepassadoEmissor10 = "+";
                            emissores[2].SinalValorRepassadoEmissor11 = "+";
                            emissores[2].SinalValorRepassadoEmissor12 = "+";
                            emissores[2].SinalValorRepassadoEmissor2 = "+";
                            emissores[2].SinalValorRepassadoEmissor3 = "+";
                            emissores[2].SinalValorRepassadoEmissor4 = "+";
                            emissores[2].SinalValorRepassadoEmissor5 = "+";
                            emissores[2].SinalValorRepassadoEmissor6 = "+";
                            emissores[2].SinalValorRepassadoEmissor7 = "+";
                            emissores[2].SinalValorRepassadoEmissor8 = "+";
                            emissores[2].SinalValorRepassadoEmissor9 = "+";
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, razaoSocialEstabelecimento, comarcaEstabelecimento, enderecoEstabelecimento, bairroEstabelecimento, cidadeEstabelecimento, estadoEstabelecimento, cepEstabelecimento, codigoMalaDiretaEstabelecimento, estabelecimentos, emissores, reservaDados });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        ConsultarExtratoDirfRetornoDTO result = TradutorResultadoInformeDIRF.TraduzirRetornoConsultarExtratoDirf(razaoSocialEstabelecimento, comarcaEstabelecimento, enderecoEstabelecimento, bairroEstabelecimento, cidadeEstabelecimento, estadoEstabelecimento, cepEstabelecimento, codigoMalaDiretaEstabelecimento, estabelecimentos, emissores);

                        Log.GravarLog(EventoLog.FimAgente, new { result });

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region WACA079 - Dirf.
        /// <summary>
        /// WACA079 - Dirf.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <returns></returns>
        public ConsultarAnosBaseDirfRetornoDTO ConsultarAnosBaseDirf(out StatusRetornoDTO statusRetornoDTO)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarExtratoEmail";
            using (Logger Log = Logger.IniciarLog("Dirf [WACA079]"))
            {


                Log.GravarLog(EventoLog.InicioAgente);

                try
                {
                    using (DirfsClient cliente = new DirfsClient())
                    {
                        short[] anosBase;
                        string mensagemRetorno;
                        string reservaDados = default(string);
                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS);

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarAnosBaseDirf(out anosBase, out mensagemRetorno, out reservaDados);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            anosBase = new short[3];
                            anosBase[0] = 2001;
                            anosBase[1] = 2002;
                            anosBase[2] = 2003;

                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, reservaDados });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        ConsultarAnosBaseDirfRetornoDTO result = TradutorResultadoInformeDIRF.TraduzirRetornoConsultarAnosBaseDirf(anosBase);

                        Log.GravarLog(EventoLog.FimAgente, new { result });

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion
    }
}
