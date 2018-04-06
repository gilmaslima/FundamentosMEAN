using System;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoLog { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoGrupoEntidade { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoEntidade { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public String Usuario { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public String Operacao { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime DataLog { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public String Responsavel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataAprovacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String ResponsavelAprovacao { get; set; }
    }
}