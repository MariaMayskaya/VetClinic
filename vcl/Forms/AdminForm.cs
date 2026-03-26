using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using vc.Database;
using vc.Models;

namespace vc.Forms
{
    public class AdminForm : Form
    {
        private User currentAdmin;
        private DatabaseHelper dbHelper;
        private TabControl tabControl;
        private DataGridView dgvUsers;
        private DataGridView dgvPets;
        private DataGridView dgvAppointments;
        private ListBox lstUpcomingAppointments;
        private ComboBox cmbStatusFilter;
        private Button btnRefresh;
        private Button btnLogout;
        private Button btnAddMedicalRecord;
        private Button btnCompleteAppointment;
        private Button btnCancelAppointment;

        
        private readonly Color MintLight = Color.FromArgb(255, 255, 255); // Основной фон

        private readonly Color MintSoft = Color.FromArgb(178, 235, 220);   // Панели и контейнеры
        private readonly Color MintMedium = Color.FromArgb(102, 204, 178); // Акцентные кнопки
        private readonly Color MintDark = Color.FromArgb(38, 139, 120);    // Заголовки, важные элементы
        private readonly Color MintDarker = Color.FromArgb(22, 102, 88);   // Критичные действия
        private readonly Color TextColor = Color.FromArgb(20, 60, 50);     // Основной текст

        public AdminForm(User admin)
        {
            currentAdmin = admin;
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            LoadData();
            ApplyStyling();
        }

        private void ApplyStyling()
        {
            // Стилизация формы
            this.BackColor = MintLight;
            this.ForeColor = TextColor;

            // Стилизация TabControl
            tabControl.BackColor = MintLight;
            tabControl.ForeColor = TextColor;

            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = MintLight;
                page.ForeColor = TextColor;
            }

            // Стилизация DataGridView
            StyleDataGridView(dgvUsers);
            StyleDataGridView(dgvPets);
            StyleDataGridView(dgvAppointments);

            // Стилизация ListBox
            lstUpcomingAppointments.BackColor = MintLight;
            lstUpcomingAppointments.ForeColor = TextColor;
            lstUpcomingAppointments.BorderStyle = BorderStyle.None;

            // Стилизация ComboBox
            cmbStatusFilter.BackColor = Color.White;
            cmbStatusFilter.ForeColor = TextColor;
            cmbStatusFilter.FlatStyle = FlatStyle.Flat;

            // Кнопки уже стилизованы в InitializeComponent
        }

