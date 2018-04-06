using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.RAV.Modelos;
using Redecard.PN.RAV.Dados.MA135;

namespace Redecard.PN.RAV.Dados
{
    class TEMP
    {
        #region MA135

        /// <summary>
        /// Retorna os emails cadastrados para o PDV consultado 
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVEmailSaida ConsultarEmails(int numeroPDV)
        {
            ModRAVEmailSaida saida = null;
            
            
            using (TransacaoMA135Client cliente = new TransacaoMA135Client())
            {
                MA135_ENTRADA entradaTrans = new MA135_ENTRADA();
                entradaTrans.MA135_COD_FUNCAO = "C";
                entradaTrans.MA135_NUM_PDV = numeroPDV;
                entradaTrans.MA135_NUM_SEQ = 1;
                
                MA135_SAIDA saidatrans = cliente.BMA135(entradaTrans);
                
                if (saidatrans.MA135_IND_MAIS_OCORRENCIA == "S")
                { }

                if (saidatrans.MA135_COD_RETORNO != 15)
                {
                    ModRAVEmail email = new ModRAVEmail();
                    email.Sequencia = entradaTrans.MA135_NUM_SEQ;
                    email.DataUltAlteracao = Convert.ToDateTime(saidatrans.MA135_DAT_ULT_ALTER);
                    email.Email 
                    saida.ListaEmails.Add(email);

                }

                cliente.BMA135(entradaTrans);
            }
            return saida;
        }



        /// <summary>
        /// Salva as alterações e inclusões de  emails do PDV.
        /// </summary>
        /// <param name="dadosEmail"></param>
        /// <returns></returns>
        public bool SalvarEmails(ModRAVEmailEntrada dadosEmail)
        {
            try
            {
                using (TransacaoMA135Client cliente = new TransacaoMA135Client())
                {
                    MA135_ENTRADA entrada = new MA135_ENTRADA();
                    
                    foreach (ModRAVEmail item in dadosEmail.ListaEmails)
                    {
                        if (item.Status != ModRAVEmail.EStatusEmail.None)
                        {
                            if (item.Status == ModRAVEmail.EStatusEmail.Incluso)
                                entrada.MA135_COD_FUNCAO = "I";

                            else
                                entrada.MA135_COD_FUNCAO = "A";

                            entrada.MA135_NUM_PDV = Convert.ToInt32(dadosEmail.NumeroPDV);
                            entrada.MA135_NUM_SEQ = (short)item.Sequencia;
                            entrada.MA135_IND_OPC1 = Convert.ToString(dadosEmail.IndEnviaFluxoCaixa);
                            entrada.MA135_IND_OPC2 = Convert.ToString(dadosEmail.IndEnviaValoresPV);
                            entrada.MA135_IND_OPC3 = Convert.ToString(dadosEmail.IndEnviaResumoOperacao);
                            entrada.MA135_IND_ENVO_EMAL = Convert.ToString(dadosEmail.IndEnviaEmail);
                            entrada.MA135_IND_ENVO_FAX = "N";
                            entrada.MA135_IND_SIT_EMAL_FAX = "A";
                            entrada.MA135_TXT_INFD_EMAL = item.Email;

                            cliente.BMA135(entrada);
                        }
                    }
                }
               

                return true;
            }
            catch
            {
                return false;
            }    
        }

        #endregion

    }
}
