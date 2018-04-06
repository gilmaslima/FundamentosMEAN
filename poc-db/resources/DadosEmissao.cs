/*
(c) Copyright [2012] Redecard S.A.
Autor : [Lucas Nicoletto da Cunha]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/11/05 - Lucas Nicoletto da Cunha - Versão Inicial
*/

using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class DadosEmissao
    {
        [DataMember]
        public PontoVenda DadosPV { get; set; }

        [DataMember]
        public string IndicadorAcessoInternet { get; set; }

        [DataMember]
        public string NomeHomePage { get; set; }

        [DataMember]
        public int CodFilialVenda { get; set; }

        [DataMember]
        public string CodGerenciaVenda { get; set; }


        [DataMember]
        public short CodZonaVenda { get; set; }


        [DataMember]
        public int CodNucleo { get; set; }

        [DataMember]
        public Int32 CodHorario { get; set; }

        [DataMember]
        public Int32 IndTipoComercial { get; set; }


        [DataMember]
        public int NumPVMatriz { get; set; }

        [DataMember]
        public string NomeFatura { get; set; }

        [DataMember]
        public int NumGrupoComercial { get; set; }

        [DataMember]
        public int NumGrupoGerencial { get; set; }

        [DataMember]
        public int TipoConsignacao { get; set; }

        [DataMember]
        public int NumPVMatrizConsignadora { get; set; }

        [DataMember]
        public int IndicadorIATA { get; set; }

        [DataMember]
        public Int32 CodLocalPagto { get; set; }

        [DataMember]
        public List<DadosProdutosNegociados> ListaProdutosNegociados { get; set; }

        [DataMember]
        public char IndSolicitacoaTecnologia { get; set; }

        [DataMember]
        public string CodTipoEquipamento { get; set; }

        [DataMember]
        public Int16 QtdeEquipamento { get; set; }

        [DataMember]
        public Int32 CodPropriedadeEquipamento { get; set; }

        [DataMember]
        public Int32 CodTipoLigacao { get; set; }

        [DataMember]
        public char LndHabilitaDigitacao { get; set; }

        [DataMember]
        public char LndHabilitaCarga { get; set; }

        [DataMember]
        public EnderecoPadrao EnderecoInstalacao { get; set; }

        [DataMember]
        public string NomeContato { get; set; }

        [DataMember]
        public int DDDInstalacao { get; set; }

        [DataMember]
        public int TelInstalacao { get; set; }

        [DataMember]
        public int RamalInstalacao { get; set; }

        [DataMember]
        public DateTime HoraFuncionamentoInicial { get; set; }

        [DataMember]
        public DateTime HoraFuncionamentoFinal { get; set; }

        [DataMember]
        public int CodRegimeTecno { get; set; }

        [DataMember]
        public int CodCentroCustoTecno { get; set; }

        [DataMember]
        public decimal ValorEquipamento { get; set; }

        [DataMember]
        public string Observacao { get; set; }

        [DataMember]
        public char LndAceitaVisa { get; set; }

        [DataMember]
        public char LndAceitaAMEX { get; set; }

        [DataMember]
        public char LndOutrosCartoes { get; set; }

        [DataMember]
        public int CodCanalFilia { get; set; }

        [DataMember]
        public int CodCelula { get; set; }

        [DataMember]
        public int CodRoteiro { get; set; }

        [DataMember]
        public int CodAgenciaFilia { get; set; }

        [DataMember]
        public int CPFVendedor { get; set; }

        [DataMember]
        public int CodInstaldorEquip { get; set; }

        [DataMember]
        public DateTime DataProposta { get; set; }

        [DataMember]
        public long NumSolicitacao { get; set; }

        [DataMember]
        public DateTime DataAberturaSolicitacao { get; set; }

        [DataMember]
        public char LndSituacao { get; set; }

        [DataMember]
        public char CodTipoMovimento { get; set; }

        [DataMember]
        public char CodFaseFilia { get; set; }

        [DataMember]
        public string CodFabricanteHardware { get; set; }

        [DataMember]
        public string CodFabricanteSoftware { get; set; }

        [DataMember]
        public long CodRENPAC { get; set; }

        [DataMember]
        public char LndShopping { get; set; }

        [DataMember]
        public long NumPDVEmissor { get; set; }

        [DataMember]
        public long CodCenario { get; set; }

        [DataMember]
        public int CodCarteira { get; set; }

        [DataMember]
        public int CodIntegrador { get; set; }


    }
}

/*
[DataContract]
public enum EHorarioFuncionamentoSvc
{
    [EnumMember]
    Comercial = 0,
    [EnumMember]
    Noturno = 1
}
[DataContract]
public enum EPropriedadeEquipSvc
{
    [EnumMember]
    REDECARD = 1,
    [EnumMember]
    TERCEIROS = 2,
    [EnumMember]
    ESTABELECIMENTO = 3
}
[DataContract]
public enum ETipoLigacaoSvc
{
    [EnumMember]
    CONCENTRADO = 1,
    [EnumMember]
    DISCADO = 2
}
[DataContract]
public enum ELocalPagtoSvc
{
    [EnumMember]
    ESTABELECIMENTO = 1,
    [EnumMember]
    CENTRALIZADORA = 2
}
[DataContract]
public enum ETipoComercialSvc
{
    [EnumMember]
    AUTONOMO = 0,
    [EnumMember]
    FILIAL = 1,
    [EnumMember]
    MATRIZ = 2
}*/