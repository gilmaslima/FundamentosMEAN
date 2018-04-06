/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Redecard.PN.Comum.Test
{
    [TestClass]
    public class HistoricoTest
    {
        private static Sessao SessaoAtual
        {
            get
            {
                return new Sessao
                {
                    NomeEntidade = "Entidade",
                    CodigoEntidade = 1250191,
                    NomeUsuario = "Usuário",
                    Email = "email.usuario@userede.com.br",
                    TipoUsuario = "M",
                    CodigoIdUsuario = 3823,
                    CodigoMatriz = 0,
                    UltimoAcesso = DateTime.Now,
                    CodigoStatus = Enumerador.Status.UsuarioAtivo
                };
            }
        }

        private static Sessao SessaoAtualIntranet
        {
            get
            {
                return new Sessao
                {
                    NomeEntidade = "Entidade",
                    CodigoEntidade = 1250191,
                    NomeUsuario = "operacional",
                    Email = "email.usuario@userede.com.br",
                    TipoUsuario = "M",
                    Funcional = "689556",
                    CodigoStatus = Enumerador.Status.UsuarioAtivo
                };
            }
        }

        [TestMethod]
        public void GerarHistoricoDeAtividades()
        {
            Historico.AlteracaoDadosEstabelecimento(SessaoAtual, "celular", "nome");
            Historico.AlteracaoDadosEstabelecimento(SessaoAtual, "endereço", "telefone");
            Historico.AlteracaoDadosEstabelecimento(SessaoAtual, "campo1", "campo2");
        
            Historico.AlteracaoDadosOutroUsuario(
                SessaoAtual, 12345, "Alexandre 123", "alexandre.123@email.com.br", "M", "nome", "celular");
        
            Historico.AlteracaoDadosUsuario(SessaoAtual, "telefone", "cpf", "celular");                        
        
            Historico.AprovacaoUsuario(SessaoAtual, 192836, "André 321", "andre.321@email.com.br", "M");
        
            Historico.CriacaoUsuario(SessaoAtual,
                1823, "Agnaldo 555", "agnaldo.555@email.com.br", "P", new List<Int32>(new [] { 1250192, 1250193 }));
        
            Historico.RecuperacaoSenha(
                18273, "José", "jose@email.com.br", "M", 1250191, "E-mail secundário");
        
            Historico.RecuperacaoUsuario(876361, "João", "joao@email.com.br", "B", 1250191);
        
            Historico.ErroConfirmacaoPositiva(7612376, "Alexandre 123", "alexandre.123@email.com.br", "M",
                1250191, "Recuperação de Usuário", "agência", "razão social");
            Historico.ErroConfirmacaoPositiva(7612376, "Alexandre 123", "alexandre.123@email.com.br", "M",
                1250191, "Acesso Completo", "cnpj", "nome do sócio");
        
            Historico.Login(SessaoAtual, DateTime.Now);
            Historico.LiberacaoAcessoCompleto(SessaoAtual);
            Historico.CriacaoUsuario(113, "Alexandre Shiroma", "alexandre@gmail.com", "B", 1250191, true, true);
            Historico.SolicitacaoCriacaoUsuario(null, null, "alexandre.2@email.com.br", null, 1250191, false, false, String.Empty);
            Historico.SolicitacaoCriacaoUsuario(111, "Alexandre 2", "alexandre.2@email.com.br", "B", 1250191, true, false, "Confirmou e-mail");
            Historico.RejeicaoUsuario(SessaoAtual, 111, "Alexandre 2", "alexandre.2@email.com.br", "B");
            Historico.BloqueioUsuarioErroSenha(111, "Alexandre 2", "alexandre.2@email.com.br", "B", 1250191);
            Historico.DesbloqueioUsuario(SessaoAtualIntranet, 111, "Alexandre 2", "alexandre.2@email.com.br", "B");
            Historico.RealizacaoServico(SessaoAtual, "RAV");
            Historico.RealizacaoServico(SessaoAtual, "Solicitação de Material");
            Historico.RealizacaoServico(SessaoAtual, "2ª Via Extrato");
            Historico.RealizacaoServico(SessaoAtual, "Cancelamento de Vendas");
            Historico.RealizacaoServico(SessaoAtual, "Comprovação de Vendas");
            Historico.RealizacaoServico(SessaoAtual, "Contratação de Consulta Cheque");

            new[] { 
                "Confirmação de acesso",
                "Recuperação de senha",
                "Informativo de recuperação de senha",
                "Solicitação de aprovação de acesso",
                "Rejeição de acesso",
                "Formulário da parte aberta bloqueado"
            }.ToList().ForEach(tipoEmail => Historico.EnvioEmail(SessaoAtual, tipoEmail, "destinatario@email.com",
                new List<Int32>(new [] { 1250192, 1250193, 1250194 })));

            Historico.ExclusaoUsuario(SessaoAtual, 981239, "Nome Usuário excluído", "email@usuarioexcluido.com", "Básico");
        }        
    }
}