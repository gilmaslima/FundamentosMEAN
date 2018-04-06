/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.AdministracaoServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// UserControl para controle de Permissões
    /// </summary>
    public partial class Permissao : UserControlBase
    {
        #region [ Propriedades Públicas ]

        /// <summary>
        /// Código dos Serviços Selecionados
        /// </summary>
        public List<Int32> ServicosSelecionados
        {
            get
            {
                Dictionary<Boolean, List<Int32>> servicos = this.RecuperarServicos();
                return servicos[true].Distinct().OrderBy(cod => cod).ToList();
            }
        }

        /// <summary>
        /// Flag indicando se todos os serviços estão selecionados no controle
        /// </summary>
        public Boolean TodosSelecionados
        {
            get { return this.ServicosNaoSelecionados.Count == 0; }
        }

        #endregion

        #region [ Propriedades Privadas ]

        /// <summary>
        /// Somente leitura
        /// </summary>        
        private Boolean ReadOnly
        {
            get
            {
                if (ViewState["ReadOnly"] == null)
                    ViewState["ReadOnly"] = false;
                return (Boolean)ViewState["ReadOnly"];
            }
            set
            {
                ViewState["ReadOnly"] = value;
            }
        }

        /// <summary>
        /// Código dos Serviços Não selecionados
        /// </summary>
        private List<Int32> ServicosNaoSelecionados
        {
            get
            {
                Dictionary<Boolean, List<Int32>> servicos = this.RecuperarServicos();
                return servicos[false].Distinct().OrderBy(cod => cod).ToList();
            }
        }

        /// <summary>
        /// Serviço Meu Usuário
        /// </summary>
        private Servico ServicoMeuUsuario
        {
            get
            {
                //Serviço "Minha Conta"
                Int32 codigoMinhaConta = 10000;

                //Descrição do item "Meu Usuário".
                //Obs: Não é por código, pois é incluído dinamicamente, podendo apresentar
                //valores distintos de acordo com o ambiente
                String descricao = "Meu Usuário";

                //Obtém o serviço que possui a descrição "Meu Usuário", e é filho de "Minha Conta"
                var servico = this.Servicos.FirstOrDefault(serv => serv.ServicoPai == codigoMinhaConta
                    && String.Compare(serv.Descricao, descricao, true) == 0);

                return servico;
            }
        }

        /// <summary>
        /// Todos os serviços disponíveis para o usuário
        /// </summary>
        private Servico[] Servicos
        {
            get { return (Servico[])ViewState["Servicos"]; }
            set { ViewState["Servicos"] = value; }
        }

        /// <summary>
        /// Código dos serviços que o usuário já possui permissão
        /// </summary>
        private List<Int32> CodigoServicosUsuario
        {
            get { return (List<Int32>)ViewState["CodigoServicosUsuario"]; }
            set { ViewState["CodigoServicosUsuario"] = value; }
        }

        /// <summary>
        /// Servicoços de nivel 1
        /// </summary>
        private List<Servico> ServicosRoot
        {
            get
            {
                if (ViewState["ServicosRoot"] == null)
                    ViewState["ServicosRoot"] = new List<Servico>();
                return (List<Servico>)ViewState["ServicosRoot"];
            }
            set
            {
                ViewState["ServicosRoot"] = value;
            }
        }

        /// <summary>
        /// Servicoços de nivel 2
        /// </summary>
        private List<Servico> ServicosSegundoNivel
        {
            get
            {
                if (ViewState["ServicosSegundoNivel"] == null)
                    ViewState["ServicosSegundoNivel"] = new List<Servico>();
                return (List<Servico>)ViewState["ServicosSegundoNivel"];
            }
            set
            {
                ViewState["ServicosSegundoNivel"] = value;
            }
        }

        /// <summary>
        /// Servicoços de nivel 3
        /// </summary>
        private List<Servico> ServicosTerceiroNivel
        {
            get
            {
                if (ViewState["ServicosTerceiroNivel"] == null)
                    ViewState["ServicosTerceiroNivel"] = new List<Servico>();
                return (List<Servico>)ViewState["ServicosTerceiroNivel"];
            }
            set
            {
                ViewState["ServicosTerceiroNivel"] = value;
            }
        }

        /// <summary>
        /// Servicoços de nivel 4
        /// </summary>
        private List<Servico> ServicosQuartoNivel
        {
            get
            {
                if (ViewState["ServicosQuartoNivel"] == null)
                    ViewState["ServicosQuartoNivel"] = new List<Servico>();
                return (List<Servico>)ViewState["ServicosQuartoNivel"];
            }
            set
            {
                ViewState["ServicosQuartoNivel"] = value;
            }
        }
        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Carrega o controle com todas as permissoes.
        /// </summary>
        /// <param name="todosServicos">Todos os Serviços existentes</param>
        /// <param name="codServicosUsuario">Código dos serviços que o usuário já possui</param>
        /// <param name="readOnly">Somente leitura</param>
        public void CarregarControle(Servico[] todosServicos, List<Int32> codServicosUsuario, Boolean readOnly)
        {
            //Armazema propriedades do controle
            this.Servicos = todosServicos;
            this.CodigoServicosUsuario = codServicosUsuario;
            this.ReadOnly = readOnly;

            this.ServicosRoot = this.Servicos.Where(s => s.ServicoPai == 0 && DeveExibirServico(this.Servicos, s)).ToList();
            this.ServicosSegundoNivel = this.Servicos.Where(s => this.ServicosRoot.Exists(sp => sp.Codigo == s.ServicoPai) && DeveExibirServico(this.Servicos, s)).ToList();
            this.ServicosTerceiroNivel = this.Servicos.Where(s => this.ServicosSegundoNivel.Exists(sp => sp.Codigo == s.ServicoPai) && DeveExibirServico(this.Servicos, s)).ToList();
            this.ServicosQuartoNivel = this.Servicos.Where(s => this.ServicosTerceiroNivel.Exists(sp => sp.Codigo == s.ServicoPai) && DeveExibirServico(this.Servicos, s)).ToList();

            rptServicosRoot.DataSource = this.ServicosRoot.OrderBy("Posicao");
            rptServicosRoot.DataBind();

            rptServicosSegundoGrupo.DataSource = this.ServicosSegundoNivel.GroupBy(s => s.ServicoPai);
            rptServicosSegundoGrupo.DataBind();

            rptServicosTerceiroGrupo.DataSource = this.ServicosTerceiroNivel.GroupBy(s => s.ServicoPai);
            rptServicosTerceiroGrupo.DataBind();
        }

        #endregion

        #region [ Métodos Privados ]
        /// <summary>
        /// Verifica se deve exibir o serviço ou não.
        /// Se for "Meu Usuário", NUNCA será exibido.
        /// </summary>
        private Boolean DeveExibirServico(Servico[] servicos, Servico servico)
        {
            //Se for "Meu Usuário", NUNCA deve ser exibido
            if (ServicoMeuUsuario != null && ServicoMeuUsuario.Codigo == servico.Codigo)
                return false;
            else
                return Descendentes(servicos, servico).Length > 0;
        }

        /// <summary>
        /// Verifica se deve trazer o serviço marcado ou não
        /// </summary>
        private Boolean DeveSelecionarServico(Servico[] servicos, List<Int32> codServicosUsuario, Servico servico)
        {
            //Somente leitura = Usuário Master
            if (this.ReadOnly)
                return true;

            List<Int32> codFilhos = Descendentes(servicos, servico).Select(s => s.Codigo).ToList();

            foreach (Int32 codFilho in codFilhos)
                if (!codServicosUsuario.Contains(codFilho))
                    return false;

            return true;
        }

        /// <summary>
        /// Retorna os serviços descendentes que sejam do mesmo tipo
        /// </summary>
        private static Servico[] Descendentes(Servico[] servicos, Servico servico)
        {
            var retorno = new List<Servico>();
            retorno.Add(servico);

            var filhos = servicos.Where(s => s.ServicoPai == servico.Codigo)
                .SelectMany(s => Descendentes(servicos, s));

            retorno.AddRange(filhos);
            return retorno.ToArray();
        }

        /// <summary>
        /// Prepara a montagem dos checkboxes
        /// </summary>
        private void PrepararCheckbox(CheckBox chk, Servico servico)
        {
            chk.Attributes["cod"] = servico.Codigo.ToString();
            chk.ToolTip = chk.Text = servico.DescricaoMenu
                .Replace("<br>", String.Empty)
                .Replace("<br/>", String.Empty)
                .Replace("<br />", String.Empty);
            chk.Enabled = !this.ReadOnly;
#if DEBUG
            chk.Text += String.Format(" [{0}]", servico.Codigo);
#endif            

            if (servico.ServicoBasico)
            {
                chk.CssClass = String.Format("{0} item-basico", chk.CssClass);
            }
        }

        /// <summary>
        /// Recupera o código dos serviço, classificando-os em "selecionados" e "não selecionados"
        /// </summary>
        /// <returns>
        /// Dicionário, chaves CHECKED "true"/"false".
        /// Os serviços selecionados estão na chave "true".
        /// Os serviços não selecionados estão na chave "false".
        /// </returns>
        private Dictionary<Boolean, List<Int32>> RecuperarServicos()
        {
            var retorno = new Dictionary<Boolean, List<Int32>>();
            retorno[true] = new List<Int32>();
            retorno[false] = new List<Int32>();

            //Meu usuário SEMPRE deve estar selecionado, então automaticamente adiciona na lista de serviços selecionados
            if (this.ServicoMeuUsuario != null)
                retorno[true].Add(this.ServicoMeuUsuario.Codigo);

            //Percorre o controle, recuperando os itens marcados
            foreach (RepeaterItem itemNivel1 in rptServicosRoot.Items)
            {
                var chkNivel1 = (CheckBox)itemNivel1.FindControl("chkMenu");
                Boolean nivel1Selecionado = chkNivel1.Checked;
                Int32 codNivel1 = chkNivel1.Attributes["cod"].ToInt32();

                retorno[nivel1Selecionado].Add(codNivel1);
            }

            foreach (RepeaterItem grupoNivel2 in rptServicosSegundoGrupo.Items)
            {
                var rptNivel2 = (Repeater)grupoNivel2.FindControl("rptServicosSegundo");
                foreach (RepeaterItem ItemNivel2 in rptNivel2.Items)
                {
                    var chkNivel2 = (CheckBox)ItemNivel2.FindControl("chkMenu");
                    Int32 codNivel2 = chkNivel2.Attributes["cod"].ToInt32();
                    Boolean nivel2Selecionado = chkNivel2.Checked;

                    retorno[nivel2Selecionado].Add(codNivel2);

                    Int32 codNivel1 = 0;

                    Servico servico = this.Servicos.FirstOrDefault(s => s.Codigo == codNivel2);
                    if (servico != null) {
                        codNivel1 = servico.ServicoPai;
                    }

                    //Se nível 2 está selecionado, o nível 1 deve obrigatoriamente ser adicionado também
                    if (nivel2Selecionado && !retorno[true].Contains(codNivel1))
                        retorno[true].Add(codNivel1);
                }
            }

            foreach (RepeaterItem grupoNivel3 in rptServicosTerceiroGrupo.Items)
            {
                var rptNivel3 = (Repeater)grupoNivel3.FindControl("rptServicosTerceiro");
                foreach (RepeaterItem itemNivel3 in rptNivel3.Items)
                {
                    var chkNivel3 = (CheckBox)itemNivel3.FindControl("chkMenu");

                    var rptNivel4 = (Repeater)itemNivel3.FindControl("rptServicosQuarto");


                    Int32 codNivel3 = chkNivel3.Attributes["cod"].ToInt32();
                    Boolean nivel3Selecionado = chkNivel3.Checked;

                    retorno[nivel3Selecionado].Add(codNivel3);

                    Int32 codNivel1 = 0;
                    Int32 codNivel2 = 0;

                    Servico servico = this.Servicos.FirstOrDefault(s => s.Codigo == codNivel3);
                    if (servico != null)
                    {
                        codNivel2 = servico.ServicoPai;
                    }

                    servico = null;
                    servico = this.Servicos.FirstOrDefault(s => s.Codigo == codNivel2);
                    if (servico != null)
                    {
                        codNivel1 = servico.ServicoPai;
                    }

                    //Se nível 3 está selecionado, o nível 1 e 2 devem obrigatoriamente ser adicionados também
                    if (nivel3Selecionado && !retorno[true].Contains(codNivel1))
                        retorno[true].Add(codNivel1);
                    if (nivel3Selecionado && !retorno[true].Contains(codNivel2))
                        retorno[true].Add(codNivel2);

                    foreach (RepeaterItem itemNivel4 in rptNivel4.Items)
                    {
                        var chkNivel4 = (CheckBox)itemNivel4.FindControl("chkMenu");
                        Int32 codNivel4 = chkNivel4.Attributes["cod"].ToInt32();
                        Boolean nivel4Selecionado = chkNivel4.Checked;

                        retorno[nivel4Selecionado].Add(codNivel4);

                        //Se nível 4 está selecionado, o nível 1, 2 e 3 devem obrigatoriamente ser adicionados também
                        if (nivel4Selecionado && !retorno[true].Contains(codNivel1))
                            retorno[true].Add(codNivel1);
                        if (nivel4Selecionado && !retorno[true].Contains(codNivel2))
                            retorno[true].Add(codNivel2);
                        if (nivel4Selecionado && !retorno[true].Contains(codNivel3))
                            retorno[true].Add(codNivel3);
                    }
                }

            }

            // garante que o que esta selecionado nao fique no que nao esta selecionado
            retorno[false] = retorno[false].Except(retorno[true]).ToList();

            return retorno;
        }

        #endregion

        /// <summary>
        /// Evento de bound do item root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosRoot_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var servicoMenu = (Servico)e.Item.DataItem;

                var divPermissaoItem = (HtmlGenericControl)e.Item.FindControl("divPermissaoItem");
                var chkMenu = (CheckBox)e.Item.FindControl("chkMenu");
                var iconeSeta = (HtmlGenericControl)e.Item.FindControl("iconeSeta");
                var spanItemTransacional = (HtmlGenericControl)e.Item.FindControl("spanItemTransacional");

                PrepararCheckbox(chkMenu, servicoMenu);
                chkMenu.Checked = DeveSelecionarServico(this.Servicos, this.CodigoServicosUsuario, servicoMenu);

                Boolean possuiFilhos = this.ServicosSegundoNivel.Exists(s => s.ServicoPai == servicoMenu.Codigo);

                if (possuiFilhos)
                {
                    divPermissaoItem.Attributes["class"] = String.Format("{0} {1}", divPermissaoItem.Attributes["class"], "has-child-rede");
                    iconeSeta.Visible = true;
                }
                spanItemTransacional.Visible = !servicoMenu.ServicoBasico;
            }
        }
        
        /// <summary>
        /// Evento de bound do item segundo nivel agrupado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosSegundoGrupo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IGrouping<int, Servico> grupo = (IGrouping<int, Servico>)e.Item.DataItem;

                var rptServicosSegundo = (Repeater)e.Item.FindControl("rptServicosSegundo");

                rptServicosSegundo.DataSource = grupo.ToList().OrderBy("Posicao");
                rptServicosSegundo.DataBind();
            }
        }

        /// <summary>
        /// Evento de bound do item segundo nivel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosSegundo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var servicoMenu = (Servico)e.Item.DataItem;

                var divPermissaoItem = (HtmlGenericControl)e.Item.FindControl("divPermissaoItem");
                var chkMenu = (CheckBox)e.Item.FindControl("chkMenu");
                var iconeSeta = (HtmlGenericControl)e.Item.FindControl("iconeSeta");
                var spanItemTransacional = (HtmlGenericControl)e.Item.FindControl("spanItemTransacional");

                PrepararCheckbox(chkMenu, servicoMenu);
                chkMenu.Checked = DeveSelecionarServico(this.Servicos, this.CodigoServicosUsuario, servicoMenu);

                Boolean possuiFilhos = this.ServicosTerceiroNivel.Exists(s => s.ServicoPai == servicoMenu.Codigo);

                if (possuiFilhos)
                {
                    divPermissaoItem.Attributes["class"] = String.Format("{0} {1}", divPermissaoItem.Attributes["class"], "has-child-rede");
                    iconeSeta.Visible = true;
                }
                spanItemTransacional.Visible = !servicoMenu.ServicoBasico;
            }
        }
        /// <summary>
        /// Evento de bound do item terceiro nivel agrupado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosTerceiroGrupo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IGrouping<int, Servico> grupo = (IGrouping<int, Servico>)e.Item.DataItem;

                var rptServicosTerceiro = (Repeater)e.Item.FindControl("rptServicosTerceiro");

                rptServicosTerceiro.DataSource = grupo.ToList().OrderBy("Posicao");
                rptServicosTerceiro.DataBind();
            }
        }
        /// <summary>
        /// Evento de bound do item terceiro nivel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosTerceiro_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var servicoMenu = (Servico)e.Item.DataItem;

                var divPermissaoGrupo = (HtmlGenericControl)e.Item.FindControl("divPermissaoGrupo");
                var divPermissaoItem = (HtmlGenericControl)e.Item.FindControl("divPermissaoItem");
                var chkMenu = (CheckBox)e.Item.FindControl("chkMenu");
                var spanItemTransacional = (HtmlGenericControl)e.Item.FindControl("spanItemTransacional");
                var rptServicosQuarto = (Repeater)e.Item.FindControl("rptServicosQuarto");

                PrepararCheckbox(chkMenu, servicoMenu);
                chkMenu.Checked = DeveSelecionarServico(this.Servicos, this.CodigoServicosUsuario, servicoMenu);

                var itensQuartoNivel = this.ServicosQuartoNivel.Where(s => s.ServicoPai == servicoMenu.Codigo);

                if (itensQuartoNivel.Count() > 0)
                {
                    rptServicosQuarto.DataSource = itensQuartoNivel.OrderBy("Posicao");
                    rptServicosQuarto.DataBind();
                }
                else
                {
                    divPermissaoGrupo.Attributes["class"] = String.Empty;
                }
                spanItemTransacional.Visible = !servicoMenu.ServicoBasico;
            }
        }
        /// <summary>
        /// Evento de bound do item quarto nivel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptServicosQuarto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var servicoMenu = (Servico)e.Item.DataItem;

                var divPermissaoItem = (HtmlGenericControl)e.Item.FindControl("divPermissaoItem");
                var chkMenu = (CheckBox)e.Item.FindControl("chkMenu");
                var spanItemTransacional = (HtmlGenericControl)e.Item.FindControl("spanItemTransacional");

                divPermissaoItem.Attributes.Add("data-codigo-pai", servicoMenu.ServicoPai.ToString());
                PrepararCheckbox(chkMenu, servicoMenu);
                chkMenu.Checked = DeveSelecionarServico(this.Servicos, this.CodigoServicosUsuario, servicoMenu);
                spanItemTransacional.Visible = !servicoMenu.ServicoBasico;
            }
        }
    }
}