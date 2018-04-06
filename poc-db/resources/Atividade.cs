/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Enumerador dos tipos de atividade
    /// </summary>
    internal enum Atividade
    {
        CriacaoUsuario                          = 1,
        SolicitacaoAcesso                       = 2,
        AlteracaoCadastroOutroUsuario           = 3,
        AlteracaoCadastroProprioUsuario         = 4,
        AlteracaoCadastroEstabelecimento        = 5,
        AprovacaoRejeicaoUsuario                = 6,
        EnvioEmail                              = 7,
        RecuperacaoSenha                        = 8,
        RecuperacaoUsuario                      = 9,
        ErroConfirmacaoPositiva                 = 10,
        LiberacaoAcessoCompleto                 = 11,
        Login                                   = 12,
        DesbloqueioUsuario                      = 13,
        BloqueioUsuarioErroSenha                = 14,
        ExclusaoUsuario                         = 15,
        RealizacaoServico                       = 16,
        DesbloqueioFormularioSolicitacaoAcesso  = 17,
        BloqueioFormularioSolicitacaoAcesso     = 18
    }
}