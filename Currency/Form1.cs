using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Currency
{
    public partial class Form1 : Form
    {
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
            public double RUB { get; set; }
        }

        private async Task GetExchangeRates()
        {
            string apiKey = "9842cc0490474826a1c02d8eb0947ba6"; // ��� API ����
            string url = $"https://openexchangerates.org/api/latest.json?app_id={apiKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var usdData = JsonSerializer.Deserialize<UsdData>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (usdData != null && usdData.Rates != null)
                {
                    // ��������� Label �� �����
                    label4.Text = $"USD to RUB: {usdData.Rates.RUB}";
                }
                else
                {
                    MessageBox.Show("�� ������� �������� ����� �����.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}");
            }
        }
    }
}
