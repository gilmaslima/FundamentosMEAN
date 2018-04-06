/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.FMS.Sharepoint.Modelo
{
    public class SPListaPadrao
    {
        /// <summary>
        /// Valor da lista criada manualmente do SP
        /// </summary>
        public string Valor { get; set; }

        /// <summary>
        /// Titulo da lista criado automaticamente pelo SP "Título"
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição da lista criada manualmente do SP (não obrigatoria)
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// CorDeFundo
        /// </summary>
        public string CorDeFundo { get; set; }

        /// <summary>
        /// CorDeLetra
        /// </summary>
        public string CorDeLetra { get; set; }
    }
}
