using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ders_Kayıt_Sistemi
{
    public partial class ogrenciEklemeEkrani : Form
    {
        public ogrenciEklemeEkrani()
        {
            InitializeComponent();
            // ToolTip nesnesi oluştur
            ToolTip toolTip1 = new ToolTip();
            toolTip1.BackColor = Color.FromArgb(0, 39, 53);
            toolTip1.ForeColor = Color.Cyan;
            toolTip1.IsBalloon = true;
            toolTip1.UseAnimation = true;
            toolTip1.UseFading = true;
            toolTip1.InitialDelay = 100;
            toolTip1.SetToolTip(txtDersKodGuncelle, "Ders kodu girmek için, 'Alınan Dersler' tablosundaki ders koduna çift tıklayın.");
            toolTip1.SetToolTip(txtDersKod, "Ders kodu girmek için, 'Dersler' tablosundaki ders koduna çift tıklayın.");
            toolTip1.SetToolTip(txtDersKodEkle, "Ders kodu girmek için, 'Dersler' tablosundaki ders koduna çift tıklayın.");

        }
        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432; Database=dersis; user ID=postgres; password=1723");


        //*********************************//Ogrenci KAYIT EKRANI//**********************************
        private void dataOgrenciBilgi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtTcKontrol.Text = dataOgrenciBilgi.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            int rowIndex = e.RowIndex;
            DataGridViewRow selectedRow = dataOgrenciBilgi.Rows[rowIndex];
            txtTcKontrol.Text = selectedRow.Cells[0].Value.ToString();
        }

        private void btnOgrenciKaydet_Click(object sender, EventArgs e)
        {
            //aynı Ogrenci bir daha kaydedilmesin
            string tckimlikNumarasi = txtTcNo.Text;
            baglanti.Open();
            string sorgu2 = "SELECT COUNT(*) FROM Ogrencibilgileri WHERE tc_kimlik_numarasi = @tckimlikNumarasi";
            NpgsqlCommand komut2 = new NpgsqlCommand(sorgu2, baglanti);
            komut2.Parameters.AddWithValue("@tckimlikNumarasi", tckimlikNumarasi);

            int kayitSayisi = Convert.ToInt32(komut2.ExecuteScalar());
            baglanti.Close();
            if (kayitSayisi > 0)
            {
                MessageBox.Show("Bu Ogrenci için kayıt zaten mevcut. Lütfen 'Kayıtlı Ogrenci Listesi' ekranından devam ediniz.", "Ogrenci Kaydı Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //yeni Ogrenci kaydı oluşturacak.

            string sorgu = "INSERT INTO Ogrencibilgileri (tc_kimlik_numarasi, isim, soyisim, dogum_yeri, dogum_tarihi, telefon_numarasi, cinsiyet) VALUES (@tcKimlikNo, @isim, @soyisim, @dogumYeri, @dogumTarihi, @telefonNumarasi, @cinsiyet)";
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@tcKimlikNo", txtTcNo.Text);
            komut.Parameters.AddWithValue("@isim", txtAd.Text);
            komut.Parameters.AddWithValue("@soyisim", txtSoyad.Text);
            komut.Parameters.AddWithValue("@dogumYeri", txtDogumYeri.Text);
            komut.Parameters.AddWithValue("@dogumTarihi", txtDogumTarihi.Text);
            komut.Parameters.AddWithValue("@telefonNumarasi", txtTelefonNumarasi.Text);
            komut.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ogrenci bilgileri başarıyla kaydedildi. Listelemek için 'Listeyi Güncelle' butonuna basınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnOgrenciListeGuncelle_Click(object sender, EventArgs e)
        {
            //DataGrid'e öğrencilerin bilgilerini getirecek
            string sorgu = "select * from ogrencibilgileri";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataOgrenciBilgi.DataSource = ds.Tables[0];
            dataOgrenciBilgi.BackgroundColor = Color.FromArgb(0, 122, 146);
            dataOgrenciBilgi.DefaultCellStyle.BackColor = Color.FromArgb(0, 122, 146);
            dataOgrenciBilgi.DefaultCellStyle.ForeColor = Color.Cyan;
        }

        private void btnOgrenciBilgiAktar_Click(object sender, EventArgs e)
        {
            //Ogrencinın bilgilerini, tabpage2'de "Ogrenci bilgileri" kısmına aktar
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("select * from ogrencibilgileri where \"tc_kimlik_numarasi\" like '" + txtTcKontrol.Text + "' ", baglanti);
            NpgsqlDataReader reader = komut.ExecuteReader();

            while (reader.Read())
            {
                txtOgrenciTc.Text = reader[0].ToString();
                txtOgrenciAdi.Text = reader[1].ToString();
                txtOgrenciSoyadi.Text = reader[2].ToString();
                txtOgrenciTelefon.Text = reader[5].ToString();

                if (reader[0] != null)
                {
                    txtOgrenciTc.Enabled = true;
                    txtOgrenciAdi.Enabled = true;
                    txtOgrenciSoyadi.Enabled = true;
                    txtOgrenciTelefon.Enabled = true;
                    MessageBox.Show("Ogrenci bilgileri başarıyla aktarıldı. 'Ders Ekleme' ekranından işlemlerinize devam edebilirsiniz.", "Aktarma Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControl.SelectedTab = pageDersEkleme;
                }
            }
            baglanti.Close();
            //DataGrid'e derslerın bilgilerini getirecek
            string sorgu = "select * from dersbilgileri";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sorgu, baglanti);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataDersler.DataSource = ds.Tables[0];
        }
        //*********************************Ogrenci KAYIT EKRANI SONU**********************************


        //*********************************Ders Ekleme Ekranı**********************************

        private void dataDersler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDersKod.Text = dataDersler.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            txtDersKodEkle.Text = dataDersler.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            int rowIndex = e.RowIndex;
            DataGridViewRow selectedRow = dataDersler.Rows[rowIndex];
            txtDersKod.Text = selectedRow.Cells[0].Value.ToString();
            txtDersKodEkle.Text = selectedRow.Cells[0].Value.ToString();
            txtOgretmenAd.Text = selectedRow.Cells[2].Value.ToString();
        }

        private void btnDersKaydet_Click(object sender, EventArgs e)
        {
            string tckimlikNumarasi = txtOgrenciTc.Text;
            string dersKodu = txtDersKodEkle.Text;

            //günlük sınır
            string sorgu2 = "SELECT COUNT(*) FROM alinandersbilgileri WHERE tc_kimliknumarasi = @tckimlikNumarasi AND ders_kodu = @dersKodu";
            baglanti.Open();
            NpgsqlCommand komut2 = new NpgsqlCommand(sorgu2, baglanti);
            komut2.Parameters.AddWithValue("@tckimlikNumarasi", tckimlikNumarasi);
            komut2.Parameters.AddWithValue("@dersKodu", dersKodu);

            int kayitSayisi = Convert.ToInt32(komut2.ExecuteScalar());
            baglanti.Close();
            if (kayitSayisi > 0)
            {
                MessageBox.Show("Bu ogrenci için bu kod numarasın sahip zaten bir ders kaydı mevcut. Lütfen başka bir ders seçiniz.", "Ders Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string sorgu = "INSERT INTO alinandersbilgileri (tc_kimliknumarasi, ders_kodu, vize_notu, final_notu, ortalama) VALUES (@tcKimlikNumarasi, @dersKodu, @vize_notu, @final_notu, @ortalama)";
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@tcKimlikNumarasi", txtOgrenciTc.Text);
            komut.Parameters.AddWithValue("@dersKodu", txtDersKodEkle.Text);
            komut.Parameters.AddWithValue("@vize_notu", Convert.ToInt32(txtVizeNotEkle.Text));
            komut.Parameters.AddWithValue("@final_notu", Convert.ToInt32(txtFinalNotEkle.Text));
            komut.Parameters.AddWithValue("@ortalama", (Convert.ToInt32(txtVizeNotEkle.Text) * 0.4) + (Convert.ToInt32(txtFinalNotEkle.Text) * 0.6));
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ders başarıyla eklendi.", "Ders Eklendi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //DataGrid'e öğrencilerin bilgilerini getirecek
            string sorgu1 = "select * from alinandersbilgileri WHERE tc_kimliknumarasi =@tcNo";
            NpgsqlCommand komut1 = new NpgsqlCommand(sorgu1, baglanti);
            komut1.Parameters.AddWithValue("@tcNo", txtOgrenciTc.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut1);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataAlinanDersler.DataSource = dt;
        }
        private void btnDersGüncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("UPDATE alinandersbilgileri SET  vize_notu=@vizeNotu, final_notu=@finalNotu, ortalama=@ortalama WHERE tc_kimliknumarasi=@tcNo AND ders_kodu=@dersKodu", baglanti);
            komut.Parameters.AddWithValue("@vizeNotu", Convert.ToInt32(txtVizeNotGuncelle.Text));
            komut.Parameters.AddWithValue("@finalNotu", Convert.ToInt32(txtFinalNotGuncelle.Text));
            komut.Parameters.AddWithValue("@ortalama", (Convert.ToInt32(txtVizeNotGuncelle.Text) * 0.4) + (Convert.ToInt32(txtFinalNotGuncelle.Text) * 0.6));
            komut.Parameters.AddWithValue("@tcNo", txtOgrenciTc.Text);
            komut.Parameters.AddWithValue("@dersKodu", txtDersKodGuncelle.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ders bilgileri başarıyla güncellendi.", "Güncelleme Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void dataAlinanDersler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDersKodGuncelle.Text = dataAlinanDersler.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

        }
        private void btnDersBilgiGetir_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from alinandersbilgileri WHERE tc_kimliknumarasi =@tcNo";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@tcNo", txtOgrenciTc.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataAlinanDersler.DataSource = dt;
        }
        private void btnDersSil_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            NpgsqlCommand komut = new NpgsqlCommand("Delete FROM alinandersbilgileri WHERE tc_kimliknumarasi=@tcno AND ders_kodu=@dersKodu", baglanti);
            komut.Parameters.AddWithValue("@tcNo", txtOgrenciTc.Text);
            komut.Parameters.AddWithValue("@dersKodu", txtDersKodGuncelle.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ders kaydı silme işlemi başarıyla gerçekleşti.", "Silme İşlemi Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string sorgu = "select * from alinandersbilgileri WHERE tc_kimliknumarasi=@tcNumara";
            NpgsqlCommand komut1 = new NpgsqlCommand(sorgu, baglanti);
            komut1.Parameters.AddWithValue("@tcNumara", txtOgrenciTc.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut1);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataAlinanDersler.DataSource = dt;
        }
        private void btnDersleriGetirKod_Click(object sender, EventArgs e)
        {
            //ders koduna göre listele
            string sorgu = "SELECT * FROM dersbilgileri WHERE ders_kodu= @dersKodu";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@dersKodu", txtDersKod.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersler.DataSource = dt;
        }
        private void btnDersleriGetirOgrtmn_Click(object sender, EventArgs e)
        {
            //ogretmen adına göre listele
            string sorgu = "SELECT * FROM dersbilgileri WHERE ogretmen_adi= @ogretmenAdi";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@ogretmenAdi", txtOgretmenAd.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersler.DataSource = dt;
        }
        private void btnDersleriGetirDonem_Click(object sender, EventArgs e)
        {
            //donem bilgisine göre listele
            string sorgu = "SELECT * FROM dersbilgileri WHERE verildigi_donem= @donem";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@donem", txtDonem.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersler.DataSource = dt;
        }
        private void btnTumDersGetir_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM dersbilgileri ";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersler.DataSource = dt;
        }

        //*********************************Ders Ekleme EKRANI SONU**********************************

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        //*********************************Ders Kayıtları EKRANI **********************************

        private void btnDersKayitListele_Click(object sender, EventArgs e)
        {
            string sorgu = "select * from alinandersbilgileri";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersKayıtları.DataSource = dt;

            label22.Visible=false;
            txtOrtalama.Visible=false;
        }

        private void dataDersKayıtları_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtKod.Text = dataDersKayıtları.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            int rowIndex = e.RowIndex;
            DataGridViewRow selectedRow = dataDersKayıtları.Rows[rowIndex];
            txtKod.Text = selectedRow.Cells[1].Value.ToString();
        }

        private void btnKodListele_Click(object sender, EventArgs e)
        {
            //ders koduna göre listele
            string sorgu = "SELECT * FROM alinandersbilgileri WHERE ders_kodu= @dersKodu";
            NpgsqlCommand komut = new NpgsqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@dersKodu", txtKod.Text);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataDersKayıtları.DataSource = dt;

            double toplam = 0;
            int sayac = 0;
            foreach (DataGridViewRow row in dataDersKayıtları.Rows)
            {
                if (row.Cells[4].Value != null) // sütun numarasını kontrol et
                {
                    toplam += Convert.ToDouble(row.Cells[4].Value);
                    sayac++;
                }
            }
            double ortalama = toplam / sayac;
            txtOrtalama.Text = ortalama.ToString();


            label22.Visible=true;
            txtOrtalama.Visible=true;
        }

        //*********************************Ders Kayıtları EKRANI SONU**********************************

    }
}
