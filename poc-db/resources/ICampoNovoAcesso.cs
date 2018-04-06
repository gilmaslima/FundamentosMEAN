using System;

namespace Redecard.PN.OutrasEntidades {

    /// <summary>
    /// Classe abstrata para implementação pelos tipo 
    /// de controle do UserControl CampoNovoAcesso.
    /// </summary>
    public abstract class ICampoNovoAcesso
    {
        /// <summary>
        /// Mensagem padrão de Campo Obrigatório
        /// </summary>
        protected static String CampoObrigatorio { get { return "* Campo obrigatório"; } }

        /// <summary>
        /// Controle associado ao campo
        /// </summary>
        public CampoNovoAcesso Controle { get; private set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="controle">Controle</param>
        public ICampoNovoAcesso(CampoNovoAcesso controle)
        {
            this.Controle = controle;
        }

        /// <summary>
        /// Validação do controle, de acordo com o tipo.
        /// </summary>
        /// <param name="exibirMensagem">
        /// Flag indicando se devem ser exibidas automaticamente as mensagens de validação
        /// </param>
        /// <returns>Se campo é válido.</returns>
        public virtual Boolean Validar(Boolean exibirMensagem) { return true; }
        
        /// <summary>
        /// Inicialização do controle.
        /// Setar máscaras, configuração do TextBox, etc.
        /// </summary>
        public abstract void InicializarControle();

        /// <summary>
        /// Obtém a classe relacionada ao tipo de campo do controle.
        /// </summary>
        /// <param name="controle">Controle</param>
        /// <returns>Classe de configuração/comportamento de acordo com o tipo do controle</returns>
        public static ICampoNovoAcesso Obter(CampoNovoAcesso controle)
        {
            switch (controle.Tipo)
            {

                case TipoCampoNovoAcesso.ConfirmacaoSenha: return new CampoConfirmacaoSenha(controle);
                case TipoCampoNovoAcesso.Senha:             return new CampoSenha(controle);               
                //caso não esperado, Tipo de Controle não configurado
                default:
                    throw new NotImplementedException(
                        String.Format("Especificar o \"Tipo\" do controle {0}.", controle.ID));
            }
        }
    
        /// <summary>
        /// Evento de PostBack customizado
        /// </summary>
        public virtual void RaisePostBackEvent(String argument) { }
    }
}