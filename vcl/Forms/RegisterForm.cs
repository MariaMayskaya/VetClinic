using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using vc.Database;
using vc.Models;

namespace vc.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtEmail;
        private TextBox txtFullName;
        private TextBox txtPhone;


        private Button btnRegister ;
        private Button btnCancel;
        private Button btnClose;

        private DatabaseHelper dbHelper;
        private Panel contentPanel;
        private Label lblTitle;
        

        public RegisterForm()
        {
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            ApplyRoundedCorners();
        }





        private void ApplyRoundedCorners()
        {
            // Скругление для формы
            this.Paint += (sender, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 30, 30, 180, 90);
                    path.AddArc(this.Width - 30, 0, 30, 30, 270, 90);
                    path.AddArc(this.Width - 30, this.Height - 30, 30, 30, 0, 90);
                    path.AddArc(0, this.Height - 30, 30, 30, 90, 90);
                    path.CloseFigure();
                    this.Region = new Region(path);
                }
            };
        }





        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Регистрация - Ветеринарная клиника";
            this.Size = new Size(550, 600);
            this.MinimumSize = new Size(520, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 18);

            // заголовок формы на форме, не на панели
            lblTitle = new Label
            {
                Text = "Регистрация нового пользователя",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(169, 223, 191),
                Location = new Point(0, 10),
                Size = new Size(this.ClientSize.Width, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };






            // Основная панель с автоскролломначинается после заголовка 
            contentPanel = new Panel
            {
                Location = new Point(0, 70), // Сдвигаем вниз, чтобы не перекрывать заголовок
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 80),
                BackColor = Color.FromArgb(18, 18, 18),
                AutoScroll = true,
                Padding = new Padding(20),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };





            contentPanel.Resize += (s, e) =>
            {
                contentPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 80);
            };




            int yPos = 10; // начинаем с отступом внутри панели





            // Добавляем поля с метками
            AddLabelAndTextBox("Логин:*", ref txtUsername, ref yPos);
            AddLabelAndTextBox("Пароль:*", ref txtPassword, ref yPos, true);
            AddLabelAndTextBox("Подтвердите пароль:*", ref txtConfirmPassword, ref yPos, true);
            AddLabelAndTextBox("Email:*", ref txtEmail, ref yPos);
            AddLabelAndTextBox("Полное имя:*", ref txtFullName, ref yPos);
            AddLabelAndTextBox("Телефон:", ref txtPhone, ref yPos);







            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(250, yPos + 10),  // Сдвинул левее, чтобы поместилась более широкая кнопка
                Size = new Size(200, 45),  // Увеличил ширину с 160 до 200
                BackColor = Color.FromArgb(169, 223, 191),
                ForeColor = Color.FromArgb(18, 18, 18),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(149, 203, 171);
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(169, 223, 191);






            // Скругление для кнопки регистрации
            btnRegister.Paint += (s, e) =>
            {
                Button b = s as Button;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 20, 20, 180, 90);
                    path.AddArc(b.Width - 20, 0, 20, 20, 270, 90);
                    path.AddArc(b.Width - 20, b.Height - 20, 20, 20, 0, 90);
                    path.AddArc(0, b.Height - 20, 20, 20, 90, 90);
                    path.CloseFigure();
                    b.Region = new Region(path);
                }
            };







            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(80, yPos + 10),
                Size = new Size(120, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(18, 18, 18),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(230, 230, 230);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.White;





            // Скругление для кнопки отмены
            btnCancel.Paint += (s, e) =>
            {
                Button b = s as Button;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 20, 20, 180, 90);
                    path.AddArc(b.Width - 20, 0, 20, 20, 270, 90);
                    path.AddArc(b.Width - 20, b.Height - 20, 20, 20, 0, 90);
                    path.AddArc(0, b.Height - 20, 20, 20, 90, 90);
                    path.CloseFigure();
                    b.Region = new Region(path);
                }
            };





            // Кнопка закрытия
            btnClose = new Button
            {
                Text = "✕",
                Location = new Point(this.ClientSize.Width - 45, 15),
                Size = new Size(35, 35),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(169, 223, 191);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(45, 45, 45);






            // Скругление для кнопки закрытия
            btnClose.Paint += (s, e) =>
            {
                Button b = s as Button;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 15, 15, 180, 90);
                    path.AddArc(b.Width - 15, 0, 15, 15, 270, 90);
                    path.AddArc(b.Width - 15, b.Height - 15, 15, 15, 0, 90);
                    path.AddArc(0, b.Height - 15, 15, 15, 90, 90);
                    path.CloseFigure();
                    b.Region = new Region(path);
                }
            };





            // Добавляем кнопки на панель
            contentPanel.Controls.Add(btnRegister);
            contentPanel.Controls.Add(btnCancel);





            // Добавляем все на форму: заголовок, панель, кнопку закрытия
            this.Controls.Add(lblTitle);
            this.Controls.Add(contentPanel);
            this.Controls.Add(btnClose);




            // Обновляем положение элементов при изменении размера
            this.Resize += (s, e) =>
            {
                lblTitle.Size = new Size(this.ClientSize.Width, 50);
                contentPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 80);
                btnClose.Location = new Point(this.ClientSize.Width - 45, 15);
            };
        }





        private void AddLabelAndTextBox(string labelText, ref TextBox textBox, ref int yPos, bool isPassword = false)
        {
            // Метка
            var label = new Label
            {
                Text = labelText,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(50, yPos),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };





            // Текстовое поле
            textBox = new TextBox
            {
                Location = new Point(210, yPos),
                Size = new Size(250, 35),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };





            if (isPassword)
                textBox.PasswordChar = '*';

            // Эффекты при фокусе
            TextBox currentTextBox = textBox;
            textBox.Enter += (s, e) => currentTextBox.BackColor = Color.FromArgb(60, 60, 60);
            textBox.Leave += (s, e) => currentTextBox.BackColor = Color.FromArgb(45, 45, 45);





            // Скругление для текстового поля
            textBox.Paint += (s, e) =>
            {
                TextBox tb = s as TextBox;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 12, 12, 180, 90);
                    path.AddArc(tb.Width - 12, 0, 12, 12, 270, 90);
                    path.AddArc(tb.Width - 12, tb.Height - 12, 12, 12, 0, 90);
                    path.AddArc(0, tb.Height - 12, 12, 12, 90, 90);
                    path.CloseFigure();
                    tb.Region = new Region(path);
                }
            };





            contentPanel.Controls.Add(label);
            contentPanel.Controls.Add(textBox);

            yPos += 55; // Увеличил отступ для лучшего расположения
        }





        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            var user = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                Email = txtEmail.Text,
                FullName = txtFullName.Text,
                Phone = txtPhone.Text
            };

            if (dbHelper.RegisterUser(user))
            {
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации. Возможно, такой пользователь уже существует.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private bool ValidateInputs()
        {



            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (отмечены *).",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }




            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }




            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный email адрес.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}









