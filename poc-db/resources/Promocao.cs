using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.Portal.Helper.DTO
{
    /// <summary>
    /// Autor: Vagner L. Borges
    /// Data criação: meados de 13/09/2010
    /// Descrição: Classe que representa uma Promoção.    
    /// </summary>
    public class Promocao
    {
        /// <summary>
        /// Item que descreve o Perfil da promoção
        /// </summary>
        public string Perfil { get; set; }

        /// <summary>
        /// Item que descreve o Link da promoção
        /// </summary>
        public string Link { get; set; }

        public Promocao() { }
    }
}
