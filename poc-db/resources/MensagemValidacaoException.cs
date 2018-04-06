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

namespace Redecard.PN.FMS.Sharepoint.Exceptions
{
    /// <summary>
    /// Este componente publica a classe MensagemValidacaoException, estendida de Exception, que expõe métodos para manipular as mensagens de validação das exceções.
    /// </summary>
    public class MensagemValidacaoException : Exception
    {
        public int Codigo { get; private set; }

        public string Mensagem { get; private set; }

        /// <summary>
        /// Construror de MensagemValidacaoException, com parâmetros de inicialização do objeto.
        /// </summary>
        /// <param name="_codigo"></param>
        /// <param name="_mensagem"></param>
        public MensagemValidacaoException(int _codigo, String _mensagem)
        {
            Codigo = _codigo;
            Mensagem = _mensagem;
        }
        /// <summary>
        /// Construror de MensagemValidacaoException, com parâmetros de inicialização do objeto.
        /// </summary>
        /// <param name="_mensagem"></param>
        public MensagemValidacaoException(String _mensagem)
        {
            Codigo = -1;
            Mensagem = _mensagem;
        }
    }
}
