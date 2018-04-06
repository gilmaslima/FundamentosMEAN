#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [13/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Representa um motivo do material de venda
    /// </summary>
    public class Kit
    {
        /// <summary>
        /// Código do Kit
        /// </summary>
        public Int32 CodigoKit { get; set; }

        /// <summary>
        /// Descrição do Kit
        /// </summary>
        public String DescricaoKit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Quantidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Motivo Motivo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DescricaoKit;
        }
    }
}