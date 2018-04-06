using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Credenciamento.Modelo
{
    public class Proposta
    {
        public Char CodTipoPessoa { get; set; }
        
        public Int64 NumCnpjCpf { get; set; }
        
        public Int32 IndSeqProp { get; set; }
        
        public String UsuarioUltimaAtualizacao { get; set; }
        
        public String UsuarioInclusao { get; set; }
        
        public Int32? CodCanal { get; set; }
        
        public Int32? CodMotivoRecusa { get; set; }
        
        public Int32? NumPdv { get; set; }
        
        public Char? CodTipoMovimento { get; set; }
        
        public Int32? CodGrupoRamo { get; set; }
        
        public Int32? CodRamoAtividade { get; set; }
        
        public DateTime? DataFundacao { get; set; }
        
        public String RazaoSocial { get; set; }
        
        public Char? IndEnderecoIgualCom { get; set; }
        
        public String PessoaContato { get; set; }
        
        public Char? IndAcessaInternet { get; set; }
        
        public String NomeEmail { get; set; }
        
        public String NomeHomePage { get; set; }
        
        public String NumDDD1 { get; set; }
        
        public Int32? NumTelefone1 { get; set; }
        
        public Int32? Ramal1 { get; set; }
        
        public String NumDDDFax { get; set; }
        
        public Int32? NumTelefoneFax { get; set; }
        
        public String NumDDD2 { get; set; }
        
        public Int32? NumTelefone2 { get; set; }
        
        public Int32? Ramal2 { get; set; }
        
        public Char? IndRegiaoLoja { get; set; }
        
        public String NomePlaqueta1 { get; set; }
        
        public String NomePlaqueta2 { get; set; }
        
        public Int32? CodFilial { get; set; }
        
        public Char? CodGerencia { get; set; }
        
        public Int32? CodCarteira { get; set; }
        
        public Int32? CodZona { get; set; }
        
        public Int32? CodNucleo { get; set; }
        
        public Int32? CodHoraFuncionamentoPV { get; set; }
        
        public Char? IndicadorMaquineta { get; set; }
        
        public Int32? QuantidadeMaquineta { get; set; }
        
        public Char? IndicadorIATA { get; set; }
        
        public Int32? CodTipoEstabelecimento { get; set; }
        
        public Char? IndComercializacaoNormal { get; set; }
        
        public Char? IndComercializacaoCatalogo { get; set; }
        
        public Char? IndComercializacaoTelefone { get; set; }
        
        public Char? IndComercializacaoEletronico { get; set; }
        
        public Int32? NumeroMatriz { get; set; }
        
        public String NomeFatura { get; set; }
        
        public Int32? CodTipoConsignacao { get; set; }
        
        public Int32? NumeroConsignador { get; set; }
        
        public Int32? NumGrupoComercial { get; set; }
        
        public Int32? NumGrupoGerencial { get; set; }
        
        public Int32? CodLocalPagamento { get; set; }
        
        public Int32? NumCentralizadora { get; set; }
        
        public Char? IndSolicitacaoTecnologia { get; set; }
        
        public String NomeProprietario1 { get; set; }
        
        public Int64? NumCPFProprietario1 { get; set; }
        
        public Char? TipoPessoaProprietario1 { get; set; }
        
        public String NomeProprietario2 { get; set; }
        
        public DateTime? DataNascProprietario2 { get; set; }
        
        public Int64? NumCPFProprietario2 { get; set; }
        
        public Char? TipoPessoaProprietario2 { get; set; }
        
        public Int32? CodCelula { get; set; }
        
        public Int32? CodRoteiro { get; set; }
        
        public Int32? CodAgenciaFiliacao { get; set; }
        
        public Int32? CodTerceirizacaoVista { get; set; }
        
        public DateTime? DataCadastroProposta { get; set; }
        
        public Int64? NumCPFVendedor { get; set; }
        
        public Int32? CodFaseFiliacao { get; set; }
        
        public Int32? CodImpressoraFiscal { get; set; }
        
        public Char? IndFinanceira { get; set; }
        
        public Int32? CodFinanceira1 { get; set; }
        
        public Int32? CodFinanceira2 { get; set; }
        
        public Int32? CodFinanceira3 { get; set; }
        
        public Char? SituacaoProposta { get; set; }
        
        public Int32? CodPesoTarget { get; set; }
        
        public Char? CodPeriodicidadeRAV { get; set; }
        
        public Int32? CodPeriodicidadeDia { get; set; }
        
        public Double? ValorTaxaAdesao { get; set; }
        
        public String IndEnvioForcaVenda { get; set; }
        
        public Int32? NumInvestigacaoPropDigitada { get; set; }
        
        public String IndOrigemProposta { get; set; }
        
        public String CodigoCampanha { get; set; }
        
        public Char? CodModeloProposta { get; set; }
        
        public Int32? CodigoEVD { get; set; }
        
        public Char? IndProntaInstalacao { get; set; }
        
        public Char? IndEnvioExtratoEmail { get; set; }
        
        public String IndControle { get; set; }
        
        public String CodTipoConselhoRegional { get; set; }
        
        public String NumConselhoRegional { get; set; }
        
        public String UFConselhoRegional { get; set; }
        
        public String CodigoCNAE { get; set; }
    }
}
