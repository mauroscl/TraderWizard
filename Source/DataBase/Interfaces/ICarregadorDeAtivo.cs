using System;
using System.Collections.Generic;
using prjDTO;

namespace DataBase.Interfaces
{
    public interface ICarregadorDeAtivo: IDisposable
    {
        IEnumerable<AtivoSelecao> Carregar();
    }
}