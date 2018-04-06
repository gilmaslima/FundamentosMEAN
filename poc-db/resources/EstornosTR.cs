using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.WAExtratoVendas;
using Redecard.PN.Extrato.Modelo.Estornos;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    public abstract class EstornosTR : ITradutor
    {
        #region [ Estorno - BKWA2940 / WAC294 / WAAQ ]

        public static String ConsultarEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(pvs.Count.ToString("D5"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(4995, ' '));

            return dados.ToString();
        }

        public static List<Estorno> ConsultarSaida(String dados)
        {
            List<Estorno> retorno = new List<Estorno>();
            CortadorMensagem cortador = new CortadorMensagem(dados);
            Int32 qtdRegistros = cortador.LerInt32(3);
            String[] valores = cortador.LerOccurs(132, 230);
            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new EstornoD()
                    {
                        NumeroPV = cortadorRegistro.LerInt32(9),
                        DataHoraVenda = cortadorRegistro.LerData(8, "yyyyMMdd", true),
                        DataHoraEstorno = cortadorRegistro.LerData(6, "HHmmss", true),
                        DescricaoTipoConta = cortadorRegistro.LerString(15),
                        DescricaoModalidadeVenda = cortadorRegistro.LerString(20),
                        Plano = cortadorRegistro.LerInt16(2),
                        DescricaoBandeira = cortadorRegistro.LerString(15),
                        NSU = cortadorRegistro.LerInt32(12),
                        CodigoTerminal = cortadorRegistro.LerString(8),
                        NumeroCartao = cortadorRegistro.LerString(19),
                        ValorVenda = cortadorRegistro.LerDecimal(15, 2),
                        IndicadorTokenizacao = cortadorRegistro.LerString(1)
                    });
                }
            }

            return retorno.ToList();
        }

        #endregion
    }
}
