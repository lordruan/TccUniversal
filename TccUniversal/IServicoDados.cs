using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TccUniversal
{
    public interface IServicoDados
    {
        Task InicializarBaseDeDados(int versao);
        SQLiteAsyncConnection Contexto
        {
            get;
        }
    }
}
