using System;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ConsultaPorTransacao;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ConsultaTransacao;
using Redecard.PN.Extrato.SharePoint.Helper;

namespace Redecard.PN.Extrato.SharePoint.ConsultaTransacao
{
    public partial class ConsultaTransacaoUserControl : BaseUserControl
    {
        protected Credito consultaPorTransacaoCreditoTID;
        protected Debito consultaPorTransacaoDebitoTID;
        protected Filtro consultaPorTransacaoFiltroTID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {             
                MultViewConsultaTransacao.SetActiveView(ViewConsultaTransacaoFiltroTID);

                if (QS != null) //Integração com requisiões da Home EMP/IBBA
                {
                    Int32 numero = QS["Numero"].ToInt32();
                    String tipoConsulta = QS["TipoConsulta"].ToUpper();

                    //Recupera dados da querystring
                    var dadosConsultaDTO = new TransacaoDadosConsultaDTO();
                    dadosConsultaDTO.DataInicial = QS["DataInicial"].ToDate("dd/MM/yyyy");
                    dadosConsultaDTO.DataFinal = QS["DataFinal"].ToDate("dd/MM/yyyy");
                    dadosConsultaDTO.TipoVenda = QS["TipoVenda"];
                    dadosConsultaDTO.NumeroEstabelecimento = QS["NumeroPv"].ToInt32();
                    switch (tipoConsulta)
                    {
                        case "CARTAO":
                            dadosConsultaDTO.NumeroCartao = numero.ToString();
                            break;
                        case "NSUCV":
                            dadosConsultaDTO.Nsu = numero;
                            break;
                        case "TID":
                        default:
                            dadosConsultaDTO.TID = numero.ToString();
                            break;
                    }
                    
                    //Carrega os dados da consulta com os parâmetros recebidos da Home
                    consultaPorTransacaoFiltro_ItemSelecionado(dadosConsultaDTO, e);
                }
            }
        }

        protected void consultaPorTransacaoFiltro_ItemSelecionado(TransacaoDadosConsultaDTO dadosConsultaDTO, EventArgs e)
        {
            if (dadosConsultaDTO.TipoVenda == "C")
            {
                using (Logger Log = Logger.IniciarLog("Consulta por Transação - Crédito"))
                {
                    consultaPorTransacaoCreditoTID.ConsultarTransacao(dadosConsultaDTO);
                    MultViewConsultaTransacao.SetActiveView(ViewConsultaTransacaoCredito);
                }
            }
            else if (dadosConsultaDTO.TipoVenda == "D")
            {
                using (Logger Log = Logger.IniciarLog("Consulta por Transação - Débito"))
                {
                    consultaPorTransacaoDebitoTID.ConsultarTransacao(dadosConsultaDTO);
                    MultViewConsultaTransacao.SetActiveView(ViewConsultaTransacaoDebito);
                }
            }
        }
    }
}
