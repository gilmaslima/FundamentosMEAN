using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
    /// </summary>
    public class ConsultarVencimentosResumoVendaBLL : RegraDeNegocioBase
    {
        private static readonly ConsultarVencimentosResumoVendaBLL instance = new ConsultarVencimentosResumoVendaBLL();

        private ConsultarVencimentosResumoVendaBLL()
        {
        }

        public static ConsultarVencimentosResumoVendaBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ConsultarVencimentosResumoVendaRetornoDTO> Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarVencimentosResumoVendaEnvioDTO envio)
        {
            try
            {
                ConsultarResumoVendasAG ag = new ConsultarResumoVendasAG();

                List<ConsultarVencimentosResumoVendaRetornoDTO> result = ag.ConsultarVencimentosResumoVenda(out statusRetornoDTO, envio);

                if (statusRetornoDTO.CodigoRetorno != 0)
                {
                    return null;
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
