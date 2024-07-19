using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Timer = System.Threading.Timer;
using System.Text.Json.Serialization;
using static Currency.Form1;



namespace Currency
{
    
    public partial class Form1 : Form
    {
        //Check time for taking rates every 10 hours
        byte CurrentHour = (byte)DateTime.Now.Hour;

        static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            
            await GetExchangeRates();
            
            
            
        }

        public class UsdData
        {
            public Rates Rates { get; set; }
        }

        public class Rates
        {
            public float RUB { get; set; }
            public decimal BTC { get; set; }

        }

        private async Task GetExchangeRates()
        {
            if (CurrentHour % 10 == 0)
            {
                string apiKey = "YOUR API KEY"; // Insert your api Key here
                string url = $"https://openexchangerates.org/api/latest.json?app_id={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var usdData = JsonSerializer.Deserialize<UsdData>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (usdData != null && usdData.Rates != null)
                {
                    // Output rates for labels
                    label4.Text = $"USD to RUB: {usdData.Rates.RUB}";
                    label5.Text = $"BTC to RUB: {1 / usdData.Rates.RUB * usdData.Rates.RUB}";
                }
                else
                {
                    MessageBox.Show("UserData or rates are null.");
                }
            }
            else
            {
                string latestFileUrl = File.ReadAllText(@"latest.json");
                var ParsedExistingFile = JsonSerializer.Deserialize<UsdData>(latestFileUrl, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (ParsedExistingFile != null && ParsedExistingFile.Rates != null)
                {
                    // Output rates for labels
                    label4.Text = $"USD to RUB: {ParsedExistingFile.Rates.RUB}";
                    label5.Text = $"BTC to RUB: {1 / ParsedExistingFile.Rates.RUB * ParsedExistingFile.Rates.RUB}";
                }
                else
                {
                    MessageBox.Show("UserData or rates are null.");
                }


            }
            
        }

        private void TakeRates()
        {

        }

    }
}
