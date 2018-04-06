using System;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// 
    /// </summary>
    public class IntranetRedecardException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 Codigo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Fonte { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IntranetRedecardException(Int32 codigo, String fonte)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="fonte"></param>
        /// <param name="ex"></param>
        public IntranetRedecardException(Int32 codigo, String fonte, Exception ex)
            : base(ex.Message, ex)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="fonte"></param>
        /// <param name="mensagem"></param>
        /// <param name="ex"></param>
        public IntranetRedecardException(Int32 codigo, String fonte, String mensagem, Exception ex)
            : base(mensagem, ex)
        {
            this.Codigo = codigo;
            this.Fonte = fonte;
        }

    }
}
