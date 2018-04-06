using System;
using System.Runtime.Serialization;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Estabelecimentos
{
    [DataContract]
    public class Estabelecimento
    {
        [DataMember]
        public String Nome { get; set; }

        [DataMember]
        public String Status { get; set; }

        [DataMember]
        public Int32 Pv { get; set; }

        [DataMember]
        public TipoEstabelecimento? Tipo { get; set; }

        public String TipoEstabelecimento
        {
            get
            {
                if (this.Tipo.HasValue)
                {
                    switch(this.Tipo.Value)
                    {
                        case WebParts.Usuarios.Estabelecimentos.TipoEstabelecimento.Proprio:
                            {
                                Sessao sessaoAtual = Sessao.Obtem();
                                                                                               
                                if (EhMatriz)
                                    return this.Tipo.GetDescription();
                                else
                                    return "Próprio";
                            }
                        default:
                            return this.Tipo.GetDescription();
                    }
                }
                else return String.Empty;
            }
        }

        [Obsolete("Centralizar verificação na Comum.Sessao")]
        private Boolean EhMatriz
        {
            get
            {
                Sessao sessao = Sessao.Obtem();

                //Se estiver acessando Como Filial, não tem como ser Matriz
                if(sessao.AcessoFilial)
                    return false;
                //Se não possui código da Matriz, ou o código da Matriz é igual ao PV, é Matriz
                else if(sessao.CodigoMatriz == 0 || sessao.CodigoMatriz == sessao.CodigoEntidade)
                    return true;
                //Se código da Matriz existe, e não é igual ao PV, é Filial
                else
                    return false;
            }
        }
    }
}
