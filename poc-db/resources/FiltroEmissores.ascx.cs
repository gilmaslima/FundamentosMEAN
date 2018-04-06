using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato
{
    public partial class FiltroEmissores : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Dados"></param>
        /// <param name="e"></param>
        public delegate void Buscar(BuscarDados Dados);

        /// <summary>
        /// 
        /// </summary>
        public event Buscar onBuscar;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        public void SetarDatas(DateTime _dataInicial, DateTime _dataFinal)
        {
            dataInicial.Text = _dataInicial.ToString("dd/MM/yyyy");
            dataFinal.Text = _dataFinal.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                this.CarregarRelatorios();
                this.SetarRelatorioAtual();
            }
        }

        /// <summary>
        /// Carrega os relatórios disponíveis para o usuário
        /// </summary>
        private void CarregarRelatorios()
        {
            Redecard.PN.Comum.Menu itemAtual = base.ObterMenuItemAtual("/sites/fechado/extrato/Paginas/pn_default.aspx"); // Recuperar item de menu da página de relatórios
            if (!object.ReferenceEquals(itemAtual, null))
            {
                foreach (Redecard.PN.Comum.Menu menu in itemAtual.Items)
                {
                    String nomeRelatorio = menu.Texto;
                    String idRelatorio = String.Empty;
                    if (menu.Paginas.Count > 0)
                    {
                        String url = menu.Paginas[0].Url;
                        idRelatorio = url.Substring(url.Length - 1);
                    }

                    if (!nomeRelatorio.Contains("Consulta") && !nomeRelatorio.Contains("Gerencie"))
                    {
                        // inclui somente os tipos Débitos e Desagendamentos, Suspensos Penhorados Retidos
                        if (idRelatorio.Equals("6") || idRelatorio.Equals("8"))
                            ddlRelatorio.Items.Add(new ListItem(nomeRelatorio, idRelatorio));
                    }
                }
            }
        }

        /// <summary>
        /// Define o relatório atual de acordo com um parametro de QueryString
        /// e 
        /// </summary>
        private void SetarRelatorioAtual()
        {
            String tipo = Request.QueryString["tipo"] as String;
            if (!String.IsNullOrEmpty(tipo))
            {
                ListItem item = ddlRelatorio.Items.FindByValue(tipo);
                if (!object.ReferenceEquals(item, null))
                {
                    item.Selected = true;
                }
            }
        }


        /// <summary>
        /// Validar os campos do filtro
        /// </summary>
        public Boolean ValidarFiltro()
        {
            Int32 _idRelatorio = Int32.Parse(ddlRelatorio.SelectedValue);
            DateTime _dataInicial = DateTime.MinValue;
            DateTime _dataFinal = DateTime.MinValue;
            Int32 _estabelecimento = int.MinValue;
            Boolean _validado = false;

            try
            {
                _dataInicial = DateTime.Parse(dataInicial.Text);
                _dataFinal = DateTime.Parse(dataFinal.Text);

                TimeSpan _data = _dataFinal - _dataInicial;
                if (_data.Days <= 30 && _data.Days >= 0)
                    _validado = true;
                else
                {
                    if (_data.Days < 0)
                        this.ExibirErro("Redecard.PN.Extrato", 315);
                    else
                        this.ExibirErro("Redecard.PN.Extrato", 311);
                }

                _estabelecimento = Int32.Parse(txtEstabelecimento.Text);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante validação de filtro", ex);
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro("Redecard.PN.Extrato", 312);
            }
            return _validado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            base.ExibirPainelExcecao(fonte, codigo);
        }


        /// <summary>
        /// Recuperar o DTO de acordo com os paramêtros informados
        /// </summary>
        /// <returns></returns>
        public BuscarDados RecuperarBuscarDadosDTO()
        {
            String _dataInicial = dataInicial.Text;
            String _dataFinal = dataFinal.Text;

            if (!String.IsNullOrEmpty(_dataInicial) && !String.IsNullOrEmpty(_dataFinal))
            {
                BuscarDados _dtoBuscar = new BuscarDados();
                // não alterar, pois o mainframe espera receber o código zero.
                _dtoBuscar.CodigoBandeira = 0;

                // informar o tipo de Relatório que deve ser apresentado
                _dtoBuscar.IDRelatorio = Int32.Parse(ddlRelatorio.SelectedValue);
                _dtoBuscar.IDTipoVenda = Int32.Parse(ddlTipoVenda.SelectedValue);
                _dtoBuscar.TipoEstabelecimento = -1;//não é usado na pesquisa

                String identificadorRelatorio = String.Format("{0}{1}", _dtoBuscar.IDRelatorio, _dtoBuscar.IDTipoVenda);

                // Seguir processamento normal
                _dtoBuscar.DataInicial = DateTime.Parse(dataInicial.Text);
                _dtoBuscar.DataFinal = DateTime.Parse(dataFinal.Text);
                _dtoBuscar.Estabelecimentos = new Int32[1];
                _dtoBuscar.Estabelecimentos[0] = Convert.ToInt32(txtEstabelecimento.Text);
                return _dtoBuscar;
            }
            return null;
        }
    }
}
