using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using AutoMapper;
using Redecard.PN.Emissores.Negocio;

namespace Redecard.PN.Emissores.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HISServicoWF_Emissores" in code, svc and config file together.
    public class HisServicoWfEmissores : ServicoBase, IHisServicoWfEmissores
    {
        /// <summary>
        /// Efetua uma Solicitção de Tecnologia
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="entradaEmissao"></param>
        /// <returns></returns>
        public bool EfetuarSolicitacao(int numEmissor, DadosEmissao entradaEmissao, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Serviço EfetuarSolicitacao"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { numEmissor, entradaEmissao });
                    Mapper.CreateMap<DadosEmissao, Modelos.DadosEmissao>();
                    Mapper.CreateMap<PontoVenda, Modelos.PontoVenda>();
                    Mapper.CreateMap<DadosTelefone, Modelos.DadosTelefone>();
                    Mapper.CreateMap<EnderecoPadrao, Modelos.EnderecoPadrao>();
                    Mapper.CreateMap<DadosBancarios, Modelos.DadosBancarios>();
                    Mapper.CreateMap<DadosProprietario, Modelos.DadosProprietario>();

                    Mapper.CreateMap<DadosProdutosNegociados, Modelos.DadosProdutosNegociados>();

                    Modelos.DadosEmissao envio = Mapper.Map<DadosEmissao, Modelos.DadosEmissao>(entradaEmissao);
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numEmissor, envio });

                    bool retorno = new NegocioEmissores().EfetivarSolicitacao(numEmissor, envio, out codigoRetorno, out mensagemRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { retorno });

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
