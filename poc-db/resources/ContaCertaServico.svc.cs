using AutoMapper;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Serviço Conta Certa
    /// </summary>
    public class ContaCertaServico : ServicoBase, IContaCertaServico
    {

        /// <summary>
        /// Verifica se os terminais das filiais enviadas são do tipo Conta Certa
        /// </summary>
        /// <param name="filiais">Filiais a serem verificadas</param>
        /// <returns>Filiais atualizadas</returns>
        public List<Servicos.FilialTerminais> VerificaTerminalContaCerta(List<Servicos.FilialTerminais> filiais)
        {
            using (Logger Log = Logger.IniciarLog("Servico Conta Certa - VerificaTerminalContaCerta"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { Filiais = filiais });

                try
                {
                    var negocioContaCerta = new Negocio.ContaCerta();

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Servicos.Filial, Modelo.Filial>();
                    Mapper.CreateMap<Modelo.Filial, Servicos.Filial>();

                    Mapper.CreateMap<Servicos.TerminalBancario, Modelo.TerminalBancario>();
                    Mapper.CreateMap<Modelo.TerminalBancario, Servicos.TerminalBancario>();

                    Mapper.CreateMap<Servicos.TerminalContaCerta, Modelo.TerminalContaCerta>();
                    Mapper.CreateMap<Modelo.TerminalContaCerta, Servicos.TerminalContaCerta>();

                    Mapper.CreateMap<Servicos.FilialTerminais, Modelo.FilialTerminais>();
                    Mapper.CreateMap<Modelo.FilialTerminais, Servicos.FilialTerminais>();

                    filiais = Mapper.Map<List<Servicos.FilialTerminais>>(
                        negocioContaCerta.VerificaTerminalContaCerta(Mapper.Map<List<Modelo.FilialTerminais>>(filiais)));

                    Log.GravarLog(EventoLog.FimServico, new { Filiais = filiais });

                    return filiais;
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
