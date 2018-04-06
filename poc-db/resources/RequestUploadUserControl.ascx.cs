using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Request.SharePoint.ImagensServico;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Specialized;


namespace Redecard.PN.Request.SharePoint.WebParts.RequestUpload
{
    public partial class RequestUploadUserControl : UserControlBase
    {
        /// <summary>Variável para armazenar os nomes dos arquivos carregados.
        /// (String,String) = (NomeArquivoOriginal,NomeArquivoServidor)</summary>
        private List<KeyValuePair<String, String>> ListaArquivos
        {
            get
            {
                if (ViewState["ListaArquivos"] == null)
                    ListaArquivos = new List<KeyValuePair<String, String>>();
                return (List<KeyValuePair<String, String>>)ViewState["ListaArquivos"];
            }
            set
            {
                ViewState["ListaArquivos"] = value;
            }
        }

        private String FlagNSU
        {
            get { return (String)ViewState["FlagNSU"]; }
            set { ViewState["FlagNSU"] = value; }
        }

        public List<String> ExtensoesValidas
        {
            get
            {
                List<String> listaExtensoes = new List<String>();
                listaExtensoes.AddRange(new String[] { ".png", ".jpg", ".jpeg", ".tiff", ".tif", ".pdf" });
                return listaExtensoes;
            }
            set { }
        }

