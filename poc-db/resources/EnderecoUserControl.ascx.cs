using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using System;
using System.ServiceModel;
using System.Web.UI;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Endereco
{
    public partial class EnderecoUserControl : UserControlBase
    {
        /// <summary>
        /// Indica se a página esta sendo editada ou nao
        /// </summary>
        protected Boolean ModoEdicao
        {
            get
            {
                if (Session["informacoescadastraismodoedicao"] != null)
                {
                    return Convert.ToBoolean(Session["informacoescadastraismodoedicao"]);
                }
                return false;
            }
            set
            {
                Session["informacoescadastraismodoedicao"] = value;
            }
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Endereço - Carregando página"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        CarregarEnderecos();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AtualizarEdicaoPagina();
        }

        /// <summary>
        /// Carregar os endereços de Estabelecimento e Correspondência na tela
        /// </summary>
        private void CarregarEnderecos()
        {
            using (Logger Log = Logger.IniciarLog("Carregando endereços"))
            {
                using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                {
                    CarregarEnderecoEstabelecimento(entidadeCliente);
                    CarregarEnderecoCorrespondencia(entidadeCliente);
                }
            }
        }

        /// <summary>
        /// Carregar o endereço de Estabelecimento na tela
        /// </summary>
        private void CarregarEnderecoEstabelecimento(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            using (Logger Log = Logger.IniciarLog("Carregando endereços estabelecimento"))
            {
                try
                {
                    Int32 codigoRetorno;
                    var enderecoEstabelecimento = entidadeCliente.ConsultarEndereco(out codigoRetorno, SessaoAtual.CodigoEntidade, "E")[0];

                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["TipoDomicilio"] = "E";

                    //lnkEnderecoEstabelecimento.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_CadastroEndereco.aspx?dados={0}", queryString.ToString());
                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarEndereco", codigoRetorno);
                    }
                    else
                    {
                        if (enderecoEstabelecimento != null)
                        {
                            lblEstabelecimentoRua.Text = enderecoEstabelecimento.EnderecoEstabelecimento;
                            lblEstabelecimentoNumero.Text = enderecoEstabelecimento.Numero.TratarValorNulo();
                            lblEstabelecimentoComplemento.Text = enderecoEstabelecimento.Complemento;
                            lblEstabelecimentoBairro.Text = enderecoEstabelecimento.Bairro;
                            lblEstabelecimentoCEP.Text = enderecoEstabelecimento.CEP;
                            lblEstabelecimentoCidade.Text = enderecoEstabelecimento.Cidade;
                            lblEstabelecimentoUF.Text = enderecoEstabelecimento.UF;
                            lblEstabelecimentoContato.Text = enderecoEstabelecimento.Contato;
                            lblEstabelecimentoTelefone.Text = enderecoEstabelecimento.Telefone.FormatarTelefone();
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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

        /// <summary>
        /// Carregar o endereço de Correspondência na tela
        /// </summary>
        private void CarregarEnderecoCorrespondencia(EntidadeServico.EntidadeServicoClient entidadeCliente)
        {
            using (Logger Log = Logger.IniciarLog("Carregando endereço correspondência"))
            {
                try
                {
                    Int32 codigoRetorno;
                    var enderecoCorrespondencia = entidadeCliente.ConsultarEndereco(out codigoRetorno, SessaoAtual.CodigoEntidade, "C")[0];

                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["TipoDomicilio"] = "C";

                    //lnkEnderecoCorrespondencia.NavigateUrl = String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_CadastroEndereco.aspx?dados={0}", queryString.ToString());
                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarEndereco", codigoRetorno);
                    }
                    else
                    {
                        if (enderecoCorrespondencia != null)
                        {
                            lblCorrespondenciaRua.Text = enderecoCorrespondencia.EnderecoEstabelecimento;
                            lblCorrespondenciaNumero.Text = enderecoCorrespondencia.Numero.TratarValorNulo();
                            lblCorrespondenciaComplemento.Text = enderecoCorrespondencia.Complemento;
                            lblCorrespondenciaBairro.Text = enderecoCorrespondencia.Bairro;
                            lblCorrespondenciaCEP.Text = enderecoCorrespondencia.CEP;
                            lblCorrespondenciaCidade.Text = enderecoCorrespondencia.Cidade;
                            lblCorrespondenciaUF.Text = enderecoCorrespondencia.UF;
                            lblCorrespondenciaContato.Text = enderecoCorrespondencia.Contato;
                            lblCorrespondenciaTelefone.Text = enderecoCorrespondencia.Telefone.FormatarTelefone();
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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

        /// <summary>
        /// Habilita/desabilita os botoes para que a pagina possa ser editada
        /// </summary>
        private void AtualizarEdicaoPagina()
        {
            //lnkBtnEditarEnderecos.Visible = !this.ModoEdicao;
        }
    }
}
