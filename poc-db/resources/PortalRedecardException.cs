using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe de tratamento de erro do Portal Redecard.
    /// </summary>
    public class PortalRedecardException : Exception
    {
        /// <summary>
        /// Código do erro
        /// </summary>
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Nome do serviço e método onde o erro ocorreu
        /// </summary>
        public String Fonte { get; set; }

        /// <summary>
        /// Contrutor
        /// </summary>
        /// <param name="codigo">Código de erro</param>
        /// <param name="fonte">Nome de onde o erro ocorreu (Servico.Metodo) ou (Classe.Metodo)</param>
        public PortalRedecardException(Int32 codigo, String fonte)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

        /// <summary>
        /// Contrutor
        /// </summary>
        /// <param name="codigo">Código de erro</param>
        /// <param name="fonte">Nome de onde o erro ocorreu (Servico.Metodo) ou (Classe.Metodo)</param>
        /// <param name="ex">Exceção original</param>
        public PortalRedecardException(Int32 codigo, String fonte, Exception ex)
            : base(ex.Message, ex)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

        /// <summary>
        /// Contrutor
        /// </summary>
        /// <param name="codigo">Código de erro</param>
        /// <param name="fonte">Nome de onde o erro ocorreu (Servico.Metodo) ou (Classe.Metodo)</param>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="excecao">Exceção original</param>
        public PortalRedecardException(Int32 codigo, String fonte, String mensagem, Exception ex)
            : base(mensagem, ex)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

    }
}