        private QueryStringSegura QS
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                    return new QueryStringSegura(Request.QueryString["dados"]);
                else
                    return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                btnConfirmar.Visible = false;
                if (QS != null)
                {
                    //decodificação da query string com os dados do chargeback                    
                    FlagNSU = QS["flgNSU"];

                    CarregarSelecionado(QS);

                    //lnkCarta.Attributes.Add("data-url", FormarUrl(1));
                    //lnkEnvelope.Attributes.Add("data-url", FormarUrl(2));
                    //lnkCartaEnvelope.Attributes.Add("data-url", FormarUrl(3));
                }
                else
                {
                    SetarAviso("Erro ao recuperar as informações passadas via QueryString. Por favor, digite os dados desejados ou retorne a tela de Comprovantes Pendentes para selecionar um processo.");

                    SwitchLabels(false);
                    SwitchTexts(true);
                }
            }
            else { SetarAviso(string.Empty); }
        }

        #region Negócio e Carga de Dados
        /// <summary>
        /// carrega na tabela o Request selecionado na página de Requests Pendentes
        /// </summary>
        /// <param name="queryString"></param>
        private void CarregarSelecionado(QueryStringSegura queryString)
        {
            txtNumProcessoSel.Text = lblNumProcessoSel.Text = queryString["NumProcesso"];
            txtNumEstSel.Text = lblNumEstSel.Text = queryString["Estabelecimento"];
            txtResumoVendasSel.Text = lblResumoVendasSel.Text = queryString["ResumoVendas"];
            txtNumCartaoSel.Text = lblNumCartaoSel.Text = queryString["NumCartao"];
            txtDataVendaSel.Text = lblDataVendaSel.Text = queryString["DataVenda"];
            txtValorVendaSel.Text = lblValorVendaSel.Text = queryString["ValorVenda"];
            txtReferenciaSel.Text = lblReferenciaSel.Text = queryString["Referencia"];
            SwitchLabels(true);
            SwitchTexts(false);
        }


        /// <summary>
        /// Carrega os arquivos gravados na sessão no repeater
        /// </summary>
        private void CarregarListaArquivos()
        {
            var possuiItens = this.ListaArquivos.Count > 0;
            btnConfirmar.Visible = possuiItens;
            rptArquivos.Visible = possuiItens;
            lblVazio.Visible = !possuiItens;
            rptArquivos.DataSource = ListaArquivos.Select(arq => arq.Key).ToArray();
            rptArquivos.DataBind();
        }

        /// <summary>
        ///  Mostra ou esconde as labels com os dados
        /// </summary>
        private void SwitchLabels(bool mostrar)
        {
            lblNumProcessoSel.Visible = mostrar;
            lblNumEstSel.Visible = mostrar;
            lblResumoVendasSel.Visible = mostrar;
            lblNumCartaoSel.Visible = mostrar;
            lblDataVendaSel.Visible = mostrar;
            lblValorVendaSel.Visible = mostrar;
            lblReferenciaSel.Visible = mostrar;
        }

        /// <summary>
        /// Mostra ou esconde as textBoxes
        /// </summary>
        /// <param name="mostrar"></param>
        private void SwitchTexts(bool mostrar)
        {
            txtNumProcessoSel.Visible = mostrar;
            txtNumEstSel.Visible = mostrar;
            txtResumoVendasSel.Visible = mostrar;
            txtNumCartaoSel.Visible = mostrar;
            txtDataVendaSel.Visible = mostrar;
            txtValorVendaSel.Visible = mostrar;
            txtReferenciaSel.Visible = mostrar;
        }

        /// <summary>
        /// Monta a url para imprimir a carta ou envelope, recebendo como parametro o tipo
        /// </summary>
        /// <param name="tipo">Carta = 1, Envelope = 2, CartaEnvelope = 3</param>
        /// <returns></returns>
        public string FormarUrl(Int32 tipo)
        {
            QueryStringSegura queryString = new QueryStringSegura();

            queryString["FlagNSU"] = this.FlagNSU;

            if (txtNumProcessoSel.Visible == false)
            {
                queryString["Processo"] = lblNumProcessoSel.Text;
                queryString["DataVenda"] = lblDataVendaSel.Text;
                queryString["ResumoVendas"] = lblResumoVendasSel.Text;
                queryString["NumCartao"] = lblNumCartaoSel.Text;
                queryString["ValorVenda"] = lblValorVendaSel.Text;
                queryString["NumPV"] = lblNumEstSel.Text;
            }
            else
            {
                queryString["Processo"] = txtNumProcessoSel.Text;
                queryString["DataVenda"] = txtDataVendaSel.Text;
                queryString["ResumoVendas"] = txtResumoVendasSel.Text;
                queryString["NumCartao"] = txtNumCartaoSel.Text;
                queryString["ValorVenda"] = txtValorVendaSel.Text;
                queryString["NumPV"] = txtNumEstSel.Text;
            }

            switch (tipo)
            {
                case 1: queryString["Modo"] = "Carta"; break;
                case 2: queryString["Modo"] = "Envelope"; break;
                case 3: queryString["Modo"] = "CartaEnvelope"; break;
            }

            return String.Format(base.web.ServerRelativeUrl + "/_layouts/Request/CartaEnvelope.aspx?dados={0}", queryString.ToString());
        }

        private bool ValidarTxt()
        {
            if (txtDataVendaSel.Visible == true && (
                txtDataVendaSel.Text.Trim() == "" ||
                txtNumCartaoSel.Text.Trim() == "" ||
                txtNumEstSel.Text.Trim() == "" ||
                txtNumProcessoSel.Text.Trim() == "" ||
                txtReferenciaSel.Text.Trim() == "" ||
                txtResumoVendasSel.Text.Trim() == "" ||
                txtValorVendaSel.Text.Trim() == ""))
                return false;
            return true;
        }

        #endregion

        #region Handlers

        private String MontarNomeArquivo(Int32 indice, String extensao)
        {
            String data = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            String numeroPV = lblNumEstSel.Text;
            String processo = lblNumProcessoSel.Text;

            if (String.IsNullOrEmpty(extensao))
                extensao = Path.GetExtension(fluComprovante.PostedFile.FileName);

            //Nome do arquivo: <YYYYMMDDHHNNSS>_<nroPV>_<processo>_<chave>_<indice>.<extensão>
            return String.Format("{0}_{1}_{2}_{3}{4}",
                data, numeroPV, processo, indice, extensao);
        }

        /// <summary>Envia o arquivo para o servidor</summary>        
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Envio de Comprovante de Venda"))
            {
                //verificação se há arquivo selecionado
                if (fluComprovante.HasFile)
                {
                    try
                    {
                        SetarAviso(string.Empty);

                        //Se o arquivo não possuir uma extensão válida ou for maior do que 1024 KB (1MB), 
                        if (!ExtensoesValidas.Contains(Path.GetExtension(fluComprovante.PostedFile.FileName)))
                        {
                            SetarAviso("Por favor, selecione um arquivo com um formato válido.");
                        }
                        else if ((fluComprovante.FileBytes.Length / 1024) > 1024)
                        {
                            SetarAviso("Por favor, selecione um arquivo com tamanho inferior ou igual a 1MB.");
                        }
                        else
                        {
                            //Instanciação do client para tratamento de imagens
                            Log.GravarMensagem("Instanciando serviço de Imagem");

                            using (ImagemServicoClient client = new ImagemServicoClient())
                            {
                                //Envia o arquivo para o servidor
                                String nomeArquivo = MontarNomeArquivo(ListaArquivos.Count, null);

                                Log.GravarLog(EventoLog.ChamadaServico, new { tamanhoBytes = fluComprovante.FileContent.Length, nomeArquivo });
                                nomeArquivo = client.EnviarImagem(fluComprovante.FileBytes, nomeArquivo);
                                Log.GravarLog(EventoLog.RetornoServico, new { nomeArquivo });
                                if (!String.IsNullOrEmpty(nomeArquivo))
                                    this.ListaArquivos.Add(new KeyValuePair<String, String>(fluComprovante.FileName, nomeArquivo));
                                else
                                    SetarAviso(String.Format("O arquivo {0} não é uma imagem válida!", Path.GetFileName(fluComprovante.FileName)));
                            }
                            if (ListaArquivos.Count == 10)
                            {
                                fluComprovante.Enabled = false;
                                btnEnviar.Enabled = false;
                            }
                        }
                    }
                    catch (FaultException<ImagensServico.GeneralFault> ex)
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
                //Caso clique sem arquivo selecionado
                else
                {
                    SetarAviso("Por favor, selecione um arquivo para enviar.");
                }
                CarregarListaArquivos();
            }
        }

        /// <summary>Acionado a cada arquivo que é adicionado no repeater, ajusta o botão de exclusão e as labels</summary>        
        protected void rptArquivos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //SetarAviso(string.Empty);
                var lblNomeArquivo = (Label)e.Item.FindControl("lblNomeArquivo");
                var btnExcluirArquivo = (LinkButton)e.Item.FindControl("btnExcluirArquivo");

                lblNomeArquivo.Text = (string)e.Item.DataItem;
                btnExcluirArquivo.CommandArgument = e.Item.ItemIndex.ToString();
            }
        }

        /// <summary>Handler dos botões de exclusão dos arquivos</summary>        
        protected void btnExcluirArquivo_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Exclusão de arquivo de Comprovante de Venda"))
            {
                LinkButton btnExcluir = (LinkButton)sender;
                Int32? itemIndex = btnExcluir.CommandArgument.ToInt32Null();

                if (itemIndex.HasValue)
                {
                    try
                    {
                        KeyValuePair<String, String> item = ListaArquivos[itemIndex.Value];

                        //Client de tratamento de imagem
                        ImagemServicoClient client = new ImagemServicoClient();

                        Log.GravarLog(EventoLog.ChamadaServico, new { nomeArquivo = item.Value });
                        //Apaga o arquivo e remove da "sessão"
                        if (client.ApagarArquivo(item.Value))
                            ListaArquivos.RemoveAt(itemIndex.Value);
                        if (ListaArquivos.Count < 10)
                        {
                            fluComprovante.Enabled = true;
                            btnEnviar.Enabled = true;
                        }

                    }
                    catch (FaultException<ImagensServico.GeneralFault> ex)
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
                CarregarListaArquivos();
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Upload de Comprovantes de Venda"))
            {
                //validação - caso não tenha carregado nenhum arquivo
                if (ListaArquivos.Count == 0)
                {
                    Log.GravarMensagem("Nenhum arquivo carregado para envio");
                    SetarAviso("Não há nenhum arquivo carregado para enviar.");
                }
                //validação das informações digitadas nos textboxes
                else if (txtDataVendaSel.Visible == true && (
                    txtDataVendaSel.Text.Trim() == "" ||
                    txtNumCartaoSel.Text.Trim() == "" ||
                    txtNumEstSel.Text.Trim() == "" ||
                    txtNumProcessoSel.Text.Trim() == "" ||
                    txtReferenciaSel.Text.Trim() == "" ||
                    txtResumoVendasSel.Text.Trim() == "" ||
                    txtValorVendaSel.Text.Trim() == ""))
                {
                    lblValidaTxt.Visible = true;
                }
                else
                {
                    lblValidaTxt.Visible = false;

                    try
                    {
                        //Inserindo no array os arquivos .PDF
                        List<String> listaArquivosPDF = ListaArquivos.Select(x => x.Value).Where(val => val.ToString().Contains(".pdf")).ToList();

                        //Removendo da lista os arquivos .PDF, para que os mesmos sejam inseridos separadamente das imagens.
                        ListaArquivos.RemoveAll(x => x.Value.Contains(".pdf"));

                        //Montando o nome do arquivo .TIF que será salvo no bando.
                        String imgArquivoTiff = MontarNomeArquivo(99, ".tif");

                        String data = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        Int32 id = 72; //Valor fixo (72) definido pela Synergy (central de fax)
                        String[] arquivosImagem = ListaArquivos.Select(arq => arq.Value).ToArray();
                        String processo = txtNumProcessoSel.Text;

                        //Instancia o serviço de recebimento e tratamento de imagens
                        using (ImagemServicoClient client = new ImagemServicoClient())
                        {
                            Log.GravarLog(EventoLog.ChamadaServico, new { arquivos = arquivosImagem, imgArquivoTiff });

                            //Consolida as imagens em um único arquivo
                            if (arquivosImagem.Length > 0)
                            {
                                client.ConsolidarImagensPagina(arquivosImagem, imgArquivoTiff);
                            }

                            Log.GravarLog(EventoLog.ChamadaServico, new { imgArquivoTiff, data, id, processo });

                            //Grava a imagem no servidor de banco de dados
                            if (ListaArquivos.Count > 0)
                            {
                                client.GravarImagens(imgArquivoTiff, data, id, processo);
                            }

                            //Grava os arquivos PDF no servidor de banco de dados.
                            foreach (var arquivo in listaArquivosPDF)
                            {
                                Log.GravarLog(EventoLog.ChamadaServico, new { arquivo });
                                client.GravarImagens(arquivo, data, id, processo);
                            }

                            //Remove as imagens temporárias fisicamente do servidor e limpa a variável de página
                            foreach (var arquivo in ListaArquivos)
                            {
                                Log.GravarLog(EventoLog.ChamadaServico, new { arquivo });
                                client.ApagarArquivo(arquivo.Value);
                            }

                            foreach (var arquivo in listaArquivosPDF)
                            {
                                Log.GravarLog(EventoLog.ChamadaServico, new { arquivo });
                                client.ApagarArquivo(arquivo);
                            }

                            Log.GravarLog(EventoLog.ChamadaServico, new { imgArquivoTiff });
                            client.ApagarArquivo(imgArquivoTiff);
                            ListaArquivos.Clear();
                            listaArquivosPDF.Clear();
                        }

                        //Redireciona para a tela de protocolo do envio dos documentos
                        QueryStringSegura queryString = new QueryStringSegura();
                        queryString["DataEnvio"] = DateTime.Now.ToString("dd/MM/yyyy");
                        queryString["HoraEnvio"] = DateTime.Now.ToString("HH'h'mm");

                        if (txtDataVendaSel.Visible == true)
                        {
                            queryString["NumEstabelecimento"] = txtNumEstSel.Text;
                            queryString["Documentos"] = txtNumProcessoSel.Text;
                        }
                        else
                        {
                            queryString["NumEstabelecimento"] = lblNumEstSel.Text;
                            queryString["Documentos"] = lblNumProcessoSel.Text;
                        }

                        //Registra no histórico/log de atividades
                        {
                            var descricaoServico = new StringBuilder("Comprovação de Vendas");
                            if (this.QS == null)
                                descricaoServico.Append(" - Manual");
                            else if (String.Compare("Credito", QS["TipoVenda"], true) == 0)
                                descricaoServico.Append(" - Crédito");
                            else if (String.Compare("Debito", QS["TipoVenda"], true) == 0)
                                descricaoServico.Append(" - Débito");
                            Historico.RealizacaoServico(SessaoAtual, descricaoServico.ToString());
                        }

                        Response.Redirect(String.Format(base.web.ServerRelativeUrl + "/Paginas/pn_ProtocoloEnvio.aspx?dados={0}", queryString.ToString()));
                    }
                    catch (FaultException<ImagensServico.GeneralFault> ex)
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

        //protected void links_Click(object sender, EventArgs e)
        //{
        //    lblValidaTxt.Visible = false;
        //    if (txtDataVendaSel.Visible == true && lnkCarta.OnClientClick == "")
        //    {
        //        if (ValidarTxt())
        //        {
        //            lnkCarta.Attributes.Add("data-url", FormarUrl(1));
        //            lnkEnvelope.Attributes.Add("data-url", FormarUrl(2));
        //            lnkCartaEnvelope.Attributes.Add("data-url", FormarUrl(3));

        //            LinkButton botao = (LinkButton)sender;
        //            StringBuilder script = new StringBuilder();
        //            if (botao.Text.Equals("Carta"))
        //            {

        //                script.Append("ExecuteOrDelayUntilScriptLoaded(function () { abrirPopUp(\"");
        //                script.Append(FormarUrl(1));
        //                script.Append("\"); }, 'SP.UI.Dialog.js'); ");
        //            }
        //            if (botao.Text.Equals("Envelope"))
        //            {
        //                script.Append("ExecuteOrDelayUntilScriptLoaded(function () { abrirPopUp(\"");
        //                script.Append(FormarUrl(2));
        //                script.Append("\"); }, 'SP.UI.Dialog.js'); ");
        //            }
        //            if (botao.Text.Equals("CartaEnvelope"))
        //            {
        //                script.Append("ExecuteOrDelayUntilScriptLoaded(function () { abrirPopUp(\"");
        //                script.Append(FormarUrl(3));
        //                script.Append("\"); }, 'SP.UI.Dialog.js'); ");
        //            }

        //            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), botao.Text, script.ToString(), true);
        //        }
        //        else
        //        {
        //            lblValidaTxt.Visible = true;
        //        }
        //    }
        //}

        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        private void SetarAviso(String aviso)
        {
            if (!String.IsNullOrEmpty(aviso))
                aviso += "<br/><br/>";
            lblValidaConfirmar.Text = aviso;
        }
    }
}
