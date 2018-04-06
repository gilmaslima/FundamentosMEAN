/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Modelo.Structure;


namespace Rede.PN.AtendimentoDigital.Modelo.Agente
{
    /// <summary>
    /// Interface para o agente DRCEP
    /// </summary>
    public interface IAgenteEnderecoCep : IAgenteListarObter<EntidadeEnderecoCep, EnderecoCepChave>
    {
        EntidadeEnderecoCep BuscarEnderecoPorCep(String cep);
    }
}
