using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Comparadores;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper.Internacionalizacao;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;

namespace Redecard.Portal.Aberto.WebParts.ParaSeuComputador
{
    /// <summary>
    /// Autor: Cristiano Dias
    /// Data da criação: 22/10/2010
    /// Descrição: Composição do WebPart de Listagem de Telas de Descanso e Papéis de Parede
    /// </summary>
    public partial class ParaSeuComputadorUserControl : UserControlBase
    {
        #region Variáveis
        /// <summary>
        /// Objeto conversor de SPFile para ItemBiblioteca
        /// </summary>
        private ITraducao<IEnumerable<SPFile>, IEnumerable<ItemBiblioteca>> traducaoSPFileItemBiblioteca = new TradutorDeSPFileParaItemBiblioteca();

        /// <summary>
        /// Quando encontrados dados da biblioteca em requisição, armazena o nome para posterior configuração do título da Web Part
        /// </summary>
        private string nomeBiblioteca = string.Empty;

        /// <summary>
        /// Mantém a quantidade de itens renderizados na tela para posterior lógica de mostragem/ocultação do botão Ver Mais
        /// </summary>
        private int totalItensRenderizados;

        /// <summary>
        /// Mantém a quantidade de itens retornados pela listagem de uma determinada biblioteca para posterior lógica de mostragem/ocultação do botão Ver Mais
        /// </summary>
        private int totalItensArmazenados;

        /// <summary>
        /// Armazena itens de biblioteca com mesmo titulo para func de agrupamento no repeater de downloads
        /// </summary>
        private IEnumerable<ItemBiblioteca> itensBiblioteca = null;
        #endregion

        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.CarregarBiblioteca();

            base.OnLoad(e);
        }

        /// <summary>
        /// Contabiliza o total de itens renderizados na tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptParaSeuComputador_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Contabiliza total de itens renderizados para posterior lógica de configuração do botão Ver Mais
                this.totalItensRenderizados += 1;

                ItemBiblioteca itemBiblioteca = e.Item.DataItem as ItemBiblioteca;
                Repeater rptLinksDownload = e.Item.FindControl("rptLinksDownload") as Repeater;

