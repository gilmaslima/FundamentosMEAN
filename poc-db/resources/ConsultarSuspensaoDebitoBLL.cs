using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos.
    /// </summary>
    public class ConsultarSuspensaoDebitoBLL : AbstractPesquisaComTotalizadorBLL<ConsultarSuspensaoEnvioDTO, BasicDTO, ConsultarSuspensaoTotaisRetornoDTO>
    {
        private static readonly ConsultarSuspensaoDebitoBLL instance = new ConsultarSuspensaoDebitoBLL();

        private ConsultarSuspensaoDebitoBLL()
        {
        }

        public static ConsultarSuspensaoDebitoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        protected override RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, ConsultarSuspensaoEnvioDTO envio)
        {
            ConsultarSuspensaoAG ag = new ConsultarSuspensaoAG();

            // suspensões de débito.
            envio.TipoSuspensao = "D";

            ConsultarSuspensaoRetornoDTO dto = ag.ConsultarSuspensao(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> result = new RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO>();
            result.Registros = dto.Registros;
            result.Totalizador = dto.Totais;
            result.QuantidadeTotalRegistros = dto.Registros.Count;

            return result;
        }

        private List<BasicDTO> PreFiltroDT(List<BasicDTO> registros, Object parametrosFiltro)
        {
            return registros.FindAll(p => p.TipoRegistro == "DT");
        }

        public RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> PesquisarDT(out StatusRetornoDTO statusRetornoDTO, ConsultarSuspensaoEnvioDTO envioDTO, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            return PesquisarComFiltro<Object>(out statusRetornoDTO, envioDTO, null, PreFiltroDT, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
        }
    }
}
