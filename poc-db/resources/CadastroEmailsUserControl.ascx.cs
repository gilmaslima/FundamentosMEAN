/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Daniel]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Daniel] – [Etapa inicial]
*/
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Collections.Generic;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;

namespace Redecard.PN.RAV.Sharepoint.WebParts.CadastroEmails
{
    public partial class CadastroEmailsUserControl : UserControlBase
    {
        #region Constantes
        private const string FONTE = "CadastroEmailsUserControl.ascx";
        private const int COGIDO_ERRO_LOAD = 3002;
        private const int CODIGO_ERRO_CADASTRAR = 3003;
        #endregion

        #region Atributos
        private string _validaSenha = bool.FalseString;
        private string _periodicidade = "";

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cadastro E-mails - Page Load"))
            {
                try
                {
                    if (Request.QueryString["dados"] != null)
                    {
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                        if (string.IsNullOrEmpty(queryString["AcessoSenha"]))
                        { Response.Redirect("pn_rav.aspx"); }
                        if (queryString["AcessoSenha"].CompareTo(bool.TrueString) != 0) { Response.Redirect("pn_rav.aspx"); }

                        SharePointUlsLog.LogMensagem(queryString["AcessoSenha"]);
                        Log.GravarMensagem(queryString["AcessoSenha"]);

                        _validaSenha = queryString["AcessoSenha"];
                    }

                    if (!Page.IsPostBack)
                    {
                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                        {
                            Button2.Visible = false;
                        }

                        lblMsg.Visible = false;
                        using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                        {
                            ModRAVEmailEntradaSaida ravEmail = cliente.ConsultarEmails(SessaoAtual.CodigoEntidade);

                            if (ravEmail != null)
                            {
                                if (ravEmail.ListaEmails != null)
                                {
                                    for (int i = 0; i < ravEmail.ListaEmails.Count; i++)
                                    {
                                        if (i < 3)
                                        {
                                            int emailSeq = ravEmail.ListaEmails[i].Sequencia;//int emailSeq = ravEmail.ListaEmails[i].Sequencia + 1;
                                            if (ravEmail.ListaEmails[i].Periodicidade == EPeriodicidadeEmail.Diario)
                                            {
                                                _periodicidade = "D";
                                            }
                                            else if (ravEmail.ListaEmails[i].Periodicidade == EPeriodicidadeEmail.Semanal)
                                            {
                                                _periodicidade = "S";
                                            }
                                            else if (ravEmail.ListaEmails[i].Periodicidade == EPeriodicidadeEmail.Quinzenal)
                                            {
                                                _periodicidade = "Q";
                                            }
                                            else
                                            {
                                                _periodicidade = "M";
                                            }
                                            (FindControl("txtEmail" + emailSeq.ToString()) as TextBox).Text = ravEmail.ListaEmails[i].Email;

                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_LOAD);
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a página Principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cadastro E-mails - Voltar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["AcessoSenha"] = bool.TrueString;
                    Response.Redirect(string.Format("pn_Principal.aspx?dados={0}", queryString.ToString()));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }


        protected EPeriodicidadeEmail retornaValorPeriodicidade(string periodicidade)
        {
            EPeriodicidadeEmail retorno = EPeriodicidadeEmail.Diario;

            if (periodicidade == "D")
            {
                retorno = EPeriodicidadeEmail.Diario;
            }
            else if (periodicidade == "S")
            {
                retorno = EPeriodicidadeEmail.Semanal;
            }

            else if (periodicidade == "Q")
            {
                retorno = EPeriodicidadeEmail.Quinzenal;
            }
            else
            {
                retorno = EPeriodicidadeEmail.Mensal;
            }

            return retorno;

        }

        /// <summary>
        /// Cadastra os emails digitados e retorna para a página Principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cadastrar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Cadastro E-mails - Cadastrar"))
            {
                try
                {
                    if (ckbAutorizacao.Checked == true)
                    {
                        //QueryStringSegura queryString = new QueryStringSegura();
                        //if (ckbAutorizacao.Checked == true)
                        //{
                        //    queryString["EnviaEmail"] = "true";
                        //}
                        //else
                        //{
                        //    queryString["EnviaEmail"] = "false";
                        //}

                        EPeriodicidadeEmail periodicidadeEmail = EPeriodicidadeEmail.Diario;
                        if (rbtPeriodo_Diario.Checked == true)
                        { periodicidadeEmail = EPeriodicidadeEmail.Diario; }
                        else if (rbtPeriodo_Semanal.Checked == true)
                        { periodicidadeEmail = EPeriodicidadeEmail.Semanal; }
                        else if (rbtPeriodo_Quinzenal.Checked == true)
                        { periodicidadeEmail = EPeriodicidadeEmail.Quinzenal; }
                        else if (rbtPeriodo_Mensal.Checked == true)
                        { periodicidadeEmail = EPeriodicidadeEmail.Mensal; }

                        ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();

                        List<ModRAVEmail> ravEmailList = cliente.ConsultarEmails(SessaoAtual.CodigoEntidade).ListaEmails;

                        ModRAVEmailEntradaSaida ravEmail = new ModRAVEmailEntradaSaida();
                        ravEmail.NumeroPDV = SessaoAtual.CodigoEntidade;
                        if (ckbAutorizacao.Checked == true)
                        {
                            ravEmail.IndEnviaEmail = 'S';
                        }
                        else
                        {
                            ravEmail.IndEnviaEmail = 'N';
                        }
                        ravEmail.IndEnviaFluxoCaixa = ' ';
                        ravEmail.IndEnviaValoresPV = ' ';
                        ravEmail.IndEnviaResumoOperacao = ' ';
                        ravEmail.ListaEmails = new List<ModRAVEmail>();

                        //Validação da mudança no email 1
                        if (txtEmail1.Text != "")
                        {
                            if (ravEmailList.Count > 0 && ravEmailList.Where(x => x.Sequencia == 1).FirstOrDefault() != null)
                            {
                                if (txtEmail1.Text.CompareTo(ravEmailList.Where(x => x.Sequencia == 1).FirstOrDefault().Email) != 0)
                                {
                                    ravEmail.ListaEmails.Add(new ModRAVEmail()
                                    {
                                        Sequencia = 1,
                                        Email = txtEmail1.Text,
                                        Status = EStatusEmail.Alterado, //"I"
                                        Periodicidade = periodicidadeEmail
                                        //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                    });
                                }
                            }
                            else
                            {

                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 1,
                                    Email = txtEmail1.Text,
                                    Status = EStatusEmail.Incluso, //"I"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }
                        else
                        {
                            if (ravEmailList.Count > 0 && ravEmailList.Where(x => x.Sequencia == 1).FirstOrDefault() != null)
                            {
                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 1,
                                    Email = ravEmailList.Where(x => x.Sequencia == 1).FirstOrDefault().Email,
                                    Status = EStatusEmail.Excluido, //"E"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }

                        //Validação da mudança no email 2
                        if (txtEmail2.Text != "")
                        {
                            if (ravEmailList.Count > 1 && ravEmailList.Where(x => x.Sequencia == 2).FirstOrDefault() != null)
                            {
                                if (txtEmail2.Text.CompareTo(ravEmailList.Where(x => x.Sequencia == 2).FirstOrDefault().Email) != 0)
                                {
                                    ravEmail.ListaEmails.Add(new ModRAVEmail()
                                    {
                                        Sequencia = 2,
                                        Email = txtEmail2.Text,
                                        Status = EStatusEmail.Alterado, //"I"
                                        Periodicidade = periodicidadeEmail
                                        //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                    });
                                }
                            }
                            else
                            {
                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 2,
                                    Email = txtEmail2.Text,
                                    Status = EStatusEmail.Incluso, //"I"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }
                        else
                        {
                            if (ravEmailList.Count > 1 && ravEmailList.Where(x => x.Sequencia == 2).FirstOrDefault() != null)
                            {
                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 2,
                                    Email = ravEmailList.Where(x => x.Sequencia == 2).FirstOrDefault().Email,
                                    Status = EStatusEmail.Excluido, //"E"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }

                        //Validação da mudança no email 3
                        if (txtEmail3.Text != "")
                        {
                            if (ravEmailList.Count > 2 && ravEmailList.Where(x => x.Sequencia == 3).FirstOrDefault() != null)
                            {
                                if (txtEmail3.Text.CompareTo(ravEmailList.Where(x => x.Sequencia == 3).FirstOrDefault().Email) != 0)
                                {
                                    ravEmail.ListaEmails.Add(new ModRAVEmail()
                                    {
                                        Sequencia = 3,
                                        Email = txtEmail3.Text,
                                        Status = EStatusEmail.Alterado, //"I"
                                        Periodicidade = periodicidadeEmail
                                        //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                    });
                                }
                            }
                            else
                            {
                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 3,
                                    Email = txtEmail3.Text,
                                    Status = EStatusEmail.Incluso, //"I"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }
                        else
                        {
                            if (ravEmailList.Count > 2 && ravEmailList.Where(x => x.Sequencia == 3).FirstOrDefault() != null)
                            {
                                ravEmail.ListaEmails.Add(new ModRAVEmail()
                                {
                                    Sequencia = 3,
                                    Email = ravEmailList.Where(x => x.Sequencia == 3).FirstOrDefault().Email,
                                    Status = EStatusEmail.Excluido, //"E"
                                    Periodicidade = periodicidadeEmail
                                    //Periodicidade = retornaValorPeriodicidade(_periodicidade)
                                });
                            }
                        }

                        ravEmail.ListaEmails.ForEach(x => SharePointUlsLog.LogMensagem(string.Format("Email: {0} - Status: {1}", x.Email, x.Status)));

                        bool status = cliente.SalvarEmails(ravEmail);
                        if (status == true)
                        {
                            this.ExibirMensagem("Cadastro E-mail", "E-mail(s) cadastrado(s) com sucesso.");

                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Cadastro de E-mail Antecipação");
                        }
                        else
                        {
                            this.ExibirMensagem("Cadastro E-mail", "Erro durante o cadastro de e-mail.");
                        }
                    }
                    else
                    {
                        lblMsg.Visible = true;
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CADASTRAR);
                }
            }
        }

        /// <summary>
        /// Método para exibir painel de informações
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        private void ExibirMensagem(string titulo, string mensagem)
        {
            Panel[] paineisDados = new Panel[1]{
                            pnlDadosGerais
                    };

            QueryStringSegura queryString = new QueryStringSegura();
            queryString["AcessoSenha"] = bool.TrueString;
            pnlDadosGerais.Visible = false;

            string path = string.Empty;

            Request.UrlReferrer.AbsoluteUri.Split('/').Take(Request.UrlReferrer.AbsoluteUri.Split('/').Count() - 1).ToList().ForEach(x => path = string.Format("{0}/{1}", path, x));

            base.ExibirPainelConfirmacaoAcao(titulo, mensagem, string.Format("{1}/pn_Principal.aspx?dados={0}", queryString.ToString(), path.Substring(1)), paineisDados);
        }
    }
}
