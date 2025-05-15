using SQLite;
using MauiAppTempoAgora.Models;

namespace MauiAppTempoAgora.Services
{
    public class HistoricoDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public HistoricoDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<HistoricoPesquisa>().Wait();
        }

        public Task<int> AdicionarPesquisaAsync(HistoricoPesquisa pesquisa)
        {
            return _database.InsertAsync(pesquisa);
        }

        public Task<List<HistoricoPesquisa>> ObterHistoricoAsync()
        {
            return _database
                .Table<HistoricoPesquisa>()
                .OrderByDescending(h => h.DataPesquisa)
                .Take(10)
                .ToListAsync();
        }

        public Task<List<HistoricoPesquisa>> BuscarPorDataAsync(DateTime data)
        {
            return _database.Table<HistoricoPesquisa>()
                .Where(h => h.DataPesquisa.Date == data.Date)
                .ToListAsync();
        }
    }
}
