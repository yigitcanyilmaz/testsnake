using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SnakeGameYigit
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        // Oyundaki yılanın parçalarını ve hedefi tutacak paneller
        Panel part; // Yılanın parçası
        Panel point = new Panel(); // Yılanın puan alacağı kare
        Panel pointCheck = new Panel(); // Yılanın alacağı puan karesini test etmek amacıyla bunu oluşturdum. Yılanın içinde point oluşmasın
        List<Panel> snake = new List<Panel>(); // Yılanın parçalarını takip etmesi için dizi atıyoruz

        string direction = "right"; // Başlangıç için hangi pozisyona gideceğini ayarlıyoruz
        int scoreshow = 0; // Skor gösterme değişkeni
        int secref = 0; // Nekadar sürede hareket edeceği
        double secref2 = 0; // 0.25 ve 0.5 değerleri olduğu için int'e çevirmek için double türünde alıyoruz
        int sizeX = 0; //X türünden oyun alanının büyüklüğü
        int sizeY = 0; //Y türünden oyun alanının büyüklüğü
        int maxscore = 0; // Hedeflenen puan



        private void label3_Click(object sender, EventArgs e)
        {
            panel5.Visible = false; //Skorları gösteren paneli saklıyoruz
            label3.Visible = false; //Start veren tuşu saklıyoruz
            Settings(); // Ayarları belirleyen void'i çalıştırdık
            panel4.Visible = false; // Skor kaydetme ve gösterme kısmını oyun esnasında saklıyoruz sadece oyun bitiminde görülebilir
            panel2.Visible = false; // Ayarlar kısmını saklıyoruz oyun bitince gözükebilir
            label2.Text = "0"; // Skor tablosunu 0 olarak ayarlıyoruz
            label4.Visible = false; // Kazandın/Kaybettin Label'ini saklıyoruz
            PanelClear(); // Önceden kalan veriler olabilir diye yılanın oynandığı paneli temizliyoruz.
            this.Focus(); // Radiobutton ve textboxlar olduğu için klavye komutları panele focuslanmıyor. Bu sayede oyun oynanabilir.
            part = new Panel(); // Yılanın başını girdik
            part.Location = new Point(sizeX-200, sizeY-200); // Yılanın başının oyun alanında spawnlanmasını sağlıyoruz (Örn. Eğer 600x600 alanda oyun oynanıyorsa 400x400 konumunda spawnlanacak)
            part.Size = new Size(20, 20); //Yılanın boyutunu 20x20 olarak belirledik
            part.BackColor = Color.Blue; //Yılanı mavi yaptık
            snake.Add(part); // Yılanı ekledik
            panel1.Controls.Add(snake[0]); // Panel1'e yılanı ekledik

            timer1.Start(); // Burada timer'i çalıştırıyoruz. Timer tool'u sayesinde geçen zamanda yılanı hareket ettirebiliriz
            CreatePoint(); // Bu void ile ilk hedeflediğimiz point'i oluşturuyoruz.
        }

        void Settings()
        {   
            sizeX = Convert.ToInt32(comboBox1.SelectedItem.ToString()); // Combobox1'de seçili olan X veriyi integer'e dönüştürdük ve sizeX'e bağladık
            sizeY = Convert.ToInt32(comboBox4.SelectedItem.ToString()); // Combobox4'de seçili olan Y veriyi integer'e dönüştürdük ve sizeY'e bağladık
            double.TryParse(comboBox2.SelectedItem.ToString(),out secref2); // Combobox2'deki seçili olan "HIZ" verisini aldık. (Double dönüştürmemizin sebebi 0.25 ve 0.50 seçilebilir olması) 
            secref = Convert.ToInt32(secref2 * 1000); // Timer'de 2 saniye 2000 olarak hesaplanıyor bundan ötürü aldığımız veriyi 1000 ile çarpıyoruz (Örn. 2 saniye için 2*1000=2000)
            maxscore = Convert.ToInt32(comboBox3.SelectedItem.ToString()); // Combobox3'den hedeflenen skoru alıyoruz
            panel1.Width = sizeX; // Combobox1'den seçtiğimiz veriyi panel1'in genişliği olarak seçiyoruz. Yani X
            panel1.Height = sizeY; // Combobox4'den seçtiğimiz veriyi panel1'in yüksekliği olarak aldık
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Bu fonksiyon seçilen süre zarfında hep tekrar edecek (Örn. 2 saniye seçildiyse 2 saniyede bir bu fonksiyon çalışacak)

            timer1.Interval = secref; //Settings'de aldığımız saniye/hareket değişkenini timer'e atıyoruz
            int locX = snake[0].Location.X; //Snake'nin X konumunu girdik
            int locY = snake[0].Location.Y; //Snake'nin Y konumunu girdik
            Stop(); // Yılanın kendini yiyip yemediği kontrolu
            PointUpdate(); // Point'in yiyip yenmediğinin kontrolü
            PointCheck(); // Hedeflenen skora ulaşılıp ulaşılmadığı
            Movement(); // Yılanı hareket ettirmek


            //Burada yılanımız eğer duvarı geçerse diğer köşesinden başlamasına yarıyor. 
            if (direction == "right") 
            {
                if (locX < sizeX-20)
                    locX += 20;
                else
                    locX = 0;
            }
            if (direction == "left")
            {
                if (locX > 0)
                    locX -= 20;
                else
                    locX = sizeX - 20;
            }
            if (direction == "down")
            {
                if (locY < sizeY - 20)
                    locY += 20;
                else
                    locY = 0;
            }
            if (direction == "up")
            {
                if (locY > 0)
                    locY -= 20;
                else
                    locY = sizeY - 20;
            }

            snake[0].Location = new Point(locX, locY);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Klavyede basılan tuşların kontrolü, buna göre yılanı hareket ettiriyoruz
            if(e.KeyCode == Keys.Right && direction!="left")
                direction = "right";
            if(e.KeyCode == Keys.Left && direction!="right")
                direction = "left";
            if (e.KeyCode == Keys.Up && direction!="down")
                direction = "up";
            if (e.KeyCode == Keys.Down && direction!="up")
                direction = "down";

        }

        void CreatePoint()
        {
            // Point oluşturma
            Random random = new Random();
            int pointX, pointY;
            pointX = random.Next(sizeX); // Alan içerisinde rastgele konum belirtiyoruz X ve Y olarak.
            pointY = random.Next(sizeY);

            pointX -= pointX % 20; //Köşeye denk gelir taşar diye böyle bir şey
            pointY -= pointY % 20;
            pointCheck.Location = new Point(pointX, pointY); // PointCheck diye yeni bir konuma atıyoruz test için
            for (int i = 0; i < snake.Count; i++)
            {
                if (snake[i].Location == pointCheck.Location) // Point'in atandığı yer yılan bölgsesi mi diye kontrol
                {
                    CreatePoint(); // Eğer yılanın içinde oluşuyorsa bu fonksiyonu tekrar çalıştır
                }
                else
                { 
                    point.Size = new Size(20, 20);
                    point.BackColor = Color.Red;
                    point.Location = new Point(pointX, pointY);
                    panel1.Controls.Add(point); // Eğer yılanın içinde değilse oluştur
                }
            }


        }
        void PointCheck()
        {
            // Hedeflenen puana ulaşıldığını kontrol
            if(scoreshow >= maxscore)
            {
                label3.Visible = true;
                label4.Text = "YOU WON";
                panel4.Visible = true;
                label4.Visible = true;
                scoreshow = 0;
                panel2.Visible = true;
                label4.ForeColor = Color.Green;
                timer1.Stop();
            }
        }
        void PointUpdate()
        {
            // Point alındıkça puan ekle
            int score = int.Parse(label2.Text);
            if (snake[0].Location == point.Location)
            {
                panel1.Controls.Remove(point);
                score += 25;
                scoreshow = score;
                label2.Text = score.ToString();
                CreatePoint();
                AddPart();
            }
        }
          
        void AddPart()
        {
            //Yılana parça ekle
            Panel addpart = new Panel();
            addpart.Size = new Size(20, 20);
            addpart.BackColor = Color.Blue;
            snake.Add(addpart);
            panel1.Controls.Add(addpart);
        }

        void Movement() 
        {
            //Yılanı hareket ettir
            for(int i= snake.Count-1; i>0; i--)
            {
                snake[i].Location = snake[i-1].Location;
            }
        }

        void Stop()
        {
            //Yılanın kendini yiyip yemediğini kontrol
            for(int i=2; i< snake.Count; i++)
            {
                if (snake[0].Location == snake[i].Location)
                {
                    label3.Visible = true;
                    label4.Visible = true;
                    label4.ForeColor = Color.Red;
                    panel2.Visible = true;
                    panel4.Visible = true;
                    label4.Text = "YOU LOSE";
                    scoreshow = 0;
                    timer1.Stop();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Uygulama başlayınca otomatik kural ekleme
            comboBox1.SelectedIndex = 2;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 2;
        }


        void PanelClear()
        {
            // Oyunun oynandığı alanı ve yılan bilgilerini sıfırla (Oyun yeniden başlarken yapılır)
            panel1.Controls.Clear();
            snake.Clear();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Advanced settings seçildiğinde radiobuttonlar false kalsın
            if(checkBox2.Checked==true)
            {
                panel3.Visible = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
            else
            {
                radioButton8.Checked = true;
                panel3.Visible = false;
            }
        }

        //Radiobuttonlardan biri seçildiğinde kurallar atansın ve advanced settings false olsun
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton8.Checked==true)
            {
                comboBox1.SelectedIndex=2;
                comboBox4.SelectedIndex=2;
                comboBox2.SelectedIndex=0;
                comboBox3.SelectedIndex=0;
                checkBox2.Checked = false;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                comboBox1.SelectedIndex = 2;
                comboBox4.SelectedIndex = 2;
                comboBox2.SelectedIndex = 1;
                comboBox3.SelectedIndex = 1;
                checkBox2.Checked = false;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked == true)
            {
                comboBox1.SelectedIndex = 2;
                comboBox4.SelectedIndex = 2;
                comboBox2.SelectedIndex = 2;
                comboBox3.SelectedIndex = 2;
                checkBox2.Checked = false;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)
            {
                comboBox1.SelectedIndex = 1;
                comboBox4.SelectedIndex = 1;
                comboBox2.SelectedIndex = 3;
                comboBox3.SelectedIndex = 3;
                checkBox2.Checked = false;
            }
        }

        // Burada skoru kaydetme var, isim yazılıp yazılmadığını kontrol ediyor.
        int score;
        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != null && !string.IsNullOrWhiteSpace(textBox1.Text))
            {
                score = Convert.ToInt32(label2.Text);
                SaveScore(textBox1.Text, score);
                MessageBox.Show("Succesful !!");
            }
            else
            {
                MessageBox.Show("Please Write Your Name!!");
            }
        }

        static void SaveScore(string name, int score)
        {
            // XML dosyasını yükle
            XmlDocument xmlDoc = new XmlDocument();
            string xmlFilePath = "scores.xml";

            if (System.IO.File.Exists(xmlFilePath))
            {
                xmlDoc.Load(xmlFilePath);
            }
            else
            {
                // Eğer XML dosyası yoksa oluştur
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(xmlDoc.CreateElement("Scores"));
            }

            // Yeni skor ve isim bilgisini ekleyerek kaydet
            XmlElement skorElementi = xmlDoc.CreateElement("Skor");
            skorElementi.SetAttribute("Name", name);
            skorElementi.SetAttribute("Score", score.ToString());

            xmlDoc.DocumentElement.AppendChild(skorElementi);
            xmlDoc.Save(xmlFilePath);
        }

        // Show List buttonu
        private void button1_Click(object sender, EventArgs e)
        {
            panel5.Visible = true;
            listBox1.Items.Clear();
            LoadAndSortScores();

        }

        // Listbox1'e xml'deki verileri sıralıyoruz puanlarına göre
        private void LoadAndSortScores()
        {
            // XML dosyasından skorları çek
            List<(string, int)> scoreList = GetScores();

            // Skorları sırala
            scoreList = scoreList.OrderByDescending(x => x.Item2).ToList();

            // ListBox'a ekleyerek göster
            foreach (var score in scoreList)
            {
                listBox1.Items.Add($"{score.Item1}: {score.Item2}");
            }
        }

        // Burada list oluşturup xml'den çekilen verileri sıralıyoruz.
        private List<(string, int)> GetScores()
        {
            List<(string, int)> scoreList = new List<(string, int)>();
            XmlDocument xmlDoc = new XmlDocument();

            // XML dosyasını yükle
            if (System.IO.File.Exists("scores.xml"))
            {
                xmlDoc.Load("scores.xml");

                // Her "Skor" elementini döngüye alarak veriyi çek
                foreach (XmlNode scoreNode in xmlDoc.SelectNodes("//Skor"))
                {
                    string name = scoreNode.Attributes["Name"].Value;
                    int score = Convert.ToInt32(scoreNode.Attributes["Score"].Value);

                    scoreList.Add((name, score));
                }
            }

            return scoreList;
        }

        // Showlist sekmesini kapat
        private void button3_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
        }
    }
}
