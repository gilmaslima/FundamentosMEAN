#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [17/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarBancoEmissor
{
    public partial class ConsultarBancoEmissorUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        private const String NomeServico = "Redecard.PN.OutrosServicos.Servicos.EmissorServico";

        /// <summary>
        /// Busca dados do cartão especificado
        /// </summary>
        protected void ConsultarCartao(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Buscando dados do cartão especificado"))
            {
                try
                {
                    Int32 codigoRetorno;

                    using (var client = new EmissorServico.EmissorServicoClient())
                    {
                        EmissorServico.Cartao[] cartoes = client.ConsultarBancoEmissor(out codigoRetorno, Int32.Parse(txtNumeroBin.Text));

                        if (codigoRetorno == 0)
                        {
                            pnlDadosCartao.Visible = true;
                            if (cartoes.Length > 0)
                            {
                                rptCartoes.Visible = true;
                                pnlVazio.Visible = false;
                                rptCartoes.DataSource = cartoes;
                                rptCartoes.DataBind();
                            }
                            else
                            {
                                rptCartoes.Visible = false;
                                pnlVazio.Visible = true;
                            }
                        }
                        else
                            this.ExibirPainelExcecao(String.Concat(NomeServico, ".ConsultarCartao"), codigoRetorno);
                    }
                }
                catch (FaultException<EmissorServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}