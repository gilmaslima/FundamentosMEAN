using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoInformeDIRF;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public static class TradutorResultadoInformeDIRF
    {
        #region WACA075 - Dirf.
        public static ConsultarExtratoDirfRetornoDTO TraduzirRetornoConsultarExtratoDirf(string razaoSocialEstabelecimento, string comarcaEstabelecimento, string enderecoEstabelecimento,  string bairroEstabelecimento, string cidadeEstabelecimento, string estadoEstabelecimento, int cepEstabelecimento, string codigoMalaDiretaEstabelecimento, EstabelecimentoDirf[] estabelecimentos, Emissor[] emissores)
        {
            ConsultarExtratoDirfRetornoDTO para = new ConsultarExtratoDirfRetornoDTO();
            para.RazaoSocialEstabelecimento = razaoSocialEstabelecimento;
            para.ComarcaEstabelecimento = comarcaEstabelecimento;
            para.EnderecoEstabelecimento = enderecoEstabelecimento;
            para.BairroEstabelecimento = bairroEstabelecimento;
            para.CidadeEstabelecimento = cidadeEstabelecimento;
            para.EstadoEstabelecimento = estadoEstabelecimento;
            para.CepEstabelecimento = cepEstabelecimento;
            para.CodigoMalaDiretaEstabelecimento = codigoMalaDiretaEstabelecimento;
            para.Estabelecimentos = TraduzirRetornoListaConsultarExtratoDirfEstabelecimento(estabelecimentos);
            para.Emissores = TraduzirRetornoListaConsultarExtratoDirfEmissor(emissores);

            return para;
        }

        private static List<ConsultarExtratoDirfEmissorRetornoDTO> TraduzirRetornoListaConsultarExtratoDirfEmissor(Emissor[] list)
        {
            List<ConsultarExtratoDirfEmissorRetornoDTO> result = new List<ConsultarExtratoDirfEmissorRetornoDTO>();

            foreach (Emissor de in list)
            {
                if (de.Cnpj != 0)
                {

                    ConsultarExtratoDirfEmissorRetornoDTO para = TraduzirRetornoConsultarExtratoDirfEmissor(de);

                    result.Add(para);
                }
            }

            return result;
        }

        private static ConsultarExtratoDirfEmissorRetornoDTO TraduzirRetornoConsultarExtratoDirfEmissor(Emissor de)
        {
            ConsultarExtratoDirfEmissorRetornoDTO para = new ConsultarExtratoDirfEmissorRetornoDTO();
            para.Cnpj = de.Cnpj;
            para.Nome = de.Nome;
            para.ValorIrEmissor1 = (de.ValorIrEmissor1 * decimal.Parse(de.SinalValorIrEmissor1.Equals("+") ? "1" : de.SinalValorIrEmissor1 + "1"));
            para.ValorIrEmissor2 = (de.ValorIrEmissor2 * decimal.Parse(de.SinalValorIrEmissor2.Equals("+") ? "1" : de.SinalValorIrEmissor2 + "1"));
            para.ValorIrEmissor3 = (de.ValorIrEmissor3 * decimal.Parse(de.SinalValorIrEmissor3.Equals("+") ? "1" : de.SinalValorIrEmissor3 + "1"));
            para.ValorIrEmissor4 = (de.ValorIrEmissor4 * decimal.Parse(de.SinalValorIrEmissor4.Equals("+") ? "1" : de.SinalValorIrEmissor4 + "1"));
            para.ValorIrEmissor5 = (de.ValorIrEmissor5 * decimal.Parse(de.SinalValorIrEmissor5.Equals("+") ? "1" : de.SinalValorIrEmissor5 + "1"));
            para.ValorIrEmissor6 = (de.ValorIrEmissor6 * decimal.Parse(de.SinalValorIrEmissor6.Equals("+") ? "1" : de.SinalValorIrEmissor6 + "1"));
            para.ValorIrEmissor7 = (de.ValorIrEmissor7 * decimal.Parse(de.SinalValorIrEmissor7.Equals("+") ? "1" : de.SinalValorIrEmissor7 + "1"));
            para.ValorIrEmissor8 = (de.ValorIrEmissor8 * decimal.Parse(de.SinalValorIrEmissor8.Equals("+") ? "1" : de.SinalValorIrEmissor8 + "1"));
            para.ValorIrEmissor9 = (de.ValorIrEmissor9 * decimal.Parse(de.SinalValorIrEmissor9.Equals("+") ? "1" : de.SinalValorIrEmissor9 + "1"));
            para.ValorIrEmissor10 = (de.ValorIrEmissor10 * decimal.Parse(de.SinalValorIrEmissor10.Equals("+") ? "1" : de.SinalValorIrEmissor10 + "1"));
            para.ValorIrEmissor11 = (de.ValorIrEmissor11 * decimal.Parse(de.SinalValorIrEmissor11.Equals("+") ? "1" : de.SinalValorIrEmissor11 + "1"));
            para.ValorIrEmissor12 = (de.ValorIrEmissor12 * decimal.Parse(de.SinalValorIrEmissor12.Equals("+") ? "1" : de.SinalValorIrEmissor12 + "1"));
            para.ValorRepassadoEmissor1 = (de.ValorRepassadoEmissor1 * decimal.Parse(de.SinalValorRepassadoEmissor1.Equals("+") ? "1" : de.SinalValorRepassadoEmissor1 + "1"));
            para.ValorRepassadoEmissor2 = (de.ValorRepassadoEmissor2 * decimal.Parse(de.SinalValorRepassadoEmissor2.Equals("+") ? "1" : de.SinalValorRepassadoEmissor2 + "1"));
            para.ValorRepassadoEmissor3 = (de.ValorRepassadoEmissor3 * decimal.Parse(de.SinalValorRepassadoEmissor3.Equals("+") ? "1" : de.SinalValorRepassadoEmissor3 + "1"));
            para.ValorRepassadoEmissor4 = (de.ValorRepassadoEmissor4 * decimal.Parse(de.SinalValorRepassadoEmissor4.Equals("+") ? "1" : de.SinalValorRepassadoEmissor4 + "1"));
            para.ValorRepassadoEmissor5 = (de.ValorRepassadoEmissor5 * decimal.Parse(de.SinalValorRepassadoEmissor5.Equals("+") ? "1" : de.SinalValorRepassadoEmissor5 + "1"));
            para.ValorRepassadoEmissor6 = (de.ValorRepassadoEmissor6 * decimal.Parse(de.SinalValorRepassadoEmissor6.Equals("+") ? "1" : de.SinalValorRepassadoEmissor6 + "1"));
            para.ValorRepassadoEmissor7 = (de.ValorRepassadoEmissor7 * decimal.Parse(de.SinalValorRepassadoEmissor7.Equals("+") ? "1" : de.SinalValorRepassadoEmissor7 + "1"));
            para.ValorRepassadoEmissor8 = (de.ValorRepassadoEmissor8 * decimal.Parse(de.SinalValorRepassadoEmissor8.Equals("+") ? "1" : de.SinalValorRepassadoEmissor8 + "1"));
            para.ValorRepassadoEmissor9 = (de.ValorRepassadoEmissor9 * decimal.Parse(de.SinalValorRepassadoEmissor9.Equals("+") ? "1" : de.SinalValorRepassadoEmissor9 + "1"));
            para.ValorRepassadoEmissor10 = (de.ValorRepassadoEmissor10 * decimal.Parse(de.SinalValorRepassadoEmissor10.Equals("+") ? "1" : de.SinalValorRepassadoEmissor10 + "1"));
            para.ValorRepassadoEmissor11 = (de.ValorRepassadoEmissor11 * decimal.Parse(de.SinalValorRepassadoEmissor11.Equals("+") ? "1" : de.SinalValorRepassadoEmissor11 + "1"));
            para.ValorRepassadoEmissor12 = (de.ValorRepassadoEmissor12 * decimal.Parse(de.SinalValorRepassadoEmissor12.Equals("+") ? "1" : de.SinalValorRepassadoEmissor12 + "1"));

            return para;
        }

        private static List<ConsultarExtratoDirfEstabelecimentoRetornoDTO> TraduzirRetornoListaConsultarExtratoDirfEstabelecimento(EstabelecimentoDirf[] list)
        {
            List<ConsultarExtratoDirfEstabelecimentoRetornoDTO> result = new List<ConsultarExtratoDirfEstabelecimentoRetornoDTO>();

            foreach (EstabelecimentoDirf de in list)
            {
                ConsultarExtratoDirfEstabelecimentoRetornoDTO para = TraduzirRetornoConsultarExtratoDirfEstabelecimento(de);

                result.Add(para);
            }
            
            return result;
        }

        private static ConsultarExtratoDirfEstabelecimentoRetornoDTO TraduzirRetornoConsultarExtratoDirfEstabelecimento(EstabelecimentoDirf de)
        {
            ConsultarExtratoDirfEstabelecimentoRetornoDTO para = new ConsultarExtratoDirfEstabelecimentoRetornoDTO();
            
            para.ValorCobrado = (de.ValorCobrado * decimal.Parse(de.SinalValorCobrado.Equals("+") ? "1" : de.SinalValorCobrado + "1"));
            para.ValorIrRecebido = (de.ValorIrRecebido * decimal.Parse(de.SinalValorIrRecebido.Equals("+") ? "1" : de.SinalValorIrRecebido + "1"));
            para.ValorRecebido = (de.ValorRecebido * decimal.Parse(de.SinalValorRecebido.Equals("+") ? "1" : de.SinalValorRecebido + "1"));
            para.ValorRepassadoEmissor = (de.ValorRepassadoEmissor * decimal.Parse(de.SinalValorRepassadoEmissor.Equals("+") ? "1" : de.SinalValorRepassadoEmissor + "1"));
            
            return para;
        }
        #endregion

        #region WACA079 - Dirf.
        public static ConsultarAnosBaseDirfRetornoDTO TraduzirRetornoConsultarAnosBaseDirf(short[] list)
        {
            ConsultarAnosBaseDirfRetornoDTO para = new ConsultarAnosBaseDirfRetornoDTO();
            para.AnosBase = new List<short>();

            foreach (short item in list)
            {
                if (item != 0)
                {
                    para.AnosBase.Add(item);
                }
            }

            return para;
        }
        #endregion
    }
}
