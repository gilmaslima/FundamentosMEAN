/*
(c) Copyright [2012] Redecard S.A.
Autor : [Lucas Nicoletto da Cunha]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/11/05 - Lucas Nicoletto da Cunha - Versão Inicial
*/
using System.Runtime.Serialization;
namespace Redecard.PN.Emissores.Modelos
{
 
    public class DadosProdutosNegociados
    {
        
        public int DiasPrazo { get; set; }

        
        public decimal TaxaRegime { get; set; }
    }
}
