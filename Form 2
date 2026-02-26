using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Cosmetica1
{
    public partial class Form2 : Form
    {
        private int currentRoleId;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private FlowLayoutPanel flowPanel;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private ComboBox cmbFilter;
        private Label lblUser;

        string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=cosmetic;Integrated Security=True;TrustServerCertificate=True";

        public Form2(int roleId, string fio)
        {
            InitializeComponent();
            currentRoleId = roleId;
            this.Text = "Каталог косметики";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateControls();
            ApplyPermissions();
            LoadProducts();
        }

        private void CreateControls()
        {
            // Верхняя панель с ФИО
            lblUser = new Label
            {
                Text = $"Пользователь: {currentRoleId switch { 1 => "Администратор", 2 => "Менеджер", _ => "Клиент" }}",
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                Font = new Font("Times New Roman", 10, FontStyle.Bold)
            };

            // Поиск
            txtSearch = new TextBox
            {
                Location = new Point(220, 10),
                Size = new Size(200, 20),
                PlaceholderText = "Поиск по названию..."
            };
            txtSearch.TextChanged += (s, e) => LoadProducts();

            // Сортировка
            cmbSort = new ComboBox
            {
                Location = new Point(430, 10),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSort.Items.AddRange(new[] { "Без сортировки", "Цена ↑", "Цена ↓" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (s, e) => LoadProducts();

            // Фильтр по категориям
            cmbFilter = new ComboBox
            {
                Location = new Point(590, 10),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.Add("Все категории");
            LoadCategories();
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadProducts();

            // Панель для карточек
            flowPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 40),
                Width = this.ClientSize.Width - 20,      // ширина формы минус отступы
                Height = this.ClientSize.Height - 100,   // высота минус место под кнопки
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            // Добавляем на форму
            this.Controls.Add(lblUser);
            this.Controls.Add(txtSearch);
            this.Controls.Add(cmbSort);
            this.Controls.Add(cmbFilter);
            this.Controls.Add(flowPanel);
            // Кнопка "Назад"
            Button btnBack = new Button
            {
                Text = "Назад",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.LightGray
            };
            btnBack.Click += BtnBack_Click;

            // Кнопка "Добавить"
            btnAdd = new Button
            {
                Text = "Добавить",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.LightGreen
            };
            btnAdd.Click += BtnAdd_Click;

            // Кнопка "Изменить"
            btnEdit = new Button
            {
                Text = "Изменить",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.LightBlue
            };
            btnEdit.Click += BtnEdit_Click;

            // Кнопка "Удалить"
            btnDelete = new Button
            {
                Text = "Удалить",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            // Кнопка "Заказы"
            Button btnOrders = new Button
            {
                Text = "Заказы",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.LightYellow
            };
            btnOrders.Click += BtnOrders_Click;

            // Добавляем кнопки на форму
            this.Controls.Add(btnBack);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnOrders);

            // Располагаем кнопки динамически внизу
            this.Layout += (s, e) =>
            {
                int bottomY = this.ClientSize.Height - 40; // отступ от низа
                int leftX = 10; // отступ слева

                btnBack.Location = new Point(leftX, bottomY);
                btnAdd.Location = new Point(leftX + 110, bottomY);
                btnEdit.Location = new Point(leftX + 220, bottomY);
                btnDelete.Location = new Point(leftX + 330, bottomY);
                btnOrders.Location = new Point(this.ClientSize.Width - 110, bottomY);
            };
        }
        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name FROM Category", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbFilter.Items.Add(reader.GetString(0));
                }
            }
        }

        private void ApplyPermissions()
        {
            bool isPrivileged = true; // ← ВРЕМЕННО включаем всё

            if (txtSearch != null) txtSearch.Visible = isPrivileged;
            if (cmbSort != null) cmbSort.Visible = isPrivileged;
            if (cmbFilter != null) cmbFilter.Visible = isPrivileged;

            if (btnAdd != null) btnAdd.Visible = isPrivileged;
            if (btnEdit != null) btnEdit.Visible = isPrivileged;
            if (btnDelete != null) btnDelete.Visible = isPrivileged;

            // Принудительно показываем кнопки, даже если они были невидимы
            if (btnAdd != null) btnAdd.Visible = true;
            if (btnEdit != null) btnEdit.Visible = true;
            if (btnDelete != null) btnDelete.Visible = true;

        }

        private void LoadProducts()
        {
            flowPanel.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT p.Name, p.Description, p.Price, p.Discount, p.AmountInStock, p.Photo
                FROM Product p
                WHERE 1=1";

                    // Поиск по названию
                    if (txtSearch.Visible && !string.IsNullOrEmpty(txtSearch.Text))
                    {
                        query += " AND p.Name LIKE @search";
                    }

                    // Фильтр по категории
                    if (cmbFilter.Visible && cmbFilter.SelectedIndex > 0)
                    {
                        query += " AND p.CategoryId = (SELECT Id FROM Category WHERE Name = @cat)";
                    }

                    // Сортировка
                    if (cmbSort.Visible && cmbSort.SelectedIndex == 1)
                    {
                        query += " ORDER BY p.Price ASC";
                    }
                    else if (cmbSort.Visible && cmbSort.SelectedIndex == 2)
                    {
                        query += " ORDER BY p.Price DESC";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (txtSearch.Visible && !string.IsNullOrEmpty(txtSearch.Text))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    }
                    if (cmbFilter.Visible && cmbFilter.SelectedIndex > 0)
                    {
                        cmd.Parameters.AddWithValue("@cat", cmbFilter.SelectedItem.ToString());
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CosmeticCard card = new CosmeticCard();

                        card.FillData(
                            reader["Name"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDecimal(reader["Price"]),
                            Convert.ToInt32(reader["Discount"]),
                            Convert.ToInt32(reader["AmountInStock"]),
                            reader["Photo"].ToString()
                        );

                        flowPanel.Controls.Add(card);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки товаров: " + ex.Message);
                }
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Form3 addForm = new Form3(); // Открываем форму добавления
            addForm.ShowDialog(); // Показываем как диалог
            LoadProducts(); // Перезагружаем список после закрытия
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Введите ID товара для редактирования:", "Редактирование", "0");
            if (string.IsNullOrEmpty(input)) return;

            int id;
            if (!int.TryParse(input, out id))
            {
                MessageBox.Show("Некорректный ID");
                return;
            }

            // Открываем Form4 с этим ID
            Form4 editForm = new Form4(id);
            editForm.ShowDialog();
            LoadProducts(); // Перезагружаем список после закрытия
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, выбран ли товар
            if (flowPanel.Controls.Count == 0)
            {
                MessageBox.Show("Нет товаров для удаления");
                return;
            }

            // Получаем выбранный товар (последний кликнутый)
            // Для простоты будем удалять первый выделенный или последний добавленный
            // Но лучше сделать выбор по ID

            // Спросим ID у пользователя (временно, для теста)
            string input = Microsoft.VisualBasic.Interaction.InputBox("Введите ID товара для удаления:", "Удаление", "0");
            if (string.IsNullOrEmpty(input)) return;

            int id;
            if (!int.TryParse(input, out id))
            {
                MessageBox.Show("Некорректный ID");
                return;
            }

            // Подтверждение
            DialogResult result = MessageBox.Show($"Удалить товар с ID {id}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            // Удаление из БД
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Product WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Товар удалён");
                        LoadProducts(); // Перезагружаем список
                    }
                    else
                    {
                        MessageBox.Show("Товар с таким ID не найден");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    private void BtnOrders_Click(object sender, EventArgs e)
        {
            Form5 ordersForm = new Form5();
            ordersForm.ShowDialog();
        }
    }
}
