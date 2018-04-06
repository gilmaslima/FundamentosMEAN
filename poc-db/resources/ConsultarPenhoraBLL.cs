using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    public class ConsultarPenhoraBLL : AbstractPesquisaComTotalizadorBLL<ConsultarPenhoraEnvioDTO, BasicDTO, ConsultarPenhoraTotaisRetornoDTO>
    {
        private static readonly ConsultarPenhoraBLL instance = new ConsultarPenhoraBLL();

        private ConsultarPenhoraBLL()
        {
        }

        public static ConsultarPenhoraBLL Instance
        {
            get
            {
                return instance;
            }
        }

        protected override RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarPenhoraTotaisRetornoDTO> ExecutarPesquisa(out StatusRetornoDTO statusRetornoDTO, ConsultarPenhoraEnvioDTO envio)
        {
            ConsultarSuspensaoAG ag = new ConsultarSuspensaoAG();

            ConsultarPenhoraRetornoDTO dto = ag.ConsultarPenhora(out statusRetornoDTO, envio);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            //PR - ConsultarPenhoraNumeroProcessoRetornoDTO
            //DT - ConsultarPenhoraDetalheProcessoCreditoRetornoDTO
            //T1 - ConsultarPenhoraTotalBandeiraRetornoDTO
            //TP - ConsultarPenhoraTotalSemBandeiraRetornoDTO

            List<BasicDTO> registros = new List<BasicDTO>();

            bool primeiroRegistroDT = true;

            foreach (BasicDTO basicDTO in dto.Registros)
            {
                string tipoRegistro = basicDTO.TipoRegistro;

                if (tipoRegistro == "PR")
                {
                    primeiroRegistroDT = true;

                    registros.Add((ConsultarPenhoraNumeroProcessoRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "DT")
                {
                    if (primeiroRegistroDT)
                    {
                        // adiciona o header.
                        registros.Add(ObterHeader("HDT"));

                        primeiroRegistroDT = false;
                    }

                    registros.Add((ConsultarPenhoraDetalheProcessoCreditoRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "T1")
                {
                    //registros.Add((ConsultarPenhoraTotalBandeiraRetornoDTO)basicDTO);
                }
                else if (tipoRegistro == "TP")
                {
                    registros.Add((ConsultarPenhoraTotalSemBandeiraRetornoDTO)basicDTO);
                }
            }

            RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarPenhoraTotaisRetornoDTO> result = new RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarPenhoraTotaisRetornoDTO>();
            result.Registros = registros;
            result.Totalizador = dto.Totais;
            result.QuantidadeTotalRegistros = registros.Count;

            return result;
        }

        private BasicDTO ObterHeader(string tipoRegistro)
        {
            BasicDTO result = new BasicDTO();
            result.TipoRegistro = tipoRegistro;

            return result;
        }
    }
}
