/*
(c) Copyright [2012] Redecard S.A.
Autor : [Daniel Coelho]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/07/30 - Daniel Coelho - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.RAV.Modelos
{
    public class ModRAVAvulsoSaida
    {
        private Int32 _Retorno;
        public Int32 Retorno
        {
            get
            {
                return _Retorno;
            }
            set
            {
                if (_Retorno != value)
                {
                    _Retorno = value;
                }
            }
        }

        private String _DataProcessamento;
        public String DataProcessamento
        {
            get
            {
                return _DataProcessamento;
            }
            set
            {
                if (_DataProcessamento != value)
                {
                    _DataProcessamento = value;
                }
            }
        }

        private String _HoraProcessamento;
        public String HoraProcessamento
        {
            get
            {
                return _HoraProcessamento;
            }
            set
            {
                if (_HoraProcessamento != value)
                {
                    _HoraProcessamento = value;
                }
            }
        }

        private Int32 _Banco;
        public Int32 Banco
        {
            get
            {
                return _Banco;
            }
            set
            {
                if (_Banco != value)
                {
                    _Banco = value;
                }
            }
        }

        private Int64 _Agencia;
        public Int64 Agencia
        {
            get
            {
                return _Agencia;
            }
            set
            {
                if (_Agencia != value)
                {
                    _Agencia = value;
                }
            }
        }

        private Int64 _Conta;
        public Int64 Conta
        {
            get
            {
                return _Conta;
            }
            set
            {
                if (_Conta != value)
                {
                    _Conta = value;
                }
            }
        }

        private Decimal _ValorMinimo;
        public Decimal ValorMinimo
        {
            get
            {
                return _ValorMinimo;
            }
            set
            {
                if (_ValorMinimo != value)
                {
                    _ValorMinimo = value;
                }
            }
        }

        private String _HoraIniD0;
        public String HoraIniD0
        {
            get
            {
                return _HoraIniD0;
            }
            set 
            {
                if (_HoraIniD0 != value)
                {
                    _HoraIniD0 = value;
                }
            }
        }

        private String _HoraFimD0;
        public String HoraFimD0
        {
            get
            {
                return _HoraFimD0;
            }
            set
            {
                if (_HoraFimD0 != value)
                {
                    _HoraFimD0 = value;
                }
            }
        }

        private String _HoraIniDn;
        public String HoraIniDn
        {
            get
            {
                return _HoraIniDn;
            }
            set
            {
                if (_HoraIniDn != value)
                {
                    _HoraIniDn = value;
                }
            }
        }

        private String _HoraFimDn;
        public String HoraFimDn
        {
            get
            {
                return _HoraFimDn;
            }
            set
            {
                if (_HoraFimDn != value)
                {
                    _HoraFimDn = value;
                }
            }
        }

        private IList<ModRAVAvulsoCredito> _DadosParaCredito = null;
        public IList<ModRAVAvulsoCredito> DadosParaCredito
        { 
            get 
            { 
                return _DadosParaCredito; 
            }
        }

        private IList<ModRAVAvulsoRetorno> _TabelaRAVs = null;
        public IList<ModRAVAvulsoRetorno> TabelaRAVs 
        { 
            get 
            {
                return _TabelaRAVs; 
            } 
        }

        private Decimal _Desconto;
        public Decimal Desconto
        {
            get
            {
                return _Desconto;
            }
            set
            {
                if (_Desconto != value)
                {
                    _Desconto = value;
                }
            }
        }

        private Decimal _ValorBruto;
        public Decimal ValorBruto
        {
            get
            {
                return _ValorBruto;
            }
            set
            {
                if (_ValorBruto != value)
                {
                    _ValorBruto = value;
                }
            }
        }

        private Decimal _ValorOriginal;
        public Decimal ValorOriginal
        {
            get
            {
                return _ValorOriginal;
            }
            set
            {
                if (_ValorOriginal != value)
                {
                    _ValorOriginal = value;
                }
            }
        }

        private DateTime _PeriodoDe;
        public DateTime PeriodoDe
        {
            get
            {
                return _PeriodoDe;
            }
            set
            {
                if (_PeriodoDe != value)
                {
                    _PeriodoDe = value;
                }
            }
        }

        private DateTime _PeriodoAte;
        public DateTime PeriodoAte
        {
            get
            {
                return _PeriodoAte;
            }
            set
            {
                if (_PeriodoAte != value)
                {
                    _PeriodoAte = value;
                }
            }
        }

        private String _MsgErro;
        public String MsgErro
        {
            get
            {
                return _MsgErro;
            }
            set
            {
                if (_MsgErro != value)
                {
                    _MsgErro = value;
                }
            }
        }

        private DateTime _FimCarencia;
        public DateTime FimCarencia
        {
            get
            {
                return _FimCarencia;
            }
            set
            {
                if (_FimCarencia != value)
                {
                    _FimCarencia = value;
                }
            }
        }

        private Decimal _ValorAntecipadoD0;
        public Decimal ValorAntecipadoD0
        {
            get
            {
                return _ValorAntecipadoD0;
            }
            set
            {
                if (_ValorAntecipadoD0 != value)
                {
                    _ValorAntecipadoD0 = value;
                }
            }
        }

        private Decimal _ValorAntecipadoD1;
        public Decimal ValorAntecipadoD1
        {
            get
            {
                return _ValorAntecipadoD1;
            }
            set
            {
                if (_ValorAntecipadoD1 != value)
                {
                    _ValorAntecipadoD1 = value;
                }
            }
        }

        private Decimal _ValorDisponivel;
        public Decimal ValorDisponivel
        {
            get
            {
                return _ValorDisponivel;
            }
            set
            {
                if (_ValorDisponivel != value)
                {
                    _ValorDisponivel = value;
                }
            }
        }

        private ModRAVAntecipa _DadosAntecipado;
        public ModRAVAntecipa DadosAntecipado
        {
            get
            {
                return _DadosAntecipado;
            }
            set
            {
                if (_DadosAntecipado != value)
                {
                    _DadosAntecipado = value;
                }
            }
        }

        public ModRAVAvulsoSaida()
        {
            _DadosParaCredito = new List<ModRAVAvulsoCredito>();
            _TabelaRAVs = new List<ModRAVAvulsoRetorno>();
            _DadosAntecipado = new ModRAVAntecipa();
        }
    }

    public class ModRAVAvulsoCredito
    {
        private DateTime _DataCredito;
        public DateTime DataCredito
        {
            get
            {
                return _DataCredito;
            }
            set
            {
                if (_DataCredito != value)
                {
                    _DataCredito = value;
                }
            }
        }

        private Decimal _TaxaEfetiva;
        public Decimal TaxaEfetiva
        {
            get
            {
                return _TaxaEfetiva;
            }
            set
            {
                if (_TaxaEfetiva != value)
                {
                    _TaxaEfetiva = value;
                }
            }
        }

        private Decimal _TaxaPeriodo;
        public Decimal TaxaPeriodo
        {
            get
            {
                return _TaxaPeriodo;
            }
            set
            {
                if (_TaxaPeriodo != value)
                {
                    _TaxaPeriodo = value;
                }
            }
        }

        private Decimal _ValorLiquido;
        public Decimal ValorLiquido
        {
            get
            {
                return _ValorLiquido;
            }
            set
            {
                if (_ValorLiquido != value)
                {
                    _ValorLiquido = value;
                }
            }
        }

        private Decimal _ValorRotativo;
        public Decimal ValorRotativo
        {
            get
            {
                return _ValorRotativo;
            }
            set
            {
                if (_ValorRotativo != value)
                {
                    _ValorRotativo = value;
                }
            }
        }

        private Decimal _ValorParcelado;
        public Decimal ValorParcelado
        {
            get
            {
                return _ValorParcelado;
            }
            set
            {
                if (_ValorParcelado != value)
                {
                    _ValorParcelado = value;
                }
            }
        }
    }

    public class ModRAVAvulsoRetorno
    {
        private Int64 _NumeroRAV;
        public Int64 NumeroRAV
        {
            get
            {
                return _NumeroRAV;
            }
            set
            {
                if (_NumeroRAV != value)
                {
                    _NumeroRAV = value;
                }
            }
        }

        private Decimal _ValorBruto;
        public Decimal ValorBruto
        {
            get
            {
                return _ValorBruto;
            }
            set
            {
                if (_ValorBruto != value)
                {
                    _ValorBruto = value;
                }
            }
        }

        private Decimal _ValorLiquido;
        public Decimal ValorLiquido
        {
            get
            {
                return _ValorLiquido;
            }
            set
            {
                if (_ValorLiquido != value)
                {
                    _ValorLiquido = value;
                }
            }
        }

        private DateTime _DataApresentacao;
        public DateTime DataApresentacao
        {
            get
            {
                return _DataApresentacao;
            }
            set
            {
                if (_DataApresentacao != value)
                {
                    _DataApresentacao = value;
                }
            }
        }

        private Int64 _QuantidadeOC;
        public Int64 QuantidadeOC
        {
            get
            {
                return _QuantidadeOC;
            }
            set
            {
                if (_QuantidadeOC != value)
                {
                    _QuantidadeOC = value;
                }
            }
        }
    }
}
