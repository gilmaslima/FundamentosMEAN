using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    public class EntidadeEC : Base
    {
        public String EnderecoIntranet { get; set; } //'nom_end_intn',
        public String TipoAtualizacao { get; set; }//'tip_atlz',
        public DateTime DataUltimaAtualizacao { get; set; }//'dth_ult_atlz',
        public String NomeResponsavelAtualizacao { get; set; } //'nom_rspn_ult_atlz',
        public Int32 TipAcessoEntidade { get; set; } //'tip_ace_etd',
        public Int32 TipServicoWebService { get; set; } //'tip_serv_ws',
        public String EnderecoIP { get; set; } //'nom_end_ip',
        public String EnderecoIP1 { get; set; }//'nom_end_ip1',
        public String EnderecoIP2 { get; set; }//'//'nom_end_ip2',
        public String EnderecoIP3 { get; set; }//'nom_end_ip3',
        public String EnderecoIP4 { get; set; }//'nom_end_ip4',
        public String EnderecoIP5 { get; set; }//'nom_end_ip5',
        public String EnderecoIP6 { get; set; }//'nom_end_ip6',
        public String EnderecoIP7 { get; set; }//'nom_end_ip7',
        public String EnderecoIP8 { get; set; }//'nom_end_ip8',
        public String EnderecoIP9 { get; set; }//'nom_end_ip9',
        public String SecureCodeGlobal_M { get; set; }//'ind_pgm_gbl',
        public String OrigemEmissorCartaoC_M { get; set; }//'ind_tip_emsr',
        public String OrigemEmissorCartaoM_M { get; set; } //'ind_tip_emsr_deb',
        public String OrigemEmissorCartaoC_V { get; set; }//'ind_tip_emsr_visa',
        public String OrigemEmissorCartaoM_V { get; set; }//''ind_tip_emsr_deb_visa',
        public String SecureCodeGlobal_V { get; set; }//'ind_pgm_gbl_visa'
    }
}
