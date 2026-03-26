using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using vc.Database;


using vc.Models;
using vc.Forms;

namespace vc.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername ;
        private TextBox txtPassword ;

        private Button  btnLogin;
        private Button btnRegister;


        private Label  lblTitle; 
        private Label lblUsername ;
        private Label lblPassword;


        private DatabaseHelper dbHelper;

        //  для капчи
        private Label captchaLabel;
        private PictureBox pictureBoxCaptcha;
        private TextBox txtCaptcha;
        private Button btnRefreshCaptcha;
        private string captchaCode;
        private Random random = new Random();

        public LoginForm()
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

            // Скругление для кнопок
            Action<Button> roundButton = (btn) =>
            {
                btn.Paint += (s, e) =>
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
            };

            roundButton(btnLogin);
            roundButton(btnRegister);
            roundButton(btnRefreshCaptcha);

            // скругление для текстовых полей
            Action<TextBox> roundTextBox = (tb) =>
            {
                tb.Paint += (s, e) =>
                {
                    TextBox textBox = s as TextBox;
                    using (var path = new GraphicsPath())
                    {
                        path.AddArc(0, 0, 10, 10, 180, 90);
                        path.AddArc(textBox.Width - 10, 0, 10, 10, 270, 90);
                        path.AddArc(textBox.Width - 10, textBox.Height - 10, 10, 10, 0, 90);
                        path.AddArc(0, textBox.Height - 10, 10, 10, 90, 90);
                        path.CloseFigure();
                        textBox.Region = new Region(path);
                    }
                };
            };

            roundTextBox(txtUsername);
            roundTextBox(txtPassword);
            roundTextBox(txtCaptcha);
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Вход в систему - Ветеринарная клиника";
            this.Size = new Size(420, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None; 
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 18); // Черный фон




            // Заголовок формы
            lblTitle = new Label
            {
                Text = "Ветеринарная клиника",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(169, 223, 191), // Мятный
                Location = new Point(60, 30),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };






            // Лейбл Логин
            lblUsername = new Label
            {
                Text = "Логин:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 90),
                Size = new Size(80, 25)
            };






            // Поле Логин
            txtUsername = new TextBox
            {
                Location = new Point(140, 90),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(45, 45, 45), // Темно-серый
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            txtUsername.Enter += (s, e) => txtUsername.BackColor = Color.FromArgb(60, 60, 60);
            txtUsername.Leave += (s, e) => txtUsername.BackColor = Color.FromArgb(45, 45, 45);




            // Лейбл Пароль
            lblPassword = new Label
            {
                Text = "Пароль:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 130),
                Size = new Size(80, 25)
            };






            // Поле Пароль
            txtPassword = new TextBox
            {
                Location = new Point(140, 130),
                Size = new Size(200, 30),
                PasswordChar = '*',
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            txtPassword.Enter += (s, e) => txtPassword.BackColor = Color.FromArgb(60, 60, 60);
            txtPassword.Leave += (s, e) => txtPassword.BackColor = Color.FromArgb(45, 45, 45);






            // Заголовок капчи
            captchaLabel = new Label
            {
                Text = "Код подтверждения:",
                ForeColor = Color.White,
                Location = new Point(50, 170),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10)
            };




            // PictureBox для капчи
            pictureBoxCaptcha = new PictureBox
            {
                Location = new Point(50, 200),
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(45, 45, 45),
                BorderStyle = BorderStyle.None
            };
            // Скругление для PictureBox
            pictureBoxCaptcha.Paint += (s, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 10, 10, 180, 90);
                    path.AddArc(pictureBoxCaptcha.Width - 10, 0, 10, 10, 270, 90);
                    path.AddArc(pictureBoxCaptcha.Width - 10, pictureBoxCaptcha.Height - 10, 10, 10, 0, 90);
                    path.AddArc(0, pictureBoxCaptcha.Height - 10, 10, 10, 90, 90);
                    path.CloseFigure();
                    pictureBoxCaptcha.Region = new Region(path);
                }
            };







            // Кнопка обновления капчи
            btnRefreshCaptcha = new Button
            {
                Text = "↻",
                Location = new Point(215, 200),
                Size = new Size(45, 45),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(169, 223, 191), // Мятный
                ForeColor = Color.FromArgb(18, 18, 18), // Черный текст
                Cursor = Cursors.Hand
            };
            btnRefreshCaptcha.FlatAppearance.BorderSize = 0;
            btnRefreshCaptcha.Click += (s, e) => GenerateCaptcha();







            // Поле ввода капчи
            txtCaptcha = new TextBox
            {
                Location = new Point(50, 255),
                Size = new Size(210, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            txtCaptcha.Enter += (s, e) => txtCaptcha.BackColor = Color.FromArgb(60, 60, 60);
            txtCaptcha.Leave += (s, e) => txtCaptcha.BackColor = Color.FromArgb(45, 45, 45);







            // Кнопка Войти
            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(70, 310),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(169, 223, 191), // Мятный
                ForeColor = Color.FromArgb(18, 18, 18), // Черный текст
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(149, 203, 171); // Темнее мятный
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(169, 223, 191);






            // Кнопка Регистрация
            btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(210, 310),
                Size = new Size(120, 40),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(18, 18, 18), // Черный текст
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(230, 230, 230); // Светло-серый
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.White;






            // Кнопка закрытия (кастомная)
            Button btnClose = new Button
            {
                Text = "✕",
                Location = new Point(370, 10),
                Size = new Size(30, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(169, 223, 191);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(45, 45, 45);





            // дбавляем все элементы
            this.Controls.AddRange(new Control[] {
                lblTitle, lblUsername, txtUsername,
                lblPassword, txtPassword,
                captchaLabel, pictureBoxCaptcha, txtCaptcha, btnRefreshCaptcha,
                btnLogin, btnRegister, btnClose
            });

            // генерируем первую капчу при загрузке
            this.Load += (s, e) => GenerateCaptcha();
        }




        private void GenerateCaptcha()
        {
            try
            {
                // Генерируем случайный код (6 цифр)
                captchaCode = random.Next(100000, 999999).ToString();

                // Создаем изображение
                Bitmap bitmap = new Bitmap(160, 45);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Заливаем фон темно-серым
                    graphics.Clear(Color.FromArgb(45, 45, 45));

                    // Добавляем шум - случайные линии
                    for (int i = 0; i < 5; i++)
                    {
                        Pen pen = new Pen(Color.FromArgb(100, 169, 223, 191), 1);
                        graphics.DrawLine(pen, random.Next(160), random.Next(45),
                            random.Next(160), random.Next(45));
                    }

                    // Рисуем текст белым цветом
                    Font font = new Font("Arial", 18, FontStyle.Bold | FontStyle.Italic);

                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        // Каждая цифра со случайным смещением
                        Brush brush = new SolidBrush(Color.White);
                        graphics.DrawString(captchaCode[i].ToString(), font, brush,
                            5 + i * 22, 5 + random.Next(-3, 3));
                    }
                }

                // Добавляем случайные точки
                for (int i = 0; i < 30; i++)
                {
                    bitmap.SetPixel(random.Next(160), random.Next(45),
                        Color.FromArgb(random.Next(100, 200), 169, 223, 191));
                }

                pictureBoxCaptcha.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при генерации капчи: " + ex.Message);
            }
        }






        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка капчи
            if (string.IsNullOrWhiteSpace(txtCaptcha.Text))
            {
                MessageBox.Show("Введите код подтверждения!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtCaptcha.Text != captchaCode)
            {
                MessageBox.Show("Неверный код подтверждения!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                GenerateCaptcha(); // Обновляем капчу
                txtCaptcha.Clear();
                return;
            }

            var user = dbHelper.Login(txtUsername.Text, txtPassword.Text);

            if (user != null)
            {
                MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успешно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (user.Role == "Admin")
                {
                    var adminForm = new AdminForm(user);
                    adminForm.Show();
                }
                else
                {
                    var mainForm = new MainForm(user);
                    mainForm.Show();
                }
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                GenerateCaptcha(); // Обновляем капчу при ошибке
                txtCaptcha.Clear();
            }
        }






        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}