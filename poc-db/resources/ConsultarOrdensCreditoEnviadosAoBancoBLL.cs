using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarOrdensCreditoEnviadosAoBancoBLL : AbstractPesquisaComTotalizadorBLL<ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO, BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO>
    {
        private static readonly ConsultarOrdensCreditoEnviadosAoBancoBLL instance = new ConsultarOrdensCreditoEnviadosAoBancoBLL();

        private ConsultarOrdensCreditoEnviadosAoBancoBLL()
        {
        }

        public static ConsultarOrdensCreditoEnviadosAoBancoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        protected override RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO envio)
        {
            ConsultarOrdensCreditoAG ag = new ConsultarOrdensCreditoAG();

            ConsultarOrdensCreditoEnviadosAoBancoRetornoDTO dto = ag.ConsultarOrdensCreditoEnviadosAoBanco(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO> result = new RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO>();
            result.Registros = dto.Registros;
            result.Totalizador = dto.Totais;
            result.QuantidadeTotalRegistros = dto.Registros.Count;

            //Recarga de Celular
            if (result.Registros != null)
            {
                foreach (BasicDTO registro in result.Registros)
                {
                    if (registro is ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO)
                    {
                        var detalhe = registro as ConsultarOrdensCreditoEnviadosAoBancoDetalheRetornoDTO;
                        //Se Código de Ajuste = 67, indica que é uma transação de Recarga de Celular
                        detalhe.IndicadorRecarga = detalhe.CodigoAjuste == 67;
                    }
                }
            }

            return result;
        }

        private List<BasicDTO> PreFiltroDT(List<BasicDTO> registros, Object parametrosFiltro)
        {
            return registros.FindAll(p => p.TipoRegistro == "DT");
        }

        public RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO> PesquisarDT(out StatusRetornoDTO statusRetornoDTO, ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO envioDTO, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            return PesquisarComFiltro<Object>(out statusRetornoDTO, envioDTO, null, PreFiltroDT, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
        }
    }
}
