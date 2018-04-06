/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 18/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.FMS.Comum;
using Redecard.PN.FMS.Comum.Log;
using Redecard.PN.FMS.Modelo;
using Redecard.PN.FMS.Agente.ServicoFMS;
using Redecard.PN.FMS.Agente.Tradutores;
using System.ServiceModel;

namespace Redecard.PN.FMS.Agente
{
    /// <summary>
    /// Este componente publica a classe ServicosFMSAgStubImp, que expõe métodos para manipular os agentes
    /// </summary>
    public class ServicosFMSAgStubImp

    {
        #region CardIssuingAgentFacade Members
        /// <summary>
        /// Este método é utilizado para atualizar ocorrência de farudes e não fraudes em transação.
        /// </summary>
        /// <param name="fraudList"></param>
        /// <returns></returns>
        public int updateFraudOrNotFraudToTransaction(fraudComposite[] fraudList)
        {
            return fraudList.Length;
        }
        /// <summary>
        /// Este método é utilizado para encontrar transações analisadas e não analisadas por transação associada.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public issuerTransaction[] findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction(long transactionId, int cardIssuingAgentNumber, int entityGroup, string userLogin, int firstResult, int maxResults)
        {
            return RetornaListaTransacoes().issuerTxList;
        }
        /// <summary>
        /// Este método é utilizado para encontrar critérios de perfil por usuário logado.
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public profileCriteria findProfileCriteriaByUserLogin(int cardIssuingAgentNumber, int entityGroup, string userLogin, string user)
        {
            profileCriteria profile = new profileCriteria();
            profile.user = "user login";
            profile.alarmTypeList = new int?[2] { 1, 2 };
            profile.transactionType = new int?[1] { 1 };
            profile.authorization = new int?[1] { 1 };
            profile.startScoreRange = 234;
            profile.endScoreRange = 56;
            profile.merchantIdList = null;
            profile.valueTransactionFrom = 4.8M;
            profile.valueTransactionFromSpecified = true;
            profile.valueTransactionTo = 6.0M;
            profile.valueTransactionToSpecified = true;
            profile.classifiedBy = 2;
            profile.entryModeList = new entryMode[2];
            profile.entryModeList[0] = new entryMode() { code = "1", description = "teste1" };
            profile.entryModeList[1] = new entryMode() { code = "2", description = "teste2" };
            profile.entryModeSelectedList = new entryMode[1];
            profile.entryModeSelectedList[0] = new entryMode() { code = "1", description = "teste1" };
            profile.mccCodeSelectedList = new issuerMCC[1];
            profile.mccCodeSelectedList[0] = new issuerMCC() { code = 1, codeSpecified = true, description = "teste3" };
            profile.stateList = new string[2];
            profile.stateList[0] = "SP";
            profile.stateList[1] = "RJ";
            profile.stateSelectedList = new string[1];
            profile.stateSelectedList[0] = "SP";
            profile.rangeBinSelectedList = new issuerRangeBin[1] { new issuerRangeBin() { initialBinRange = "550000", finalBinRange = "552000" } };
       
            return profile;
        }
        /// <summary>
        /// Este método é utilizado para encontrar a lista de transações por transação associada.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public issuerTransaction[] findTransactionListByAssociatedTransaction(long transactionId, int cardIssuingAgentNumber, int entityGroup, string userLogin, int timeout)
        {
            return RetornaListaTransacoes().issuerTxList ;
        }
        /// <summary>
        /// Este método é utilizado para bloquear o cartão
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="transactionId"></param>
        /// <param name="timeout"></param>
        public void @lock(int cardIssuingAgentNumber, int entityGroup, string userLogin, long transactionId, int timeout)
        {
            return;
        }
        /// <summary>
        /// Este método é utilizado para encontrar o range bin do cartão.
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="ica"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public issuerRangeBin[] findRangeBinByCardIssuingAgent(int cardIssuingAgentNumber, int entityGroup, string userLogin, long? ica, int firstResult, int maxResults)
        {
            issuerRangeBin[] ranges = new issuerRangeBin[1];
            ranges[0] = new issuerRangeBin();
            ranges[0].initialBinRange = "500000";
            ranges[0].finalBinRange = "550000";

            return  ranges;
        }
        /// <summary>
        /// Este método é utilizado para encontrar produtividade analisada agrupada por data.
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="user"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public dateReport findAnalystProductivityGroupedByDate(int cardIssuingAgentNumber, int entityGroup, string userLogin, string user, System.DateTime beginDate, System.DateTime endDate)
        {
            //dateReport date = new dateReport();


            //productivityByDate[] productivity = new productivityByDate[1];
            //productivity[0] = new productivityByDate();
            //productivity[0].date = DateTime.Parse("01/01/2010");
            //productivity[0].dateSpecified = true;

            //recordByDate[] record = new recordByDate[1];
            //record[0] = new recordByDate();
            //record[0].userLogin = "teste0";
            //record[0].analysedCardAmount = 23;
            //record[0].fraudulentCardAmount = 40;
            //record[0].fraudulentTransactionAmount = 89;
            //record[0].fraudTotalValue = 1.3M;
            //record[0].fraudTotalValueSpecified = true;
            //record[0].notFraudulentCardAmount = 325;
            //record[0].notFraudulentTransactionAmount = 521;
            //record[0].notFraudTotalValue = 6.5M;
            //record[0].notFraudTotalValueSpecified = true;

            //productivity[0].recordByDateList = record;

            //productivity[0].totalAnalysedCardAmount = 123;
            //productivity[0].totalFraudulentTransactionAmount = 3215;
            //productivity[0].totalFraudTotalValue = 1.6M;
            //productivity[0].totalFraudTotalValueSpecified = true;
            //productivity[0].totalNotFraudulentCardAmount = 58;
            //productivity[0].totalNotFraudulentTransactionAmount = 32;
            //productivity[0].totalNotFraudTotalValue = 0.9M;
            //productivity[0].totalNotFraudTotalValueSpecified = true;

            //date.dateProductivityList = productivity;

            //date.generalTotalAnalysedCardAmount = 24;
            //date.generalTotalFraudulentCardAmount = 68;
            //date.generalTotalFraudulentTransactionAmount = 09;
            //date.generalTotalFraudTotalValue = 1.6M;
            //date.generalTotalFraudTotalValueSpecified = true;
            //date.generalTotalNotFraudulentCardAmount = 76;
            //date.generalTotalNotFraudulentTransactionAmount = 87;
            //date.generalTotalNotFraudTotalValue = 1.9M;
            //date.generalTotalNotFraudTotalValueSpecified = true;
            //date.totalRecordAmount = 98;
            //return date;
            return null;
        }
        /// <summary>
        /// Este método é utilizado para desloquear o cartão.
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="transactionId"></param>
        public void releaseLock(int cardIssuingAgentNumber, int entityGroup, string userLogin, long transactionId)
        {
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardIssuingAgentNumber"></param>
        /// <param name="entityGroup"></param>
        /// <param name="userLogin"></param>
        /// <param name="transactionStatus"></param>
        /// <param name="periodType"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public issuerTransaction[] findTransactionListByPeriodAndStatus(int cardIssuingAgentNumber, int entityGroup, string userLogin, int transactionStatus, int periodType, System.DateTime beginDate, System.DateTime endDate, int firstResult, int maxResults)
        {
            return RetornaListaTransacoes().issuerTxList;
        }

        public long countAllSuspectTransactionListByCardIssuingAgentNumberAndUserLogin(int cardIssuingAgentNumber, int entityGroup, string userLogin)
        {
            return 200;
        }

        public systemParameterComposite findSystemParameters(int cardIssuingAgentNumber, int entityGroup, string userLogin)
        {
            systemParameterComposite systemParameter = new systemParameterComposite();
            systemParameter.hasAccess = 1;
            systemParameter.intervalDaysMaxAmount = 3;
            systemParameter.retroactiveDaysMaxAmount = 9;
            systemParameter.timeoutLock = 180;

            return systemParameter;
        }

        public long countAllTransactionListByPeriodAndStatus(int cardIssuingAgentNumber, int entityGroup, string userLogin, int transactionStatus, int periodType, System.DateTime beginDate, System.DateTime endDate)
        {
            return 200;
        }

        public long countAllAnalysedAndNotAnalysedTransactionListByAssociatedTransaction(long transactionId, int cardIssuingAgentNumber, int entityGroup, string userLogin)
        {
            return 200;
        }

        public issuerMCC[] findMerchantCategoryCodeList(long? mccCode, string mccDescription, int firstResult, int maxResults)
        {
            issuerMCC[] array = new issuerMCC[1];
            array[0] = new issuerMCC();
            array[0].code = 22;
            array[0].description = "teste";
            return array;

        }

        public long countAllRangeBinByCardIssuingAgent(int cardIssuingAgentNumber, int entityGroup, string userLogin, long? ica)
        {
            return 200;
        }

        //public responseType[] findResponseTypeList(string userLogin, int cardIssuingAgentNumber, int entityGroup)
        //{

        //    responseType[] response = new responseType[1];
        //    response[0] = new responseType();

        //    response[0].responseTypeDescripion = "Response Type Description";
        //    response[0].responseTypeId = 5;
        //    response[0].name = "Falsificação";
        //    response[0].responseTypeIdSpecified = true;
        //    return response;
        //}

        public string[] findUsersByCardIssuingAgent(int cardIssuingAgentNumber, int entityGroup, string userLogin)
        {
            return  new string[] { "todos", "teste0", "teste1", "teste2", "teste3" };
        }

        public issuerTransaction[] findAnalysedTransactionListByUserLoginAndPeriod(int cardIssuingAgentNumber, int entityGroup, string userLogin, string user, System.DateTime beginDate, System.DateTime endDate, int firstResult, int maxResults)
        {
            return RetornaListaTransacoes().issuerTxList;

        }

        public long countAllAnalysedTransactionListByUserLoginAndPeriod(int cardIssuingAgentNumber, int entityGroup, string userLogin, string user, System.DateTime beginDate, System.DateTime endDate)
        {
            return 200;
        }

        public issuerTransactionComposite findTransactionListByCardAccountNumber(string cardAccountNumber, int cardIssuingAgentNumber, int entityGroup, string userLogin, int timeout)
        {
             
            return new issuerTransactionComposite()
            {
                issuerTxList = RetornaListaTransacoes().issuerTxList,
                queryStatus = 0,
                remainderTimeoutLock = 0
            };
        }

        public long countAllMerchantCategoryCodeList(long? mccCode, string mccDescription)
        {
            return 200;
        }

        public findCountComposite findSuspectTransactionListByCardIssuingAgentNumberAndUserLogin(int cardIssuingAgentNumber, int entityGroup, string userLogin, int firstResult, int maxResults)
        {
            return RetornaListaTransacoes();
        }

        public analystReport findAnalystProductivityGroupedByAnalyst(int cardIssuingAgentNumber, int entityGroup, string userLogin, string user, System.DateTime beginDate, System.DateTime endDate)
        {

            analystReport analyst = new analystReport();

            productivityByAnalyst[] productivity = new productivityByAnalyst[1];
            productivity[0] = new productivityByAnalyst();

            productivity[0].userLogin = "User Login";

            recordByAnalyst[] record = new recordByAnalyst[1];
            record[0] = new recordByAnalyst();

            record[0].date = DateTime.Parse("12/12/2009");
            record[0].dateSpecified = true;
            record[0].analysedCardAmount = 34;
            record[0].fraudulentCardAmount = 52;
            record[0].fraudulentTransactionAmount = 95;
            record[0].fraudTotalValue = 5.2M;
            record[0].fraudTotalValueSpecified = true;
            record[0].notFraudulentCardAmount = 1;
            record[0].notFraudulentTransactionAmount = 67;
            record[0].notFraudTotalValue = 2.8M;
            record[0].notFraudTotalValueSpecified = true;

            productivity[0].recordByAnalistList = record;
            productivity[0].totalAnalysedCardAmount = 09;
            productivity[0].totalFraudulentCardAmount = 698;
            productivity[0].totalFraudulentTransactionAmount = 18;
            productivity[0].totalFraudTotalValue = 6.8M;
            productivity[0].totalFraudTotalValueSpecified = false;
            productivity[0].totalNotFraudulentCardAmount = 98;
            productivity[0].totalNotFraudulentTransactionAmount = 4563;
            productivity[0].totalNotFraudTotalValue = 87.7M;
            productivity[0].totalNotFraudTotalValueSpecified = false;

            analyst.analystProductivityList = productivity;
            analyst.generalTotalAnalysedCardAmount = 675;
            analyst.generalTotalFraudulentCardAmount = 585;
            analyst.generalTotalFraudulentTransactionAmount = 098;
            analyst.generalTotalFraudTotalValue = 87.1M;
            analyst.generalTotalFraudTotalValueSpecified = true;
            analyst.generalTotalNotFraudulentCardAmount = 2;
            analyst.generalTotalNotFraudulentTransactionAmount = 9;
            analyst.generalTotalNotFraudTotalValue = 6.8M;
            analyst.generalTotalNotFraudTotalValueSpecified = true;
            analyst.totalRecordAmount = 5642;

            return analyst;

            //return null;
        }

        public string updateAnalystProfile(int cardIssuingAgentNumber, int entityGroup, string userLogin, profileCriteria profileCriteria)
        {
            string txtErro = "[090008] Conteúdo do campo profileCriteria.merchantIdList inválido: {151515} asasd asdf asdf";
            return txtErro;
        }

        #endregion

        private findCountComposite RetornaListaTransacoes()
        {
            findCountComposite findComposite = new findCountComposite();

            issuerTransaction[] issuer = new issuerTransaction[5];
            issuer[0] = new issuerTransaction();

            issuer[0].transactionId = 123;
            issuer[0].cardAccountNumber = "1111111111111111111";
            issuer[0].valueTransaction = 1.1M;
            issuer[0].valueTransactionSpecified = true;
            issuer[0].dateTransaction = DateTime.Now;
            issuer[0].analysedDate = DateTime.Now;
            issuer[0].handlingDate = DateTime.Now;
            issuer[0].analysedDateSpecified = true;
            issuer[0].dateTransactionSpecified = true;
            issuer[0].score = 1;
            issuer[0].scoreSpecified = true;
            issuer[0].mccCode = 325;
            issuer[0].mccPortDescription = "mcc port";
            issuer[0].state = "SP";
            issuer[0].cardType = 1;
            issuer[0].cardAssoc = "card assoc";
            issuer[0].autorization = 1;
            issuer[0].merchantId = 879;
            issuer[0].merchantName = "merchan name field";
            issuer[0].entryMode = "entry mode field";
            issuer[0].entryModeDesc = "mode desc field";
            issuer[0].handlingUserLogin = "handling user login";
            issuer[0].handlingDateSpecified = true;
            issuer[0].handlingComment = "handling comment field";
            issuer[0].alarmType = 0;
            issuer[0].fraudSituation = 0;
            issuer[0].lockStatus = 0;

            //responseType response = new responseType();
            //response.responseTypeDescripion = "Response Type Description";
            //response.responseTypeId = 1;

            //issuer[0].responseType = response;

            issuer[1] = new issuerTransaction();

            issuer[1].transactionId = 124;
            issuer[1].cardAccountNumber = "222222222222222";
            issuer[1].valueTransaction = 1.8M;
            issuer[1].valueTransactionSpecified = true;
            issuer[1].dateTransaction = DateTime.Now;
            issuer[1].analysedDate = DateTime.Now;
            issuer[1].handlingDate = DateTime.Now;
            issuer[1].dateTransactionSpecified = true;
            issuer[1].score = 1;
            issuer[1].scoreSpecified = true;
            issuer[1].mccCode = 325;
            issuer[1].mccPortDescription = "mcc port";
            issuer[1].state = "SP";
            issuer[1].cardType = 1;
            issuer[1].cardAssoc = "card assoc";
            issuer[1].autorization = 2;
            issuer[1].merchantId = 879;
            issuer[1].merchantName = "merchan name field";
            issuer[1].entryMode = "entry mode field";
            issuer[1].entryModeDesc = "mode desc field";
            issuer[1].handlingUserLogin = "handling user login";
            //issuer[1].handlingDate = DateTime.Parse("01/01/0001");
            issuer[1].handlingDateSpecified = true;
            issuer[1].handlingComment = "handling comment field";
            issuer[1].alarmType = 1;
            issuer[1].fraudSituation = 0;
            issuer[1].lockStatus = 1;

            //response = new responseType();
            //response.responseTypeDescripion = "Response Type Description";
            //response.responseTypeId = 1;

            //issuer[1].responseType = response;

            issuer[2] = new issuerTransaction();

            issuer[2].transactionId = 125;
            issuer[2].cardAccountNumber = "33333333333";
            issuer[2].valueTransaction = 1.8M;
            issuer[2].valueTransactionSpecified = true;
            issuer[2].dateTransaction = DateTime.Now;
            issuer[2].analysedDate = DateTime.Now;
            issuer[2].handlingDate = DateTime.Now;
            issuer[2].dateTransactionSpecified = true;
            issuer[2].score = 1;
            issuer[2].scoreSpecified = true;
            issuer[2].mccCode = 325;
            issuer[2].mccPortDescription = "mcc port";
            issuer[2].state = "RJ";
            issuer[2].cardType = 2;
            issuer[2].cardAssoc = "card assoc";
            issuer[2].autorization = 1;
            issuer[2].merchantId = 879;
            issuer[2].merchantName = "merchan name field";
            issuer[2].entryMode = "entry mode field";
            issuer[2].entryModeDesc = "mode desc field";
            issuer[2].handlingUserLogin = "handling user login";
            issuer[2].handlingDateSpecified = true;
            issuer[2].handlingComment = "handling comment field";
            issuer[2].alarmType = 2;
            issuer[2].fraudSituation = 0;
            issuer[2].lockStatus = 1;

            //response = new responseType();
            //response.responseTypeDescripion = "Response Type Description";
            //response.responseTypeId = 2;

            //issuer[2].responseType = response;


            issuer[3] = new issuerTransaction();

            issuer[3].transactionId = 124;
            issuer[3].cardAccountNumber = "444444444444444";
            issuer[3].valueTransaction = 1.8M;
            issuer[3].valueTransactionSpecified = true;
            issuer[3].dateTransaction = DateTime.Now;
            issuer[3].analysedDate = DateTime.Now;
            issuer[3].handlingDate = DateTime.Now;
            issuer[3].dateTransactionSpecified = true;
            issuer[3].score = 1;
            issuer[3].scoreSpecified = true;
            issuer[3].mccCode = 325;
            issuer[3].mccPortDescription = "mcc port";
            issuer[3].state = "SP";
            issuer[3].cardType = 1;
            issuer[3].cardAssoc = "card assoc";
            issuer[3].autorization = 2;
            issuer[3].merchantId = 879;
            issuer[3].merchantName = "merchan name field";
            issuer[3].entryMode = "entry mode field";
            issuer[3].entryModeDesc = "mode desc field";
            issuer[3].handlingUserLogin = "handling user login";
            issuer[3].handlingDateSpecified = true;
            issuer[3].handlingComment = "handling comment field";
            issuer[3].alarmType = 2;
            issuer[3].fraudSituation = 1; 
            issuer[3].lockStatus = 1;

            //response = new responseType();
            //response.responseTypeDescripion = "Response Type Description";
            //response.responseTypeId = 3;
            //issuer[3].responseType = response;


            issuer[4] = new issuerTransaction();

            issuer[4].transactionId = 124;
            issuer[4].cardAccountNumber = "5555555555555";
            issuer[4].valueTransaction = 1.8M;
            issuer[4].valueTransactionSpecified = true;
            issuer[4].dateTransaction = DateTime.Parse("15/07/2010");
            issuer[4].handlingDate = DateTime.Parse("15/07/2010");
            issuer[4].analysedDate = DateTime.Parse("15/07/2010");
            issuer[4].dateTransactionSpecified = true;
            issuer[4].score = 1;
            issuer[4].scoreSpecified = true;
            issuer[4].mccCode = 325;
            issuer[4].mccPortDescription = "mcc port";
            issuer[4].state = "SP";
            issuer[4].cardType = 1;
            issuer[4].cardAssoc = "card assoc";
            issuer[4].autorization = 2;
            issuer[4].merchantId = 879;
            issuer[4].merchantName = "merchan name field";
            issuer[4].entryMode = "entry mode field";
            issuer[4].entryModeDesc = "mode desc field";
            issuer[4].handlingUserLogin = "handling user login";
            issuer[4].handlingDateSpecified = true;
            issuer[4].handlingComment = "handling comment field";
            issuer[4].alarmType = 0;
            issuer[4].fraudSituation = 2; 
            issuer[4].lockStatus = 0;

            //response = new responseType();
            //response.responseTypeDescripion = "Response Type Description";
            //response.responseTypeId = 3;
            //issuer[4].responseType = response;

            findComposite.issuerTxList = issuer;
            findComposite.associatedCount = 100;//findComposite.issuerTxList.Length;

            findComposite.retrievedHoursAmount = 100;
            findComposite.totalHoursInPeriod = 360;

            return findComposite;
        }

       



    }
}
