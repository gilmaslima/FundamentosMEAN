using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Credenciamento.Servicos
{
    [DataContract]
    public class Tecnologia
    {
        [DataMember]
        public Char CodTipoPessoa { get; set; }
        [DataMember]
        public Int64 NumCNPJ { get; set; }
        [DataMember]
        public Int32 NumSeqProp { get; set; }
        [DataMember]
        public String CodTipoEquipamento { get; set; }
        [DataMember]
        public Int32 QtdeTerminalSolicitado { get; set; }
        [DataMember]
        public Int32 CodPropEquipamento { get; set; }
        [DataMember]
        public Int32 CodTipoLigacao { get; set; }
        [DataMember]
        public Char IndHabilitaVendaDigitada { get; set; }
        [DataMember]
        public Char IndEnderecoIgualComercial { get; set; }
        [DataMember]
        public String LogradouroTecnologia { get; set; }
        [DataMember]
        public String ComplEnderecoTecnologia { get; set; }
        [DataMember]
        public String NumEnderecoTecnologia { get; set; }
        [DataMember]
        public String BairroTecnologia { get; set; }
        [DataMember]
        public String CidadeTecnologia { get; set; }
        [DataMember]
        public String EstadoTecnologia { get; set; }
        [DataMember]
        public String CodCepTecnologia { get; set; }
        [DataMember]
        public String CodComplCepTecnologia { get; set; }
        [DataMember]
        public String NomeContato { get; set; }
        [DataMember]
        public String NumDDD { get; set; }
        [DataMember]
        public Int32 NumTelefone { get; set; }
        [DataMember]
        public Int32 NumRamal { get; set; }
        [DataMember]
        public String CodFabricanteHardware { get; set; }
        [DataMember]
        public String NomeFabricanteHardware { get; set; }
        [DataMember]
        public String CodFornecedorSoftware { get; set; }
        [DataMember]
        public String NomeFornecedorSoftware { get; set; }
        [DataMember]
        public Int32 NumeroRenpac { get; set; }
        [DataMember]
        public Int32 DiaInicioFuncionamento { get; set; }
        [DataMember]
        public Int32 DiaFimFuncionamento { get; set; }
        [DataMember]
        public Int32 HoraInicioFuncionamento { get; set; }
        [DataMember]
        public Int32 HoraFimFuncionamento { get; set; }
        [DataMember]
        public Int32 CodRegimeTecnologia { get; set; }
        [DataMember]
        public Int32 CodCentroCustoTecnologia { get; set; }
        [DataMember]
        public Double ValorEquipamento { get; set; }
        [DataMember]
        public Int32 CodFilialTecnologia { get; set; }
        [DataMember]
        public String Observacao { get; set; }
        [DataMember]
        public Int32 QtdeCheckOut { get; set; }
        [DataMember]
        public Int32 NumPontoVenda { get; set; }
        [DataMember]
        public Char IndPinPad { get; set; }
        [DataMember]
        public Char IndFct { get; set; }
        [DataMember]
        public String UsuarioUltimaAtualizacao { get; set; }
        [DataMember]
        public Int32? CodigoCenario { get; set; }
        [DataMember]
        public String CodigoEventoEspecial { get; set; }
        [DataMember]
        public Int32? NumCpfTecnologia { get; set; }
        [DataMember]
        public Int32? AcaoComercial { get; set; }
        [DataMember]
        public Char? TerminalFatrExps { get; set; }
    }
}