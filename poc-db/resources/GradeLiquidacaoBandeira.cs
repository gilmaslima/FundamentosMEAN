using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrasEntidades.Modelo
{
    /// <summary>
    /// DADOS DA GRADE PARA LIQUIDACAO FINANCEIRA (SPB) AO BANCOS  VALORES ANALITICO POR BANDEIRA                            
    /// BOOK : PV761CB
    /// </summary>
    public class GradeLiquidacaoBandeira:ModeloBase
    {
        /// <summary>
        /// TIPO DE REGISTRO.
        /// 01 - REGISTRO BANCO PRINCIPAL.
        /// </summary>
        public int TipoRegistro { get; set; }

        /// <summary>
        ///TIPO DE SOLICITACAO.
        ///B - BANCO PARTICIPANTE
        ///L - BANCO LIQUIDANDE
        /// </summary>
        public string TipoSolicitacao { get; set; }
        /// <summary>
        /// TIPO DE MOVIMENTACAO.
        /// C - VALOR A RECEBER
        /// D - VALOR A PAGAR
        /// </summary>
        public string TipoMovimentacao { get; set; }

        /// <summary>
        /// SALDO TOTAL PARA LIQUIDACAO.
        /// </summary>
        public decimal ValorSaldoLiquidacao { get; set; }
        /// <summary>
        /// CODIGO DA AGENCIA DO BANCO
        /// </summary>
        public string Agencia { get; set; }

        /// <summary>
        /// CODIGO DA CONTA DO BANCO.
        /// </summary>
        public string ContaCorrente { get; set; }

        /// <summary>
        /// VLR DEB. GRADE MASTER
        /// </summary>
        public decimal ValorDebitoMaster { get; set; }

        /// <summary>
        /// SINAL DO VLR DEB. GRADE MASTER
        /// </summary>
        public string SinalDebitoMaster { get; set; }

        /// <summary>
        /// VLR DEB. GRADE VISA
        /// </summary>
        public decimal ValorDebitoVisa { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE VISA
        /// </summary>
        public string SinalDebitoVisa { get; set; }

        /// <summary>
        /// VLR DEB. GRADE CABA
        /// </summary>
        public decimal ValorDebitoCabal { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE CABAL
        /// </summary>
        public string SinalDebitoCabal { get; set; }

        /// <summary>
        /// VLR DEB. GRADE CONSTRUCARD
        /// </summary>
        public decimal ValorDebitoConstrucard { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE CONSTRUCARD
        /// </summary>
        public string SinalDebitoConstrucard { get; set; }

        /// <summary>
        /// VLR DEB. GRADE HIPERCARD
        /// </summary>
        public decimal ValorDebitoHipercard { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE HIPERCARD
        /// </summary>
        public string SinalDebitoHipercard { get; set; }

        /// <summary>
        /// VLR DEB. GRADE INSTITUICAO X
        /// </summary>
        public decimal ValorDebitoInstituicaoX { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE BANDEIRA X
        /// </summary>
        public string SinalDebitoInstituicaoX { get; set; }

        /// <summary>
        /// VLR CRE. GRADE MASTER
        /// </summary>
        public decimal ValorCreditoMaster { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE BANDEIRA X
        /// </summary>
        public string SinalCreditoMaster { get; set; }

        /// <summary>
        /// VLR CRE. GRADE VISA
        /// </summary>
        public decimal ValorCreditoVisa { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE VISA
        /// </summary>
        public string SinalCreditoVisa { get; set; }

        /// <summary>
        /// VLR CRE. GRADE CABAL
        /// </summary>
        public decimal ValorCreditoCabal { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE CABAL
        /// </summary>
        public string SinalCreditoCabal { get; set; }

        /// <summary>
        /// VLR CRE. GRADE CONSTRUCARD
        /// </summary>
        public decimal ValorCreditoConstrucard { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE CONSTRUCARD
        /// </summary>
        public string SinalCreditoConstrucard { get; set; }

        /// <summary>
        /// VLR CRE. GRADE HIPERCAD
        /// </summary>
        public decimal ValorCreditoHipercard { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE HIPERCAD
        /// </summary>
        public string SinalCreditoHipercard { get; set; }

        /// <summary>
        /// VLR CRE. GRADE INSTITUICAO X
        /// </summary>
        public decimal ValorCreditoInstituicaoX { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE INSTITUICAO X
        /// </summary>
        public string SinalCreditoInstituicaoX { get; set; }

        /// <summary>
        /// VLR DEBITO GRADE SALDO
        /// </summary>
        public decimal ValorDebitoSaldo { get; set; }
        /// <summary>
        /// SINAL DO VLR DEBITO GRADE SALDO
        /// </summary>
        public string SinalDebitoSaldo { get; set; }

        /// <summary>
        /// VLR CRE. GRADE SALDO
        /// </summary>
        public decimal ValorCreditoSaldo { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE SALDO
        /// </summary>
        public string SinalCreditoSaldo { get; set; }

        /// <summary>
        /// VLR CRE. GRADE  INSTITUICAO SICREDI
        /// </summary>
        public decimal ValorCreditoSicredi { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE INSTITUICAO SICREDI
        /// </summary>
        public string SinalCreditoSicredi { get; set; }
        /// <summary>
        /// VLR DEB. GRADE  INSTITUICAO SICREDI
        /// </summary>
        public decimal ValorDebitoSicredi { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE INSTITUICAO SICREDI
        /// </summary>
        public string SinalDebitoSicredi { get; set; }

        /// <summary>
        /// VLR DEB. GRADE BANESCARD
        /// </summary>
        public decimal ValorCreditoBanescard { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE BANESCARD
        /// </summary>
        public string SinalCreditoBanescard { get; set; }
        /// <summary>
        /// VLR CRE. GRADE BANESCARD
        /// </summary>
        public decimal ValorDebitoBanescard { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE BANESCARD
        /// </summary>
        public string SinalDebitoBanescard { get; set; }

        /// <summary>
        /// VLR DEB. GRADE ELO
        /// </summary>
        public decimal ValorCreditoElo { get; set; }
        /// <summary>
        /// SINAL DO VLR DEB. GRADE ELO
        /// </summary>
        public string SinalCreditoElo { get; set; }
        /// <summary>
        /// VLR CRE. GRADE ELO
        /// </summary>
        public decimal ValorDebitoElo { get; set; }
        /// <summary>
        /// SINAL DO VLR CRE. GRADE ELO
        /// </summary>
        public string SinalDebitoElo { get; set; }
    }
}
