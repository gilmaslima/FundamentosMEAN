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
    /// Classe de entidade do RAV Automático - Entrada.
    /// </summary>
    [DataContract]
    public class ModRAVAutomatico
    {
        [DataMember]
        public string IndFull { get; set; }

        [DataMember]
        public List<ModRAVAutomaticoBandeira> Bandeiras { get; set; }

        [DataMember]
        public ECodFuncao Funcao { get; set; }

        [DataMember]
        public long NumeroPDV { get; set; }

        [DataMember]
        public string CodigoProduto { get; set; }

        [DataMember]
        public char IndContratoPortal { get; set; }

        [DataMember]
        public char IndPRFComercial { get; set; }

        [DataMember]
        public ElndProdutoAntecipa TipoRAV { get; set; }

        [DataMember]
        public int NumParcelaIni { get; set; }
        
        [DataMember]
        public int NumParcelaFim { get; set; }
        
        [DataMember]
        public int CodSituacao { get; set; }
        
        [DataMember]
        public EPeriodicidade Periodicidade { get; set; }

        [DataMember]
        public EDiaSemana DiaSemana { get; set; }

        [DataMember]
        public string DiaAntecipacao { get; set; }
        
        [DataMember]
        public DateTime DataVigenciaIni { get; set; }

        [DataMember]
        public DateTime DataVigenciaFim { get; set; }

        [DataMember]
        public decimal ValorMinimo { get; set; }

        [DataMember]
        public ElndAntecEstoq IndAnteEstoq { get; set; }

        [DataMember]
        public DateTime DataIniEstoq { get; set; }
        
        [DataMember]
        public string CodVenda { get; set; }
                
        [DataMember]
        public DateTime DataContrato { get; set; }

        [DataMember]
        public string DataContratoFormatada { get; set; }
        
        [DataMember]
        public string NomeContrato { get; set; }

        [DataMember]
        public int CodMotivoExclusao { get; set; }

        [DataMember]
        public string DescMotivoExclusao { get; set; }

        [DataMember]
        public long NumeroPDVRef { get; set; }

        [DataMember]
        public int QtdeDiasCancelamento { get; set; }

        [DataMember]
        public ModRAVAutomaticoDados DadosRetorno { get; set; }

        public ModRAVAutomatico()
        { DadosRetorno = new ModRAVAutomaticoDados(); }
    }

    public enum ECodFuncao
    { Simulacao, Efetivar, Consultar }

    public enum EPeriodicidade
    { Diario, Semanal, Quinzenal, Mensal }

    public enum EDiaSemana
    { Segunda, Terca, Quarta, Quinta, Sexta }

    public enum ElndAntecEstoq
    { Sim, Nao }

    /// <summary>
    /// Classe de entidade do RAV Automático - Saída.
    /// </summary>
    [DataContract]
    public class ModRAVAutomaticoDados
    {
        [DataMember]
        public int CodRetorno { get; set; }

        [DataMember]
        public string MsgRetorno { get; set; }

        [DataMember]
        public string Estabelecimento { get; set; }

        [DataMember]
        public string CPF_CNPJ { get; set; }

        [DataMember]
        public int CodCategoria { get; set; }

        [DataMember]
        public string DescCategoria { get; set; }

        [DataMember]
        public decimal TaxaCategoria { get; set; }

        [DataMember]
        public string DescSituacaoCategoria { get; set; }

        [DataMember]
        public int CodSituacaoPendente { get; set; }

        [DataMember]
        public DateTime DataBaseAntecipacao { get; set; }

        [DataMember]
        public DateTime DataProximaAntecipacao { get; set; }

        [DataMember]
        public int CodOpidAlteracao { get; set; }

        [DataMember]
        public DateTime DataAlteracao { get; set; }

        [DataMember]
        public string HoraAlteracao { get; set; }

        [DataMember]
        public int CodOpidAutorizacao { get; set; }

        [DataMember]
        public DateTime DataAutorizacao { get; set; }

        [DataMember]
        public string HoraAutorizacao { get; set; }

        [DataMember]
        public DateTime DataAgendaExclusao { get; set; }

        [DataMember]
        public long NumMatrix { get; set; }

        [DataMember]
        public char IndBloqueio { get; set; }

        [DataMember]
        public decimal TaxaFidelizacao { get; set; }

        [DataMember]
        public DateTime DataIniFidelizacao { get; set; }

        [DataMember]
        public DateTime DataFimFidelizacao { get; set; }


        [DataMember]
        public String DescricaoBandeiraSelecionada { get; set; }

        [DataMember]
        public Decimal ValorTarifaAntecipacaoRAV { get; set; }

        [DataMember]
        public Int16 CodigoProdutoAntecipacao { get; set; }

        [DataMember]
        public String NomeProdutoAntecipacao { get; set; }

        [DataMember]
        public String DescricaoProdutoAntecipacao { get; set; }
    }
}