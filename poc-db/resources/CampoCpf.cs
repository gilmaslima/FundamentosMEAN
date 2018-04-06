/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    /// <summary>
    /// Classe para implementação do CampoNovoAcesso - CPF
    /// </summary>
    public class CampoCpf : ICampoNovoAcesso
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="controle">Instância do controle CampoNovoAcesso</param>
        public CampoCpf(CampoNovoAcesso controle) : base(controle) { }

        /// <summary>
        /// Inicialização do controle.
        /// Atribuição de configurações específicas (máscara, textMode, etc)
        /// </summary>
        public override void InicializarControle() 
        {
            this.Controle.TextBox.Attributes["alt"] = "cpf";
        }

        /// <summary>
        /// Valida conteúdo do controle, se CPF é válido
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <returns>Se conteúdo é válido</returns>
        public override Boolean Validar(Boolean exibirMensagem)
        {
            Boolean obrigatorio = this.Controle.Obrigatorio;
            Boolean preenchido = !String.IsNullOrEmpty(this.Controle.Text);
            Boolean valido = ValidarCpf(this.Controle.Text);

            //Se campo obrigatório
            if (!preenchido && obrigatorio)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErroSemIcone(ICampoNovoAcesso.CampoObrigatorio);
                return false;
            }

            //Se preenchido e número inválido, erro
            if (preenchido && !valido)
            {
                if (exibirMensagem)
                    this.Controle.ExibirMensagemErro("CPF inválido");
                return false;
            }

            if (exibirMensagem)
                this.Controle.ExibirMensagemSucesso("CPF válido");
            return true;
        }

        /// <summary>
        /// Validação de CPF de Usuário
        /// <para>1. Verifica se CPF está preenchido</para>
        /// <para>2. Verifica se CPF é algoritmicamente válido</para>
        /// <para>3. Verifica se CPF já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código dos PVs</param>
        /// <returns>Se CPF é válido</returns>
        public Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades)
        {
            return this.Validar(exibirMensagem, codigoEntidades, null);
        }

        /// <summary>
        /// Validação de CPF de Usuário
        /// <para>1. Verifica se CPF está preenchido</para>
        /// <para>2. Verifica se CPF é algoritmicamente válido</para>
        /// <para>3. Verifica se CPF já existe para o PV</para>
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código dos PVs</param>
        /// <param name="codigoIdUsuario">ID do Usuário a validar</param>
        /// <returns>Se CPF é válido</returns>
        public Boolean Validar(Boolean exibirMensagem, Int32[] codigoEntidades, Int32? codigoIdUsuario)
        {
            using (Logger log = Logger.IniciarLog("Validação de CPF de Usuário"))
            {
                Boolean logicamenteValido = this.Validar(exibirMensagem);
                if (!logicamenteValido)
                    return false;

                Boolean cpfDisponivel = true;
                var usuarios = default(UsuarioServico.Usuario[]);

                foreach (Int32 codigoEntidade in codigoEntidades)
                {
                    try
                    {
                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            usuarios = ctx.Cliente.ConsultarPorCpf(this.Cpf.Value, codigoEntidade, 1);
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }

                    if (usuarios != null)
                    {
                        var usuario = usuarios.FirstOrDefault();

                        if (usuario != null)
                            if (usuario.CodigoIdUsuario != codigoIdUsuario)
                                cpfDisponivel = false;
                    }
                }

                if (!cpfDisponivel)
                {
                    if (exibirMensagem)
                        this.Controle.ExibirMensagemErro("Este CPF já possui usuário cadastrado");
                    return false;
                }

                if (cpfDisponivel)
                    this.Controle.ExibirMensagemSucesso("CPF válido");
            }

            return true;
        }

        /// <summary>
        /// Validação do CPF com máscara 000.000.000-00
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <returns>Se válido</returns>
        public static Boolean ValidarCpf(String cpf)
        {
            Int32[] multiplicador1 = new Int32[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32[] multiplicador2 = new Int32[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            String tempCpf = default(String);
            String digito = default(String);
            Int32 soma = default(Int32);
            Int32 resto = default(Int32);

            //CPF com máscara de ter exatamente 14 caracteres
            if(cpf.Length != 14)
                return false;
            
            //Verifica separadores
            if (cpf[3] != '.' || cpf[7] != '.' || cpf[11] != '-')
                return false;

            //Remove caracteres especiais
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            //Verifica se sobraram apenas números
            for (Int32 i = 0; i < cpf.Length; i++)
                if (!Char.IsNumber(cpf[i]))
                    return false;

            //Valida dígito verificador
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (Int32 i = 0; i < 9; i++)
                soma += tempCpf[i].ToString().ToInt32() * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (Int32 i = 0; i < 10; i++)
                soma += tempCpf[i].ToString().ToInt32() * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
                
        /// <summary>
        /// Cpf
        /// </summary>
        public Int64? Cpf
        {
            get 
            { 
                return new String(this.Controle.Text.Where(c => Char.IsNumber(c)).ToArray()).ToInt64Null();
            }
            set 
            {
                this.Controle.Text = AplicarMascara(value);
            }
        }

        /// <summary>
        /// Aplica máscara CPF (999.999.999-99)
        /// </summary>
        public static String AplicarMascara(Int64? cpf)
        {
            if (cpf.HasValue)
            {
                String valor = cpf.Value.ToString().PadLeft(11, '0');

                if (valor.Length == 11)
                {
                    valor = valor.Insert(3, ".");
                    valor = valor.Insert(7, ".");
                    valor = valor.Insert(11, "-");
                }
                return valor;
            }
            else
                return String.Empty;
        }
    }
}