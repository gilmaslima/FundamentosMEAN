/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Text;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Agentes.Tradutores
{
    /// <summary>
    /// Classe auxiliar tradutora para o tratamento da chamada e retorno
    /// dos programas mainframe para o projeto Corban
    /// </summary>
    public class TradutorCorban
    {
        #region [Transações Corban]
        
        /// <summary>
        /// Preparação da área de entrada do programa mainframe
        /// </summary>
        /// <param name="pvs">Listagem de estabelecimentos</param>
        /// <returns>Área de entrada</returns>
        public static String ConsultarTransacoesEntrada(Int32[] pvs)
        {
            var dados = new StringBuilder();

            //B266E-QTD-PV
            dados.Append(pvs.Length.ToString("D5"));

            //B266E-TAB-PDV
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Length > iPV ? pvs[iPV] : 0).ToString("D9"));

            //B266E-REDADO3
            dados.Append(String.Empty.PadRight(4995, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Tratamento da área de retorno da chamada do programa mainframe
        /// </summary>
        /// <param name="dados">Área de retorno que deve ser tratada</param>
        /// <param name="tipoRegistro">Tipo do registro</param>
        /// <param name="quantidadeTransacoes">Quantidade de transações retornadas</param>
        /// <returns></returns>
        public static List<Modelo.TransacaoCorban> ConsultarTransacoesSaida(
            String dados, out String tipoRegistro, out Int32 quantidadeTransacoes)
        {
            var retorno = new List<Modelo.TransacaoCorban>();

            var cortador = new CortadorMensagem(dados);

            //B266S-TIPREG
            tipoRegistro = cortador.LerString(1);

            //B266S-QTD-REG
            quantidadeTransacoes = cortador.LerInt32(3);

            //B266S-OCORR
            String[] valores = cortador.LerOccurs(198, 150);

            foreach (String valor in valores)
            {
                if (retorno.Count < quantidadeTransacoes)
                {
                    var cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new Modelo.TransacaoCorban
                    {
                        NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                        DataPagamento = cortadorRegistro.LerString(8).ToDateTimeNull("ddMMyyyy"),
                        HoraPagamento = cortadorRegistro.LerString(8).ToInt32(0).ToString("D6").ToDateTimeNull("HHmmss"),
                        CodigoServico = cortadorRegistro.LerDecimal(15, 0),
                        DescricaoTipoConta = cortadorRegistro.LerString(20),
                        DescricaoFormaPagamento = cortadorRegistro.LerString(10),
                        DescricaoBandeira = cortadorRegistro.LerString(15),
                        CodigoBarras = cortadorRegistro.LerString(56),
                        NomeOperadora = cortadorRegistro.LerString(20),
                        ValorBrutoPagamento = cortadorRegistro.LerDecimal(15,2),
                        StatusConta = cortadorRegistro.LerString(20)
                    });
                }
            }

            return retorno;
        }

        #endregion
    }
}
