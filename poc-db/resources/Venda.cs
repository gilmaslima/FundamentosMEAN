using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.DataCash.Modelo.Util;
using System.Globalization;

namespace Redecard.PN.DataCash.Modelo
{
    /// <summary>
    /// Enum contendo os tipos de transação de uma venda
    /// </summary>
    public enum enTipoTransacao
    {
        [Title("Crédito")]
        Credito,
        [Title("Crédito AVS")]
        CreditoAVS,
        [Title("Pré-Autorização")]
        PreAutorizacao,
        [Title("Pré-Autorização AVS")]
        PreAutorizacaoAVS,
        [Title("Pagamento Recorrente")]
        PagamentoRecorrente,
        [Title("IATA")]
        IATA,
        [Title("Boleto")]
        Boleto
    }

    /// <summary>
    /// Enum contendo as formas de pagamento de uma venda
    /// </summary>
    public enum enFormaPagamento
    {
        [Title("À vista")]
        Avista,
        [Title("Parcelado")]
        Parcelado,
        [Title("Parcelado Estabelecimento")]
        ParceladoEstabelecimento,
        [Title("Parcelado Emissor")]
        ParceladoEmissor
    }

    /// <summary>
    /// Enum contendo as formas de recorrencia de uma venda
    /// </summary>
    public enum enFormaRecorrencia
    {
        [Title("Agendado")]
        FireForget,
        [Title("Histórico")]
        HistoricRecurring
    }

    /// <summary>
    /// Enum contendo as bandeiras do cartão
    /// </summary>
    public enum enBandeiras
    {
        [Title("Mastercard")]
        Master,
        [Title("Visa")]
        Visa,
        [Title("Diners")]
        Diners,
        [Title("Hipercard")]
        Hipercard,
        [Title("Elo")]
        Elo,
        [Title("American Express")]
        Amex,
        [Title("Hiper")]
        Hiper
    }

    /// <summary>
    /// Classe de Venda
    /// </summary>
    [Serializable]
    public abstract class Venda
    {

        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public virtual enTipoTransacao TipoTransacao { get; set; }

        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public virtual enFormaPagamento FormaPagamento { get; set; }

        /// <summary>
        /// Dados do cartão da venda
        /// </summary>
        public Cartao DadosCartao { get; set; }

        /// <summary>
        /// Núemro do pedido da venda
        /// </summary>
        public String NumeroPedido { get; set; }

        /// <summary>
        /// Valor da venda
        /// </summary>
        public Decimal Valor { get; set; }

