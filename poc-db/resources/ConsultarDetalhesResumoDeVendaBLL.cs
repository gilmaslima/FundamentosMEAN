using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarDetalhesResumoDeVendaBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarDetalhesResumoDeVendaBLL instance = new ConsultarDetalhesResumoDeVendaBLL();

        private ConsultarDetalhesResumoDeVendaBLL()
        {
        }

        public static ConsultarDetalhesResumoDeVendaBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarDetalhesResumoDeVendaRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarDetalhesResumoDeVendaEnvioDTO envio)
        {
            try
            {
                List<ConsultarDetalhesResumoDeVendaRetornoDTO> result = new List<ConsultarDetalhesResumoDeVendaRetornoDTO>();

                ConsultarResumoVendasAG ag = new ConsultarResumoVendasAG();

                ConsultarWACA700EnvioDTO envioWACA700 = new ConsultarWACA700EnvioDTO();
                envioWACA700.NumeroEstabelecimento = envio.NumeroEstabelecimento;
                envioWACA700.NumeroResumoVenda = envio.NumeroResumoVenda;
                envioWACA700.DataApresentacao = envio.DataApresentacao;

                List<ConsultarWACA700RetornoDTO> registrosWACA700 = ag.ConsultarWACA700(out statusRetornoDTO, envioWACA700);

                if (statusRetornoDTO.CodigoRetorno != 0)
                {
                    return null;
                }

                foreach (ConsultarWACA700RetornoDTO registroWACA700 in registrosWACA700)
                {
                    ConsultarDetalhesResumoDeVendaRetornoDTO retornoDTO = new ConsultarDetalhesResumoDeVendaRetornoDTO();
                    retornoDTO.Detalhe = registroWACA700.Detalhe;
                    retornoDTO.NumeroMes = registroWACA700.NumeroMes;
                    retornoDTO.Timestamp = registroWACA700.Timestamp;
                    retornoDTO.TipoResumoVenda = registroWACA700.TipoResumoVenda;

                    ConsultarWACA701EnvioDTO envioWACA701 = new ConsultarWACA701EnvioDTO();
                    envioWACA701.NumeroEstabelecimento = envio.NumeroEstabelecimento;
                    envioWACA701.NumeroResumoVenda = envio.NumeroResumoVenda;
                    envioWACA701.DataApresentacao = envio.DataApresentacao;
                    envioWACA701.TipoResumoVenda = registroWACA700.TipoResumoVenda;
                    envioWACA701.Timestamp = registroWACA700.Timestamp;

                    ConsultarWACA701RetornoDTO registroWACA701 = ag.ConsultarWACA701(out statusRetornoDTO, envioWACA701);

                    if (statusRetornoDTO.CodigoRetorno != 0)
                    {
                        continue;
                    }

                    retornoDTO.ResumoVenda = registroWACA701.ResumoVenda;
                    retornoDTO.ValorApresentado = registroWACA701.ValorApresentado;
                    retornoDTO.QuantidadeComprovantesVenda = registroWACA701.QuantidadeComprovantesVenda;
                    retornoDTO.ValorApurado = registroWACA701.ValorApurado;
                    retornoDTO.DataApresentacaoRetornado = registroWACA701.DataApresentacaoRetornado;
                    retornoDTO.ValorDesconto = registroWACA701.ValorDesconto;
                    retornoDTO.DataProcessamento = registroWACA701.DataProcessamento;
                    retornoDTO.ValorGorjetaTaxaEmbarque = registroWACA701.ValorGorjetaTaxaEmbarque;
                    retornoDTO.TipoResumo = registroWACA701.TipoResumo;
                    retornoDTO.ValorCotacao = registroWACA701.ValorCotacao;
                    retornoDTO.TipoMoeda = registroWACA701.TipoMoeda;
                    retornoDTO.IndicadorTaxaEmbarque = registroWACA701.IndicadorTaxaEmbarque;

                    result.Add(retornoDTO);
                }

                return result;
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
    }
}
