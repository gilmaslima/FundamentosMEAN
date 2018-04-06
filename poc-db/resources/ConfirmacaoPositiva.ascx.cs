#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [31/05/2012] – [André Garcia] – [Criação]
*/
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Colocar duas páginas na Confirmação Positiva
- [31/05/2012] – [André Garcia] – [Alteração]
*/
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Redecard.PN.Comum;
using System.ServiceModel;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    public partial class ConfirmacaoPositiva : UserControlBase
    {

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class DadosConfirmacaoPositiva
        {
            [XmlAttribute]
            public Decimal NumeroEstabelecimento { get; set; }

            [XmlAttribute]
            public Int32 BancoCredito { get; set; }

            [XmlAttribute]
            public string Agencia { get; set; }

            [XmlAttribute]
            public string CCCredito { get; set; }

            [XmlAttribute]
            public Decimal CPF { get; set; }

            [XmlAttribute]
            public Boolean Empresa { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void EnviarClickHandle(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void VoltarClickHandle(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProximaPaginaClickHandle(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event EnviarClickHandle EnviarClick;

        /// <summary>
        /// 
        /// </summary>
        public event VoltarClickHandle VoltarClick;

        /// <summary>
        /// 
        /// </summary>
        public event ProximaPaginaClickHandle ProximaPaginaClick;

        /// <summary>
        /// 
        /// </summary>
        private const String _dados_key = "__key__dados__ViewState";

        /// <summary>
        /// 
        /// </summary>
        private DadosConfirmacaoPositiva _dados = null;


        /// <summary>
        /// Carregamento da página, preenche listagem de bancos
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregando página, preenchendo listagem de bancos"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        this.ConsultarBancos();
                        if (InformacaoUsuario.Existe())
                        {
                            InformacaoUsuario dados = InformacaoUsuario.Recuperar();

                            using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                            {
                                Int32 codigo = 0;
                                UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                                {
                                    GrupoEntidade = new UsuarioServico.GrupoEntidade()
                                    {
                                        Codigo = dados.GrupoEntidade
                                    },
                                    Codigo = dados.NumeroPV
                                };
                                var usuarios = client.ConsultarPorCodigoEntidade(out codigo, dados.Usuario, entidade);
                                if (codigo > 0 && usuarios.Length < 1)
                                    base.ExibirPainelExcecao("UsuarioServico.Consultar", codigo);
                                else
                                {
                                    UsuarioServico.Usuario usuario = usuarios[0];
                                    if (usuario.BloqueadoConfirmacaoPositiva)
                                        ExibirTelaBloqueio();
                                    else
                                    {
                                        if (dados.CpfCnpjEstabelecimento > 0)
                                        {
                                            // Deve ser exibida somente a segunda tela de
                                            // confirmação positiva. Isso porque o CPF/CNPJ
                                            // já foi preenchido/validado em um passo anterior
                                            this.ExibirPagina2();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), String.Empty);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, String.Empty);
                }
            }
        }

        /// <summary>
        /// Voltar para o inicio do processo
        /// </summary>
        protected void VoltarHome(Object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Voltar para o inicio do processo
        /// </summary>
        protected void Voltar(Object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(VoltarClick, null))
            {
                VoltarClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// Exibe a tela de Usuário Bloqueado caso o usuário já tenham tentado realizar 6 vezes a 
        /// Confirmação pPositiva.
        /// </summary>
        private void ExibirTelaBloqueio()
        {
            pnlErroPrincipal.Visible = false;
            pnlBotaoErro.Visible = false;
            pnlPagina1.Visible = false;
            pnlPagina2.Visible = false;
            pnlBloqueado.Visible = true;
        }

        /// <summary>
        /// Exibe painel customizado
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel customizado</returns>
        private Panel RetornarPainelExcecao(String fonte, Int32 codigo)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo, String mensagemAdicional)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += "<br /><br />" + mensagemAdicional;
            }
            //((QuadroAviso)quadroAviso).CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            ((QuadroAviso)quadroAviso).CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            pnlBotaoErro.Visible = true;
            pnlPagina1.Visible = false;
            pnlPagina2.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public DadosConfirmacaoPositiva Dados
        {
            get
            {
                if (object.ReferenceEquals(_dados, null) && object.ReferenceEquals(ViewState[_dados_key], null))
                {
                    _dados = new DadosConfirmacaoPositiva();
                    ViewState[_dados_key] = _dados;
                }
                return ViewState[_dados_key] as DadosConfirmacaoPositiva;
            }
            set
            {
                if (!object.ReferenceEquals(_dados, null))
                    _dados = ViewState[_dados_key] as DadosConfirmacaoPositiva;
                _dados = value;
                ViewState[_dados_key] = _dados;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<Int32, String> _propriedadesAleatorias = new Dictionary<Int32, String> {
            { 1, "pnlCEP" },
            { 2, "pnlTelefone" },
            { 4, "pnlBancoDebito" },
            { 3, "pnlLimiteParcelas" },
            { 6, "pnlTipoPV" },
        };

        /// <summary>
        /// 
        /// </summary>
        protected Int32 RecuperarIDPergunta(Int32 index)
        {
            if (index > 0)
            {
                Int32 _index = 1;
                foreach (KeyValuePair<Int32, String> painel in _propriedadesAleatorias)
                {
                    Panel panel = this.FindControl(painel.Value) as Panel;
                    if (panel.Visible)
                    {
                        if (index == _index)
                            return painel.Key;
                        else
                            _index++;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="perguntaID"></param>
        /// <returns></returns>
        protected String RecuperarRespostaPergunta(int perguntaID)
        {
            String valorRetorno = String.Empty;
            switch (perguntaID)
            {
                case 1:
                    // Remover - caractere "-" da validação por CEP
                    valorRetorno = txtCEP.Text.Replace("-", String.Empty);
                    break;
                case 2:
                    valorRetorno = txtTelefone.Text;
                    break;
                case 3:
                    valorRetorno = txtLimiteParcelas.Text;
                    break;
                case 4:
                    valorRetorno = this.FormatarDomicilioBancario(ddlBancoDebito.SelectedValue, txtAgenciaDebito.Text, txtContaCorrenteDebito.Text);
                    break;
                case 6:
                    valorRetorno = ddlTipoPV.SelectedValue;
                    break;
                case 7:
                    valorRetorno = this.FormatarDomicilioBancario(ddlBancoCredito.SelectedValue, txtAgenciaCredito.Text, txtContaCorrenteCredito.Text);
                    break;
            }
            return valorRetorno;
        }

        /// <summary>
        /// Cancelar o processo de Confirmação Positiva
        /// </summary>
        protected void Cancelar(object sender, EventArgs e)
        {
            VoltarHome(sender, e);
        }

        /// <summary>
        /// Invoca o evento de validação, esse evento deve estar atachado na classe que hospeda o controle.
        /// </summary>
        protected void EnviarDados(object sender, EventArgs e)
        {
            if (EnviarClick != null)
                EnviarClick(this, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        private string AtribuirZerosEsquerda(String original, Int32 numeroTotal)
        {
            string format = "000000000000000";
            int number = numeroTotal - original.Length;
            return original.Insert(0, format.Substring(0, number));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="banco"></param>
        /// <param name="agencia"></param>
        /// <param name="contacorrente"></param>
        /// <returns></returns>
        public string FormatarDomicilioBancario(string banco, string agencia, string contacorrente)
        {
            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();

                // Regra de Conta Corrente para quando o banco selecionado for
                // caixa economica federal, as c/c das caixa tem sempre 10 posições, começando
                // por 3 (Pessoa Jurídica) e 1 (Pessoa Física)
                if (!String.IsNullOrEmpty(banco) && banco.Trim() == "104") // CEF
                {
                    contacorrente = contacorrente.Trim();
                    if (contacorrente.Length < 10)
                    {
                        if (this.Dados.Empresa || info.Empresa)
                            contacorrente = "3" + AtribuirZerosEsquerda(contacorrente, 9);
                        else
                            contacorrente = "1" + AtribuirZerosEsquerda(contacorrente, 9);
                    }
                }
                string _banco = (banco.Length < 3 ? this.AtribuirZerosEsquerda(banco, 3) : banco);
                string _agencia = (agencia.Length < 5 ? this.AtribuirZerosEsquerda(agencia, 5) : agencia);
                string _contacorrente = (contacorrente.Length < 15 ? this.AtribuirZerosEsquerda(contacorrente, 15) : contacorrente);
                return String.Format("{0}{1}{2}", _banco, _agencia, _contacorrente);
            }
            return String.Empty;
        }

        /// <summary>
        /// Chama o serviço para validar a confirmação positiva do usuário.
        /// </summary>
        public Int32 ValidarCamposObrigatorios()
        {
            using (Logger Log = Logger.IniciarLog("Validando campos obrigatórios"))
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                    int numeroPV = usuario.NumeroPV;
                    Int32 codigoRetorno = 0;
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        codigoRetorno = client.ValidarConfirmacaoPositivaObrigatoria(numeroPV, this.Dados.NumeroEstabelecimento);
                        //if (codigoRetorno > 0)
                        //    client.IncrementarQuantidadeConfirmacaoPositiva(usuario.NumeroPV, usuario.GrupoEntidade, usuario.Usuario);
                    }
                    return codigoRetorno;
                }
                else
                    // Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.
                    return 1050;
            }
        }

        /// <summary>
        /// Chama o serviço para validar a confirmação positiva do usuário.
        /// </summary>
        public Int32 ValidarCamposVariaveis()
        {
            using (Logger Log = Logger.IniciarLog("Validando campos variáveis"))
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                    int numeroPV = usuario.NumeroPV;

                    this.Dados.CPF = Decimal.Parse(NormalizarString(txtCPF.Text));
                    Decimal cpf = this.Dados.CPF;

                    String domicilioBancarioCredito = String.Empty;
                    if (pnlBancoCredito.Visible) {
                        this.Dados.BancoCredito = Int32.Parse(NormalizarString(ddlBancoCredito.SelectedValue));
                        this.Dados.Agencia = NormalizarString(txtAgenciaCredito.Text);
                        this.Dados.CCCredito = NormalizarString(txtContaCorrenteCredito.Text);

                        domicilioBancarioCredito = this.FormatarDomicilioBancario(this.Dados.BancoCredito.ToString(), this.Dados.Agencia, this.Dados.CCCredito);
                    }

                    int _perguntaID1 = this.RecuperarIDPergunta(1);
                    int _perguntaID2 = this.RecuperarIDPergunta(2);

                    String _respostaPergunta1 = this.RecuperarRespostaPergunta(_perguntaID1);
                    String _respostaPergunta2 = this.RecuperarRespostaPergunta(_perguntaID2);

                    UsuarioServico.Pergunta p1 = new UsuarioServico.Pergunta()
                    {
                        CodigoPergunta = _perguntaID1,
                        PerguntaVariavel = true,
                        Resposta = _respostaPergunta1
                    };
                    UsuarioServico.Pergunta p2 = new UsuarioServico.Pergunta()
                    {
                        CodigoPergunta = _perguntaID2,
                        PerguntaVariavel = true,
                        Resposta = _respostaPergunta2
                    };
                    UsuarioServico.Pergunta p3 = new UsuarioServico.Pergunta()
                    {
                        CodigoPergunta = 3,
                        PerguntaVariavel = false,
                        Resposta = cpf.ToString()
                    };
                    List<UsuarioServico.Pergunta> perguntas = new List<UsuarioServico.Pergunta>();
                    perguntas.Add(p1);
                    perguntas.Add(p2);
                    perguntas.Add(p3);

                    // Verificar se a propriedade de Domicilio de Crédito está disponível
                    if (!String.IsNullOrEmpty(domicilioBancarioCredito))
                    {
                        UsuarioServico.Pergunta p4 = new UsuarioServico.Pergunta()
                        {
                            CodigoPergunta = 2,
                            PerguntaVariavel = false,
                            Resposta = domicilioBancarioCredito
                        };
                        perguntas.Add(p4);
                    }

                    Int32 codigoRetorno = 0;
                    var perguntasIncorretas = default(UsuarioServico.Pergunta[]);

                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        codigoRetorno = client.ValidarConfirmacaoPositivaVariavel(out perguntasIncorretas, numeroPV, perguntas.ToArray());
                        //if (codigoRetorno == 0)
                        //    client.ReiniciarQuantidadeConfirmacaoPositiva(usuario.NumeroPV, usuario.GrupoEntidade, usuario.Usuario);
                        //else
                        //{
                        //    client.IncrementarQuantidadeConfirmacaoPositiva(usuario.NumeroPV, usuario.GrupoEntidade, usuario.Usuario);
                        //}
                    }
                    return codigoRetorno;
                }
                else
                    // Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.
                    return 1050;
            }
        }

        /// <summary>
        /// Recuperar a quantidade de tentativas da confirmação positiva
        /// </summary>
        private Int32 RecuperarQuantidadeTentivas(out String mensagem)
        {
            using (Logger Log = Logger.IniciarLog("Recuperando quantidade tentativas"))
            {
                Int32 codigoRetorno = 0;
                mensagem = String.Empty;
                // Recuperar a quantidade restantes de Confirmação Positiva
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                        {
                            Codigo = usuario.NumeroPV,
                            GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = usuario.GrupoEntidade
                            }
                        };
                        var usuarios = client.ConsultarPorCodigoEntidade(out codigoRetorno, usuario.Usuario, entidade);
                        if (codigoRetorno == 0 && usuarios.Length > 0)
                        {
                            var _usuario = usuarios[0];
                            if (!_usuario.BloqueadoConfirmacaoPositiva)
                            {
                                mensagem = String.Format("Você ainda possui <b>{0}</b> tentativas.", 6 - _usuario.QuantidadeTentativaConfirmacaoPositiva);
                            }
                        }
                    }
                }
                return codigoRetorno;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProximaPagina(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Próxima página"))
            {
                // Armazenar dados no viewstate da primeira página
                this.Dados.NumeroEstabelecimento = Decimal.Parse(NormalizarString(txtNumero.Text));
                this.Dados.Empresa = (rdoPF.Checked ? false : true);

                Int32 codigoRetorno = this.ValidarCamposObrigatorios();
                if (codigoRetorno == 0)
                {
                    this.ExibirPagina2();
                }
                else
                {
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                        Int32 codigoRetornoBloqueio = 0;

                        UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                        {
                            GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = usuario.GrupoEntidade
                            },
                            Codigo = usuario.NumeroPV
                        };
                        var usuarios = client.ConsultarPorCodigoEntidade(out codigoRetornoBloqueio, usuario.Usuario, entidade);
                        if (usuarios.Length > 0)
                        {
                            UsuarioServico.Usuario _usuario = usuarios[0];
                            if (_usuario.BloqueadoConfirmacaoPositiva)
                                ExibirTelaBloqueio();
                            else
                            {
                                Int32 codigoRetornoQuantidade = 0;
                                String mensagem = String.Empty;
                                codigoRetornoQuantidade = RecuperarQuantidadeTentivas(out mensagem);
                                if (codigoRetornoQuantidade > 0)
                                    this.ExibirErro("SharePoint.RecuperarDados", codigoRetornoQuantidade, mensagem);
                                else
                                    this.ExibirErro("SharePoint.RecuperarDados", codigoRetorno, mensagem);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExibirPagina2()
        {
            pnlPagina1.Visible = false;
            pnlPagina2.Visible = true;

            if (!object.ReferenceEquals(ProximaPaginaClick, null))
            {
                ProximaPaginaClick(this, new EventArgs());
            }

            this.ExibirValidacoesAleatorias();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected String NormalizarString(String original)
        {
            return Regex.Replace(original, @"[^\w]", "");
        }

        /// <summary>
        /// Exibe duas propriedades aleatórias de validação da Confirmação Positiva
        /// </summary>
        protected void ExibirValidacoesAleatorias()
        {
            using (Logger Log = Logger.IniciarLog("Exibindo validações aleatórias"))
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario info = InformacaoUsuario.Recuperar();

                    hdfTipoNumero.Text = (this.Dados.Empresa ? "3000" : "1000");
                    using (EntidadeServico.EntidadeServicoClient _client = new EntidadeServico.EntidadeServicoClient())
                    {
                        EntidadeServico.Pergunta[] perguntas = _client.ConsultarPerguntasAleatorias(info.NumeroPV);
                        
                        List<EntidadeServico.Pergunta> _perguntas = new List<EntidadeServico.Pergunta>();
                        _perguntas.AddRange(perguntas);

                        // Exibir Domicilio Bancário de Crédito se Houver
                        foreach (EntidadeServico.Pergunta pergunta in _perguntas)
                        {
                            if (pergunta.CodigoPergunta == 7)
                                pnlBancoCredito.Visible = true;
                        }
                        _perguntas.RemoveAll(x => x.CodigoPergunta == 7);

                        Random r = new Random();
                        Int32 maxValue = _perguntas.Count;

                        if (_perguntas.Count > 1)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Int32 panelIndex = r.Next(maxValue);
                                Int32 _idControle = _perguntas[panelIndex].CodigoPergunta;
                                String _nomeControle = _propriedadesAleatorias[_idControle];
                                Panel pnl = this.FindControl(_nomeControle) as Panel;
                                if (!object.ReferenceEquals(pnl, null))
                                {
                                    if (pnl.Visible)
                                        i--;
                                    else
                                        pnl.Visible = true;
                                }
                            }
                        }
                        else
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, String.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Consulta os bancos e preenche combo
        /// </summary>
        protected void ConsultarBancos()
        {
            using (Logger Log = Logger.IniciarLog("Consultando bancos e preenchendo combos"))
            {
                using (EntidadeServico.EntidadeServicoClient entidadeClient = new EntidadeServico.EntidadeServicoClient())
                {
                    EntidadeServico.Banco[] bancos = entidadeClient.ConsultarBancosConfirmacaoPositiva();

                    foreach (EntidadeServico.Banco banco in bancos)
                    {
                        ddlBancoCredito.Items.Add(
                            new ListItem(banco.Codigo.ToString() + " - " + banco.Descricao, banco.Codigo.ToString()));
                        ddlBancoDebito.Items.Add(
                            new ListItem(banco.Codigo.ToString() + " - " + banco.Descricao, banco.Codigo.ToString()));
                    }
                }
            }
        }
    }
}
