using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.ZeroDolar.SharePoint {

    public class ServicosDisponiveis {

        public ServicosDisponiveis(int id, string nome, int situacao, string termosECondicoes, string atencaoCancelamento) {
            this.Id = id;
            this.Nome = nome;
            this.Situacao = situacao;
            this.TermosECondicoes = string.Format("{0} - {1}", termosECondicoes, this.Nome);
            this.AtencaoCancelamento = string.Format("{0} - {1}", atencaoCancelamento, this.Nome);
        }
        public string Nome { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// 0 = Desativado, 1 = Ativado, 2 = Aguardando liberação
        /// </summary>
        public int Situacao { get; set; }

        public string TermosECondicoes { get; set; }

        public string AtencaoCancelamento { get; set; }
    }

    public class ZeroDolarServicos {

        public static List<ServicosDisponiveis> ListaServicos { get; set; }

        internal static List<ServicosDisponiveis> ObterListaServicosDisponiveis() {
            if (ListaServicos == null) {
                ListaServicos = new List<ServicosDisponiveis>();

                ListaServicos.Add(new ServicosDisponiveis(1, "Zero Dolar", 0, "Termos e condicoes para serviço", "Atenção cancelamento"));
                ListaServicos.Add(new ServicosDisponiveis(2, "Outro Serviço", 1, "Termos e condicoes para serviço", "Atenção cancelamento"));
                ListaServicos.Add(new ServicosDisponiveis(4, "Serviço pago", 1, "Termos e condicoes para serviço", "Atenção cancelamento"));
                ListaServicos.Add(new ServicosDisponiveis(5, "Mais um serviço", 1, "Termos e condicoes para serviço", "Atenção cancelamento"));
                ListaServicos.Add(new ServicosDisponiveis(6, "Serviço legal", 0, "Termos e condicoes para serviço", "Atenção cancelamento"));

            }
            return ListaServicos;
        }

        internal static void CancelarServico(int idServico) {
            if (ListaServicos == null)
                throw new Exception("Lista Serviços esta nulo");

            ServicosDisponiveis servicoCancelar = ListaServicos.Find(obj => obj.Id.Equals(idServico));
            if (servicoCancelar.Situacao == 1) {
                servicoCancelar.Situacao = 0;
            }
        }

        internal static void ContratarServico(int idServico) {
            if (ListaServicos == null)
                throw new Exception("Lista Serviços esta nulo");

            ServicosDisponiveis servicoCancelar = ListaServicos.Find(obj => obj.Id.Equals(idServico));
            if (servicoCancelar.Situacao == 0) {
                servicoCancelar.Situacao = 1;
            }
        }
    }
}
