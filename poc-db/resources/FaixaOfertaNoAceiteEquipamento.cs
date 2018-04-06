/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Japão - Equipamento de uma Faixa da Oferta no aceite
    /// </summary>
    public class FaixaOfertaNoAceiteEquipamento
    {
        /// <summary>
        /// Código do equipamento - Tecnologia
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// Valor do aluguel
        /// </summary>
        public Decimal ValorAluguel { get; set; }
    }
}
