using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await CarregarHistoricoAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar histórico: {ex}");
                await DisplayAlert("Erro", "Falha ao carregar histórico", "OK");
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    lbl_res.Text = "Preencha a cidade.";
                    return;
                }

                Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);
                if (t == null)
                {
                    lbl_res.Text = "Sem dados de Previsão";
                    return;
                }

                lbl_res.Text = $"Latitude: {t.lat}\n" +
                              $"Longitude: {t.lon}\n" +
                              $"Nascer do Sol: {t.sunrise}\n" +
                              $"Por do Sol: {t.sunset}\n" +
                              $"Temp Máx: {t.temp_max}\n" +
                              $"Temp Min: {t.temp_min}\n";

                wv_mapa.Source =
                    $"https://embed.windy.com/embed.html?" +
                    $"type=map&location=coordinates&metricRain=mm&" +
                    $"metricTemp=ºC&metricWind=km/h&zoom=4&overlay=wind&" +
                    $"product=ecmwf&level=surface" +
                    $"&lat={t.lat?.ToString().Replace(",", ".")}" +
                    $"&lon={t.lon?.ToString().Replace(",", ".")}";

                var historico = new HistoricoPesquisa
                {
                    Termo = txt_cidade.Text!,
                    Latitude = t.lat.HasValue ? t.lat.Value.ToString() : string.Empty,
                    Longitude = t.lon.HasValue ? t.lon.Value.ToString() : string.Empty,
                    TempMax = t.temp_max.HasValue ? t.temp_max.Value.ToString() : string.Empty,
                    TempMin = t.temp_min.HasValue ? t.temp_min.Value.ToString() : string.Empty,
                    Sunrise = !string.IsNullOrEmpty(t.sunrise) ? t.sunrise : string.Empty,
                    Sunset = !string.IsNullOrEmpty(t.sunset) ? t.sunset : string.Empty,
                    DataPesquisa = DateTime.Now
                };

                try
                {
                    await App.Database.AdicionarPesquisaAsync(historico);
                }
                catch (Exception dbEx)
                {
                    Debug.WriteLine($"Erro ao salvar histórico: {dbEx}");
                    await DisplayAlert("Erro", "Falha ao salvar histórico", "OK");
                }

                try
                {
                    await CarregarHistoricoAsync();
                }
                catch (Exception loadEx)
                {
                    Debug.WriteLine($"Erro ao recarregar histórico: {loadEx}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro geral: {ex}");
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private void OnMapaHistoricoClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is MauiAppTempoAgora.Models.HistoricoPesquisa item)
            {
                string url = $"https://embed.windy.com/embed.html?" +
                             $"type=map&location=coordinates&metricRain=mm&metricTemp=ºC&metricWind=km/h&zoom=4&overlay=wind&product=ecmwf&level=surface" +
                             $"&lat={item.Latitude?.Replace(",", ".")}" +
                             $"&lon={item.Longitude?.Replace(",", ".")}";

                wv_mapa.Source = url;
            }
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var local = await Geolocation.Default.GetLocationAsync(request);
                if (local != null)
                {
                    lbl_coords.Text = $"Latitude: {local.Latitude}\nLongitude: {local.Longitude}";
                    await GetCidadeAsync(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Nenhuma localização.";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão de localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro geolocalização: {ex}");
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task GetCidadeAsync(double lat, double lon)
        {
            try
            {
                var places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                var place = places.FirstOrDefault();
                if (place != null)
                    txt_cidade.Text = place.Locality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao obter cidade: {ex}");
                await DisplayAlert("Erro ao obter cidade", "Não foi possível obter o nome da cidade.", "OK");
            }
        }

        private async Task CarregarHistoricoAsync()
        {
            var historico = await App.Database.ObterHistoricoAsync();
            cv_historico.ItemsSource = historico;
        }

        private async void BtnBuscarHistorico_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Busca todo o histórico e filtra em memória para evitar erros no método SQLite
                var todos = await App.Database.ObterHistoricoAsync();
                var filtrado = todos
                    .Where(h => h.DataPesquisa.Date == datePicker.Date)
                    .ToList();

                cv_historico.ItemsSource = filtrado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao filtrar histórico em memória: {ex}");
                await DisplayAlert("Erro", "Não foi possível filtrar o histórico.", "OK");
            }
        }
    }
}
