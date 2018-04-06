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
using System.Web;

namespace Redecard.PN.FMS.Servico.Tradutor
{
    /// <summary>
    /// Este componente publica a classe TradutorCadastroIP, que expõe métodos para manipular o cadastro  de ip.
    /// </summary>
    public static class TradutorCadastroIP
    {
        /// <summary>
        /// Este método é utilizado para traduzir a lista de ips autorizados.
        /// </summary>
        /// <param name="listaIpAutorizado"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados> TraduzirListaIpAutorizado(List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados> listaIpAutorizado)
        {
            List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados> result = new List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados>();

            foreach (Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados de in listaIpAutorizado)
            {
                Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados para = TraduzirIpAutorizado(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para traduzir os ips autorizados.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private static Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados TraduzirIpAutorizado(Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados de)
        {
            Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados para = new Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados();
            para.CdIPAssociado = de.CdIPAssociado;
            para.NumeroIP = de.NumeroIP;
            para.EhPassivelValidacaoIP = de.EhPassivelValidacao;

            return para;
        }
        /// <summary>
        /// Este método é utilizado para traduzir a lista de ips autorizadodo  modelo para serviço.
        /// </summary>
        /// <param name="listaIpAutorizado"></param>
        /// <returns></returns>
        public static List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados> TraduzirListaIpAutorizadoModeloParaServico(List<Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados> listaIpAutorizado)
        {
            List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados> result = new List<Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados>();

            foreach (Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados de in listaIpAutorizado)
            {
                Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados para = TraduzirIpAutorizadoModeloParaServico(de);

                result.Add(para);
            }

            return result;
        }
        /// <summary>
        /// Este método é utilizado para  traduzir o ip autorizado do modelo para o serviço.
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private static Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados TraduzirIpAutorizadoModeloParaServico(Redecard.PN.FMS.Modelo.CadastroIPs.IPsAutorizados de)
        {
            Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados para = new Redecard.PN.FMS.Servico.Modelo.CadastroIPs.IPsAutorizados();
            para.CdIPAssociado = de.CdIPAssociado;
            para.NumeroIP = de.NumeroIP;
            para.EhPassivelValidacao = de.EhPassivelValidacaoIP;

            return para;
        }
    }
}