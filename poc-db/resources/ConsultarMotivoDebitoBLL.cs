using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultarMotivoDebitoBLL
    {
        private static readonly ConsultarMotivoDebitoBLL instance = new ConsultarMotivoDebitoBLL();

        private ConsultarMotivoDebitoBLL()
        {
        }

        public static ConsultarMotivoDebitoBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public ConsultarMotivoDebitoRetornoDTO Pesquisar(out StatusRetornoDTO statusRetornoDTO, ConsultarMotivoDebitoEnvioDTO envioDTO)
        {
            ConsultarLiquidacoesAG ag = new ConsultarLiquidacoesAG();

            ConsultarMotivoDebitoRetornoDTO result = null;

            //Verifica qual versão de programa no mainframe deve ser chamada
            if ("ISF".CompareTo(envioDTO.Versao) == 0)
                result = ag.ConsultarMotivoDebito(out statusRetornoDTO, envioDTO);
            else                
                result = ag.ConsultarMotivoDebitoISD3(out statusRetornoDTO, envioDTO);

            if (statusRetornoDTO.CodigoRetorno != 0)
            {
                return null;
            }

            return result;
        }
    }
}
