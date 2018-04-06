/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Redecard.PN.FMS.Sharepoint.ControlTemplates;
using Redecard.PN.FMS.Sharepoint.Exceptions;
using Redecard.PN.FMS.Sharepoint.Interfaces;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.FMS.Sharepoint
{

    /// <summary>
    /// Classe abstrata responsável pela inversão de controle das telas diretamente resposáveis pelas consultas em gridView.
    /// 
    /// RelatorioGridBaseUserControl é responsável pelos listeners de ordenação da coluna e de paginação, além de fornecer um acesso 
    /// aos metodos de consulta (MontaGrid e MontaGridInicial) e fornecendo os atributos que serão utilizados na chamada do serviço. 
    /// 
    /// Alem disso, utiliza de composicao de controles e marcacoes de interface para orquestrar a exportacao dos relatorios.
    /// 
    /// </summary>
    /// <typeparam name="TEnumCriteirio">O tipo genérico "TEnumCriteirio" é relativo a um enum com o mapeamento das colunas 
    /// do GridView (Enum que é retornado do serviço da CPQD, que, por sua vez, é retransmitido para a 
    /// reordenação).</typeparam>
    public abstract class RelatorioGridBaseUserControl<TEnumCriteirio> : BaseUserControl
    {
        #region Abstract Methods
        /// <summary>
        /// Enum estático contendo o membro do enum/Coluna que será ordenado inicialmente.
        /// </summary>
        /// <returns></returns>
        protected abstract TEnumCriteirio ObterCriterioOrdemInicial();

        /// <summary>
        /// Método que deve conter toda a lógica de manipulação e montagem das grids
        /// Este método será chamado em cada evento do componente de paginação
        /// </summary>
        protected abstract long MontaGrid(FMSClient objClient, TEnumCriteirio criterioOrdem, OrdemClassificacao ordemClassificacao, int primeiroRegistro, int quantidadeRegistroPagina);

        /// <summary>
        /// Um controle HTML runat="server" no qual os controles e elementos DOM da paginação serão incrementados.
        /// </summary>
        /// <returns></returns>
        protected abstract Control GetControleOndeColocarPaginacao();
        #endregion

        #region Atributos

        private const int QUANTIDADE_REGISTROS_POR_PAGINA = 10;

        /// <summary>
        /// Propriedade de ordem de critério
        /// </summary>
        protected TEnumCriteirio CriterioOrdem
        {
            get
            {
                return ViewState["CriterioOrdem"] == null ? ObterCriterioOrdemInicial() : (TEnumCriteirio)ViewState["CriterioOrdem"];
            }
            set
            {
                ViewState["CriterioOrdem"] = value;
            }
        }

        /// <summary>
        /// Propriedade de ordem de classificação
        /// </summary>
        protected OrdemClassificacao OrdemClassificacao
        {
            get
            {
                return ViewState["OrdemClassificacao"] == null ? OrdemClassificacao.Ascendente : (OrdemClassificacao)ViewState["OrdemClassificacao"];
            }
            set
            {
                ViewState["OrdemClassificacao"] = value;
            }
        }
        /// <summary>
        /// Propriedade de controle de paginação
        /// </summary>
        protected Paginacao PaginacaoControl { get; set; }
        /// <summary>
        /// Propriedade de controle da tabela de ações.
        /// </summary>
        private TabelaAcoes TabelaAcoesControl { get; set; }
        #endregion

        /// <summary>
        /// 
        /// Controles iniciais
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MontaPaginacao();
        }
        /// <summary>
        /// Ao carregar a página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);//Sempre chame o Base...

            VerificaEDefineMecanismoExportacao();
        }
        /// <summary>
        /// Verifica se a classe concreta implementa a interface IPossuiRelatorio, e 
        /// caso possua, atribui o listener ao click do objeto de exportacao.
        /// 
        /// Quando clicado no link, sera chamado o metodo GeraRetornoExportacao()
        /// </summary>
        private void VerificaEDefineMecanismoExportacao()
        {
            //Tipo dinamico do user control nao reconhece a interface utilizando IsAssignableFrom
            if (this.GetType().GetInterfaces().ToList().Find(i => i == typeof(IPossuiExportacao)) != null)
            {
                TabelaAcoes tabAcoes = (TabelaAcoes)this.FindControl("tabAcoes");

                if (tabAcoes != null)
                {
                    tabAcoes.onExportarClick += this.onExportarClick;
                }
            }
        }

        /// <summary>
        /// Evento que inicia a exportação
        /// </summary>
        /// <param name="e"></param>
        public void onExportarClick(EventArgs e)
        {
            IPossuiExportacao iPossuiExportacao = this as IPossuiExportacao;

            iPossuiExportacao.Exportar();
        }

        /// <summary>
        /// Carrgea o Control template necessário para a paginação,
        /// Para isso ele pega o Elemento atribuido em GetControleOndeColocarPaginacao 
        /// e adiciona os contorles da paginação.
        /// </summary>
        private void MontaPaginacao()
        {
            Control con = this.GetControleOndeColocarPaginacao();
            if (con != null)
            {
                PaginacaoControl = LoadControl("~/_CONTROLTEMPLATES/Redecard.PN.FMS.Sharepoint/Paginacao.ascx") as Paginacao;
                PaginacaoControl.ID = "paginacao_control";
                PaginacaoControl.RegistrosPorPagina = QUANTIDADE_REGISTROS_POR_PAGINA;
                PaginacaoControl.onPaginacaoChanged += onPaginacaoChanged;
                con.Controls.Add(PaginacaoControl);
            }
        }
        /// <summary>
        /// Listener que chama a remontagem do grid para cada evento (reordenação, moção de página, etc) 
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="e"></param>
        private void onPaginacaoChanged(int pagina, EventArgs e)
        {
            MontaGrid(pagina);
        }


        /// <summary>
        /// Chama o metodo MontaGrid passando os valores padrão para uma consulta inicial, esse metodo deve 
        /// ser chamado logo na chamada do link
        /// </summary>
        protected void MontaGridInicial()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    if (PaginacaoControl == null)
                    {
                        MontaGrid(objClient, CriterioOrdem, this.OrdemClassificacao,
                                                                         Constantes.PosicaoInicialPrimeiroRegistro,
                                                                         Constantes.PosicaoInicialPrimeiroRegistro);
                    }
                    else
                    {
                        PaginacaoControl.PaginaAtual = 1;
                        PaginacaoControl.QuantidadeTotalRegistros = MontaGrid(objClient, CriterioOrdem,
                                                                             this.OrdemClassificacao,
                                                                             Constantes.PosicaoInicialPrimeiroRegistro,
                                                                             PaginacaoControl.RegistrosPorPagina);
                    }
                }
            }
            catch (MensagemValidacaoException ex)
            {
                base.OnError(ex);
            }
            catch (Exception ex)
            {
                //Tratamento de execao da montagem dos Grids sera tratado no OnError
                this.OnError(ex);
            }
        }


        /// <summary>
        /// Chama os metodos de montagem do Grid com os parâmetros Criterio de ordem, 
        /// ordem de classificação registro inicial, página inicial e registro por página, alem, claro, de 
        /// tratar as excecoes lancadas e injetar as dependecias do servico que sera chamado.
        /// </summary>
        /// <param name="pagina"></param>
        private void MontaGrid(int pagina)
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    if (PaginacaoControl == null)
                    {
                        MontaGrid(
                                 objClient,
                                 this.CriterioOrdem,
                                 this.OrdemClassificacao,
                                 Constantes.PosicaoInicialPrimeiroRegistro,
                                 Constantes.PosicaoInicialPrimeiroRegistro);
                    }
                    else
                    {
                        PaginacaoControl.PaginaAtual = pagina;
                        PaginacaoControl.QuantidadeTotalRegistros = MontaGrid(
                                                                             objClient,
                                                                             this.CriterioOrdem,
                                                                             this.OrdemClassificacao,
                                                                             (PaginacaoControl.RegistroInicialPaginaAtual-1),
                                                                             PaginacaoControl.RegistrosPorPagina);
                    }
                }
            }
            catch (MensagemValidacaoException ex)
            {
                base.OnError(ex);
            }
            catch (Exception ex)
            {
                //Tratamento de execao da montagem dos Grids sera tratado no OnError
                this.OnError(ex);
            }
        }
        /// <summary>
        /// Metodo listener que deve ser chamado no ascx ao reordenar uma gridView
        /// 
        /// Para adicionar o atributo em uma gridView, vá ao seu ascx e na gridView adicione os atributos >
        /// 
        /// "OnSorting="GridView_Sorting" AllowSorting="True"
        /// 
        /// Ex:      <asp:GridView ID="grvDados" runat="server"
        ///        AutoGenerateColumns="False" ShowFooter="true" 
        ///        CssClass="gridDados tblRelatorioValores" enableviewstate="false" >
        ///    <b>    onrowdatabound="grvDados_RowDataBound" OnSorting="GridView_Sorting" </b>
        ///    <b>    AllowSorting="True"> </b>
        /// 
        ///  Cada atributo do header do grid View, se possuir ordenação orientada pelo serviço, deve ser colocado o atributo
        ///   SortExpression="{CAMPO_ENUM}"
        ///   
        /// onde {CAMPO_ENUM} = Enum referente a tabela (O mesmo do tipo generico da classe concreta que implementa essa Abstract.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            TEnumCriteirio colunaAtualGrid = (TEnumCriteirio)Enum.Parse(typeof(TEnumCriteirio), e.SortExpression);

            TEnumCriteirio colunaViewState = this.CriterioOrdem;

            if (Convert.ToInt32(colunaViewState) == Convert.ToInt32(colunaAtualGrid))
            {
                if (this.OrdemClassificacao == Servico.FMS.OrdemClassificacao.Ascendente)
                {
                    this.OrdemClassificacao = Servico.FMS.OrdemClassificacao.Descendente;
                }
                else
                {
                    this.OrdemClassificacao = Servico.FMS.OrdemClassificacao.Ascendente;
                }
            }
            else
            {
                this.CriterioOrdem = colunaAtualGrid;
                this.OrdemClassificacao = Servico.FMS.OrdemClassificacao.Ascendente;
            }

            if (PaginacaoControl == null)
            {
                MontaGrid(0);
            }
            else
            {
                MontaGrid(PaginacaoControl.PaginaAtual);
            }
        }

        protected string FormatarDataExibicao(DateTime data)
        {
            if (data == DateTime.MinValue)
                return "";
            else
                return data.ToString("dd/MM/yyyy HH:mm:ss");
        }

        #region Validações padrão
        /// <summary>
        /// Recebe os controles da pagina que possuem a data inicial e final, as valida e retorna um struct com as datas, ou 
        /// lanca uma excecao descrevendo o erro ocorrido. 
        /// </summary>
        /// <param name="textoDatainicial"></param>
        /// <param name="textoDataFinal"></param>
        /// <returns></returns>
        protected IntervaloData ValidarData(System.Web.UI.WebControls.TextBox textoDataInicial, System.Web.UI.WebControls.TextBox textoDataFinal)
        {
            return this.ValidarData(textoDataInicial.Text, textoDataFinal.Text);
        }

        /// <summary>
        /// Recebe os controles da pagina que possuem a data inicial e final, as valida e retorna um struct com as datas, ou 
        /// lanca uma excecao descrevendo o erro ocorrido. 
        /// </summary>
        /// <param name="textoDatainicial"></param>
        /// <param name="textoDataFinal"></param>
        /// <returns></returns>
        protected IntervaloData ValidarData(string textoDataInicial, string textoDataFinal)
        {
            try
            {
                IntervaloData retorno = new IntervaloData();

                if (string.IsNullOrEmpty(textoDataInicial))
                {
                    throw new MensagemValidacaoException(324, "Data inicial obrigatória.");
                }
                else
                {
                    retorno.dataInicial = DateTime.Parse(textoDataInicial);
                }

                if (string.IsNullOrEmpty(textoDataFinal))
                {
                    throw new MensagemValidacaoException(325, "Data final obrigatória.");
                }
                else
                {
                    retorno.dataFinal = DateTime.Parse(textoDataFinal);
                }

                if (retorno.dataFinal.CompareTo(DateTime.Now) > 0)
                {
                    throw new MensagemValidacaoException(326, "Data final não pode ser maior que data atual.");
                }

                if (retorno.dataInicial.CompareTo(retorno.dataFinal) > 0)
                {
                    throw new MensagemValidacaoException(327, "Data final não pode ser menor que data inicial.");
                }

                TimeSpan intervalo = retorno.dataFinal.Subtract(retorno.dataInicial);

                if (intervalo.Days > base.ParametrosSistema.QuantidadeMaximaIntervaloDiasPesquisas)
                {
                    throw new MensagemValidacaoException(string.Format("O intervalo máximo de dias para a consulta é de {0} dias.", base.ParametrosSistema.QuantidadeMaximaIntervaloDiasPesquisas.ToString()));
                }

                if (retorno.dataInicial.Day < (DateTime.Now.Day - base.ParametrosSistema.QuantidadeMaximaDiasRetroativosPesquisas))
                {
                    throw new MensagemValidacaoException(string.Format("O prazo máximo para pesquisa é de {0} dias.", base.ParametrosSistema.QuantidadeMaximaDiasRetroativosPesquisas.ToString()));
                }

                return retorno;
            }
            catch (MensagemValidacaoException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new MensagemValidacaoException("Período inválido.");
            }
        }

        protected struct IntervaloData
        {
            public DateTime dataInicial;
            public DateTime dataFinal;
        };
        /// <summary>
        /// Este método é utilizado para  validar o retorno da pesquisa por cartão.
        /// </summary>
        /// <param name="retorno"></param>
        protected void ValidarRetornoPesquisaPorCartao(PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno retorno)
        {
            if (retorno.TipoRespostaListaEmissor == TipoRespostaListaEmissor.Ok)
            {
                return;
            }

            switch (retorno.TipoRespostaListaEmissor)
            {
                case TipoRespostaListaEmissor.CartaoEmAnalisePorOutroUsuario:
                    {
                        base.ExibirPainelExcecao(FMS_FONTE, 203);
                        break;
                    }
                case TipoRespostaListaEmissor.NaoExistemTransacoes:
                case TipoRespostaListaEmissor.NaoExistemTransacoesAlarmadas:
                    {
                         base.ExibirPainelExcecao(FMS_FONTE, 304);
                        break;
                    }
                case TipoRespostaListaEmissor.CartaoJaAnalisado:
                    {
                        base.ExibirPainelExcecao(FMS_FONTE, 322);
                        break;
                    }
            }
        }
        #endregion
    }
}
