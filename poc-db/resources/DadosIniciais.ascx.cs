using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.Credenciamento.Sharepoint.Servicos;
using Microsoft.SharePoint;
using System.Collections.Generic;
using Rede.PN.Credenciamento.Modelo;
using System.Linq;
using System.ServiceModel;

namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class DadosIniciais : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]


        private Int64 NumeroCNPJ
        {
            get
            {
                return txtCNPJCPFPessoaJuridica.Text.Replace("/", "").Replace(".", "").Replace("-", "").ToInt64();
            }
        }

        private Int64 NumeroCPF
        {
            get
            {
                return txtCPFPessoaFisica.Text.Replace("/", "").Replace(".", "").Replace("-", "").ToInt64();
            }
        }

        /// <summary>
        /// NumeroMatriz
        /// </summary>
        public Int32 NumeroMatriz
        {
            get
            {
                if (ViewState["NumeroMatriz"] == null)
                    return 0;

                return (Int32)ViewState["NumeroMatriz"];
            }
            set
            {
                ViewState["NumeroMatriz"] = value;
            }
        }

        /// <summary>
        /// Label Endereco, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private Label LabelEndereco
        {
            get
            {
                Label lblEndereco;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    lblEndereco = lblEnderecoPessoaJuridica;
                }
                else
                {
                    lblEndereco = lblEnderecoPessoaFisica;
                }

                return lblEndereco;
            }
        }

        /// <summary>
        /// TextBox CPF, e CNPJ, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private TextBox TextBoxCpfCnpj
        {
            get
            {
                TextBox txtCpfCnpj;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    txtCpfCnpj = txtCNPJCPFPessoaJuridica;
                }
                else
                {
                    txtCpfCnpj = txtCPFPessoaFisica;
                }

                return txtCpfCnpj;
            }
        }

        /// <summary>
        /// TextBox CEP de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private TextBox TextBoxCEP
        {
            get
            {
                TextBox txtCep;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    txtCep = txtCepPessoaJuridica;
                }
                else
                {
                    txtCep = txtCEPPessoaFisica;
                }

                return txtCep;
            }
        }

        /// <summary>
        /// TextBox Nome do cliente, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private TextBox TextBoxNomeCliente
        {
            get
            {
                TextBox txtNomeCliente;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    txtNomeCliente = txtNomeClientePessoaJuridica;
                }
                else
                {
                    txtNomeCliente = txtNomeClientePessoaFisica;
                }

                return txtNomeCliente;
            }
        }

        /// <summary>
        /// TextBox DDD do telefone do cliente, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private TextBox TextBoxDDD
        {
            get
            {
                TextBox txtDDD;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    txtDDD = txtDDDPessoaJuridica;
                }
                else
                {
                    txtDDD = txtDDDPessoaFisica;
                }

                return txtDDD;
            }
        }

        /// <summary>
        /// TextBox do telefone do cliente, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private TextBox TextBoxTelefone
        {
            get
            {
                TextBox txtTelefone;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    txtTelefone = txtTelefonePessoaJuridica;
                }
                else
                {
                    txtTelefone = txtTelefonePessoaFisica;
                }

                return txtTelefone;
            }
        }

        /// <summary>
        /// DropwDownList Canal, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private DropDownList DropDownListCanal
        {
            get
            {
                DropDownList ddlCanal;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    ddlCanal = ddlCanalPessoaJuridica;
                }
                else
                {
                    ddlCanal = ddlCanalPessoaFisica;
                }

                return ddlCanal;
            }
        }

        /// <summary>
        /// DropwDownList Celula, de acordo com o tipo de pessoa selecionada.
        /// </summary>
        private DropDownList DropDownListCelula
        {
            get
            {
                DropDownList ddlCelula;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    ddlCelula = ddlCelulaPessoaJuridica;
                }
                else
                {
                    ddlCelula = ddlCelularPessoaFisica;
                }

                return ddlCelula;
            }
        }

        /// <summary>
        /// DropwDownList Ramo Atividade.
        /// </summary>
        private DropDownList DropDownListRamoAtividade
        {
            get
            {
                DropDownList ddlRamoAtividade;

                if (mvTipoPessoa.ActiveViewIndex == 0)
                {
                    ddlRamoAtividade = ddlRamoAtividadePessoaJuridica;
                }
                else
                {
                    ddlRamoAtividade = ddlRamoAtividadePessoaFisica;
                }

                return ddlRamoAtividade;
            }
        }

        #endregion [Propriedades]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Iniciais - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        //Inicializa o objeto caso a tela seja iniciada do Zero (!Page.IsPostBack)
                        Credenciamento = new Modelo.Credenciamento();
                        CarregarDadosIniciais();
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);

                }
            }
        }

        /// <summary>
        /// Carrega dados Iniciais da Tela
        /// </summary>
        public void CarregarDadosIniciais()
        {
            try
            {
                InicializarObjeto();
                CarregarControles();

                //Se possuir CNPJCPF no carregamento é uma proposta continuada
                if (Credenciamento.Proposta.NumeroCnpjCpf != 0)
                    PreencherTelaPropostaContinuada();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Inicializa Objeto Credenciamanto na View caso seja nulo
        /// </summary>
        private void InicializarObjeto()
        {
            //Obrigatório inicializar o credenciamento quando tela iniciada sem postBack

            if (Credenciamento.Produtos == null)
                Credenciamento.Produtos = new List<Modelo.Produto>();

            if (Credenciamento.Proprietarios == null)
                Credenciamento.Proprietarios = new List<Modelo.Proprietario>();

            if (Credenciamento.Servicos == null)
                Credenciamento.Servicos = new List<Modelo.Servico>();

            if (Credenciamento.Enderecos == null)
                Credenciamento.Enderecos = new List<Modelo.Endereco>();

            if (Credenciamento.DomiciliosBancarios == null)
                Credenciamento.DomiciliosBancarios = new List<Modelo.DomicilioBancario>();

            if (Credenciamento.Proposta == null)
                Credenciamento.Proposta = new Modelo.Proposta();

            if (Credenciamento.Tecnologia == null)
                Credenciamento.Tecnologia = new Modelo.Tecnologia();

            if (Credenciamento.DadosCampanhas == null)
                Credenciamento.DadosCampanhas = new List<Modelo.DadosCampanha>();

            if (Credenciamento.OfertasPrecoUnico == null)
                Credenciamento.OfertasPrecoUnico = new List<Modelo.OfertaPrecoUnico>();

            if (Credenciamento.ProdutoParceiro == null)
                Credenciamento.ProdutoParceiro = new List<Modelo.ProdutoParceiro>();
        }

        /// <summary>
        /// Carrega controles da página
        /// </summary>
        private void CarregarControles()
        {
            CarregarListaCanais();
            CarregarListaCelulas();
            CarregarRamosAtividade();

        }

        /// <summary>
        /// Carrega controle de lista de canais
        /// </summary>
        private void CarregarListaCanais()
        {
            Int32 codCanal = GetIDCanal();

            ddlCanalPessoaFisica.Items.Clear();
            ddlCanalPessoaJuridica.Items.Clear();

            ServicosGE.ConsultaCanais(null, null, "=").ForEach(c =>
            {
                ddlCanalPessoaFisica.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodCanal, c.NomeCanal), c.CodCanal.ToString()));
                ddlCanalPessoaJuridica.Items.Add(new ListItem(String.Format("{0} - {1}", c.CodCanal, c.NomeCanal), c.CodCanal.ToString()));
            });

            ddlCanalPessoaFisica.Items.Insert(0, new ListItem("", ""));
            ddlCanalPessoaJuridica.Items.Insert(0, new ListItem("", ""));

            ddlCanalPessoaFisica.SelectedValue = codCanal.ToString();
            ddlCanalPessoaFisica.Enabled = codCanal == 14 ? true : false;

            ddlCanalPessoaJuridica.SelectedValue = codCanal.ToString();
            ddlCanalPessoaJuridica.Enabled = codCanal == 14 ? true : false;

            Credenciamento.Proposta.CodigoCanal = codCanal;
        }

        /// <summary>
        /// Carrega controle de lista de células
        /// </summary>
        private void CarregarListaCelulas()
        {
            ddlCelularPessoaFisica.Items.Clear();
            ddlCelulaPessoaJuridica.Items.Clear();

            ServicosGE.ConsultaCelulas(CanalSelecionado, null, null).ForEach(c =>
            {
                ddlCelulaPessoaJuridica.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodCelula, c.NomeCelula), c.CodCelula.ToString()));
                ddlCelularPessoaFisica.Items.Add(new ListItem(String.Format(@"{0} - {1}", c.CodCelula, c.NomeCelula), c.CodCelula.ToString()));
            });

            ddlCelularPessoaFisica.Items.Insert(0, new ListItem("", ""));
            ddlCelulaPessoaJuridica.Items.Insert(0, new ListItem("", ""));

            if (ddlCelulaPessoaJuridica.Items.FindByValue(SessaoAtual.CodigoEntidade.ToString()) != null)
                ddlCelulaPessoaJuridica.SelectedValue = SessaoAtual.CodigoEntidade.ToString();

            ddlCelulaPessoaJuridica.Enabled = DropDownListCanal.Enabled;

            if (ddlCelularPessoaFisica.Items.FindByValue(SessaoAtual.CodigoEntidade.ToString()) != null)
                ddlCelularPessoaFisica.SelectedValue = SessaoAtual.CodigoEntidade.ToString();

            ddlCelularPessoaFisica.Enabled = DropDownListCanal.Enabled;
        }

        /// <summary>
        /// Carrega lista dos ramos de atuação disponiveis
        /// </summary>
        private void CarregarRamosAtuacao()
        {
            var ramosAtuacao = ServicosGE.ConsultaRamosAtuacao();
            ddlRamoAtuacaoPessoaJuridica.Items.Clear();

            foreach (var ramo in ramosAtuacao)
            {
                ddlRamoAtuacaoPessoaJuridica.Items.Add(new ListItem
                {
                    Text = String.Format(@"{0} - {1}", ramo.CodGrupoRamoAtividade, ramo.DescrRamoAtividade),
                    Value = ramo.CodGrupoRamoAtividade.ToString()
                });
            }

            ddlRamoAtuacaoPessoaJuridica.Items.Insert(0, new ListItem("", ""));
        }

        /// <summary>
        /// Carrega lista dos ramos de atividade disponiveis.
        /// </summary>
        /// <param name="args">Argumentos</param>
        private void CarregarRamosAtividade()
        {
            ddlRamoAtividadePessoaJuridica.Items.Clear();
            var ramosAtividadeJuridica = ServicosGE.ConsultaRamosAtividade('J', ddlCanalPessoaJuridica.SelectedValue.ToInt32(), null, null).Where(r => r.CodGrupoRamo == ddlRamoAtuacaoPessoaJuridica.SelectedValue.ToInt32());
            foreach (var ramoAtividade in ramosAtividadeJuridica)
            {
                ddlRamoAtividadePessoaJuridica.Items.Add(new ListItem
                {
                    Text = String.Format(@"{0} - {1}", ramoAtividade.CodRamoAtivididade, ramoAtividade.DescRamoAtividade),
                    Value = ramoAtividade.CodRamoAtivididade.ToString()
                });
            }
            ddlRamoAtividadePessoaJuridica.Items.Insert(0, new ListItem("", ""));

            ddlRamoAtividadePessoaFisica.Items.Clear();
            var ramosAtividadeFisica = ServicosGE.ConsultaRamosAtividade('F', ddlCanais.SelectedValue.ToInt32(), null, null);

            ddlRamoAtividadePessoaFisica.Items.AddRange(PreencheListaRamosAtividadePessoaFisica(ramosAtividadeFisica).ToArray());
        }

        /// <summary>
        /// Preenche uma lista de ramos de atividade de pessoa física
        /// </summary>
        /// <param name="ramosAtividade">Lista Ramos de Atividade</param>
        /// <returns>retorna lista de ramos de atividades Pessoa Física</returns>
        private List<ListItem> PreencheListaRamosAtividadePessoaFisica(List<GERamosAtd.ListaRamosAtividadesPorCanalTipoPessoa> ramosAtividade)
        {
            var retorno = new List<ListItem>();

            retorno.Add(new ListItem(String.Empty));
            foreach (var ramo in ramosAtividade.OrderBy(r => r.CodRamoAtivididade))
            {
                retorno.Add(new ListItem
                {
                    Text = String.Format(@"{0} - {1}", ramo.CodRamoAtivididade, ramo.DescRamoAtividade),
                    Value = String.Format(@"{0}{1:0000}", ramo.CodGrupoRamo, ramo.CodRamoAtivididade)
                });
            }

            return retorno;
        }

        /// <summary>
        /// Consulta dados da filial
        /// </summary>
        private void CarregarDadosMatriz(int codigoPdvMatriz, Int64 numeroCnpj)
        {
            Modelo.DadosMatriz dadosMatriz = ServicosWF.RecuperarDadosMatriz(codigoPdvMatriz, TipoOperacao.Comercial, "CR");

            ExibirDadosCNPJ(true);

            if (Credenciamento.Proposta == null)
                Credenciamento.Proposta = new Modelo.Proposta();

            Credenciamento.Proposta.DataFundacao = dadosMatriz.PontoDeVenda.DataFundacao;
            Credenciamento.Proposta.RazaoSocial = dadosMatriz.PontoDeVenda.RazaoSocial;
            Credenciamento.Proposta.CodigoGrupoRamo = dadosMatriz.PontoDeVenda.CodigoGrupoRamo;
            Credenciamento.Proposta.CodigoRamoAtividade = dadosMatriz.PontoDeVenda.CodigoRamoAtivididade;

            Credenciamento.Proprietarios = dadosMatriz.Proprietarios;

            NumeroMatriz = dadosMatriz.PontoDeVenda.NumeroMatriz.HasValue ? dadosMatriz.PontoDeVenda.NumeroMatriz.Value : 0;

            foreach (DomicilioBancarioMatriz domicilioBancarioMatriz in dadosMatriz.DomicilioBancarioMatriz)
            {
                TipoDomicilioBancario tipoDomicilioBancario = TipoDomicilioBancario.Credito;

                if (domicilioBancarioMatriz.CodigoTipoOperacao == TipoOperacao.Credito)
                    tipoDomicilioBancario = TipoDomicilioBancario.Credito;
                else if (domicilioBancarioMatriz.CodigoTipoOperacao == TipoOperacao.Debito)
                    tipoDomicilioBancario = TipoDomicilioBancario.Debito;
                else if (domicilioBancarioMatriz.CodigoTipoOperacao == TipoOperacao.Construcard)
                    tipoDomicilioBancario = TipoDomicilioBancario.Construcard;

                Credenciamento.DomiciliosBancarios.Add(
                    new Modelo.DomicilioBancario
                    {
                        TipoDomicilioBancario = tipoDomicilioBancario,
                        CodigoTipoPessoa = rbTipoPessoa.SelectedValue.ElementAt(0),
                        NumeroCNPJ = numeroCnpj,
                        NumeroSeqProp = Credenciamento.Proposta.IndicadorSequenciaProposta,
                        IndicadorTipoOperacaoProd = (int)domicilioBancarioMatriz.CodigoTipoOperacao,
                        IndicadorDomicilioDuplicado = ' ',
                        CodigoBanco = domicilioBancarioMatriz.CodigoBancoAtual.Value,
                        NomeBanco = domicilioBancarioMatriz.NomeBancoAtual,
                        CodigoAgencia = domicilioBancarioMatriz.CodigoAgenciaAtual.Value,
                        NumeroContaCorrente = domicilioBancarioMatriz.NumeroContaAtual,
                        DataHoraUltimaAtualizacao = DateTime.Now,
                        UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                        IndConfirmacaoDomicilio = ' ',
                        IndicadorTipoAcaoBanco = TipoAcaoBanco.Inclusao
                    }
                );
            }

            //Controle de Campos
            PermitirEdicaoDadosCNPJ(TipoEstabelecimento.Filial, false);
            //Preenchimento dos campos com dados
            CarregarDadosCNPJ();
        }

        /// <summary>
        /// Consulta dados do Serasa para pessoas físca ou jurídica
        /// </summary>
        private void CarregarDadosSerasa()
        {
            Modelo.Serasa dadosPJ;
            dadosPJ = ServicosWF.ConsultarDadosCNPJ(Modelo.TipoPessoa.Juridica, NumeroCNPJ, GetIDCanal(), 'A');
            bool retornoSerasa = false;


            ExibirDadosCNPJ(true);
            if (Credenciamento.Proposta == null)
                Credenciamento.Proposta = new Modelo.Proposta();

            if (dadosPJ.RetornoSerasa == RetornoSerasa.Sim)
            {
                retornoSerasa = true;



                Credenciamento.Proposta.DataFundacao = dadosPJ.Empresa.DataFundacao;
                Credenciamento.Proposta.CodigoCNAE = dadosPJ.Cnae.CodigoClasseCNAE;
                Credenciamento.Proposta.RazaoSocial = dadosPJ.Empresa.RazaoSocial;
                Credenciamento.Proprietarios = dadosPJ.Proprietarios.ToList();

                //Dados Grupo Ramo e Ramo Atividade
                Credenciamento.Proposta.CodigoGrupoRamo = dadosPJ.Cnae.CodigoGrupoRamo;
                Credenciamento.Proposta.CodigoRamoAtividade = dadosPJ.Cnae.CodigoRamoAtivididade;

                foreach (Proprietario objProprietario in Credenciamento.Proprietarios)
                    objProprietario.DadosRetornadosSerasa = true;
            }

            PermitirEdicaoDadosCNPJ(TipoEstabelecimento.Matriz, retornoSerasa);

            CarregarDadosCNPJ();
        }

        /// <summary>
        /// Valida Visibilidade e Edição dos campos do GridView de Proprietários 
        /// </summary>
        /// <param name="participacaoAcionaria">participacaoAcionaria</param>
        private void AplicarVisibilidadeEdicaoProprietarios()
        {
            double participacaoAcionaria = 0;
            bool? DadosRetornadosSerasa = null;
            bool regraParticipacaoMenor = false;

            foreach (Proprietario objProprietario in Credenciamento.Proprietarios)
            {
                //Soma de Participação acionária
                participacaoAcionaria += objProprietario.ParticipacaoAcionaria;

                //Alimenta cm retorno Serasa
                if (DadosRetornadosSerasa == null)
                    DadosRetornadosSerasa = Credenciamento.Proprietarios.First().DadosRetornadosSerasa;
            }

            regraParticipacaoMenor = participacaoAcionaria < 51;

            //valor padrão
            lbNovoProprietario.Enabled = true;

            foreach (GridViewRow row in gvProprietario.Rows)
            {
                //Se dados retornados do Serasa bloquear edição, exclusão e inserção de Proprietários
                if (DadosRetornadosSerasa == true)
                {
                    ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Enabled = false;
                    ((TextBox)row.FindControl("txtGridViewNomePropietario")).Enabled = false;
                    ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Enabled = regraParticipacaoMenor;

                    ((ImageButton)row.FindControl("ibGridViewRemover")).Enabled = false;
                    lbNovoProprietario.Enabled = false;
                }
                else
                {
                    /// Solicitado por Fernando Azeituno e Vinicius Oberdan em 31/05/2016
                    /// remoção de bloco de código para travar preenchimento de proprietários em caso de Filial,
                    /// devido a PV cancelado entrar nesta regra e não permitir continuidade.
                    ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Enabled = true;
                    ((TextBox)row.FindControl("txtGridViewNomePropietario")).Enabled = true;
                    ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Enabled = true;

                    ((ImageButton)row.FindControl("ibGridViewRemover")).Enabled = true;
                    lbNovoProprietario.Enabled = true;
                }

                //Muda mascara para aceitar numeros decimais quando retorno do serasa
                if (((TextBox)row.FindControl("txtGridViewPartAcionaria")).Enabled)
                {
                    ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Text = ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Text.Split(',')[0];
                    ((TextBox)row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-decimal-participacao", "mascara-participacao");
                }
                else
                {
                    if (Credenciamento.Proposta.CodigoTipoEstabelecimento != 1 || !Credenciamento.Proposta.NumeroMatriz.HasValue)
                        ((TextBox)row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-participacao", "mascara-decimal-participacao");
                }

                //Setar Mascara do Campo CNPJ/CPF Quando informação vêm do objeto - E quando é preenchido na tela seta via javascript: onblur="MascaraCPFCNPJ(this)
                var tipoPessoaProprietario = ((HiddenField)row.FindControl("hfTipoPessoaProprietario")).Value;
                var txtCpfCnpj = ((TextBox)row.FindControl("txtGridViewCNPJCPF"));
                if (tipoPessoaProprietario.Equals("J"))
                    txtCpfCnpj.Text = txtCpfCnpj.Text.CpfCnpjToLong().FormatToCnpj();
                else if (tipoPessoaProprietario.Equals("F"))
                    txtCpfCnpj.Text = txtCpfCnpj.Text.CpfCnpjToLong().FormatToCpf();

            }
        }

        /// <summary>
        /// Popula os controles de dados de CPNJ, com as informações da Session de Credenciamento.
        /// </summary>
        private void CarregarDadosCNPJ()
        {

            ExibirDadosCNPJ(true);

            txtRazaoSocialPessoaJuridica.Text = Credenciamento.Proposta.RazaoSocial;
            txtDataFundacaoPessoaJuridica.Text = Credenciamento.Proposta.DataFundacao != null ? Credenciamento.Proposta.DataFundacao.Value.ToString("dd/MM/yyyy") : String.Empty;
            txtCNAEPessoaJuridica.Text = Credenciamento.Proposta.CodigoCNAE;

            CarregarRamosAtuacao();

            if (Credenciamento.Proposta.CodigoGrupoRamo.HasValue && Credenciamento.Proposta.CodigoGrupoRamo != 0)
            {
                ddlRamoAtuacaoPessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoGrupoRamo.ToString();
            }

            CarregarRamosAtividade();
            if (Credenciamento.Proposta.CodigoRamoAtividade.HasValue && Credenciamento.Proposta.CodigoRamoAtividade != 0)
            {
                ddlRamoAtividadePessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoRamoAtividade.ToString();
            }


            PopularGridProprietarios();
        }

        /// <summary>
        /// Busca ID do Canal de acordo com a Entidade logada no portal
        /// </summary>
        /// <returns>retorna ID do Canal</returns>
        private Int32 GetIDCanal()
        {
            SPList list = SPContext.Current.Web.Lists.TryGetList("EntidadeCanal");
            SPQuery query = new SPQuery();
            query.Query = String.Format("<Where><Eq><FieldRef Name='IDGrupoEntidade' /><Value Type='Text'>{0}</Value></Eq></Where>", SessaoAtual.GrupoEntidade);
            query.RowLimit = 1;

            SPListItemCollection items = list.GetItems(query);
            if (items.Count > 0)
                return items[0]["IDCanal"].ToString().ToInt32(4);

            return 4;

        }

        /// <summary>
        /// Busca o Canal selecionado no dropdown de canal
        /// </summary>
        /// <returns>retorna valor do Canal Selecionado</returns>
        private int CanalSelecionado
        {
            get
            {
                return DropDownListCanal.SelectedValue.ToInt32();
            }
        }

        /// <summary>
        /// Evento disparado ao clicar no botão Salvar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                rqfRamoAtuacaoPessoaJuridica.Enabled = false;
                rqfRamoAtividadePessoaJuridica.Enabled = false;
                rqfRazaoSocialPessoaJuridica.Enabled = false;
                cvDataFundacaoPessoaJuridica.Enabled = false;
                cvDataNascPessoaFisica.Enabled = false;
                rqfRamoAtividadePessoaFisica.Enabled = false;

                Page.Validate();
                if (Page.IsValid)
                {

                    CapturarInformacoesPreenchidas();
                    Credenciamento.Proposta.CodigoFaseFiliacao = 0;
                    Credenciamento.Proposta.IndicadorSequenciaProposta = ServicosWF.SalvarDadosIniciais(PreencheProposta(), Credenciamento.Enderecos.FirstOrDefault(), Credenciamento.Proprietarios, Credenciamento.DomiciliosBancarios);
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            finally
            {
                Credenciamento = null;
                NovaProposta(sender, e);
            }

        }

        public event EventHandler NovaProposta;

        /// <summary>
        /// Evento do clique no botão continuar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Iniciais - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        SalvarDadosIniciais();
                        Continuar(sender, e);
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        /// <summary>
        /// Preenche tela com dados retornados do serivço Continuar Proposta
        /// </summary>
        private void PreencherTelaPropostaContinuada()
        {
            //CONTROLE DE EXIBIÇÃO DE VIEW
            if (Credenciamento.Proposta.CodigoTipoPessoa == 'J')
            {
                mvTipoPessoa.ActiveViewIndex = 0;
                ExibirDadosCNPJ(true);
                CarregarRamosAtuacao();
                CarregarRamosAtividade();
            }
            else
            {
                mvTipoPessoa.ActiveViewIndex = 1;
            }

            if (Credenciamento.Proposta.CodigoTipoMovimento != 'U' && Credenciamento.Proposta.CodigoTipoPessoa == 'J')
            {
                //UPDATE DE DADOS CASO O ITEM RETORNE DO SERASA E CONTROLE DE TELA
                if (Credenciamento.Proposta.CodigoTipoEstabelecimento != null && Credenciamento.Proposta.CodigoTipoEstabelecimento == 1)
                {//Se for Filial controle de tela de filial e buscar dados da Matriz novamente

                    //Controles de tela
                    CarregarRamosAtuacao();

                    if (Credenciamento.Proposta.NumeroMatriz.HasValue)
                    {
                        //Carregar Dados Matriz é chamado pois a empresa é uma filial da Matriz e buscará os dados da Matriz
                        CarregarDadosMatriz(Credenciamento.Proposta.NumeroMatriz.Value, Credenciamento.Proposta.NumeroCnpjCpf);
                    }

                    PermitirEdicaoDadosCNPJ(TipoEstabelecimento.Filial, false);
                }
                else
                {
                    //Se for Matriz, recuperar dados do Serasa (Com lógica de retornoSerasa=Sim/Não dentro do Método)
                    CarregarDadosSerasa();
                }
            }
            else
            {
                //UPDATE DE DADOS CASO O ITEM RETORNE DO SERASA E CONTROLE DE TELA
                if (Credenciamento.Proposta.CodigoTipoEstabelecimento != null && Credenciamento.Proposta.CodigoTipoEstabelecimento == 1)
                    //Se for Filial controle de tela de filial
                    PermitirEdicaoDadosCNPJ(TipoEstabelecimento.Filial, false);
                //else
                //Se for Matriz, recuperar dados do Serasa (Com lógica de retornoSerasa=Sim/Não dentro do Método)
                //CarregarDadosSerasa();
            }


            //PREENCHER DADOS NA TELA
            rbTipoPessoa.SelectedValue = Credenciamento.Proposta.CodigoTipoPessoa.ToString();

            if (Credenciamento.Proposta.CodigoTipoPessoa == 'J')
            {
                txtCNPJCPFPessoaJuridica.Text = Credenciamento.Proposta.NumeroCnpjCpf.FormatToCnpj();

                //if (Credenciamento.Proposta.CodigoCanal != null)
                //    ddlCanalPessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoCanal.ToString();

                //if (Credenciamento.Proposta.CodigoCelula != null)
                //    ddlCelulaPessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoCelula.ToString();

                if (Credenciamento.Proposta.CodigoGrupoRamo != null && Credenciamento.Proposta.CodigoGrupoRamo != 0)
                {
                    ddlRamoAtuacaoPessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoGrupoRamo.ToString();
                    CarregarRamosAtividade();

                    if (Credenciamento.Proposta.CodigoRamoAtividade != null && Credenciamento.Proposta.CodigoRamoAtividade != 0)
                        ddlRamoAtividadePessoaJuridica.SelectedValue = Credenciamento.Proposta.CodigoRamoAtividade.ToString();
                }

                if (Credenciamento.Proposta.DataFundacao != null)
                    txtDataFundacaoPessoaJuridica.Text = Credenciamento.Proposta.DataFundacao.Value.ToString("dd/MM/yyyy");

                if (Credenciamento.Proposta.NumeroTelefone1 != null)
                    txtTelefonePessoaJuridica.Text = Credenciamento.Proposta.NumeroTelefone1.ToString();

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.CodigoCNAE))
                    txtCNAEPessoaJuridica.Text = Credenciamento.Proposta.CodigoCNAE.Trim();

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.PessoaContato))
                    txtNomeClientePessoaJuridica.Text = Credenciamento.Proposta.PessoaContato.Trim();

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD1))
                    txtDDDPessoaJuridica.Text = Credenciamento.Proposta.NumeroDDD1.Trim();

                if (Credenciamento.Proposta.Ramal1 != null)
                    txtRamalPessoaJuridica.Text = Credenciamento.Proposta.Ramal1.ToString();

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.RazaoSocial))
                    txtRazaoSocialPessoaJuridica.Text = Credenciamento.Proposta.RazaoSocial.Trim();

            }
            else
            {
                txtCPFPessoaFisica.Text = Credenciamento.Proposta.NumeroCnpjCpf.FormatToCpf();
                //if (Credenciamento.Proposta.CodigoCanal != null)
                //    ddlCanalPessoaFisica.SelectedValue = Credenciamento.Proposta.CodigoCanal.ToString();

                //if (Credenciamento.Proposta.CodigoCelula != null)
                //    ddlCelularPessoaFisica.SelectedValue = Credenciamento.Proposta.CodigoCelula.ToString();

                if (Credenciamento.Proposta.CodigoGrupoRamo != null && Credenciamento.Proposta.CodigoRamoAtividade != null)
                    ddlRamoAtividadePessoaFisica.SelectedValue = String.Format("{0}{1:0000}", Credenciamento.Proposta.CodigoGrupoRamo, Credenciamento.Proposta.CodigoRamoAtividade);

                if (Credenciamento.Proposta.NumeroTelefone1 != null)
                    txtTelefonePessoaFisica.Text = Credenciamento.Proposta.NumeroTelefone1.ToString();

                if (Credenciamento.Proposta.DataFundacao != null)
                    txtDataNascPessoaFisica.Text = Credenciamento.Proposta.DataFundacao.Value.ToString("dd/MM/yyyy");

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.PessoaContato))
                    txtNomeClientePessoaFisica.Text = Credenciamento.Proposta.PessoaContato.Trim();

                if (!String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD1))
                    txtDDDPessoaFisica.Text = Credenciamento.Proposta.NumeroDDD1.Trim();

            }

            //Endereco
            if (Credenciamento.Enderecos != null && Credenciamento.Enderecos.Count > 0)
            {
                if (Credenciamento.Proposta.CodigoTipoPessoa == 'J')
                {
                    txtCepPessoaJuridica.Text = String.Format("{0}-{1}", Credenciamento.Enderecos.FirstOrDefault().CodigoCep, Credenciamento.Enderecos.FirstOrDefault().CodigoComplementoCep);

                    BuscaCEP(txtCepPessoaJuridica, lblEnderecoPessoaJuridica);
                }
                else
                {
                    txtCEPPessoaFisica.Text = String.Format("{0}-{1}", Credenciamento.Enderecos.FirstOrDefault().CodigoCep, Credenciamento.Enderecos.FirstOrDefault().CodigoComplementoCep);
                    BuscaCEP(txtCEPPessoaFisica, lblEnderecoPessoaFisica);
                }
            }

            //Proprietarios
            if (Credenciamento.Proprietarios.Count() > 0 && Credenciamento.Proposta.CodigoTipoPessoa == 'J')
            {
                PopularGridProprietarios();
            }



        }

        /// <summary>
        /// Método que busca informaçoes preenchidas na tela, e as coloca na viewstate de credenciamento
        /// </summary>
        private void CapturarInformacoesPreenchidas()
        {
            Credenciamento.Proposta.CodigoTipoPessoa = rbTipoPessoa.SelectedValue.ElementAt(0);
            Credenciamento.Proposta.NumeroCnpjCpf = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64();

            if (!String.IsNullOrEmpty(DropDownListCanal.SelectedValue))
                Credenciamento.Proposta.CodigoCanal = DropDownListCanal.SelectedValue.ToInt32();


            Credenciamento.Proposta.CodigoTipoMovimento = Credenciamento.Proposta.CodigoTipoMovimento.HasValue ? Credenciamento.Proposta.CodigoTipoMovimento.Value : 'I';
            if (mvTipoPessoa.ActiveViewIndex == 0)
            {
                Credenciamento.Proposta.DataFundacao = txtDataFundacaoPessoaJuridica.Text.ToDate("dd/MM/yyyy");
                Credenciamento.Proposta.CodigoGrupoRamo = ddlRamoAtuacaoPessoaJuridica.SelectedValue.ToInt32();
                Credenciamento.Proposta.CodigoRamoAtividade = ddlRamoAtividadePessoaJuridica.SelectedValue.ToInt32();
            }
            else
            {
                Credenciamento.Proposta.DataFundacao = txtDataNascPessoaFisica.Text.ToDate("dd/MM/yyyy");

                if (!String.IsNullOrEmpty(ddlRamoAtividadePessoaFisica.SelectedValue))
                    Credenciamento.Proposta.CodigoGrupoRamo = ddlRamoAtividadePessoaFisica.SelectedValue.Substring(0, 1).ToInt32();
                if (!String.IsNullOrEmpty(ddlRamoAtividadePessoaFisica.SelectedValue))
                    Credenciamento.Proposta.CodigoRamoAtividade = ddlRamoAtividadePessoaFisica.SelectedValue.Substring(1, 4).ToInt32();
            }

            if (Credenciamento.Proposta.CodigoTipoPessoa.Equals('J'))
                Credenciamento.Proposta.RazaoSocial = txtRazaoSocialPessoaJuridica.Text.ToUpper();
            else
                Credenciamento.Proposta.RazaoSocial = txtNomeClientePessoaFisica.Text.ToUpper();

            if (!String.IsNullOrEmpty(TextBoxNomeCliente.Text))
                Credenciamento.Proposta.PessoaContato = TextBoxNomeCliente.Text.ToUpper();

            Credenciamento.Proposta.NumeroDDD1 = TextBoxDDD.Text;
            Credenciamento.Proposta.NumeroTelefone1 = TextBoxTelefone.Text.RemoverCaracteresEspeciais().RemoverAcentos().ToInt32();

            Credenciamento.Proposta.Ramal1 = mvTipoPessoa.ActiveViewIndex == 0 && !String.IsNullOrEmpty(txtRamalPessoaJuridica.Text) ?
                txtRamalPessoaJuridica.Text.RemoverCaracteresEspeciais().RemoverAcentos().ToInt32() : 0;


            if (Credenciamento.Proposta.IndicadorEnderecoIgualComercial == null)
                Credenciamento.Proposta.IndicadorEnderecoIgualComercial = 'S';

            if (Credenciamento.Proposta.IndicadorAcessoInternet == null)
                Credenciamento.Proposta.IndicadorAcessoInternet = 'N';

            if (!String.IsNullOrEmpty(DropDownListCelula.SelectedValue))
                Credenciamento.Proposta.CodigoCelula = DropDownListCelula.SelectedValue.ToInt32();
            else
                Credenciamento.Proposta.CodigoCelula = 0;

            Credenciamento.Proposta.NomeFatura = Credenciamento.Proposta.NomeFatura ?? String.Empty;
            Credenciamento.Proposta.CodigoAgenciaFiliacao = 0;
            Credenciamento.Proposta.CodigoTerceirizacaoVista = 0;
            Credenciamento.Proposta.DataCadastroProposta = DateTime.Now;
            Credenciamento.Proposta.CodigoImpressoraFiscal = 0;
            Credenciamento.Proposta.IndicadorFinanceira = 'N';
            Credenciamento.Proposta.CodigoFinanceira1 = 0;
            Credenciamento.Proposta.CodigoFinanceira2 = 0;
            Credenciamento.Proposta.CodigoFinanceira3 = 0;
            Credenciamento.Proposta.SituacaoProposta = 'C';
            Credenciamento.Proposta.CodigoPesoTarget = 0;
            Credenciamento.Proposta.CodigoPeriodicidadeRAV = ' ';
            Credenciamento.Proposta.CodigoPeriodicidadeDia = 0;
            Credenciamento.Proposta.IndicadorEnvioForcaVenda = "N";
            Credenciamento.Proposta.IndicadorOrigemProposta = "Portal";
            Credenciamento.Proposta.IndicadorControle = "PI";
            Credenciamento.Proposta.IndicadorComercializacaoNormal = ' ';
            Credenciamento.Proposta.IndicadorComercializacaoCatalogo = ' ';
            Credenciamento.Proposta.IndicadorComercializacaoTelefone = ' ';
            Credenciamento.Proposta.IndicadorComercializacaoEletronico = ' ';
            Credenciamento.Proposta.CodigoCNAE = txtCNAEPessoaJuridica.Text;

            Credenciamento.Proposta.UsuarioUltimaAtualizacao = this.SessaoAtual.LoginUsuario;
            Credenciamento.Proposta.UsuarioInclusao = this.SessaoAtual.LoginUsuario;

            if (Credenciamento.Proposta.NomeEmail == null)
                Credenciamento.Proposta.NomeEmail = " ";
            if (Credenciamento.Proposta.NomeHomePage == null)
                Credenciamento.Proposta.NomeHomePage = " ";
            if (Credenciamento.Proposta.NumeroDDDFax == null)
                Credenciamento.Proposta.NumeroDDDFax = " ";
            if (Credenciamento.Proposta.NumeroTelefoneFax == null)
                Credenciamento.Proposta.NumeroTelefoneFax = 0;
            if (Credenciamento.Proposta.IndicadorRegiaoLoja == null)
                Credenciamento.Proposta.IndicadorRegiaoLoja = ' ';
            if (Credenciamento.Proposta.NomePlaqueta1 == null)
                Credenciamento.Proposta.NomePlaqueta1 = " ";
            if (Credenciamento.Proposta.NomePlaqueta2 == null)
                Credenciamento.Proposta.NomePlaqueta2 = " ";
            if (Credenciamento.Proposta.CodigoFilial == null)
                Credenciamento.Proposta.CodigoFilial = 0;
            if (Credenciamento.Proposta.CodigoZona == null)
                Credenciamento.Proposta.CodigoZona = 0;
            if (Credenciamento.Proposta.CodigoNucleo == null)
                Credenciamento.Proposta.CodigoNucleo = 0;
            if (Credenciamento.Proposta.IndicadorMaquineta == null)
                Credenciamento.Proposta.IndicadorMaquineta = 'N';
            if (Credenciamento.Proposta.QuantidadeMaquineta == null)
                Credenciamento.Proposta.QuantidadeMaquineta = 0;
            if (Credenciamento.Proposta.IndicadorIATA == null)
                Credenciamento.Proposta.IndicadorIATA = ' ';
            if (Credenciamento.Proposta.NumeroConsignador == null)
                Credenciamento.Proposta.NumeroConsignador = 0;
            if (Credenciamento.Proposta.IndicadorSolicitacaoTecnologia == null)
                Credenciamento.Proposta.IndicadorSolicitacaoTecnologia = 'N';
            if (Credenciamento.Proposta.CodigoAgenciaFiliacao == null)
                Credenciamento.Proposta.CodigoAgenciaFiliacao = 0;
            if (Credenciamento.Proposta.CodigoTerceirizacaoVista == null)
                Credenciamento.Proposta.CodigoTerceirizacaoVista = 0;
            if (Credenciamento.Proposta.DataCadastroProposta == null)
                Credenciamento.Proposta.DataCadastroProposta = DateTime.Now;
            if (Credenciamento.Proposta.CodigoFaseFiliacao == null || Credenciamento.Proposta.CodigoFaseFiliacao < 1)
                Credenciamento.Proposta.CodigoFaseFiliacao = 1;
            if (Credenciamento.Proposta.CodigoImpressoraFiscal == null)
                Credenciamento.Proposta.CodigoImpressoraFiscal = 0;
            if (Credenciamento.Proposta.IndicadorFinanceira == null)
                Credenciamento.Proposta.IndicadorFinanceira = 'N';
            if (Credenciamento.Proposta.CodigoFinanceira1 == null)
                Credenciamento.Proposta.CodigoFinanceira1 = 0;
            if (Credenciamento.Proposta.CodigoFinanceira2 == null)
                Credenciamento.Proposta.CodigoFinanceira2 = 0;
            if (Credenciamento.Proposta.CodigoFinanceira3 == null)
                Credenciamento.Proposta.CodigoFinanceira3 = 0;
            if (Credenciamento.Proposta.SituacaoProposta == null)
                Credenciamento.Proposta.SituacaoProposta = 'C';
            if (Credenciamento.Proposta.CodigoPesoTarget == null)
                Credenciamento.Proposta.CodigoPesoTarget = 0;
            if (Credenciamento.Proposta.CodigoPeriodicidadeRAV == null)
                Credenciamento.Proposta.CodigoPeriodicidadeRAV = ' ';
            if (Credenciamento.Proposta.CodigoPeriodicidadeDia == null)
                Credenciamento.Proposta.CodigoPeriodicidadeDia = 0;
            if (Credenciamento.Proposta.IndicadorEnvioForcaVenda == null)
                Credenciamento.Proposta.IndicadorEnvioForcaVenda = "N";
            if (Credenciamento.Proposta.IndicadorOrigemProposta == null)
                Credenciamento.Proposta.IndicadorOrigemProposta = "Portal";
            if (Credenciamento.Proposta.IndicadorControle == null)
                Credenciamento.Proposta.IndicadorControle = "PI";
            if (Credenciamento.Proposta.IndicadorComercializacaoNormal == null)
                Credenciamento.Proposta.IndicadorComercializacaoNormal = ' ';
            if (Credenciamento.Proposta.IndicadorComercializacaoCatalogo == null)
                Credenciamento.Proposta.IndicadorComercializacaoCatalogo = ' ';
            if (Credenciamento.Proposta.IndicadorComercializacaoTelefone == null)
                Credenciamento.Proposta.IndicadorComercializacaoTelefone = ' ';
            if (Credenciamento.Proposta.IndicadorComercializacaoEletronico == null)
                Credenciamento.Proposta.IndicadorComercializacaoEletronico = ' ';
            if (Credenciamento.Proposta.NumeroTelefone2 == null)
                Credenciamento.Proposta.NumeroTelefone2 = 0;
            if (Credenciamento.Proposta.NumeroDDD2 == null)
                Credenciamento.Proposta.NumeroDDD2 = " ";
            if (Credenciamento.Proposta.Ramal2 == null)
                Credenciamento.Proposta.Ramal2 = 0;
            if (Credenciamento.Proposta.NomeProprietario1 == null)
                Credenciamento.Proposta.NomeProprietario1 = null;
            if (Credenciamento.Proposta.NumeroCPFProprietario1 == null)
                Credenciamento.Proposta.NumeroCPFProprietario1 = 0;
            if (Credenciamento.Proposta.NomeProprietario2 == null)
                Credenciamento.Proposta.NomeProprietario2 = null;
            if (Credenciamento.Proposta.NumeroCPFProprietario2 == null)
                Credenciamento.Proposta.NumeroCPFProprietario2 = 0;
            if (Credenciamento.Proposta.DataNascProprietario2 == null)
                Credenciamento.Proposta.DataNascProprietario2 = null;
            if (Credenciamento.Proposta.TipoPessoaProprietario1 == null)
                Credenciamento.Proposta.TipoPessoaProprietario1 = ' ';
            if (Credenciamento.Proposta.TipoPessoaProprietario2 == null)
                Credenciamento.Proposta.TipoPessoaProprietario2 = ' ';
            if (Credenciamento.Proposta.UfConselhoRegional == null)
                Credenciamento.Proposta.UfConselhoRegional = " ";
            if (Credenciamento.Proposta.NumeroConselhoRegional == null)
                Credenciamento.Proposta.NumeroConselhoRegional = "";
            if (Credenciamento.Proposta.CodigoTipoConselhoRegional == null)
                Credenciamento.Proposta.CodigoTipoConselhoRegional = "";
            if (Credenciamento.Proposta.IndicadorControle == null)
                Credenciamento.Proposta.IndicadorControle = " ";
            if (Credenciamento.Proposta.IndicadorProntaInstalacao == null)
                Credenciamento.Proposta.IndicadorProntaInstalacao = ' ';
            if (Credenciamento.Proposta.CodigoEVD == null)
                Credenciamento.Proposta.CodigoEVD = 0;
            if (Credenciamento.Proposta.CodigoGerencia == null)
                Credenciamento.Proposta.CodigoGerencia = ' ';
            if (Credenciamento.Proposta.CodigoCarteira == null)
                Credenciamento.Proposta.CodigoCarteira = 0;
            if (Credenciamento.Proposta.CodigoHoraFuncionamentoPV == null)
                Credenciamento.Proposta.CodigoHoraFuncionamentoPV = 0;
            if (Credenciamento.Proposta.CodigoTipoConsignacao == null)
                Credenciamento.Proposta.CodigoTipoConsignacao = 0;

            //Criar ViewState
            Credenciamento.Proposta.NumeroMatriz = Credenciamento.Proposta.NumeroMatriz != null ? Credenciamento.Proposta.NumeroMatriz : 0;

            if (Credenciamento.Proposta.NumeroGrupoComercial == null)
                Credenciamento.Proposta.NumeroGrupoComercial = 0;
            if (Credenciamento.Proposta.NumeroGrupoGerencial == null)
                Credenciamento.Proposta.NumeroGrupoGerencial = 0;
            if (Credenciamento.Proposta.CodigoLocalPagamento == null)
                Credenciamento.Proposta.CodigoLocalPagamento = 0;
            if (Credenciamento.Proposta.NumeroCentralizadora == null)
                Credenciamento.Proposta.NumeroCentralizadora = 0;
            if (Credenciamento.Proposta.CodigoRoteiro == null)
                Credenciamento.Proposta.CodigoRoteiro = 0;
            if (Credenciamento.Proposta.CodigoAgenciaFiliacao == null)
                Credenciamento.Proposta.CodigoAgenciaFiliacao = 0;
            if (Credenciamento.Proposta.CodigoTerceirizacaoVista == null)
                Credenciamento.Proposta.CodigoTerceirizacaoVista = 0;
            if (Credenciamento.Proposta.DataCadastroProposta == null)
                Credenciamento.Proposta.DataCadastroProposta = DateTime.Now;
            if (Credenciamento.Proposta.NumeroCPFVendedor == null)
                Credenciamento.Proposta.NumeroCPFVendedor = 0;
            if (Credenciamento.Proposta.ValorTaxaAdesao == null)
                Credenciamento.Proposta.ValorTaxaAdesao = 0;
            if (Credenciamento.Proposta.CodigoCampanha == null)
                Credenciamento.Proposta.CodigoCampanha = " ";
            if (Credenciamento.Proposta.CodModeloProposta == null)
                Credenciamento.Proposta.CodModeloProposta = ' ';
            if (Credenciamento.Proposta.IndicadorEnvioExtratoEmail == null)
                Credenciamento.Proposta.IndicadorEnvioExtratoEmail = ' ';
            if (Credenciamento.Proposta.CodigoTipoConselhoRegional == null)
                Credenciamento.Proposta.CodigoTipoConselhoRegional = " ";
            if (Credenciamento.Proposta.CodigoMotivoRecusa == null)
                Credenciamento.Proposta.CodigoMotivoRecusa = 0;
            if (Credenciamento.Proposta.NumeroInvestigacaoPropostaDigitada == null)
                Credenciamento.Proposta.NumeroInvestigacaoPropostaDigitada = 0;
            if (Credenciamento.Proposta.NumeroPontoDeVenda == null)
                Credenciamento.Proposta.NumeroPontoDeVenda = 0;

            //Endereco
            if (Credenciamento.Enderecos.Count() == 0)
                if (Credenciamento.Proposta.CodigoTipoPessoa.Equals('J'))
                    BuscaCEP(txtCepPessoaJuridica, lblEnderecoPessoaJuridica);
                else
                    EventoBuscaCEP(txtCEPPessoaFisica, lblEnderecoPessoaFisica);

            Modelo.Endereco endereco = Credenciamento.Enderecos.FirstOrDefault();

            List<Modelo.Proprietario> proprietarios = new List<Modelo.Proprietario>();

            if (mvTipoPessoa.ActiveViewIndex == 0)
                foreach (GridViewRow row in gvProprietario.Rows)
                {
                    proprietarios.Add(new Modelo.Proprietario
                    {
                        //Aplicar regra
                        TipoAcaoProprietario = !String.IsNullOrEmpty(((HiddenField)row.FindControl("hfTipoAcaoProprietario")).Value) ? (TipoAcaoProprietario)((HiddenField)row.FindControl("hfTipoAcaoProprietario")).Value.ElementAt(0) : TipoAcaoProprietario.Alterar,
                        CodigoTipoPessoa = (TipoPessoa)rbTipoPessoa.SelectedValue.ToString().ElementAt(0),
                        NumeroCNPJ = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                        NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta,
                        CodigoTipoPesssoaProprietario = ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Text.RemoverCaracteresEspeciais().Length > 11 ? Modelo.TipoPessoa.Juridica : Modelo.TipoPessoa.Fisica,
                        NumeroCNPJCPFProprietario = ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                        NomeProprietario = ((TextBox)row.FindControl("txtGridViewNomePropietario")).Text.ToUpper().Trim(),
                        ParticipacaoAcionaria = ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Text.ToDouble(),
                        UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                        DataNascimentoProprietario = DateTime.Now
                    });
                }

            if (mvTipoPessoa.ActiveViewIndex == 1)
                proprietarios.Add(new Modelo.Proprietario
                {
                    TipoAcaoProprietario = TipoAcaoProprietario.Incluir,
                    CodigoTipoPessoa = (TipoPessoa)rbTipoPessoa.SelectedValue.ToString().ElementAt(0),
                    NumeroCNPJ = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                    NumeroSequenciaProposta = Credenciamento.Proposta.IndicadorSequenciaProposta,
                    CodigoTipoPesssoaProprietario = Modelo.TipoPessoa.Fisica,
                    NumeroCNPJCPFProprietario = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                    NomeProprietario = txtNomeClientePessoaFisica.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToUpper(),
                    ParticipacaoAcionaria = 100,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataNascimentoProprietario = DateTime.Now
                });

            Credenciamento.Proprietarios = proprietarios;
        }

        /// <summary>
        /// Método responsável em salvar dados da tela utilizando o método salvar dados iniciais
        /// </summary>
        public void SalvarDadosIniciais()
        {
            CapturarInformacoesPreenchidas();

            Credenciamento.Proposta.IndicadorSequenciaProposta = ServicosWF.SalvarDadosIniciais(PreencheProposta(), Credenciamento.Enderecos.FirstOrDefault(e => e.IndicadorTipoEndereco.Equals('1')), Credenciamento.Proprietarios, Credenciamento.DomiciliosBancarios);
        }

        public event EventHandler Continuar;

        /// <summary>
        /// Evento disparado ao mudar o tipo de pessoa.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rbTipoPessoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            mvTipoPessoa.ActiveViewIndex = rbTipoPessoa.SelectedIndex;
            upDadosIniciais.Update();
        }

        /// <summary>
        /// Evento disparado ao mudar o canal
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlCanal_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CarregarListaCelulas();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o ramo de atuação
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ddlRamoAtuacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CarregarRamosAtividade();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP Pessoa Juridica
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCepPessoaJuridica_TextChanged(object sender, EventArgs e)
        {
            EventoBuscaCEP(txtCepPessoaJuridica, lblEnderecoPessoaJuridica);
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP Pessoa Fisica
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCEPPessoaFisica_TextChanged(object sender, EventArgs e)
        {
            EventoBuscaCEP(txtCEPPessoaFisica, lblEnderecoPessoaFisica);
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        private void EventoBuscaCEP(TextBox txtCep, Label lblEndereco)
        {
            //if (Request.Form.Get("__EVENTTARGET") == txtCep.UniqueID)
            //{
            BuscaCEP(txtCep, lblEndereco);
            //}
        }

        /// <summary>
        /// Busca CEP no Serviço
        /// </summary>
        /// <param name="txtCep">textbox CEP</param>
        /// <param name="lblEndereco">label Endereço</param>
        private void BuscaCEP(TextBox txtCep, Label lblEndereco)
        {
            if (!String.IsNullOrEmpty(txtCep.Text) && txtCep.Text.Length == 9)
            {
                String cep = txtCep.Text.Replace("-", "");
                String endereco = String.Empty;
                String bairro = String.Empty;
                String cidade = String.Empty;
                String uf = String.Empty;
                String cepAnterior = String.Empty;

                Int32 codigoRetorno = 0;

                try
                {
                    codigoRetorno = ServicosDR.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
                }
                catch (FaultException<DRCepServico.GeneralFault> fe)
                {
                    Logger.GravarErro("Credenciamento", fe);
                    SharePointUlsLog.LogErro(fe);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                }

                Endereco objEndereco = new Endereco();

                if (Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == '1').Count() > 0)
                {
                    objEndereco = Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == '1').FirstOrDefault();

                    if (objEndereco != null)
                        cepAnterior = String.Format("{0}{1}", objEndereco.CodigoCep, objEndereco.CodigoComplementoCep);

                    Credenciamento.Enderecos.Remove(Credenciamento.Enderecos.Where(i => i.IndicadorTipoEndereco == '1').FirstOrDefault());
                }

                objEndereco.IndicadorTipoEndereco = '1';
                objEndereco.CodigoTipoPessoa = (Modelo.TipoPessoa)rbTipoPessoa.SelectedValue.ElementAt(0);
                objEndereco.NumeroCNPJ = this.NumeroCNPJ;
                objEndereco.Logradouro = endereco;
                objEndereco.Bairro = bairro;
                objEndereco.Cidade = cidade;
                objEndereco.Estado = uf;
                objEndereco.CodigoCep = TextBoxCEP.Text.Split('-')[0];
                objEndereco.CodigoComplementoCep = TextBoxCEP.Text.Split('-')[1];
                objEndereco.DataHoraUltimaAtualizacao = DateTime.Now;
                objEndereco.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;

                if (!cep.Equals(cepAnterior))
                {
                    objEndereco.ComplementoEndereco = String.Empty;
                    objEndereco.NumeroEndereco = String.Empty;
                }


                if (objEndereco.IndicadorTipoEndereco == 0 && objEndereco.IndicadorTipoEndereco == ' ')
                    objEndereco.IndicadorTipoEndereco = '1';

                if (objEndereco.IndicadorLocalizadoEmShopping == 0 && objEndereco.IndicadorLocalizadoEmShopping == ' ')
                    objEndereco.IndicadorLocalizadoEmShopping = ' ';

                Credenciamento.Enderecos.Add(objEndereco);

                if (codigoRetorno == 38)
                    lblEndereco.Text = String.Format("{0}, {1}, {2}, {3}", endereco, bairro, cidade, uf);
                else if (codigoRetorno == 1)
                    lblEndereco.Text = String.Format("{0}, {1}", cidade, uf);
                else
                    lblEndereco.Text = String.Empty;
            }
            else
                lblEndereco.Text = String.Empty;
        }


        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCNPJ_ServerValidate(object source, ServerValidateEventArgs args)
        {

            if (txtCNPJCPFPessoaJuridica.Text.Length == 18)
                args.IsValid = txtCNPJCPFPessoaJuridica.Text.Replace(".", "").Replace("-", "").Replace("/", "").IsValidCNPJ();
            else
                args.IsValid = false;
        }

        /// <summary>
        /// Valida CNPJ ou CPF
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCNPJCPFProprietario_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (!String.IsNullOrEmpty(args.Value))
            {
                String cpfCnpj = args.Value.Replace(".", "").Replace("-", "").Replace("/", "");

                if (cpfCnpj.Length == 11)
                    args.IsValid = cpfCnpj.IsValidCPF();
                else if (cpfCnpj.Length == 14)
                    args.IsValid = cpfCnpj.IsValidCNPJ();

                Int32 quantidadeOcorrenciasMesmoCnpj = 0;

                foreach (GridViewRow row in gvProprietario.Rows)
                {
                    var cnpjProprietario = ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Text;
                    if (args.Value == cnpjProprietario)
                        quantidadeOcorrenciasMesmoCnpj++;
                }

                if (quantidadeOcorrenciasMesmoCnpj > 1)
                    args.IsValid = false;
            }
        }



        /// <summary>
        /// Habilita, ou desabilita os campos de edição dos dados do CNPJ.
        /// </summary>
        /// <param name="tipoEstabelecimento">Tipo de Estabelecimento</param>
        /// <param name="retornoSerasa">Retorno do Serasa</param>
        private void PermitirEdicaoDadosCNPJ(TipoEstabelecimento? tipoEstabelecimento, bool retornoSerasa)
        {
            if (retornoSerasa)
            {
                //Retorno Serasa não pode modificar informação sobre Empresa //Agora pode modificar atuãção e atividade do retorno serasa
                ddlRamoAtuacaoPessoaJuridica.Enabled = true;
                ddlRamoAtividadePessoaJuridica.Enabled = true;
                txtRazaoSocialPessoaJuridica.Enabled = false;
                //txtDataFundacaoPessoaJuridica.Enabled = false;
                txtCNAEPessoaJuridica.Enabled = true;

                //Controle JQuery de Data
                //txtDataFundacaoPessoaJuridica.CssClass = txtDataFundacaoPessoaJuridica.CssClass.Replace("dtPicker", String.Empty);
                //txtDataFundacaoPessoaJuridica.Attributes.CssStyle.Add("width", "120px");
            }
            else
            {
                if (tipoEstabelecimento != null)
                    if (tipoEstabelecimento == TipoEstabelecimento.Matriz)
                    {
                        //Empresa Matriz pode modificar dados da empresa
                        ddlRamoAtuacaoPessoaJuridica.Enabled = true;
                        ddlRamoAtividadePessoaJuridica.Enabled = true;
                        txtRazaoSocialPessoaJuridica.Enabled = true;
                        txtDataFundacaoPessoaJuridica.Enabled = true;
                        txtCNAEPessoaJuridica.Enabled = true;

                        //Controle JQuery de Data
                        //txtDataFundacaoPessoaJuridica.CssClass = String.Format("{0} {1}", txtDataFundacaoPessoaJuridica.CssClass, "dtPicker");
                    }
                    else
                    {
                        //Empresa Filial pode modificar alguns dados da Empresa
                        ddlRamoAtuacaoPessoaJuridica.Enabled = true;
                        ddlRamoAtividadePessoaJuridica.Enabled = true;
                        txtRazaoSocialPessoaJuridica.Enabled = false;
                        //txtDataFundacaoPessoaJuridica.Enabled = false;
                        txtCNAEPessoaJuridica.Enabled = true;

                        //Controle JQuery de Data
                        //txtDataFundacaoPessoaJuridica.CssClass = txtDataFundacaoPessoaJuridica.CssClass.Replace("dtPicker", String.Empty);
                        //txtDataFundacaoPessoaJuridica.Attributes.CssStyle.Add("width", "120px");
                    }
            }
        }

        /// <summary>
        /// Exibe ou oculta o place holder de dados de CNPJ.
        /// </summary>
        /// <param name="visible">Manter visível</param>
        private void ExibirDadosCNPJ(bool visible)
        {
            phDadosCNPJ.Visible = visible;
        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCPF_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtCPFPessoaFisica.Text.Length == 14)
                args.IsValid = txtCPFPessoaFisica.Text.Replace(".", "").Replace("-", "").IsValidCPF();
            else
                args.IsValid = false;
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do textbox de CPF.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCPFPessoaFisica_TextChanged(object sender, EventArgs e)
        {
            Boolean existemPropostasPendentesOuPvAtivo = ServicosWF.PropostasPendentes(((Char)((int)TipoPessoa.Fisica)), NumeroCPF);
            Boolean existePvCancelado = ServicosWF.RecuperaQuantidadePvs(((Char)((int)TipoPessoa.Fisica)), NumeroCPF);

            if (existemPropostasPendentesOuPvAtivo || existePvCancelado)
            {
                CapturarInformacoesPreenchidas();
                Credenciamento.Proposta.NumeroCnpjCpf = NumeroCPF;
                ContinuarRecuperarProposta(sender, e);
            }
            else
            {
                //Limpar dados do CNPJ antigo
                Credenciamento = new Modelo.Credenciamento();

                txtNomeClientePessoaFisica.Text = string.Empty;
                txtDDDPessoaFisica.Text = String.Empty;
                txtTelefonePessoaFisica.Text = String.Empty;
                txtCEPPessoaFisica.Text = String.Empty;
                txtDataNascPessoaFisica.Text = String.Empty;
                ddlRamoAtividadePessoaFisica.SelectedIndex = 0;
                InicializarObjeto();
            }
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do textbox de CNPJ.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void txtCNPJCPFPessoaJuridica_TextChanged(object sender, EventArgs e)
        {

            if (txtCNPJCPFPessoaJuridica.Text.Replace(".", "").Replace("-", "").Replace("/", "").IsValidCNPJ())
            {
                try
                {
                    ExibirDadosCNPJ(txtCNPJCPFPessoaJuridica.Text.Length >= 18);

                    if (mvTipoPessoa.ActiveViewIndex == 0 && txtCNPJCPFPessoaJuridica.Text.Length >= 18)
                    {
                        ////Limpar dados do CNPJ antigo
                        //Manter valor do CEP se existir
                        Credenciamento = new Modelo.Credenciamento();
                        InicializarObjeto();
                        LimparCamposFormularioPessoaJuridica();
                        txtNomeClientePessoaJuridica.Text = string.Empty;
                        txtDDDPessoaJuridica.Text = String.Empty;
                        txtTelefonePessoaJuridica.Text = String.Empty;
                        txtRamalPessoaJuridica.Text = String.Empty;
                        txtCepPessoaJuridica.Text = String.Empty;

                        //Validação extra para evitar continuar recadastramento com retorno de clientes sem endereço (não exibe nada na tela)
                        var propostas = new List<Modelo.PropostaPendente>();
                        Credenciamento.Proposta.CodigoTipoPessoa = rbTipoPessoa.SelectedValue.ElementAt(0);
                        Credenciamento.Proposta.NumeroCnpjCpf = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64();

                        List<Modelo.PropostaPendente> propostasIncompletas = Servicos.ServicosWF.RecuperarPropostasIncompletas(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf, null);
                        List<Modelo.PropostaPendente> propostasCanceladasOuAtivas = Servicos.ServicosGE.RecuperarPropostasCanceladasOuAtivas(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf);
                        propostas.AddRange(propostasIncompletas);
                        propostas.AddRange(propostasCanceladasOuAtivas);
                        //Fim da validação extra

                        Boolean existemPropostasPendentesOuPvAtivo = ServicosWF.PropostasPendentes(((Char)((int)TipoPessoa.Juridica)), NumeroCNPJ);
                        Boolean existePvCancelado = ServicosWF.RecuperaQuantidadePvs(((Char)((int)TipoPessoa.Juridica)), NumeroCNPJ);

                        if ((existemPropostasPendentesOuPvAtivo || existePvCancelado) && propostas.Count != 0)
                        {
                            //goto tela recuperacao propostas
                            CapturarInformacoesPreenchidas();
                            Credenciamento.Proposta.NumeroCnpjCpf = txtCNPJCPFPessoaJuridica.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64();
                            ContinuarRecuperarProposta(sender, e);
                        }
                        else
                        {
                            GEPontoVen.PontoVendaConsultaTipoEstabCredenciamento tipoEstabelecimento = ServicosGE.ConsultaTipoEstabelecimento(NumeroCNPJ, null);
                            Credenciamento.Proposta.CodigoTipoEstabelecimento = tipoEstabelecimento.CodTipoEstabelecimento;

                            if (tipoEstabelecimento.CodTipoEstabelecimento == null && rbTipoPessoa.SelectedValue.Equals("J"))
                                Credenciamento.Proposta.CodigoTipoEstabelecimento = 2;
                            else
                                Credenciamento.Proposta.CodigoTipoEstabelecimento = 1;

                            Credenciamento.Proposta.NumeroMatriz = tipoEstabelecimento.NumPdvMatriz;




                            if (tipoEstabelecimento.CodTipoEstabelecimento != null && tipoEstabelecimento.CodTipoEstabelecimento == 1)
                            {
                                //Controles de tela
                                CarregarRamosAtuacao();

                                if (tipoEstabelecimento.NumPdvMatriz.HasValue)
                                {
                                    //Carregar Dados Matriz é chamado pois a empresa é uma filial da Matriz e buscará os dados da Matriz
                                    CarregarDadosMatriz(tipoEstabelecimento.NumPdvMatriz.Value, tipoEstabelecimento.NumCNPJ.Value);
                                }

                            }
                            else
                            {
                                //A empresa é uma Matriz e verifica neste método se possui dados cadastrados no Serasa, carregando conforme a resposta.
                                CarregarDadosSerasa();
                            }
                        }
                    }
                    else
                    {
                        LimparCamposFormularioPessoaJuridica();
                        ExibirDadosCNPJ(false);
                    }


                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }


        /// <summary>
        /// Metodo que limpa os campos do formulario de pessoa juridica
        /// </summary>
        private void LimparCamposFormularioPessoaJuridica()
        {
            txtRazaoSocialPessoaJuridica.Text = String.Empty;

            if (ddlRamoAtuacaoPessoaJuridica.Items.Count > 0)
                ddlRamoAtuacaoPessoaJuridica.SelectedIndex = 0;

            if (ddlRamoAtividadePessoaJuridica.Items.Count > 0)
                ddlRamoAtividadePessoaJuridica.SelectedIndex = 0;

            txtDataFundacaoPessoaJuridica.Text = String.Empty;
            txtCNAEPessoaJuridica.Text = String.Empty;

            gvProprietario.DataSource = null;
            gvProprietario.DataBind();
        }

        /// <summary>
        /// DataBind do GridView de Proprietários
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void gvProprietario_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Proprietario objPropretario = (Modelo.Proprietario)e.Row.DataItem;
                string cpfCnpj = objPropretario.NumeroCNPJCPFProprietario.ToString();
                string nomePropietario = objPropretario.NomeProprietario;
                string parteAcionaria = objPropretario.ParticipacaoAcionaria.ToString("##0.00");
                TipoAcaoProprietario tipoAcaoProprietario = objPropretario.TipoAcaoProprietario == TipoAcaoProprietario.Desconhecido ? TipoAcaoProprietario.Alterar : objPropretario.TipoAcaoProprietario;

                if (String.Compare(cpfCnpj, "0") == 0)
                    cpfCnpj = String.Empty;

                if (objPropretario.DadosRetornadosSerasa != null)
                    if (objPropretario.DadosRetornadosSerasa.Value)
                        //Controle JQuery de Data
                        ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-participacao", "mascara-decimal-participacao");
                    else
                        ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-decimal-participacao", "mascara-participacao");
                else
                    ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-decimal-participacao", "mascara-participacao");

                //Casas decimais para retorno de matriz
                if (Credenciamento.Proposta.CodigoTipoEstabelecimento == 1 && Credenciamento.Proposta.NumeroMatriz.HasValue)
                    ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-participacao", "mascara-decimal-participacao");
                else
                    ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass = ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).CssClass.Replace("mascara-decimal-participacao", "mascara-participacao");

                //Alimentar controles do repeater com dados
                ((TextBox)e.Row.FindControl("txtGridViewCNPJCPF")).Text = cpfCnpj;
                if (nomePropietario != null)
                    ((TextBox)e.Row.FindControl("txtGridViewNomePropietario")).Text = nomePropietario.Trim();
                ((TextBox)e.Row.FindControl("txtGridViewPartAcionaria")).Text = parteAcionaria;
                ((HiddenField)e.Row.FindControl("hfTipoAcaoProprietario")).Value = ((char)tipoAcaoProprietario).ToString();
                ((ImageButton)e.Row.FindControl("ibGridViewRemover")).CommandArgument = objPropretario.NumeroCNPJCPFProprietario.ToString();
                if (objPropretario.CodigoTipoPesssoaProprietario != TipoPessoa.Desconhecido)
                    ((HiddenField)e.Row.FindControl("hfTipoPessoaProprietario")).Value = ((Char)objPropretario.CodigoTipoPesssoaProprietario).ToString();
                ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(((ImageButton)e.Row.FindControl("ibGridViewRemover")));


                if (tipoAcaoProprietario == TipoAcaoProprietario.Excluir)
                    e.Row.Visible = false;

            }
        }

        /// <summary>
        /// Evento do clique no botão excluir dentro do grid de proprietarios.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Grid Proprietarios - Excluir"))
            {
                try
                {
                    string cnpj = ((ImageButton)sender).CommandArgument;
                    ImageButton btn = (ImageButton)sender;
                    GridViewRow gvRow = (GridViewRow)btn.NamingContainer;

                    Credenciamento.Proprietarios = RetornaDadosGridProprietario();

                    //Se proprietário for do banco de dados Da Matriz ou do Serasa (marcar para excluir do BD WF)
                    Proprietario proprietario = Credenciamento.Proprietarios.Where(i => i.NumeroCNPJCPFProprietario == cnpj.ToInt64()).FirstOrDefault();

                    if (proprietario != null)
                    {
                        //Seta como excluisão do Banco de Dados (qnd retorna da Matriz ou do Serasa)
                        Credenciamento.Proprietarios.Remove(Credenciamento.Proprietarios.Where(i => i.NumeroCNPJCPFProprietario == cnpj.ToInt64()).FirstOrDefault());
                        if (proprietario.TipoAcaoProprietario == TipoAcaoProprietario.Alterar)
                        {
                            proprietario.TipoAcaoProprietario = TipoAcaoProprietario.Excluir;
                            Credenciamento.Proprietarios.Add(proprietario);
                        }
                    }

                    PopularGridProprietarios();
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        /// <summary>
        /// Evento do clique no botão incluir novo proprietario
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            try
            {
                //manter conteúdo do grid
                Credenciamento.Proprietarios = RetornaDadosGridProprietario();

                Credenciamento.Proprietarios.Add(new Modelo.Proprietario
                {
                    TipoAcaoProprietario = TipoAcaoProprietario.Incluir
                });

                PopularGridProprietarios();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Preenche List de proprietários com base na grid preenchida de proprietários
        /// </summary>
        /// <returns>retorna lista de proprietários</returns>
        private List<Proprietario> RetornaDadosGridProprietario()
        {
            List<Proprietario> objProprietarios = new List<Proprietario>();

            foreach (GridViewRow row in gvProprietario.Rows)
            {

                objProprietarios.Add(new Modelo.Proprietario
                {
                    //Aplicar regra
                    TipoAcaoProprietario = !String.IsNullOrEmpty(((HiddenField)row.FindControl("hfTipoAcaoProprietario")).Value.ToString()) ? (TipoAcaoProprietario)((HiddenField)row.FindControl("hfTipoAcaoProprietario")).Value.ToString().ElementAt(0) : TipoAcaoProprietario.Alterar,
                    CodigoTipoPessoa = (TipoPessoa)rbTipoPessoa.SelectedValue.ToString().ElementAt(0),
                    NumeroCNPJ = TextBoxCpfCnpj.Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                    NumeroSequenciaProposta = 0,
                    CodigoTipoPesssoaProprietario = ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Text.Length > 14 ? Modelo.TipoPessoa.Juridica : Modelo.TipoPessoa.Fisica,
                    NumeroCNPJCPFProprietario = ((TextBox)row.FindControl("txtGridViewCNPJCPF")).Text.RemoverAcentos().RemoverCaracteresEspeciais().ToInt64(),
                    NomeProprietario = ((TextBox)row.FindControl("txtGridViewNomePropietario")).Text.Trim(),
                    ParticipacaoAcionaria = ((TextBox)row.FindControl("txtGridViewPartAcionaria")).Text.ToDouble(),
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataNascimentoProprietario = DateTime.Now
                });
            }

            return objProprietarios;
        }

        /// <summary>
        /// Alimenta Grid dos Proprietários, executa o bind e controla visibilidade/Edição
        /// </summary>
        private void PopularGridProprietarios()
        {
            foreach (Proprietario objProprietario in Credenciamento.Proprietarios)
                if (objProprietario.TipoAcaoProprietario == TipoAcaoProprietario.Desconhecido)
                    objProprietario.TipoAcaoProprietario = TipoAcaoProprietario.Alterar;

            //Alimenta com dados
            gvProprietario.DataSource = Credenciamento.Proprietarios;
            gvProprietario.DataBind();

            //Controle de visibilidade e edição dos dados
            AplicarVisibilidadeEdicaoProprietarios();

            upDadosIniciais.Update();
        }

        /// <summary>
        /// Retorna Objeto Proposta com base nos dados preenchidos no Credenciamento.Proposta normalizando formatação dos dados
        /// </summary>
        /// <returns>retorna proposta</returns>
        protected Modelo.Proposta PreencheProposta()
        {
            Proposta proposta = new Proposta();

            proposta.CodigoTipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa;

            Int64 numCnpjCpf = 0;

            if (Credenciamento.Proposta.CodigoTipoPessoa == 'J')
            {
                numCnpjCpf = Credenciamento.Proposta.NumeroCnpjCpf;
                proposta.RazaoSocial = !String.IsNullOrEmpty(Credenciamento.Proposta.RazaoSocial) ? Credenciamento.Proposta.RazaoSocial.ToUpper() : String.Empty;
            }
            else
            {
                numCnpjCpf = Credenciamento.Proposta.NumeroCnpjCpf;
                proposta.RazaoSocial = !String.IsNullOrEmpty(Credenciamento.Proposta.RazaoSocial) ? Credenciamento.Proposta.RazaoSocial.ToUpper() : String.Empty;
            }

            proposta.DataFundacao = Credenciamento.Proposta.DataFundacao = Credenciamento.Proposta.DataFundacao != null && Credenciamento.Proposta.DataFundacao != DateTime.MinValue ? Credenciamento.Proposta.DataFundacao : new DateTime(1900, 1, 1);
            proposta.NumeroCnpjCpf = numCnpjCpf;
            proposta.IndicadorSequenciaProposta = (Int32)Credenciamento.Proposta.IndicadorSequenciaProposta;
            proposta.UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
            proposta.UsuarioInclusao = SessaoAtual.LoginUsuario;
            proposta.CodigoCanal = Credenciamento.Proposta.CodigoCanal;
            proposta.CodigoMotivoRecusa = 0;
            proposta.NumeroPontoDeVenda = Credenciamento.Proposta.NumeroPontoDeVenda != null ? Credenciamento.Proposta.NumeroPontoDeVenda : 0;
            proposta.CodigoTipoMovimento = Credenciamento.Proposta.CodigoTipoMovimento;
            proposta.CodigoGrupoRamo = Credenciamento.Proposta.CodigoGrupoRamo;
            proposta.CodigoRamoAtividade = Credenciamento.Proposta.CodigoRamoAtividade;

            if (proposta.IndicadorEnderecoIgualComercial == null)
                proposta.IndicadorEnderecoIgualComercial = 'S';

            proposta.PessoaContato = !String.IsNullOrEmpty(Credenciamento.Proposta.PessoaContato) ? Credenciamento.Proposta.PessoaContato.ToUpper() : String.Empty;
            proposta.IndicadorAcessoInternet = 'N';
            proposta.NomeEmail = !String.IsNullOrEmpty(Credenciamento.Proposta.NomeEmail) ? Credenciamento.Proposta.NomeEmail : String.Empty;
            proposta.NomeHomePage = !String.IsNullOrEmpty(Credenciamento.Proposta.NomeHomePage) ? Credenciamento.Proposta.NomeHomePage : String.Empty;
            proposta.NumeroDDD1 = !String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD1) ? Credenciamento.Proposta.NumeroDDD1 : String.Empty;
            proposta.NumeroTelefone1 = Credenciamento.Proposta.NumeroTelefone1 != null ? Credenciamento.Proposta.NumeroTelefone1 : 0;
            proposta.Ramal1 = Credenciamento.Proposta.Ramal1 != null ? Credenciamento.Proposta.Ramal1 : 0;
            proposta.NumeroDDDFax = !String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDDFax) ? Credenciamento.Proposta.NumeroDDDFax : String.Empty;
            proposta.NumeroTelefoneFax = Credenciamento.Proposta.NumeroTelefoneFax != null ? Credenciamento.Proposta.NumeroTelefoneFax : 0;
            proposta.NumeroDDD2 = !String.IsNullOrEmpty(Credenciamento.Proposta.NumeroDDD2) ? Credenciamento.Proposta.NumeroDDD2 : String.Empty;
            proposta.NumeroTelefone2 = Credenciamento.Proposta.NumeroTelefone2 != null ? Credenciamento.Proposta.NumeroTelefone2 : 0;
            proposta.Ramal2 = Credenciamento.Proposta.Ramal2 != null ? Credenciamento.Proposta.Ramal2 : 0;
            proposta.IndicadorRegiaoLoja = Credenciamento.Proposta.IndicadorRegiaoLoja;
            proposta.NomePlaqueta1 = String.Empty;
            proposta.NomePlaqueta2 = String.Empty;
            proposta.CodigoFilial = 0;
            proposta.CodigoGerencia = Credenciamento.Proposta.CodigoGerencia != null ? Credenciamento.Proposta.CodigoGerencia : ' ';
            proposta.CodigoCarteira = Credenciamento.Proposta.CodigoCarteira != null ? Credenciamento.Proposta.CodigoCarteira : 0;
            proposta.CodigoZona = 0;
            proposta.CodigoNucleo = 0;
            proposta.CodigoHoraFuncionamentoPV = Credenciamento.Proposta.CodigoHoraFuncionamentoPV != null ? Credenciamento.Proposta.CodigoHoraFuncionamentoPV : 0;
            proposta.IndicadorMaquineta = 'N';
            proposta.QuantidadeMaquineta = 0;
            proposta.IndicadorIATA = ' ';
            if (Credenciamento.Proposta.CodigoTipoPessoa == 'F')
                proposta.CodigoTipoEstabelecimento = 0;
            else if (Credenciamento.Proposta.CodigoTipoEstabelecimento != null)
                proposta.CodigoTipoEstabelecimento = Credenciamento.Proposta.CodigoTipoEstabelecimento;
            else
                proposta.CodigoTipoEstabelecimento = 2;

            proposta.IndicadorComercializacaoNormal = (Credenciamento.Proposta.IndicadorComercializacaoNormal != null && Credenciamento.Proposta.IndicadorComercializacaoNormal == 'S') ? 'N' : 'S';
            proposta.IndicadorComercializacaoCatalogo = (Credenciamento.Proposta.IndicadorComercializacaoCatalogo != null && Credenciamento.Proposta.IndicadorComercializacaoCatalogo == 'S') ? 'S' : 'N';
            proposta.IndicadorComercializacaoTelefone = (Credenciamento.Proposta.IndicadorComercializacaoTelefone != null && Credenciamento.Proposta.IndicadorComercializacaoTelefone == 'S') ? 'S' : 'N';
            proposta.IndicadorComercializacaoEletronico = (Credenciamento.Proposta.IndicadorComercializacaoEletronico != null && Credenciamento.Proposta.IndicadorComercializacaoEletronico == 'S') ? 'S' : 'N';
            proposta.NumeroMatriz = Credenciamento.Proposta.NumeroMatriz != null ? Credenciamento.Proposta.NumeroMatriz : 0;
            proposta.NomeFatura = !String.IsNullOrEmpty(Credenciamento.Proposta.NomeFatura) ? Credenciamento.Proposta.NomeFatura.ToUpper() : String.Empty;
            proposta.CodigoTipoConsignacao = null;
            proposta.NumeroConsignador = 0;
            proposta.NumeroGrupoComercial = Credenciamento.Proposta.NumeroGrupoComercial != null ? Credenciamento.Proposta.NumeroGrupoComercial : 0;
            proposta.NumeroGrupoGerencial = Credenciamento.Proposta.NumeroGrupoGerencial != null ? Credenciamento.Proposta.NumeroGrupoGerencial : 0;
            proposta.CodigoLocalPagamento = Credenciamento.Proposta.CodigoLocalPagamento != null ? Credenciamento.Proposta.CodigoLocalPagamento : 0;
            proposta.NumeroCentralizadora = Credenciamento.Proposta.NumeroCentralizadora != null ? Credenciamento.Proposta.NumeroCentralizadora : 0;
            proposta.IndicadorSolicitacaoTecnologia = 'N';
            proposta.NomeProprietario1 = null;
            proposta.NumeroCPFProprietario1 = null;
            proposta.TipoPessoaProprietario1 = null;
            proposta.NomeProprietario2 = null;
            proposta.DataNascProprietario2 = null;
            proposta.NumeroCPFProprietario2 = null;
            proposta.TipoPessoaProprietario2 = null;
            proposta.CodigoCelula = Credenciamento.Proposta.CodigoCelula;
            proposta.CodigoRoteiro = null;
            proposta.CodigoAgenciaFiliacao = 0;
            proposta.CodigoTerceirizacaoVista = 0;
            proposta.DataCadastroProposta = DateTime.MinValue != Credenciamento.Proposta.DataCadastroProposta ? Credenciamento.Proposta.DataCadastroProposta : new DateTime(2000, 1, 1);
            proposta.NumeroCPFVendedor = Credenciamento.Proposta.NumeroCPFVendedor != null ? Credenciamento.Proposta.NumeroCPFVendedor : 0;
            proposta.CodigoFaseFiliacao = Credenciamento.Proposta.CodigoFaseFiliacao != null ? Credenciamento.Proposta.CodigoFaseFiliacao : 1;
            proposta.CodigoImpressoraFiscal = 0;
            proposta.IndicadorFinanceira = 'N';
            proposta.CodigoFinanceira1 = 0;
            proposta.CodigoFinanceira2 = 0;
            proposta.CodigoFinanceira3 = 0;
            proposta.SituacaoProposta = 'C';
            proposta.CodigoPesoTarget = 0;
            proposta.CodigoPeriodicidadeRAV = ' ';
            proposta.CodigoPeriodicidadeDia = 0;
            proposta.ValorTaxaAdesao = Credenciamento.Proposta.ValorTaxaAdesao != null ? Credenciamento.Proposta.ValorTaxaAdesao : 0;
            proposta.IndicadorEnvioForcaVenda = "N";
            proposta.NumeroInvestigacaoPropostaDigitada = null;
            proposta.IndicadorOrigemProposta = "Portal";
            proposta.CodigoCampanha = Credenciamento.Proposta.CodigoCampanha;
            proposta.CodModeloProposta = null;
            proposta.CodigoEVD = Credenciamento.Proposta.CodigoEVD;
            proposta.IndicadorProntaInstalacao = null;
            proposta.IndicadorEnvioExtratoEmail = Credenciamento.Proposta.IndicadorEnvioExtratoEmail != null ? Credenciamento.Proposta.IndicadorEnvioExtratoEmail : 'N';
            proposta.IndicadorControle = "PI";
            proposta.CodigoTipoConselhoRegional = null;
            proposta.NumeroConselhoRegional = null;
            proposta.UfConselhoRegional = null;
            proposta.CodigoCNAE = !String.IsNullOrEmpty(Credenciamento.Proposta.CodigoCNAE) ? Credenciamento.Proposta.CodigoCNAE : String.Empty;
            proposta.CodigoMotivoRecusa = 0;
            proposta.NumeroPontoDeVenda = Credenciamento.Proposta.NumeroPontoDeVenda.HasValue ? Credenciamento.Proposta.NumeroPontoDeVenda : 0;

            return proposta;
        }

        public event EventHandler ContinuarRecuperarProposta;

        /// <summary>
        /// Valida Telefone pessoa jurídica
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvTelefoneJuridica_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ServicosGE.ValidaTelefone(txtTelefonePessoaJuridica.Text);
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Valida Telefone pessoa física
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvTelefoneFisica_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ServicosGE.ValidaTelefone(txtTelefonePessoaFisica.Text);
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Valida campos data
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = ServicosGE.ValidaData(args.Value);
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Valida CEP
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCep_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                if (args.Value.Length == 9)
                    args.IsValid = true;
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }
    }
}
