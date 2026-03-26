using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;


using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using vc.Database;
using vc.Models;

namespace vc.Forms
{
    public class MainForm : Form
    {
        private User currentUser;
        private DatabaseHelper dbHelper; 

        private ListBox lstPets ;
        private ListBox lstAppointments;

        private Button btnAddPet  ;
        private Button btnAddAppointment;
        private Button btnViewHistory ;
        private Button btnLogout;

        private Label lblWelcome;

        // Цветовая схема
        private readonly Color mintColor = Color.FromArgb(169, 223, 191);   // Мятный
        private readonly Color darkMintColor = Color.FromArgb(149, 203, 171); // Темно-мятный
        private readonly Color darkBgColor = Color.FromArgb(18, 18, 18);    // Почти черный
        private readonly Color panelBgColor = Color.FromArgb(30, 30, 30);   // Темно-серый для панелей
        private readonly Color inputBgColor = Color.FromArgb(45, 45, 45);   // Серый для полей ввода
        private readonly Color textColor = Color.FromArgb(237, 242, 247);   // Светлый текст
        private readonly Color cancelColor = Color.FromArgb(220, 80, 80);   // Красный для отмены

        public MainForm(User user)
        {
            currentUser = user;
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            LoadData();
        }





        private void InitializeComponent()
        {
            // Настройки главной формы
            this.Text = $"Главная - Ветеринарная клиника ({currentUser.FullName})";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = darkBgColor;

            // Создаем панель для скругленных углов формы
            this.Paint += (sender, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 30, 30, 180, 90);
                    path.AddArc(this.Width - 30, 0, 30, 30, 270, 90);
                    path.AddArc(this.Width - 30, this.Height - 30, 30, 30, 0, 90);
                    path.AddArc(0, this.Height - 30, 30, 30, 90, 90);
                    path.CloseAllFigures();
                    this.Region = new Region(path);
                }
            };




