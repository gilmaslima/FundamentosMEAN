#region Histórico do Arquivo
/*
(c) Copyright [2013] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   :
- [03/09/2013] – [Agnaldo Costa] – [Criação]
*/
#endregion
using System;
using System.Text;

namespace Redecard.PN.Emissores.Modelos
{
    /// <summary>
    /// Classe de modelo base onde existem propriedades que são reutilizados em outros modelos
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Código do Modelo
        /// </summary>
        public virtual Int32 Codigo { get; set; }

        /// <summary>
        /// Descrição do Modelo
        /// </summary>
        public virtual String Descricao { get; set; }

        /// <summary>
        /// Nome do responsável por alguma ação efetuada
        /// </summary>
        public String NomeResponsavel { get; set; }
    }
}
