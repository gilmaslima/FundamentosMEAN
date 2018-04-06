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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.RAV.Servicos;

namespace Redecard.PN.RAV.Servicos
{    
    [ServiceContract]
    public interface IServicoPortalRAV
    {
        #region RAV Avulso
        /// <summary>
        /// Verifica o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAvulsoSaida VerificarRAVDisponivel(int numeroPDV);

        /// <summary>
        /// Verifica o RAV Avulso disponível de um PDV específico.
        /// Equivalente ao método VerificarRAVDisponivel, porém utiliza cacheamento dos dados.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAvulsoSaida VerificarRAVDisponivel_Cache(int numeroPDV);

        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível através da URA.
        /// </summary>
        /// <param name="numeroPDV">Número da Entidade</param>
        /// <param name="tipoCredito">Tipo da Antecipação do Crédito
        ///     <example>0: Antecipação D+0;</example>
        ///     <example>1: Antecipação D+1</example>
        /// </param>
        /// <returns>Modelo com os dados de saída do RAV Avulso</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAvulsoSaida VerificarRAVDisponivelURA(Int32 numeroPDV, short tipoCredito);

        /// <summary>
        /// Consulta o RAV Avulso disponível de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da pesquisa. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorAntecipado">O valor a ser antecipado.</param>
        /// <returns>Classe de dados de um RAV Avulso.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAvulsoSaida ConsultarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorAntecipado);

        /// <summary>
        /// Efetua o RAV Avulso de um PDV específico.
        /// </summary>
        /// <param name="entradaRAV">Os dados de entrada da efetuação. Consultar documentação para informações.</param>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <param name="tipoCredito">O tipo de crédito.</param>
        /// <param name="valorSolicitado">O valor solicitado a ser antecipado.</param>
        /// <returns>O código de retorno da transação. Consultar documentação para lista de códigos de retorno.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        int EfetuarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, int numeroPDV, int tipoCredito, decimal valorSolicitado);

        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.MA30 ExecutarMA30(Servicos.MA30 chamadaMA30);

        #endregion

        #region RAV Automático
        /// <summary>
        /// Verifica se existe o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        bool VerificarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade);

        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAutomatico ConsultarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade);
        
        /// <summary>
        /// Consulta o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAutomatico ConsultarRAVAutomaticoPersonalizado(int numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String _diaSemana, String _diaAntecipacao, String sBandeiras);

        /// <summary>
        /// Efetua o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        int EfetuarRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade, string sBandeiras);
        
        /// <summary>
        /// Efetua o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser efetuado.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Int32 EfetuarRAVAutomaticoPersonalizado(int numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String diaSemana, String diaAntecipacao, String sBandeiras);

        /// <summary>
        /// Simula o RAV Automático de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser simulado.</param>
        /// <returns>Classe de dados de um RAV Automático.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        Servicos.ModRAVAutomatico SimularRAVAutomatico(int numeroPDV, char tipoVenda, char periodicidade);
        #endregion

        #region RAV Email
        /// <summary>
        /// Salva os emails para as notificações de um RAV.
        /// </summary>
        /// <param name="dadosEmail">Classe de dados preenchida. Consultar documentação para informações.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        bool SalvarEmails(ModRAVEmailEntradaSaida dadosEmail);

        /// <summary>
        /// Consulta os emails registrados no RAV de um PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV a ser pesquisado.</param>
        /// <returns>Classe de dados com a lista de emails no RAV.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        ModRAVEmailEntradaSaida ConsultarEmails(int numeroPDV);
        #endregion

        #region RAV Senha
        /// <summary>
        /// Valida a senha de RAV de um PDV específico.
        /// </summary>
        /// <param name="senha">A senha.</param>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        bool ValidarSenha(string senha, int numeroPDV);

        /// <summary>
        /// Verifica o acesso ao RAV um número PDV específico.
        /// </summary>
        /// <param name="numeroPDV">O número do PDV.</param>
        /// <returns>True se foi realizado com sucesso, False se não foi.</returns>
        [OperationContract]
        [FaultContract(typeof(ServicoRAVException))]
        bool VerificarAcesso(int numeroPDV);
        #endregion
    }
}
