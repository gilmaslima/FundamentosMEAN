/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe RangeBinBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Rande Bin para o FMS.
    /// </summary>
    public class RangeBinBLL
    {
        /// <summary>
        /// Este método é utilizado para pesquisar range bin por emissor.
        /// </summary>
        /// <param name="numeroEmissor"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="ica"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistro"></param>
        /// <param name="renovarContador"></param>
        /// <returns></returns>
        public RespostaListaFaixaBin PesquisarRangeBinPorEmissor(int numeroEmissor, int grupoEntidade,
            string usuarioLogin, long ica, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistro, bool renovarContador)
        {
            RespostaListaFaixaBin faixaBin = new RespostaListaFaixaBin();

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();
            faixaBin.ListaFaixaBin = fmsClient.PesquisarRangeBinPorEmissor(numeroEmissor, grupoEntidade, usuarioLogin, ica, posicaoPrimeiroRegistro, quantidadeMaximaRegistro);

            if (renovarContador)
                faixaBin.QuantidadeRegistros = fmsClient.ContarRangeBinPorEmissor(numeroEmissor, grupoEntidade, usuarioLogin, ica);

            return faixaBin;


        }
    }
}
