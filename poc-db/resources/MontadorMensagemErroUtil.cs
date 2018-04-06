using System;
using System.Collections.Generic;
using System.Text;

namespace Redecard.Portal.Helper.Validacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe que implementa uma mensagem de erro com base em coleções de erros (vide classe SumarioValidacao)
    /// </summary>
    public class MontadorMensagemErroUtil
    {
        private MontadorMensagemErroUtil(){}

        /// <summary>
        /// Monta uma mensagem com base no objeto de sumário de validação sem cabeçalho
        /// </summary>
        /// <param name="sumario"></param>
        /// <returns></returns>
        public static string Montar(SumarioValidacao sumario)
        {
            return MontadorMensagemErroUtil.Montar(sumario,null);
        }

        /// <summary>
        /// Monta uma mensagem com base no objeto de sumário de validação com cabeçalho informado
        /// </summary>
        /// <param name="sumario"></param>
        /// <returns></returns>
        public static string Montar(SumarioValidacao sumario, string cabecalho)
        {
            StringBuilder sb;
            if (!string.IsNullOrEmpty(cabecalho))
                sb = new StringBuilder(cabecalho + @"\n");
            else
                sb = new StringBuilder();

            IList<Inconsistencia> inconsistencias = sumario.Inconsistencias;
            foreach (Inconsistencia i in inconsistencias)
                sb.AppendFormat("- {0}{1}",i.Mensagem, @"\n");

            return sb.ToString();
        }
    }
}