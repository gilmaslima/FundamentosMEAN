using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Request.Agentes.COMTIXARequest;

namespace Redecard.PN.Request.Agentes
{
    public class Request : RequestBase
    {

#if DEBUG
        List<COMTIXARequest.BXA790_LINHA_REQUEST> ListaBXA790;
        List<COMTIXARequest.BXA770_LINHA_DEBITO> ListaBXA770;
        List<COMTIXARequest.BXA780_LINHA_REQUEST> ListaBXA780;

        public Request()
        {
            #region BXA790
            ListaBXA790 = new List<COMTIXARequest.BXA790_LINHA_REQUEST>();
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6220800694000", BXA790_NR_RESUMO = "26830108", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "11971606", BXA790_NR_CARTAO = "501317135", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 1", BXA790_DT_TRANSACAO = "20120601", BXA790_VL_TRANSACAO = "1936,04", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120731", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120825", BXA790_NUM_REF="000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6220901699000", BXA790_NR_RESUMO = "96560713", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "11971606", BXA790_NR_CARTAO = "3154350", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 2", BXA790_DT_TRANSACAO = "20120331", BXA790_VL_TRANSACAO = "1817,4", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120731", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120825", BXA790_NUM_REF = "000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221001160000", BXA790_NR_RESUMO = "92032042", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "11971606", BXA790_NR_CARTAO = "501484195", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 3", BXA790_DT_TRANSACAO = "20120627", BXA790_VL_TRANSACAO = "146,99", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120731", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120827", BXA790_NUM_REF = "000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221301323000", BXA790_NR_RESUMO = "3866310", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "31832091", BXA790_NR_CARTAO = "5396140735503110", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 4", BXA790_DT_TRANSACAO = "20120627", BXA790_VL_TRANSACAO = "1366,12", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120802", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120827", BXA790_NUM_REF = "000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221301480000", BXA790_NR_RESUMO = "2510969", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "31832296", BXA790_NR_CARTAO = "5278879010680790", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 5", BXA790_DT_TRANSACAO = "20120120", BXA790_VL_TRANSACAO = "3427,65", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120802", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120827", BXA790_NUM_REF = "000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221301486000", BXA790_NR_RESUMO = "96860031", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "501649928", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 6", BXA790_DT_TRANSACAO = "20120711", BXA790_VL_TRANSACAO = "455,14", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120802", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "3221600840000", BXA790_NR_RESUMO = "37849360", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "37159488", BXA790_NR_CARTAO = "2040896", BXA790_TP_CARTAO = "INT", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 7", BXA790_DT_TRANSACAO = "20120725", BXA790_VL_TRANSACAO = "267,57", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF = "000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601045000", BXA790_NR_RESUMO = "96660738", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "749350673", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 8", BXA790_DT_TRANSACAO = "20120511", BXA790_VL_TRANSACAO = "1463,67", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601453000", BXA790_NR_RESUMO = "81332761", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500481429", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 9", BXA790_DT_TRANSACAO = "20120724", BXA790_VL_TRANSACAO = "227,8", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601564000", BXA790_NR_RESUMO = "67929859", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "748867750", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 10", BXA790_DT_TRANSACAO = "20120723", BXA790_VL_TRANSACAO = "369,42", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601603000", BXA790_NR_RESUMO = "67929859", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "749561934", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 11", BXA790_DT_TRANSACAO = "20120723", BXA790_VL_TRANSACAO = "56,95", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601604000", BXA790_NR_RESUMO = "68131047", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500627675", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 12", BXA790_DT_TRANSACAO = "20120604", BXA790_VL_TRANSACAO = "61,57", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601605000", BXA790_NR_RESUMO = "75931357", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500767900", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 13", BXA790_DT_TRANSACAO = "20120710", BXA790_VL_TRANSACAO = "380,57", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601606000", BXA790_NR_RESUMO = "75931357", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500815862", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 14", BXA790_DT_TRANSACAO = "20120710", BXA790_VL_TRANSACAO = "570,47", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221601843000", BXA790_NR_RESUMO = "37849361", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "37159488", BXA790_NR_CARTAO = "2040629", BXA790_TP_CARTAO = "INT", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 15", BXA790_DT_TRANSACAO = "20120725", BXA790_VL_TRANSACAO = "1300,52", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221602415000", BXA790_NR_RESUMO = "96760839", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "501088163", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 16", BXA790_DT_TRANSACAO = "20120701", BXA790_VL_TRANSACAO = "907,84", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221602417000", BXA790_NR_RESUMO = "96760744", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "501504553", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 17", BXA790_DT_TRANSACAO = "20120626", BXA790_VL_TRANSACAO = "535,27", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823" , BXA790_NUM_REF="000012" });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221602422000", BXA790_NR_RESUMO = "96760286", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500840065", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 18", BXA790_DT_TRANSACAO = "20120604", BXA790_VL_TRANSACAO = "1196,76", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221602425000", BXA790_NR_RESUMO = "96860093", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "748315251", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 19", BXA790_DT_TRANSACAO = "20120714", BXA790_VL_TRANSACAO = "508,87", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "20120823", BXA790_NUM_REF="000012"  });
            ListaBXA790.Add(new COMTIXARequest.BXA790_LINHA_REQUEST { BXA790_PROCESSO_S = "6221602426000", BXA790_NR_RESUMO = "96860073", BXA790_CENTRAL = "1250191", BXA790_NUM_PDV = "10990577", BXA790_NR_CARTAO = "500016110", BXA790_TP_CARTAO = "NAC", BXA790_FLAG_NSU_CARTAO = "N", BXA790_DESC_MOTIVO = "Motivo 20", BXA790_DT_TRANSACAO = "20120713", BXA790_VL_TRANSACAO = "3850,2", BXA790_CANAL_ENVIO = "1", BXA790_DT_ENVIO = "20120803", BXA790_SOL_ATENDIDA = "NÃO", BXA790_QUAL_RECEB = "AUTORIZACAO DIFERE", BXA790_DT_LIMITE_ENVO = "" , BXA790_NUM_REF="000012" });
            #endregion
            #region BXA770
            ListaBXA770 = new List<COMTIXARequest.BXA770_LINHA_DEBITO>();
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222803265997"), BXA770_NR_RESUMO = Convert.ToInt32("96860031"), BXA770_NR_CARTAO = "500017490", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120711"), BXA770_VL_TRANSACAO = Convert.ToDecimal("471,69"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("199,48"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222803264997"), BXA770_NR_RESUMO = Convert.ToInt32("96860031"), BXA770_NR_CARTAO = "500017096", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120711"), BXA770_VL_TRANSACAO = Convert.ToDecimal("771,77"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("332,47"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222803252997"), BXA770_NR_RESUMO = Convert.ToInt32("96760448"), BXA770_NR_CARTAO = "748273033", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120612"), BXA770_VL_TRANSACAO = Convert.ToDecimal("293,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120817"), BXA770_VL_DEBITO = Convert.ToDecimal("75,4"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222703119997"), BXA770_NR_RESUMO = Convert.ToInt32("96860093"), BXA770_NR_CARTAO = "748293616", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120714"), BXA770_VL_TRANSACAO = Convert.ToDecimal("508,87"), BXA770_DT_DEBITO = Convert.ToInt32("20120820"), BXA770_VL_DEBITO = Convert.ToDecimal("215,96"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222403091997"), BXA770_NR_RESUMO = Convert.ToInt32("96760856"), BXA770_NR_CARTAO = "500053444", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120702"), BXA770_VL_TRANSACAO = Convert.ToDecimal("240,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120817"), BXA770_VL_DEBITO = Convert.ToDecimal("87,26"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222403090997"), BXA770_NR_RESUMO = Convert.ToInt32("96760856"), BXA770_NR_CARTAO = "500054598", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120702"), BXA770_VL_TRANSACAO = Convert.ToDecimal("240,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120817"), BXA770_VL_DEBITO = Convert.ToDecimal("87,26"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6222305433997"), BXA770_NR_RESUMO = Convert.ToInt32("96760370"), BXA770_NR_CARTAO = "502220838", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120608"), BXA770_VL_TRANSACAO = Convert.ToDecimal("200,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("45,17"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221600468997"), BXA770_NR_RESUMO = Convert.ToInt32("11949697"), BXA770_NR_CARTAO = "750730782", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("28130456"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120403"), BXA770_VL_TRANSACAO = Convert.ToDecimal("2705"),   BXA770_DT_DEBITO = Convert.ToInt32("20120806"), BXA770_VL_DEBITO = Convert.ToDecimal("527,48"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221400703997"), BXA770_NR_RESUMO = Convert.ToInt32("96660679"), BXA770_NR_CARTAO = "748612284", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120509"), BXA770_VL_TRANSACAO = Convert.ToDecimal("890,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("868,31"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402742997"), BXA770_NR_RESUMO = Convert.ToInt32("96760874"), BXA770_NR_CARTAO = "501540063", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120703"), BXA770_VL_TRANSACAO = Convert.ToDecimal("864,27"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("842,67"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402743997"), BXA770_NR_RESUMO = Convert.ToInt32("96860073"), BXA770_NR_CARTAO = "748979805", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120713"), BXA770_VL_TRANSACAO = Convert.ToDecimal("1226,07"),BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("1195,42"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "N", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402683997"), BXA770_NR_RESUMO = Convert.ToInt32("96760330"), BXA770_NR_CARTAO = "502259127", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120606"), BXA770_VL_TRANSACAO = Convert.ToDecimal("278,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("271,61"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "N", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402731997"), BXA770_NR_RESUMO = Convert.ToInt32("96760681"), BXA770_NR_CARTAO = "502361753", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120623"), BXA770_VL_TRANSACAO = Convert.ToDecimal("669,47"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("652,74"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "N", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402735997"), BXA770_NR_RESUMO = Convert.ToInt32("96760701"), BXA770_NR_CARTAO = "748442085", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120624"), BXA770_VL_TRANSACAO = Convert.ToDecimal("701,37"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("683,84"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402733997"), BXA770_NR_RESUMO = Convert.ToInt32("96760701"), BXA770_NR_CARTAO = "748463598", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120624"), BXA770_VL_TRANSACAO = Convert.ToDecimal("770,67"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("751,41"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402737997"), BXA770_NR_RESUMO = Convert.ToInt32("96760681"), BXA770_NR_CARTAO = "501795130", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120623"), BXA770_VL_TRANSACAO = Convert.ToDecimal("452,57"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("441,26"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402686997"), BXA770_NR_RESUMO = Convert.ToInt32("96760618"), BXA770_NR_CARTAO = "501101211", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120620"), BXA770_VL_TRANSACAO = Convert.ToDecimal("997,27"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("972,34"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "S" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221401306997"), BXA770_NR_RESUMO = Convert.ToInt32("90932919"), BXA770_NR_CARTAO = "502083639", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120620"), BXA770_VL_TRANSACAO = Convert.ToDecimal("56,95"),  BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("55,53"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "S", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221402687997"), BXA770_NR_RESUMO = Convert.ToInt32("96760989"), BXA770_NR_CARTAO = "501656139", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120709"), BXA770_VL_TRANSACAO = Convert.ToDecimal("1433,32"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO =Convert.ToDecimal( "1308,77"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "N" });
            ListaBXA770.Add(new COMTIXARequest.BXA770_LINHA_DEBITO { BXA770_PROCESSO = Convert.ToDecimal("6221401307997"), BXA770_NR_RESUMO = Convert.ToInt32("64929276"), BXA770_NR_CARTAO = "748644987", BXA770_TP_CARTAO = "NAC", BXA770_CENTRAL = Convert.ToInt32("1250191"), BXA770_NUM_PDV = Convert.ToInt32("10990577"), BXA770_DT_TRANSACAO = Convert.ToInt32("20120716"), BXA770_VL_TRANSACAO = Convert.ToDecimal("40"), BXA770_DT_DEBITO = Convert.ToInt32("20120821"), BXA770_VL_DEBITO = Convert.ToDecimal("39"), BXA770_MOT_DEBITO = 1, BXA770_IND_PARC = "S", BXA770_FLAG_NSU_CARTAO = "N", BXA770_IND_REQ = "S" });
            #endregion
            #region BXA780
            ListaBXA780 = new List<COMTIXARequest.BXA780_LINHA_REQUEST>();
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120731", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120711", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "500017490", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96860031", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222803265997", BXA780_QUAL_RECEB = "1", BXA780_VL_TRANSACAO = "471,69" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120731", BXA780_SOL_ATENDIDA = "N", BXA780_IND_DEB = "N", BXA780_DT_TRANSACAO = "20120711", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "500017096", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96860031", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222803264997", BXA780_QUAL_RECEB = "2", BXA780_VL_TRANSACAO = "771,77" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120731", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120612", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "748273033", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760448", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222803252997", BXA780_QUAL_RECEB = "3", BXA780_VL_TRANSACAO = "293,57" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120802", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120714", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "748293616", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96860093", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222703119997", BXA780_QUAL_RECEB = "2", BXA780_VL_TRANSACAO = "508,87" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120802", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120702", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "500053444", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760856", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222403091997", BXA780_QUAL_RECEB = "3", BXA780_VL_TRANSACAO = "240,57" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120802", BXA780_SOL_ATENDIDA = "N", BXA780_IND_DEB = "N", BXA780_DT_TRANSACAO = "20120702", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "500054598", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760856", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222403090997", BXA780_QUAL_RECEB = "4", BXA780_VL_TRANSACAO = "240,57" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120608", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "502220838", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760370", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6222305433997", BXA780_QUAL_RECEB = "5", BXA780_VL_TRANSACAO = "200,57" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120403", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "750730782", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "11949697", BXA780_NUM_PDV = "28130456", BXA780_PROCESSO_S = "6221600468997", BXA780_QUAL_RECEB = "4", BXA780_VL_TRANSACAO = "2705" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120509", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "748612284", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96660679", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6221400703997", BXA780_QUAL_RECEB = "3", BXA780_VL_TRANSACAO = "890,57" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "N", BXA780_IND_DEB = "N", BXA780_DT_TRANSACAO = "20120703", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "501540063", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760874", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6221402742997", BXA780_QUAL_RECEB = "5", BXA780_VL_TRANSACAO = "864,27" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120509", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "748979805", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96860073", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6221402743997", BXA780_QUAL_RECEB = "2", BXA780_VL_TRANSACAO = "1226,07" });
            ListaBXA780.Add(new COMTIXARequest.BXA780_LINHA_REQUEST { BXA780_CANAL_ENVIO = "3", BXA780_CENTRAL = "1250191", BXA780_DESC_MOTIVO = "Motivo teste", BXA780_DT_ENVIO = "20120803", BXA780_SOL_ATENDIDA = "S", BXA780_IND_DEB = "S", BXA780_DT_TRANSACAO = "20120703", BXA780_FLAG_NSU_CARTAO = "N", BXA780_NR_CARTAO = "502259127", BXA780_TP_CARTAO = "NAC", BXA780_NR_RESUMO = "96760330", BXA780_NUM_PDV = "10990577", BXA780_PROCESSO_S = "6221402683997", BXA780_QUAL_RECEB = "3", BXA780_VL_TRANSACAO = "278,57" });
            #endregion
        }
