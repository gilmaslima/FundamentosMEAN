/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe XXX, que expõe métodos para manipular as execeções do sistema FMS.
    /// </summary>
    public class FMSComCampoException : FMSException
    {

        public int CodigoCampoOrigem { private set; get; }
        
        /// <summary>
        /// Este método é utilizado para manipular as execeções do sistema FMS. 
        /// </summary>
        /// <param name="mensagem"></param>
        public FMSComCampoException(TipoExcecaoServico tipoExcecao, string mensagem, int codigoCampoOrigem)
            :base(tipoExcecao, mensagem)
        {
            
            CodigoCampoOrigem = codigoCampoOrigem; 
        }

        public override int ObterCodigoOcorrencia()
        {
            int codigoOcorrencia;
            
            codigoOcorrencia = base.ObterCodigoOcorrencia() + CodigoCampoOrigem ;

            return codigoOcorrencia;

        }

    }

}
