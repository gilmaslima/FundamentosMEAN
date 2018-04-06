/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;
using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;

namespace Redecard.PN.FMS.Sharepoint.Helpers
{
    /// <summary>
    /// Este componente publica a classe UtilitariosHelper, que expõe métodos utilitários do sistema de ajuda.
    /// </summary>
    public class UtilitariosHelper
    {
        /// <summary>
        /// Obtem Ip de Requisicao
        /// </summary>
        /// <param name="requisicao"></param>
        /// <returns></returns>
        public static string ObterIpRequisicao(HttpRequest requisicao)
        {
            if (requisicao.Headers["TRUE-CLIENT-IP"] != null)
            {
                return requisicao.Headers["TRUE-CLIENT-IP"];
            }
            else if (requisicao.Headers["originalip"] != null)
            {
                return requisicao.Headers["originalip"];
            }
            else
            {
                return requisicao.UserHostAddress;
            }
        }
        
        public static string ObterCartaoMascarado(string cartao)
        {
            string retorno = cartao;

            if (cartao.Length > 10)
                retorno = cartao.Substring(0, 6) + new string('X', cartao.Length - 10) + cartao.Substring(cartao.Length - 4);

            return retorno;
        }

        public static string ValidaDatasForm(string dataInicial, string dataFinal, int? quantidadeMaxIntervaloDiasPequisa, int? quantidadeMaxDiasRetroativosPesquisas)
        {
            string sReturn = string.Empty;
            DateTime valDataInicial = DateTime.MinValue;
            DateTime valDataFinal = DateTime.MinValue;
            TimeSpan difEntreDias;

            #region Valida Datas Nulas

            if (string.IsNullOrEmpty(dataInicial))
            {
                sReturn = "Data inicial obrigat&oacute;ria";
                return sReturn;
            }
            else
            {
                valDataInicial = DateTime.Parse(dataInicial);
            }

            if (string.IsNullOrEmpty(dataFinal))
            {
                sReturn = "Data final obrigat&oacute;ria";
                return sReturn;
            }
            else
            {
                valDataFinal = DateTime.Parse(dataFinal);
            }

            #endregion

            #region Comparação entre datas (Inicial, Final e Hoje)

            if (valDataFinal.CompareTo(DateTime.Now) > 0)
            {
                sReturn = "Data final n&atilde;o pode ser maior que data atual.";
                return sReturn;
            }

            if (valDataInicial.CompareTo(valDataFinal) > 0)
            {
                sReturn = "Data final n&atilde;o pode ser menor que data inicial.";
                return sReturn;
            }

            #endregion

            #region Valida períodos de busca

            if (quantidadeMaxIntervaloDiasPequisa != null)
            {
                difEntreDias = valDataFinal.Subtract(valDataInicial);

                if (difEntreDias.Days > quantidadeMaxIntervaloDiasPequisa)
                {
                    sReturn = "O per&iacute;odo entre as datas n&atilde;o pode ser maior que " + quantidadeMaxIntervaloDiasPequisa + " dia(s).";
                    return sReturn;
                }
            }

            if (quantidadeMaxDiasRetroativosPesquisas != null)
            {
                if (valDataInicial.Day < (DateTime.Now.Day - quantidadeMaxDiasRetroativosPesquisas))
                {
                    sReturn = "Per&iacute;odo dispon&iacute;vel para consulta: &Uacute;ltimos " + quantidadeMaxDiasRetroativosPesquisas + " dia(s).";
                    return sReturn;
                }
            }

            #endregion

            return sReturn;
        }

        /// <summary>
        /// Para rodar a aplicação em modo teste incluir variável de ambiente  'AMBIENTETESTE_RESOURCE' com valor '1' na sessão variáveis de amibente -> variáveis do sistema
        /// </summary>
        /// <returns></returns>
        public static bool EhAmbienteTesteLocal()
        {
            return (System.Environment.GetEnvironmentVariable("AMBIENTETESTE_RESOURCE", EnvironmentVariableTarget.Machine) != null);
        }

        #region Compactar ViewState
        public static byte[] CompactarViewState(byte[] bytes)
        {
            MemoryStream MSsaida = new MemoryStream();
            GZipStream gzip = new GZipStream(MSsaida,
                              CompressionMode.Compress, true);
            gzip.Write(bytes, 0, bytes.Length);
            gzip.Close();
            return MSsaida.ToArray();
        }

        public static byte[] DescompactarViewState(byte[] bytes)
        {
            MemoryStream MSentrada = new MemoryStream();
            MSentrada.Write(bytes, 0, bytes.Length);
            MSentrada.Position = 0;
            GZipStream gzip = new GZipStream(MSentrada,
                              CompressionMode.Decompress, true);
            MemoryStream MSsaida = new MemoryStream();
            byte[] buffer = new byte[64];
            int leitura = -1;
            leitura = gzip.Read(buffer, 0, buffer.Length);
            while (leitura > 0)
            {
                MSsaida.Write(buffer, 0, leitura);
                leitura = gzip.Read(buffer, 0, buffer.Length);
            }
            gzip.Close();
            return MSsaida.ToArray();
        }
        #endregion
    }
}
