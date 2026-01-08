using FonYonetim.Concrete;
using FonYonetim.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FonYonetim
{
    public partial class Form1 : Form
    {
        public void DataGridListele()
        {
            dataGridView2.DataSource = c.StockMarkets.OrderBy(x => x.StockMarketTargetPricePercent).ToList();
        }

        public void NasdaqDataGridListele()
        {
            dataGridView1.DataSource = c.Nasdaqs.OrderByDescending(x => x.NasdaqMarketTargetPricePercent).ToList();
        }

        public void BistDataGridListele()
        {
            dataGridView3.DataSource = c.Bists.OrderByDescending(x => x.BistMarketTargetPricePercent).ToList();
        }

        public void BistDolarDataGridListele()
        {
            dataGridView4.DataSource = c.BistDolars.OrderByDescending(x => x.BistMarketTargetPricePercent).ToList();
        }

        public Form1()
        {
            InitializeComponent();
        }

        Context c = new Context();
        Model model = new Model();

        private void button1_Click(object sender, EventArgs e)
        {
            // API verilerini çekeceğimiz endpoint'in URL'si
            string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1&sparkline=false";

            // Belirtilen URL'ye HTTP isteği oluşturma
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // API'ye isteği gönderme ve cevap alma
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Cevap akışının içeriğini okuyup bir dizeye dönüştürme
            string content = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // JSON verilerini deserialize etmek için JavaScriptSerializer nesnesi oluşturma
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            // JSON verilerini BitCoin nesnelerinin bir listesine deserialize etme
            List<Bitcoin> jsonObject = serializer.Deserialize<List<Bitcoin>>(content);

            //StockMarket tablomdaki verileri modele aktardım
            model.StockMarketList = c.StockMarkets.ToList();

            //Döngü ile tablodaki tüm verileri gezip anlık fiyatlarını güncelledim ve hedef fiyata yakınlık olayını hallettim
            foreach (var item in model.StockMarketList)
            {
                item.StockMarketPrice = jsonObject.Where(x => x.symbol == item.StockMarketSymbol).Select(x => x.current_price).FirstOrDefault();

                if (item.StockMarketPrice != 0)//Apiden fiyatları alabilmişse hedef fiyata olan yakınlığı hesapla
                {
                    item.StockMarketTargetPricePercent = Convert.ToDouble((((item.StockMarketTargetPrice / item.StockMarketPrice) - 1) * 100).ToString("0.00")); //.ToString("0.00")
                }
                else
                {
                    item.StockMarketTargetPricePercent = 0; //apiden fiyatı alamadıysan hedef fiyata yakınlığı hesaplamana gerek yok 0 gönder yeter
                }

                c.StockMarkets.Update(item);
                c.SaveChanges();
            }

            DataGridListele();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Textbox1 e girilen symbol veri tabanında varmı diye bakıyorum, varsa product'ın içine koyuyorum
            var product = c.StockMarkets.Where(x => x.StockMarketSymbol == textBox1.Text).FirstOrDefault();

            //Girilen Symbol veri tabanında varsa hedef fiyatı günceller, yoksa yeni kayıt açar.
            if (product != null)
            {
                //sadece hedef fiyatı güncelliyorum diğer veriler aynı kalacak
                product.StockMarketTargetPrice = Convert.ToDouble(textBox2.Text);
                c.StockMarkets.Update(product);
                c.SaveChanges();
            }
            else
            {
                //izleme listesine yeni bir kayıt oluşturuyorum
                StockMarket sm = new StockMarket();
                sm.StockMarketSymbol = textBox1.Text;
                sm.StockMarketPrice = 0;
                sm.StockMarketTargetPrice = Convert.ToDouble(textBox2.Text);
                sm.StockMarketTargetPricePercent = 0;
                c.StockMarkets.Add(sm);
                c.SaveChanges();
            }

            //yenileme butonunu tetikliyorum ki güncel fiyat bilgisi her veri eklendiğinde
            button1_Click(sender, e);

        }

        public class ExchangeRates
        {
            public string Base { get; set; }
            public DateTime Date { get; set; }
            public Dictionary<string, double> Rates { get; set; }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridListele();
            NasdaqDataGridListele();
            BistDataGridListele();
            BistDolarDataGridListele();


            // Döviz oranları için sınıf

            try
            {
                // API'den döviz kurlarını almak için kullanılan URL
                string apiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

                // Web isteği oluşturma
                WebClient client = new WebClient();
                string response = client.DownloadString(apiUrl);

                // JSON yanıtını döviz oranları nesnesine dönüştürme
                ExchangeRates exchangeRates = JsonConvert.DeserializeObject<ExchangeRates>(response);

                // Türk Lirası'nın dolar karşısındaki kuru
                double tryRate = exchangeRates.Rates["TRY"];

                label10.Text = tryRate.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir hata oluştu: " + ex.Message);
                label10.Text = "Hata";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //datagridden seçili satırın ID ve Symbol değerlerini alıyorum
            int ID = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value);
            string Symbol = dataGridView2.CurrentRow.Cells[1].Value.ToString();

            //Veri silinsin mi silinmesin mi diye soruyorum
            if (MessageBox.Show(Symbol + " verisini silmek istediğinizden emin misiniz?", "Bu Veri Silinecek?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Veritabanından ilgili ID nin verisini siliyorum
                var silinceksatir = c.StockMarkets.Find(ID);
                c.StockMarkets.Remove(silinceksatir);
                c.SaveChanges();
            }
            else
            {
                //hiçbir işlem yapılmayacak
            }

            DataGridListele();

        }



        private void button7_Click(object sender, EventArgs e)
        {
            //Textbox1 e girilen symbol veri tabanında varmı diye bakıyorum, varsa product'ın içine koyuyorum
            var nasdaq = c.Nasdaqs.Where(x => x.NasdaqMarketSymbol == textBox4.Text).FirstOrDefault();

            //Girilen Symbol veri tabanında varsa hedef fiyatı günceller, yoksa yeni kayıt açar.
            if (nasdaq != null)
            {
                //sadece hedef fiyatı güncelliyorum diğer veriler aynı kalacak
                nasdaq.NasdaqMarketTargetPrice = Convert.ToDouble(textBox5.Text);
                c.Nasdaqs.Update(nasdaq);
                c.SaveChanges();
            }
            else
            {
                //izleme listesine yeni bir kayıt oluşturuyorum
                Nasdaq sm = new Nasdaq();
                sm.NasdaqMarketSymbol = textBox4.Text;
                sm.NasdaqMarketPrice = 0;
                sm.NasdaqMarketTargetPrice = Convert.ToDouble(textBox5.Text);
                sm.NasdaqMarketTargetPricePercent = 0;
                c.Nasdaqs.Add(sm);
                c.SaveChanges();
            }

            //yenileme butonunu tetikliyorum ki güncel fiyat bilgisi her veri eklendiğinde
            NasdaqDataGridListele();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //datagridden seçili satırın ID ve Symbol değerlerini alıyorum
            int ID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            string Symbol = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            //Veri silinsin mi silinmesin mi diye soruyorum
            if (MessageBox.Show(Symbol + " verisini silmek istediğinizden emin misiniz?", "Bu Veri Silinecek?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Veritabanından ilgili ID nin verisini siliyorum
                var silinceksatir = c.Nasdaqs.Find(ID);
                c.Nasdaqs.Remove(silinceksatir);
                c.SaveChanges();
            }
            else
            {
                //hiçbir işlem yapılmayacak
            }

            NasdaqDataGridListele();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            //listeyi getir
            var nasdaqListe = c.Nasdaqs.OrderBy(x => x.LastUpdateDate).Take(25).ToList();
            //    var nasdaqListe = c.Nasdaqs.Where(x => x.NasdaqMarketTargetPricePercent <= -80).ToList();  // < ise uzakları  > ise hedefe yakın olanları gösterir
            int sayac = 0;
            string apiKey = "QMR45NCCCEF7ANST";
            //string apiKey = "C7SUCO1R4KJ9D01L";

            using (WebClient client = new WebClient())
            {
                foreach (var sirket in nasdaqListe)
                {
                    //fiyat verisini al
                    string symbol = sirket.NasdaqMarketSymbol; //Sembol
                    string interval = "60min"; //Veri zamanı 5min vs..
                    string QUERY_URL = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&interval={interval}&apikey={apiKey}"; //api cümlesi
                    Uri queryUri = new Uri(QUERY_URL);
                    string json_dataa = client.DownloadString(queryUri);
                    JObject jsonData = JObject.Parse(json_dataa);
                    var priceStr = jsonData["Global Quote"]["05. price"].ToString();

                    //fiyat verisini kendime uygun formata çeviriyorum Api den bana istediğim gibi gelmiyor
                    double fiyat = double.Parse(priceStr.ToString());  //fiyat verisini double değişkene aldım
                    double yeniFiyat = double.Parse(fiyat.ToString().Remove(fiyat.ToString().Length - 2) + "00");  //fiyatın son iki hanesi ne olursa olsun silip çift sıfır ekliyorum ki aşağıdaki 100 e bölme işleminde küsürat çıkmasın.
                    double newFiyat = double.Parse(yeniFiyat.ToString("F2")) / 100; //son iki hanesini sildim çünkü fazla geliyordu ve onu 100 e böldüm
                    string newFiyatNoktalı = newFiyat.ToString().Insert(newFiyat.ToString().Length - 2, ",");

                    //fiyat verisini sql e yaz
                    sirket.NasdaqMarketPrice = double.Parse(newFiyatNoktalı);
                    sirket.NasdaqMarketTargetPricePercent = Convert.ToDouble((((sirket.NasdaqMarketTargetPrice / sirket.NasdaqMarketPrice) - 1) * 100).ToString("0.00"));
                    sirket.LastUpdateDate = DateTime.Now;
                    c.Nasdaqs.Update(sirket);
                    c.SaveChanges();

                    //Güncellenenleri listele
                    NasdaqDataGridListele();

                    //progressbar da ne seviyede olduğumuza bakalım
                    double artisDegeri = 100.0 / nasdaqListe.Count();
                    if ((progressBar1.Value + artisDegeri) <= 100)
                    {
                        progressBar1.Value += Convert.ToInt32(Math.Round(artisDegeri));
                    }
                    sayac++;

                    textBox3.Text = "Toplam Veri Sayısı : " + nasdaqListe.Count() + " Yüklenen : " + sayac.ToString();

                    //Her requestten sonra bekliyorum ki 
                    await Task.Delay(19000); // 19 saniye beklemek için
                }

            }

            //Tüm işlemler bittiyse bittiğini görebileyim
            progressBar1.Value = 99;
            textBox3.Text += " Bitti...";

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        //Bist Listele
        public Uri url;
        public string html;
        private void button4_Click(object sender, EventArgs e)
        {
            url = new Uri("http://bigpara.hurriyet.com.tr/borsa/hisse-senetleri/");

            //8 WEB SAYFASI İÇİN 8'LİK DÖNGÜ
            for (int sayfa = 1; sayfa <= 7; sayfa++)
            {
                try
                {
                    url = new Uri("http://bigpara.hurriyet.com.tr/borsa/hisse-fiyatlari/" + sayfa + "/");
                }
                catch (UriFormatException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                catch (ArgumentException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                WebClient client2 = new WebClient();
                client2.Encoding = Encoding.UTF8;
                try
                {
                    html = client2.DownloadString(url);
                }
                catch (WebException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                doc2.LoadHtml(html);
                //45 SATIR İÇİN 45'LİK DÖNGÜ
                try
                {
                    string HisseAdi;
                    string GüncelFiyat;
                    for (int say = 1; say <= 45; say++)
                    {
                        try
                        {
                            HisseAdi = (doc2.DocumentNode.SelectSingleNode("//*[@id='content']/div[2]/div[6]/div/div/div[2]/ul[" + say + "]/li[1]").InnerText);
                            GüncelFiyat = ((doc2.DocumentNode.SelectSingleNode("//*[@id='content']/div[2]/div[6]/div/div/div[2]/ul[" + say + "]/li[2]").InnerText));

                            var sirket = c.Bists.Where(x => x.BistMarketSymbol == HisseAdi).FirstOrDefault();

                            if (sirket != null)
                            {
                                sirket.BistMarketPrice = double.Parse(GüncelFiyat);
                                sirket.BistMarketTargetPricePercent = Convert.ToDouble((((sirket.BistMarketTargetPrice / sirket.BistMarketPrice) - 1) * 100).ToString("0.00"));
                                c.Bists.Update(sirket);
                                c.SaveChanges();
                            }

                            if (HisseAdi == "ZOREN")
                            {
                                say = 46; //zoren son şirket ondan sonra devam etmeye gerek yok. alfabetik gidiyor.
                            }
                        }
                        catch
                        {
                            //Son sayfada 45 satır yok. Hata verirse hiçbir şey yapma!!
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    if (MessageBox.Show("Hatalı xPath", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
            }

            BistDataGridListele();
        }

        //Bist Ekle
        private void button9_Click(object sender, EventArgs e)
        {
            //Textbox1 e girilen symbol veri tabanında varmı diye bakıyorum, varsa product'ın içine koyuyorum
            var bist = c.Bists.Where(x => x.BistMarketSymbol == textBox6.Text).FirstOrDefault();

            //Girilen Symbol veri tabanında varsa hedef fiyatı günceller, yoksa yeni kayıt açar.
            if (bist != null)
            {
                //sadece hedef fiyatı güncelliyorum diğer veriler aynı kalacak
                bist.BistMarketTargetPrice = Convert.ToDouble(textBox7.Text);
                c.Bists.Update(bist);
                c.SaveChanges();
            }
            else
            {
                //izleme listesine yeni bir kayıt oluşturuyorum
                Bist sm = new Bist();
                sm.BistMarketSymbol = textBox6.Text;
                sm.BistMarketPrice = 0;
                sm.BistMarketTargetPrice = Convert.ToDouble(textBox7.Text);
                sm.BistMarketTargetPricePercent = 0;
                c.Bists.Add(sm);
                c.SaveChanges();
            }

            //yenileme butonunu tetikliyorum ki güncel fiyat bilgisi her veri eklendiğinde
            BistDataGridListele();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //datagridden seçili satırın ID ve Symbol değerlerini alıyorum
            int ID = Convert.ToInt32(dataGridView3.CurrentRow.Cells[0].Value);
            string Symbol = dataGridView3.CurrentRow.Cells[1].Value.ToString();

            //Veri silinsin mi silinmesin mi diye soruyorum
            if (MessageBox.Show(Symbol + " verisini silmek istediğinizden emin misiniz?", "Bu Veri Silinecek?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Veritabanından ilgili ID nin verisini siliyorum
                var silinceksatir = c.Bists.Find(ID);
                c.Bists.Remove(silinceksatir);
                c.SaveChanges();
            }
            else
            {
                //hiçbir işlem yapılmayacak
            }

            BistDataGridListele();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //Textbox1 e girilen symbol veri tabanında varmı diye bakıyorum, varsa product'ın içine koyuyorum
            var bist = c.BistDolars.Where(x => x.BistMarketSymbol == textBox8.Text).FirstOrDefault();

            //Girilen Symbol veri tabanında varsa hedef fiyatı günceller, yoksa yeni kayıt açar.
            if (bist != null)
            {
                //sadece hedef fiyatı güncelliyorum diğer veriler aynı kalacak
                bist.BistMarketTargetPriceDolar = Convert.ToDouble(textBox9.Text);
                c.BistDolars.Update(bist);
                c.SaveChanges();
            }
            else
            {
                //izleme listesine yeni bir kayıt oluşturuyorum
                BistDolar sm = new BistDolar();
                sm.BistMarketSymbol = textBox8.Text;
                sm.BistMarketPriceDolar = 0;
                sm.BistMarketTargetPriceDolar = Convert.ToDouble(textBox9.Text);
                sm.BistMarketTargetPricePercent = 0;
                c.BistDolars.Add(sm);
                c.SaveChanges();
            }

            //yenileme butonunu tetikliyorum ki güncel fiyat bilgisi her veri eklendiğinde
            BistDolarDataGridListele();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //datagridden seçili satırın ID ve Symbol değerlerini alıyorum
            int ID = Convert.ToInt32(dataGridView4.CurrentRow.Cells[0].Value);
            string Symbol = dataGridView4.CurrentRow.Cells[1].Value.ToString();

            //Veri silinsin mi silinmesin mi diye soruyorum
            if (MessageBox.Show(Symbol + " verisini silmek istediğinizden emin misiniz?", "Bu Veri Silinecek?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Veritabanından ilgili ID nin verisini siliyorum
                var silinceksatir = c.BistDolars.Find(ID);
                c.BistDolars.Remove(silinceksatir);
                c.SaveChanges();
            }
            else
            {
                //hiçbir işlem yapılmayacak
            }

            BistDolarDataGridListele();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            url = new Uri("http://bigpara.hurriyet.com.tr/borsa/hisse-senetleri/");

            double dolarFiyati = Convert.ToDouble(label10.Text);

            //8 WEB SAYFASI İÇİN 8'LİK DÖNGÜ
            for (int sayfa = 1; sayfa <= 7; sayfa++)
            {
                try
                {
                    url = new Uri("http://bigpara.hurriyet.com.tr/borsa/hisse-fiyatlari/" + sayfa + "/");
                }
                catch (UriFormatException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                catch (ArgumentException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                WebClient client2 = new WebClient();
                client2.Encoding = Encoding.UTF8;
                try
                {
                    html = client2.DownloadString(url);
                }
                catch (WebException)
                {
                    if (MessageBox.Show("Hatalı Url", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
                HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                doc2.LoadHtml(html);
                //45 SATIR İÇİN 45'LİK DÖNGÜ
                try
                {
                    string HisseAdi;
                    string GüncelFiyat;
                    for (int say = 1; say <= 45; say++)
                    {
                        try
                        {

                            HisseAdi = (doc2.DocumentNode.SelectSingleNode("//*[@id='content']/div[2]/div[6]/div/div/div[2]/ul[" + say + "]/li[1]").InnerText);
                            GüncelFiyat = ((doc2.DocumentNode.SelectSingleNode("//*[@id='content']/div[2]/div[6]/div/div/div[2]/ul[" + say + "]/li[2]").InnerText));

                            var sirket = c.BistDolars.Where(x => x.BistMarketSymbol == HisseAdi).FirstOrDefault();

                            if (sirket != null)
                            {
                                sirket.BistMarketPriceDolar = Math.Round(double.Parse(GüncelFiyat) / dolarFiyati, 2);
                                sirket.BistMarketTargetPricePercent = Convert.ToDouble((((sirket.BistMarketTargetPriceDolar / sirket.BistMarketPriceDolar) - 1) * 100).ToString("0.00"));
                                c.BistDolars.Update(sirket);
                                c.SaveChanges();
                            }

                            if (HisseAdi == "ZOREN")
                            {
                                say = 46; //zoren son şirket ondan sonra devam etmeye gerek yok. alfabetik gidiyor.
                            }
                        }
                        catch
                        {
                            //Son sayfada 45 satır yok. Hata verirse hiçbir şey yapma!!
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    if (MessageBox.Show("Hatalı xPath", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                    }
                }
            }

            BistDolarDataGridListele();
        }
    }
}
