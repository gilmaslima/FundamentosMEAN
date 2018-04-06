using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using Redecard.Portal.Helper.Web;
using System.Text;

namespace Redecard.Portal.Aberto.WebParts.ListagemFotos
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data da criação: 03/09/2010
    /// Descrição: Composição do WebPart de Listagem de Fotos
    /// Modificado em: 09/11/2010
    /// Descrição: A galeria de fotos deverá abrir a primeira galeria de fotos listada o DropDownList.
    /// </summary>
    public partial class ListagemFotosUserControl : UserControlBase
    {
        #region Variáveis
        private static string textoPadraoSelecione = RedecardHelper.ObterResource("listagemfotos_selecioneopcao");
        private static string valorPadraoSelecione = string.Empty;

        private DTOFoto fotoTopo = null;
        private bool primeiraFotoRenderizada = true;
        #endregion

        #region Eventos
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.CarregarGaleriasDeFotos();

            this.CarregarFotos(this.Galeria);

            base.OnLoad(e);
        }

        /// <summary>
        /// Evento disparado para cada item de foto vinculada no controle de listagem
        /// Simplesmente armazena a referência a primeira foto renderizada. Mais detalhes na implementação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptListagemFotos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Considera apenas a primeira foto; despreza o resto.
                if (this.primeiraFotoRenderizada)
                {
                    //Armazena a referência a primeira foto renderizada para posterior registro de javascript para seleção de imagem na WebPart de exibição (vide método RegistrarScriptExibicaoImagem(DTOFoto foto))
                    this.fotoTopo = e.Item.DataItem as DTOFoto;

                    this.primeiraFotoRenderizada = false;
                }
            }
        }
        #endregion

        #region Propriedades

        public string MsgTitulo
        {
            get { return RedecardHelper.ObterResource("fotosSlideShow_Titulo"); }
        }

        public string MsgDescritivo
        {
            get { return RedecardHelper.ObterResource("fotosSlideShow_Descritivo"); }
        }

        /// <summary>
        /// Nome da âncora utilizada para posicionamento do usuário na página
        /// </summary>
        public string AncoraListagem
        {
            get
            {
                return "wptListagem";
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
        /// Obtém referência à web part que contém este UserControl
        /// </summary>
        private ListagemFotos WebPart
        {
            get
            {
                return this.Parent as ListagemFotos;
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

        /// <summary>
        /// Acesso à Galeria selecionada
        /// </summary>
        private string Galeria
        {
            get
            {
                if (Request.Params[ChavesQueryString.Galeria] == null)
                    return string.Empty;

                return URLUtils.URLDecode(Request.Params[ChavesQueryString.Galeria].ToString());
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Monta uma string JavaScript de redirecionamento com base nos parâmetros informados
        /// </summary>
        /// <param name="urlAtual"></param>
        /// <param name="nomeParametro">Nome do parâmetro que será utilizado para o valor do item selecionado no controle. Exemplo: PAGINA=1</param>
        /// <param name="NomeAncora"></param>
        /// <returns></returns>
        protected virtual string ObterInstrucaoRedirecionamentoJS(string urlAtual, string nomeParametro, string nomeAncora)
        {
            return string.Format("window.location.href='{0}{1}{2}=' + window.encodeURIComponent(this.options[this.selectedIndex].value) + '#" + nomeAncora + "';",
                                urlAtual,
                                urlAtual.IndexOf('?') == -1 ? "?" : string.Empty,
                                nomeParametro);
        }

        /// <summary>
        /// Carrega a DropDownList de Galerias de Fotos
        /// As galerias são obtidas a partir da listagem das fotos existentes no repositório, e as duplicatas são eliminadas.
        /// Por último a coleção é ordenada alfabeticamente
        /// </summary>
        private void CarregarGaleriasDeFotos()
        {
            //Obtém a URL pedindo para ignorar os parâmetros Galeria e Página
            string urlAtual = URLUtils.ObterURLAtual(gal => gal.Trim().ToUpper().Equals(ChavesQueryString.Galeria.ToUpper()) || gal.Trim().ToUpper().Equals(ChavesQueryString.Pagina.ToUpper()));

            //Esvazia o controle
            this.slcTypeCardBenefits.Items.Clear();

            //Adiciona o evento de auto-redirecionamento ao controle DropDown ao selecionar um novo item
            this.slcTypeCardBenefits.Attributes.Add("onchange", this.ObterInstrucaoRedirecionamentoJS(urlAtual, ChavesQueryString.Galeria, this.AncoraListagem));

            //Obtém as galerias para carregamento
            IList<string> galerias = this.ObterGalerias();

            //Carrega o controle com as galerias
            if (galerias != null)
                galerias.ToList().ForEach(g => this.slcTypeCardBenefits.Items.Add(new ListItem(g, g)));

            //Adiciona um primeiro item no controle
            this.slcTypeCardBenefits.Items.Insert(0, new ListItem(textoPadraoSelecione, valorPadraoSelecione));

            //Seleciona a galeria atual, se informada
            ListItem liGaleria = this.slcTypeCardBenefits.Items.FindByValue(this.Galeria);
            if (liGaleria != null)
                liGaleria.Selected = true;
            else
                this.slcTypeCardBenefits.SelectedValue = valorPadraoSelecione;
        }

        /// <summary>
        /// Solicita as galerias de imagens com o Repositório de Fotos
        /// </summary>
        /// <returns></returns>
        private IList<string> ObterGalerias()
        {
            IList<string> galerias = null;

            try
            {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOFoto, FotosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        galerias = (from gal in repository.GetAllItems()
                                    select gal.Galeria).OrderBy(g => g).Distinct().ToList();
                    });
                }
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }

            return galerias;
        }

        /// <summary>
        /// Analisa a galeria informada para listagem.
        /// Caso nenhuma galeria tenha sido selecionada/informada, seleciona no DropDown o primeiro item listado (que não é "Selecione uma opção") e retorna o nome desta galeria para carregamento
        /// Método adicionado em 09/11/2010
        /// </summary>
        /// <param name="galeria"></param>
        /// <returns></returns>
        private string VerificaEObtemGaleriaPadrao(string galeria)
        {
            //Caso nenhuma galeria tenha sido selecionada/informada,
            //seleciona no DropDown o primeiro item listado (que não é "Selecione uma opção") e retorna o nome desta galeria para carregamento
            if (galeria.Equals(valorPadraoSelecione))
            {
                //Se não houver fotos no repositório, a primeira linha logo abaixo dispara exceção IndexOutOfRangeException
                //Trata e simplesmente retorna valorPadrao (vazio) no catch
                try
                {
                    ListItem liPrimeiraGaleria = this.slcTypeCardBenefits.Items[1];

                    if (liPrimeiraGaleria != null)
                    {
                        this.slcTypeCardBenefits.ClearSelection();

                        //Seleciona a primeira galeria
                        liPrimeiraGaleria.Selected = true;

                        //Retorna a galeria selecionada
                        return liPrimeiraGaleria.Value;
                    }

                    //Pode ser que não haja nenhuma galeria. Retorna vazio
                    return valorPadraoSelecione;
                }
                catch (Exception) { return valorPadraoSelecione; }
            }

            //Uma galeria já foi selecionada. Retorna
            return galeria;
        }

        /// <summary>
        /// Carrega a lista de fotos com base na galeria informada
        /// </summary>
        /// <param name="galeria"></param>
        private void CarregarFotos(string galeria)
        {
            string gal = this.VerificaEObtemGaleriaPadrao(galeria);

            try
            {
                ListaPaginada<DTOFoto> fotos = null;
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOFoto, FotosItem>>())
                {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate
                    {
                        fotos = new ListaPaginada<DTOFoto>(repository.GetItems(foto => foto.Galeria.Equals(gal)).OrderByDescending(foto => foto.ID), this.Paginador.Pagina, this.ItensPorPagina);
                    });
                }

                //Renderiza as fotos
                this.RenderizarListaFotos(fotos);

                //Configura o paginador
                this.Paginador.MontarPaginador(fotos.TotalItens, this.ItensPorPagina, this.AncoraListagem);

                //Solicita o registro do script de invocação de seleção de uma imagem da lista para renderizar na Exibição de fotos
                this.RegistrarScriptExibicaoImagem(this.fotoTopo);
            }
            catch (Exception)
            {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// Renderiza a lista no controle de lista de fotos
        /// </summary>
        /// <param name="fotos"></param>
        private void RenderizarListaFotos(IEnumerable<DTOFoto> fotos)
        {
            this.rptListagemFotos.DataSource = fotos;
            this.rptListagemFotos.DataBind();
        }

        /// <summary>
        /// Registra no cliente a chamada ao script de exibição de uma determinada foto na Web Part de exibição de fotos
        /// </summary>
        /// <param name="foto"></param>
        private void RegistrarScriptExibicaoImagem(DTOFoto foto)
        {
            //Se não houver fotos no repositório, a variável foto virá com referência nula
            if (foto != null)
            {
                StringBuilder sbFoto_JSON = new StringBuilder();
                sbFoto_JSON.Append("{");
                sbFoto_JSON.AppendFormat("src : \"{0}\"",foto.Url);
                sbFoto_JSON.Append(",");
                sbFoto_JSON.AppendFormat("alt : \"{0}\"",foto.Titulo);
                sbFoto_JSON.Append(",");
                sbFoto_JSON.AppendFormat("description : \"{0}\"",foto.Descricao);
                sbFoto_JSON.Append("}");

                Page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "_selecaoImagem", string.Format("selecionaImagem({0},'{1}','{2}');",sbFoto_JSON,this.MsgTitulo,this.MsgDescritivo), true);
            }
        }
        #endregion
    }
}