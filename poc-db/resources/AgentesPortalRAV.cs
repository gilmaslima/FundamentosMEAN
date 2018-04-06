using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Modelos;
using Redecard.PN.RAV.Agentes.ModuloRAV;

namespace Redecard.PN.RAV.Agentes
{
    public class AgPortalRAV : AgentesBase
    {
        #region Constantes
        public const string MENSAGEM_VIP = "Seu estabelecimento faz parte de um grupo de clientes especiais da Redecard. Para antecipar seus recebíveis, ligue para a Central de Atendimento.<br/><br/><b>Capitais e Regiões Metropolitanas</b><br/> 2846-1100 / 4001-4446 <br/><br/>De segunda a sexta, das 09h às 17h, exceto feriados.";
        #endregion


        #region SINGLETON
        private static AgPortalRAV agentesPortalRav = null;

        //public const int CODIGO_ERRO = 400;
        //public const string FONTE = "Redecard.PN.Agentes";

        private AgPortalRAV()
        { }

        /// <summary>
        /// Retorna a instância do objeto.
        /// </summary>
        /// <returns></returns>
        public static AgPortalRAV GetInstance()
        {
            if (agentesPortalRav == null)
            { agentesPortalRav = new AgPortalRAV(); }
            return agentesPortalRav;
        }
        #endregion

        #region Transação MA30 - RAV Avulso

        public Modelos.MA30 ExecutarMA30(Modelos.MA30 chamadaMA30)
        {
            using (Logger Log = Logger.IniciarLog("Consulta RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { chamadaMA30 });

                Modelos.MA30 retornoMA30 = new MA30();

                string dataDoProcessamento = chamadaMA30.MA030_DAT_PROCESSAMENTO; //"";//MA030_DAT_PROCESSAMENTO, 
                string horaDoProcessamento = chamadaMA30.MA030_HOR_PROCESSAMENTO; //"";//MA030_HOR_PROCESSAMENTO, 
                short bancoParaCredito = chamadaMA30.MA030_BANCO;//0;//MA030_BANCO, 
                int agenciaParaCredito = chamadaMA30.MA030_AGENCIA; //0;//MA030_AGENCIA, 
                decimal contaParaCredito = chamadaMA30.MA030_CONTA;//0;//MA030_CONTA,
                decimal valorMinimoParaAntecipacao = chamadaMA30.MA030_VALOR_MINIMO; //0;//MA030_VALOR_MINIMO,
                short horaInicioAntecipacaoD0 = chamadaMA30.MA030_HORA_INI_D0; //0;//MA030_HORA_INI_D0, 
                short horaFimAntecipacaoD0 = chamadaMA30.MA030_HORA_FIM_D0; //0;//MA030_HORA_FIM_D0, 
                short horaInicioAntecipacaoDN = chamadaMA30.MA030_HORA_INI_DN; //0;//MA030_HORA_INI_DN,
                short horaFimAntecipacaoDN = chamadaMA30.MA030_HORA_FIM_DN; ; //MA030_HORA_FIM_DN,
                decimal taxaDeDesconto = chamadaMA30.MA030_PCT_DESCONTO;//0; //MA030_PCT_DESCONTO, 
                decimal valorTotalDisponivelParaAntecipacao = chamadaMA30.MA030_VALOR_BRUTO; //0;//MA030_VALOR_BRUTO, 
                decimal valorBrutoOriginal = chamadaMA30.MA030_VALOR_ORIG;//0; //MA030_VALOR_ORIG, 
                string inicioPeriodoAntecipacao = chamadaMA30.MA030_DAT_PERIODO_DE;//"";//MA030_DAT_PERIODO_DE, 
                string fimPeriodoAntecipacao = chamadaMA30.MA030_DAT_PERIODO_ATE;//"";//MA030_DAT_PERIODO_ATE,  
                string mensagemDeErro = chamadaMA30.MA030_MSGERRO; //"";//MA030_MSGERRO,
                string dataFimCarencia = chamadaMA30.MA030_DATA_FIM_CARENCIA; //""; //MA030_DATA_FIM_CARENCIA,
                decimal valorTotalAntecipadoParaPagamentoD0 = chamadaMA30.MA030_VALOR_ANTEC_D0; //0;//MA030_VALOR_ANTEC_D0, 
                decimal valorTotalAntecipadoParaPagamentoD1 = chamadaMA30.MA030_VALOR_ANTEC_D1; //0;//MA030_VALOR_ANTEC_D1,
                decimal valorDisponivel = chamadaMA30.MA030_VALOR_DISP_ANTEC; //0;//MA030_VALOR_DISP_ANTEC, 
                int qtdDeRVExistente = chamadaMA30.MA030_RV_QTD_RV; //0; //MA030_RV_QTD_RV, 

                short tipoCredito = chamadaMA30.MA030_TIP_CREDITO; // 00; //MA030_TIP_CREDITO, 

                ModuloRAV.FILLER[] filler = new ModuloRAV.FILLER[2]; //= new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                ModuloRAV.FILLER1[] filler1 = new ModuloRAV.FILLER1[20]; // = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 

                string totalParcial = chamadaMA30.MA030_CA_IND_ANTEC; //"";//MA030_CA_IND_ANTEC, 

                //decimal valorAAntecipar = 0;
                Decimal valorParcialAntecipacao = chamadaMA30.MA030_CA_VAL_ANTEC; //""; //MA030_CA_VAL_ANTEC,
                Decimal valorAntecipado = chamadaMA30.MA030_VALOR_A_ANTECIPAR;
                //valorParcialAntecipacao = valorAntecipado.ToString().Replace(',', '.');

                //if (valorParcialAntecipacao.IndexOf('.') > 0)
                //{
                //    valorParcialAntecipacao = (valorParcialAntecipacao.Split('.')[0] + valorParcialAntecipacao.Split('.')[1]).PadLeft(15, '0');
                //}
                //else
                //{
                //    valorParcialAntecipacao = valorParcialAntecipacao.PadLeft(13, '0') + "00";
                //}
                //valorAAntecipar = valorAntecipado;//MA030_VALOR_A_ANTECIPAR

                string tipoDeSelecaoPorPeriodo = chamadaMA30.MA030_CA_IND_DATA_ANTEC;//""; //MA030_CA_IND_DATA_ANTEC, 
                string periodoDeSelecaoDe = chamadaMA30.MA030_CA_PER_DATA_DE; //"";//MA030_CA_PER_DATA_DE, 
                string periodoDeSelecaoAte = chamadaMA30.MA030_CA_PER_DATA_ATE;//"";//MA030_CA_PER_DATA_ATE, 
                string DataQueTemRestricao = chamadaMA30.MA030_DAT_RESTRICAO; //"";//MA030_DAT_RESTRICAO, 
                int numPdv = chamadaMA30.MA030_NUM_PDV; //numeroPDV;//MA030_NUM_PDV, 
                short funcao = chamadaMA30.MA030_FUNCAO;//MA030_FUNCAO, 




                string indProduto = chamadaMA30.MA030_CA_IND_PRODUTO; //entradaRAV.IndProduto.ToString();

                short tipCredito = chamadaMA30.MA030_TIP_CREDITO;// (short)tipoCredito; //MA030_TIP_CREDITO, 

                short canal = chamadaMA30.MA030_CANAL; //06;//MA030_CANAL, 

                Decimal valorMaxAntecUraSemSenha = chamadaMA30.MA030_VL_MX_ANT_URA_SNHA;

                Decimal valorTarifa = chamadaMA30.MA030_VAL_TRFA;
                Decimal percentualTarifa = chamadaMA30.MA030_PCT_TRFA;
                Int16 codigoProdutoAntecipacao = chamadaMA30.MA030_COD_PROD_ANTC;
                String nomeProdutoAntecipacao = chamadaMA30.MA030_NOM_PROD_ANTC;
                String descricaoProdutoAntecipacao = chamadaMA30.MA030_DSC_PROD_ANTC;

                short ret = 0;
                using (ModuloRAVClient cliente = new ModuloRAVClient())
                {

                    Log.GravarLog(EventoLog.ChamadaHIS, new
                    {
                        ret,
                        dataDoProcessamento,
                        horaDoProcessamento,
                        bancoParaCredito,
                        agenciaParaCredito,
                        contaParaCredito,
                        valorMinimoParaAntecipacao,
                        horaInicioAntecipacaoD0,
                        horaFimAntecipacaoD0,
                        horaInicioAntecipacaoDN,
                        horaFimAntecipacaoDN,
                        taxaDeDesconto,
                        valorTotalDisponivelParaAntecipacao,
                        valorBrutoOriginal,
                        periodoDeSelecaoDe,
                        periodoDeSelecaoAte,
                        mensagemDeErro,
                        dataFimCarencia,
                        valorTotalAntecipadoParaPagamentoD0,
                        valorTotalAntecipadoParaPagamentoD1,
                        valorDisponivel,
                        qtdDeRVExistente,
                        numPdv,
                        funcao,
                        tipoCredito,
                        canal,
                        valorAntecipado,
                        totalParcial,
                        valorParcialAntecipacao,
                        tipoDeSelecaoPorPeriodo,
                        DataQueTemRestricao,
                        indProduto,
                        valorMaxAntecUraSemSenha,
                        valorTarifa,
                        percentualTarifa,
                        codigoProdutoAntecipacao,
                        nomeProdutoAntecipacao,
                        descricaoProdutoAntecipacao
                    });


                    cliente.BMA030(ref numPdv, ref funcao, ref tipoCredito, ref canal, ref valorAntecipado, ref ret, ref dataDoProcessamento, ref horaDoProcessamento, ref bancoParaCredito,
                        ref agenciaParaCredito, ref contaParaCredito, ref valorMinimoParaAntecipacao, ref horaInicioAntecipacaoD0, ref horaFimAntecipacaoD0, ref horaInicioAntecipacaoDN,
                        ref horaFimAntecipacaoDN, ref filler, ref taxaDeDesconto, ref valorTotalDisponivelParaAntecipacao, ref valorBrutoOriginal, ref inicioPeriodoAntecipacao, ref fimPeriodoAntecipacao,
                        ref mensagemDeErro, ref dataFimCarencia, ref valorTotalAntecipadoParaPagamentoD0, ref valorTotalAntecipadoParaPagamentoD1, ref valorDisponivel, ref qtdDeRVExistente,
                        ref filler1, ref totalParcial, ref valorParcialAntecipacao, ref tipoDeSelecaoPorPeriodo, ref periodoDeSelecaoDe, ref periodoDeSelecaoAte, ref DataQueTemRestricao,
                        ref indProduto, ref valorMaxAntecUraSemSenha, ref valorTarifa, ref percentualTarifa, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao);


                    Log.GravarLog(EventoLog.RetornoHIS, new
                    {
                        ret,
                        dataDoProcessamento,
                        horaDoProcessamento,
                        bancoParaCredito,
                        agenciaParaCredito,
                        contaParaCredito,
                        valorMinimoParaAntecipacao,
                        horaInicioAntecipacaoD0,
                        horaFimAntecipacaoD0,
                        horaInicioAntecipacaoDN,
                        horaFimAntecipacaoDN,
                        taxaDeDesconto,
                        valorTotalDisponivelParaAntecipacao,
                        valorBrutoOriginal,
                        periodoDeSelecaoDe,
                        periodoDeSelecaoAte,
                        mensagemDeErro,
                        dataFimCarencia,
                        valorTotalAntecipadoParaPagamentoD0,
                        valorTotalAntecipadoParaPagamentoD1,
                        valorDisponivel,
                        qtdDeRVExistente,
                        numPdv,
                        funcao,
                        tipoCredito,
                        canal,
                        valorAntecipado,
                        totalParcial,
                        valorParcialAntecipacao,
                        tipoDeSelecaoPorPeriodo,
                        DataQueTemRestricao,
                        indProduto,
                        valorMaxAntecUraSemSenha,
                        valorTarifa,
                        percentualTarifa,
                        codigoProdutoAntecipacao,
                        nomeProdutoAntecipacao,
                        descricaoProdutoAntecipacao,
                        filler,
                        filler1
                    });
                }

                retornoMA30.MA030_DAT_PROCESSAMENTO = dataDoProcessamento; //"";//MA030_DAT_PROCESSAMENTO, 
                retornoMA30.MA030_HOR_PROCESSAMENTO = horaDoProcessamento; //= chamadaMA30.MA030_HOR_PROCESSAMENTO; //"";//MA030_HOR_PROCESSAMENTO, 
                retornoMA30.MA030_BANCO = bancoParaCredito; //= chamadaMA30.MA030_BANCO;//0;//MA030_BANCO, 
                retornoMA30.MA030_AGENCIA = agenciaParaCredito; //= chamadaMA30.MA030_AGENCIA; //0;//MA030_AGENCIA, 
                retornoMA30.MA030_CONTA = contaParaCredito; //= chamadaMA30.MA030_CONTA;//0;//MA030_CONTA,
                retornoMA30.MA030_VALOR_MINIMO = valorMinimoParaAntecipacao; // = chamadaMA30.MA030_VALOR_MINIMO; //0;//MA030_VALOR_MINIMO,
                retornoMA30.MA030_HORA_INI_D0 = horaInicioAntecipacaoD0; // = chamadaMA30.MA030_HORA_INI_D0; //0;//MA030_HORA_INI_D0, 
                retornoMA30.MA030_HORA_FIM_D0 = horaFimAntecipacaoD0; //= chamadaMA30.MA030_HORA_FIM_D0; //0;//MA030_HORA_FIM_D0, 
                retornoMA30.MA030_HORA_INI_DN = horaInicioAntecipacaoDN; // = chamadaMA30.MA030_HORA_INI_DN; //0;//MA030_HORA_INI_DN,
                retornoMA30.MA030_HORA_FIM_DN = horaFimAntecipacaoDN; // = chamadaMA30.MA030_HORA_FIM_DN; ; //MA030_HORA_FIM_DN,
                retornoMA30.MA030_PCT_DESCONTO = taxaDeDesconto; // = chamadaMA30.MA030_PCT_DESCONTO;//0; //MA030_PCT_DESCONTO, 
                retornoMA30.MA030_VALOR_BRUTO = valorTotalDisponivelParaAntecipacao; // = chamadaMA30.MA030_VALOR_BRUTO; //0;//MA030_VALOR_BRUTO, 
                retornoMA30.MA030_VALOR_ORIG = valorBrutoOriginal; // = chamadaMA30.MA030_VALOR_ORIG;//0; //MA030_VALOR_ORIG, 
                retornoMA30.MA030_DAT_PERIODO_DE = inicioPeriodoAntecipacao; // = chamadaMA30.MA030_DAT_PERIODO_DE;//"";//MA030_DAT_PERIODO_DE, 
                retornoMA30.MA030_DAT_PERIODO_ATE = fimPeriodoAntecipacao; // = chamadaMA30.MA030_DAT_PERIODO_ATE;//"";//MA030_DAT_PERIODO_ATE,  
                retornoMA30.MA030_MSGERRO = String.Format("Código Retorno: {0}; Mensagem: {1}", ret.ToString(), mensagemDeErro); // = chamadaMA30.MA030_MSGERRO; //"";//MA030_MSGERRO,
                retornoMA30.MA030_DATA_FIM_CARENCIA = dataFimCarencia; // = chamadaMA30.MA030_DATA_FIM_CARENCIA; //""; //MA030_DATA_FIM_CARENCIA,
                retornoMA30.MA030_VALOR_ANTEC_D0 = valorTotalAntecipadoParaPagamentoD0; // = chamadaMA30.MA030_VALOR_ANTEC_D0; //0;//MA030_VALOR_ANTEC_D0, 
                retornoMA30.MA030_VALOR_ANTEC_D1 = valorTotalAntecipadoParaPagamentoD1; //= chamadaMA30.MA030_VALOR_ANTEC_D1; //0;//MA030_VALOR_ANTEC_D1,
                retornoMA30.MA030_VALOR_DISP_ANTEC = valorDisponivel; // = chamadaMA30.MA030_VALOR_DISP_ANTEC; //0;//MA030_VALOR_DISP_ANTEC, 
                retornoMA30.MA030_RV_QTD_RV = qtdDeRVExistente; //= chamadaMA30.MA030_RV_QTD_RV; //0; //MA030_RV_QTD_RV, 

                retornoMA30.MA030_TIP_CREDITO = tipoCredito; //= chamadaMA30.MA030_TIP_CREDITO; // 00; //MA030_TIP_CREDITO, 

                retornoMA30.MA030_CA_IND_ANTEC = totalParcial; // = chamadaMA30.MA030_CA_IND_ANTEC; //"";//MA030_CA_IND_ANTEC, 

                //decimal valorAAntecipar = 0;
                retornoMA30.MA030_CA_VAL_ANTEC = valorParcialAntecipacao; // = chamadaMA30.MA030_CA_VAL_ANTEC; //""; //MA030_CA_VAL_ANTEC,
                retornoMA30.MA030_VALOR_A_ANTECIPAR = valorAntecipado; // = chamadaMA30.MA030_VALOR_A_ANTECIPAR;
                //valorParcialAntecipacao = valorAntecipado.ToString().Replace(',', '.');

                //if (valorParcialAntecipacao.IndexOf('.') > 0)
                //{
                //    valorParcialAntecipacao = (valorParcialAntecipacao.Split('.')[0] + valorParcialAntecipacao.Split('.')[1]).PadLeft(15, '0');
                //}
                //else
                //{
                //    valorParcialAntecipacao = valorParcialAntecipacao.PadLeft(13, '0') + "00";
                //}
                //valorAAntecipar = valorAntecipado;//MA030_VALOR_A_ANTECIPAR

                retornoMA30.MA030_DSC_PROD_ANTC = descricaoProdutoAntecipacao;
                retornoMA30.MA030_COD_PROD_ANTC = codigoProdutoAntecipacao;
                retornoMA30.MA030_NOM_PROD_ANTC = nomeProdutoAntecipacao;
                retornoMA30.MA030_VL_MX_ANT_URA_SNHA = valorMaxAntecUraSemSenha;
                retornoMA30.MA030_VAL_TRFA = valorTarifa;
                retornoMA30.MA030_PCT_TRFA = percentualTarifa;


                retornoMA30.MA030_CA_IND_DATA_ANTEC = tipoDeSelecaoPorPeriodo;  //= chamadaMA30.MA030_CA_IND_DATA_ANTEC;//""; //MA030_CA_IND_DATA_ANTEC, 
                retornoMA30.MA030_CA_PER_DATA_DE = periodoDeSelecaoDe; // = chamadaMA30.MA030_CA_PER_DATA_DE; //"";//MA030_CA_PER_DATA_DE, 
                retornoMA30.MA030_CA_PER_DATA_ATE = periodoDeSelecaoAte; // = chamadaMA30.MA030_CA_PER_DATA_ATE;//"";//MA030_CA_PER_DATA_ATE, 
                retornoMA30.MA030_DAT_RESTRICAO = DataQueTemRestricao; // = chamadaMA30.MA030_DAT_RESTRICAO; //"";//MA030_DAT_RESTRICAO, 
                retornoMA30.MA030_NUM_PDV = numPdv; // = chamadaMA30.MA030_NUM_PDV; //numeroPDV;//MA030_NUM_PDV, 
                retornoMA30.MA030_FUNCAO = funcao; // = chamadaMA30.MA030_FUNCAO;//MA030_FUNCAO, 

                retornoMA30.MA030_CA_IND_PRODUTO = indProduto; // = chamadaMA30.MA030_CA_IND_PRODUTO; //entradaRAV.IndProduto.ToString();
                retornoMA30.MA030_TIP_CREDITO = tipCredito; //= chamadaMA30.MA030_TIP_CREDITO;// (short)tipoCredito; //MA030_TIP_CREDITO, 
                retornoMA30.MA030_CANAL = canal; //= chamadaMA30.MA030_CANAL; //06;//MA030_CANAL, 

                Modelos.FILLER dadosPCT = null;
                retornoMA30.filler = new List<Modelos.FILLER>();
                //PCT
                foreach (ModuloRAV.FILLER dados in filler) //FILLER,
                {
                    dadosPCT = new Modelos.FILLER();

                    dadosPCT.MA030_DAT_CREDITO = dados.MA030_DAT_CREDITO;
                    dadosPCT.MA030_PCT_EFETIVA = dados.MA030_PCT_EFETIVA;
                    dadosPCT.MA030_PCT_PERIODO = dados.MA030_PCT_PERIODO;
                    dadosPCT.MA030_VALOR_LIQUIDO = dados.MA030_VALOR_LIQUIDO;
                    dadosPCT.MA030_VALOR_PARCELADO = dados.MA030_VALOR_PARCELADO;

                    retornoMA30.filler.Add(dadosPCT);
                }

                Modelos.FILLER1 dadosRV = null;
                retornoMA30.filler1 = new List<Modelos.FILLER1>();
                //RV
                foreach (ModuloRAV.FILLER1 dados in filler1) //FILLER1,
                {
                    dadosRV = new Modelos.FILLER1();

                    dadosRV.MA030_RV_DAT_APRS = dados.MA030_RV_DAT_APRS;
                    dadosRV.MA030_RV_NUM_RV = dados.MA030_RV_NUM_RV;
                    dadosRV.MA030_RV_QTD_OC = dados.MA030_RV_QTD_OC;
                    dadosRV.MA030_RV_VAL_BRTO = dados.MA030_RV_VAL_BRTO;
                    dadosRV.MA030_RV_VAL_LQDO = dados.MA030_RV_VAL_LQDO;

                    retornoMA30.filler1.Add(dadosRV);
                }



                return retornoMA30;
            }
        }

        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível. 
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida VerificarRAVDisponivel(Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verificação de RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV });

