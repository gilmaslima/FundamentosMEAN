/*
(c) Copyright [2012] Redecard S.A.
Autor : [Lucas Nicoleto da Cunha]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/12 - Lucas Nicoletto da Cunha - Criação 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using Redecard.PN.Emissores.Modelos;
using Redecard.PN.Emissores.Agentes.COMTIWF;
using Redecard.PN.Emissores.Agentes.COMTIZP;
using Redecard.PN.Comum;
using System.Text;
using System.ServiceModel;

namespace Redecard.PN.Emissores.Agentes
{
    public class AgentesEmissores : AgentesBase
    {
        #region SINGLETON
        private static AgentesEmissores agentesEmissores = null;

        public AgentesEmissores()
        { }

        /// <summary>
        /// Retorna a instância do objeto.
        /// </summary>
        /// <returns></returns>
        public static AgentesEmissores GetInstance()
        {
            if (agentesEmissores == null)
            { agentesEmissores = new AgentesEmissores(); }
            return agentesEmissores;
        }
        #endregion

        #region BWF01EMI  Solicitação Tecnologia

        /// <summary>
        /// Efetiva Solicitaçao
        /// </summary>
        /// <param name="numEmissor">Número do Emissor</param>
        /// <param name="enntradaEmissao">Entrada Emissao</param>
        /// <param name="numPV">Numero do PV</param>
        /// <returns>Dados Emissao</returns>
        public bool EfetivarSolicitacao(int numEmissor, DadosEmissao entradaEmissao, out Int32 codigoRetorno, out String mensagemRetorno)
        {
            DadosEmissao saida = new DadosEmissao();
            bool retorno = false;
            try
            {
                codigoRetorno = 0;
                mensagemRetorno = string.Empty;

                Logger.IniciarLog("EfetivarSolicitacao");
                Logger.GravarLog("Parametros recebidos", new { entradaEmissao });

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(new String('X', 32));//CTRL_EMI
                stringBuilder.Append(entradaEmissao.DadosPV.TipoPessoa);//COD_TIP_PES_EMI
                stringBuilder.Append(entradaEmissao.DadosPV.Cnpj.PadLeft(15, '0'));//NUM_CGC_ESTB_EMI
                stringBuilder.Append(entradaEmissao.DadosPV.CodigoRamoAtividade.PadLeft(5, '0'));//COD_RAM_ATVD_EMI
                stringBuilder.Append(String.Join("", entradaEmissao.DadosPV.RazaoSocial.Take(27)).PadRight(27, ' '));//NOM_RZSC_ESTB_EMI

                stringBuilder.Append(new String('0', 8));//DAT_FUND_ESTB_EMI

                //Dados Bancarios
                //Crédito
                if (!object.Equals(entradaEmissao.DadosPV.DadosBancarioCredito, null))
                {
                    stringBuilder.Append(entradaEmissao.DadosPV.DadosBancarioCredito.CodBanco.ToString().PadLeft(4, '0'));//COD_BCO_EMI
                    stringBuilder.Append(entradaEmissao.DadosPV.DadosBancarioCredito.NumAgencia.ToString().PadLeft(4, '0'));//COD_AGE_EMI
                    stringBuilder.Append(FormataContaCorrente(entradaEmissao.DadosPV.DadosBancarioCredito.CodBanco, entradaEmissao.DadosPV.DadosBancarioCredito.NumContaCorrente));

                }
                else
                {
                    stringBuilder.Append(new String('0', 23));

                }
                for (int x = 0; x < 2; x++)
                {
                    //Débito
                    if (!object.Equals(entradaEmissao.DadosPV.DadosBancarioDebito, null))
                    {
                        stringBuilder.Append(entradaEmissao.DadosPV.DadosBancarioDebito.CodBanco.ToString().PadLeft(4, '0'));//COD_BCO_EMI
                        stringBuilder.Append(entradaEmissao.DadosPV.DadosBancarioDebito.NumAgencia.ToString().PadLeft(4, '0'));//COD_AGE_EMI
                        stringBuilder.Append(FormataContaCorrente(entradaEmissao.DadosPV.DadosBancarioDebito.CodBanco, entradaEmissao.DadosPV.DadosBancarioDebito.NumContaCorrente));
                    }
                    else
                    {
                        stringBuilder.Append(new String('0', 23));
                    }
                }
                //conta construcard
                stringBuilder.Append(new String('0', 23));


                string[] cep;

                //Endereço do PV
                if (!object.Equals(entradaEmissao.DadosPV.Endereco, null))
                {

                    cep = entradaEmissao.DadosPV.Endereco.Cep.Split('-');
                    stringBuilder.AppendFormat("{0,-60}", entradaEmissao.DadosPV.Endereco.Endereco);
                    stringBuilder.AppendFormat("{0,-20}", entradaEmissao.DadosPV.Endereco.Complemento);
                    stringBuilder.AppendFormat("{0,-6}", entradaEmissao.DadosPV.Endereco.NumeroEndereco);
                    stringBuilder.AppendFormat("{0,-20}", entradaEmissao.DadosPV.Endereco.Bairro);
                    stringBuilder.AppendFormat("{0,-25}", entradaEmissao.DadosPV.Endereco.Cidade);
                    stringBuilder.AppendFormat("{0,-2}", entradaEmissao.DadosPV.Endereco.Uf);
                    stringBuilder.Append(cep.Length > 0 ? cep[0].PadRight(5, ' ') : new String(' ', 5));
                    stringBuilder.Append(cep.Length > 1 ? cep[1].PadRight(3, ' ') : new String(' ', 3));
                }
                else
                {

                    stringBuilder.Append(new String(' ', 141));
                }

                if (!object.Equals(entradaEmissao.DadosPV.EnderecoEntrega, null))
                {
                    cep = entradaEmissao.DadosPV.EnderecoEntrega.Cep.Split('-');
                    stringBuilder.AppendFormat("{0,-60}", entradaEmissao.DadosPV.EnderecoEntrega.Endereco);
                    stringBuilder.AppendFormat("{0,-20}", entradaEmissao.DadosPV.EnderecoEntrega.Complemento);
                    stringBuilder.AppendFormat("{0,-6}", entradaEmissao.DadosPV.EnderecoEntrega.NumeroEndereco);
                    stringBuilder.AppendFormat("{0,-20}", entradaEmissao.DadosPV.EnderecoEntrega.Bairro);
                    stringBuilder.AppendFormat("{0,-25}", entradaEmissao.DadosPV.EnderecoEntrega.Cidade);
                    stringBuilder.AppendFormat("{0,-2}", entradaEmissao.DadosPV.EnderecoEntrega.Uf);
                    stringBuilder.Append(cep.Length > 0 ? cep[0].PadRight(5, ' ') : new String(' ', 5));//COD-CEP-EMI
                    stringBuilder.Append(cep.Length > 1 ? cep[1].PadRight(3, ' ') : new String(' ', 3));//COD-CPL-CEP-EMI 
                }
                else
                {

                    stringBuilder.Append(new String(' ', 141));
                }


                stringBuilder.Append(string.Format("{0,-30}", entradaEmissao.DadosPV.PessoaContato));//NOM_PES_CTTO_PDV_EMI 
                stringBuilder.Append(!String.IsNullOrEmpty(entradaEmissao.DadosPV.Email) ? "S" : "N");//COD_INDC_ACES_INTN_EMI 
                stringBuilder.Append(new String(' ', 50));//NOM_EML_PDV_EMI
                stringBuilder.Append(new String(' ', 50));//NOM_HMPG_PDV_EMI 

                if (!object.Equals(entradaEmissao.DadosPV.Telefone, null))
                {
                    stringBuilder.AppendFormat("{0,-4}", entradaEmissao.DadosPV.Telefone.DDD);//NUM_DDD_PDV_EMI 
                    stringBuilder.Append(entradaEmissao.DadosPV.Telefone.Telefone.ToDecimalNull(0).Value.ToString().PadLeft(10, '0'));//NUM_TEL_PDV_EMI 
                    stringBuilder.Append(new String('0', 4));//entradaEmissao.DadosPV.Telefone.Ramal.ToInt16Null(0).Value.ToString().PadLeft(4, '0'));//NUM_RML_PDV_EMI 
                }
                //if (!object.Equals(entradaEmissao.DadosPV.Fax, null))
                //{
                stringBuilder.Append(new String(' ', 4));//"{0,-4}", entradaEmissao.DadosPV.Fax.DDD);//NUM_DDD_FAX_PDV_EMI 
                stringBuilder.Append(new String('0', 10));//entradaEmissao.DadosPV.Fax.Telefone.ToDecimalNull(0).Value.ToString().PadLeft(10, '0'));//NUM_FAX_PDV_EMI 
                //}

                //if (!object.Equals(entradaEmissao.DadosPV.Telefone2, null))
                //{
                stringBuilder.Append(new String(' ', 4));//Format("{0,-4}", entradaEmissao.DadosPV.Telefone2.DDD.PadRight(4, ' '));//NUM_DDD_PDV_2_EMI 
                stringBuilder.Append(new String('0', 10));//entradaEmissao.DadosPV.Telefone2.Telefone.ToDecimalNull(0).Value.ToString().PadLeft(10, '0'));//NUM_TEL_PDV_2_EMI 
                stringBuilder.Append(new String('0', 4));//entradaEmissao.DadosPV.Telefone2.Ramal.ToInt16Null(0).Value.ToString().PadLeft(4, '0'));//NUM_RML_PDV_2_EMI 
                //}

                stringBuilder.Append(new String(' ', 11));//NOM_PLQT_1_PDV_EMI 
                stringBuilder.AppendFormat("{0,-11}", entradaEmissao.DadosPV.NomePlaqueta2);//NOM_PLQT_2_PDV_EMI 
                stringBuilder.Append(new String('0', 4));//COD_FIL_EMI 
                stringBuilder.Append("V");//COD_GRNC_EMI 
                stringBuilder.Append(new String('0', 3));//COD_CART_EMI 


                //Zona

                stringBuilder.Append(new String('0', 4));//COD_ZONAX_EMI 

                stringBuilder.Append(new String('0', 4));//COD_NCLO_EMI 

                stringBuilder.Append("0");//COD_HRRO_FNCN_PDV_EMI 
                stringBuilder.Append("N");//COD_INDC_ENVO_MQNT_EMI 
                stringBuilder.Append(new String('0', 3));//QTD_MQNT_EMI 
                stringBuilder.AppendFormat("{0,-1}", entradaEmissao.DadosPV.TipoEstabelecimento);//COD_TIP_ESTB_PDV_EMI 

                stringBuilder.Append("S");//IND_CMRC_NORM_EMI 
                stringBuilder.Append("N");//IND_CMRC_CTLG_EMI 
                stringBuilder.Append("N");//IND_CMRC_TEL_EMI 
                stringBuilder.Append("N");//IND_CMRC_ELR_EMI 
                stringBuilder.Append(new String('0', 9));//R2_NUM_PDV_EMI 
                stringBuilder.Append(String.Join("", entradaEmissao.DadosPV.NomeComercial.Take(23)).PadRight(23, ' '));//NOM_FAT_PDV_EMI 

                stringBuilder.Append(new String('0', 9));//NUM_GRU_CMC_EMI 
                stringBuilder.Append(new String('0', 9));//NUM_GRU_EMI 
                stringBuilder.Append("0");//COD_TIP_CNG_PDV_EMI 
                stringBuilder.Append(new String('0', 9));//R3_NUM_PDV_EMI 

                stringBuilder.Append("N");//IND_IATA_EMI 

                if (entradaEmissao.DadosPV.TipoEstabelecimento.ToInt16() == 0)
                {
                    stringBuilder.Append("1");//COD_LOCL_PGMN_EMI 
                    stringBuilder.Append(new String('0', 9));//R1_NUM_PDV_EMI 
                    //R1_NUM_PDV_EMI = 0;
                }
                else if (entradaEmissao.DadosPV.TipoEstabelecimento.ToInt16() == 1 || entradaEmissao.DadosPV.TipoEstabelecimento.ToInt16() == 2)
                {
                    stringBuilder.Append("2");//COD_LOCL_PGMN_EMI 

                    stringBuilder.Append(entradaEmissao.DadosPV.CodigoCentral.ToString().PadLeft(9, '0'));//R1_NUM_PDV_EMI 
                }


                //Produto
                stringBuilder.Append(new String('0', 72));

                stringBuilder.Append("S");//IND_SLC_TCNL_EMI 
                stringBuilder.AppendFormat("{0,-3}", entradaEmissao.CodTipoEquipamento);// COD_TIP_EQPM_EMI 
                stringBuilder.Append(entradaEmissao.QtdeEquipamento.ToString().PadLeft(3, '0'));//QTD_TERM_SLC_EMI 
                stringBuilder.Append("1");//COD_PRPD_POS_EMI 
                stringBuilder.Append("2");//COD_TIP_LGCO_EMI 
                stringBuilder.Append("N");//IND_HBTC_DGTC_EMI 
                stringBuilder.Append("N");//IND_HBTC_CRGA_PRPV_EMI 
                stringBuilder.Append(new String(' ', 60));//NOM_ENDR_TCNL_EMI 
                stringBuilder.Append(new String(' ', 20));//NOM_CPL_ENDR_TCNL_EMI 
                stringBuilder.Append(new String(' ', 6));//NUM_END_TCNL_EMI 
                stringBuilder.Append(new String(' ', 20));//NOM_BRR_TCNL_EMI 
                stringBuilder.Append(new String(' ', 25));//NOM_CID_TCNL_EMI 
                stringBuilder.Append(new String(' ', 2));//NOM_EST_TCNL_EMI 

                stringBuilder.Append(new String(' ', 5));//COD_CEP_TCNL_EMI 
                stringBuilder.Append(new String(' ', 3));//COD_CPL_CEP_TCNL_EMI 
                stringBuilder.Append(new String(' ', 30));//NOM_PES_CTTO_TCNL_EMI 
                stringBuilder.Append(new String(' ', 4));//NUM_DDD_TCNL_EMI 
                //175

                String NUM_TEL_TCNL_EMI = new String('0', 10);

                stringBuilder.Append(new String('0', 4));//NUM_RAM_TCNL_EMI 
                stringBuilder.Append(new String(' ', 8));//HOR_FNCN_INI_EMI 
                stringBuilder.Append(new String(' ', 8));//HOR_FNCN_FIM_EMI 
                stringBuilder.Append(new String('0', 3));//COD_RGME_TCNL_EMI 
                stringBuilder.Append(new String('0', 4));//COD_CNTR_CUST_TCNL_EMI 
                stringBuilder.Append((entradaEmissao.ValorEquipamento * 100).ToString().PadLeft(12, '0'));//VAL_PRCO_EQPM_EMI 
                stringBuilder.Append(new String(' ', 255));//DES_OBSR_EMI 
                stringBuilder.Append(" ");//IND_ACEI_VISA_EMI 
                stringBuilder.Append(" ");//IND_ACEI_AMEX_EMI 
                stringBuilder.Append(" ");//IND_ACEI_OUTR_EMI 

                //Dados do Proprietario
                if (!object.Equals(entradaEmissao.DadosPV.ListaProprietarios, null) && entradaEmissao.DadosPV.ListaProprietarios.Count > 0)
                {
                    stringBuilder.AppendFormat("{0,-30}", entradaEmissao.DadosPV.ListaProprietarios[0].Nome);
                    stringBuilder.AppendFormat((entradaEmissao.DadosPV.ListaProprietarios[0].DataNascimento != null && entradaEmissao.DadosPV.ListaProprietarios[0].DataNascimento != DateTime.MinValue) ? entradaEmissao.DadosPV.ListaProprietarios[0].DataNascimento.ToString("yyyyMMdd") : new String('0', 8));
                    stringBuilder.Append(entradaEmissao.DadosPV.ListaProprietarios[0].CPF.ToString().PadLeft(15, '0'));
                    stringBuilder.Append(new String('0', 5));

                    for (int cnt = 0; cnt < 4; cnt++)
                    {
                        stringBuilder.Append(new String('0', 8));
                        stringBuilder.Append(new String(' ', 30));
                        stringBuilder.Append(new String('0', 15));
                        stringBuilder.Append(new String('0', 5));
                    }
                }
                else
                {
                    for (int cnt = 0; cnt < 5; cnt++)
                    {
                        stringBuilder.Append(new String('0', 8));
                        stringBuilder.Append(new String(' ', 30));
                        stringBuilder.Append(new String('0', 15));
                        stringBuilder.Append(new String('0', 5));

                    }
                }


                stringBuilder.Append("01");//COD_CNL_EMI 
                stringBuilder.Append(numEmissor.ToString().PadLeft(5, '0'));//COD_CEL_EMI 
                stringBuilder.Append(new String('0', 4));//COD_RTRO_EMI 
                stringBuilder.Append(entradaEmissao.CodAgenciaFilia.ToString().PadLeft(4, '0'));//COD_AGE_CNL_FLCO_EMI 
                stringBuilder.Append(entradaEmissao.CPFVendedor.ToString().PadLeft(15, '0'));//COD_CPF_VDD_PDV_EMI 
                stringBuilder.Append(new String('0', 3));//COD_TRCR_VSTA_EMI 
                stringBuilder.Append(DateTime.Now.ToString("yyyyMMdd"));//DAT_CDST_PROP_EMI 
                stringBuilder.Append(new String('0', 9));//NUM_SLC_EMI 
                stringBuilder.Append(new String('0', 8));//DAT_ABTA_SLC_EMI 
                stringBuilder.Append(new String(' ', 1));//INDC_SIT_PROP_EMI 
                stringBuilder.Append("T");//COD_TIP_MOV_EMI 
                stringBuilder.Append(new String(' ', 1));//COD_FASE_FLCO_EMI 
                stringBuilder.Append(new String(' ', 2));//COD_FBRC_HDW_EMI 
                stringBuilder.AppendFormat("{0,-2}", entradaEmissao.CodIntegrador.ToString());//COD_FORN_SFTW_EMI 
                stringBuilder.Append(new String('0', 15));//COD_NUM_RENPAC_EMI 
                stringBuilder.Append(new String(' ', 1));//IND_LOC_SHOP_EMI 

                if (!object.Equals(entradaEmissao.DadosPV.ListaProprietarios, null))
                {
                    foreach (DadosProprietario item in entradaEmissao.DadosPV.ListaProprietarios)
                    {
                        stringBuilder.AppendFormat("{0,-1}", item.TipoPessoa);
                    }

                    for (int cnt = 0; cnt < (5 - entradaEmissao.DadosPV.ListaProprietarios.Count); cnt++)
                    {
                        stringBuilder.AppendFormat(new String(' ', 1));
                    }
                }
                else
                {
                    for (int cnt = 0; cnt < 5; cnt++)
                    {
                        stringBuilder.AppendFormat(new String(' ', 1));
                    }
                }
                stringBuilder.AppendFormat(entradaEmissao.DadosPV.Codigo.ToString().PadLeft(9, '0'));//NUM_PDV_EMI 
                stringBuilder.AppendFormat("N");//INDC_PROD_TAII_EMI 
                stringBuilder.AppendFormat(new String(' ', 1));//FILLER
                stringBuilder.AppendFormat(new String('0', 5));//COD_CNRO_EMI 
                stringBuilder.AppendFormat(new String('0', 5));//cod-rde 
                stringBuilder.AppendFormat(new String('0', 5));//cod-tip-cnxo
                stringBuilder.AppendFormat(new String(' ', 81));//FILLER
                String envio = stringBuilder.ToString();
                Logger.GravarLog("Chamada ao Método HIS BWF01", new
                {
                    envio
                });

                //Efetua a Solicitação
                using (var context = new ContextoWCF<COMTIWFClient>())
                {
                    context.Cliente.BWF01(
                        ref envio
                        );

                    Logger.GravarLog("Retorno método HIS BWF01", new { envio });
                    if (!String.IsNullOrEmpty(envio) && envio.Length > 0)
                    {
                        codigoRetorno = String.Join("", envio.Skip(1).Take(2)).ToInt32Null(0).Value;
                        mensagemRetorno = String.Join("", envio.Skip(3).Take(29));

                        if (codigoRetorno != 0)
                        {
                            Logger.GravarLog("Retorno método EfetivarSolicitacao", new { codigoRetorno, mensagemRetorno });
                            return false;
                        }
                    }
                }
                Logger.GravarLog("Retorno método EfetivarSolicitacao", new { codigoRetorno, mensagemRetorno });
                retorno = true;
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// Consulta Cobranças de PV por Período
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="mes">Mês</param>
        /// <param name="ano">Ano</param>
        /// <param name="PV">PV</param>
        /// <returns></returns>
        public EntidadeConsultaTrava ConsultaPeriodo(Int16 numEmissor, Int32 codigoProduto, Int16 mes, Int16 ano,
             out Int16 codigoRetorno)
        {
            EntidadeConsultaTrava entidade = new EntidadeConsultaTrava();
            Logger.IniciarLog("Início método ConsultaPeriodo");

            try
            {
                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    codigoRetorno = 0;

                    String codRetorno = "0";
                    string mensagem = string.Empty;
                    String vlTotPagar = "0";
                    String vlTotOcorrencias = "0";
                    String vlTotMaster = "0";
                    String vlTotVisa = "0";
                    //decimal nada = 0;
                    //decimal vlPrecoMedio = 0;

                    Logger.GravarLog("Chamada ao método HIS ZP382", new { vlTotOcorrencias, vlTotPagar, vlTotMaster, vlTotVisa, numEmissor, codigoProduto, ano, mes });

                    List<ZP382_OCORR> ListaSaida = context.Cliente.ZP382(out codRetorno, out mensagem, out vlTotOcorrencias, out vlTotPagar, out vlTotMaster, out vlTotVisa, numEmissor.ToString(), codigoProduto.ToString("D5"), ano.ToString(), mes.ToString("D2"));

                    Logger.GravarLog("Retorno chamada ao método HIS ZP382", new { codRetorno, mensagem, vlTotOcorrencias, vlTotPagar, vlTotMaster, vlTotVisa });

                    entidade = new EntidadeConsultaTrava()
                    {
                        ValorTotalCobranca = vlTotPagar.ToDecimalNull(0).Value / 100,
                        DadosConsultaTravas =
                        new List<DadosConsultaTrava>(
                            ListaSaida.Where(f => f.ZP382_QT_PV.ToInt32() > 0).Select(x => new DadosConsultaTrava
                        {
                            QuantidadePVs = x.ZP382_QT_PV.ToInt32(0),
                            TotalCobranca = x.ZP382_TT_COB.ToDecimalNull(0).Value / 100,
                            TotalCobrancaMasterCard = x.ZP382_TT_COB_MC.ToDecimalNull(0).Value / 100,
                            TotalCobrancaVisa = x.ZP382_TT_COB_VS.ToDecimalNull(0).Value / 100,
                            FaixaFinalFaturamento = x.ZP382_FX_FIM.ToDecimalNull(0).Value / 100,
                            FaixaInicialFaturamento = x.ZP382_FX_INI.ToDecimalNull(0).Value / 100,
                            FatorMultiplicado = x.ZP382_FT_MULT.ToDecimalNull(0).Value / 100
                        }))
                    };
                    codigoRetorno = codRetorno.ToInt16(0);
                    Logger.GravarLog("Retorno do método ConsultaPeriodo", new { codigoRetorno, mensagem, entidade });
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            return entidade;
        }

        public EntidadeConsultaTrava ConsultarTotaisCobranca(Int16 funcao,
            Int32 numPv,
            decimal cnpj,
            String dataDe,
            String dataAte,
            Int16 codigoBanco,
            Int32 codigoProduto,
            Int16 anoCompetencia,
            Int16 mesCompetencia,
            decimal precoMedioReferencia,
            out Int16 codigoRetorno,
            out String mensagemRetorno)
        {
            EntidadeConsultaTrava entidade = new EntidadeConsultaTrava();
            try
            {
                Logger.IniciarLog("Início método ConsultarTotaisCobranca");

                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    codigoRetorno = 0;
                    mensagemRetorno = string.Empty;

                    Int16 totalOcrrenciasRetornada = 0;
                    String valorTotalFaixaMasterCard = "0";
                    String vlorTotalFaixaVisa = "0";
                    String valorTotalFaixas = "0";
                    String valorTotalCobradoFaixaMasterCard = "0";
                    String valorTotalCobradoFaixaVisa = "0";
                    String valorTotalCobradoFaixas = "0";
                    String valorTotalCobrancaMasterCard = "0";
                    String valorTotalCobrancaVisa = "0";
                    String valorTotalCobrancas = "0";

                    Logger.GravarLog("Chamada ao método HIS ZP380", new
                    {
                        funcao,
                        numPv,
                        cnpj,
                        dataDe,
                        dataAte,
                        codigoBanco,
                        codigoProduto,
                        anoCompetencia,
                        mesCompetencia,
                        precoMedioReferencia
                    });
                    List<ZP380_OCORR> listaRetorno = context.Cliente.ZP380(out codigoRetorno,
                        out mensagemRetorno,
                        out totalOcrrenciasRetornada,
                        out valorTotalFaixaMasterCard,
                        out vlorTotalFaixaVisa,
                        out valorTotalFaixas,
                        out valorTotalCobradoFaixaMasterCard,
                        out valorTotalCobradoFaixaVisa,
                        out valorTotalCobradoFaixas,
                        out valorTotalCobrancaMasterCard,
                        out valorTotalCobrancaVisa,
                        out valorTotalCobrancas,
                        funcao, numPv, cnpj, dataDe, dataAte, codigoBanco,
                        codigoProduto, anoCompetencia, mesCompetencia, precoMedioReferencia.ToString());

                    Logger.GravarLog("Retorno chamada ao método HIS ZP380", new
                    {
                        listaRetorno,
                        codigoRetorno,
                        mensagemRetorno,
                        totalOcrrenciasRetornada,
                        valorTotalFaixaMasterCard,
                        vlorTotalFaixaVisa,
                        valorTotalFaixas,
                        valorTotalCobradoFaixaMasterCard,
                        valorTotalCobradoFaixaVisa,
                        valorTotalCobradoFaixas,
                        valorTotalCobrancaMasterCard,
                        valorTotalCobrancaVisa,
                        valorTotalCobrancas
                    });
                    entidade.ValorTotalFaixaMasterCard = valorTotalFaixaMasterCard.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalFaixaVisa = vlorTotalFaixaVisa.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalFaixas = valorTotalFaixas.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobradoFaixaMasterCard = valorTotalCobradoFaixaMasterCard.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobradoFaixaVisa = valorTotalCobradoFaixaVisa.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobradoFaixas = valorTotalCobradoFaixas.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobrancaMasterCard = valorTotalCobrancaMasterCard.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobrancaVisa = valorTotalCobrancaVisa.ToDecimalNull(0).Value / 100;
                    entidade.ValorTotalCobranca = valorTotalCobrancas.ToDecimalNull(0).Value / 100;


                    entidade.DadosConsultaTravas =
                        new List<DadosConsultaTrava>(listaRetorno.Where(i => i.ZP380_QT_PV > 0).Select(x => new DadosConsultaTrava
                        {
                            FatorMultiplicado = x.ZP380_FAT_MULT,
                            FaixaInicialFaturamento = x.ZP380_FX_INI_FAT.ToDecimalNull(0).Value / 100,
                            FaixaFinalFaturamento = x.ZP380_FX_FIM_FAT.ToDecimalNull(0).Value / 100,
                            QuantidadeDias = x.ZP380_QT_DIAS,
                            QuantidadePVs = x.ZP380_QT_PV,
                            TotalCobranca = x.ZP380_TT_COB.ToDecimalNull(0).Value / 100,
                            TotalCobrancaMasterCard = x.ZP380_TT_COBMC.ToDecimalNull(0).Value / 100,
                            TotalCobrancaVisa = x.ZP380_TT_COBVS.ToDecimalNull(0).Value / 100,
                            TotalFaturamento = x.ZP380_TT_FAT.ToDecimalNull(0).Value / 100,
                            TotalFaturamentoMasterCard = x.ZP380_TT_FATMC.ToDecimalNull(0).Value / 100,
                            TotalFaturamentoVisa = x.ZP380_TT_FATVS.ToDecimalNull(0).Value / 100
                        }));
                    Logger.GravarLog("Retorno do método ConsultarTotaisCobranca", new { codigoRetorno, entidade });
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            return entidade;
        }

        public List<InformacaoCobranca> ConsultaInformacaoCobranca(Int16 numEmissor, Int16 codProduto, Int16 mes, Int16 ano, out Int16 cod_erro)
        {
            EntidadeConsultaTrava entidade = new EntidadeConsultaTrava();
            cod_erro = 0;
            List<InformacaoCobranca> lstRetorno = null;
            try
            {
                Logger.IniciarLog("Início método ConsultaInformacaoCobranca");

                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    string mensagem = "";
                    Logger.GravarLog("Chamada ao método HIS ZP379", new { numEmissor, codProduto, ano, mes });

                    List<ZP379_OCORR> list = context.Cliente.ZP379(out cod_erro, out mensagem, numEmissor, codProduto, ano, mes);
                    Logger.GravarLog("Retorno chamada ao método HIS ", new { list, cod_erro, mensagem });
                    lstRetorno = PreencheModeloInformacaoCobranca(list);
                    Logger.GravarLog("Retorno do método ConsultaInformacaoCobranca", new { cod_erro, mensagem, lstRetorno });
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return lstRetorno;
        }

        private List<InformacaoCobranca> PreencheModeloInformacaoCobranca(List<ZP379_OCORR> listaOcorrencia)
        {
            List<InformacaoCobranca> lstRetorno = new List<InformacaoCobranca>();

            foreach (ZP379_OCORR item in listaOcorrencia)
            {
                lstRetorno.Add(new InformacaoCobranca()
                {
                    Tipo = item.ZP379_DEB_CRE,
                    PercentualInicialMarketShare = item.ZP379_PC_INI_MKSH,
                    PercentualFinalMarketShare = item.ZP379_PC_FIM_MKSH,
                    PercentualMarketShare = item.ZP379_PC_MRKT_SHR,
                    PrecoMedioReferencia = item.ZP379_PR_MED_REF.ToDecimalNull(0).Value / 100,
                    ValorTotalCobrado = item.ZP379_VL_TOT_COB,
                    ValorTotalFaturamento = item.ZP379_VL_TOT_FAT
                });
            }

            return lstRetorno;

        }



        public List<InformacaoPVCobrada> ConsultarInformacoesPVCobranca(
           Int16 funcao, Int32 numeroPv, Decimal cnpj,
            String datade, String datate, Int16 codBanco, Int32 codProduto, Int16 anoComp,
            Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Decimal fatorMultiplicador,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 codigoRetorno, out String mensagemRetorno)
        {
            codigoRetorno = 0;
            mensagemRetorno = string.Empty;
            Int32 totalOcorrencias = 0;

            rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
            Logger.IniciarLog("Método ConsultarInformacoesPVCobranca");

            String continua = rechamada.GetValueOrDefault<String>("continua");
            Int32 chaveNumeroPv = rechamada.GetValueOrDefault<Int32>("chaveNumeroPv");
            Int32 chaveCodigoProduto = rechamada.GetValueOrDefault<Int32>("chaveCodigoProduto");
            String chaveCreditoDebito = rechamada.GetValueOrDefault<String>("chaveCreditoDebito");
            String chaveSiglaProduto = rechamada.GetValueOrDefault<String>("chaveSiglaProduto");
            Int32 chaveAnoMes = rechamada.GetValueOrDefault<Int32>("chaveAnoMes");
            String chaveDataInicial = rechamada.GetValueOrDefault<String>("chaveDataInicial");
            Decimal chaveFaixaInicialFatramento = rechamada.GetValueOrDefault<Int32>("chaveFaixaInicialFatramento");
            Decimal chaveFaixaFinalFatramento = rechamada.GetValueOrDefault<Int32>("chaveFaixaFinalFatramento");

            List<InformacaoPVCobrada> lstRetorno = new List<InformacaoPVCobrada>();
            Int32 qtdChamadas = 0;
            Int32 contadorChamada = 0;
            String filler = string.Empty;
            try
            {
                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    List<ZP381_OCORR> lstRetornoChamada = new List<ZP381_OCORR>();
                    Logger.GravarLog("Chamda ao método HIS ZP381", new
                    {
                        funcao,
                        numeroPv,
                        cnpj,
                        datade,
                        datate,
                        codBanco,
                        codProduto,
                        anoComp,
                        mesComp,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento,
                        fatorMultiplicador,
                        filler
                    });
                    //Buscando Preço Médio
                    lstRetornoChamada.AddRange(
                        context.Cliente.ZP381(funcao, numeroPv, cnpj, datade, datate, codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento,
                        fatorMultiplicador, filler, ref continua, ref chaveNumeroPv, ref chaveCodigoProduto, ref chaveAnoMes,
                        ref chaveCreditoDebito, ref chaveSiglaProduto, ref chaveDataInicial, ref chaveFaixaInicialFatramento, ref chaveFaixaFinalFatramento,
                        out codigoRetorno, out mensagemRetorno, out totalOcorrencias));

                    Logger.GravarLog("retorno do método HSI ZP381", new { continua, chaveNumeroPv, chaveCodigoProduto, chaveAnoMes, chaveCreditoDebito, chaveSiglaProduto, chaveDataInicial, chaveFaixaInicialFatramento, chaveFaixaFinalFatramento, codigoRetorno, mensagemRetorno, totalOcorrencias });

                    if (totalOcorrencias > 0)
                    {
                        qtdChamadas = (totalOcorrencias / 250) + 1;

                        contadorChamada += 1;

                        indicadorRechamada = (String.Compare(continua, "F") != 0) || (qtdChamadas != contadorChamada);
                        //continua = "S";

                    }
                    else
                    {
                        indicadorRechamada = false;
                    }

                    lstRetorno.AddRange(lstRetornoChamada.Where(filtro => filtro.ZP381R_NUM_PV != 0 && filtro.ZP381R_NUM_CNPJ.ToInt32(0) != 0).Select(item => new InformacaoPVCobrada()
                    {
                        Cnpj = item.ZP381R_NUM_CNPJ.ToDecimalNull(0).Value,
                        CodigoAgencia = item.ZP381R_COD_AGE,
                        Tipo = item.ZP381R_CRE_DEB,
                        DataFinal = item.ZP381R_DAT_FIM,
                        DataInicial = item.ZP381R_DAT_INI,
                        NumeroConta = item.ZP381R_NRO_CTA,
                        NumeroPV = item.ZP381R_NUM_PV,
                        PVCentralizador = item.ZP381R_ID_PV_CT,
                        QuantidadeDias = item.ZP381R_QTD_DIAS,
                        SiglaProduto = item.ZP381R_SGL_PROD,
                        ValCobranca = item.ZP381R_VAL_COB.ToDecimalNull(0).Value / 100,
                        ValorFaturamento = item.ZP381R_VAL_FAT.ToDecimalNull(0).Value / 100
                    }));

                    rechamada["continua"] = continua;
                    rechamada["chaveNumeroPv"] = chaveNumeroPv;
                    rechamada["chaveCodigoProduto"] = chaveCodigoProduto;
                    rechamada["chaveCreditoDebito"] = chaveCreditoDebito;
                    rechamada["chaveSiglaProduto"] = chaveSiglaProduto;
                    rechamada["chaveAnoMes"] = chaveAnoMes;
                    rechamada["chaveDataInicial"] = chaveDataInicial;
                    rechamada["chaveFaixaInicialFatramento"] = chaveFaixaInicialFatramento;
                    rechamada["chaveFaixaFinalFatramento"] = chaveFaixaFinalFatramento;

                    Logger.GravarLog("retorno do método ConsultarInformacoesPVCobranca", new { lstRetorno });
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return lstRetorno;

        }

        public List<InformacaoDetalhada> ConsultarInformacoesDetalhadas(Int16 codBanco, Int32 codProduto, Int16 anoComp,
        Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento,
            ref Dictionary<String, Object> rechamada, out Boolean indicadorRechamada, out Int16 codigoRetorno, out String mensagemRetorno)
        {
            Logger.IniciarLog("Início método ConsultarInformacoesDetalhadas");
            codigoRetorno = 0;
            mensagemRetorno = string.Empty;
            Int16 totalOcorrencias = 0;
            Decimal valorTotalFaixa = 0;

            rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
            String continua = rechamada.GetValueOrDefault<String>("continua");
            Int32 chaveNumeroPv = rechamada.GetValueOrDefault<Int32>("chaveNumeroPv");

            List<InformacaoDetalhada> lstRetorno = new List<InformacaoDetalhada>();

            try
            {
                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    List<ZP383_OCORR> lstRetornoChamada = new List<ZP383_OCORR>();
                    Logger.GravarLog("Chamada ao método HIS ZP383", new { codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento });

                    //Buscando Preço Médio
                    lstRetornoChamada.AddRange(context.Cliente.ZP383(codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento,
                        ref continua, ref chaveNumeroPv,
                        out codigoRetorno, out mensagemRetorno, out totalOcorrencias, out valorTotalFaixa));
                    Logger.GravarLog("Retorno chamada ao método HIS ZP383", new
                    {
                        continua,
                        chaveNumeroPv,
                        codigoRetorno,
                        mensagemRetorno,
                        totalOcorrencias,
                        valorTotalFaixa
                    });

                    if (totalOcorrencias > 300)
                    {
                        indicadorRechamada = (String.Compare(continua, "F") != 0);
                        //continua = "S";
                    }
                    else
                    {
                        indicadorRechamada = false;
                    }

                    lstRetorno.AddRange(lstRetornoChamada.Where(filtro => filtro.ZP383_PV != 0 && filtro.ZP383_CNPJ.ToInt32(0) != 0).Select(item => new InformacaoDetalhada()
                    {
                        Cnpj = item.ZP383_CNPJ.ToDecimalNull(0).Value,
                        CodigoAgencia = item.ZP383_AGENC.ToInt32(0),
                        NumeroPV = item.ZP383_PV,
                        ContaCorrente = item.ZP383_CONTA,
                        PeriodoFinal = !string.IsNullOrEmpty(item.ZP383_PER_FIM) ? item.ZP383_PER_FIM.Replace('.', '/') : string.Empty,
                        PeriodoInicial = !string.IsNullOrEmpty(item.ZP383_PER_INI) ? item.ZP383_PER_INI.Replace('.', '/') : string.Empty,
                        ValorFinalCobranca = item.ZP383_VL_FIM.ToDecimalNull(0).Value / 100,
                        ValorLiquido = item.ZP383_VL_LIQ.ToDecimalNull(0).Value / 100
                    }));
                    Logger.GravarLog("Retorno do método ConsultarInformacoesDetalhadas", new { codigoRetorno, lstRetorno });
                    rechamada["continua"] = continua;
                    rechamada["chaveNumeroPv"] = chaveNumeroPv;
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return lstRetorno;

        }
        public List<DetalheFatura> ConsultarDetalheFatura(Int16 codBanco, Int32 codProduto, Int16 anoComp,
        Int16 mesComp, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Int32 pvOriginal,
            ref Dictionary<String, Object> rechamada, out Boolean indicadorRechamada, out Int16 codigoRetorno, out String mensagemRetorno)
        {
            Logger.IniciarLog("Início método ConsultarInformacoesDetalhadas");

            codigoRetorno = 0;
            mensagemRetorno = string.Empty;
            Int16 totalOcorrencias = 0;
            Decimal valorTotalPV = 0;

            rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
            String continua = rechamada.GetValueOrDefault<String>("continua");
            Int32 chaveNumeroPv = rechamada.GetValueOrDefault<Int32>("chaveNumeroPv");

            List<DetalheFatura> lstRetorno = new List<DetalheFatura>();

            try
            {
                using (var context = new ContextoWCF<COMTIZPClient>())
                {
                    List<ZP384_OCORR> lstRetornoChamada = new List<ZP384_OCORR>();
                    Logger.GravarLog("Chamada ao método HIS ZP384", new
                    {
                        codBanco,
                        codProduto,
                        anoComp,
                        mesComp,
                        faixaInicialFaturamento,
                        faixaFinalFaturamento,
                        pvOriginal
                    });
                    //Buscando Preço Médio
                    lstRetornoChamada.AddRange(context.Cliente.ZP384(codBanco, codProduto, anoComp, mesComp, faixaInicialFaturamento, faixaFinalFaturamento,
                        pvOriginal, ref continua, ref chaveNumeroPv,
                        ref codigoRetorno, ref mensagemRetorno, ref totalOcorrencias, ref valorTotalPV));

                    Logger.GravarLog("Retorno chamada ao método HIS ", new
                    {
                        lstRetornoChamada,
                        continua,
                        chaveNumeroPv,
                        codigoRetorno,
                        mensagemRetorno,
                        totalOcorrencias,
                        valorTotalPV
                    });

                    if (totalOcorrencias > 300)
                    {
                        indicadorRechamada = (String.Compare(continua, "F") != 0);
                    }
                    else
                    {
                        indicadorRechamada = false;
                    }

                    lstRetorno.AddRange(lstRetornoChamada.Where(filtro => filtro.ZP384_PV != 0 && filtro.ZP384_CNPJ.ToInt32(0) != 0).Select(item => new DetalheFatura()
                    {
                        Cnpj = item.ZP384_CNPJ.ToDecimalNull(0).Value,
                        CodigoAgencia = item.ZP384_AGENC.ToInt32(0),
                        NumeroPV = item.ZP384_PV,
                        ContaCorrente = item.ZP384_CONTA,
                        QuantidadeDias = item.ZP384_QT_DIAS,
                        Tipo = item.ZP384_IND_CD,
                        SiglaProduto = item.ZP384_SGL_PROD,
                        PeriodoFinal = !string.IsNullOrEmpty(item.ZP384_PER_FIM) ? item.ZP384_PER_FIM.Replace('.', '/') : string.Empty,
                        PeriodoInicial = !string.IsNullOrEmpty(item.ZP384_PER_INI) ? item.ZP384_PER_INI.Replace('.', '/') : string.Empty,
                        ValorLiquido = item.ZP384_VL_LIQ.ToDecimalNull(0).Value / 100
                    }));

                    rechamada["continua"] = continua;
                    rechamada["chaveNumeroPv"] = chaveNumeroPv;

                    Logger.GravarLog("Retorno do método ", new { codigoRetorno, lstRetorno });
                }
            }
            catch (FaultException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro", ex);
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
            return lstRetorno;

        }

        String FormataContaCorrente(int codigoBanco, int numeroConta)
        {
            String retorno = string.Empty;
            switch (codigoBanco)
            {
                case 399:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(6, '0'));
                    break;
                case 409:
                case 27:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(7, '0'));
                    break;
                case 237:
                case 353:
                case 341:
                case 008:
                case 634:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(8, '0'));
                    break;
                case 033:
                case 151:
                case 745:
                case 001:
                case 347:
                case 477:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(9, '0'));
                    break;
                case 070:
                case 230:
                case 041:
                case 104:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(10, '0'));
                    break;
                case 424:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(11, '0'));
                    break;
                default:
                    retorno = String.Format("{0}", numeroConta.ToString().PadLeft(15, '0'));
                    break;
            }
            return retorno.PadRight(15, ' ');
        }
    }
}