        /// <summary>
        /// Valor da venda formatado 
        /// </summary>
        public String ValorFormatado
        {
            get
            {
                return this.Valor.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        public AnalisedeRisco AnalisedeRisco { get; set; }
        public String CaminhoUserControlComprovante { get; set; }

        public String CaminhoUserControlConfirmacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 GrupoFornecedor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 PVFornecedor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 CodigoEntidade { get; set; }

        /// <summary>
        /// PV é distribuidor
        /// </summary>
        public Boolean PVDistribuidor {get; set; }

        /// <summary>
        /// Instancia uma venda tipada de acordo com o tipo da transação, 
        /// forma de pagamento e forma de recorrência
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <param name="formaPagamento">Forma de pagamento</param>
        /// <param name="formaRecorrencia">Forma de recorrência</param>
        /// <returns>Venda</returns>
        public static Venda InstanciarVenda(enTipoTransacao tipoTransacao, enFormaPagamento formaPagamento, enFormaRecorrencia formaRecorrencia)
        {
            switch (tipoTransacao)
            {
                case enTipoTransacao.Credito:
                    switch (formaPagamento)
                    {
                        case enFormaPagamento.Avista: return new Modelo.VendaCreditoAVista();
                        case enFormaPagamento.ParceladoEstabelecimento: return new Modelo.VendaCreditoParceladoEstabelecimento();
                        case enFormaPagamento.ParceladoEmissor: return new Modelo.VendaCreditoParceladoEmissor();
                    }
                    break;
                case enTipoTransacao.PreAutorizacao:
                    switch (formaPagamento)
                    {
                        case enFormaPagamento.Avista: return new Modelo.VendaPreAutorizacaoAVista();
                        case enFormaPagamento.Parcelado: return new Modelo.VendaPreAutorizacaoParcelado();
                    }
                    break;
                case enTipoTransacao.PagamentoRecorrente:
                    switch (formaRecorrencia)
                    {
                        case enFormaRecorrencia.FireForget: return new Modelo.VendaPagamentoRecorrenteFireForget();
                        case enFormaRecorrencia.HistoricRecurring: return new Modelo.VendaPagamentoRecorrenteHistoricRecurring();
                    }
                    break;
                case enTipoTransacao.CreditoAVS:
                    switch (formaPagamento)
                    {
                        case enFormaPagamento.Avista: return new Modelo.VendaCreditoAVSAVista();
                        case enFormaPagamento.ParceladoEstabelecimento: return new Modelo.VendaCreditoAVSParceladoEstabelecimento();
                        case enFormaPagamento.ParceladoEmissor: return new Modelo.VendaCreditoAVSParceladoEmissor();
                    }
                    break;
                case enTipoTransacao.PreAutorizacaoAVS:
                    return new Modelo.VendaPreAutorizacaoAVS();
                case enTipoTransacao.IATA:
                    switch (formaPagamento)
                    {
                        case enFormaPagamento.Avista: return new VendaPagamentoIATAAvista();
                        case enFormaPagamento.ParceladoEstabelecimento: return new VendaPagamentoIATAParceladoEstabelecimento();
                    }
                    break;
                case enTipoTransacao.Boleto:
                    return new Modelo.VendaBoleto();
            }
            
            return default(Venda);
        }
    }

    /// <summary>
    /// Classe do cartão que realiza uma venda
    /// </summary>
    [Serializable]
    public class Cartao
    {
        /// <summary>
        /// Nome do Portador do cartão
        /// </summary>
        public String NomePortador { get; set; }

        /// <summary>
        /// Número do cartão
        /// </summary>
        public String Numero { get; set; }

        /// <summary>
        /// Número do cartão formatado com espaços
        /// </summary>
        public String NumeroFormatado
        {
            get
            {
                if (String.IsNullOrEmpty(this.Numero))
                    return this.Numero;

                StringBuilder sbCartaoFormatado = new StringBuilder();
                for (Int32 iDigito = 0; iDigito < this.Numero.Length; iDigito++)
                {
                    if (iDigito != 0 && iDigito % 4 == 0)
                        sbCartaoFormatado.Append(" ");
                    sbCartaoFormatado.Append(this.Numero[iDigito]);
                }
                return sbCartaoFormatado.ToString();
            }
        }

        /// <summary>
        /// Número do cartão visível somente os últimos 4 dígitos
        /// </summary>
        public String NumeroCriptografado
        {
            get
            {
                StringBuilder sbCartaoFormatado = new StringBuilder();
                sbCartaoFormatado.Append("**** **** **** ");
                sbCartaoFormatado.Append(this.Numero.Right(4));
                return sbCartaoFormatado.ToString();
            }
        }

        /// <summary>
        /// Bandeira do cartão
        /// </summary>
        public enBandeiras Bandeira { get; set; }

        /// <summary>
        /// Mês de validade do cartão
        /// </summary>
        public String MesValidade { get; set; }

        /// <summary>
        /// Ano de validade do cartão
        /// </summary>
        public String AnoValidade { get; set; }

        /// <summary>
        /// Validade do cartão formatada
        /// </summary>
        public String Validade
        {
            get
            {
                return this.MesValidade + "/" + AnoValidade;
            }
        }

        /// <summary>
        /// Código de segurança do cartão
        /// </summary>
        public String CodigoSeguranca { get; set; }

        /// <summary>
        /// Código de segurança do cartão
        /// </summary>
        public String Parcelas { get; set; }
    }

    /// <summary>
    /// Class contendo as informações sobre os juros nas compras de crédito parceladas emissor
    /// </summary>
    [Serializable]
    public class Taxa
    {
        /// <summary>
        /// Custo efetivo
        /// </summary>
        public String CET { get; set; }

        /// <summary>
        /// Taxa de juros
        /// </summary>
        public String Juros { get; set; }

        /// <summary>
        /// Valor da parcela com juros
        /// </summary>
        public Decimal ValorParcelaJuros { get; set; }

        /// <summary>
        /// Valor total com juros
        /// </summary>
        public Decimal ValorTotalJuros { get; set; }

        /// <summary>
        /// Custo efetivo formatado 
        /// </summary>
        public String CETFormatado
        {
            get
            {
                return this.CET == "-" ? "-" : String.Concat(this.CET, "%");
            }
        }

        /// <summary>
        /// Juros formatado 
        /// </summary>
        public String JurosFormatado
        {
            get
            {
                return this.Juros == "-" ? "-" : String.Concat(this.Juros, "%");
            }
        }

        /// <summary>
        /// Valor da parcela com juros formatado 
        /// </summary>
        public String ValorParcelaJurosFormatado
        {
            get
            {
                return this.ValorParcelaJuros == 0 ? "-" : this.ValorParcelaJuros.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Valor total com juros formatado 
        /// </summary>
        public String ValorTotalJurosFormatado
        {
            get
            {
                return this.ValorTotalJuros == 0 ? "-" : this.ValorTotalJuros.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }
    }

    /// <summary>
    /// Classe do contendo as informações do titular do cartão
    /// </summary>
    [Serializable]
    public class InfoTitular
    {
        /// <summary>
        /// CPF do titular
        /// </summary>
        public String CPF { get; set; }

        /// <summary>
        /// Endereço do titular
        /// </summary>
        public Endereco Endereco { get; set; }
    }

    /// <summary>
    /// Classe de Passageiros para transações de venda IATA
    /// </summary>
    [Serializable]
    public class Passageiro
    {
        /// <summary>
        /// Indice
        /// </summary>
        public Int32 Id { get; set; }
        /// <summary>
        /// Nome do passageiro
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// Bilhete do passageiro
        /// </summary>
        public String NumeroBilhete { get; set; }
        /// <summary>
        /// Código de referência do Passageiro
        /// </summary>
        public String CodigoReferencia { get; set; }


    }

    /// <summary>
    /// Classe de venda de crédito à vista
    /// </summary>
    [Serializable]
    public class VendaCreditoAVista : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.Credito;
            }
        }

        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.Avista;
            }
        }
    }

