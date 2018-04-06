using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento
{
    public partial class ModCancelamentoConsulta
    {
        #region Properties
        public string TipoTransacaoDescricao
        {
            get
            {
                    switch (TipoTransacao)
                    {
                        case 1:
                            return "A vista";
                        case 2:
                            return "Parcelado Estabelecimento";
                        case 3:
                            return "Parcelado Emissor";
                        default:
                            return string.Empty;
                    }
            }}

        public string NumeroNSUFormatada
        {
            get
            {
                return !string.IsNullOrEmpty(this.NumeroNSU) ? this.NumeroNSU : string.Empty;
            }
        }

        public string NumeroPVFormatada
        {
            get
            {
                return this.NumeroPV > 0 ? this.NumeroPV.ToString() : string.Empty;
            }
        }

        public string NumeroAvisoCancelFormatada
        {
            get
            {
                return this.NumeroAvisoCancel > 0 ? this.NumeroAvisoCancel.ToString() : string.Empty;
            }
        }

        public string DataLibercaoFormatada
        {
            get
            {
                return !string.IsNullOrEmpty(this.DataLibercao) ? this.DataLibercao : string.Empty;
            }
        }

        public string DataCancelamentoFormatada
        {
            get
            {
                DateTime dt;
                string str = this.DataCancelamento.ToString().Length == 8 ? this.DataCancelamento.ToString() : "0" + this.DataCancelamento.ToString();

                if (DateTime.TryParseExact(str, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dt))
                {
                    return dt.ToString("dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string ValorTransacaoFormatada
        {
            get
            {
                return this.ValorTransacao > 0 ? this.ValorTransacao.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }

        public string ValorCanceladoFormatado
        {
            get
            {
                return this.ValorCancelado > 0 ? this.ValorCancelado.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }

        public string DescCodigoCancelamentoFormatada
        {

            get
            {
                if (!string.IsNullOrEmpty(this.DescCodigoCancelamento))
                {
                    if (this.DescCodigoCancelamento == "Cancel. Internet (IS)")
                    {
                        return "Internet (IS)";
                    }
                    if (this.DescCodigoCancelamento == "Cancel. Internet")
                    {
                        return "Internet";
                    }
                }
                return string.Empty;
            }

            //    get
            //    {
            //        return !string.IsNullOrEmpty(this.DescCodigoCancelamento) ? this.DescCodigoCancelamento : string.Empty;
            //    }
        }

        public string DescEstabelecimentoFormatada
        {
            get
            {
                return !string.IsNullOrEmpty(this.DescEstabelecimento) ? this.DescEstabelecimento : string.Empty;
            }
        }

        public string DescTipoCancelamentoFormatada
        {
            get
            {
                if (!string.IsNullOrEmpty(this.TipoCancelamento))
                {
                    if (this.TipoCancelamento == "P")
                    {
                        return "Parcial";
                    }
                    if (this.TipoCancelamento == "T")
                    {
                        return "Total";
                    }
                }
                return string.Empty;
            }

            //     get
            //    {
            //       return !string.IsNullOrEmpty(this.TipoCancelamento) ? this.DescTipoCancelamento : string.Empty;
            //  }
        }

        public string NumCartaoFormatada
        {
            get
            {
                return !string.IsNullOrEmpty(this.NumCartao) ? this.NumCartao : string.Empty;
            }
        }

        #endregion
    }
}
