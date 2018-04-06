using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.Tradutor
{
    public static class TradutorConsultarExtratoDirf
    {
        public static ConsultarExtratoDirfEnvioDTO TraduzirEnvioConsultarExtratoDirf(ConsultarExtratoDirfEnvio de)
        {
            ConsultarExtratoDirfEnvioDTO para = new ConsultarExtratoDirfEnvioDTO();
            para.NumeroEstabelecimento = de.NumeroEstabelecimento;
            para.AnoBaseDirf = de.AnoBaseDirf;
            para.CnpjEstabelecimento = de.CnpjEstabelecimento;

            return para;
        }

        public static ConsultarExtratoDirfRetorno TraduzirRetornoConsultarExtratoDirf(ConsultarExtratoDirfRetornoDTO de)
        {
            ConsultarExtratoDirfRetorno para = new ConsultarExtratoDirfRetorno();
            para.RazaoSocialEstabelecimento = de.RazaoSocialEstabelecimento;
            para.ComarcaEstabelecimento = de.ComarcaEstabelecimento;
            para.EnderecoEstabelecimento = de.EnderecoEstabelecimento;
            para.BairroEstabelecimento = de.BairroEstabelecimento;
            para.CidadeEstabelecimento = de.CidadeEstabelecimento;
            para.EstadoEstabelecimento = de.EstadoEstabelecimento;
            para.CepEstabelecimento = de.CepEstabelecimento;
            para.CodigoMalaDiretaEstabelecimento = de.CodigoMalaDiretaEstabelecimento;
            para.Estabelecimentos = TraduzirRetornoListaConsultarExtratoDirfEstabelecimento(de.Estabelecimentos);
            para.Emissores = TraduzirRetornoListaConsultarExtratoDirfEmissor(de.Emissores);

            return para;
        }

        private static List<ConsultarExtratoDirfEmissorRetorno> TraduzirRetornoListaConsultarExtratoDirfEmissor(List<ConsultarExtratoDirfEmissorRetornoDTO> list)
        {
            List<ConsultarExtratoDirfEmissorRetorno> result = new List<ConsultarExtratoDirfEmissorRetorno>();

            foreach (ConsultarExtratoDirfEmissorRetornoDTO de in list)
            {
                ConsultarExtratoDirfEmissorRetorno para = TraduzirRetornoConsultarExtratoDirfEmissor(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarExtratoDirfEmissorRetorno TraduzirRetornoConsultarExtratoDirfEmissor(ConsultarExtratoDirfEmissorRetornoDTO de)
        {
            ConsultarExtratoDirfEmissorRetorno para = new ConsultarExtratoDirfEmissorRetorno();
            para.Cnpj = de.Cnpj;
            para.Nome = de.Nome;
            para.ValorIrEmissor1 = de.ValorIrEmissor1;
            para.ValorIrEmissor2 = de.ValorIrEmissor2;
            para.ValorIrEmissor3 = de.ValorIrEmissor3;
            para.ValorIrEmissor4 = de.ValorIrEmissor4;
            para.ValorIrEmissor5 = de.ValorIrEmissor5;
            para.ValorIrEmissor6 = de.ValorIrEmissor6;
            para.ValorIrEmissor7 = de.ValorIrEmissor7;
            para.ValorIrEmissor8 = de.ValorIrEmissor8;
            para.ValorIrEmissor9 = de.ValorIrEmissor9;
            para.ValorIrEmissor10 = de.ValorIrEmissor10;
            para.ValorIrEmissor11 = de.ValorIrEmissor11;
            para.ValorIrEmissor12 = de.ValorIrEmissor12;
            para.ValorRepassadoEmissor1 = de.ValorRepassadoEmissor1;
            para.ValorRepassadoEmissor2 = de.ValorRepassadoEmissor2;
            para.ValorRepassadoEmissor3 = de.ValorRepassadoEmissor3;
            para.ValorRepassadoEmissor4 = de.ValorRepassadoEmissor4;
            para.ValorRepassadoEmissor5 = de.ValorRepassadoEmissor5;
            para.ValorRepassadoEmissor6 = de.ValorRepassadoEmissor6;
            para.ValorRepassadoEmissor7 = de.ValorRepassadoEmissor7;
            para.ValorRepassadoEmissor8 = de.ValorRepassadoEmissor8;
            para.ValorRepassadoEmissor9 = de.ValorRepassadoEmissor9;
            para.ValorRepassadoEmissor10 = de.ValorRepassadoEmissor10;
            para.ValorRepassadoEmissor11 = de.ValorRepassadoEmissor11;
            para.ValorRepassadoEmissor12 = de.ValorRepassadoEmissor12;

            return para;
        }

        private static List<ConsultarExtratoDirfEstabelecimentoRetorno> TraduzirRetornoListaConsultarExtratoDirfEstabelecimento(List<ConsultarExtratoDirfEstabelecimentoRetornoDTO> list)
        {
            List<ConsultarExtratoDirfEstabelecimentoRetorno> result = new List<ConsultarExtratoDirfEstabelecimentoRetorno>();

            foreach (ConsultarExtratoDirfEstabelecimentoRetornoDTO de in list)
            {
                ConsultarExtratoDirfEstabelecimentoRetorno para = TraduzirRetornoConsultarExtratoDirfEstabelecimento(de);

                result.Add(para);
            }

            return result;
        }

        private static ConsultarExtratoDirfEstabelecimentoRetorno TraduzirRetornoConsultarExtratoDirfEstabelecimento(ConsultarExtratoDirfEstabelecimentoRetornoDTO de)
        {
            ConsultarExtratoDirfEstabelecimentoRetorno para = new ConsultarExtratoDirfEstabelecimentoRetorno();
            para.ValorCobrado = de.ValorCobrado;
            para.ValorIrRecebido = de.ValorIrRecebido;
            para.ValorRecebido = de.ValorRecebido;
            para.ValorRepassadoEmissor = de.ValorRepassadoEmissor;

            return para;
        }
    }
}