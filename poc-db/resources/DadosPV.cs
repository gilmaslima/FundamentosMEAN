/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/31 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Modelos
{
    
    public class DadosPV
    {
        
        public int NumPV { get; set; }

        
        public string NomeEstabelecimento { get; set; }

        
        public string CGC { get; set; }

        
        public string TipoPod { get; set; }

        
        public int NumPVRef { get; set; }

        
        public string Domicilio { get; set; }

        
        public string Situacao { get; set; }

        
        public string TipoPV { get; set; }

        
        public string Indicador { get; set; }
    }
}
