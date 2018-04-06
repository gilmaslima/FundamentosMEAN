using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Agentes
{
    public class CartaSolicitacao : AgentesBase
    {
        /// <summary>
        /// Retorna a carta da solicitação cadastrada
        /// </summary>
        /// <param name="numeroSolicitacao">Número de solicitação</param>
        /// <param name="codigoTipoServico">Código do tipo de Serviço</param>
        /// <param name="linhasCarta">Objeto com o conteúdo da carta em linhas</param>
        /// <param name="quantidadeLinhasCarta">Quantidade de linhas na carta</param>
        /// <param name="codigoRetorno">Código de retorno do mainframe</param>
        public void Manutencao(Decimal numeroSolicitacao, String codigoTipoServico, out Modelo.CartaSolicitacao linhasCarta, out Int16 quantidadeLinhasCarta,
                out Int16 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Retorna a carta da solicitação cadastrada"))
            {
                Log.GravarLog(EventoLog.InicioAgente);

                try
                {
                    using (var COMTI = new COMTIWMOutrosServicos.COMTIWMClient())
                    {
                        COMTIWMOutrosServicos.COM01_PARM_LINHAS[] linhasCartaCOMTI = new COMTIWMOutrosServicos.COM01_PARM_LINHAS[200];
                        codigoRetorno = 0;
                        quantidadeLinhasCarta = 0;

#if !DEBUG
                    Int16 codigoTexto = 0;
                    Int16 quantidadeLinhas = 0;
                    Int16 codigoSistema = 0;
                    Int32 codigoEstabelecimento = 0;
                    String codigoUsuario = "ISUSER";
                    Int32 codigoComponente = 0;
                    String impressao = "N";
                    Int16 sequencia = 0;
                    Int32 cep = 0;
                    String retornoPrograma = "";
                    Int16 retornoCodigoTexto = 0;
                    Int16 retornoCodigoSistema = 0;
                    String retornoCodigoUsuario = "";
                    Int32 retornoCodigoComponente = 0;
                    String retornoImpressao = "";
                    String retornoBanner = "";
                    String mensagemRetorno = "";
                    COMTIWMOutrosServicos.COM01_RETWMT58_ERRO retornoMensagemErro = new COMTIWMOutrosServicos.COM01_RETWMT58_ERRO();
                        Log.GravarLog(EventoLog.ChamadaHIS, new
                        {
                            codigoTipoServico,
                            codigoTexto,
                            codigoSistema,
                            quantidadeLinhas,
                            codigoEstabelecimento,
                            codigoUsuario,
                            codigoComponente,
                            numeroSolicitacao,
                            impressao,
                            sequencia,
                            cep
                        });

                    linhasCartaCOMTI = COMTI.Manutencao(out retornoPrograma, out retornoCodigoTexto, out retornoCodigoSistema, out codigoRetorno, out quantidadeLinhasCarta, out retornoCodigoUsuario,
                                    out retornoCodigoComponente, out retornoImpressao, out retornoBanner, out mensagemRetorno, out retornoMensagemErro,
                                    codigoTipoServico, codigoTexto, codigoSistema, quantidadeLinhas, codigoEstabelecimento, codigoUsuario, codigoComponente, numeroSolicitacao,
                                    impressao, sequencia, cep);

                        Log.GravarLog(EventoLog.RetornoHIS, new { retornoPrograma, retornoCodigoTexto, retornoCodigoSistema, codigoRetorno, quantidadeLinhasCarta, retornoCodigoUsuario,
                                    retornoCodigoComponente, retornoImpressao, retornoBanner, mensagemRetorno, retornoMensagemErro });

#endif
                        linhasCarta = PreencherModeloCarta(linhasCartaCOMTI);
                        Log.GravarLog(EventoLog.FimAgente, new { linhasCarta });

                    }

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        private Modelo.CartaSolicitacao PreencherModeloCarta(COMTIWMOutrosServicos.COM01_PARM_LINHAS[] linhasCarta)
        {


            try
            {
                Modelo.CartaSolicitacao carta = null;
#if !DEBUG
                if (linhasCarta.Length > 0)
                {
                    carta = new Modelo.CartaSolicitacao();
                    carta.LinhasCarta = new List<string>();
                    
                    foreach (COMTIWMOutrosServicos.COM01_PARM_LINHAS linha in linhasCarta)
                    {
                        carta.LinhasCarta.Add(linha.COM01_PARM_LIN_TX);
                    }
                }
#else
                carta = new Modelo.CartaSolicitacao();
                carta.LinhasCarta = new List<string>();

                carta.LinhasCarta.Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris bibendum sagittis molestie. Proin aliquet lacus porta mi interdum et vestibulum nisi consectetur. Praesent a nisi et purus vulputate aliquam. Mauris pharetra imperdiet augue non accumsan. Proin turpis dolor, condimentum quis porttitor vitae, molestie in enim. Sed dignissim tempor tortor eu commodo. Aliquam quis aliquet nulla. Nam metus eros, euismod eleifend suscipit lacinia, tincidunt sed mauris.");
                carta.LinhasCarta.Add("Vestibulum lobortis, leo vitae viverra tempus, eros turpis facilisis velit, accumsan placerat erat metus ut felis. Sed id augue risus. In hac habitasse platea dictumst. Nullam quis felis turpis, et consequat eros. Aenean turpis justo, fermentum sit amet viverra quis, vulputate ac orci. Morbi lacus lacus, pulvinar tempor rutrum id, blandit quis metus. Pellentesque sagittis, sapien cursus lacinia volutpat, quam eros lacinia tellus, et vehicula mauris diam eget est.");
                carta.LinhasCarta.Add("Aliquam ultricies arcu quis ante iaculis a ullamcorper diam tempus. Quisque mollis nunc vitae nisi convallis pulvinar. Quisque pharetra justo vel orci fermentum vitae egestas urna hendrerit. Nam at felis magna. Mauris et magna est. Pellentesque a augue vitae sem tempus porta. Aliquam eu risus quis lacus aliquet eleifend eget eu tellus. Duis nec elit ligula. Cras ornare risus nec mi elementum sit amet facilisis turpis consequat. In hac habitasse platea dictumst. Proin nunc justo, laoreet non sodales et, ultricies nec eros. Aenean tincidunt purus et leo egestas tincidunt. Nam non enim nisi, nec convallis leo.");
#endif
                return carta;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}