        private void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = MintLight;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = MintMedium;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MintDark;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.RowHeadersDefaultCellStyle.BackColor = MintSoft;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = MintMedium;
        }

        private void InitializeComponent()
        {
            this.Text = $"Административная панель - {currentAdmin.FullName}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Вкладки
            tabControl = new TabControl
            {
                Location = new Point(10, 50),
                Size = new Size(1165, 650),
                Font = new Font("Segoe UI", 9)
            };

            // Вкладка пользователей
            var tabUsers = new TabPage("Пользователи");
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            tabUsers.Controls.Add(dgvUsers);

            // Вкладка питомцев
            var tabPets = new TabPage("Питомцы");
            dgvPets = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvPets.CellDoubleClick += DgvPets_CellDoubleClick;
            tabPets.Controls.Add(dgvPets);

            // Вкладка записей
            var tabAppointments = new TabPage("Записи на прием");

            var appointmentsPanel = new Panel { Dock = DockStyle.Fill, BackColor = MintLight };

            var filterPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = MintSoft
            };

            var lblFilter = new Label
            {
                Text = "Фильтр по статусу:",
                Location = new Point(10, 10),
                Size = new Size(120, 25),
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            cmbStatusFilter = new ComboBox
            {
                Location = new Point(140, 10),
                Size = new Size(150, 25),
                BackColor = Color.White,
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 9)
            };
            cmbStatusFilter.Items.AddRange(new object[] { "Все", "Запланирован", "Завершен", "Отменен" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += CmbStatusFilter_SelectedIndexChanged;





            dgvAppointments = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(1130, 400),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };





            var btnPanel = new Panel
            {
                Location = new Point(10, 460),
                Size = new Size(1130, 50),
                BackColor = MintLight
            };





            btnCompleteAppointment = new Button
            {
                Text = "Отметить как завершенный",
                Location = new Point(10, 10),
                Size = new Size(200, 30),
                BackColor = MintMedium,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDark }
            };
            btnCompleteAppointment.Click += BtnCompleteAppointment_Click;




            btnCancelAppointment = new Button
            {
                Text = "Отменить запись",
                Location = new Point(220, 10),
                Size = new Size(150, 30),
                BackColor = MintDarker,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDark }
            };
            btnCancelAppointment.Click += BtnCancelAppointment_Click;





            btnAddMedicalRecord = new Button
            {
                Text = "Добавить мед. запись",
                Location = new Point(380, 10),
                Size = new Size(180, 30),
                BackColor = MintDark,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDarker }
            };
            btnAddMedicalRecord.Click += BtnAddMedicalRecord_Click;





            filterPanel.Controls.AddRange(new Control[] { lblFilter, cmbStatusFilter });
            btnPanel.Controls.AddRange(new Control[] { btnCompleteAppointment, btnCancelAppointment, btnAddMedicalRecord });
            appointmentsPanel.Controls.AddRange(new Control[] { filterPanel, dgvAppointments, btnPanel });
            tabAppointments.Controls.Add(appointmentsPanel);





            // Вкладка ближайших записей
            var tabUpcoming = new TabPage("Ближайшие записи");
            lstUpcomingAppointments = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = MintLight,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9)
            };
            lstUpcomingAppointments.DrawMode = DrawMode.OwnerDrawFixed;
            lstUpcomingAppointments.DrawItem += LstUpcomingAppointments_DrawItem;
            lstUpcomingAppointments.ItemHeight = 150;
            lstUpcomingAppointments.DoubleClick += LstUpcomingAppointments_DoubleClick;
            tabUpcoming.Controls.Add(lstUpcomingAppointments);




            // Вкладка аптеки
            var tabPharmacy = new TabPage("Аптека");




            // Создаем панель для размещения PharmacyForm
            var pharmacyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = MintLight
            };






            try
            {
                // создаем экземпляр PharmacyForm   но не показываем как отдельное окно
                var pharmacyForm = new PharmacyForm(currentAdmin);

                // настраиваем PharmacyForm для встраивания в панель
                pharmacyForm.TopLevel = false;
                pharmacyForm.FormBorderStyle = FormBorderStyle.None;
                pharmacyForm.Dock = DockStyle.Fill;
                pharmacyForm.Visible = true;
                pharmacyForm.BackColor = MintLight;

                // добавляем PharmacyForm в панель
                pharmacyPanel.Controls.Add(pharmacyForm);
            }
            catch (Exception ex)
            {
                //в  случае ошибки показываем сообщение на панели
                var errorLabel = new Label
                {
                    Text = $"Ошибка загрузки аптеки: {ex.Message}",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = MintDarker,
                    BackColor = MintLight,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                pharmacyPanel.Controls.Add(errorLabel);
            }

            tabPharmacy.Controls.Add(pharmacyPanel);

            // добавляем все вкладки в TabControl
            tabControl.TabPages.AddRange(new TabPage[] {
                tabUsers,
                tabPets,
                tabAppointments,
                tabUpcoming,
                tabPharmacy
            });

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Панель администратора",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(300, 30),
                ForeColor = MintDark
            };

            // Кнопка обновления
            btnRefresh = new Button
            {
                Text = "Обновить данные",
                Location = new Point(1000, 10),
                Size = new Size(150, 30),
                BackColor = MintMedium,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDark }
            };
            btnRefresh.Click += BtnRefresh_Click;





            // кнопка  выхода
            btnLogout = new Button
            {
                Text = "Выход",
                Location = new Point(1060, 720),
                Size = new Size(100, 30),
                BackColor = MintDarker,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDark }
            };
            btnLogout.Click += BtnLogout_Click;





            // добавляем все элементы на форму
            this.Controls.AddRange(new Control[] {
                lblTitle,
                tabControl,
                btnRefresh,
                btnLogout
            });
        }
        


        private void LoadData()
        {
            try
            {
                // загрузка пользователей
                var users = dbHelper.GetAllUsers();
                dgvUsers.DataSource = null;
                dgvUsers.DataSource = users;

                // Скрываем пароль
                if (dgvUsers.Columns["Password"] != null)
                    dgvUsers.Columns["Password"].Visible = false;

                // Загрузка питомцев
                var pets = dbHelper.GetAllPets();
                dgvPets.DataSource = null;
                dgvPets.DataSource = pets;

                // Загрузка записей
                LoadAppointments();

                // ззагрузка ближайших записей
                var upcoming = dbHelper.GetUpcomingAppointments();
                lstUpcomingAppointments.DataSource = null;
                lstUpcomingAppointments.DataSource = upcoming;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadAppointments()
        {
            var appointments = dbHelper.GetAllAppointments();

            if (cmbStatusFilter != null && cmbStatusFilter.SelectedIndex > 0)
            {
                string status = cmbStatusFilter.SelectedItem.ToString();
                string dbStatus = status == "Запланирован" ? "Scheduled" :
                                 status == "Завершен" ? "Completed" : "Cancelled";
                appointments = appointments.Where(a => a.Status == dbStatus).ToList();
            }

            dgvAppointments.DataSource = null;
            dgvAppointments.DataSource = appointments;



            // Настройка отображения
            if (dgvAppointments.Columns["AppointmentDate"] != null)
            {
                dgvAppointments.Columns["AppointmentDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }
        }






        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAppointments();
        }

        private void DgvPets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var pet = (Pet)dgvPets.Rows[e.RowIndex].DataBoundItem;
                var historyForm = new MedicalHistoryForm(pet, currentAdmin);
                historyForm.ShowDialog();
            }
        }






        private void LstUpcomingAppointments_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = (ListBox)sender;
            var appointment = (Appointment)listBox.Items[e.Index];

            e.DrawBackground();

           
            using (var backBrush = new SolidBrush(e.Index % 2 == 0 ? MintLight : MintSoft))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            
            using (var accentBrush = new SolidBrush(MintDark))
            {
                e.Graphics.FillRectangle(accentBrush, e.Bounds.Left, e.Bounds.Top, 3, e.Bounds.Height);
            }

            using (var brush = new SolidBrush(TextColor))
            {
                string text = $"Клиент: {appointment.OwnerName}\n" +
                             $"Питомец: {appointment.PetName}\n" +
                             $"Дата: {appointment.AppointmentDate:dd.MM.yyyy HH:mm}\n" +
                             $"Причина: {appointment.Reason}";

                e.Graphics.DrawString(text, new Font("Segoe UI", 9), brush, e.Bounds.Left + 10, e.Bounds.Top + 5);
            }

            e.DrawFocusRectangle();
        }

        private void LstUpcomingAppointments_DoubleClick(object sender, EventArgs e)
        {
            if (lstUpcomingAppointments.SelectedItem is Appointment appointment)
            {
                var result = MessageBox.Show("Хотите добавить медицинскую запись для этого приема?",
                    "Создание мед. записи", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var pet = dbHelper.GetPetById(appointment.PetId);
                    if (pet != null)
                    {
                        var recordForm = new AddMedicalRecordForm(pet, appointment, currentAdmin);
                        if (recordForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }

        private void BtnCompleteAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;

                if (appointment.Status == "Scheduled")
                {
                    var result = MessageBox.Show("Отметить запись как завершенную?",
                        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (dbHelper.UpdateAppointmentStatus(appointment.Id, "Completed"))
                        {
                            MessageBox.Show("Статус обновлен!");

                            // Предложение добавить медицинскую запись
                            var addRecord = MessageBox.Show("Добавить медицинскую запись?",
                                "Мед. запись", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (addRecord == DialogResult.Yes)
                            {
                                var pet = dbHelper.GetPetById(appointment.PetId);
                                if (pet != null)
                                {
                                    var recordForm = new AddMedicalRecordForm(pet, appointment, currentAdmin);
                                    if (recordForm.ShowDialog() == DialogResult.OK)
                                    {
                                        LoadData();
                                    }
                                }
                            }

                            LoadData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Можно отметить только запланированные записи!");
                }
            }
        }

        private void BtnCancelAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;

                var result = MessageBox.Show("Отменить запись?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (dbHelper.UpdateAppointmentStatus(appointment.Id, "Cancelled"))
                    {
                        MessageBox.Show("Запись отменена!");
                        LoadData();
                    }
                }
            }
        }

        private void BtnAddMedicalRecord_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.SelectedRows.Count > 0)
            {
                var appointment = (Appointment)dgvAppointments.SelectedRows[0].DataBoundItem;
                var pet = dbHelper.GetPetById(appointment.PetId);

                if (pet != null)
                {
                    var recordForm = new AddMedicalRecordForm(pet, appointment, currentAdmin);
                    if (recordForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else if (dgvPets.SelectedRows.Count > 0)
            {
                var pet = (Pet)dgvPets.SelectedRows[0].DataBoundItem;
                var recordForm = new AddMedicalRecordForm(pet, null, currentAdmin);
                if (recordForm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись на прием или питомца!");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();

            // Принудительно обновляем вкладку аптеки, пересоздавая её
            try
            {
                // Находим вкладку аптеки
                foreach (TabPage page in tabControl.TabPages)
                {
                    if (page.Text == "Аптека")
                    {
                        // Очищаем и пересоздаем содержимое
                        page.Controls.Clear();

                        var pharmacyPanel = new Panel { Dock = DockStyle.Fill, BackColor = MintLight };

                        var pharmacyForm = new PharmacyForm(currentAdmin);
                        pharmacyForm.TopLevel = false;
                        pharmacyForm.FormBorderStyle = FormBorderStyle.None;
                        pharmacyForm.Dock = DockStyle.Fill;
                        pharmacyForm.Visible = true;
                        pharmacyForm.BackColor = MintLight;

                        pharmacyPanel.Controls.Add(pharmacyForm);
                        page.Controls.Add(pharmacyPanel);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки при обновлении аптеки
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }
    }
}


