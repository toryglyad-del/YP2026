using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Cosmetica1
{
    public partial class Form3 : Form
    {
        private TextBox txtName;
        private TextBox txtDescription;
        private TextBox txtPrice;
        private TextBox txtDiscount;
        private TextBox txtStock;
        private TextBox txtPhoto;
        private ComboBox cmbCategory;
        private ComboBox cmbProducer;
        private ComboBox cmbProvider;
        private ComboBox cmbUnit;
        private Button btnSave;
        private Button btnCancel;

        string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=cosmetic;Integrated Security=True;TrustServerCertificate=True";

        public Form3()
        {
            InitializeComponent();
            this.Text = "Добавление товара";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            CreateControls();
            LoadComboBoxes();
        }

        private void CreateControls()
        {
            int y = 20;
            int labelX = 20;
            int fieldX = 150;
            int fieldWidth = 200;

            // Название
            AddLabel("Название:", labelX, y);
            txtName = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Описание
            AddLabel("Описание:", labelX, y);
            txtDescription = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Цена
            AddLabel("Цена:", labelX, y);
            txtPrice = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Скидка (%)
            AddLabel("Скидка %:", labelX, y);
            txtDiscount = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Остаток
            AddLabel("Остаток:", labelX, y);
            txtStock = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Фото
            AddLabel("Фото (путь):", labelX, y);
            txtPhoto = AddTextBox(fieldX, y, fieldWidth);
            y += 30;

            // Категория
            AddLabel("Категория:", labelX, y);
            cmbCategory = new ComboBox { Location = new Point(fieldX, y), Size = new Size(fieldWidth, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbCategory);
            y += 30;

            // Производитель
            AddLabel("Производитель:", labelX, y);
            cmbProducer = new ComboBox { Location = new Point(fieldX, y), Size = new Size(fieldWidth, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbProducer);
            y += 30;

            // Поставщик
            AddLabel("Поставщик:", labelX, y);
            cmbProvider = new ComboBox { Location = new Point(fieldX, y), Size = new Size(fieldWidth, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbProvider);
            y += 30;

            // Единица измерения
            AddLabel("Ед. измерения:", labelX, y);
            cmbUnit = new ComboBox { Location = new Point(fieldX, y), Size = new Size(fieldWidth, 20), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbUnit);
            y += 40;

            // Кнопки
            btnSave = new Button { Text = "Сохранить", Location = new Point(100, y), Size = new Size(100, 30) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "Отмена", Location = new Point(220, y), Size = new Size(100, 30) };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label { Text = text, Location = new Point(x, y), Size = new Size(120, 20) };
            this.Controls.Add(lbl);
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            TextBox txt = new TextBox { Location = new Point(x, y), Size = new Size(width, 20) };
            this.Controls.Add(txt);
            return txt;
        }

        private void LoadComboBoxes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Категории
                SqlCommand cmdCat = new SqlCommand("SELECT Id, Name FROM Category", conn);
                SqlDataReader readerCat = cmdCat.ExecuteReader();
                while (readerCat.Read())
                {
                    cmbCategory.Items.Add(new ComboboxItem { Text = readerCat["Name"].ToString(), Value = readerCat["Id"] });
                }
                readerCat.Close();

                // Производители
                SqlCommand cmdProd = new SqlCommand("SELECT Id, Name FROM Producer", conn);
                SqlDataReader readerProd = cmdProd.ExecuteReader();
                while (readerProd.Read())
                {
                    cmbProducer.Items.Add(new ComboboxItem { Text = readerProd["Name"].ToString(), Value = readerProd["Id"] });
                }
                readerProd.Close();

                // Поставщики
                SqlCommand cmdProv = new SqlCommand("SELECT Id, Name FROM Provider", conn);
                SqlDataReader readerProv = cmdProv.ExecuteReader();
                while (readerProv.Read())
                {
                    cmbProvider.Items.Add(new ComboboxItem { Text = readerProv["Name"].ToString(), Value = readerProv["Id"] });
                }
                readerProv.Close();

                // Единицы измерения
                SqlCommand cmdUnit = new SqlCommand("SELECT Id, Name FROM Unit", conn);
                SqlDataReader readerUnit = cmdUnit.ExecuteReader();
                while (readerUnit.Read())
                {
                    cmbUnit.Items.Add(new ComboboxItem { Text = readerUnit["Name"].ToString(), Value = readerUnit["Id"] });
                }
                readerUnit.Close();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Проверка заполнения
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Заполните название и цену");
                return;
            }

            if (cmbCategory.SelectedItem == null || cmbProducer.SelectedItem == null || cmbProvider.SelectedItem == null || cmbUnit.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию, производителя, поставщика и единицу измерения");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Получаем следующий свободный Id
                    SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(Id), 0) + 1 FROM Product", conn);
                    int newId = (int)cmdMax.ExecuteScalar();
                    string query = @"INSERT INTO Product 
                        (Id, Name, Description, Price, Discount, AmountInStock, Photo, CategoryId, ProducerId, ProviderId, UnitId)
                        VALUES 
                        (@id, @name, @desc, @price, @discount, @stock, @photo, @catId, @prodId, @provId, @unitId)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    // Цена
                    decimal price = 0;
                    if (!decimal.TryParse(txtPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    {
                        if (!decimal.TryParse(txtPrice.Text, out price))
                        {
                            MessageBox.Show("Некорректный формат цены");
                            return;
                        }
                    }
                    cmd.Parameters.AddWithValue("@price", price);

                    // Скидка
                    int discount = 0;
                    if (!string.IsNullOrEmpty(txtDiscount.Text))
                    {
                        if (!int.TryParse(txtDiscount.Text, out discount))
                        {
                            MessageBox.Show("Некорректный формат скидки");
                            return;
                        }
                    }
                    cmd.Parameters.AddWithValue("@discount", discount);

                    // Остаток
                    int stock = 0;
                    if (!string.IsNullOrEmpty(txtStock.Text))
                    {
                        if (!int.TryParse(txtStock.Text, out stock))
                        {
                            MessageBox.Show("Некорректный формат остатка");
                            return;
                        }
                    }
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                    string priceText = txtPrice.Text.Replace(',', '.');
                    cmd.Parameters.AddWithValue("@photo", txtPhoto.Text);
                    cmd.Parameters.AddWithValue("@catId", ((ComboboxItem)cmbCategory.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@prodId", ((ComboboxItem)cmbProducer.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@provId", ((ComboboxItem)cmbProvider.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@unitId", ((ComboboxItem)cmbUnit.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@id", newId);
                    // временный артикул

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Товар добавлен!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}

    // Вспомогательный класс для ComboBox
    
