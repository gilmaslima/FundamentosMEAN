/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TerminalDetalhado
    /// </summary>
    [DataContract]
    public class TerminalDetalhadoRest : TerminalRest
    {
        /// <summary>
        /// Flex
        /// </summary>
        [DataMember]
        public Boolean Flex { get; set; }

        /// <summary>
        /// Fabricante
        /// </summary>
        [DataMember]
        public String Fabricante { get; set; }

        /// <summary>
        /// Versão software
        /// </summary>
        [DataMember]
        public String VersaoSoftware { get; set; }

        /// <summary>
        /// Versão kernel
        /// </summary>
        [DataMember]
        public String VersaoKernel { get; set; }

        #region TipoConexao
        /// <summary>
        /// Tipo conexão
        /// </summary>
        [IgnoreDataMember]
        public TipoTerminalTipoConexao? TipoConexaoCodigo { get; set; }

        /// <summary>
        /// Descriçaõ do Tipo de Conexao
        /// </summary>
        [DataMember(Name = "TipoConexao", EmitDefaultValue = false)]
        public String TipoConexaoDescricao
        {
            get
            {
                return TipoConexaoCodigo != null ? TipoConexaoCodigo.ToString() : "";
            }
            set { this.TipoConexaoDescricao = value; }
        }
        #endregion
        /// <summary>
        /// Código rede
        /// </summary>
        [DataMember]
        public String CodigoRede { get; set; }

        /// <summary>
        /// Chip
        /// </summary>
        [DataMember]
        public List<Chip> Chip { get; set; }

        #region Proprietario
        /// <summary>
        /// Proprietário
        /// </summary>
        [IgnoreDataMember]
        public TipoTerminalProprietario? ProprietarioCodigo { get; set; }

        /// <summary>
        /// Descrição do Proprietario
        /// </summary>
        [DataMember(Name = "Proprietario", EmitDefaultValue = false)]
        public String ProprietarioDescricao
        {
            get
            {
                return ProprietarioCodigo != null ? ProprietarioCodigo.ToString() : "";
            }
            set { this.ProprietarioDescricao = value; }
        }

        #endregion

        /// <summary>
        /// Posição
        /// </summary>
        [DataMember]
        public String Posicao { get; set; }

        /// <summary>
        /// Número Ativo SAP
        /// </summary>
        [DataMember]
        public String NumeroAtivoSap { get; set; }

        /// <summary>
        /// Data compra
        /// </summary>
        [DataMember]
        public DateTime? DataCompra { get; set; }

        /// <summary>
        /// Data recebimento
        /// </summary>
        [DataMember]
        public DateTime? DataRecebimento { get; set; }

        /// <summary>
        /// Data instalação
        /// </summary>
        [DataMember]
        public DateTime? DataInstalacao { get; set; }

        /// <summary>
        /// Data atualização
        /// </summary>
        [DataMember]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Data atualização TG
        /// </summary>
        [DataMember]
        public DateTime? DataAtualizacaoTg { get; set; }

        /// <summary>
        /// Alterado por
        /// </summary>
        [DataMember]
        public String AlteradoPor { get; set; }

        /// <summary>
        /// Bloqueado
        /// </summary>
        [DataMember]
        public Boolean Bloqueado { get; set; }

        /// <summary>
        /// Inativo
        /// </summary>
        [DataMember]
        public Boolean Inativo { get; set; }

        /// <summary>
        /// Reintegrado
        /// </summary>
        [DataMember]
        public Boolean Reintegrado { get; set; }

        /// <summary>
        /// Carga pendente
        /// </summary>
        [DataMember]
        public Boolean CargaPendente { get; set; }

        /// <summary>
        /// Inicialização pendente
        /// </summary>
        [DataMember]
        public Boolean InicializacaoPendente { get; set; }

        /// <summary>
        /// Venda digitada
        /// </summary>
        [DataMember]
        public VendaDigitadaTerminal VendaDigitada { get; set; }

        /// <summary>
        /// Sazonal
        /// </summary>
        [DataMember]
        public Boolean Sazonal { get; set; }

        /// <summary>
        /// Placa trocada
        /// </summary>
        [DataMember]
        public Boolean PlacaTrocada { get; set; }

        /// <summary>
        /// Integrador
        /// </summary>
        [DataMember]
        public Integrador Integrador { get; set; }

        /// <summary>
        /// Caraterística
        /// </summary>
        [DataMember]
        public List<ChaveValor> Caracteristica { get; set; }
    }
}
