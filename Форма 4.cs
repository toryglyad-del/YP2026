using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;


namespace Cosmetica1
{
    public partial class Form4 : Form
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

        private int editingId; // ID редактируемого товара

        string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=cosmetic;Integrated Security=True;TrustServerCertificate=True";

        // Конструктор для редактирования (принимает ID товара)
        public Form4(int productId)
        {
            InitializeComponent();
            editingId = productId;
            this.Text = "Изменение товара";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            CreateControls();
            LoadComboBoxes();
            LoadProductData(); // Загружаем данные товара
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

        private void LoadProductData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Product WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", editingId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtName.Text = reader["Name"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        txtPrice.Text = Convert.ToDecimal(reader["Price"]).ToString();
                        txtDiscount.Text = reader["Discount"].ToString();
                        txtStock.Text = reader["AmountInStock"].ToString();
                        txtPhoto.Text = reader["Photo"].ToString();

                        // Выбираем нужные элементы в комбобоксах
                        SelectComboBoxItem(cmbCategory, Convert.ToInt32(reader["CategoryId"]));
                        SelectComboBoxItem(cmbProducer, Convert.ToInt32(reader["ProducerId"]));
                        SelectComboBoxItem(cmbProvider, Convert.ToInt32(reader["ProviderId"]));
                        SelectComboBoxItem(cmbUnit, Convert.ToInt32(reader["UnitId"]));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки товара: " + ex.Message);
                }
            }
        }

        private void SelectComboBoxItem(ComboBox combo, int value)
        {
            foreach (ComboboxItem item in combo.Items)
            {
                if ((int)item.Value == value)
                {
                    combo.SelectedItem = item;
                    break;
                }
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

            if (cmbCategory.SelectedItem == null || cmbProducer.SelectedItem == null ||
                cmbProvider.SelectedItem == null || cmbUnit.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию, производителя, поставщика и единицу измерения");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE Product SET
                        Name = @name,
                        Description = @desc,
                        Price = @price,
                        Discount = @discount,
                        AmountInStock = @stock,
                        Photo = @photo,
                        CategoryId = @catId,
                        ProducerId = @prodId,
                        ProviderId = @provId,
                        UnitId = @unitId
                        WHERE Id = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    // Цена — универсальный парсинг
                    // 1. Сначала объявляем
                    string priceText = txtPrice.Text.Trim();

                    // 2. Потом используем
                    priceText = priceText.Replace(',', '.');

                    // 3. Дальше парсим
                    decimal price = 0;
                    if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    {
                        MessageBox.Show("Некорректный формат цены");
                        return;
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
                    cmd.Parameters.AddWithValue("@id", editingId);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@photo", txtPhoto.Text);
                    cmd.Parameters.AddWithValue("@catId", ((ComboboxItem)cmbCategory.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@prodId", ((ComboboxItem)cmbProducer.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@provId", ((ComboboxItem)cmbProvider.SelectedItem).Value);
                    cmd.Parameters.AddWithValue("@unitId", ((ComboboxItem)cmbUnit.SelectedItem).Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Товар обновлён!");
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
