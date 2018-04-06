using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Modelo.Servicos;

namespace Redecard.PN.Extrato.Agentes.Tradutores
{
    /// <summary>
    /// Classe tradutora dos parâmetros de entrada e saída 
    /// das consultas HIS (AREA_FIXA)
    /// </summary>
    public class ServicosTR : ITradutor
    {
        #region [ Serviços - Gateway - WACA1260 / WA1260 / ISGH ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarGatewayEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(3000, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<Gateway> ConsultarGatewaySaida(String dados)
        {
            List<Gateway> retorno = new List<Gateway>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(3);

            String[] valores = cortador.LerOccurs(97, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);

                    retorno.Add(new Gateway
                    {
                        EstabelecimentoCobranca = cortadorRegistro.LerInt32(9),
                        DataCobranca = cortadorRegistro.LerData(7, "MM/yyyy", true),
                        QuantidadeContratada = cortadorRegistro.LerInt32(7),
                        QuantidadeRealizadas = cortadorRegistro.LerInt32(7),
                        QuantidadeExcedidas = cortadorRegistro.LerInt32(7),
                        ValorPacote = cortadorRegistro.LerDecimal(13, 2),
                        ValorTarifa = cortadorRegistro.LerDecimal(13, 2),
                        ValorExcedido = cortadorRegistro.LerDecimal(13, 2),
                        ValorTotalCobranca = cortadorRegistro.LerDecimal(13, 2)
                    });
                }
            }
            return retorno;
        }

        #endregion

        #region [ Serviços - Boleto - WACA1261 / WA1261 / ISGI ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarBoletoEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(3000, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<Boleto> ConsultarBoletoSaida(String dados)
        {
            List<Boleto> retorno = new List<Boleto>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(3);

            String[] valores = cortador.LerOccurs(97, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new Boleto
                    {
                        EstabelecimentoCobranca = cortadorRegistro.LerInt32(9),
                        DataCobranca = cortadorRegistro.LerData(7, "MM/yyyy", true),
                        QuantidadeContratada = cortadorRegistro.LerInt32(7),
                        QuantidadeImpressa = cortadorRegistro.LerInt32(7),
                        QuantidadeExcedida = cortadorRegistro.LerInt32(7),
                        ValorPacote = cortadorRegistro.LerDecimal(13, 2),
                        ValorTarifa = cortadorRegistro.LerDecimal(13, 2),
                        ValorExcedido = cortadorRegistro.LerDecimal(13, 2),
                        ValorTotalCobrado = cortadorRegistro.LerDecimal(13, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Serviços - Análise de Risco / Monitoring - WACA1262 / WA1262 / ISGJ ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarAnaliseRiscoEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(3000, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<AnaliseRisco> ConsultarAnaliseRiscoSaida(String dados)
        {
            List<AnaliseRisco> retorno = new List<AnaliseRisco>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(3);

            String[] valores = cortador.LerOccurs(97, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new AnaliseRisco
                    {
                        EstabelecimentoCobranca = cortadorRegistro.LerInt32(9),
                        DataCobranca = cortadorRegistro.LerData(7, "MM/yyyy", true),
                        QuantidadeContratada = cortadorRegistro.LerInt32(7),
                        QuantidadeUtilizada = cortadorRegistro.LerInt32(7),
                        QuantidadeExcedida = cortadorRegistro.LerInt32(7),
                        ValorPacote = cortadorRegistro.LerDecimal(13, 2),
                        ValorTarifa = cortadorRegistro.LerDecimal(13, 2),
                        ValorExcedido = cortadorRegistro.LerDecimal(13, 2),
                        ValorTotalCobranca = cortadorRegistro.LerDecimal(13, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Serviços - Manual Review - WACA1263 / WA1263 / ISGK ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarManualReviewEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(3000, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<ManualReview> ConsultarManualReviewSaida(String dados)
        {
            List<ManualReview> retorno = new List<ManualReview>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(3);

            String[] valores = cortador.LerOccurs(97, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new ManualReview
                    {
                        EstabelecimentoCobranca = cortadorRegistro.LerInt32(9),
                        DataCobranca = cortadorRegistro.LerData(7, "MM/yyyy", true),
                        QuantidadeContratada = cortadorRegistro.LerInt32(7),
                        QuantidadeRealizada = cortadorRegistro.LerInt32(7),
                        QuantidadeExcedida = cortadorRegistro.LerInt32(7),
                        ValorPacote = cortadorRegistro.LerDecimal(13, 2),
                        ValorTarifa = cortadorRegistro.LerDecimal(13, 2),
                        ValorExcedido = cortadorRegistro.LerDecimal(13, 2),
                        ValorTotalCobranca = cortadorRegistro.LerDecimal(13, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Serviços - Serasa/AVS - WACA1210 / WA1210 / ISFK ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarSerasaAVSEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            dados.Append(pvs.Count.ToString("D4"));
            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(2996, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<SerasaAvs> ConsultarSerasaAVSSaida(String dados)
        {
            List<SerasaAvs> retorno = new List<SerasaAvs>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(77, 380);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    String tipoRegistro = cortadorRegistro.LerString(1);

                    if (String.Compare("1", tipoRegistro, true) == 0) //Serasa
                    {
                        retorno.Add(new Serasa
                        {
                            TipoRegistro = tipoRegistro,
                            NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                            QuantidadeMinimaConsultas = cortadorRegistro.LerInt32(6),
                            ValorFranquia = cortadorRegistro.LerDecimal(9, 4),
                            AnoMesCobranca = cortadorRegistro.LerString(7),
                            QuantidadeConsultasEfetuadas = cortadorRegistro.LerInt32(6),
                            QuantidadeConsultasEfetuadasOK = cortadorRegistro.LerInt32(6),
                            ValorCobranca = cortadorRegistro.LerDecimal(11, 2),
                            ValorConsulta = cortadorRegistro.LerDecimal(9, 4),
                            RegimeFranquia = cortadorRegistro.LerInt32(3)
                        });
                    }
                    else if (String.Compare("2", tipoRegistro, true) == 0) // AVS
                    {
                        retorno.Add(new AVS
                        {
                            TipoRegistro = tipoRegistro,
                            NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                            QuantidadeMinimaConsultas = cortadorRegistro.LerInt32(6),
                            ValorFranquia = cortadorRegistro.LerDecimal(9, 4),
                            AnoMesCobranca = cortadorRegistro.LerString(7),
                            QuantidadeConsultasEfetuadas = cortadorRegistro.LerInt32(6),
                            QuantidadeConsultasEfetuadasOK = cortadorRegistro.LerInt32(6),
                            ValorCobranca = cortadorRegistro.LerDecimal(11, 2),
                            ValorConsulta = cortadorRegistro.LerDecimal(9, 4),
                            RegimeFranquia = cortadorRegistro.LerInt32(3)
                        });
                    }
                }
            }

            return retorno;
        }

        #endregion

        #region [ Serviços - Novo Pacote - BKWA1260 / WA2400 / ISGH ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarNovoPacoteEntrada(List<Int32> pvs)
        {
            StringBuilder dados = new StringBuilder();

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));
            dados.Append("".PadRight(3000, ' '));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<NovoPacote> ConsultarNovoPacoteSaida(String dados)
        {
            List<NovoPacote> retorno = new List<NovoPacote>();

            CortadorMensagem cortador = new CortadorMensagem(dados);

            Int32 qtdRegistros = cortador.LerInt32(3);

            String[] valores = cortador.LerOccurs(97, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    CortadorMensagem cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new NovoPacote
                    {
                        EstabelecimentoCobranca = cortadorRegistro.LerInt32(9),
                        DataCobranca = cortadorRegistro.LerData(7, "MM/yyyy", true),
                        QuantidadeContratada = cortadorRegistro.LerInt32(7),
                        QuantidadeRealizada = cortadorRegistro.LerInt32(7),
                        QuantidadeExcedida = cortadorRegistro.LerInt32(7),
                        ValorPacote = cortadorRegistro.LerDecimal(13, 2),
                        ValorTarifa = cortadorRegistro.LerDecimal(13, 2),
                        ValorExcedido = cortadorRegistro.LerDecimal(13, 2),
                        ValorTotalCobranca = cortadorRegistro.LerDecimal(13, 2)
                    });
                }
            }

            return retorno;
        }

        #endregion

        #region [ Serviços - Recarga de Celular - BKWA2410 / WA241 / ISIA ]

        /// <summary>
        /// Traduz dados para chamada do serviço HIS
        /// </summary>
        public static String ConsultarRecargaCelularEntrada(List<Int32> pvs)
        {
            var dados = new StringBuilder();

            dados.Append(pvs.Count.ToString("D4"));

            for (Int32 iPV = 0; iPV < 3000; iPV++)
                dados.Append((pvs.Count > iPV ? pvs[iPV] : 0).ToString("D9"));

            return dados.ToString();
        }

        /// <summary>
        /// Traduz retorno do serviço HIS para dados
        /// </summary>
        public static List<RecargaCelular> ConsultarRecargaCelularSaida(String dados)
        {
            var retorno = new List<RecargaCelular>();
            var cortador = new CortadorMensagem(dados);
            Int32 qtdRegistros = cortador.LerInt32(6);

            String[] valores = cortador.LerOccurs(71, 300);

            foreach (String valor in valores)
            {
                if (retorno.Count < qtdRegistros)
                {
                    var cortadorRegistro = new CortadorMensagem(valor);
                    retorno.Add(new RecargaCelular
                    {
                        NumeroEstabelecimento = cortadorRegistro.LerInt32(9),
                        DataReferencia = cortadorRegistro.LerData(6, "MMyyyy"),
                        QuantidadeTransacao = cortadorRegistro.LerInt32(5),
                        ValorTotalTransacao = cortadorRegistro.LerDecimal(15, 2),
                        ValorTotalComissao = cortadorRegistro.LerDecimal(13, 2),
                        NumeroRv = cortadorRegistro.LerInt32(9),
                        DataPagamento = cortadorRegistro.LerData(10, "dd/MM/yyyy", true)
                    });
                }
            }

            return retorno;
        }

        #endregion
    
    }
}