            // Верхняя панель с заголовком
            Panel topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = panelBgColor
            };




            // Кнопка закрытия
            Button btnClose = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = mintColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 20),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = mintColor;
            topPanel.Controls.Add(btnClose);

            lblWelcome = new Label
            {
                Text = $"Добро пожаловать, {currentUser.FullName}!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = mintColor,
                Location = new Point(30, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(lblWelcome);






            // Левая панель с питомцами
            Panel leftPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(350, 500),
                BackColor = panelBgColor,
                BorderStyle = BorderStyle.None
            };






            // Добавляем скругление для левой панели
            ApplyRoundedCorners(leftPanel, 15);

            var lblPets = new Label
            {
                Text = "МОИ ПИТОМЦЫ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = mintColor,
                Location = new Point(15, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };




            lstPets = new ListBox
            {
                Location = new Point(15, 50),
                Size = new Size(320, 350),
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11)
            };
            lstPets.SelectedIndexChanged += LstPets_SelectedIndexChanged;
            lstPets.DrawMode = DrawMode.OwnerDrawFixed;
            lstPets.DrawItem += LstPets_DrawItem;
            lstPets.ItemHeight = 45;


            btnAddPet = CreateStyledButton("+ ДОБАВИТЬ ПИТОМЦА", new Point(15, 410), mintColor);
            btnAddPet.Click += BtnAddPet_Click;



            leftPanel.Controls.AddRange(new Control[] { lblPets, lstPets, btnAddPet });





            // Правая панель с записями
            Panel rightPanel = new Panel
            {
                Location = new Point(390, 100),
                Size = new Size(690, 500),
                BackColor = panelBgColor,
                BorderStyle = BorderStyle.None
            };





            // Добавляем скругление для правой панели
            ApplyRoundedCorners(rightPanel, 15);

            var lblAppointments = new Label
            {
                Text = "ЗАПИСИ НА ПРИЕМ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = mintColor,
                Location = new Point(15, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };





            lstAppointments = new ListBox
            {
                Location = new Point(15, 50),
                Size = new Size(660, 350),
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            lstAppointments.DrawMode = DrawMode.OwnerDrawFixed;
            lstAppointments.DrawItem += LstAppointments_DrawItem;
            lstAppointments.ItemHeight = 75;




            btnAddAppointment = CreateStyledButton("+ ЗАПИСАТЬСЯ", new Point(15, 410), mintColor);
            btnAddAppointment.Click += BtnAddAppointment_Click;



            btnViewHistory = CreateStyledButton("📋 ИСТОРИЯ", new Point(245, 410), darkMintColor);
            btnViewHistory.Click += BtnViewHistory_Click;
            btnViewHistory.Enabled = false;



            btnLogout = CreateStyledButton("🚪 ВЫЙТИ", new Point(475, 410), cancelColor);
            btnLogout.Click += BtnLogout_Click;



            rightPanel.Controls.AddRange(new Control[] {
                lblAppointments, lstAppointments,
                btnAddAppointment, btnViewHistory, btnLogout
            });





            // Добавляем все на форму
            this.Controls.Add(topPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);





            // Обработчик изменения размера для кнопки закрытия
            this.Resize += (s, e) =>
            {
                btnClose.Location = new Point(this.Width - 50, 20);
            };
        }





        // Метод для скругления углов панелей
        private void ApplyRoundedCorners(Panel panel, int radius)
        {
            panel.Paint += (sender, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();

                    // Закрашиваем фон панели
                    using (var brush = new SolidBrush(panel.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Устанавливаем регион для кликабельности
                    panel.Region = new Region(path);
                }
            };
        }





        private Button CreateStyledButton(string text, Point location, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };





            // Скругляем углы кнопки
            btn.Paint += (sender, e) =>
            {
                Button b = sender as Button;
                using (var path = new GraphicsPath())
                {
                    int radius = 10;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(b.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(b.Width - radius, b.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, b.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    b.Region = new Region(path);
                }
            };

            // Эффекты при наведении
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = ControlPaint.Light(color);
                btn.ForeColor = darkBgColor;
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = color;
                btn.ForeColor = Color.White;
            };

            return btn;
        }





        private void LstPets_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = (ListBox)sender;
            var pet = (Pet)listBox.Items[e.Index];

            e.DrawBackground();

            // Цвет фона для выбранного элемента
            Color backColor;
            Color itemTextColor;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = mintColor;
                itemTextColor = darkBgColor;
            }
            else
            {
                backColor = inputBgColor;
                itemTextColor = this.textColor;
            }

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            using (var brush = new SolidBrush(itemTextColor))
            using (var nameFont = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var breedFont = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString(pet.Name, nameFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 8);
                e.Graphics.DrawString($"{pet.Breed}, {pet.Age} лет", breedFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 25);
            }

            e.DrawFocusRectangle();
        }





        private void LstAppointments_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = (ListBox)sender;
            var appointment = (Appointment)listBox.Items[e.Index];

            e.DrawBackground();

            // Цвет в зависимости от статуса
            Color statusColor;
            string statusText;

            if (appointment.Status == "Scheduled")
            {
                statusColor = mintColor;
                statusText = "ЗАПЛАНИРОВАН";
            }
            else if (appointment.Status == "Completed")
            {
                statusColor = Color.FromArgb(72, 199, 142);
                statusText = "ЗАВЕРШЕН";
            }
            else if (appointment.Status == "Cancelled")
            {
                statusColor = cancelColor;
                statusText = "ОТМЕНЕН";
            }
            else
            {
                statusColor = mintColor;
                statusText = appointment.Status;
            }






            // Фон элемента
            using (var backBrush = new SolidBrush(inputBgColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }



            // Ммаленькая полоска статуса слева
            using (var statusBrush = new SolidBrush(statusColor))
            {
                e.Graphics.FillRectangle(statusBrush, e.Bounds.Left, e.Bounds.Top, 5, e.Bounds.Height);
            }




            using (var brush = new SolidBrush(textColor))
            using (var boldFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var normalFont = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString(appointment.PetName, boldFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 8);
                e.Graphics.DrawString($"Дата: {appointment.AppointmentDate:dd.MM.yyyy HH:mm}", normalFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 28);

                if (!string.IsNullOrEmpty(appointment.Reason))
                {
                    e.Graphics.DrawString($"Причина: {appointment.Reason}", normalFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 43);
                }



                //статус справа
                SizeF statusSize = e.Graphics.MeasureString(statusText, new Font("Segoe UI", 8, FontStyle.Bold));
                using (var statusBrush = new SolidBrush(statusColor))
                {
                    e.Graphics.DrawString(statusText, new Font("Segoe UI", 8, FontStyle.Bold),
                        statusBrush, e.Bounds.Right - statusSize.Width - 15, e.Bounds.Top + 8);
                }
            }

            e.DrawFocusRectangle();
        }











        private void LoadData()
        {
            var pets = dbHelper.GetUserPets(currentUser.Id);
            lstPets.DataSource = pets;
            lstPets.DisplayMember = "Name";

            var appointments = dbHelper.GetUserAppointments(currentUser.Id);
            lstAppointments.DataSource = appointments;
        }

        private void LstPets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnViewHistory.Enabled = lstPets.SelectedItem != null;
        }

        private void BtnAddPet_Click(object sender, EventArgs e)
        {
            var petForm = CreateStyledDialog("Добавление питомца", 500);

            var txtName = CreateStyledTextBox(new Point(150, 50), 250);
            var cmbSpecies = CreateStyledComboBox(new Point(150, 100), 250);
            cmbSpecies.Items.AddRange(new[] { "Собака", "Кошка", "Птица", "Грызун", "Другое" });

            var txtBreed = CreateStyledTextBox(new Point(150, 150), 250);
            var nudAge = new NumericUpDown
            {
                Location = new Point(150, 200),
                Size = new Size(250, 30),
                Maximum = 30,
                Minimum = 0,
                Value = 1,
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };

            petForm.Controls.AddRange(new Control[] {
                CreateStyledLabel("ИМЯ:", new Point(50, 50)),
                txtName,
                CreateStyledLabel("ВИД:", new Point(50, 100)),
                cmbSpecies,
                CreateStyledLabel("ПОРОДА:", new Point(50, 150)),
                txtBreed,
                CreateStyledLabel("ВОЗРАСТ:", new Point(50, 200)),
                nudAge
            });

            Button btnSave = CreateDialogButton("СОХРАНИТЬ", new Point(100, 300), mintColor);
            btnSave.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите имя питомца!");
                    return;
                }

                var pet = new Pet
                {
                    Name = txtName.Text,
                    Species = cmbSpecies.SelectedItem?.ToString() ?? "Другое",
                    Breed = txtBreed.Text,
                    Age = (int)nudAge.Value,
                    OwnerId = currentUser.Id
                };

                if (dbHelper.AddPet(pet))
                {
                    MessageBox.Show("Питомец успешно добавлен!");
                    petForm.Close();
                    LoadData();
                }
            };

            Button btnCancel = CreateDialogButton("ОТМЕНА", new Point(270, 300), cancelColor);
            btnCancel.Click += (s, args) => petForm.Close();

            petForm.Controls.Add(btnSave);
            petForm.Controls.Add(btnCancel);
            petForm.ShowDialog();
        }

        private void BtnAddAppointment_Click(object sender, EventArgs e)
        {
            var pets = dbHelper.GetUserPets(currentUser.Id);
            if (pets.Count == 0)
            {
                MessageBox.Show("Сначала добавьте питомца!");
                return;
            }

            var appForm = CreateStyledDialog("Запись на прием", 600);

            // ИСПРАВЛЕННЫЙ ComboBox для питомцев
            var cmbPets = new ComboBox
            {
                Location = new Point(150, 50),
                Size = new Size(300, 30),
                DataSource = pets.ToList(),
                DisplayMember = "Name",
                ValueMember = "Id",
                BackColor = inputBgColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                DrawMode = DrawMode.OwnerDrawFixed
            };

            // Правильный обработчик DrawItem для отображения имени питомца
            cmbPets.DrawItem += (s, drawArgs) =>
            {
                drawArgs.DrawBackground();
                if (drawArgs.Index >= 0)
                {
                    var pet = cmbPets.Items[drawArgs.Index] as Pet;
                    if (pet != null)
                    {
                        using (Brush brush = new SolidBrush(textColor))
                        {
                            drawArgs.Graphics.DrawString(pet.Name, drawArgs.Font, brush,
                                drawArgs.Bounds.Left, drawArgs.Bounds.Top);
                        }
                    }
                }
                drawArgs.DrawFocusRectangle();
            };

            var dtpDate = new DateTimePicker
            {
                Location = new Point(150, 100),
                Size = new Size(300, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy",
                MinDate = DateTime.Now.Date,
                MaxDate = DateTime.Now.AddMonths(1),
                BackColor = inputBgColor,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10)
            };

            var lstTimeSlots = new ListBox
            {
                Location = new Point(150, 150),
                Size = new Size(300, 120),
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 25
            };

            // Обработчик DrawItem для отображения временных слотов
            lstTimeSlots.DrawItem += (s, drawArgs) =>
            {
                drawArgs.DrawBackground();
                if (drawArgs.Index >= 0)
                {
                    var slot = lstTimeSlots.Items[drawArgs.Index] as AvailableTimeSlot;
                    if (slot != null)
                    {
                        string displayText = slot.DisplayTime;
                        if (!slot.IsAvailable)
                        {
                            displayText += " (занято)";
                            using (Brush redBrush = new SolidBrush(cancelColor))
                            {
                                drawArgs.Graphics.DrawString(displayText, drawArgs.Font, redBrush,
                                    drawArgs.Bounds.Left, drawArgs.Bounds.Top);
                            }
                        }
                        else
                        {
                            using (Brush brush = new SolidBrush(textColor))
                            {
                                drawArgs.Graphics.DrawString(displayText, drawArgs.Font, brush,
                                    drawArgs.Bounds.Left, drawArgs.Bounds.Top);
                            }
                        }
                    }
                }
                drawArgs.DrawFocusRectangle();
            };

            var txtReason = new TextBox
            {
                Location = new Point(150, 290),
                Size = new Size(300, 70),
                Multiline = true,
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };

            dtpDate.ValueChanged += (s, args) =>
            {
                var slots = dbHelper.GetAvailableTimeSlots(dtpDate.Value);
                lstTimeSlots.DataSource = null;
                lstTimeSlots.DataSource = slots;
            };

            var initialSlots = dbHelper.GetAvailableTimeSlots(dtpDate.Value);
            lstTimeSlots.DataSource = initialSlots;

            appForm.Controls.AddRange(new Control[] {
                CreateStyledLabel("ПИТОМЕЦ:", new Point(50, 50)),
                cmbPets,
                CreateStyledLabel("ДАТА:", new Point(50, 100)),
                dtpDate,
                CreateStyledLabel("ВРЕМЯ:", new Point(50, 150)),
                lstTimeSlots,
                CreateStyledLabel("ПРИЧИНА:", new Point(50, 290)),
                txtReason
            });

            Button btnSave = CreateDialogButton("ЗАПИСАТЬСЯ", new Point(100, 400), mintColor);
            btnSave.Click += (s, args) =>
            {
                if (lstTimeSlots.SelectedItem == null)
                {
                    MessageBox.Show("Выберите время приема!");
                    return;
                }

                var selectedSlot = (AvailableTimeSlot)lstTimeSlots.SelectedItem;
                if (!selectedSlot.IsAvailable)
                {
                    MessageBox.Show("Это время уже занято!");
                    return;
                }

                var appointment = new Appointment
                {
                    PetId = ((Pet)cmbPets.SelectedItem).Id,
                    OwnerId = currentUser.Id,
                    AppointmentDate = selectedSlot.DateTime,
                    Reason = txtReason.Text
                };

                if (dbHelper.CreateAppointment(appointment))
                {
                    MessageBox.Show("Запись успешно создана!");
                    appForm.Close();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Это время уже занято. Попробуйте другое.");
                    var slots = dbHelper.GetAvailableTimeSlots(dtpDate.Value);
                    lstTimeSlots.DataSource = null;
                    lstTimeSlots.DataSource = slots;
                }
            };

            Button btnCancel = CreateDialogButton("ОТМЕНА", new Point(300, 400), cancelColor);
            btnCancel.Click += (s, args) => appForm.Close();

            appForm.Controls.Add(btnSave);
            appForm.Controls.Add(btnCancel);
            appForm.ShowDialog();
        }

        private Form CreateStyledDialog(string title, int height)
        {
            var form = new Form
            {
                Text = title,
                Size = new Size(500, height),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = panelBgColor
            };

            // Заголовок формы
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = mintColor,
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            form.Controls.Add(titleLabel);

            // Линия под заголовком
            Panel linePanel = new Panel
            {
                Location = new Point(20, 45),
                Size = new Size(460, 2),
                BackColor = mintColor
            };
            form.Controls.Add(linePanel);

            // Скругление формы
            form.Paint += (s, ev) =>
            {
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 30, 30, 180, 90);
                    path.AddArc(form.Width - 30, 0, 30, 30, 270, 90);
                    path.AddArc(form.Width - 30, form.Height - 30, 30, 30, 0, 90);
                    path.AddArc(0, form.Height - 30, 30, 30, 90, 90);
                    path.CloseAllFigures();
                    form.Region = new Region(path);
                }
            };

            return form;
        }

        private Label CreateStyledLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = new Size(90, 30),
                ForeColor = mintColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
        }

        private TextBox CreateStyledTextBox(Point location, int width)
        {
            return new TextBox
            {
                Location = location,
                Size = new Size(width, 30),
                BackColor = inputBgColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
        }

        private ComboBox CreateStyledComboBox(Point location, int width)
        {
            ComboBox cmb = new ComboBox
            {
                Location = location,
                Size = new Size(width, 30),
                BackColor = inputBgColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                DrawMode = DrawMode.OwnerDrawFixed
            };

            cmb.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    using (Brush brush = new SolidBrush(textColor))
                    {
                        e.Graphics.DrawString(cmb.Items[e.Index].ToString(), e.Font, brush, e.Bounds.Left, e.Bounds.Top);
                    }
                }
            };

            return cmb;
        }

        private Button CreateDialogButton(string text, Point location, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // Скругляем углы кнопки
            btn.Paint += (sender, e) =>
            {
                Button b = sender as Button;
                using (var path = new GraphicsPath())
                {
                    int radius = 8;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(b.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(b.Width - radius, b.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, b.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    b.Region = new Region(path);
                }
            };

            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(color);
            btn.MouseLeave += (s, e) => btn.BackColor = color;

            return btn;
        }

        private void BtnViewHistory_Click(object sender, EventArgs e)
        {
            if (lstPets.SelectedItem is Pet selectedPet)
            {
                var historyForm = new MedicalHistoryForm(selectedPet, currentUser);
                historyForm.ShowDialog();
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