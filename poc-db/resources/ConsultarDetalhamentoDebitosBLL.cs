using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Agentes;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarDetalhamentoDebitosBLL : AbstractPesquisaComTotalizadorBLL<ConsultarDetalhamentoDebitosEnvioDTO, ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO>
    {
        private static readonly ConsultarDetalhamentoDebitosBLL instance = new ConsultarDetalhamentoDebitosBLL();

        private ConsultarDetalhamentoDebitosBLL()
        {
        }

        public static ConsultarDetalhamentoDebitosBLL Instance
        {
            get
            {
                return instance;
            }
        }

        protected override RetornoPesquisaComTotalizadorDTO<ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, ConsultarDetalhamentoDebitosEnvioDTO envio)
        {
            ConsultarExtratosAG ag = new ConsultarExtratosAG();

            ConsultarDetalhamentoDebitosRetornoDTO dto = null;

            //Verifica qual versão de programa no mainframe deve ser chamada
            if ("ISF".CompareTo(envio.Versao) == 0)
                dto = ag.ConsultarDetalhamentoDebitos(out statusRetornoDTO, envio);
            else                
                dto = ag.ConsultarDetalhamentoDebitosISD2(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            RetornoPesquisaComTotalizadorDTO<ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO> result = new RetornoPesquisaComTotalizadorDTO<ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO>();
            result.Registros = dto.Registros;
            result.Totalizador = dto.Totais;
            result.QuantidadeTotalRegistros = dto.Registros.Count;

            return result;
        }
    }
}
