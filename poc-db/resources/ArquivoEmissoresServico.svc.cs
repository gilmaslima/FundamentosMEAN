using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using System.IO;
using Redecard.PN.Emissores.Negocio;
using AutoMapper;

namespace Redecard.PN.Emissores.Servicos
{
    public class ArquivoEmissoresServico : ServicoBase, IArquivoEmissoresServico
    {
        public System.IO.Stream DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo)
        {
            using (Logger Log = Logger.IniciarLog("Serviço DownloadArquivo"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codEmissor, mesArquivo, anoArquivo });

                    MemoryStream stream = new MemoryStream();
                    byte[] buffer = new NegocioEmissores().DownloadArquivo(codEmissor, mesArquivo, anoArquivo);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Position = 0;
                    return stream;
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


        public void SalvarArquivo(DadosArquivo dadosArquivo)
        {
            using (Logger Log = Logger.IniciarLog("Serviço SalvarArquivo"))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(dadosArquivo.Arquivo))
                    {
                        Log.GravarLog(EventoLog.InicioServico, new { dadosArquivo.Tamanho, dadosArquivo.NomeArquivo, dadosArquivo.CodigoEmissor, dadosArquivo.Mes, dadosArquivo.Ano });
                        byte[] conteudoArquivo = reader.ReadBytes(dadosArquivo.Tamanho);

                        bool retorno = new NegocioEmissores().SalvarArquivo(conteudoArquivo, dadosArquivo.NomeArquivo, dadosArquivo.CodigoEmissor, dadosArquivo.Mes, dadosArquivo.Ano, DateTime.Now);

                        Log.GravarLog(EventoLog.FimServico, new { retorno });
                    }

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


        public List<DownloadMes> ObterPeriodosDisponiveis(string codEmissor, string anoInicial, string anoFinal)
        {
            using (Logger Log = Logger.IniciarLog("Serviço ObterPeriodosDisponiveis"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { codEmissor, anoInicial, anoFinal });

                    MemoryStream stream = new MemoryStream();

                    List<Modelos.DownloadMes> lstRetornoConsulta = new NegocioEmissores().ObterPeriodosDisponiveis(codEmissor, anoInicial, anoFinal);

                    Mapper.CreateMap<Modelos.DownloadMes, DownloadMes>();

                    List<DownloadMes> lstRetorno = Mapper.Map<List<Modelos.DownloadMes>, List<DownloadMes>>(lstRetornoConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { lstRetorno });

                    return lstRetorno;
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
