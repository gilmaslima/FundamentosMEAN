/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoRecargaCelular;
using Redecard.PN.Extrato.Modelo.RecargaCelular;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    /// <summary>
    /// Classe tradutora dos parâmetros de entrada e saída 
    /// das consultas HIS
    /// </summary>
    public class RecargaCelularTR : ITradutor
    {
        #region [ Recarga de Celular - Detalhes - BKWA2420 / WA242 / ISIB ]

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        internal static List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalheSaida(
            Int32 qtdRegistros, WA242S_DETALHE[] dados)
        {
            var retorno = new List<RecargaCelularDetalhe>();

            foreach (WA242S_DETALHE dado in dados)
            {
                if (retorno.Count < qtdRegistros)
                {
                    retorno.Add(new RecargaCelularDetalhe
                    {
                        DataHoraTransacao = String.Join(String.Empty, 
                            String.Concat(dado.WA242S_DAT_TRAN, dado.WA242S_HOR_TRAN)
                            .Where(c => Char.IsNumber(c))).ToDateTimeNull("ddMMyyyyHHmmss"),
                        NumeroCelular = dado.WA242S_NUM_CEL,
                        NumeroNsu = dado.WA242S_NUM_NSU,
                        NomeOperadora = dado.WA242S_DSC_OPER,
                        StatusComissao = dado.WA242S_STA_CMSS,
                        StatusTransacao = dado.WA242S_STA_TRAN,
                        ValorComissao = dado.WA242S_VAL_CMSS,
                        ValorTransacao = dado.WA242S_VAL_TRAN
                    });
                }
            }

            return retorno;
        }

        #endregion
    }
}