    /// <summary>
    /// Classe de venda de crédito parcelado estabelecimento
    /// </summary>
    [Serializable]
    public class VendaCreditoParceladoEstabelecimento : VendaCreditoAVista
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.Credito;
            }
        }

        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.ParceladoEstabelecimento;
            }
        }
    }

    /// <summary>
    /// Classe de venda de crédito parcelado emissor
    /// </summary>
    [Serializable]
    public class VendaCreditoParceladoEmissor : VendaCreditoAVista
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.Credito;
            }
        }

        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.ParceladoEmissor;
            }
        }
    }

    /// <summary>
    /// Classe de venda de crédito à vista
    /// </summary>
    [Serializable]
    public class VendaCreditoAVSAVista : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.CreditoAVS;
            }
        }

        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.Avista;
            }
        }

        /// <summary>
        /// Informações do titular
        /// </summary>
        public InfoTitular InfoTitular { get; set; }
    }

    /// <summary>
    /// Classe de venda de crédito parcelado estabelecimento
    /// </summary>
    [Serializable]
    public class VendaCreditoAVSParceladoEstabelecimento : VendaCreditoAVSAVista
    {
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.ParceladoEstabelecimento;
            }
        }

    }

    /// <summary>
    /// Classe de venda de crédito parcelado emissor
    /// </summary>
    [Serializable]
    public class VendaCreditoAVSParceladoEmissor : VendaCreditoAVSAVista
    {
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.ParceladoEmissor;
            }
        }
    }

    /// <summary>
    /// Classe de venda de pré-autorização à vista
    /// </summary>
    [Serializable]
    public class VendaPreAutorizacaoAVista : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.PreAutorizacao;
            }
        }
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.Avista;
            }
        }
    }

    /// <summary>
    /// Classe de venda de pré-autorização parcelado
    /// </summary>
    [Serializable]
    public class VendaPreAutorizacaoParcelado : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.PreAutorizacao;
            }
        }
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.Parcelado;
            }
        }
    }

    /// <summary>
    /// Classe de venda de pré-autorização AVS
    /// </summary>
    [Serializable]
    public class VendaPreAutorizacaoAVS : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.PreAutorizacaoAVS;
            }
        }

        /// <summary>
        /// Informações do titular
        /// </summary>
        public InfoTitular InfoTitular { get; set; }
    }

    /// <summary>
    /// Classe abstrata de venda de pagamento recorrente
    /// </summary>
    [Serializable]
    public abstract class VendaPagamentoRecorrente : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.PagamentoRecorrente;
            }
        }

        /// <summary>
        /// Forma de recorrência da venda
        /// </summary>
        public abstract enFormaRecorrencia FormaRecorrencia { get; }
        
    }

    /// <summary>
    /// Classe de venda de pagamento recorrente Fire and Forget
    /// </summary>
    [Serializable]
    public class VendaPagamentoRecorrenteFireForget : VendaPagamentoRecorrente
    {
        /// <summary>
        /// Forma de recorrencia da venda
        /// </summary>
        public override enFormaRecorrencia FormaRecorrencia
        {
            get
            {
                return enFormaRecorrencia.FireForget;
            }
        }

        /// <summary>
        /// Valor da recorrência
        /// </summary>
        public Decimal ValorRecorrencia { get; set; }

        /// <summary>
        /// Valor da recorrência formatada R$
        /// </summary>
        public String ValorRecorrenciaFormatada
        {
            get
            {
                return ValorRecorrencia.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Frequência
        /// </summary>
        public String Frequencia { get; set; }

        /// <summary>
        /// Texto de exibição da frequência
        /// </summary>
        public String FrequenciaExibicao { get; set; }

        /// <summary>
        /// Data de início
        /// </summary>
        public String DataInicio { get; set; }

        /// <summary>
        /// Data início formatada dd/MM/yyyy
        /// </summary>
        public String DataInicioFormatada
        {
            get
            {
                return DataInicio;
                //return string.Format("{0}/{1}/{2}"
                //    , DataInicio.Substring(8,2)
                //    , DataInicio.Substring(6,2)
                //    , DataInicio.Substring(1,4));
            }
        }

        /// <summary>
        /// Quantidade de recorrência
        /// </summary>
        public String QuantidadeRecorencia { get; set; }

        /// <summary>
        /// Valor da última cobrança
        /// </summary>
        public Decimal ValorUltimaCobranca { get; set; }

        /// <summary>
        /// Valor da última cobrança formatada R$
        /// </summary>
        public String ValorUltimaCobrancaFormatada
        {
            get
            {
                return ValorUltimaCobranca.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        /// <summary>
        /// Data da última cobrança
        /// </summary>
        public String DataUltimaCobranca { get; set; }

        /// <summary>
        /// Data da última cobrança formatada dd/MM/yyyy
        /// </summary>
        public String DataUltimaCobrancaFormatada
        {
            get
            {
                return DataUltimaCobranca;
            }
        }

    }

    /// <summary>
    /// Classe de venda de pagamento recorrente Historic Recurring
    /// </summary>
    [Serializable]
    public class VendaPagamentoRecorrenteHistoricRecurring : VendaPagamentoRecorrente
    {
        /// <summary>
        /// Forma de recorrencia da venda
        /// </summary>
        public override enFormaRecorrencia FormaRecorrencia
        {
            get
            {
                return enFormaRecorrencia.HistoricRecurring;
            }
        }
    }

    /// <summary>
    /// Classe de venda de boleto
    /// </summary>
    [Serializable]
    public class VendaBoleto : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.Boleto;
            }
        }

        /// <summary>
        /// Dados do cliente
        /// </summary>
        public DadosCliente DadosCliente { get; set; }

        /// <summary>
        /// Endereço de cobrança
        /// </summary>
        public Endereco EnderecoCobranca { get; set; }

        /// <summary>
        /// Dados de Pagamento
        /// </summary>
        public DadosPagamento DadosPagamento { get; set; }
    }



    /// <summary>
    /// Classe de venda de pagamento IATA
    /// </summary>
    [Serializable]
    public abstract class VendaPagamentoIATA : Venda
    {
        /// <summary>
        /// Tipo de transação da venda
        /// </summary>
        public override enTipoTransacao TipoTransacao
        {
            get
            {
                return enTipoTransacao.IATA;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 NumeroReferencia { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Passageiro> Passageiros { get; set; }
        /// <summary>
        /// Nome da Agencia Viagem
        /// </summary>
        public String NomeAgenciaViagem { get; set; }
        
        public String TipoVoo { get; set; }

        public Decimal ValorPassagem { get; set; }
        public Decimal TotalPassagem { get { return (ValorPassagem * Passageiros.Count); } }
        public Decimal TotalaPagar
        {
            get
            {
                Decimal total = TotalPassagem + TotalTaxaEmbarque;
                return total;
            }
        }

        public String TotalaPagarFormatada
        {
            get
            {
                return this.TotalaPagar.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        public Decimal TotalTaxaEmbarque { get { return (TaxaEmbarque * Passageiros.Count); } }
        public String FusoHorarioPartida { get; set; }
        public String FusoHorarioPartidaDescricao { get; set; }

        public String CodigoCompanhia { get; set; }
        public String Classe { get; set; }
        public String ClasseDescricao { get; set; }
        public String CodigoAeroportoPartida { get; set; }
        public String CodigoAeroportoDestino { get; set; }
        public String DataPartida { get; set; }
        public Decimal TaxaEmbarque { get; set; }
        public String TaxaEmbarqueFormatada
        {
            get
            {
                return this.TaxaEmbarque.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

    }

    /// <summary>
    /// Classe de venda de pagamento IATA à vista
    /// </summary>
    [Serializable]
    public class VendaPagamentoIATAAvista : VendaPagamentoIATA
    {
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.Avista;
            }
        }
    }

    /// <summary>
    /// Classe de venda de pagamento IATA parcelado estabelecimento
    /// </summary>
    [Serializable]
    public class VendaPagamentoIATAParceladoEstabelecimento : VendaPagamentoIATA
    {
        /// <summary>
        /// Forma de pagamento da venda
        /// </summary>
        public override enFormaPagamento FormaPagamento
        {
            get
            {
                return enFormaPagamento.ParceladoEstabelecimento;
            }
        }
    }
}
