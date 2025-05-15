using SQLite;
using System;

namespace MauiAppTempoAgora.Models
{
    public class HistoricoPesquisa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Termo { get; set; }
        public DateTime DataPesquisa { get; set; } = DateTime.Now;

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TempMax { get; set; }
        public string TempMin { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
    }
}
