using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class PontoDeVenda
    {

        [DataMember]
        public Int32? NumeroPontoDeVenda { get; set; }

        [DataMember]
        public Int64? NumeroCNPJCPF { get; set; }

        [DataMember]
        public String RazaoSocial { get; set; }

        [DataMember]
        public DateTime? DataCadastroPontoVenda { get; set; }

        [DataMember]
        public DateTime? DataAssinaturaProposta { get; set; }

        [DataMember]
        public DateTime? DataLiberacaoRav { get; set; }

        [DataMember]
        public Int64? NumeroCPFVendedor { get; set; }

        [DataMember]
        public Int32? CodigoTipoEstabelecimento { get; set; }

        [DataMember]
        public Int32? CodigoVisibilidadePontoVenda { get; set; }

        [DataMember]
        public Int32? CodigoHorarioFuncionamento { get; set; }

        [DataMember]
        public Int32? CodigoClassificacaoRisco { get; set; }

        [DataMember]
        public Int32? CodigoComercializacao { get; set; }

        [DataMember]
        public Int32? IndicadorPontoVendaVip { get; set; }

        [DataMember]
        public Char? CodigoCategoriaMercadologica { get; set; }

        [DataMember]
        public Char? CodigoCategoriaFinanceira { get; set; }

        [DataMember]
        public Char? CodigoCancelamentoPontoVenda { get; set; }

        [DataMember]
        public Int32? CodigoInterCodeDiners { get; set; }

        [DataMember]
        public DateTime? DataCancelamentoPontoVenda { get; set; }

        [DataMember]
        public DateTime? DataReativacaoPontoVenda { get; set; }

        [DataMember]
        public String PessoaContato { get; set; }

        [DataMember]
        public String NomeLogradouro { get; set; }

        [DataMember]
        public String NomeBairro { get; set; }

        [DataMember]
        public String NomeCidade { get; set; }

        [DataMember]
        public String NomeUF { get; set; }

        [DataMember]
        public String CodigoCEP { get; set; }

        [DataMember]
        public String CodigoCompCEP { get; set; }

        [DataMember]
        public String NumeroDDD1 { get; set; }

        [DataMember]
        public Int32? NumeroTelefone1 { get; set; }

        [DataMember]
        public Int32? NumeroRamal1 { get; set; }

        [DataMember]
        public String NomeLogradouroCorrespondencia { get; set; }

        [DataMember]
        public String NomeBairroCorrespondencia { get; set; }

        [DataMember]
        public String NomeCidadeCorrespondencia { get; set; }

        [DataMember]
        public String NomeUFCorrespondencia { get; set; }

        [DataMember]
        public String CodigoCEPCorrespondencia { get; set; }

        [DataMember]
        public String CodigoCompCEPCorrespondencia { get; set; }

        [DataMember]
        public Char? IndTipoOperacaoCredito { get; set; }

        [DataMember]
        public Char? IndTipoOperacaoDebito { get; set; }

        [DataMember]
        public Char? IndTipoOperacaoSidecard { get; set; }

        [DataMember]
        public Char? CodigoPracaForaPontoVenda { get; set; }

        [DataMember]
        public String NomePlaqueta1 { get; set; }

        [DataMember]
        public String NomePlaqueta2 { get; set; }

        [DataMember]
        public String NomeFatura { get; set; }

        [DataMember]
        public Int32? CodigoCredenciamentoPontoVenda { get; set; }

        [DataMember]
        public Char? CodigoCategoriaMarketing { get; set; }

        [DataMember]
        public Int32? CodigoComplementoCategoria { get; set; }

        [DataMember]
        public DateTime? DataComplementoCategoria { get; set; }

        [DataMember]
        public Int32? CodigoCancelamentoAtendimento { get; set; }

        [DataMember]
        public Char? CodigoCategoriaAnterior { get; set; }

        [DataMember]
        public DateTime? DataCategoria { get; set; }

        [DataMember]
        public Char? IndicadorBloqueioOdc { get; set; }

        [DataMember]
        public Int32? CodigoMotivoProximaVisita { get; set; }

        [DataMember]
        public DateTime? DataProximaVisita { get; set; }

        [DataMember]
        public DateTime? DataOutroPontovenda { get; set; }

        [DataMember]
        public DateTime? DataUltimaRVPontoVenda { get; set; }

        [DataMember]
        public Int32? CodigoProdutoSidecard { get; set; }

        [DataMember]
        public String DescricaoProdutoSidecard { get; set; }

        [DataMember]
        public Int32? CodigoGrupoRamo { get; set; }

        [DataMember]
        public String DescricaoGrupoRamo { get; set; }

        [DataMember]
        public Int32? CodigoRamoAtivididade { get; set; }

        [DataMember]
        public String DescricaoRamoAtividade { get; set; }

        [DataMember]
        public Char? CodigoCategoria { get; set; }

        [DataMember]
        public String DescricaoCategoria { get; set; }

        [DataMember]
        public Int32? CodigoFilial { get; set; }

        [DataMember]
        public String NomeFilial { get; set; }

        [DataMember]
        public Int32? CodigoZona { get; set; }

        [DataMember]
        public String NomeZona { get; set; }

        [DataMember]
        public Char? CodigoNucleo { get; set; }

        [DataMember]
        public String NomeNucleo { get; set; }

        [DataMember]
        public Int32? NumeroCentralizadora { get; set; }

        [DataMember]
        public Int32? CodigoCanal { get; set; }

        [DataMember]
        public String DescricaoCanal { get; set; }

        [DataMember]
        public Int32? CodigoCelula { get; set; }

        [DataMember]
        public Char? CodigoIdentificacaoCelula { get; set; }

        [DataMember]
        public DateTime? DataUltimaAtualizacaoRegistro { get; set; }

        [DataMember]
        public String UsuarioUltimaAtualizacaoRegistro { get; set; }

        [DataMember]
        public String TimestampUltimaAtualizacaoRegistro { get; set; }

        [DataMember]
        public Char? CodigoRoteiro { get; set; }

        [DataMember]
        public String DescricaoCategoriaAnterior { get; set; }

        [DataMember]
        public String DescricaoCelula { get; set; }

        [DataMember]
        public Char? CodigoTipoPessoa { get; set; }

        [DataMember]
        public DateTime? DataFundacao { get; set; }

        [DataMember]
        public Int32? NumeroCentralizadora2 { get; set; }

        [DataMember]
        public Int32? NumeroGrupoComercial { get; set; }

        [DataMember]
        public Int32? NumeroMatriz { get; set; }

        [DataMember]
        public String TimestampUltimaAtualizacaoRegistro2 { get; set; }

        [DataMember]
        public Int32? IndicadorPontoVendaVip2 { get; set; }

        [DataMember]
        public String DescricaoComplementoCategoria { get; set; }

        [DataMember]
        public Char? IndicadorTipoOperacaoVoucher { get; set; }

        [DataMember]
        public Char? IndicadorRecadastramento { get; set; }

        [DataMember]
        public DateTime? DataRecadastramento { get; set; }

        [DataMember]
        public String NumeroDDDFax { get; set; }

        [DataMember]
        public Int32? NumeroTelefoneFax { get; set; }

        [DataMember]
        public String Email { get; set; }

        [DataMember]
        public Int32? CodigoSubCategoriaMarketing { get; set; }

        [DataMember]
        public Int32? CodigoAgenciaFiliacao { get; set; }

        [DataMember]
        public Double? PercentualDescontoFiliacao { get; set; }

        [DataMember]
        public Double? PercentualDescontoOcorrencias { get; set; }

        [DataMember]
        public String NumeroLogradouro { get; set; }

        [DataMember]
        public String ComplementoEndereco { get; set; }

        [DataMember]
        public String NumeroLogradouroCorrespondencia { get; set; }

        [DataMember]
        public String ComplementoEnderecoCorrespondencia { get; set; }

        [DataMember]
        public String NumeroDDD2 { get; set; }

        [DataMember]
        public Int32? NumeroTelefone2 { get; set; }

        [DataMember]
        public Int32? NumeroRamal2 { get; set; }

        [DataMember]
        public String HomePage { get; set; }

        [DataMember]
        public Char? IndicadorAcessaInternet { get; set; }

        [DataMember]
        public Char? IndicadorLocalizacaoFisica { get; set; }

        [DataMember]
        public Char? IndicadorConservacaoLoja { get; set; }

        [DataMember]
        public Char? IndicadorRegiaoLoja { get; set; }

        [DataMember]
        public Char? IndicadorCartaoVisa { get; set; }

        [DataMember]
        public Char? IndicadorCartaoAmex { get; set; }

        [DataMember]
        public Char? IndicadorCartaoOutros { get; set; }

        [DataMember]
        public Int32? CodigoTipoConsignacao { get; set; }

        [DataMember]
        public Int32? NumeroConsignador { get; set; }

        [DataMember]
        public Char? IndicadorComercializacaoNormal { get; set; }

        [DataMember]
        public Char? IndicadorComercializacaoCatalogo { get; set; }

        [DataMember]
        public Char? IndicadorComercializacaoTelefone { get; set; }

        [DataMember]
        public Char? IndicadorComercializacaoEletronico { get; set; }

        [DataMember]
        public DateTime? DataAlteracaoEnderecoCorrespondencia { get; set; }

        [DataMember]
        public Char? IndicadorDevolucaoCorrespondencia { get; set; }

        [DataMember]
        public DateTime? DataConfirmacaoDados { get; set; }

        [DataMember]
        public Int32? NumeroGrupoGerencial { get; set; }

        [DataMember]
        public Char? IndicadorImpressoraFiscal { get; set; }

        [DataMember]
        public Int32? CodigoImpressoraFiscal { get; set; }

        [DataMember]
        public Char? IndicadorCreditoProprio { get; set; }

        [DataMember]
        public Char? IndicadorChequePreDatado { get; set; }

        [DataMember]
        public Int32? ValorFaturamentoMensal { get; set; }

        [DataMember]
        public Char? IndicadorFinanceira { get; set; }

        [DataMember]
        public Int32? CodigoFinanceira1 { get; set; }

        [DataMember]
        public Int32? CodigoFinanceira2 { get; set; }

        [DataMember]
        public Int32? CodigoFinanceira3 { get; set; }

        [DataMember]
        public String UsuarioCadastroPontoVenda { get; set; }

        [DataMember]
        public Char? IndicadorAceitaRavPos { get; set; }

        [DataMember]
        public DateTime? DataCadastroSefaz { get; set; }

        [DataMember]
        public DateTime? DataCancelamentoSefaz { get; set; }

        [DataMember]
        public Char? IndicadorExclusividadeVoucher { get; set; }

        [DataMember]
        public Char? IndicadorPurchase { get; set; }

        [DataMember]
        public Char? CodigoTipoMoedaRamo { get; set; }

        [DataMember]
        public Char? IndicadorMemoFile { get; set; }

        [DataMember]
        public Int64? NumeroCNPJCPFWM { get; set; }

        [DataMember]
        public Char? CodigoNet { get; set; }

        [DataMember]
        public Double? ValorTaxaAdesao { get; set; }

        [DataMember]
        public Double? PercentualDescontoFiliacao2 { get; set; }

        [DataMember]
        public Char? IndicadorEnvioExtratoEmail { get; set; }

        [DataMember]
        public String TimestampEnderecoComercial { get; set; }

        [DataMember]
        public String TimestampEnderecoCorrespondecia { get; set; }

        [DataMember]
        public String NumeroDDDCV { get; set; }

        [DataMember]
        public Int32? NumeroTelefoneCV { get; set; }

        [DataMember]
        public Int32? NumeroRamalCV { get; set; }

        [DataMember]
        public DateTime? DataAlteracaoEnderecoComercial { get; set; }

        [DataMember]
        public Int32? NumeroSegmentoAtual { get; set; }

        [DataMember]
        public Int32? NumeroSegmentoAnterior { get; set; }

        [DataMember]
        public Int32? NumeroSegmentoTendencia { get; set; }

        [DataMember]
        public DateTime? DataUltimaAtualizacaoSegmento { get; set; }

        [DataMember]
        public DateTime? DataVigenciaSegmento { get; set; }

        [DataMember]
        public Int32? CodigoClienteEspecial { get; set; }

        [DataMember]
        public String DescricaoClienteEspecial { get; set; }

        [DataMember]
        public Int32? NumeroSegmentoTecnologia { get; set; }

        [DataMember]
        public Int32? CodigoPromocaoEmissor { get; set; }

        [DataMember]
        public Int32? CodigoBancoPromocaoEmissor { get; set; }

        [DataMember]
        public String TimestampPromocaoEmissor { get; set; }

        [DataMember]
        public String NomeLogradouroTecnologia { get; set; }

        [DataMember]
        public String NomeBairroTecnologia { get; set; }

        [DataMember]
        public String NomeCidadeTecnologia { get; set; }

        [DataMember]
        public String NomeUFTecnologia { get; set; }

        [DataMember]
        public String CodigoCEPTecnologia { get; set; }

        [DataMember]
        public String CodigoComplementoCEPTecnologia { get; set; }

        [DataMember]
        public String NumeroLogradouroTecnologia { get; set; }

        [DataMember]
        public String ComplementoEnderecoTecnologia { get; set; }

        [DataMember]
        public DateTime? DataAlteracaoEnderecoTecnologia { get; set; }

        [DataMember]
        public String TimestampEndTecnologia { get; set; }

    }
}