#endif

        public List<Modelo.Comprovante> ConsultarRequestPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            String transacao,
            ref Int16 flTemReg,
            ref Decimal codUltimoProcesso,
            ref Int16 qtdLinhasOcorrencia,
            ref Int32 qtdTotalOcorrencias,
            ref String filler,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Comprovantes Pendentes - Crédito [BXA790/XA790/IS68]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { 
                    codEstabelecimento, codProcesso, origem, transacao, flTemReg,
                    codUltimoProcesso, qtdLinhasOcorrencia, qtdTotalOcorrencias, filler });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA790_LINHA_REQUEST[] requests;
                    COMTIXARequest.BXA790_MSG_RETORNO retorno;

                    //Executa consulta de comprovantes pendentes                
#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random();
                    retorno = new COMTIXARequest.BXA790_MSG_RETORNO
                    {
                        BXA790_COD_RETORNO = 0,
                        BXA790_DESC_RETORNO = ""
                    };
                    qtdLinhasOcorrencia = (Int16)random.Next(1, 150);
                    qtdTotalOcorrencias = ListaBXA790.Count();
                    filler = "";
                    if (codUltimoProcesso == 0) codUltimoProcesso = 10000000001;
                    flTemReg = (Int16)(codProcesso > 0 ? 1 : 0);

                    String[] tpCartao = new String[] { "NAC", "INT" };
                    String[] flgNSUCartao = new String[] { "C", "N" };
                    String[] simNao = new String[] { "S", "N" };

                    if (codProcesso > 0)
                    {
                        ListaBXA790 = ListaBXA790.FindAll(delegate(COMTIXARequest.BXA790_LINHA_REQUEST item) { return item.BXA790_PROCESSO_S == codProcesso.ToString(); });
                    }
                    requests = ListaBXA790.ToArray();
                    codUltimoProcesso = codUltimoProcesso + qtdLinhasOcorrencia;
