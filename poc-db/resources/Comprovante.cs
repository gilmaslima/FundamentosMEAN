using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Request.Modelo
{
    /// <summary>
    /// Classse Modelo para Comprovantes Pendentes e Histórico de Comprovantes
    /// </summary>
    public class Comprovante
    {
        /// <summary>
        /// Número do Processo
        /// </summary>
        public Decimal Processo { get; set; }
                
        /// <summary>
        /// Número do Resumo de Venda
        /// </summary>
        public Decimal ResumoVenda { get; set; }
        
        /// <summary>
        /// Número da Centralizadora
        /// </summary>
        public Decimal Centralizadora { get; set; }

        /// <summary>
        /// Número do Ponto de Venda / Estabelecimento
        /// </summary>
        public Decimal PontoVenda { get; set; }
        
        /// <summary>
        /// Descrição do Motivo
        /// </summary>
        public String Motivo { get; set; }

        /// <summary>
        /// Código motivo do processo
        /// </summary>
        public Int16 CodigoMotivoProcesso { get; set; }

        /// <summary>
        /// Data da Venda
        /// </summary>
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Valor da Venda
        /// </summary>
        public Decimal ValorVenda { get; set; }

        /// <summary>
        /// Data limite para envio dos documentos
        /// </summary>
        public DateTime? DataLimiteEnvioDocumentos { get; set; }

        /// <summary>
        /// Situação dos documentos atendidos
        /// </summary>
        public Boolean SolicitacaoAtendida { get; set; }

        /// <summary>
        /// Qualidade do recebimento dos documentos
        /// 1 - SEM ASSINATURA  
        /// 2 - CV DIFERE                  
        /// 3 - SEM CV                     
        /// 4 - AUTORIZACAO DIFERE         
        /// 5 - DOC EM BRANCO
        /// 6 - CV RASURADO                
        /// 7 - CV SEM DECALQUE            
        /// 8 - VALOR DIFERE               
        /// 9 - FALTA DOC
        /// 10 - DOC ILEGIVEL               
        /// 11 - DOC INVALIDO
        /// 12 - DOC OK                     
        /// </summary>
        public String QualidadeRecebimentoDocumentos { get; set; }

        /// <summary>
        /// Canal de envio dos documentos
        /// '1' = Correio
        /// '2' = Fax
        /// '3' = E-mail
        /// '4' = Bloqueado Envio
        /// </summary>              
        public Decimal? CanalEnvio { get; set; }

        /// <summary>
        /// Descrição do canal de envio dos documentos
        /// </summary>
        public String DescricaoCanalEnvio { get; set; }

        /// <summary>
        /// Data de envio dos documentos
        /// </summary>
        public DateTime? DataEnvio { get; set; }

        /// <summary>
        /// Número do Cartão ou NSU
        /// </summary>
        public String NumeroCartao { get; set; }

        /// <summary>
        /// Tipo do Cartão
        /// 'NAC' = Nacional
        /// 'INT' = Internacional
        /// </summary>        
        public String TipoCartao { get; set; }

        /// <summary>
        /// Flag NSU do cartão
        /// Se 'N', o campo contém o NSU - para as Transações Komerci
        /// Se 'C', o campo contém o cartão - para as demais transações
        /// </summary>        
        public Char? FlagNSUCartao { get; set; }

        /// <summary>
        /// Indicador de débito
        /// </summary>
        public Boolean? IndicadorDebito { get; set; }

        /// <summary>
        /// Número de referência
        /// </summary>
        public String NumeroReferencia { get; set; }
    }
}