                ModRAVAvulsoSaida ravSaida = null;

#if DEBUG
                ravSaida = new ModRAVAvulsoSaida
                {
                    ValorDisponivel = DateTime.Now.ToString("ddMMyyyy").ToInt32() / 1000m
                };
#else

                try
                {
                    short ret = 0;
                    string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                    string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                    short bancoParaCredito = 0;//MA030_BANCO, 
                    int agenciaParaCredito = 0;//MA030_AGENCIA, 
                    decimal contaParaCredito = 0;//MA030_CONTA,
                    decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                    short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                    short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                    short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                    short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                    ModuloRAV.FILLER[] filler = new ModuloRAV.FILLER[2]; // = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                    decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                    decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                    decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                    string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                    string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                    string mensagemDeErro = "";//MA030_MSGERRO,
                    string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                    decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                    decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                    decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                    int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                    ModuloRAV.FILLER1[] filler1 = new ModuloRAV.FILLER1[20]; // = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                    string totalParcial = " ";//MA030_CA_IND_ANTEC, 
                    string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                    string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                    string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                    string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                    int numPdv = numeroPDV;//MA030_NUM_PDV, 
                    short funcao = 04;// (short)entradaRAV.Funcao;//MA030_FUNCAO, 
                    short tipoCredito = 00; //MA030_TIP_CREDITO, 
                    short canal = 06;// (short)entradaRAV.Canal;//MA030_CANAL, 
                    decimal valorAAntecipar = 0;//MA030_VALOR_A_ANTECIPAR
                    string indProduto = ""; //MA030_CA_IND_PRODUTO
                    Decimal valorAntecipado = 0; //MA030_VALOR_A_ANTECIPAR

                    Decimal valorMaxAntecUraSemSenha = 0; //MA030_VL_MX_ANT_URA_SNHA

                    Decimal valorTarifa = 0; //MA030_VAL_TRFA
                    Decimal percentualTarifa = 0; //MA030_PCT_TRFA
                    Int16 codigoProdutoAntecipacao = 0; //MA030_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA030_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA030_DSC_PROD_ANTC
                    Decimal valorParcialAntecipacao = 0; //MA030_CA_VAL_ANTEC


                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            ret,
                            dataDoProcessamento,
                            horaDoProcessamento,
                            bancoParaCredito,
                            agenciaParaCredito,
                            contaParaCredito,
                            valorMinimoParaAntecipacao,
                            horaInicioAntecipacaoD0,
                            horaFimAntecipacaoD0,
                            horaInicioAntecipacaoDN,
                            horaFimAntecipacaoDN,
                            taxaDeDesconto,
                            valorTotalDisponivelParaAntecipacao,
                            valorBrutoOriginal,
                            periodoDeSelecaoDe,
                            periodoDeSelecaoAte,
                            mensagemDeErro,
                            dataFimCarencia,
                            valorTotalAntecipadoParaPagamentoD0,
                            valorTotalAntecipadoParaPagamentoD1,
                            valorDisponivel,
                            qtdDeRVExistente,
                            numPdv,
                            funcao,
                            tipoCredito,
                            canal,
                            valorAntecipado,
                            totalParcial,
                            valorParcialAntecipacao,
                            tipoDeSelecaoPorPeriodo,
                            DataQueTemRestricao,
                            indProduto,
                            valorMaxAntecUraSemSenha,
                            valorTarifa,
                            percentualTarifa,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao

                        });


                        cliente.BMA030(ref numPdv, ref funcao, ref tipoCredito, ref canal, ref valorAntecipado, ref ret, ref dataDoProcessamento, ref horaDoProcessamento, ref bancoParaCredito,
                            ref agenciaParaCredito, ref contaParaCredito, ref valorMinimoParaAntecipacao, ref horaInicioAntecipacaoD0, ref horaFimAntecipacaoD0, ref horaInicioAntecipacaoDN,
                            ref horaFimAntecipacaoDN, ref filler, ref taxaDeDesconto, ref valorTotalDisponivelParaAntecipacao, ref valorBrutoOriginal, ref inicioPeriodoAntecipacao, ref fimPeriodoAntecipacao,
                            ref mensagemDeErro, ref dataFimCarencia, ref valorTotalAntecipadoParaPagamentoD0, ref valorTotalAntecipadoParaPagamentoD1, ref valorDisponivel, ref qtdDeRVExistente,
                            ref filler1, ref totalParcial, ref valorParcialAntecipacao, ref tipoDeSelecaoPorPeriodo, ref periodoDeSelecaoDe, ref periodoDeSelecaoAte, ref DataQueTemRestricao,
                            ref indProduto, ref valorMaxAntecUraSemSenha, ref valorTarifa, ref percentualTarifa, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao);