#else
                //Instancia o serviço de acesso ao Mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();

                //Executa chamada do serviço
                client.ConsultarRequestPendente(
                    ref codEstabelecimento,
                    ref codProcesso,
                    ref origem,
                    ref transacao,
                    out retorno,
                    ref flTemReg,
                    ref codUltimoProcesso,
                    ref qtdLinhasOcorrencia,
                    ref qtdTotalOcorrencias,
                    out requests,
                    ref filler);
#endif                    
                    //Converte mensagem de retorno para Modelo
                    codigoRetorno = retorno.BXA790_COD_RETORNO;

                    //Retorna comprovantes pendentes, convertendo itens para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(requests).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new {
                        flTemReg, codUltimoProcesso, qtdLinhasOcorrencia, qtdTotalOcorrencias, filler, requests, retorno, comprovantes });

                    return comprovantes;
                }
                catch (Exception ex)
                {                    
                    Log.GravarErro(ex);                    
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public Int32 ConsultarCanal(
            Int32 codEstabelecimento,
            String origem,
            ref Int16 codigoCanal,
            ref String descricaoCanal,
            ref Int64 codigoOcorrencia)
        {
            using (Logger Log = Logger.IniciarLog("Como ser avisado - Consulta Canal [BXA380/XA380/IS63]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, origem, codigoCanal, descricaoCanal, codigoOcorrencia });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA380_MSG_RETORNO msgRetorno;

                    //Efetua consulta do canal, retornando objeto contendo mensagem de retorno
#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    msgRetorno = new COMTIXARequest.BXA380_MSG_RETORNO
                    {
                        BXA380_COD_RETORNO = 0,
                        BXA380_DESC_RETORNO = "OK"
                    };

                    Int16[] canais = new Int16[] { 1, 6, 7, 8 };

                    Random random = new Random();
                    codigoCanal = canais[random.Next(0, 4)];
                    descricaoCanal = "Descrição Canal " + codigoCanal;
#else
                //Instancia o serviço de acesso ao mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();
                msgRetorno = client.ConsultarCanal(
                    out codigoCanal, out descricaoCanal, codEstabelecimento, origem);
#endif
                    Log.GravarLog(EventoLog.FimAgente, new { codigoCanal, descricaoCanal, codigoOcorrencia, msgRetorno });

                    //Retornando o código de retorno
                    return msgRetorno.BXA380_COD_RETORNO;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.Comprovante> HistoricoRequest(
            Int32 codEstabelecimento,
            DateTime dataIni,
            DateTime dataFim,
            Decimal codProcesso,
            String origem,
            String transacao,
            ref Int16 temReg,
            ref Decimal ultimoProcesso,
            ref Int16 qtdOcorrencias,
            ref String filler,
            ref Int64 CO,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Histórico de Comprovantes - Crédito [BXA780/XA780/IS39]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new {
                    codEstabelecimento, dataIni, dataFim, codProcesso, origem,
                    transacao, temReg, ultimoProcesso, qtdOcorrencias, filler, CO });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA780_LINHA_REQUEST[] requests;
                    COMTIXARequest.BXA780_MSG_RETORNO retorno;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random();
                    retorno = new COMTIXARequest.BXA780_MSG_RETORNO
                    {
                        BXA780_COD_RETORNO = 0,
                        BXA780_DESC_RETORNO = ""
                    };

                    ultimoProcesso = 0;
                    if (codProcesso > 0)
                    {
                        ListaBXA780 = ListaBXA780.FindAll(delegate(COMTIXARequest.BXA780_LINHA_REQUEST item) { return item.BXA780_PROCESSO_S == codProcesso.ToString(); });
                    }
                    qtdOcorrencias = ListaBXA780.Count.ToString().ToInt16();// new Int16[] { (Int16)random.Next(190), 190 }[random.Next(2)];
                    temReg = (Int16)(qtdOcorrencias == 190 ? 1 : 0);
                    filler = "";
                    requests = ListaBXA780.ToArray();
#else
                //Instancia o serviço de acesso ao mainframe
                String ultProcesso;
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();
                retorno = client.HistoricoRequest(
                      out temReg,
                      out ultProcesso,
                      out qtdOcorrencias,
                      out requests,
                      out filler,
                      codEstabelecimento,
                      dataIni.ToString("yyyyMMdd").ToInt32(),
                      dataFim.ToString("yyyyMMdd").ToInt32(),
                      codProcesso,
                      origem,
                      transacao);
                Decimal.TryParse(ultProcesso, out ultimoProcesso);
#endif

                    //Conversão de objeto de retorno para Modelo
                    codigoRetorno = retorno.BXA780_COD_RETORNO;

                    //Retorna o histórico de comprovantes, convertendo para Modelo
                    List<Modelo.Comprovante> comprovantes = this.PreencherModelo(requests).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new {                        
                        temReg, ultimoProcesso, qtdOcorrencias, filler, CO, codigoRetorno, comprovantes, retorno, requests });

                    return comprovantes;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.RecebimentoCV> RecebimentoCV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            out Int16 flTemReg,
            out Int16 qtdOcorrencias,
            out String filler,
            String sistemaOrigem,
            String transacaoOrigem,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Recebimento de Comprovante de Vendas [BXA760/XA760/IS66]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, codProcesso, sistemaOrigem, transacaoOrigem });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA760_LINHA_REG[] itens;
                    COMTIXARequest.BXA760_RETORNO retorno;

                    //Retorno do log de recebimento de CVs
#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    flTemReg = 0;
                    qtdOcorrencias = (Int16)random.Next(1, 150);
                    filler = "";

                    itens = new COMTIXARequest.BXA760_LINHA_REG[qtdOcorrencias];
                    for (int i = 0; i < qtdOcorrencias; i++)
                    {
                        itens[i] = new COMTIXARequest.BXA760_LINHA_REG
                        {
                            BXA760_COD_RCBM_MF = (Int16)random.Next(1, 100),
                            BXA760_DAT_RCBM_MF = 2012 * 10000 + random.Next(1, 13) * 100 + random.Next(1, 29),
                            BXA760_DSC_RCBM_MF = "Descrição " + random.Next(10000).ToString()
                        };
                    }

                    retorno = new COMTIXARequest.BXA760_RETORNO
                    {
                        BXA760_COD_RETORNO = 0,
                        BXA760_MSG_RETORNO = "OK"
                    };
#else
                //Instanciação do serviço de consulta ao mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();

                //Executa chamada do serviço
                retorno = client.RecebimentoCV(
                    out flTemReg,
                    out qtdOcorrencias,
                    out itens,
                    out filler,
                    codEstabelecimento,
                    codProcesso,
                    sistemaOrigem,
                    transacaoOrigem);
#endif
                    codigoRetorno = retorno.BXA760_COD_RETORNO;

                    List<Modelo.RecebimentoCV> cv = this.PreencherModelo(itens).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { flTemReg, qtdOcorrencias, filler, codigoRetorno, cv, retorno, itens });

                    return cv;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.ParcelaRV> ComposicaoRV(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            String transacao,
            out Int16 qtdOcorrencias,
            out Decimal valorVenda,
            out Decimal valorCancelamento,
            out Int16 qtdParcelas,
            out Int16 qtdParcelasQuitadas,
            out Int16 qtdParcelasAVencer,
            out Decimal valorDeb,
            out String filler,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Composição de Resumo de Vendas [BXA740/XA740/IS69]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, codProcesso, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA740_DADOS[] itens;
                    COMTIXARequest.BXA740_MSG_RETORNO retorno;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random(DateTime.Now.Millisecond);
                    qtdOcorrencias = (Int16)random.Next(1, 150);
                    valorVenda = (Decimal)random.NextDouble() * 1000000;
                    valorCancelamento = (Decimal)random.NextDouble() * 1000000;
                    qtdParcelas = qtdOcorrencias;
                    qtdParcelasQuitadas = (Int16)(qtdOcorrencias - random.Next(qtdOcorrencias));
                    qtdParcelasAVencer = (Int16)(qtdOcorrencias - qtdParcelasQuitadas);
                    valorDeb = (Decimal)random.NextDouble() * 1000000;
                    filler = "";

                    itens = new COMTIXARequest.BXA740_DADOS[qtdOcorrencias];
                    for (int i = 0; i < qtdOcorrencias; i++)
                    {
                        itens[i] = new COMTIXARequest.BXA740_DADOS
                        {
                            BXA740_DT_CRE_PAR = 20120000 + random.Next(13) * 100 + random.Next(29),
                            BXA740_NU_PAR = (Int16)random.Next(20),
                            BXA740_VL_LIQ_RES_PAR = (Decimal)random.NextDouble() * 1000000
                        };
                    }

                    retorno = new COMTIXARequest.BXA740_MSG_RETORNO
                    {
                        BXA740_COD_RETORNO = 0,
                        BXA740_DESC_RETORNO = "OK"
                    };
#else
                //Instanciação do serviço de consulta ao mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();

                //Executa serviço de consulta
                retorno = client.ComposicaoRV(
                    out qtdOcorrencias,
                    out itens,
                    out valorVenda,
                    out valorCancelamento,
                    out qtdParcelas,
                    out qtdParcelasQuitadas,
                    out qtdParcelasAVencer,
                    out valorDeb,
                    out filler,
                    codEstabelecimento,
                    codProcesso,
                    origem,
                    transacao);
#endif

                    //Mapeia itens de retorno para modelos de negócio
                    codigoRetorno = retorno.BXA740_COD_RETORNO;

                    List<Modelo.ParcelaRV> parcelaRV = this.PreencherModelo(itens).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new {
                        qtdOcorrencias, valorVenda, valorCancelamento, qtdParcelas, qtdParcelasQuitadas, 
                        qtdParcelasAVencer, valorDeb, filler, codigoRetorno, parcelaRV, retorno, itens });

                    return parcelaRV;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public String MotivoDebito(
            Int32 codigoMotivoDebito,
            String origem,
            String transacao,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Motivo de Débito [BXA750/XA750/IS65]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codigoMotivoDebito, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    String descricaoMotivoDebito;
                    COMTIXARequest.BXA750_RETORNO retorno;

#if DEBUG
                    Random random = new Random(DateTime.Now.Millisecond);
                    descricaoMotivoDebito = string.Format(
                        "Descrição motivo débito código {0} {1} \0 \0 \0\0\0\0\0\0\0\0 asda", codigoMotivoDebito, random.Next(100000));

                    retorno = new COMTIXARequest.BXA750_RETORNO
                    {
                        BXA750_COD_RETORNO = 0,
                        BXA750_MSG_RETORNO = "OK"
                    };
#else
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();
                retorno = client.MotivoDebito(
                    out descricaoMotivoDebito,
                    codigoMotivoDebito,
                    origem,
                    transacao);
#endif

                    codigoRetorno = retorno.BXA750_COD_RETORNO;
                    descricaoMotivoDebito = descricaoMotivoDebito.Replace("\0", "");

                    Log.GravarLog(EventoLog.FimAgente, new { codigoRetorno, descricaoMotivoDebito, retorno });

                    return descricaoMotivoDebito;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Modelo.AvisoDebito> ConsultarDebitoPendente(
            Int32 codEstabelecimento,
            Decimal codProcesso,
            String origem,
            String transacao,
            out Int16 flTemReg,
            out Decimal codUltimoProcesso,
            out Int16 qtdOcorrencias,
            out String filler,
            out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Avisos de Débito - Crédito [BXA770/XA770/IS67]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, codProcesso, origem, transacao });

                try
                {
                    //Variáveis auxiliares
                    COMTIXARequest.BXA770_LINHA_DEBITO[] itens;
                    COMTIXARequest.BXA770_MSG_RETORNO retorno;

#if DEBUG
                    //População de variáveis com dados fakes, para testes
                    Random random = new Random();
                    flTemReg = 0;
                    codUltimoProcesso = 0;
                    if (codProcesso > 0)
                    {
                        ListaBXA770 = ListaBXA770.FindAll(delegate(COMTIXARequest.BXA770_LINHA_DEBITO item) { return item.BXA770_PROCESSO == codProcesso; });
                    }

                    qtdOcorrencias = ListaBXA770.Count.ToString().ToInt16();// (Int16)random.Next(1, 150);
                    filler = "";
                    itens = ListaBXA770.ToArray();
                    retorno = new COMTIXARequest.BXA770_MSG_RETORNO
                    {
                        BXA770_COD_RETORNO = 0,
                        BXA770_DESC_RETORNO = "OK"
                    };
#else
                //Instanciação do serviço de consulta ao mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();

                //Executa serviço de consulta
                retorno = client.ConsultarDebitoPendente(
                    out flTemReg,
                    out codUltimoProcesso,
                    out qtdOcorrencias,
                    out itens,
                    out filler,
                    codEstabelecimento,
                    codProcesso,
                    origem,
                    transacao);
#endif
                    //Mapeia itens de retorno para modelos de negócio
                    codigoRetorno = retorno.BXA770_COD_RETORNO;

                    List<Modelo.AvisoDebito> avisosDebito = this.PreencherModelo(itens).ToList();

                    Log.GravarLog(EventoLog.FimAgente, new { flTemReg, codUltimoProcesso, qtdOcorrencias,
                        filler, codigoRetorno, avisosDebito, retorno, itens });

                    return avisosDebito;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public Int32 AtualizarCanal(Int32 codEstabelecimento, String origem, Int16 canalRecebimento,
            out String msgRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Atualizar Canal [BXA390/XA390/IS64]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codEstabelecimento, origem, canalRecebimento });

                try
                {
                    COMTIXARequest.BXA390_MSG_RETORNO retorno;

#if DEBUG
                    retorno = new COMTIXARequest.BXA390_MSG_RETORNO
                    {
                        BXA390_DESC_RETORNO = "OK",
                        BXA390_COD_RETORNO = 0
                    };
#else
                //Instancia o serviço de acesso ao mainframe
                COMTIXARequest.COMTIXAClient client = new COMTIXARequest.COMTIXAClient();
                retorno = client.AtualizarCanal(
                      codEstabelecimento,
                      origem,
                      canalRecebimento);
#endif
                    msgRetorno = retorno.BXA390_DESC_RETORNO;

                    Log.GravarLog(EventoLog.FimAgente, new { msgRetorno, retorno });

                    return retorno.BXA390_COD_RETORNO;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }


        #region [ Preencher Modelos ]

        private List<Modelo.Comprovante> PreencherModelo(COMTIXARequest.BXA780_LINHA_REQUEST[] requests)
        {
            List<Modelo.Comprovante> retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                Modelo.Comprovante comprovante;
                foreach (COMTIXARequest.BXA780_LINHA_REQUEST request in requests)
                {
                    comprovante = new Modelo.Comprovante
                    {
                        CanalEnvio = request.BXA780_CANAL_ENVIO.ToDecimalNull(default(Decimal)).Value,
                        Centralizadora = request.BXA780_CENTRAL.ToDecimalNull(default(Decimal)).Value,
                        DataEnvio = this.ParseDate(request.BXA780_DT_ENVIO, "yyyyMMdd"),
                        DataVenda = this.ParseDate(request.BXA780_DT_TRANSACAO, "yyyyMMdd"),
                        FlagNSUCartao = request.BXA780_FLAG_NSU_CARTAO.FirstOrDefault(),
                        IndicadorDebito = "S".Equals(request.BXA780_IND_DEB, StringComparison.InvariantCultureIgnoreCase),
                        Motivo = request.BXA780_DESC_MOTIVO,
                        NumeroCartao = request.BXA780_NR_CARTAO,
                        PontoVenda = request.BXA780_NUM_PDV.ToDecimalNull(default(Decimal)).Value,
                        Processo = request.BXA780_PROCESSO_S.ToDecimalNull(default(Decimal)).Value,
                        QualidadeRecebimentoDocumentos = request.BXA780_QUAL_RECEB,
                        ResumoVenda = request.BXA780_NR_RESUMO.ToDecimalNull(default(Decimal)).Value,
                        SolicitacaoAtendida = "S".Equals(request.BXA780_SOL_ATENDIDA, StringComparison.InvariantCultureIgnoreCase),
                        TipoCartao = request.BXA780_TP_CARTAO,
                        //Dividido por 100, pois no mainframe, é do tipo PIC 9(13)V9(2) e na importação do book utilizamos String para o campo (pois permite valores nulos).
                        ValorVenda = request.BXA780_VL_TRANSACAO.ToDecimalNull(default(Decimal)).Value / 100m,
                        DataLimiteEnvioDocumentos = null
                    };
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        private List<Modelo.Comprovante> PreencherModelo(COMTIXARequest.BXA790_LINHA_REQUEST[] requests)
        {
            List<Modelo.Comprovante> retorno = new List<Modelo.Comprovante>();

            if (requests != null)
            {
                Modelo.Comprovante comprovante;

                foreach (COMTIXARequest.BXA790_LINHA_REQUEST request in requests)
                {
                    comprovante = new Modelo.Comprovante();
                    comprovante.Processo = request.BXA790_PROCESSO_S.ToDecimalNull(default(Decimal)).Value;
                    comprovante.CanalEnvio = request.BXA790_CANAL_ENVIO.ToDecimalNull(default(Decimal)).Value;
                    comprovante.Centralizadora = request.BXA790_CENTRAL.ToDecimalNull(default(Decimal)).Value;
                    comprovante.DataEnvio = this.ParseDate(request.BXA790_DT_ENVIO, "yyyyMMdd");
                    comprovante.DataLimiteEnvioDocumentos = this.ParseDate(request.BXA790_DT_LIMITE_ENVO, "yyyyMMdd");
                    comprovante.DataVenda = this.ParseDate(request.BXA790_DT_TRANSACAO, "yyyyMMdd");
                    comprovante.FlagNSUCartao = request.BXA790_FLAG_NSU_CARTAO.FirstOrDefault();
                    comprovante.Motivo = request.BXA790_DESC_MOTIVO;
                    comprovante.NumeroCartao = request.BXA790_NR_CARTAO;
                    comprovante.PontoVenda = request.BXA790_NUM_PDV.ToDecimalNull(default(Decimal)).Value;
                    comprovante.QualidadeRecebimentoDocumentos = request.BXA790_QUAL_RECEB;
                    comprovante.ResumoVenda = request.BXA790_NR_RESUMO.ToDecimalNull(default(Decimal)).Value;
                    comprovante.SolicitacaoAtendida = "S".Equals(request.BXA790_SOL_ATENDIDA, StringComparison.InvariantCultureIgnoreCase);
                    comprovante.TipoCartao = request.BXA790_TP_CARTAO;
                    //Dividido por 100, pois no mainframe, é do tipo PIC 9(13)V9(2) e na importação do book utilizamos String para o campo (pois permite valores nulos).
                    comprovante.ValorVenda = request.BXA790_VL_TRANSACAO.ToDecimalNull(default(Decimal)).Value / 100m;
                    comprovante.IndicadorDebito = null;
                    comprovante.NumeroReferencia = request.BXA790_NUM_REF;
                    retorno.Add(comprovante);
                }
            }

            return retorno;
        }

        private List<Modelo.RecebimentoCV> PreencherModelo(COMTIXARequest.BXA760_LINHA_REG[] itens)
        {
            List<Modelo.RecebimentoCV> retorno = new List<Modelo.RecebimentoCV>();

            if (itens != null)
            {
                Modelo.RecebimentoCV modelo;
                foreach (COMTIXARequest.BXA760_LINHA_REG item in itens)
                {
                    modelo = new Modelo.RecebimentoCV
                    {
                        CodigoRecebimento = item.BXA760_COD_RCBM_MF,
                        DescricaoRecebimento = item.BXA760_DSC_RCBM_MF,
                        DataRecebimento = this.ParseDate(item.BXA760_DAT_RCBM_MF, "yyyyMMdd")
                    };
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        private List<Modelo.ParcelaRV> PreencherModelo(COMTIXARequest.BXA740_DADOS[] itens)
        {
            List<Modelo.ParcelaRV> retorno = new List<Modelo.ParcelaRV>();

            if (itens != null)
            {
                Modelo.ParcelaRV modelo;
                foreach (COMTIXARequest.BXA740_DADOS item in itens)
                {
                    modelo = new Modelo.ParcelaRV();
                    modelo.DataParcela = this.ParseDate(item.BXA740_DT_CRE_PAR, "yyyyMMdd");
                    modelo.NumeroParcela = item.BXA740_NU_PAR;
                    modelo.ValorLiquido = item.BXA740_VL_LIQ_RES_PAR;
                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        private List<Modelo.AvisoDebito> PreencherModelo(COMTIXARequest.BXA770_LINHA_DEBITO[] itens)
        {
            List<Modelo.AvisoDebito> retorno = new List<Modelo.AvisoDebito>();

            if (itens != null)
            {
                Modelo.AvisoDebito modelo;

                foreach (COMTIXARequest.BXA770_LINHA_DEBITO item in itens)
                {
                    modelo = new Modelo.AvisoDebito
                    {
                        Centralizadora = item.BXA770_CENTRAL,
                        CodigoMotivoDebito = item.BXA770_MOT_DEBITO,
                        DataCancelamento = this.ParseDate(item.BXA770_DT_DEBITO, "yyyyMMdd"),
                        DataVenda = this.ParseDate(item.BXA770_DT_TRANSACAO, "yyyyMMdd"),
                        FlagNSUCartao = item.BXA770_FLAG_NSU_CARTAO.FirstOrDefault(),
                        IndicadorParcela = "S".Equals(item.BXA770_IND_PARC, StringComparison.InvariantCultureIgnoreCase),
                        IndicadorRequest = "S".Equals(item.BXA770_IND_REQ, StringComparison.InvariantCultureIgnoreCase),
                        NumeroCartao = item.BXA770_NR_CARTAO,
                        PontoVenda = item.BXA770_NUM_PDV,
                        Processo = item.BXA770_PROCESSO,
                        ResumoVenda = item.BXA770_NR_RESUMO,
                        TipoCartao = item.BXA770_TP_CARTAO,
                        ValorLiquidoCancelamento = item.BXA770_VL_DEBITO,
                        ValorVenda = item.BXA770_VL_TRANSACAO
                    };

                    retorno.Add(modelo);
                }
            }
            return retorno;
        }

        #endregion

        /// <summary>
        /// Consulta o total de requests pendentes de Crédito.
        /// Utilizado na HomePage Segmentada.        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do book:
        /// - Book BKXA0791 / Programa XA791 / Transação XAHS
        /// </remarks>
        /// <param name="numeroPv">Número do Estabelecimento</param>
        /// <returns>Quantidade de requests pendentes Crédito</returns>
        public Int32 ConsultarTotalPendentesCredito(Int32 numeroPv)
        {
            using (var Log = Logger.IniciarLog("Consultar Total Requests Pendentes - Crédito (BKXA0791/XA791/XAHS)"))
            {
                try
                {                    
                    //Variável de retorno
                    Int32 totalRequests = default(Int32);

#if DEBUG
                    totalRequests = new Random().Next(1000);
#else
                    Log.GravarLog(EventoLog.ChamadaHIS, numeroPv);
                    //Chamada mainframe
                    using (var ctx = new ContextoWCF<COMTIXAClient>())
                        ctx.Cliente.ConsultarTotalPendentes(ref numeroPv, ref totalRequests);
                    Log.GravarLog(EventoLog.RetornoHIS, totalRequests);
#endif
                    //Retorno
                    return totalRequests;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}