using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.FMS.Sharepoint.WebParts.CriterioSelecaoTransacao
{
    /// <summary>
    /// Publica o serviço 'Criterio de Selecao por Transação' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class CriterioSelecaoTransacaoUserControl : BaseUserControl
    {
        #region Attributes
        /// <summary>
        /// Armazena os critérios do usuário selecionado no combo. Esta propriedade vai sendo modificada a cada Post-Back com os valores
        /// preenchidos pelo usuário.
        /// Como ela vai sendo constantemente atualiza, no final basta atualizá-la no serviço.
        /// </summary>
        private Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginRetorno Criterio
        {
            get
            {
                return (Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginRetorno)ViewState["Criterio"];
            }
            set
            {
                ViewState["Criterio"] = value;
            }
        }


        /// <summary>
        /// Armazena a Lista de MCCs que foi resultado da última busca e está na tela neste momento
        /// </summary>
        public MCC[] ListaMCCResultadoBusca
        {
            get
            {
                return (MCC[])ViewState["ListaMCCResultadoBusca"];
            }
            set
            {
                ViewState["ListaMCCResultadoBusca"] = value;
            }
        }

        /// <summary>
        /// Armazena a Lista de Range de BINs que foi resultado da última busca e está na tela neste momento
        /// </summary>
        public FaixaBin[] ListaFaixaBinResultadoBusca
        {
            get
            {
                return (FaixaBin[])ViewState["ListaFaixaBinResultadoBusca"];
            }
            set
            {
                ViewState["ListaFaixaBinResultadoBusca"] = value;
            }
        }

        #endregion

        #region Eventos da página
        /// <summary>
        /// Evento que irá ocorrer ao carregar a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Na primeira vez preenche o combo dos analistas e obtem do servico o criterio para este analista
                PreencherComboAnalistas();
                this.Criterio = CarregarDadosDoAnalista(cboAnalistas.SelectedValue);
            }
            else
            {
                // Atualiza criterio com os dados modificados da tela pelo usuário
                AtualizarCriterioComInformacoesDigitadas();
            }

            // Preenche novamente a tela
            PreencherCamposDaTelaComCriterio(Criterio);
        }
        #endregion Eventos

        #region Servicos
        /// <summary>
        /// Carrega as configurações de um analista (Criterio)
        /// </summary>
        /// <param name="loginAnalista">Login do analista</param>
        /// <returns>As configuracoes de criterio de selecao do analista</returns>
        private Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginRetorno CarregarDadosDoAnalista(String analistaSelecionado)
        {
            using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
            {
                Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginEnvio envio = new Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginEnvio()
                {
                    GrupoEntidade = GetSessaoAtual.GrupoEntidade,
                    NumeroEmissor = GetSessaoAtual.CodigoEntidade,
                    Usuario = analistaSelecionado,
                    UsuarioLogin = GetSessaoAtual.LoginUsuario
                };
                return objClient.PesquisarCriteriosSelecaoPorUsuarioLogin(envio);
            }
        }

        /// <summary>
        /// Grava na CPQD o critério já modificado pelo usuário
        /// </summary>
        private void GravarCriterioDeSelecaoNoSistema(Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginRetorno criterio)
        {
            using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
            {
                objClient.AtualizarCriteriosSelecao(GetSessaoAtual.CodigoEntidade,
                        GetSessaoAtual.LoginUsuario,
                        GetSessaoAtual.GrupoEntidade,
                        criterio);
            }
        }
        #endregion Servicos

        #region Preenche informações na tela
        /// <summary>
        /// Preecnhe combo com os analistas do Emissor
        /// </summary>
        private void PreencherComboAnalistas()
        {
            using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
            {
                string[] analistas = objClient.PesquisarUsuariosPorEmissor(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);
                cboAnalistas.Items.Clear();
                foreach (string z in analistas)
                {
                    cboAnalistas.Items.Add(new ListItem()
                    {
                        Value = z,
                        Text = z
                    });
                }
                if (!GetSessaoAtual.UsuarioMaster())
                {
                    cboAnalistas.SelectedValue = GetSessaoAtual.LoginUsuario;
                    //cboAnalistas.Enabled = false;
                    pnlAnalistas.Visible = false;
                }
            }
        }



        /// <summary>
        /// Preenche os campos da tela com os critérios de seleção atribuidos ao usuário.
        /// </summary>
        /// <param name="criterios">Critérios de seleção do usuário retornados do serviço</param>
        private void PreencherCamposDaTelaComCriterio(Servico.FMS.PesquisarCriteriosSelecaoPorUsuarioLoginRetorno criterios)
        {
            PreencherTiposAlarme(criterios.TipoAlarme);
            //PreencherTiposTransacao(criterios.TipoTransacaoSelecionadas);
            PreencherResultadoAutorizacao(criterios.ResultadoAutorizacao);
            PreencherCriterioClassificacao(criterios.CriterioClassificacao);

            txtScoreInicial.Text = criterios.InicioFaixaScore.ToString();
            txtScoreFinal.Text = criterios.FimFaixaScore.ToString();
            txtValorInicialTransacao.Text = criterios.ValorTransacaoInicial.ToString("0.00");
            txtValorFinalTransacao.Text = criterios.ValorTransacaoFinal.ToString("0.00");

            PreencherListasEntryModes(criterios.EntryModes, criterios.EntryModesSelecionados);
            PreencherListasUF(criterios.UF, criterios.UFsSelecionadas);
            PreencherListaEstabelecimentosSelecionados(criterios.EstabelecimentosSelecionados);
            PreencherListasMCC(this.ListaMCCResultadoBusca, criterios.MCCsSelecionados);
            PreencherListasRangesBin(this.ListaFaixaBinResultadoBusca, criterios.RangeBinsSelecionados);

            PreencherListaStatusCartao(criterios.SituacoesCartaoSelecionados);
        }

        /// <summary>
        /// Atualiza a lista de status do cartão.
        /// </summary>
        /// <param name="lista"></param>
        private void PreencherListaStatusCartao(SituacaoCartao[] lista)
        {
            chkAnalisado.Checked = false;
            chkNaoAnalisado.Checked = false;

            if (lista != null)
            {
                foreach (SituacaoCartao statusCartao in lista)
                {
                    switch (statusCartao)
                    {
                        case SituacaoCartao.Analisado:
                            {
                                chkAnalisado.Checked = true;
                                break;
                            }
                        case SituacaoCartao.NaoAnalisado:
                            {
                                chkNaoAnalisado.Checked = true;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Preenche os campos da tela de tipo de alarme.
        /// </summary>
        /// <param name="tiposAlarme"></param>
        private void PreencherTiposAlarme(TipoAlarme[] tiposAlarme)
        {
            chkPoc.Checked = false;
            chkUtilizacao.Checked = false;
            chkScore.Checked = false;

            if (tiposAlarme != null)
            {
                foreach (TipoAlarme tipoAlarmeSelecionado in tiposAlarme)
                {
                    switch (tipoAlarmeSelecionado)
                    {
                        case TipoAlarme.POC:
                            {
                                chkPoc.Checked = true;
                                break;
                            }
                        case TipoAlarme.UTL:
                            {
                                chkUtilizacao.Checked = true;
                                break;
                            }
                        case TipoAlarme.Score:
                            {
                                chkScore.Checked = true;
                                break;
                            }
                    }
                }
            }
        }
        /// <summary>
        /// Preenche os campos da tela de tipo de transação.
        /// </summary>
        /// <param name="tiposTransacao"></param>
        private void PreencherTiposTransacao(TipoTransacao[] tiposTransacao)
        {
            chkDebito.Checked = false;
            chkCredito.Checked = false;

            if (tiposTransacao != null)
            {
                foreach (TipoTransacao tipoCartaoSelecionado in tiposTransacao)
                {
                    switch (tipoCartaoSelecionado)
                    {
                        case TipoTransacao.Debito:
                            {
                                chkDebito.Checked = true;
                                break;
                            }
                        case TipoTransacao.Credito:
                            {
                                chkCredito.Checked = true;
                                break;
                            }
                    }
                }
            }
        }
        /// <summary>
        /// Preenche os campos da tela de resultado de autorização.
        /// </summary>
        /// <param name="resultadosAutorizacao"></param>
        private void PreencherResultadoAutorizacao(CriterioResultadoAutorizacao[] resultadosAutorizacao)
        {
            chkAprovada.Checked = false;
            chkNegada.Checked = false;

            if (resultadosAutorizacao != null)
            {
                foreach (CriterioResultadoAutorizacao resultadoAutorizacao in resultadosAutorizacao)
                {
                    switch (resultadoAutorizacao)
                    {
                        case CriterioResultadoAutorizacao.Aprovada:
                            {
                                chkAprovada.Checked = true;
                                break;
                            }
                        case CriterioResultadoAutorizacao.Negada:
                            {
                                chkNegada.Checked = true;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Preenche os campos da tela de critério de classificação.
        /// </summary>
        /// <param name="criterioSelecionado"></param>
        private void PreencherCriterioClassificacao(CriterioClassificacao criterioSelecionado)
        {
            switch (criterioSelecionado)
            {
                case CriterioClassificacao.DataTransacao:
                    {
                        rdDtaTransacao.Checked = true;
                        break;
                    }
                case CriterioClassificacao.Score:
                    {
                        rdScore.Checked = true;
                        break;
                    }
                case CriterioClassificacao.Valor:
                    {
                        rdValor.Checked = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Carrega na lista de EntryModes os disponíveis e selecionados
        /// </summary>
        /// <param name="itensDisponiveis">Lista com todos os EntryModes disponíveis (inclusive os selecionados)</param>
        /// <param name="selectionadosArray">Lista de EntryModes selecionados</param>
        private void PreencherListasEntryModes(EntryMode[] itensDisponiveis, EntryMode[] selecionadosArray)
        {

            IEnumerable<EntryMode> fromArray;

            HtmlSelect lstTo = listToEntryMode;
            HtmlSelect lstFrom = listFromEntryMode;

            ApagaItensSelect(lstTo);
            ApagaItensSelect(lstFrom);

            // Se já existe alguma busca
            if (itensDisponiveis != null)
            {
                // Exclui da lista da busca os códigos que já estão na lista de selecionados
                if (selecionadosArray != null)
                {
                    fromArray =
                       from res in itensDisponiveis
                       where !(from sel in selecionadosArray
                               select sel.Codigo).Contains(res.Codigo)
                       select res;
                }
                else
                {
                    fromArray = itensDisponiveis;
                }

                // Atribui os itens na lista
                lstFrom.Items.AddRange(
                   (from f in fromArray
                    select ObterListItemEntryMode(f)).ToArray()
                );
            }


            // Atribui a lista de selecionados
            if (selecionadosArray != null)
            {
                lstTo.Items.AddRange(
                       (from f in selecionadosArray
                        select ObterListItemEntryMode(f)).ToArray()
                );
            }
        }


        /// <summary>
        /// Preenche a lista de estabelecimentos selecionados
        /// </summary>
        /// <param name="listaEstabelecimentos">Lista de códigos de estabelecimentos selecionados</param>
        private void PreencherListaEstabelecimentosSelecionados(long[] listaEstabelecimentos)
        {
            ApagaItensSelect(lstEstabelecimento);

            lstEstabelecimento.Items.AddRange(
                (from l in listaEstabelecimentos
                 select new ListItem(l.ToString(), l.ToString())).ToArray()
            );
        }

        /// <summary>
        /// Preenche na tela as listas de UFs
        /// </summary>
        /// <param name="ufsDisponiveis">Lista completa</param>
        /// <param name="ufsSelecionadas">Itens selecionados</param>
        private void PreencherListasUF(string[] ufsDisponiveis, string[] ufsSelecionadas)
        {
            listFromUF.Items.Clear();
            listToUF.Items.Clear();

            if (ufsSelecionadas != null)
            {
                List<String> ufsFrom = ufsDisponiveis.Except(ufsSelecionadas).ToList<String>();
                listFromUF.DataSource = ufsFrom;
                listFromUF.DataBind();
                listToUF.DataSource = ufsSelecionadas;
                listToUF.DataBind();
            }
            else
            {
                listFromUF.DataSource = ufsDisponiveis;
                listFromUF.DataBind();
            }
        }

        /// <summary>
        /// Apaga todos os itens de um select.
        /// </summary>
        /// <param name="sel"></param>
        private void ApagaItensSelect(HtmlSelect sel)
        {
            sel.Items.Clear();
        }
        /// <summary>
        /// Preenche o list de MCCs na tela
        /// </summary>
        private void PreencherListasMCC(MCC[] resultadoBuscaArray, MCC[] selecionadosArray)
        {
            IEnumerable<MCC> fromArray;

            HtmlSelect lstTo = listToMCC;
            HtmlSelect lstFrom = listFromMCC;

            ApagaItensSelect(lstTo);
            ApagaItensSelect(lstFrom);


            // Se já existe alguma busca
            if (resultadoBuscaArray != null)
            {
                // Exclui da lista da busca os códigos que já estão na lista de selecionados
                if (selecionadosArray != null)
                {
                    fromArray =
                       from res in resultadoBuscaArray
                       where !(from sel in selecionadosArray
                               select sel.CodigoMCC).Contains(res.CodigoMCC)
                       select res;
                }
                else
                {
                    fromArray = resultadoBuscaArray;
                }
                // Atribui os itens na lista
                lstFrom.Items.AddRange(
                   (from f in fromArray
                    select ObterListItemMcc(f)).ToArray()
                );
            }


            // Atribui a lista de selecionados
            if (selecionadosArray != null)
            {
                lstTo.Items.AddRange(
                   (from f in selecionadosArray
                    select ObterListItemMcc(f)).ToArray()
                );
            }
        }

        /// <summary>
        /// Preenche a lista de Bins na tela
        /// </summary>
        private void PreencherListasRangesBin(FaixaBin[] resultadoBuscaArray, FaixaBin[] selecionadosArray)
        {
            IEnumerable<FaixaBin> fromArray;

            HtmlSelect lstTo = listToRange;
            HtmlSelect lstFrom = listFromRange;

            ApagaItensSelect(lstTo);
            ApagaItensSelect(lstFrom);

            // Se já existe alguma busca
            if (resultadoBuscaArray != null)
            {
                // Exclui da lista da busca os códigos que já estão na lista de selecionados
                if (selecionadosArray != null)
                {
                    fromArray =
                       from res in resultadoBuscaArray
                       where !(from sel in selecionadosArray
                               select sel.ValorInicial + "|" + sel.ValorFinal).Contains(res.ValorInicial + "|" + res.ValorFinal)
                       select res;
                }
                else
                {
                    fromArray = resultadoBuscaArray;
                }

                // Atribui os itens na lista
                lstFrom.Items.AddRange(
                   (from f in fromArray
                    select ObterListItemFaixaBin(f)).ToArray()
                );
            }


            // Atribui a lista de selecionados
            if (selecionadosArray != null)
            {
                lstTo.Items.AddRange(
                       (from f in selecionadosArray
                        select ObterListItemFaixaBin(f)).ToArray()
                );
            }
        }
        #endregion

        #region Obtem informacoes digitadas na tela
        /// <summary>
        ///  Atualiza os critérios com informacoes digitadas.
        /// </summary>
        private void AtualizarCriterioComInformacoesDigitadas()
        {
            PesquisarCriteriosSelecaoPorUsuarioLoginRetorno criterio = Criterio;

            criterio.Usuario = cboAnalistas.SelectedValue;

            Regex reNonDigit = new Regex("[^0-9]");

            // Atualiza valor transação
            criterio.ValorTransacaoInicial = decimal.Parse(txtValorInicialTransacao.Text);
            criterio.ValorTransacaoFinal = decimal.Parse(txtValorFinalTransacao.Text);


            // Atualiza resultado autorizacao
            List<CriterioResultadoAutorizacao> criterios = new List<CriterioResultadoAutorizacao>();
            if (chkAprovada.Checked)
                criterios.Add(CriterioResultadoAutorizacao.Aprovada);
            if (chkNegada.Checked)
                criterios.Add(CriterioResultadoAutorizacao.Negada);
            criterio.ResultadoAutorizacao = criterios.ToArray();

            // Atualiza tipo transacoes
            List<TipoTransacao> tipoTransacoes = new List<TipoTransacao>();
            if (chkCredito.Checked)
            {
                tipoTransacoes.Add(TipoTransacao.Credito);
            }
            if (chkDebito.Checked)
            {
                tipoTransacoes.Add(TipoTransacao.Debito);
            }
            criterio.TipoTransacaoSelecionadas = tipoTransacoes.ToArray();


            // Atualiza tipo alamre
            List<TipoAlarme> resultadosSelecionados = new List<TipoAlarme>();
            if (chkPoc.Checked)
                resultadosSelecionados.Add(TipoAlarme.POC);
            if (chkScore.Checked)
                resultadosSelecionados.Add(TipoAlarme.Score);
            if (chkUtilizacao.Checked)
                resultadosSelecionados.Add(TipoAlarme.UTL);
            criterio.TipoAlarme = resultadosSelecionados.ToArray();


            // Atualiza faixa score
            criterio.InicioFaixaScore = long.Parse(reNonDigit.Replace(txtScoreInicial.Text, ""));
            criterio.FimFaixaScore = long.Parse(reNonDigit.Replace(txtScoreFinal.Text, ""));


            // Atualiza criterio classificacao
            if (rdDtaTransacao.Checked)
                criterio.CriterioClassificacao = CriterioClassificacao.DataTransacao;
            if (rdScore.Checked)
                criterio.CriterioClassificacao = CriterioClassificacao.Score;
            if (rdValor.Checked)
                criterio.CriterioClassificacao = CriterioClassificacao.Valor;

            // Atualiza UFs
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Carrega UFs value: [" + hdnUFSelecionados.Value + "]");

                String[] ufs = new String[0] { };

                if (!String.IsNullOrEmpty(hdnUFSelecionados.Value))
                {
                    ufs = (hdnUFSelecionados.Value).Split('|');
                }

                // Atualiza as opções atuais ainda não gravadas no serviço
                criterio.UFsSelecionadas = ufs;
            }


            // Atualiza faixa BINs
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Carrega BINs value: [" + hdnRangeBinSelecionados.Value + "]");

                FaixaBin[] ranges = new FaixaBin[0] { };

                if (!String.IsNullOrEmpty(hdnRangeBinSelecionados.Value))
                {
                    // Obtem a lista gerada no hidden field
                    String[] itensSel = (hdnRangeBinSelecionados.Value).Split('|');

                    // Remonta as faixas de range correspondentes ao que o usuário selecionou
                    ranges =
                        (
                            from r in itensSel
                            select new FaixaBin()
                            {
                                ValorInicial = r.Split(':')[0],
                                ValorFinal = r.Split(':')[1]
                            }
                        ).ToArray();
                }

                // Atualiza as opções atuais ainda não gravadas no serviço
                criterio.RangeBinsSelecionados = ranges;
            }


            // Atualiza MCCs
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Carrega MCCs value: [" + hdnMCCSelecionados.Value + "]");

                MCC[] mcc = new MCC[0] { };
                if (!String.IsNullOrEmpty(hdnMCCSelecionados.Value))
                {

                    // Obtem a lista gerada no hidden field
                    String[] itensSel = (hdnMCCSelecionados.Value).Split('|');

                    // No conjunto universo, tem que ter tanto os que estavam selecionados como os do resultado da
                    // ultima busca. Potencialmente todos eles podem ter sido selecionados
                    List<MCC> universo = new List<MCC>();
                    if (this.ListaMCCResultadoBusca != null)
                    {
                        universo.AddRange(this.ListaMCCResultadoBusca);
                    }
                    if (criterio.MCCsSelecionados != null)
                    {
                        universo.AddRange(criterio.MCCsSelecionados);
                    }
                    var dic = universo.GroupBy(p => p.CodigoMCC).Select(g => g.First()).ToDictionary(p => p.CodigoMCC);

                    // Obtem os objetos MCC correspondentes ao que o usuário selecionou
                    mcc =
                        (
                            from s in itensSel
                            select dic[s]
                        ).ToArray();
                }
                // Atualiza as opções atuais ainda não gravadas no serviço
                criterio.MCCsSelecionados = mcc;
            }

            // Atualiza estabelecimentos
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Carrega estabelecimentos value: [" + hdnEstabelecimentosSelecionados.Value + "]");

                long[] estabelecimentos = new long[0] { };
                if (!String.IsNullOrEmpty(hdnEstabelecimentosSelecionados.Value))
                {
                    estabelecimentos =
                        (
                            from e in hdnEstabelecimentosSelecionados.Value.Split('|')
                            select long.Parse(e)
                        ).ToArray();
                }
                // Atualiza as opções atuais ainda não gravadas no serviço
                criterio.EstabelecimentosSelecionados = estabelecimentos;
            }


            // Atualiza EntryModes
            {
                Redecard.PN.Comum.SharePointUlsLog.LogMensagem("Carrega EntryModes value: [" + hdnEntryModeSelecionado.Value + "]");

                EntryMode[] em = new EntryMode[0] { };
                if (!String.IsNullOrEmpty(hdnEntryModeSelecionado.Value))
                {

                    // Obtem a lista gerada no hidden field
                    String[] itensSel = (hdnEntryModeSelecionado.Value).Split('|');

                    // No conjunto universo, tem que ter tanto os que estavam selecionados como os do resultado da
                    // ultima busca. Potencialmente todos eles podem ter sido selecionados
                    List<EntryMode> universo = new List<EntryMode>();
                    if (criterio.EntryModes != null)
                    {
                        universo.AddRange(criterio.EntryModes);
                    }
                    if (criterio.EntryModesSelecionados != null)
                    {
                        universo.AddRange(criterio.EntryModesSelecionados);
                    }

                    var dic = universo.GroupBy(p => p.Codigo).Select(g => g.First()).ToDictionary(p => p.Codigo);

                    // Obtem os objetos MCC correspondentes ao que o usuário selecionou
                    em =
                        (
                            from s in itensSel
                            select dic[s]
                        ).ToArray();

                }

                // Atualiza as opções atuais ainda não gravadas no serviço
                criterio.EntryModesSelecionados = em;
            }

            // atualiza a lista de status do cartão.
            List<SituacaoCartao> listaStatusCartao = new List<SituacaoCartao>();
            if (chkAnalisado.Checked)
                listaStatusCartao.Add(SituacaoCartao.Analisado);
            if (chkNaoAnalisado.Checked)
                listaStatusCartao.Add(SituacaoCartao.NaoAnalisado);
            criterio.SituacoesCartaoSelecionados = listaStatusCartao.ToArray();

            // Reatribui valor na propriedade (para ViewState é inócuo, mas se mudar um dia já tem suporte)
            this.Criterio = criterio;
        }
        #endregion

        #region Helpers
        private static ListItem ObterListItemMcc(MCC mcc)
        {
            return new ListItem()
            {
                Value = mcc.CodigoMCC,
                Text = String.Format("{0} - {1} ", mcc.CodigoMCC, mcc.DescricaoMCC)
            };
        }

        private static ListItem ObterListItemFaixaBin(FaixaBin faixa)
        {
            return new ListItem()
            {
                Text = string.Format("{0} - {1} ", faixa.ValorFinal, faixa.ValorInicial),
                Value = string.Format("{0}:{1}", faixa.ValorFinal, faixa.ValorInicial)
            };
        }


        private static ListItem ObterListItemEntryMode(EntryMode itemDisponivel)
        {
            return new ListItem()
            {
                Text = string.Format("{0} - {1} ", itemDisponivel.Codigo, itemDisponivel.Descricao),
                Value = string.Format("{0}", itemDisponivel.Codigo)
            };
        }
        #endregion

        #region Eventos de click em tela
        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão MCC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMCC_Click(object sender, EventArgs e)
        {
            try
            {
                long? cdMCC = null;
                if (string.IsNullOrEmpty(txtNumero.Text))
                {
                    cdMCC = null;
                }
                else
                {
                    long l = 0;
                    if (!long.TryParse(txtNumero.Text, out l))
                    {
                        // Colocar no padrão esta mensagem de erro
                        throw new MensagemValidacaoException("Valor inválido para o MCC");
                    }
                    cdMCC = l;
                }

                PesquisarMCCenvio envio = new PesquisarMCCenvio();
                envio.CodigoMCC = cdMCC;
                envio.DescricaoMCC = txtNomeMCC.Text;
                envio.GrupoEntidade = GetSessaoAtual.GrupoEntidade;
                envio.NumeroEmissor = GetSessaoAtual.CodigoEntidade.ToString();
                envio.PosicaoPrimeiroRegistro = 0;
                envio.QuantidadeMaximaRegistros = 50; // Limitar em 50 para não estourar memória da tela
                envio.RenovarContador = false;
                envio.UsuarioLogin = GetSessaoAtual.LoginUsuario;

                // Executa Serviço de busca
                PesquisarMCCRetorno retorno;
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    retorno = objClient.PesquisarMerchantPorCodigoCategoria(envio);
                }


                // Armazena a lista no ViewState
                this.ListaMCCResultadoBusca = retorno.MCCs;

                // Atualiza tela
                PreencherListasMCC(this.ListaMCCResultadoBusca, this.Criterio.MCCsSelecionados);

            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }



        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão ICA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnIca_Click(object sender, EventArgs e)
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    PesquisarRangeBinPorEmissorEnvio envio = new PesquisarRangeBinPorEmissorEnvio();
                    envio.grupoEntidade = GetSessaoAtual.GrupoEntidade;

                    long ica = 0;

                    if (!long.TryParse(txtIca.Text, out ica))
                    {
                        throw new MensagemValidacaoException("Código da mensagem deve ser preenchido.");
                    }
                    envio.ica = ica;
                    envio.numeroEmissor = GetSessaoAtual.CodigoEntidade;
                    envio.posicaoPrimeiroRegistro = 0;
                    envio.quantidadeMaximaRegistro = 50; // Limitar em 50 para não estourar memória da tela
                    envio.renovarContador = false;
                    envio.usuarioLogin = GetSessaoAtual.LoginUsuario;

                    PesquisarRangeBinPorEmissorRetorno retorno = objClient.PesquisarRangeBinPorEmissor(envio);

                    this.ListaFaixaBinResultadoBusca = retorno.ListaFaixaBin;


                    // Atualiza tela
                    PreencherListasRangesBin(this.ListaFaixaBinResultadoBusca, this.Criterio.RangeBinsSelecionados);
                }

            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }

        }


        /// <summary>
        /// Evento que irá ocorrer ao clicar no botão atualizar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualiza_Click(object sender, EventArgs e)
        {
            try
            {
                // A propriedade Criterio já vai estar atualizada pelo Page_Load com os dados digitados na tela.
                // É só armazenar o que estiver nela.
                GravarCriterioDeSelecaoNoSistema(this.Criterio);

                base.ExibirMensagem("Operação realizada com sucesso.");
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }


        /// <summary>
        /// Evento que irá ocorrer ao clicar no combo de analistas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboAnalistas_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtem do servico o criterio para este analista
            this.Criterio = CarregarDadosDoAnalista(cboAnalistas.SelectedValue);
            // Preenche novamente a tela
            PreencherCamposDaTelaComCriterio(Criterio);
        }

        #endregion
        /// <summary>
        /// Este método é utilizado para saber se os parãmetros sistema estão carregados.
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return true;
        }
    }
}