                        DataQueTemRestricao = DataQueTemRestricao.Replace('\0', new char());

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            ret,
                            dataDoProcessamento,
                            horaDoProcessamento,
                            bancoParaCredito,
                            agenciaParaCredito,
                            contaParaCredito,
                            valorMinimoParaAntecipacao,
                            horaInicioAntecipacaoD0,
                            horaFimAntecipacaoD0,
                            horaInicioAntecipacaoDN,
                            horaFimAntecipacaoDN,
                            taxaDeDesconto,
                            valorTotalDisponivelParaAntecipacao,
                            valorBrutoOriginal,
                            periodoDeSelecaoDe,
                            periodoDeSelecaoAte,
                            mensagemDeErro,
                            dataFimCarencia,
                            valorTotalAntecipadoParaPagamentoD0,
                            valorTotalAntecipadoParaPagamentoD1,
                            valorDisponivel,
                            qtdDeRVExistente,
                            numPdv,
                            funcao,
                            tipoCredito,
                            canal,
                            valorAntecipado,
                            totalParcial,
                            valorParcialAntecipacao,
                            tipoDeSelecaoPorPeriodo,
                            DataQueTemRestricao,
                            indProduto,
                            valorMaxAntecUraSemSenha,
                            valorTarifa,
                            percentualTarifa,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao
                        });

                        ravSaida = new ModRAVAvulsoSaida();

                        if (ret == 05 || ret == 48 || ret == 59 || ret == 71)
                        {
                            ravSaida.Retorno = -1;
                            switch (ret)
                            {
                                case 05:
                                    ravSaida.MsgErro = string.Format("{0}", MENSAGEM_VIP); break;
                                case 48:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "TIPO SELECAO PERIODO INVALIDO", mensagemDeErro); break;
                                case 59:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "ESTABELECIMENTO CATEGORIA SMALL", mensagemDeErro); break;
                                case 71:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "ESTABELECIMENTO CATEGORIA SMALL RIO", mensagemDeErro); break;
                                default:
                                    ravSaida.MsgErro = mensagemDeErro; break;
                            }
                        }
                        else
                        {

                            ravSaida.Agencia = agenciaParaCredito;
                            ravSaida.Banco = bancoParaCredito;
                            ravSaida.Conta = long.Parse(contaParaCredito.ToString());
                            ravSaida.DataProcessamento = dataDoProcessamento;
                            ravSaida.Desconto = taxaDeDesconto;
                            ravSaida.FimCarencia = HISUtils.ConvertToDate(dataFimCarencia);
                            ravSaida.HoraFimD0 = horaFimAntecipacaoD0.ToString();
                            ravSaida.HoraFimDn = horaFimAntecipacaoDN.ToString();
                            ravSaida.HoraIniD0 = horaInicioAntecipacaoD0.ToString();
                            ravSaida.HoraIniDn = horaInicioAntecipacaoDN.ToString();
                            ravSaida.HoraProcessamento = horaDoProcessamento;
                            ravSaida.MsgErro = mensagemDeErro;
                            ravSaida.PeriodoAte = HISUtils.ConvertToDate(periodoDeSelecaoAte);
                            ravSaida.PeriodoDe = HISUtils.ConvertToDate(periodoDeSelecaoDe);
                            if (ret == 3 || ret == 4)
                            {
                                ravSaida.Retorno = 70000;
                            }
                            else
                            {
                                ravSaida.Retorno = (70000 + ret).ToString().ToInt32();
                            }
                            ravSaida.ValorAntecipadoD0 = valorTotalAntecipadoParaPagamentoD0;
                            ravSaida.ValorAntecipadoD1 = valorTotalAntecipadoParaPagamentoD1;
                            ravSaida.ValorBruto = valorBrutoOriginal;
                            ravSaida.ValorDisponivel = valorTotalDisponivelParaAntecipacao;
                            ravSaida.ValorMinimo = valorMinimoParaAntecipacao;
                            ravSaida.ValorOriginal = valorBrutoOriginal;

                            //ravSaida.DadosAntecipado.DataAte = HISUtils.ConvertToDate(fimPeriodoAntecipacao); //Não estão retornando na transação?
                            //ravSaida.DadosAntecipado.DataDe = HISUtils.ConvertToDate(inicioPeriodoAntecipacao); //Não estão retornando na transação?
                            ravSaida.DadosAntecipado.Indicador = totalParcial == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total;
                            ravSaida.DadosAntecipado.IndicadorData = tipoDeSelecaoPorPeriodo == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao;
                            ravSaida.DadosAntecipado.IndicadorProduto = DataQueTemRestricao == "R" ? ElndProdutoAntecipa.Rotativo : DataQueTemRestricao == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                            ravSaida.DadosAntecipado.Valor = valorAAntecipar;
                            ravSaida.DadosAntecipado.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravSaida.DadosAntecipado.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;
                            ravSaida.DadosAntecipado.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravSaida.DadosAntecipado.ValorTarifa = valorTarifa;
                            ravSaida.DadosAntecipado.ValorMaxAntecUraSenha = valorMaxAntecUraSemSenha;


                            foreach (ModuloRAV.FILLER f in filler)
                            {

                                ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito()
                                {
                                    DataCredito = HISUtils.ConvertToDate(f.MA030_DAT_CREDITO),
                                    TaxaEfetiva = f.MA030_PCT_EFETIVA,
                                    TaxaPeriodo = f.MA030_PCT_PERIODO,
                                    ValorLiquido = f.MA030_VALOR_LIQUIDO,
                                    ValorParcelado = f.MA030_VALOR_PARCELADO,
                                    ValorRotativo = f.MA030_VALOR_ROTATIVO
                                });
                            }

                            foreach (ModuloRAV.FILLER1 f in filler1)
                            {
                                ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno()
                                {
                                    DataApresentacao = HISUtils.ConvertToDate(f.MA030_RV_DAT_APRS),
                                    NumeroRAV = f.MA030_RV_NUM_RV,
                                    QuantidadeOC = f.MA030_RV_QTD_OC,
                                    ValorBruto = f.MA030_RV_VAL_BRTO,
                                    ValorLiquido = f.MA030_RV_VAL_LQDO
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
#endif

                Log.GravarLog(EventoLog.FimAgente, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Método que realiza a verificação de RAV Avulso disponível através da URA.
        /// </summary>
        /// <param name="numeroPDV">Número da Entidade</param>
        /// <param name="tipoCredito">Tipo da Antecipação do Crédito
        ///     <example>0: Antecipação D+0</example>
        ///     <example>1: Antecipação D+1</example>
        /// </param>
        /// <returns>Modelo com os dados de saída do RAV Avulso</returns>
        public ModRAVAvulsoSaida VerificarRAVDisponivelURA(Int32 numeroPDV, short tipoCredito)
        {
            using (Logger Log = Logger.IniciarLog("Verificação de RAV Avulso disponível através da URA"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoCredito });

                ModRAVAvulsoSaida ravSaida = null;

#if DEBUG
                ravSaida = new ModRAVAvulsoSaida
                {
                    ValorDisponivel = DateTime.Now.ToString("ddMMyyyy").ToInt32() / 1000m
                };
#else

                try
                {
                    Int16 ret = 0;
                    string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                    string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                    short bancoParaCredito = 0;//MA030_BANCO, 
                    int agenciaParaCredito = 0;//MA030_AGENCIA, 
                    decimal contaParaCredito = 0;//MA030_CONTA,
                    decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                    short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                    short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                    short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                    short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                    ModuloRAV.FILLER[] filler = new ModuloRAV.FILLER[2]; // = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                    decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                    decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                    decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                    string mensagemDeErro = "";//MA030_MSGERRO,
                    string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                    decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                    decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                    decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                    int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                    ModuloRAV.FILLER1[] filler1 = new ModuloRAV.FILLER1[20]; // = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                    string totalParcial = " ";//MA030_CA_IND_ANTEC, 
                    string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                    string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                    string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                    string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                    int numPdv = numeroPDV;//MA030_NUM_PDV, 
                    short funcao = 01;// (short)entradaRAV.Funcao;//MA030_FUNCAO, 
                    short canal = 05;// (short)entradaRAV.Canal;//MA030_CANAL, 
                    decimal valorAAntecipar = 0;//MA030_VALOR_A_ANTECIPAR
                    string indProduto = ""; //MA030_CA_IND_PRODUTO

                    Decimal valorAntecipado = 0; //MA030_VALOR_A_ANTECIPAR

                    Decimal valorMaxAntecUraSemSenha = 0; //MA030_VL_MX_ANT_URA_SNHA

                    Decimal valorTarifa = 0; //MA030_VAL_TRFA
                    Decimal percentualTarifa = 0; //MA030_PCT_TRFA
                    Int16 codigoProdutoAntecipacao = 0; //MA030_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA030_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA030_DSC_PROD_ANTC
                    Decimal valorParcialAntecipacao = 0; //MA030_CA_VAL_ANTEC
                    String inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                    String fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE, 


                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { 
                            numPdv, 
                            funcao, 
                            tipoCredito, 
                            canal, 
                            valorAAntecipar, 
                            totalParcial, 
                            valorParcialAntecipacao, 
                            tipoDeSelecaoPorPeriodo, 
                            periodoDeSelecaoDe, 
                            periodoDeSelecaoAte, 
                            indProduto 
                        });

                        cliente.BMA030(ref numPdv, ref funcao, ref tipoCredito, ref canal, ref valorAntecipado, ref ret, ref dataDoProcessamento, ref horaDoProcessamento, ref bancoParaCredito,
                            ref agenciaParaCredito, ref contaParaCredito, ref valorMinimoParaAntecipacao, ref horaInicioAntecipacaoD0, ref horaFimAntecipacaoD0, ref horaInicioAntecipacaoDN,
                            ref horaFimAntecipacaoDN, ref filler, ref taxaDeDesconto, ref valorTotalDisponivelParaAntecipacao, ref valorBrutoOriginal, ref inicioPeriodoAntecipacao, ref fimPeriodoAntecipacao,
                            ref mensagemDeErro, ref dataFimCarencia, ref valorTotalAntecipadoParaPagamentoD0, ref valorTotalAntecipadoParaPagamentoD1, ref valorDisponivel, ref qtdDeRVExistente,
                            ref filler1, ref totalParcial, ref valorParcialAntecipacao, ref tipoDeSelecaoPorPeriodo, ref periodoDeSelecaoDe, ref periodoDeSelecaoAte, ref DataQueTemRestricao,
                            ref indProduto, ref valorMaxAntecUraSemSenha, ref valorTarifa, ref percentualTarifa, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao);

                        DataQueTemRestricao = DataQueTemRestricao.Replace('\0', new char());

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            ret,
                            dataDoProcessamento,
                            horaDoProcessamento,
                            bancoParaCredito,
                            agenciaParaCredito,
                            contaParaCredito,
                            valorMinimoParaAntecipacao,
                            horaInicioAntecipacaoD0,
                            horaFimAntecipacaoD0,
                            horaInicioAntecipacaoDN,
                            horaFimAntecipacaoDN,
                            taxaDeDesconto,
                            valorTotalDisponivelParaAntecipacao,
                            valorBrutoOriginal,
                            periodoDeSelecaoDe,
                            periodoDeSelecaoAte,
                            mensagemDeErro,
                            dataFimCarencia,
                            valorTotalAntecipadoParaPagamentoD0,
                            valorTotalAntecipadoParaPagamentoD1,
                            valorDisponivel,
                            qtdDeRVExistente,
                            numPdv,
                            funcao,
                            tipoCredito,
                            canal,
                            valorAntecipado,
                            totalParcial,
                            valorParcialAntecipacao,
                            tipoDeSelecaoPorPeriodo,
                            DataQueTemRestricao,
                            indProduto,
                            valorMaxAntecUraSemSenha,
                            valorTarifa,
                            percentualTarifa,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao
                        });

                        ravSaida = new ModRAVAvulsoSaida();

                        if (ret == 05 || ret == 48 || ret == 59 || ret == 71)
                        {
                            ravSaida.Retorno = -1;
                            switch (ret)
                            {
                                case 05:
                                    ravSaida.MsgErro = string.Format("{0}", MENSAGEM_VIP); break;
                                case 48:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "TIPO SELECAO PERIODO INVALIDO", mensagemDeErro); break;
                                case 59:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "ESTABELECIMENTO CATEGORIA SMALL", mensagemDeErro); break;
                                case 71:
                                    ravSaida.MsgErro = string.Format("{0} - {1}", "ESTABELECIMENTO CATEGORIA SMALL RIO", mensagemDeErro); break;
                                default:
                                    ravSaida.MsgErro = mensagemDeErro; break;
                            }
                        }
                        else
                        {

                            ravSaida.Agencia = agenciaParaCredito;
                            ravSaida.Banco = bancoParaCredito;
                            ravSaida.Conta = long.Parse(contaParaCredito.ToString());
                            ravSaida.DataProcessamento = dataDoProcessamento;
                            ravSaida.Desconto = taxaDeDesconto;
                            ravSaida.FimCarencia = HISUtils.ConvertToDate(dataFimCarencia);
                            ravSaida.HoraFimD0 = horaFimAntecipacaoD0.ToString();
                            ravSaida.HoraFimDn = horaFimAntecipacaoDN.ToString();
                            ravSaida.HoraIniD0 = horaInicioAntecipacaoD0.ToString();
                            ravSaida.HoraIniDn = horaInicioAntecipacaoDN.ToString();
                            ravSaida.HoraProcessamento = horaDoProcessamento;
                            ravSaida.MsgErro = mensagemDeErro;
                            ravSaida.PeriodoAte = HISUtils.ConvertToDate(periodoDeSelecaoAte);
                            ravSaida.PeriodoDe = HISUtils.ConvertToDate(periodoDeSelecaoDe);
                            if (ret == 3 || ret == 4)
                            {
                                ravSaida.Retorno = 70000;
                            }
                            else
                            {
                                ravSaida.Retorno = (70000 + ret).ToString().ToInt32();
                            }
                            ravSaida.ValorAntecipadoD0 = valorTotalAntecipadoParaPagamentoD0;
                            ravSaida.ValorAntecipadoD1 = valorTotalAntecipadoParaPagamentoD1;
                            ravSaida.ValorBruto = valorBrutoOriginal;
                            ravSaida.ValorDisponivel = valorTotalDisponivelParaAntecipacao;
                            ravSaida.ValorMinimo = valorMinimoParaAntecipacao;
                            ravSaida.ValorOriginal = valorBrutoOriginal;

                            //ravSaida.DadosAntecipado.DataAte = HISUtils.ConvertToDate(fimPeriodoAntecipacao); //Não estão retornando na transação?
                            //ravSaida.DadosAntecipado.DataDe = HISUtils.ConvertToDate(inicioPeriodoAntecipacao); //Não estão retornando na transação?
                            ravSaida.DadosAntecipado.Indicador = totalParcial == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total;
                            ravSaida.DadosAntecipado.IndicadorData = tipoDeSelecaoPorPeriodo == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao;
                            ravSaida.DadosAntecipado.IndicadorProduto = DataQueTemRestricao == "R" ? ElndProdutoAntecipa.Rotativo : DataQueTemRestricao == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                            ravSaida.DadosAntecipado.Valor = valorAAntecipar;
                            ravSaida.DadosAntecipado.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravSaida.DadosAntecipado.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;
                            ravSaida.DadosAntecipado.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravSaida.DadosAntecipado.ValorTarifa = valorTarifa;
                            ravSaida.DadosAntecipado.ValorMaxAntecUraSenha = valorMaxAntecUraSemSenha;


                            foreach (ModuloRAV.FILLER f in filler)
                            {

                                ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito()
                                {
                                    DataCredito = HISUtils.ConvertToDate(f.MA030_DAT_CREDITO),
                                    TaxaEfetiva = f.MA030_PCT_EFETIVA,
                                    TaxaPeriodo = f.MA030_PCT_PERIODO,
                                    ValorLiquido = f.MA030_VALOR_LIQUIDO,
                                    ValorParcelado = f.MA030_VALOR_PARCELADO,
                                    ValorRotativo = f.MA030_VALOR_ROTATIVO
                                });
                            }

                            foreach (ModuloRAV.FILLER1 f in filler1)
                            {
                                ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno()
                                {
                                    DataApresentacao = HISUtils.ConvertToDate(f.MA030_RV_DAT_APRS),
                                    NumeroRAV = f.MA030_RV_NUM_RV,
                                    QuantidadeOC = f.MA030_RV_QTD_OC,
                                    ValorBruto = f.MA030_RV_VAL_BRTO,
                                    ValorLiquido = f.MA030_RV_VAL_LQDO
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
#endif

                Log.GravarLog(EventoLog.FimAgente, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Método que realiza a consulta de RAV Avulso disponível.
        /// </summary>
        /// <param name="entradaRAV"></param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorAntecipado"></param>
        /// <returns></returns>
        public ModRAVAvulsoSaida ConsultarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorAntecipado)
        {
            using (Logger Log = Logger.IniciarLog("Consulta RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { entradaRAV, numeroPDV, tipoCredito, valorAntecipado });

                //RAVDadosRetorno ravDadosRetorno = new RAVDadosRetorno();
                ModRAVAvulsoSaida ravSaida = new ModRAVAvulsoSaida();


                string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                short bancoParaCredito = 0;//MA030_BANCO, 
                int agenciaParaCredito = 0;//MA030_AGENCIA, 
                decimal contaParaCredito = 0;//MA030_CONTA,
                decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                ModuloRAV.FILLER[] filler = new ModuloRAV.FILLER[2]; //= new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                string mensagemDeErro = "";//MA030_MSGERRO,
                string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                ModuloRAV.FILLER1[] filler1 = new ModuloRAV.FILLER1[20]; // = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 

                string totalParcial = "";//MA030_CA_IND_ANTEC, 
                Decimal valorParcialAntecipacao = default(Decimal); //MA030_CA_VAL_ANTEC,
                decimal valorAAntecipar = 0;

                if (valorAntecipado > 0)
                {
                    totalParcial = "P";//MA030_CA_IND_ANTEC, 
                    valorParcialAntecipacao = valorAntecipado;

                    //if (valorParcialAntecipacao.IndexOf('.') > 0)
                    //{
                    //    valorParcialAntecipacao = (valorParcialAntecipacao.Split('.')[0] + valorParcialAntecipacao.Split('.')[1]).PadLeft(15, '0');
                    //}
                    //else
                    //{
                    //    valorParcialAntecipacao = valorParcialAntecipacao.PadLeft(13, '0') + "00";
                    //}
                    valorAAntecipar = valorAntecipado;//MA030_VALOR_A_ANTECIPAR
                }

                string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                int numPdv = numeroPDV;//MA030_NUM_PDV, 
                short funcao = 01;//MA030_FUNCAO, 
                short tipCredito = (short)tipoCredito; //MA030_TIP_CREDITO, 
                short ret = default(short);

                string indProduto = entradaRAV.IndProduto.ToString();

                if (entradaRAV != null)
                    tipCredito = (short)entradaRAV.DiasCredito;

                short canal = 06;//MA030_CANAL, 

                Decimal valorMaxAntecUraSemSenha = 0; //MA030_VL_MX_ANT_URA_SNHA
                Decimal valorTarifa = 0; //MA030_VAL_TRFA
                Decimal percentualTarifa = 0; //MA030_PCT_TRFA
                Int16 codigoProdutoAntecipacao = 0; //MA030_COD_PROD_ANTC
                String nomeProdutoAntecipacao = String.Empty; //MA030_NOM_PROD_ANTC
                String descricaoProdutoAntecipacao = String.Empty; //MA030_DSC_PROD_ANTC

                try
                {
                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { 
                            numeroPDV, 
                            funcao, 
                            tipCredito, 
                            canal, 
                            valorAAntecipar, 
                            totalParcial, 
                            valorParcialAntecipacao, 
                            tipoDeSelecaoPorPeriodo, 
                            periodoDeSelecaoDe, 
                            periodoDeSelecaoAte, 
                            indProduto 
                        });

                        cliente.BMA030(ref numPdv, ref funcao, ref tipCredito, ref canal, ref valorAntecipado, ref ret, ref dataDoProcessamento, ref horaDoProcessamento, ref bancoParaCredito,
                          ref agenciaParaCredito, ref contaParaCredito, ref valorMinimoParaAntecipacao, ref horaInicioAntecipacaoD0, ref horaFimAntecipacaoD0, ref horaInicioAntecipacaoDN,
                          ref horaFimAntecipacaoDN, ref filler, ref taxaDeDesconto, ref valorTotalDisponivelParaAntecipacao, ref valorBrutoOriginal, ref inicioPeriodoAntecipacao, ref fimPeriodoAntecipacao,
                          ref mensagemDeErro, ref dataFimCarencia, ref valorTotalAntecipadoParaPagamentoD0, ref valorTotalAntecipadoParaPagamentoD1, ref valorDisponivel, ref qtdDeRVExistente,
                          ref filler1, ref totalParcial, ref valorParcialAntecipacao, ref tipoDeSelecaoPorPeriodo, ref periodoDeSelecaoDe, ref periodoDeSelecaoAte, ref DataQueTemRestricao,
                          ref indProduto, ref valorMaxAntecUraSemSenha, ref valorTarifa, ref percentualTarifa, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            ret,
                            dataDoProcessamento,
                            horaDoProcessamento,
                            bancoParaCredito,
                            agenciaParaCredito,
                            contaParaCredito,
                            valorMinimoParaAntecipacao,
                            horaInicioAntecipacaoD0,
                            horaFimAntecipacaoD0,
                            horaInicioAntecipacaoDN,
                            horaFimAntecipacaoDN,
                            taxaDeDesconto,
                            valorTotalDisponivelParaAntecipacao,
                            valorBrutoOriginal,
                            periodoDeSelecaoDe,
                            periodoDeSelecaoAte,
                            mensagemDeErro,
                            dataFimCarencia,
                            valorTotalAntecipadoParaPagamentoD0,
                            valorTotalAntecipadoParaPagamentoD1,
                            valorDisponivel,
                            qtdDeRVExistente,
                            numPdv,
                            funcao,
                            tipCredito,
                            canal,
                            valorAntecipado,
                            totalParcial,
                            valorParcialAntecipacao,
                            tipoDeSelecaoPorPeriodo,
                            DataQueTemRestricao,
                            indProduto,
                            valorMaxAntecUraSemSenha,
                            valorTarifa,
                            percentualTarifa,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao
                        });

                        if (ret > 0)
                        {
                            ravSaida.Retorno = HISUtils.ConvertToInt((70000 + ret).ToString());
                        }
                        else
                        {
                            ravSaida.Retorno = HISUtils.ConvertToInt(ret.ToString());
                        }

                        ravSaida.Agencia = agenciaParaCredito;
                        ravSaida.Banco = bancoParaCredito;
                        ravSaida.Conta = long.Parse(contaParaCredito.ToString());
                        ravSaida.DataProcessamento = dataDoProcessamento;
                        ravSaida.Desconto = taxaDeDesconto;
                        ravSaida.FimCarencia = HISUtils.ConvertToDate(dataFimCarencia);
                        ravSaida.HoraFimD0 = horaFimAntecipacaoD0.ToString();
                        ravSaida.HoraFimDn = horaFimAntecipacaoDN.ToString();
                        ravSaida.HoraIniD0 = horaInicioAntecipacaoD0.ToString();
                        ravSaida.HoraIniDn = horaInicioAntecipacaoDN.ToString();
                        ravSaida.HoraProcessamento = horaDoProcessamento;
                        ravSaida.MsgErro = mensagemDeErro;
                        ravSaida.PeriodoAte = HISUtils.ConvertToDate(periodoDeSelecaoAte);
                        ravSaida.PeriodoDe = HISUtils.ConvertToDate(periodoDeSelecaoDe);

                        ravSaida.ValorAntecipadoD0 = valorTotalAntecipadoParaPagamentoD0;
                        ravSaida.ValorAntecipadoD1 = valorTotalAntecipadoParaPagamentoD1;
                        ravSaida.ValorBruto = valorBrutoOriginal;
                        ravSaida.ValorDisponivel = valorTotalDisponivelParaAntecipacao;
                        ravSaida.ValorMinimo = valorMinimoParaAntecipacao;
                        ravSaida.ValorOriginal = valorBrutoOriginal;

                        ravSaida.DadosAntecipado.DataAte = HISUtils.ConvertToDate(fimPeriodoAntecipacao); //Não está retornando na transação?
                        ravSaida.DadosAntecipado.DataDe = HISUtils.ConvertToDate(inicioPeriodoAntecipacao); //Não está retornando na transação?
                        ravSaida.DadosAntecipado.Indicador = totalParcial == "P" ? ElndAntecipa.Parcial : ElndAntecipa.Total;
                        ravSaida.DadosAntecipado.IndicadorData = tipoDeSelecaoPorPeriodo == "V" ? ElndDataAntecipa.Vencimento : ElndDataAntecipa.Apresentacao;
                        ravSaida.DadosAntecipado.IndicadorProduto = DataQueTemRestricao == "R" ? ElndProdutoAntecipa.Rotativo : DataQueTemRestricao == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                        ravSaida.DadosAntecipado.Valor = valorAAntecipar;

                        ravSaida.DadosAntecipado.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                        ravSaida.DadosAntecipado.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;
                        ravSaida.DadosAntecipado.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                        ravSaida.DadosAntecipado.ValorTarifa = valorTarifa;
                        ravSaida.DadosAntecipado.ValorMaxAntecUraSenha = valorMaxAntecUraSemSenha;

                        foreach (ModuloRAV.FILLER f in filler)
                        {
                            ravSaida.DadosParaCredito.Add(new ModRAVAvulsoCredito()
                            {
                                DataCredito = HISUtils.ConvertToDate(f.MA030_DAT_CREDITO),
                                TaxaEfetiva = f.MA030_PCT_EFETIVA,
                                TaxaPeriodo = f.MA030_PCT_PERIODO,
                                ValorLiquido = f.MA030_VALOR_LIQUIDO,
                                ValorParcelado = f.MA030_VALOR_PARCELADO,
                                ValorRotativo = f.MA030_VALOR_ROTATIVO
                            });
                        }

                        foreach (ModuloRAV.FILLER1 f in filler1)
                        {
                            ravSaida.TabelaRAVs.Add(new ModRAVAvulsoRetorno()
                            {
                                DataApresentacao = HISUtils.ConvertToDate(f.MA030_RV_DAT_APRS),
                                NumeroRAV = f.MA030_RV_NUM_RV,
                                QuantidadeOC = f.MA030_RV_QTD_OC,
                                ValorBruto = f.MA030_RV_VAL_BRTO,
                                ValorLiquido = f.MA030_RV_VAL_LQDO
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { ravSaida });
                return ravSaida;
            }
        }

        /// <summary>
        /// Método que realiza a efetivação de RAV Avulso disponível.
        /// </summary>
        /// <param name="entradaRAV"></param>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoCredito"></param>
        /// <param name="valorSolicitado"></param>
        /// <returns></returns>
        public Int32 EfetuarRAVAvulso(ModRAVAvulsoEntrada entradaRAV, Int32 numeroPDV, Int32 tipoCredito, Decimal valorSolicitado)
        {
            using (Logger Log = Logger.IniciarLog("Efetivação de RAV Avulso disponível"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { entradaRAV, numeroPDV, tipoCredito, valorSolicitado });

                //RAVDadosRetorno ravDadosRetorno = new RAVDadosRetorno();
                int codigoRetorno = 0;

                try
                {
                    string dataDoProcessamento = "";//MA030_DAT_PROCESSAMENTO, 
                    string horaDoProcessamento = "";//MA030_HOR_PROCESSAMENTO, 
                    short bancoParaCredito = 0;//MA030_BANCO, 
                    int agenciaParaCredito = 0;//MA030_AGENCIA, 
                    decimal contaParaCredito = 0;//MA030_CONTA,
                    decimal valorMinimoParaAntecipacao = 0;//MA030_VALOR_MINIMO,
                    short horaInicioAntecipacaoD0 = 0;//MA030_HORA_INI_D0, 
                    short horaFimAntecipacaoD0 = 0;//MA030_HORA_FIM_D0, 
                    short horaInicioAntecipacaoDN = 0;//MA030_HORA_INI_DN,
                    short horaFimAntecipacaoDN = 0; //MA030_HORA_FIM_DN,
                    ModuloRAV.FILLER[] filler = new ModuloRAV.FILLER[2]; // = new List<FILLER>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER> FILLER,
                    decimal taxaDeDesconto = 0; //MA030_PCT_DESCONTO, 
                    decimal valorTotalDisponivelParaAntecipacao = 0;//MA030_VALOR_BRUTO, 
                    decimal valorBrutoOriginal = 0; //MA030_VALOR_ORIG, 
                    string inicioPeriodoAntecipacao = "";//MA030_DAT_PERIODO_DE, 
                    string fimPeriodoAntecipacao = "";//MA030_DAT_PERIODO_ATE,  
                    string mensagemDeErro = "";//MA030_MSGERRO,
                    string dataFimCarencia = ""; //MA030_DATA_FIM_CARENCIA,
                    decimal valorTotalAntecipadoParaPagamentoD0 = 0;//MA030_VALOR_ANTEC_D0, 
                    decimal valorTotalAntecipadoParaPagamentoD1 = 0;//MA030_VALOR_ANTEC_D1,
                    decimal valorDisponivel = 0;//MA030_VALOR_DISP_ANTEC, 
                    int qtdDeRVExistente = 0; //MA030_RV_QTD_RV, 
                    ModuloRAV.FILLER1[] filler1 = new ModuloRAV.FILLER1[20]; // = new List<FILLER1>();//System.Collections.Generic.List<Redecard.PN.RAV.Dados.ServicosRAV.FILLER1> FILLER1, 
                    string totalParcial = "";//MA030_CA_IND_ANTEC, 
                    Decimal valorParcialAntecipacao = default(Decimal); //MA030_CA_VAL_ANTEC,
                    decimal valorAAntecipar = 0;
                    string indProduto = entradaRAV.IndProduto.ToString();

                    if (entradaRAV.ValorAntecipado > 0)
                    {
                        totalParcial = "P";//MA030_CA_IND_ANTEC, 
                        valorParcialAntecipacao = entradaRAV.ValorAntecipado;

                        valorAAntecipar = entradaRAV.ValorAntecipado;//MA030_VALOR_A_ANTECIPAR
                    }

                    string tipoDeSelecaoPorPeriodo = ""; //MA030_CA_IND_DATA_ANTEC, 
                    string periodoDeSelecaoDe = "";//MA030_CA_PER_DATA_DE, 
                    string periodoDeSelecaoAte = "";//MA030_CA_PER_DATA_ATE, 
                    string DataQueTemRestricao = "";//MA030_DAT_RESTRICAO, 
                    int numPdv = numeroPDV;//MA030_NUM_PDV, 
                    short funcao = 02;//MA030_FUNCAO, 
                    short canal = 06;//MA030_CANAL,   
                    short tipCredito = (short)tipoCredito; //MA030_TIP_CREDITO, 
                    short ret = default(short);

                    Decimal valorMaxAntecUraSemSenha = 0; //MA030_VL_MX_ANT_URA_SNHA
                    Decimal valorTarifa = 0; //MA030_VAL_TRFA
                    Decimal percentualTarifa = 0; //MA030_PCT_TRFA
                    Int16 codigoProdutoAntecipacao = 0; //MA030_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA030_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA030_DSC_PROD_ANTC

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {

                        Log.GravarLog(EventoLog.ChamadaHIS, new { 
                            numPdv, 
                            funcao, 
                            tipCredito, 
                            canal, 
                            valorAAntecipar, 
                            totalParcial, 
                            valorParcialAntecipacao, 
                            tipoDeSelecaoPorPeriodo, 
                            periodoDeSelecaoDe, 
                            periodoDeSelecaoAte, 
                            indProduto });


                        cliente.BMA030(ref numPdv, ref funcao, ref tipCredito, ref canal, ref valorAAntecipar, ref ret, ref dataDoProcessamento, ref horaDoProcessamento, ref bancoParaCredito,
                          ref agenciaParaCredito, ref contaParaCredito, ref valorMinimoParaAntecipacao, ref horaInicioAntecipacaoD0, ref horaFimAntecipacaoD0, ref horaInicioAntecipacaoDN,
                          ref horaFimAntecipacaoDN, ref filler, ref taxaDeDesconto, ref valorTotalDisponivelParaAntecipacao, ref valorBrutoOriginal, ref inicioPeriodoAntecipacao, ref fimPeriodoAntecipacao,
                          ref mensagemDeErro, ref dataFimCarencia, ref valorTotalAntecipadoParaPagamentoD0, ref valorTotalAntecipadoParaPagamentoD1, ref valorDisponivel, ref qtdDeRVExistente,
                          ref filler1, ref totalParcial, ref valorParcialAntecipacao, ref tipoDeSelecaoPorPeriodo, ref periodoDeSelecaoDe, ref periodoDeSelecaoAte, ref DataQueTemRestricao,
                          ref indProduto, ref valorMaxAntecUraSemSenha, ref valorTarifa, ref percentualTarifa, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            ret,
                            dataDoProcessamento,
                            horaDoProcessamento,
                            bancoParaCredito,
                            agenciaParaCredito,
                            contaParaCredito,
                            valorMinimoParaAntecipacao,
                            horaInicioAntecipacaoD0,
                            horaFimAntecipacaoD0,
                            horaInicioAntecipacaoDN,
                            horaFimAntecipacaoDN,
                            filler,
                            taxaDeDesconto,
                            valorTotalDisponivelParaAntecipacao,
                            valorBrutoOriginal,
                            periodoDeSelecaoDe,
                            periodoDeSelecaoAte,
                            mensagemDeErro,
                            dataFimCarencia,
                            valorTotalAntecipadoParaPagamentoD0,
                            valorTotalAntecipadoParaPagamentoD1,
                            valorDisponivel,
                            qtdDeRVExistente,
                            filler1,
                            totalParcial,
                            valorParcialAntecipacao,
                            tipoDeSelecaoPorPeriodo,
                            DataQueTemRestricao,
                            indProduto
                        });

                        if (ret > 0)
                        {
                            codigoRetorno = 70000 + ret;
                        }
                        else
                        {
                            codigoRetorno = ret;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { codigoRetorno });
                return codigoRetorno;
            }
        }
        #endregion

        #region Transação MA61 - RAV Automatico
        /// <summary>
        /// Método que realiza a consulta do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        public ModRAVAutomatico ConsultarRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade });

                ModRAVAutomatico ravDados = null;

                try
                {
                    string codigoFuncao = "21"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString(); //"R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 12; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); //"D";//MA061_COD_PERIODICIDADE, 
                    string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                    string diaAntecipacao = "";//MA061_DIA_ANTC, 
                    string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "";//MA061_IND_FULL



                    FILLER2[] bandeiras = new FILLER2[100]; //FILLER OCCURS 100


                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida

                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        if (codigoRetorno == 0)
                        {
                            if (numPDV != 0)
                            {
                                ravDados = new ModRAVAutomatico();

                                if (!Object.Equals(bandeiras, null))
                                {
                                    ravDados.IndFull = indFull;
                                    ravDados.Bandeiras = PreencherBandeiras(bandeiras);
                                }

                                ravDados.CodRetorno = codigoRetorno;
                                ravDados.MensagemRetorno = mensagemDeRetorno;
                                ravDados.CodigoProduto = codigoProduto;
                                ravDados.CodMotivoExclusao = codigoMotivoExclusao;
                                ravDados.CodSituacao = codigoSituacaoPendencia;
                                ravDados.CodVenda = codigoCanalDeVenda;

                                if (!string.IsNullOrEmpty(dataDeContratacaoProduto))
                                {
                                    ravDados.DataContrato = HISUtils.ConvertToDate(dataDeContratacaoProduto);
                                }
                                else
                                {
                                    ravDados.DataContrato = null;
                                }

                                ravDados.DataIniEstoq = HISUtils.ConvertToDate(dataBaseProxAntec);
                                ravDados.DataVigenciaFim = HISUtils.ConvertToDate(dataFimVigencia);
                                ravDados.DataVigenciaIni = HISUtils.ConvertToDate(dataInicioVigencia);
                                ravDados.DescMotivoExclusao = descricaoMotivoExclusao;
                                ravDados.DiaAntecipacao = diaAntecipacao;
                                ravDados.IndAnteEstoq = indicadorAnteciparEstoque == "S" ? ElndAntecEstoq.Sim : ElndAntecEstoq.Nao;
                                ravDados.IndContratoPortal = Convert.ToChar(codigoCanalDeVenda);
                                ravDados.IndPRFComercial = Convert.ToChar(usuarioPertenceAreaComecial);
                                ravDados.NomeContato = nomeDoContatoDoEstabelecimento;
                                ravDados.NumeroPDV = numeroPDV;
                                ravDados.NumeroPDVRef = numeroPvReferencia;
                                ravDados.NumParcelaFim = numeroDParcelasFinal;
                                ravDados.NumParcelaIni = numeroDParcelasFinal;
                                switch (codigoPeriodicidade)
                                {
                                    case "D":
                                        { ravDados.Periodicidade = EPeriodicidade.Diario; break; }
                                    case "S":
                                        { ravDados.Periodicidade = EPeriodicidade.Semanal; break; }
                                    case "Q":
                                        { ravDados.Periodicidade = EPeriodicidade.Quinzenal; break; }
                                    case "M":
                                        { ravDados.Periodicidade = EPeriodicidade.Mensal; break; }
                                }
                                ravDados.QtdeDiasCancelamento = qtdDeDiasDeAgendamentoParaExclusao;

                                ravDados.TipoRAV = tipoRvASerAntecipado == "R" ? ElndProdutoAntecipa.Rotativo : tipoRvASerAntecipado == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                                ravDados.ValorMinimo = valorMinimoAntecipar;
                                ravDados.Funcao = codigoFuncao == "11" ? ECodFuncao.Simulacao : codigoFuncao == "21" ? ECodFuncao.Consultar : ECodFuncao.Efetivar;

                                ravDados.DadosRetorno.CodCategoria = HISUtils.ConvertToInt32(codigoCategoriaTaxa);
                                ravDados.DadosRetorno.CodOpidAlteracao = HISUtils.ConvertToInt32(codigoOperadorDeAlteracao);
                                ravDados.DadosRetorno.CodOpidAutorizacao = HISUtils.ConvertToInt32(codigoOperacaoAutorizacao);
                                ravDados.DadosRetorno.CodRetorno = 70100 + codigoRetorno;
                                ravDados.DadosRetorno.CodSituacaoPendente = codigoSituacaoPendente;
                                ravDados.DadosRetorno.CpfCnpj = numeroCNPJCPF.ToString();
                                ravDados.DadosRetorno.DataAgendaExclusao = HISUtils.ConvertToDate(dataAgendadaParaExclusao);
                                ravDados.DadosRetorno.DataAlteracao = HISUtils.ConvertToDate(dataAlteracao);
                                ravDados.DadosRetorno.DataAutorizacao = HISUtils.ConvertToDate(dataAutorizacao);
                                ravDados.DadosRetorno.DataBaseAntecipacao = HISUtils.ConvertToDate(dataBaseAntcEstoque);
                                ravDados.DadosRetorno.DataFimFidelizacao = HISUtils.ConvertToDate(dataFimFidelizacao);
                                ravDados.DadosRetorno.DataIniFidelizacao = HISUtils.ConvertToDate(dataInicioFidelizacao);
                                ravDados.DadosRetorno.DataProximaAntecipacao = HISUtils.ConvertToDate(dataDaProximaAntecipacao);
                                ravDados.DadosRetorno.DescCategoria = nomeDaCategoria;
                                ravDados.DadosRetorno.DescSituacaoCategoria = descricaoPendenciaDaTaxa;
                                ravDados.DadosRetorno.Estabelecimento = nomeEstabelecimento;
                                ravDados.DadosRetorno.HoraAlteracao = horaAlteracao;
                                ravDados.DadosRetorno.HoraAutorizacao = horaAutorizacao;
                                ravDados.DadosRetorno.IndBloqueio = !string.IsNullOrEmpty(indicadorDeBloqueioPorFidelizacao) && indicadorDeBloqueioPorFidelizacao.Length == 1 ? Convert.ToChar(indicadorDeBloqueioPorFidelizacao) : ' ';
                                ravDados.DadosRetorno.MsgRetorno = mensagemDeRetorno;
                                ravDados.DadosRetorno.NumMatrix = numeroDaMatriz;
                                ravDados.DadosRetorno.TaxaCategoria = taxaCategoria;
                                ravDados.DadosRetorno.TaxaFidelizacao = taxaDeFidelizacao;
                                ravDados.DadosRetorno.DescricaoBandeiraSelecionada = descricaoBandeiraSelecionada;
                                ravDados.DadosRetorno.ValorTarifaAntecipacaoRAV = valorTarifaAntecipacaoRAV;
                                ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                                ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                                ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                                switch (diaSemanaAntecipacao)
                                {
                                    case "SEG":
                                        { ravDados.DiaSemana = EDiaSemana.Segunda; break; }
                                    case "TER":
                                        { ravDados.DiaSemana = EDiaSemana.Terca; break; }
                                    case "QUA":
                                        { ravDados.DiaSemana = EDiaSemana.Quarta; break; }
                                    case "QUI":
                                        { ravDados.DiaSemana = EDiaSemana.Quinta; break; }
                                    case "SEX":
                                        { ravDados.DiaSemana = EDiaSemana.Sexta; break; }
                                    default:
                                        { break; }
                                }
                            }
                        }
                        else
                        {
                            ravDados = new ModRAVAutomatico();

                            if (!Object.Equals(bandeiras, null))
                            {
                                ravDados.IndFull = indFull;
                                ravDados.Bandeiras = PreencherBandeiras(bandeiras);
                            }

                            ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                            ravDados.CodRetorno = 70100 + codigoRetorno;
                            ravDados.MensagemRetorno = mensagemDeRetorno;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { ravDados });
                return ravDados;
            }
        }

        /// <summary>
        /// Método que realiza a consulta de Bandeiras de  RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        private FILLER2[] ConsultarBandeirasRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de Bandeiras RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade });

                FILLER2[] bandeiras = new FILLER2[100]; //FILLER OCCURS 100
                try
                {
                    string codigoFuncao = "21"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString(); //"R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 12; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); //"D";//MA061_COD_PERIODICIDADE, 
                    string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                    string diaAntecipacao = "";//MA061_DIA_ANTC, 
                    string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "";//MA061_IND_FULL


                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { bandeiras });
                return bandeiras;
            }
        }

        /// <summary>
        /// Método que realiza a consulta personalizada do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <param name="valor"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public ModRAVAutomatico ConsultarRAVAutomaticoPersonalizado(Int32 numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String _diaSemana, String _diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Consulta personalizada de RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim });
                ModRAVAutomatico ravDados = null;

                try
                {
                    string codigoFuncao = "11"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString(); //"R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 12; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); // "D";//MA061_COD_PERIODICIDADE, 
                    //string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                    //string diaAntecipacao = "";//MA061_DIA_ANTC, 
                    string diaSemanaAntecipacao = !String.IsNullOrEmpty(_diaSemana) ? _diaSemana : String.Empty;// MA061_DIA_SEMANA, 
                    string diaAntecipacao = !String.IsNullOrEmpty(_diaAntecipacao) ? _diaAntecipacao : String.Empty;//MA061_DIA_ANTC, 
                    string dataInicioVigencia = dataIni.ToString("dd.MM.yyyy"); //"";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = dataFim.ToString("dd.MM.yyyy"); // "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = valor; //0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "";//MA061_IND_FULL
                    FILLER2[] bandeiras = new FILLER2[100];

                    Int16 i = 0;
                    Int16 totalSel = 0;

                    if (!String.IsNullOrEmpty(sBandeiras))
                    {
                        foreach (string bandeira in sBandeiras.Split(';'))
                        {
                            FILLER2 filler2 = new FILLER2();
                            filler2.MA061_COD_BNDR = short.Parse(bandeira.Split('#')[0]);
                            filler2.MA061_DSC_BNDR = bandeira.Split('#')[1];

                            if (bandeira.Split('#')[2] == "S")
                            {
                                totalSel++;
                                filler2.MA061_IND_SEL = bandeira.Split('#')[2];
                            }

                            bandeiras[i] = filler2;
                            i++;
                        }
                        if (sBandeiras.Split(';').Count() == totalSel)
                        {
                            indFull = "S";
                        }
                    }

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        if (codigoRetorno == 0)
                        {
                            ravDados = new ModRAVAutomatico();

                            ravDados.CodigoProduto = codigoProduto;
                            ravDados.CodMotivoExclusao = codigoMotivoExclusao;
                            ravDados.CodSituacao = codigoSituacaoPendencia;
                            ravDados.CodVenda = codigoCanalDeVenda;
                            ravDados.DataContrato = HISUtils.ConvertToDate(dataDeContratacaoProduto);
                            ravDados.DataIniEstoq = HISUtils.ConvertToDate(dataBaseProxAntec);
                            ravDados.DataVigenciaFim = HISUtils.ConvertToDate(dataFimVigencia);
                            ravDados.DataVigenciaIni = HISUtils.ConvertToDate(dataInicioVigencia);
                            ravDados.DescMotivoExclusao = descricaoMotivoExclusao;
                            ravDados.DiaAntecipacao = diaAntecipacao;
                            ravDados.IndAnteEstoq = indicadorAnteciparEstoque == "S" ? ElndAntecEstoq.Sim : ElndAntecEstoq.Nao;
                            ravDados.IndContratoPortal = Convert.ToChar(codigoCanalDeVenda);
                            ravDados.IndPRFComercial = Convert.ToChar(usuarioPertenceAreaComecial);
                            ravDados.NomeContato = nomeDoContatoDoEstabelecimento;
                            ravDados.NumeroPDV = numeroPDV;
                            ravDados.NumeroPDVRef = numeroPvReferencia;
                            ravDados.NumParcelaFim = numeroDParcelasFinal;
                            ravDados.NumParcelaIni = numeroDParcelasFinal;
                            switch (codigoPeriodicidade)
                            {
                                case "D":
                                    { ravDados.Periodicidade = EPeriodicidade.Diario; break; }
                                case "S":
                                    { ravDados.Periodicidade = EPeriodicidade.Semanal; break; }
                                case "Q":
                                    { ravDados.Periodicidade = EPeriodicidade.Quinzenal; break; }
                                case "M":
                                    { ravDados.Periodicidade = EPeriodicidade.Mensal; break; }
                            }
                            ravDados.QtdeDiasCancelamento = qtdDeDiasDeAgendamentoParaExclusao;

                            ravDados.IndFull = indFull;
                            ravDados.Bandeiras = PreencherBandeiras(bandeiras);

                            ravDados.TipoRAV = tipoRvASerAntecipado == "R" ? ElndProdutoAntecipa.Rotativo : tipoRvASerAntecipado == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                            ravDados.ValorMinimo = valorMinimoAntecipar;
                            ravDados.Funcao = codigoFuncao == "11" ? ECodFuncao.Simulacao : codigoFuncao == "21" ? ECodFuncao.Consultar : ECodFuncao.Efetivar;

                            ravDados.DadosRetorno.CodCategoria = HISUtils.ConvertToInt32(codigoCategoriaTaxa);
                            ravDados.DadosRetorno.CodOpidAlteracao = HISUtils.ConvertToInt32(codigoOperadorDeAlteracao);
                            ravDados.DadosRetorno.CodOpidAutorizacao = HISUtils.ConvertToInt32(codigoOperacaoAutorizacao);
                            ravDados.DadosRetorno.CodRetorno = codigoRetorno;
                            ravDados.DadosRetorno.CodSituacaoPendente = codigoSituacaoPendente;
                            ravDados.DadosRetorno.CpfCnpj = numeroCNPJCPF.ToString();
                            ravDados.DadosRetorno.DataAgendaExclusao = HISUtils.ConvertToDate(dataAgendadaParaExclusao);
                            ravDados.DadosRetorno.DataAlteracao = HISUtils.ConvertToDate(dataAlteracao);
                            ravDados.DadosRetorno.DataAutorizacao = HISUtils.ConvertToDate(dataAutorizacao);
                            ravDados.DadosRetorno.DataBaseAntecipacao = HISUtils.ConvertToDate(dataBaseAntcEstoque);
                            ravDados.DadosRetorno.DataFimFidelizacao = HISUtils.ConvertToDate(dataFimFidelizacao);
                            ravDados.DadosRetorno.DataIniFidelizacao = HISUtils.ConvertToDate(dataInicioFidelizacao);
                            ravDados.DadosRetorno.DataProximaAntecipacao = HISUtils.ConvertToDate(dataDaProximaAntecipacao);
                            ravDados.DadosRetorno.DescCategoria = nomeDaCategoria;
                            ravDados.DadosRetorno.DescSituacaoCategoria = descricaoPendenciaDaTaxa;
                            ravDados.DadosRetorno.Estabelecimento = nomeEstabelecimento;
                            ravDados.DadosRetorno.HoraAlteracao = horaAlteracao;
                            ravDados.DadosRetorno.HoraAutorizacao = horaAutorizacao;
                            ravDados.DadosRetorno.IndBloqueio = !string.IsNullOrEmpty(indicadorDeBloqueioPorFidelizacao) ? Convert.ToChar(indicadorDeBloqueioPorFidelizacao) : ' ';
                            ravDados.DadosRetorno.MsgRetorno = mensagemDeRetorno;
                            ravDados.DadosRetorno.NumMatrix = numeroDaMatriz;
                            ravDados.DadosRetorno.TaxaCategoria = taxaCategoria;
                            ravDados.DadosRetorno.TaxaFidelizacao = taxaDeFidelizacao;

                            ravDados.DadosRetorno.DescricaoBandeiraSelecionada = descricaoBandeiraSelecionada;
                            ravDados.DadosRetorno.ValorTarifaAntecipacaoRAV = valorTarifaAntecipacaoRAV;
                            ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                            switch (diaSemanaAntecipacao)
                            {
                                case "SEG":
                                    { ravDados.DiaSemana = EDiaSemana.Segunda; break; }
                                case "TER":
                                    { ravDados.DiaSemana = EDiaSemana.Terca; break; }
                                case "QUA":
                                    { ravDados.DiaSemana = EDiaSemana.Quarta; break; }
                                case "QUI":
                                    { ravDados.DiaSemana = EDiaSemana.Quinta; break; }
                                case "SEX":
                                    { ravDados.DiaSemana = EDiaSemana.Sexta; break; }
                                default:
                                    { break; }
                            }
                        }
                        else
                        {

                            ravDados = new ModRAVAutomatico();

                            ravDados.IndFull = indFull;
                            ravDados.Bandeiras = PreencherBandeiras(bandeiras);

                            ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                            ravDados.CodRetorno = 70100 + codigoRetorno;
                            ravDados.MensagemRetorno = mensagemDeRetorno;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { ravDados });
                return ravDados;
            }
        }

        /// <summary>
        /// Método que realiza a efetivação do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public int EfetuarRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade, string sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetuação de RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade });


                try
                {
                    string codigoFuncao = "12"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString(); // "R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 12; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); //"D";//MA061_COD_PERIODICIDADE, 
                    string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                    string diaAntecipacao = "";//MA061_DIA_ANTC, 
                    string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string operadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string operacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "N";//MA061_IND_FULL
                    FILLER2[] bandeiras = new FILLER2[100];
                    Int16 i = 0;
                    Int16 totalSel = 0;

                    foreach (string bandeira in sBandeiras.Split(';'))
                    {
                        FILLER2 filler2 = new FILLER2();
                        filler2.MA061_COD_BNDR = short.Parse(bandeira.Split('#')[0]);
                        filler2.MA061_DSC_BNDR = bandeira.Split('#')[1];

                        if (bandeira.Split('#')[2] == "S")
                        {
                            totalSel++;
                            filler2.MA061_IND_SEL = bandeira.Split('#')[2];
                        }

                        bandeiras[i] = filler2;
                        i++;
                    }

                    if (sBandeiras.Split(';').Count() == totalSel)
                    {
                        indFull = "S";
                    }

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            operadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            operacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref operadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref operacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            operadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            operacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });
                    }

                    Int32 retorno = 0;
                    if (codigoRetorno == 0)
                    {
                        retorno = int.Parse(codigoRetorno.ToString());
                    }
                    else
                    {
                        retorno = 70100 + int.Parse(codigoRetorno.ToString());
                    }

                    Log.GravarLog(EventoLog.FimAgente, new { retorno });
                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Método que realiza a efetivação personalizada do RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="periodicidade"></param>
        /// <param name="valor"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Int32 EfetuarRAVAutomaticoPersonalizado(Int32 numeroPDV, char tipoVenda, char periodicidade, decimal valor, DateTime dataIni, DateTime dataFim, String _diaSemana, String _diaAntecipacao, String sBandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Efetivação personalizada do RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade, valor, dataIni, dataFim });

                try
                {
                    string codigoFuncao = "12"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString(); // "R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 3; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); //"D";//MA061_COD_PERIODICIDADE, 
                    string diaSemanaAntecipacao = !String.IsNullOrEmpty(_diaSemana) ? _diaSemana : String.Empty;// MA061_DIA_SEMANA, 
                    string diaAntecipacao = !String.IsNullOrEmpty(_diaAntecipacao) ? _diaAntecipacao : String.Empty;//MA061_DIA_ANTC, 
                    string dataInicioVigencia = dataIni.ToString("dd.MM.yyyy"); //"";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = dataFim.ToString("dd.MM.yyyy"); // "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = valor;// 0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string operadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string operacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "N";//MA061_IND_FULL
                    FILLER2[] bandeiras = new FILLER2[100];
                    Int16 i = 0;
                    Int16 totalSel = 0;

                    if (!String.IsNullOrEmpty(sBandeiras))
                    {
                        foreach (string bandeira in sBandeiras.Split(';'))
                        {
                            FILLER2 filler2 = new FILLER2();
                            filler2.MA061_COD_BNDR = short.Parse(bandeira.Split('#')[0]);
                            filler2.MA061_DSC_BNDR = bandeira.Split('#')[1];

                            if (bandeira.Split('#')[2] == "S")
                            {
                                totalSel++;
                                filler2.MA061_IND_SEL = bandeira.Split('#')[2];
                            }

                            bandeiras[i] = filler2;
                            i++;
                        }
                        if (sBandeiras.Split(';').Count() == totalSel)
                        {
                            indFull = "S";
                        }
                    }

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            operadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            operacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref operadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref operacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoRetorno,
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            operadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            operacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        Int32 retorno = 0;
                        if (codigoRetorno == 0)
                        {
                            retorno = int.Parse(codigoRetorno.ToString());
                        }
                        else
                        {
                            retorno = 70100 + int.Parse(codigoRetorno.ToString());
                        }

                        Log.GravarLog(EventoLog.FimAgente, new { retorno });
                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Método que simula a inclusão de um RAV Automático.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVAutomatico SimularRAVAutomatico(Int32 numeroPDV, char tipoVenda, char periodicidade)
        {
            using (Logger Log = Logger.IniciarLog("Simulação de inclusão de RAV Automático"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV, tipoVenda, periodicidade });
                ModRAVAutomatico ravDados = null;

                try
                {
                    string codigoFuncao = "11"; //  MA061_COD_FUNCAO, 
                    decimal numPDV = numeroPDV; //  MA061_NUM_PDV, 
                    string codigoProduto = "RA"; // MA061_COD_PRODUTO, 
                    string codigoCanalDeVenda = "S";// MA061_IND_CONTRATO_PORTAL, 
                    string usuarioPertenceAreaComecial = "N"; //MA061_IND_PRF_COMERCIAL, 
                    string tipoRvASerAntecipado = tipoVenda.ToString();// "R"; //MA061_TIP_RV, 
                    short numeroDParcelasInicial = 1; //MA061_NUM_PRCL_INI, 
                    short numeroDParcelasFinal = 3; // MA061_NUM_PRCL_FIM, 
                    short codigoSituacaoPendencia = 0;//MA061_COD_SIT_PENDENCIA, 
                    string codigoPeriodicidade = periodicidade.ToString(); // "D";//MA061_COD_PERIODICIDADE, 
                    string diaSemanaAntecipacao = "";// MA061_DIA_SEMANA, 
                    string diaAntecipacao = "";//MA061_DIA_ANTC, 
                    string dataInicioVigencia = "";//MA061_DAT_VIG_INI, 
                    string dataFimVigencia = "";//MA061_DAT_VIG_FIM, 
                    decimal valorMinimoAntecipar = 0;//MA061_VAL_MIN_ANTC, 
                    string indicadorAnteciparEstoque = "";//MA061_IND_ANTC_ESTOQ, 
                    string dataBaseAntcEstoque = "";//MA061_DAT_INI_ESTOQ, 
                    string operadorQueFezVendaProduto = "";//MA061_COD_OPID_VENDA, 
                    string dataDeContratacaoProduto = "";//MA061_DAT_CONTRATO, 
                    string nomeDoContatoDoEstabelecimento = "";//MA061_NOM_CONTATO, 
                    short codigoMotivoExclusao = 0;//MA061_COD_MOT_EXCLUSAO, 
                    string descricaoMotivoExclusao = "";//MA061_DES_MOT_EXCLUSAO, 
                    int numeroPvReferencia = 0;//MA061_NUM_PDV_REF, 
                    short qtdDeDiasDeAgendamentoParaExclusao = 0;// MA061_QTD_DIA_CANC, 
                    string paraUsoFuturoEntrada = "";//MA061_RESERVA_ENTRADA, 
                    short codigoRetorno = 0;// MA061_COD_RETORNO, 
                    string mensagemDeRetorno = "";//MA061_MSG_RETORNO, 
                    string nomeEstabelecimento = "";//MA061_NOM_PDV, 
                    decimal numeroCNPJCPF = 0;//MA061_NUM_CNPJ, 
                    string codigoCategoriaTaxa = "";//MA061_COD_CATEG, 
                    string nomeDaCategoria = "";//MA061_DES_CATEG, 
                    decimal taxaCategoria = 0;//MA061_PCT_TAXA_CATEG, 
                    string descricaoPendenciaDaTaxa = "";// MA061_DES_SIT_CATEG, 
                    short codigoSituacaoPendente = 0;// MA061_COD_SIT_PENDENTE, 
                    string dataBaseProxAntec = "";// MA061_DAT_BASE_ANTC, 
                    string dataDaProximaAntecipacao = "";// MA061_DAT_PRX_ANTC, 
                    string codigoOperadorDeAlteracao = "";//MA061_COD_OPID_ALTER, 
                    string dataAlteracao = "";// MA061_DAT_ALTER, 
                    string horaAlteracao = "";//MA061_HOR_ALTER, 
                    string codigoOperacaoAutorizacao = "";//MA061_COD_OPID_AUTOR, 
                    string dataAutorizacao = "";//MA061_DAT_AUTOR, 
                    string horaAutorizacao = "";//MA061_HOR_AUTOR, 
                    string dataAgendadaParaExclusao = ""; // MA061_DAT_AGND_EXC, 
                    int numeroDaMatriz = 0;//MA061_NUM_MATRIZ, 
                    string indicadorDeBloqueioPorFidelizacao = "";// MA061_IND_BLQ_FDLZ, 
                    decimal taxaDeFidelizacao = 0;//MA061_TAXA_FDLZ, 
                    string dataInicioFidelizacao = "";//MA061_DAT_INI_FDLZ, 
                    string dataFimFidelizacao = "";//MA061_DAT_FIM_FDLZ, 

                    String descricaoBandeiraSelecionada = String.Empty; //MA061_DSC_BNDR_SEL
                    Decimal valorTarifaAntecipacaoRAV = 0; //MA061_VAL_TARIFA
                    Int16 codigoProdutoAntecipacao = 0; //MA061_COD_PROD_ANTC
                    String nomeProdutoAntecipacao = String.Empty; //MA061_NOM_PROD_ANTC
                    String descricaoProdutoAntecipacao = String.Empty; //MA061_DSC_PROD_ANTC

                    string paraUsoFuturoSaida = "";//MA061_RESERVA_SAIDA
                    string indFull = "";//MA061_IND_FULL


                    FILLER2[] bandeiras = new FILLER2[100];
                    //ModRAVAutomatico ravAuto = ;
                    bandeiras = ConsultarBandeirasRAVAutomatico(numeroPDV, tipoVenda, periodicidade);

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        cliente.BMA061(ref codigoFuncao, ref numPDV, ref codigoProduto, ref codigoCanalDeVenda, ref usuarioPertenceAreaComecial,
                                    ref tipoRvASerAntecipado, ref numeroDParcelasInicial, ref numeroDParcelasFinal, ref codigoSituacaoPendencia,
                                    ref codigoPeriodicidade, ref diaSemanaAntecipacao, ref diaAntecipacao, ref dataInicioVigencia, ref dataFimVigencia,
                                    ref valorMinimoAntecipar, ref indicadorAnteciparEstoque, ref dataBaseAntcEstoque, ref operadorQueFezVendaProduto,
                                    ref dataDeContratacaoProduto, ref nomeDoContatoDoEstabelecimento, ref codigoMotivoExclusao, ref descricaoMotivoExclusao,
                                    ref numeroPvReferencia, ref qtdDeDiasDeAgendamentoParaExclusao, ref indFull, ref bandeiras, ref paraUsoFuturoEntrada, ref codigoRetorno,
                                    ref mensagemDeRetorno, ref nomeEstabelecimento, ref numeroCNPJCPF, ref codigoCategoriaTaxa, ref nomeDaCategoria,
                                    ref taxaCategoria, ref descricaoPendenciaDaTaxa, ref codigoSituacaoPendente, ref dataBaseProxAntec,
                                    ref dataDaProximaAntecipacao, ref codigoOperadorDeAlteracao, ref dataAlteracao, ref horaAlteracao,
                                    ref codigoOperacaoAutorizacao, ref dataAutorizacao, ref horaAutorizacao, ref dataAgendadaParaExclusao, ref numeroDaMatriz,
                                    ref indicadorDeBloqueioPorFidelizacao, ref taxaDeFidelizacao, ref dataInicioFidelizacao, ref dataFimFidelizacao,
                                    ref descricaoBandeiraSelecionada, ref valorTarifaAntecipacaoRAV, ref codigoProdutoAntecipacao, ref nomeProdutoAntecipacao, ref descricaoProdutoAntecipacao,
                                    ref paraUsoFuturoSaida);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            codigoFuncao,
                            numPDV,
                            codigoProduto,
                            codigoCanalDeVenda,
                            usuarioPertenceAreaComecial,
                            tipoRvASerAntecipado,
                            numeroDParcelasInicial,
                            numeroDParcelasFinal,
                            codigoSituacaoPendencia,
                            codigoPeriodicidade,
                            diaSemanaAntecipacao,
                            diaAntecipacao,
                            dataInicioVigencia,
                            dataFimVigencia,
                            valorMinimoAntecipar,
                            indicadorAnteciparEstoque,
                            dataBaseAntcEstoque,
                            operadorQueFezVendaProduto,
                            dataDeContratacaoProduto,
                            nomeDoContatoDoEstabelecimento,
                            codigoMotivoExclusao,
                            descricaoMotivoExclusao,
                            numeroPvReferencia,
                            qtdDeDiasDeAgendamentoParaExclusao,
                            indFull,
                            bandeiras,
                            paraUsoFuturoEntrada,
                            codigoRetorno,
                            mensagemDeRetorno,
                            nomeEstabelecimento,
                            numeroCNPJCPF,
                            codigoCategoriaTaxa,
                            nomeDaCategoria,
                            taxaCategoria,
                            descricaoPendenciaDaTaxa,
                            codigoSituacaoPendente,
                            dataBaseProxAntec,
                            dataDaProximaAntecipacao,
                            codigoOperadorDeAlteracao,
                            dataAlteracao,
                            horaAlteracao,
                            codigoOperacaoAutorizacao,
                            dataAutorizacao,
                            horaAutorizacao,
                            dataAgendadaParaExclusao,
                            numeroDaMatriz,
                            indicadorDeBloqueioPorFidelizacao,
                            taxaDeFidelizacao,
                            dataInicioFidelizacao,
                            dataFimFidelizacao,
                            descricaoBandeiraSelecionada,
                            valorTarifaAntecipacaoRAV,
                            codigoProdutoAntecipacao,
                            nomeProdutoAntecipacao,
                            descricaoProdutoAntecipacao,
                            paraUsoFuturoSaida
                        });

                        if (codigoRetorno == 0)
                        {
                            ravDados = new ModRAVAutomatico();

                            ravDados.CodigoProduto = codigoProduto;
                            ravDados.CodMotivoExclusao = codigoMotivoExclusao;
                            ravDados.CodSituacao = codigoSituacaoPendencia;
                            ravDados.CodVenda = codigoCanalDeVenda;
                            ravDados.DataContrato = HISUtils.ConvertToDate(dataDeContratacaoProduto);
                            ravDados.DataIniEstoq = HISUtils.ConvertToDate(dataBaseProxAntec);
                            ravDados.DataVigenciaFim = HISUtils.ConvertToDate(dataFimVigencia);
                            ravDados.DataVigenciaIni = HISUtils.ConvertToDate(dataInicioVigencia);
                            ravDados.DescMotivoExclusao = descricaoMotivoExclusao;
                            ravDados.DiaAntecipacao = diaAntecipacao;
                            ravDados.IndAnteEstoq = indicadorAnteciparEstoque == "S" ? ElndAntecEstoq.Sim : ElndAntecEstoq.Nao;
                            ravDados.IndContratoPortal = Convert.ToChar(codigoCanalDeVenda);
                            ravDados.IndPRFComercial = Convert.ToChar(usuarioPertenceAreaComecial);
                            ravDados.NomeContato = nomeDoContatoDoEstabelecimento;
                            ravDados.NumeroPDV = numeroPDV;
                            ravDados.NumeroPDVRef = numeroPvReferencia;
                            ravDados.NumParcelaFim = numeroDParcelasFinal;
                            ravDados.NumParcelaIni = numeroDParcelasFinal;
                            switch (codigoPeriodicidade)
                            {
                                case "D":
                                    { ravDados.Periodicidade = EPeriodicidade.Diario; break; }
                                case "S":
                                    { ravDados.Periodicidade = EPeriodicidade.Semanal; break; }
                                case "Q":
                                    { ravDados.Periodicidade = EPeriodicidade.Quinzenal; break; }
                                case "M":
                                    { ravDados.Periodicidade = EPeriodicidade.Mensal; break; }
                            }
                            ravDados.QtdeDiasCancelamento = qtdDeDiasDeAgendamentoParaExclusao;
                            ravDados.IndFull = indFull;
                            ravDados.Bandeiras = PreencherBandeiras(bandeiras);

                            ravDados.TipoRAV = tipoRvASerAntecipado == "R" ? ElndProdutoAntecipa.Rotativo : tipoRvASerAntecipado == "P" ? ElndProdutoAntecipa.Parcelado : ElndProdutoAntecipa.Ambos;
                            ravDados.ValorMinimo = valorMinimoAntecipar;
                            ravDados.Funcao = codigoFuncao == "11" ? ECodFuncao.Simulacao : codigoFuncao == "21" ? ECodFuncao.Consultar : ECodFuncao.Efetivar;

                            ravDados.DadosRetorno.CodCategoria = HISUtils.ConvertToInt32(codigoCategoriaTaxa);
                            ravDados.DadosRetorno.CodOpidAlteracao = HISUtils.ConvertToInt32(codigoOperadorDeAlteracao);
                            ravDados.DadosRetorno.CodOpidAutorizacao = HISUtils.ConvertToInt32(codigoOperacaoAutorizacao);
                            ravDados.DadosRetorno.CodRetorno = 70100 + codigoRetorno;
                            ravDados.DadosRetorno.CodSituacaoPendente = codigoSituacaoPendente;
                            ravDados.DadosRetorno.CpfCnpj = numeroCNPJCPF.ToString();
                            ravDados.DadosRetorno.DataAgendaExclusao = HISUtils.ConvertToDate(dataAgendadaParaExclusao);
                            ravDados.DadosRetorno.DataAlteracao = HISUtils.ConvertToDate(dataAlteracao);
                            ravDados.DadosRetorno.DataAutorizacao = HISUtils.ConvertToDate(dataAutorizacao);
                            ravDados.DadosRetorno.DataBaseAntecipacao = HISUtils.ConvertToDate(dataBaseAntcEstoque);
                            ravDados.DadosRetorno.DataFimFidelizacao = HISUtils.ConvertToDate(dataFimFidelizacao);
                            ravDados.DadosRetorno.DataIniFidelizacao = HISUtils.ConvertToDate(dataInicioFidelizacao);
                            ravDados.DadosRetorno.DataProximaAntecipacao = HISUtils.ConvertToDate(dataDaProximaAntecipacao);
                            ravDados.DadosRetorno.DescCategoria = nomeDaCategoria;
                            ravDados.DadosRetorno.DescSituacaoCategoria = descricaoPendenciaDaTaxa;
                            ravDados.DadosRetorno.Estabelecimento = nomeEstabelecimento;
                            ravDados.DadosRetorno.HoraAlteracao = horaAlteracao;
                            ravDados.DadosRetorno.HoraAutorizacao = horaAutorizacao;



                            if (indicadorDeBloqueioPorFidelizacao == "")
                            {
                                ravDados.DadosRetorno.IndBloqueio = new char();
                            }
                            else
                            {
                                ravDados.DadosRetorno.IndBloqueio = Convert.ToChar(indicadorDeBloqueioPorFidelizacao);
                            }

                            ravDados.DadosRetorno.MsgRetorno = mensagemDeRetorno;
                            ravDados.DadosRetorno.NumMatrix = numeroDaMatriz;
                            ravDados.DadosRetorno.TaxaCategoria = taxaCategoria;
                            ravDados.DadosRetorno.TaxaFidelizacao = taxaDeFidelizacao;

                            ravDados.DadosRetorno.DescricaoBandeiraSelecionada = descricaoBandeiraSelecionada;
                            ravDados.DadosRetorno.ValorTarifaAntecipacaoRAV = valorTarifaAntecipacaoRAV;
                            ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                            switch (diaSemanaAntecipacao)
                            {
                                case "SEG":
                                    { ravDados.DiaSemana = EDiaSemana.Segunda; break; }
                                case "TER":
                                    { ravDados.DiaSemana = EDiaSemana.Terca; break; }
                                case "QUA":
                                    { ravDados.DiaSemana = EDiaSemana.Quarta; break; }
                                case "QUI":
                                    { ravDados.DiaSemana = EDiaSemana.Quinta; break; }
                                case "SEX":
                                    { ravDados.DiaSemana = EDiaSemana.Sexta; break; }
                                default:
                                    { break; }
                            }
                        }
                        else
                        {
                            ravDados = new ModRAVAutomatico();

                            ravDados.IndFull = indFull;
                            ravDados.Bandeiras = PreencherBandeiras(bandeiras);
                            ravDados.DadosRetorno.CodigoProdutoAntecipacao = codigoProdutoAntecipacao;
                            ravDados.DadosRetorno.NomeProdutoAntecipacao = nomeProdutoAntecipacao;
                            ravDados.DadosRetorno.DescricaoProdutoAntecipacao = descricaoProdutoAntecipacao;

                            ravDados.CodRetorno = 70100 + codigoRetorno;
                            ravDados.DadosRetorno.CodRetorno = 70100 + codigoRetorno;
                            ravDados.MensagemRetorno = mensagemDeRetorno;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }

                Log.GravarLog(EventoLog.FimAgente, new { ravDados });
                return ravDados;
            }
        }

        private List<ModRAVAutomaticoBandeira> PreencherBandeiras(FILLER2[] bandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Início preenchendo bandeiras"))
            {
                Logger.GravarLog("Bandeiras retornadas pelo MA061", new { bandeiras });

                try
                {
                    List<ModRAVAutomaticoBandeira> listaBandeiras = new List<ModRAVAutomaticoBandeira>();
                    foreach (FILLER2 filler in bandeiras)
                    {
                        if (filler.MA061_COD_BNDR != 0)
                        {
                            ModRAVAutomaticoBandeira bandeira = new ModRAVAutomaticoBandeira();
                            bandeira.CodBandeira = filler.MA061_COD_BNDR;
                            bandeira.DscBandeira = filler.MA061_DSC_BNDR;
                            bandeira.IndSel = filler.MA061_IND_SEL;

                            listaBandeiras.Add(bandeira);
                        }
                    }

                    return listaBandeiras;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region Transação MA135 - RAV Email
        /// <summary>
        /// Retorna os emails cadastrados para o PDV consultado.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public ModRAVEmailEntradaSaida ConsultarEmails(Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de e-mails cadastrados para o PV"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { numeroPDV });

                ModRAVEmailEntradaSaida saida = null;

                try
                {
                    string codigoFuncao = "C";//MA135_COD_FUNCAO, 
                    int codigoEstabelecimento = numeroPDV; //MA135_NUM_PDV, 
                    string indicadorEnviaEmail = "";//MA135_IND_ENVO_EMAL, 
                    string indicadorEnviaFax = "N";//MA135_IND_ENVO_FAX,
                    string enderecoEmail = "";//MA135_TXT_INFD_EMAL, 
                    string indicadorEnvioFluxoCaixa = "";//MA135_IND_OPC1, 
                    string indicadorEnvioValoresPV = "";//MA135_IND_OPC2,
                    string indicadorEnvioResumoOperacao = "";//MA135_IND_OPC3, 
                    string indicadorSituacaoEmailFax = "A";//MA135_IND_SIT_EMAL_FAX, 
                    //string indEnvioFax = "N";
                    string opidQueFezAlteracao = "";//MA135_NUM_FNCL_CAD_EMAL, 
                    short codigoRetorno = 0;//MA135_COD_RETORNO, 
                    string mensagemRetorno = "";//MA135_MSG_RETORNO, 
                    string erroSqlCode = "";//MA135_COD_SQLCODE, 
                    string existemMaisRegistros = "";//MA135_IND_MAIS_OCORRENCIA,
                    string dataUltimaAlteracao = "";//MA135_DAT_ULT_ALTER, 
                    string dataInclusaoCadastro = "";//MA135_DAT_INC_CAD

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        saida = new ModRAVEmailEntradaSaida();
                        //Máximo de 3 emails
                        for (int i = 0; i < 3; i++)
                        {

                            short numeroSequencia = (short)(i + 1);//MA135_NUM_SEQ,
                            Log.GravarLog(EventoLog.ChamadaHIS, new
                            {
                                codigoFuncao,
                                codigoEstabelecimento,
                                numeroSequencia,
                                indicadorEnviaEmail,
                                indicadorEnviaFax,
                                enderecoEmail,
                                indicadorEnvioFluxoCaixa,
                                indicadorEnvioValoresPV,
                                indicadorEnvioResumoOperacao,
                                indicadorSituacaoEmailFax,
                                opidQueFezAlteracao,
                                codigoRetorno,
                                mensagemRetorno,
                                erroSqlCode,
                                existemMaisRegistros,
                                dataUltimaAlteracao,
                                dataInclusaoCadastro
                            });
                            cliente.BMA135M(ref codigoFuncao, ref codigoEstabelecimento, ref numeroSequencia, ref indicadorEnviaEmail, ref indicadorEnviaFax, ref enderecoEmail,
                                           ref indicadorEnvioFluxoCaixa, ref indicadorEnvioValoresPV, ref indicadorEnvioResumoOperacao, ref indicadorSituacaoEmailFax,
                                           ref opidQueFezAlteracao, ref codigoRetorno, ref mensagemRetorno, ref erroSqlCode, ref existemMaisRegistros, ref dataUltimaAlteracao,
                                           ref dataInclusaoCadastro);

                            Log.GravarLog(EventoLog.RetornoHIS, new
                            {
                                codigoFuncao,
                                codigoEstabelecimento,
                                numeroSequencia,
                                indicadorEnviaEmail,
                                indicadorEnviaFax,
                                enderecoEmail,
                                indicadorEnvioFluxoCaixa,
                                indicadorEnvioValoresPV,
                                indicadorEnvioResumoOperacao,
                                indicadorSituacaoEmailFax,
                                opidQueFezAlteracao,
                                codigoRetorno,
                                mensagemRetorno,
                                erroSqlCode,
                                existemMaisRegistros,
                                dataUltimaAlteracao,
                                dataInclusaoCadastro
                            });

                            if (codigoRetorno != 15)
                            {
                                ModRAVEmail email = new ModRAVEmail();
                                email.Sequencia = numeroSequencia;
                                email.DataUltAlteracao = HISUtils.ConvertToDate(dataUltimaAlteracao);
                                email.DataUltInclusao = HISUtils.ConvertToDate(dataUltimaAlteracao);
                                email.Email = enderecoEmail;
                                saida.ListaEmails.Add(email);
                            }

                            if (existemMaisRegistros == "N")
                            {
                                break;
                            }
                        }

                        Log.GravarLog(EventoLog.FimAgente, new { saida });
                        return saida;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Salva as alterações e inclusões de emails do PDV.
        /// </summary>
        /// <param name="dadosEmail"></param>
        /// <returns></returns>
        public Boolean SalvarEmails(ModRAVEmailEntradaSaida dadosEmail)
        {
            using (Logger Log = Logger.IniciarLog("Salvando alterações e inclusões de e-mails do PV"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { dadosEmail });

                try
                {
                    if (dadosEmail.NumeroPDV <= 0)
                    {
                        return false;
                    }

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        String codigoFuncao = "";//MA135_COD_FUNCAO, 

                        foreach (ModRAVEmail item in dadosEmail.ListaEmails)
                        {
                            if (item.Status != EStatusEmail.None)
                            {
                                if (item.Status == EStatusEmail.Incluso)
                                {
                                    codigoFuncao = "I";
                                }
                                else if (item.Status == EStatusEmail.Alterado)
                                {
                                    codigoFuncao = "A";
                                }
                                else
                                {
                                    codigoFuncao = "A";
                                }

                                int codigoEstabelecimento = HISUtils.ConvertToInt32(dadosEmail.NumeroPDV.ToString()); //MA135_NUM_PDV, 
                                short numeroSequenciaEmailFax = (short)item.Sequencia;//MA135_NUM_SEQ, 
                                string indicadorEnviaEmail = Convert.ToString(dadosEmail.IndEnviaEmail);//MA135_IND_ENVO_EMAL, 
                                string indicadorEnviaFax = "N";//MA135_IND_ENVO_FAX,
                                string enderecoEmail = item.Email;//MA135_TXT_INFD_EMAL, 
                                string indicadorEnvioFluxoCaixa = Convert.ToString(dadosEmail.IndEnviaFluxoCaixa);//MA135_IND_OPC1, 
                                string indicadorEnvioValoresPV = Convert.ToString(dadosEmail.IndEnviaValoresPV);//MA135_IND_OPC2,
                                string indicadorEnvioResumoOperacao = Convert.ToString(dadosEmail.IndEnviaResumoOperacao);//MA135_IND_OPC3, 
                                string indicadorSituacaoEmailFax = string.Empty;
                                if (item.Status != EStatusEmail.Excluir)
                                {
                                    indicadorSituacaoEmailFax = "A";//MA135_IND_SIT_EMAL_FAX, 
                                }
                                else
                                {
                                    indicadorSituacaoEmailFax = "C";//MA135_IND_SIT_EMAL_FAX, 
                                }
                                //string indEnvioFax = "N";
                                string opidQueFezAlteracao = "";//MA135_NUM_FNCL_CAD_EMAL, 
                                short codigoRetorno = 0;//MA135_COD_RETORNO, 
                                string mensagemRetorno = "";//MA135_MSG_RETORNO, 
                                string erroSqlCode = "";//MA135_COD_SQLCODE, 
                                string existemMaisRegistros = "";//MA135_IND_MAIS_OCORRENCIA,
                                string dataUltimaAlteracao = "";//item.DataUltAlteracao.ToString();//MA135_DAT_ULT_ALTER, 
                                string dataInclusaoCadastro = "";//MA135_DAT_INC_CAD

                                Log.GravarLog(EventoLog.ChamadaHIS, new
                                {
                                    codigoFuncao,
                                    codigoEstabelecimento,
                                    numeroSequenciaEmailFax,
                                    indicadorEnviaEmail,
                                    indicadorEnviaFax,
                                    enderecoEmail,
                                    indicadorEnvioFluxoCaixa,
                                    indicadorEnvioValoresPV,
                                    indicadorEnvioResumoOperacao,
                                    indicadorSituacaoEmailFax,
                                    opidQueFezAlteracao,
                                    codigoRetorno,
                                    mensagemRetorno,
                                    erroSqlCode,
                                    existemMaisRegistros,
                                    dataUltimaAlteracao,
                                    dataInclusaoCadastro
                                });

                                cliente.BMA135M(ref codigoFuncao, ref codigoEstabelecimento, ref numeroSequenciaEmailFax, ref indicadorEnviaEmail, ref indicadorEnviaFax,
                                            ref enderecoEmail, ref indicadorEnvioFluxoCaixa, ref indicadorEnvioValoresPV, ref indicadorEnvioResumoOperacao,
                                            ref indicadorSituacaoEmailFax, ref opidQueFezAlteracao, ref codigoRetorno, ref mensagemRetorno, ref erroSqlCode,
                                            ref existemMaisRegistros, ref dataUltimaAlteracao, ref dataInclusaoCadastro);

                                Log.GravarLog(EventoLog.RetornoHIS, new
                                {
                                    codigoFuncao,
                                    codigoEstabelecimento,
                                    numeroSequenciaEmailFax,
                                    indicadorEnviaEmail,
                                    indicadorEnviaFax,
                                    enderecoEmail,
                                    indicadorEnvioFluxoCaixa,
                                    indicadorEnvioValoresPV,
                                    indicadorEnvioResumoOperacao,
                                    indicadorSituacaoEmailFax,
                                    opidQueFezAlteracao,
                                    codigoRetorno,
                                    mensagemRetorno,
                                    erroSqlCode,
                                    existemMaisRegistros,
                                    dataUltimaAlteracao,
                                    dataInclusaoCadastro
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                bool retorno = true;
                Log.GravarLog(EventoLog.FimAgente, new { retorno });
                return retorno;
            }
        }
        #endregion

        #region Transação RW110 - Senha
        /// <summary>
        /// Método para verificar se o usuário tem acesso ao RAV.
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean ValidarSenha(String senha, Int32 numeroPDV)
        {
            using (Logger Log = Logger.IniciarLog("Verificação se o usuário possui acesso ao RAV"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { senha, numeroPDV });
                try
                {
                    string sistemaChamador = "IS";//BRW110E_SISTEMA, 
                    int numPDV = numeroPDV; //BRW110E_NUM_PDV, 
                    short codigoDoProduto = 1;//BRW110E_COD_PROD,
                    short tamanhoSenha = (short)senha.Length;//BRW110E_TAM_SENHA, 
                    string senhaCriptografada = senha;//BRW110E_SENHA, 
                    short telefoneChamadorDDD = 0;//BRW110E_NUM_DDD, 
                    int telefoneChamador = 0;//BRW110E_NUM_FONE, 
                    string areaLivre = "";//BRW110E_LIVRE, 
                    short codRetorno = 0;//BRW110S_COD_ERRO, 
                    string mensagemErro = "";//BRW110S_MSG_ERRO, 
                    string dataUltimaSolicitacao = "";//BRW110S_ULT_SOLIC, 
                    short qtdDiasParaExpirarSenha = 0;//BRW110S_QTD_DIAS, 
                    string filler = "";//BRW110S_FILLER

                    using (ModuloRAVClient cliente = new ModuloRAVClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            sistemaChamador,
                            numPDV,
                            codigoDoProduto,
                            tamanhoSenha,
                            senhaCriptografada,
                            telefoneChamadorDDD,
                            telefoneChamador,
                            areaLivre,
                            codRetorno,
                            mensagemErro,
                            dataUltimaSolicitacao,
                            qtdDiasParaExpirarSenha,
                            filler
                        });

                        cliente.BRW110(ref sistemaChamador, ref numPDV, ref codigoDoProduto, ref tamanhoSenha, ref senhaCriptografada, ref telefoneChamadorDDD, ref telefoneChamador,
                            ref areaLivre, ref codRetorno, ref mensagemErro, ref dataUltimaSolicitacao, ref qtdDiasParaExpirarSenha, ref filler);

                        Log.GravarLog(EventoLog.RetornoHIS, new
                        {
                            sistemaChamador,
                            numPDV,
                            codigoDoProduto,
                            tamanhoSenha,
                            senhaCriptografada,
                            telefoneChamadorDDD,
                            telefoneChamador,
                            areaLivre,
                            codRetorno,
                            mensagemErro,
                            dataUltimaSolicitacao,
                            qtdDiasParaExpirarSenha,
                            filler
                        });

                        if (codRetorno != 0)
                        {
                            //TODO: Gravar Log
                        }

                        bool retorno = codRetorno == 0 ? true : false;
                        Log.GravarLog(EventoLog.FimAgente, new { retorno });
                        return retorno;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Método para verificar se o usuário tem acesso ao RAV.
        /// </summary>
        /// <param name="numeroPDV"></param>
        /// <returns></returns>
        public Boolean VerificarAcesso(Int32 numeroPDV)
        {
            return true;
        }
        #endregion


    }
}

