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
using Redecard.PN.FMS.Agente;
using Redecard.PN.FMS.Modelo;

namespace Redecard.PN.FMS.Negocio
{
    /// <summary>
    /// Este componente publica a classe MccBLL, que expõe métodos para manipular os dados de merchant category code.
    /// </summary>
    public class MccBLL
    {
        /// <summary>
        /// Este componente publica a classe MccBLL, consumida a partir da camada de serviços, e expõe métodos para consultar o webservice, via as classes de agentes, para expor os serviços de Merchant Categoy Code para o FMS.
        /// </summary>
        /// <param name="numeroEmisso"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="usuarioLogin"></param>
        /// <param name="codigoMCC"></param>
        /// <param name="descricaoMCC"></param>
        /// <param name="posicaoPrimeiroRegistro"></param>
        /// <param name="quantidadeMaximaRegistros"></param>
        /// <param name="renovarContador"></param>
        /// <returns></returns>
        public RespostaListaMCC PesquisarListaMCC(string numeroEmissor, int grupoEntidade, string usuarioLogin,
            long? codigoMCC, string descricaoMCC, int posicaoPrimeiroRegistro, int quantidadeMaximaRegistros, bool renovarContador)
        {

            IServicosFMS fmsClient = ServicoFMSFactory.RetornaClient();

            RespostaListaMCC respostaMCC = new RespostaListaMCC(); ;
            
            respostaMCC.ListaMCC = fmsClient.PesquisarListaMCC(codigoMCC, descricaoMCC, posicaoPrimeiroRegistro, quantidadeMaximaRegistros);

            if (renovarContador)
                respostaMCC.QuantidadeRegistros = fmsClient.ContarRegistrosMCC(codigoMCC, descricaoMCC);

            return respostaMCC;
        }
    }
}
