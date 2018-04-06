/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.RAV.Servicos
{
    /// <summary>
    /// Classe de entidade do RAV Antecipa.
    /// </summary>
    [DataContract]
    public class ModRAVAntecipa
    {
        [DataMember]
        public ElndAntecipa Indicador { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public ElndDataAntecipa IndicadorData { get; set; }

        [DataMember]
        public DateTime DataDe { get; set; }

        [DataMember]
        public DateTime DataAte { get; set; }

        [DataMember]
        public String NomeProdutoAntecipacao { get; set; }

        [DataMember]
        public String DescricaoProdutoAntecipacao { get; set; }

        [DataMember]
        public Int32 CodigoProdutoAntecipacao { get; set; }

        [DataMember]
        public Decimal ValorMaxAntecUraSenha { get; set; }

        [DataMember]
        public Decimal ValorTarifa { get; set; }

        [DataMember]
        public Decimal PercentualTarifa { get; set; }

        [DataMember]
        public ElndProdutoAntecipa IndicadorProduto { get; set; }
    }

    public enum ElndAntecipa
    { Total, Parcial }

    public enum ElndDataAntecipa
    { Apresentacao, Vencimento }

    public enum ElndProdutoAntecipa
    { Rotativo, Parcelado, Ambos }
}