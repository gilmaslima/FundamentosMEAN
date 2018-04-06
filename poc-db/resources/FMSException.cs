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
    public class FMSException : System.Exception
    {

        private const int ValorInicialFaixaExceptions = 0;
        
        protected TipoExcecaoServico TipoExcecao { private set; get; }
        public string Mensagem { private set; get; }
        
        /// <summary>
        /// Este método é utilizado para manipular as execeções do sistema FMS. 
        /// </summary>
        /// <param name="mensagem"></param>
        public FMSException(TipoExcecaoServico tipoExcecao, string mensagem)
        {
            TipoExcecao = tipoExcecao;
            Mensagem = mensagem;
        }

        public virtual int ObterCodigoOcorrencia()
        {
            int codigoOcorrencia;
            
            codigoOcorrencia = ValorInicialFaixaExceptions + Convert.ToInt32(TipoExcecao);
            
            return codigoOcorrencia;

        }
    }
}
