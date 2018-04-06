using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class PontoVenda
    {
        [DataMember]
        public int Codigo { get; set; }

        [DataMember]
        public string RazaoSocial { get; set; }

        [DataMember]
        public DateTime DataFundacao { get; set; }

        [DataMember]
        public string Cnpj { get; set; }

        [DataMember]
        public string TipoPessoa { get; set; }

        [DataMember]
        public string NomeComercial { get; set; }

        [DataMember]
        public string PessoaContato { get; set; }

        [DataMember]
        public DadosTelefone Telefone { get; set; }

        [DataMember]
        public DadosTelefone Telefone2 { get; set; }

        [DataMember]
        public DadosTelefone Fax { get; set; }

        [DataMember]
        public string CodigoRamoAtividade { get; set; }
        [DataMember]
        public string DescricaoRamoAtividade { get; set; }


        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool Centralizadora { get; set; }

        [DataMember]
        public EnderecoPadrao Endereco { get; set; }

        [DataMember]
        public EnderecoPadrao EnderecoEntrega { get; set; }

        [DataMember]
        public String TipoEstabelecimento { get; set; }
        [DataMember]
        public Int32 CodigoCentral { get; set; }
        [DataMember]
        public DadosBancarios DadosBancarioCredito { get; set; }
        [DataMember]
        public DadosBancarios DadosBancarioDebito { get; set; }
        [DataMember]
        public DadosBancarios DadosBancarioMaestro { get; set; }
        [DataMember]
        public DadosBancarios DadosBancarioConstrucard { get; set; }

        [DataMember]
        public List<DadosProprietario> ListaProprietarios { get; set; }

        [DataMember]
        public string NomePlaqueta1 { get; set; }

        [DataMember]
        public string NomePlaqueta2 { get; set; }
    }
}
