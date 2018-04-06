/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – Renao Cara – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;


namespace Redecard.PN.FMS.Agente.Tradutores
{
    /// <summary>
    /// Este componente publica a classe TranslatorRespostaListaTransacoesEmissor, e expõe métodos para traduzir os dados de resposta da lista de transações por emissor, oriundos do webservice
    /// </summary>
    public class TranslatorRespostaListaTransacoesEmissor
    {
        /// <summary>
        /// Este método é utilizado para traduzir os dados de resposta da lista de transações por emissor oriundos do webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static RespostaListaTransacoesEmissor TranslateTipoRespostaListaEmissorWSTipoRespostaListaEmissorBusiness(issuerTransactionComposite from)
        {
            RespostaListaTransacoesEmissor to = new RespostaListaTransacoesEmissor();

            to.ListaTransacoesEmissor = new List<TransacaoEmissor>();
            if (from.issuerTxList != null)
            {
                foreach (issuerTransaction issuerTransaction in from.issuerTxList)
                {
                    to.ListaTransacoesEmissor.Add(TranslatorTransacaoEmissor.TranslateIssuerTransactionToTransacaoEmissorBusiness(issuerTransaction));
                }
            }
            
            to.SegundosRestanteBloqueio = from.remainderTimeoutLock;
            to.TipoRespostaListaEmissor = (TipoRespostaListaEmissor)from.queryStatus;

            return to;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os dados de resposta da lista de transações por emissor oriundos do webservice. 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static issuerTransactionComposite TranslateipoRespostaListaEmissorBusinessTipoRespostaListaEmissorWST(RespostaListaTransacoesEmissor from)
        {
            issuerTransactionComposite to = new issuerTransactionComposite();

            to.issuerTxList = new issuerTransaction[from.ListaTransacoesEmissor.Count];

            int contador = 0;
            foreach (TransacaoEmissor registro in from.ListaTransacoesEmissor)
            {
                to.issuerTxList[contador] = TranslatorTransacaoEmissor.TranslateTransacaoEmissorBusinessToIssuerTransaction(registro);
                contador++;
            }
            to.remainderTimeoutLock = from.SegundosRestanteBloqueio;
            to.queryStatus = (int)from.TipoRespostaListaEmissor;

            return to;
        }

    }
}
