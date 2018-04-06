#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [30/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Representa uma propriedade da ocorrência
    /// </summary>
    public class Propriedade
    {
        /// <summary>
        /// Ordem em que deve ser apresentada a propriedade
        /// </summary>
        public Int32 OrdemApresentacao { get; set; }

        /// <summary>
        /// Código/Nome interno do campo
        /// </summary>
        public String CodigoCampo { get; set; }

        /// <summary>
        /// Nome/Label do campo
        /// </summary>
        public String NomeCampo { get; set; }

        /// <summary>
        /// Informa se o campo é obrigatório ou não
        /// </summary>
        public Boolean Obrigatorio { get; set; }

        /// <summary>
        /// Informa em qual formato o campo deve ser enviado
        /// </summary>
        public String MascaraCampo { get; set; }

        /// <summary>
        /// Informa se o campo possui casas decimais, somente para campo númericos
        /// </summary>
        public Int32 CasasDecimais { get; set; }

        /// <summary>
        /// Informa o tipo de campo
        /// </summary>
        public String TipoCampo { get; set; }
    }
}