                if (itemBiblioteca != null && rptLinksDownload != null)
                {
                    //Popula a lista de links de download para o item atual
                    //Filtra por itens cujo título é igual ao título do item atual da renderização
                    this.PopularItensLinkDownload(rptLinksDownload, this.ObterItensFiltradosPor(i => i.Titulo.ToUpper().Equals(itemBiblioteca.Titulo.ToUpper())));
                }
            }
        }
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém a flag indicando se haverá paginação no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private bool PaginacaoHabilitada
        {
            get
            {
                return this.WebPart.PaginacaoHabilitada;
            }
        }

        /// <summary>
        /// Obtém a url da página de listagem de itens de uma determinada biblioteca
        /// </summary>
        private string URLPaginaListagemBiblioteca
        {
            get
            {
                return this.WebPart.URLPaginaVisualizacao;
            }
        }

        /// <summary>
        /// Obtém os itens no página estipulados no controle WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private int ItensPorPagina
        {
            get
            {
                return this.WebPart.QuantidadeItensPorPagina;
            }
        }

        /// <summary>
        /// Obtém o nome da Biblioteca configurada nas propriedades da WebPart que instancia este UserControl
        /// Se algo der errado ao obter este valor da webpart, um valor padrão é retornado
        /// </summary>
        private string BibliotecaPadrao
        {
            get
            {
                return this.WebPart.NomeBibliotecaExibicao;
            }
        }

        /// <summary>
        /// Acesso à Biblioteca selecionada (leia comentários dentro do código)
        /// </summary>
        private string Biblioteca
        {
            get
            {
                //A WebPart, a princípio, é consumida por 2 páginas. Uma utiliza a propriedade da Web Part que informa
                //a biblioteca que será utilizada para listar os itens, e a outra se baseia em query string para o mesmo.
                //A lógica abaixo leva em consideração primeiro a querystring. Se não estiver informada, usa a propriedade da web part.
                if (Request.Params[ChavesQueryString.Biblioteca] == null)
                    return this.BibliotecaPadrao;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Biblioteca].ToString());
            }
        }

        /// <summary>
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ParaSeuComputador WebPart
        {
            get
            {
                return this.Parent as ParaSeuComputador;
            }
        }

        /// <summary>
        /// Referência tipada ao user control de paginação
        /// </summary>
        private UserControlPaginador Paginador
        {
            get
            {
                return this.ucPaginadorHibrido as UserControlPaginador;
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Carrega a lista de itens com base na biblioteca informada
        /// </summary>
        private void CarregarBiblioteca()
        {
            ListaPaginada<ItemBiblioteca> arquivos = null;

            try
            {
                bool pagHabilitada = this.PaginacaoHabilitada;

                //Solicita os arquivos do repositório e mantém os itens da biblioteca para posterior agrupamento de links de download na renderização dos itens na instância da página
                this.itensBiblioteca = this.ObterItens(this.Biblioteca);

                //Solicita os arquivos - filtra por itens únicos
                arquivos = new ListaPaginada<ItemBiblioteca>(this.itensBiblioteca.Distinct(new ItemBibliotecaTituloComparer()), //Dados
                                                             pagHabilitada ? this.Paginador.Pagina : null, //Índice da página
                                                             this.ItensPorPagina); //Qtd de itens por página

                //Mantém o total de itens armazenados para lógica de configuração do botão Ver Mais
                this.totalItensArmazenados = arquivos.TotalItens;

                //Configura o título da biblioteca
                this.ConfigurarTitulo();

                if (this.totalItensArmazenados > 0)
                {
                    //Popula os arquivos pro usuário
                    this.PopularItens(arquivos);

                    //Configura o paginador de acordo
                    if (pagHabilitada)
                        this.Paginador.MontarPaginador(arquivos.TotalItens, this.ItensPorPagina, null);

                    //Configura o botão Ver Mais
                    this.ConfigurarBotaoVerMais();
                }
                else
                    this.ltlMensagem.Text = RedecardHelper.ObterResource("paraSeuComputador_PesquisaSemResultado");
            }
            catch (BibliotecaInexistenteException)
            {
                //Notifica o usuário caso a biblioteca solicitada não exista
                this.ltlMensagem.Text = RedecardHelper.ObterResource("paraseucomputador_bibliotecainexistente");
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Solicita os itens da biblioteca informada
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ItemBiblioteca> ObterItens(string biblioteca)
        {
            IEnumerable<SPFile> arquivosBiblioteca = null;

            //Obtém referência ao site raiz (sem variation) para obtenção das bibliotecas e seus itens
            using (SPWeb oWeb = SPContext.Current.Site.OpenWeb().Site.RootWeb)
            {
                SPList spList = null;

                try
                {
                    //Obtém referência à biblioteca
                    spList = oWeb.Lists[biblioteca];

                    //Assinala o nome armazenado no servidor da biblioteca
                    this.nomeBiblioteca = spList.Title;
                }
                catch (Exception)
                {
                    throw new BibliotecaInexistenteException();
                }

                //Retorna se não houver itens
                if (spList.ItemCount.Equals(0))
                    return new List<ItemBiblioteca>(0);

                //Na biblioteca, as subpastas têm nomes de rótulos de variação (Variations) do site (pt-BR, en-US...).
                //Com base nisso, busca os itens na pasta correta, e a propriedade Language pode nos ajudar nesta listagem
                Idioma oIdioma = InternacionalUtils.ObterDadosIdioma((Idiomas)oWeb.Language);
                
                //Obtém a subpasta com os arquivos cujo nome é o nome do rótulo de variação do site
                SPFolder lFolder = spList.RootFolder.SubFolders[oIdioma.Rotulo];

                //Recupera os arquivos os arquivos
                arquivosBiblioteca = lFolder.Files.OfType<SPFile>();
            }

            return traducaoSPFileItemBiblioteca.Traduzir(arquivosBiblioteca);
        }

        /// <summary>
        /// Filtra os itens recuperados do repositório com base na função de condição(opcional, o que fará com que retorna tudo)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ItemBiblioteca> ObterItensFiltradosPor(Func<ItemBiblioteca, bool> funcaoCondicao)
        {
            if (funcaoCondicao == null)
                return this.itensBiblioteca;

            return this.itensBiblioteca.Where(funcaoCondicao);
        }

        /// <summary>
        /// Popula os itens retornados para o usuário
        /// </summary>
        /// <param name="arquivos"></param>
        private void PopularItens(IEnumerable<ItemBiblioteca> arquivos)
        {
            this.rptParaSeuComputador.DataSource = arquivos;
            this.rptParaSeuComputador.DataBind();
        }

        /// <summary>
        /// Popula os itens de link de download de um determinado item de biblioteca
        /// </summary>
        /// <param name="rptLinksDownload"></param>
        /// <param name="itens"></param>
        private void PopularItensLinkDownload(Repeater rptLinksDownload, IEnumerable<ItemBiblioteca> itens)
        {
            rptLinksDownload.DataSource = itens;
            rptLinksDownload.DataBind();
        }

        /// <summary>
        /// Atribui ao topo da galeria o nome da biblioteca em visualização
        /// </summary>
        /// <param name="biblioteca"></param>
        private void ConfigurarTitulo()
        {
            this.ltlNomeBiblioteca.Text = this.nomeBiblioteca;
        }

        /// <summary>
        /// Configura a visibilidade e a URL do link Ver Mais.
        /// Quanto a visibilidade: é visível quando a paginação não está habilitada e vice-versa e quando o número de itens mostrados é menor que o total existente no repositório.
        /// Quanto a URL: página configurada como propriedade da WebPart seguida do parâmetro e valor ?Biblioteca=XPTO
        /// </summary>
        private void ConfigurarBotaoVerMais()
        {
            //Para disponibilização do Link Ver Mais, a WebPart deve ter a URL da página configurada
            if (string.IsNullOrEmpty(this.URLPaginaListagemBiblioteca))
            {
                this.phlVerMais.Visible = false;
                this.lnkVerMais.NavigateUrl = string.Empty;
            }
            else
            {
                if (!this.PaginacaoHabilitada && this.totalItensRenderizados < this.totalItensArmazenados)
                {
                    this.phlVerMais.Visible = true;
                    this.lnkVerMais.NavigateUrl = string.Format("{0}/{1}?{2}={3}", SPContext.Current.Web.Url, this.URLPaginaListagemBiblioteca, ChavesQueryString.Biblioteca, URLUtils.URLEncode(this.Biblioteca));
                }
            }
        }
        #endregion
    }
}