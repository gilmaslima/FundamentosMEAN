#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [05/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Linq;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// 
    /// </summary>
    public enum TipoDado
    {
        Usuario = 1,
        Senha = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class RecuperarDados : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public TipoDado Tipo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void EnviarClickHandle(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event EnviarClickHandle EnviarClick;

        /// <summary>
        /// Invoca o evento de validação, esse evento deve estar atachado na classe que hospeda o controle.
        /// </summary>
        protected void EnviarDados(object sender, EventArgs e)
        {
            if (EnviarClick != null)
                EnviarClick(this, new EventArgs());
        }

        /// <summary>
        /// Cancelar o processo de Confirmação Positiva
        /// </summary>
        protected void Cancelar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Muda o label de acordo com o tipo de recuperação de dados
            if (!Page.IsPostBack)
            {
                label.Text = (Tipo == TipoDado.Senha ? "Usuário" : "Senha");
                if (Tipo == TipoDado.Usuario)
                    txtValor.TextMode = System.Web.UI.WebControls.TextBoxMode.Password;
            }
        }

        /// <summary>
        /// Verificar se o número do PV e a senha batem com algum usuário cadastrado no sistema, estamos
        /// utilizando a senha como chave para o usuário pq não existe um identificador único.
        /// </summary>
        /// <returns></returns>
        private Int32 ValidarUsuario()
        {
            using (Logger Log = Logger.IniciarLog("Validando usuário"))
            {
                UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
         {
             Codigo = Int32.Parse(txtNEstabelecimento.Text),
             GrupoEntidade = new UsuarioServico.GrupoEntidade()
             {
                 Codigo = 1
             }
         };

                using (UsuarioServico.UsuarioServicoClient _client = new UsuarioServico.UsuarioServicoClient())
                {
                    Int32 codigoRetorno = 0;
                    UsuarioServico.Usuario[] _usuarios = null;

                    if (Tipo == TipoDado.Senha)
                    {
                        _usuarios = _client.ConsultarPorCodigoEntidade(out codigoRetorno, txtValor.Text, entidade);
                        codigoRetorno = ValidarPorUsuario(_usuarios);
                    }
                    else
                    {
                        String senha = EncriptadorSHA1.EncryptString(txtValor.Text);
                        _usuarios = _client.ConsultarPorSenha(out codigoRetorno, senha, entidade);
                        codigoRetorno = ValidarPorSenha(_usuarios);
                    }

                    if (codigoRetorno == 0)
                    {
                        // validar cnpj/cpf do estabelecimento
                        Decimal cpfcnpj = Decimal.Parse(NormalizarString(txtNumero.Text));
                        return _client.ValidarConfirmacaoPositivaObrigatoria(entidade.Codigo, cpfcnpj);
                    }
                    else
                        return codigoRetorno;
                }
            }
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
        /// Verifica se é possível recuperar os dados pelo nome do usuário
        /// </summary>
        /// <param name="_usuarios"></param>
        /// <returns></returns>
        private Int32 ValidarPorUsuario(UsuarioServico.Usuario[] _usuarios)
        {
            using (Logger Log = Logger.IniciarLog("Validando por usuário"))
            {
                String nomeUsuario = txtValor.Text;
                var usuariosMatch = _usuarios.Where(x => x.Codigo == nomeUsuario);
                Int32 usuarioCount = usuariosMatch.Count();

                if (usuarioCount > 0 && usuarioCount < 2) // Válido, achou somente um usuário
                {
                    List<UsuarioServico.Usuario> usuarios = usuariosMatch.ToList<UsuarioServico.Usuario>();
                    UsuarioServico.Usuario usuario = usuarios[0];

                    // Somente deixar o usuário master fazer o esqueci do usuário e senha, para
                    // um usuário padrão, exibir uma mensagem informativa
                    if (usuario.TipoUsuario.ToLowerInvariant().Equals("m"))
                    {
                        InformacaoUsuario.Criar(0, Int32.Parse(txtNEstabelecimento.Text), String.Empty);
                        InformacaoUsuario _dados = InformacaoUsuario.Recuperar();
                        _dados.NumeroPV = Int32.Parse(NormalizarString(txtNEstabelecimento.Text));
                        _dados.CpfCnpjEstabelecimento = Decimal.Parse(NormalizarString(txtNumero.Text));
                        _dados.GrupoEntidade = usuario.Entidade.GrupoEntidade.Codigo;
                        _dados.Senha = usuario.Senha;
                        _dados.Empresa = (rdoPJ.Checked);
                        _dados.Usuario = usuario.Codigo;
                        _dados.NomeCompleto = usuario.Descricao;
                        _dados.EsqueciUsuario = false;
                        InformacaoUsuario.Salvar(_dados);
                        return 0;
                    }
                    else
                    {
                        // Error de Validação 1048 - 
                        return 1048;
                    }
                }
                else
                    // Erro 1069 - 
                    return 1069;
            }
        }

        /// <summary>
        /// Verifica se é possível recuperar os dados pela senha do usuário
        /// </summary>
        /// <param name="_usuarios"></param>
        /// <returns></returns>
        private Int32 ValidarPorSenha(UsuarioServico.Usuario[] _usuarios)
        {
            using (Logger Log = Logger.IniciarLog("Validando por senha"))
            {
                String senhaCriptogfada = EncriptadorSHA1.EncryptString(txtValor.Text);
                var usuariosMatch = _usuarios.Where(x => x.Senha == senhaCriptogfada || x.SenhaTemporaria == senhaCriptogfada);
                Int32 usuarioCount = usuariosMatch.Count();
                if (usuarioCount > 0 && usuarioCount < 2) // Válido, achou somente um usuário
                {
                    List<UsuarioServico.Usuario> usuarios = usuariosMatch.ToList<UsuarioServico.Usuario>();
                    UsuarioServico.Usuario usuario = usuarios[0];

                    // Somente deixar o usuário master fazer o esqueci do usuário e senha, para
                    // um usuário padrão, exibir uma mensagem informativa
                    if (usuario.TipoUsuario.ToLowerInvariant().Equals("m"))
                    {
                        InformacaoUsuario.Criar(0, Int32.Parse(txtNEstabelecimento.Text), String.Empty);
                        InformacaoUsuario _dados = InformacaoUsuario.Recuperar();
                        _dados.NumeroPV = Int32.Parse(txtNEstabelecimento.Text);
                        _dados.CpfCnpjEstabelecimento = Decimal.Parse(NormalizarString(txtNumero.Text));
                        _dados.GrupoEntidade = usuario.Entidade.GrupoEntidade.Codigo;
                        _dados.Senha = usuario.Senha;
                        _dados.Empresa = (rdoPJ.Checked);
                        _dados.Usuario = usuario.Codigo;
                        _dados.NomeCompleto = usuario.Descricao;
                        _dados.EsqueciUsuario = true;
                        InformacaoUsuario.Salvar(_dados);
                        return 0;
                    }
                    else
                        // Error de Validação 1050 - 
                        return 1048;
                }
                else
                    // Erro 1069 - 
                    return 1069;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32 Validar()
        {
            using (Logger Log = Logger.IniciarLog("Validando"))
            {
                Int32 codigoRetornoIS;
                Int32 codigoRetornoGE;

                // Busca pelo Entidade
                using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                {
                    EntidadeServico.Entidade[] entidades = client.Consultar(out codigoRetornoIS, out codigoRetornoGE, Int32.Parse(txtNEstabelecimento.Text), null);

                    if (codigoRetornoIS > 0 || codigoRetornoGE > 0)
                        return (codigoRetornoIS > 0 ? codigoRetornoIS : codigoRetornoGE);
                    else
                        return this.ValidarUsuario();
                }
            }
        }
    }
}
