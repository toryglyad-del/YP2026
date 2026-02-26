using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Cosmetica1
{
    public partial class Form5 : Form
    {
        private DataGridView dgvOrders;
        private Button btnSave;
        private Button btnDelete;
        private Label lblTitle;

        string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=cosmetic;Integrated Security=True;TrustServerCertificate=True";

        public Form5()
        {
            InitializeComponent();
            this.Text = "Управление заказами";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateControls();
            LoadOrders();
        }

        private void CreateControls()
        {
            // Заголовок
            lblTitle = new Label
            {
                Text = "Список заказов",
                Location = new Point(10, 10),
                Size = new Size(200, 30),
                Font = new Font("Times New Roman", 14, FontStyle.Bold)
            };

            // Таблица заказов
            dgvOrders = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(860, 350),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Кнопка "Сохранить"
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(10, 410),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnSave.Click += BtnSave_Click;

            // Кнопка "Удалить заказ"
            btnDelete = new Button
            {
                Text = "Удалить заказ",
                Location = new Point(140, 410),
                Size = new Size(120, 30),
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            // Добавляем на форму
            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvOrders);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnDelete);
        }

        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    o.Id AS 'Номер заказа',
                    o.CreationDate AS 'Дата создания',
                    o.DeliveryDate AS 'Дата доставки',
                    (pp.PostCode + ', ' + pp.City + ', ' + pp.Street + ', ' + pp.Building) AS 'Пункт выдачи',
                    u.Surname + ' ' + u.Name + ' ' + u.Patronymic AS 'Покупатель',
                    o.ReceiptCode AS 'Код получения',
                    os.Name AS 'Статус'
                FROM [Order] o
                JOIN OrderStatus os ON o.StatusId = os.Id
                JOIN [User] u ON o.UserId = u.Id
                LEFT JOIN PickUpPoint pp ON o.PickUpPoint = pp.Id
                ORDER BY o.CreationDate DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    System.Data.DataTable table = new System.Data.DataTable();
                    adapter.Fill(table);
                    dgvOrders.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки заказов: " + ex.Message);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // TODO: сохранение изменений статуса или даты
            MessageBox.Show("Сохранение изменений");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ для удаления");
                return;
            }

            int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["Номер заказа"].Value);

            DialogResult result = MessageBox.Show($"Удалить заказ №{orderId}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Сначала удаляем связанные товары
                    SqlCommand cmdDetail = new SqlCommand("DELETE FROM ProductInOrder WHERE OrderId = @id", conn);
                    cmdDetail.Parameters.AddWithValue("@id", orderId);
                    cmdDetail.ExecuteNonQuery();

                    // Потом сам заказ
                    SqlCommand cmdOrder = new SqlCommand("DELETE FROM [Order] WHERE Id = @id", conn);
                    cmdOrder.Parameters.AddWithValue("@id", orderId);
                    cmdOrder.ExecuteNonQuery();

                    MessageBox.Show("Заказ удалён");
                    LoadOrders(); // обновляем список
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}
