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
        //Check time 
        byte CurrentHour = (byte)DateTime.Now.Hour;

        //Make Http client
        static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            //Settings for displaying in form
            label6.Text = $"Current Time: {DateTime.Now}";

            label4.Text = "USD to RUB: ";
            label5.Text = "BTC to RUB: ";

            pictureBox1.Width = 50;
            pictureBox1.Height = 50;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Width = 50;
            pictureBox2.Height = 50;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //Request for API in appropriate time
            byte ChosenTimeInterval = (byte)numericUpDown1.Value;
            await GetExchangeRates(ChosenTimeInterval);

        }

        //Classes for JSON doc form API
        public class UsdData
        {
            public Rates Rates { get; set; }
        }

        public class Rates
        {
            public float RUB { get; set; }
            public float BTC { get; set; }
        }


        // Function for API
        private async Task GetExchangeRates(byte ChosenTimeInterval)
        {

            //Case 1: Interval checking

            if (CurrentHour % ChosenTimeInterval == 0)
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
                    label5.Text = $"BTC to RUB: {(1 / usdData.Rates.BTC) * usdData.Rates.RUB}";
                }
                else
                {
                    MessageBox.Show("UserData or rates are null.");
                }
            }

            //Case 2: checking from json doc in project folder

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
                    string BTCRate = Convert.ToString((1 / ParsedExistingFile.Rates.BTC) * ParsedExistingFile.Rates.RUB);
                    label5.Text = $"BTC to RUB: {string.Format("{0:#,###0.#}", BTCRate)}";
                }
                else
                {
                    MessageBox.Show("UserData or rates are null.");
                }


            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            label7.Text = $"Chosen interval: every {numericUpDown1.Value} hours";
        }

        // Button for instant api request
        private async void button2_Click(object sender, EventArgs e)
        {
            await GetExchangeRates(1); // Argument given for execution Case 1
        }
    }
}
