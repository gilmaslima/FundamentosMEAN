/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/11/05 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Modelos
{
    
    public class DadosVendedor
    {
        
        public DateTime DataFundacao { get; set; }

        
        public string Cpf { get; set; }
    }
}
