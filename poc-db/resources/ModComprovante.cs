using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento
{
    /// <summary>
    /// Classe para extender as propriedade da lista de consulta dia
    /// </summary>
    public partial class ModComprovante : ICloneable
    {
        #region Propriedades
        public string DataTransacaoFormatada
        {
            get {
                return this.DataTransacao != null && this.DataTransacao.CompareTo(DateTime.MinValue) != 0 ? this.DataTransacao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DataInclusaoFormatada
        {
            get
            {
                return this.DataInclusao != null && this.DataInclusao.CompareTo(DateTime.MinValue) != 0 ? this.DataInclusao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DataCartaFormatada
        {
            get
            {
                return this.DataCarta != null && this.DataCarta.CompareTo(DateTime.MinValue) != 0 ? this.DataCarta.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string ValorCancelamentoFormatada
        {
            get
            {
                return this.ValorCancelamento != null && this.ValorCancelamento > 0 ? this.ValorCancelamento.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }

        public string ValorTransacaoFormatada
        {
            get
            {
                return this.ValorTransacao != null && this.ValorTransacao > 0 ? this.ValorTransacao.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }

        public string ValorLiquidoFormatada
        {
            get
            {
                return this.ValorLiquido != null && this.ValorLiquido > 0 ? this.ValorLiquido.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;        
            }
        }

        public string NumAvisoCancelamentoFormatada
        {
            get
            {
                return !string.IsNullOrEmpty(this.NumAvisoCancel) ? this.NumAvisoCancel : string.Empty;
            }
        }

        public string NumEstabelecimentoFormatada
        {
            get
            {
                return this.NumEstabelecimento != null && this.NumEstabelecimento > 0 ? this.NumEstabelecimento.ToString() : string.Empty;     
            }
        }

        public string NumNSUFormatada
        {
            get
            {
                return this.NumNSU.ToString();
            }
        }

        public string TipoCancelamentoFormatado {

            get
            {
                return this.TipoCancelamento == "T" ? "Total" : this.TipoCancelamento == "P" ? "Parcial" : string.Empty;
            }
        }

        public string TipoVendaFormatado
        {

            get
            {
                if(TipoTransacao == Convert.ToString(1))
                {
                return "À vista";
                }

                if (TipoTransacao == Convert.ToString(2))
                {
                    return "Parc. Estabelec.";
                }
                if (TipoTransacao == Convert.ToString(3))
                {
                    return "Parc. Emissor.";
                }
                return "";
            }
        }


        #endregion

        public object Clone()
        {
            return new ModComprovante()
            {
                CodigoCancelamento = this.CodigoCancelamento,
                DataCarta = this.DataCarta,
                DataInclusao = this.DataInclusao,
                DataTransacao = this.DataTransacao,
                DescCodigoCancelamento = this.DescCodigoCancelamento,
                DescEstabelecimento = this.DescEstabelecimento,
                DescTipoCancelamento = this.DescTipoCancelamento,
                DescTipoTransacao = this.DescTipoTransacao,
                ExtensionData = this.ExtensionData,
                NumAvisoCancel = this.NumAvisoCancel,
                NumCartao = this.NumCartao,
                NumCentralizadora = this.NumCentralizadora,
                NumEstabelecimento = this.NumEstabelecimento,
                NumIdentifCarta = this.NumIdentifCarta,
                NumMicrofilme = this.NumMicrofilme,
                NumNSU = this.NumNSU,
                NumPV = this.NumPV,
                NumResumoVenda = this.NumResumoVenda,
                QtdParcelas = this.QtdParcelas,
                TipoCancelamento = this.TipoCancelamento,
                TipoTransacao = this.TipoTransacao,
                Transacao = this.Transacao,
                ValorCancelamento = this.ValorCancelamento,
                ValorLiquido = this.ValorLiquido,
                ValorTransacao = this.ValorTransacao
            };
        }
    }
}