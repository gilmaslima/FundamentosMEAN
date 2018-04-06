/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Redecard.PN.FMS.Comum
{
    /// <summary>
    /// Este componente publica a classe XXX, que expõe métodos para manipular as mensagens de erro.
    /// </summary>
    public class MensagemDeErro
    {

        /// <summary>
        ///  Enumeração utilizado para campos.
        /// </summary>
        public enum Campo
        {
             CampoNaoInformado = 0,
             NumeroDoEmissor = 1,
             GrupoDeEntidade = 2,
             LoginUsario = 3,
             Usuario = 4,
             DataInicialPeriodo = 5,
             DataFinalPeriodo = 6,
             CriterioSelecao = 7,
             LoginAnalista = 8,
             NumeroCartao = 9,
             SituacaoDeFraude = 10,
             TipoAlarme = 11,
             SituacaoTransacao = 12,
             TipoPeriodo = 13,
             CodigoMCC = 14,
             DescricaoMCC = 15,
             CriterioOrdenacao = 16,
             TiposAlarmeSelecionados = 17,
             TiposTransacaoSelecionadas = 18,
             SituacaoTransacaoSelecionadas = 19,
             ICA = 20,
             TempoTimeout = 21,
             ListaRegistrosDeFraude = 22,
             CriterioSelecaoEstabelecimento = 23,
             TipoResposta = 24,
             CodigoAgenteEmissor = 25,
             CodigoEstabelecimento = 26,
             IndicadorTipoCartao = 27,
             IndicadorStatusCartao = 28,
             CriterioClassificacao = 29,
             CartaoEstabelecimentoClassificadoPor = 30,
             CartaoClassificadoPor = 31
        }
        /// <summary>
        /// Classe utilizada para constantes de código de erro.
        /// </summary>
        public class CodigosErro
        {

            public static readonly int ErroComunicacaoOuIndisponibilidadeServidores = 2001;
            public static readonly int ErroPassagemParametrosPaginacao = 3001;
            public static readonly int EmissorNaoEncontrado = 4001;
            public static readonly int CartaoEmAnalisePorOutroUsuario = 6001;
            public static readonly int EstabelecimentoEmAnalisePorOutroUsuario = 6002;

            public static readonly int ErroDuranteExecucaoDaSolicitacao = 9000;
            public static readonly int ErroDeSeguranca = 9001;
            public static readonly int UsuarioSemPermissaoAFuncionalidade = 9002;
            public static readonly int NaoForamEncontradasTransacoesParaOCartao = 9003;
            public static readonly int NaoExistemTransacoesAnalisadasParaConfirmacao = 9004;
            public static readonly int InformarTipoDeResposta = 9005;
            public static readonly int TipoAlarmeSemSelecao = 9006;
            public static readonly int TipoTransacaoSemSelecao = 9007;
            public static readonly int DataFinalFaixaScoreMaisRecenteQueDataInicial = 9008;
            public static readonly int DataFinalTransacaoMaisRecenteQueDataInicial = 9009;
            public static readonly int PeriodoSelecionadoMaiorQueOPermitidoEmParametrizacao = 9010;
            public static readonly int DataFinalMaisRecenteQueDataInicial = 9011;
            public static readonly int NumeroCartaoInvalido = 9013;
            public static readonly int TipoRespostaNaoSelecionado = 9014;
            public static readonly int OperacaoConcluidaComSucesso = 9015;
            public static readonly int BloqueioRenovado = 9016;
            public static readonly int SituacaoNaoInformada = 9017;
            public static readonly int LimiteEstabelecimentosAlcancados = 9018;
            public static readonly int EstabelecimentoJaSelecionado = 9019;
            public static readonly int NaoForamEncontradosRegistrosPesquisa = 9020;
            public static readonly int InformeNumeroDoCartao = 9021;
            public static readonly int EstabelecimentoDeveSerNumerico = 9022;
            public static readonly int CartaoJaAnalisado = 9023;
            public static readonly int TransacoesAlarmadasNaoAnalisadas = 9024;
        }
        
        #region Construtor
        /// <summary>
        ///  Construtor para mensagem de erro.
        /// </summary>
        /// <param name="codigoErro"></param>
        public MensagemDeErro(int codigoErro)
        {
            this.Codigo = codigoErro;
            IDictionary<string, string> detalhes = MensagemErroXMLHelper.ObterDadosErroPorCodigoErro(Codigo);
            this.Mensagem = detalhes["mensagem"];
            this.EhExcecao = bool.Parse(detalhes["ehExcecao"]);
        }
        /// <summary>
        /// Construtor para mensagem de erro.
        /// </summary>
        /// <param name="tipoExcecao"></param>
        /// <param name="campoOrigem"></param>
        public MensagemDeErro(TipoExcecao tipoExcecao, Campo campoOrigem)
        {
            this.Codigo = int.Parse(((int)tipoExcecao).ToString("0") + ((int)campoOrigem).ToString("000"));
            IDictionary<string, string> detalhes = MensagemErroXMLHelper.ObterDadosErroPorCodigoErro(Codigo);
            this.Mensagem = detalhes["mensagem"];
            this.EhExcecao = bool.Parse(detalhes["ehExcecao"]);
        }
        /// <summary>
        /// Este construtor é utilizado para criar mensagem de tipo de ececeção, campo de origem e formato do valor.
        /// </summary>
        /// <param name="tipoExcecao"></param>
        /// <param name="campoOrigem"></param>
        /// <param name="valorFormat"></param>
        public MensagemDeErro(TipoExcecao tipoExcecao, Campo campoOrigem, string valorFormat)
        {
            this.Codigo = int.Parse(((int)tipoExcecao).ToString("0") + ((int)campoOrigem).ToString("000"));
            IDictionary<string, string> detalhes = MensagemErroXMLHelper.ObterDadosErroPorCodigoErro(Codigo);
            this.Mensagem = string.Format(detalhes["mensagem"], valorFormat);
            this.EhExcecao = bool.Parse(detalhes["ehExcecao"]);
        }
        /// <summary>
        /// Este construtor é utilizado  primeiramente para identificar erro de "estabelecimento inexistente" na atualização de critéros de selecao. 
        ///identifica via regexp e retorna  estabelecimento com erro na mensagem de erro.
        /// </summary>
        /// <param name="tipoExcecao"></param>
        /// <param name="msgErro"></param>
        /// <param name="campoOrigem"></param>
        public MensagemDeErro(TipoExcecao tipoExcecao, string msgErro, Campo campoOrigem)
        {
            this.Codigo = int.Parse(((int)tipoExcecao).ToString("0") + ((int)campoOrigem).ToString("000"));
            IDictionary<string, string> detalhes = MensagemErroXMLHelper.ObterDadosErroPorCodigoErro(Codigo);

            string regexIdentificarCodigoCampo = @"{(\S+)}";
            string valorCodigoDigitado = Regex.Match(msgErro, regexIdentificarCodigoCampo).Groups[1].Value;

            this.Mensagem =  string.Format(detalhes["mensagem"], valorCodigoDigitado);
            this.EhExcecao = bool.Parse(detalhes["ehExcecao"]);
        }
        /// <summary>
        /// Este construtor é utilizado para criar mensagem de erro com codigo e mensagem.
        /// </summary>
        /// <param name="codigoErro"></param>
        /// <param name="msgErro"></param>
        public MensagemDeErro(int codigoErro, string msgErro)
        {
            this.Codigo = codigoErro;
         
            this.Mensagem = msgErro;
            this.EhExcecao = false;
        }

        #endregion

        #region Propriedades

        public int Codigo { get; private set; }
        public string Mensagem { get; private set; }
        public bool EhExcecao { get; private set; }

        #endregion
        /// <summary>
        /// Redefinir o método toString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Codigo.ToString() + " - " + Mensagem;
        }
    }
}
