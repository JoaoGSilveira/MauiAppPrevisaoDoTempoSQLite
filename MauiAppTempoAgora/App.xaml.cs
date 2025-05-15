using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora;

public partial class App : Application
{
    private static HistoricoDatabase? _database;

    public static HistoricoDatabase Database
    {
        get
        {
            if (_database == null)
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "historico.db3");
                _database = new HistoricoDatabase(path);
            }
            return _database;
        }
    }

    public App()
    {
        InitializeComponent(); // <- Esse método vem do XAML
        MainPage = new AppShell(); // ou MainPage() se você não estiver usando Shell
    }
}
