/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

namespace Redecard.PN.OutrasEntidades
{
    /// <summary>
    /// Enumerador do tipo do controle
    /// </summary>
    public enum TipoCampoNovoAcesso
    {                     
        /// <summary>Celular</summary>
        Celular,

        /// <summary>CNPJ</summary>
        CNPJ,

        /// <summary>Confirmação de E-mail</summary>
        ConfirmacaoEmail,

        /// <summary>Confirmação de Senha</summary>
        ConfirmacaoSenha,

        /// <summary>CPF</summary>
        CPF,

        /// <summary>E-mail</summary>
        Email,

        /// <summary>E-mail secundário</summary>
        EmailSecundario,

        /// <summary>Estabelecimento</summary>
        Estabelecimento,

        /// <summary>Senha</summary>
        Senha,

        /// <summary>Telefone</summary>
        Telefone,

        /// <summary>Texto</summary>
        Texto
    }
}