using System;
using System.Collections.Generic;
using DTO;

namespace DataBase.Interfaces
{
    public interface ICarregadorDeAtivo: IDisposable
    {
        IEnumerable<AtivoSelecao> Carregar();
    }
}