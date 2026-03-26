using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using vc.Database;


using vc.Models;

namespace vc.Forms
{
    public class AddMedicalRecordForm : Form
    {
        private Pet pet;
        private Appointment appointment ;
        private User doctor ;
        private DatabaseHelper dbHelper ;

        private TextBox txtDiagnosis;
        private TextBox txtTreatment ;
        private TextBox txtPrescription;

        private Button btnSave;// Кнопка сохранения
        private Button btnCancel; // Кнопка отмены


        private Label lblTitle;// Заголовок формы

        public AddMedicalRecordForm(Pet selectedPet, Appointment selectedAppointment, User   currentDoctor)
        {
            // инициализация полей переданными данными
            pet = selectedPet;
            appointment = selectedAppointment;
            doctor = currentDoctor;
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
            btnSave.Paint += (s, e) => RoundButton(s as Button, 15);
            btnCancel.Paint += (s, e) => RoundButton(s as Button, 15);
        }

        private void RoundButton(Button btn, int radius)
        {
            if (btn == null) return;
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                btn.Region = new Region(path);
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"Медицинская запись - {pet.Name}";
            this.Size = new Size(650, 600);
            this.MinimumSize = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None; 
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 18); // Черный фон

            int yPos = 130;
            int spacing = 45;

            // Заголовок
            lblTitle = new Label
            {
                Text = $"Медицинская запись для {pet.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(169, 223, 191), // Мятный
                Location = new Point(20, 15),
                Size = new Size(500, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Подзаголовок с информацией о животном
            var lblSpecies = new Label
            {
                Text = $"Вид: {pet.Species} | Порода: {pet.Breed} | Возраст: {pet.Age} лет",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(20, 50),
                Size = new Size(500, 25)
            };
            this.Controls.Add(lblSpecies);





            if (appointment != null)
            {
                var lblAppointment = new Label
                {
                    Text = $"Прием: {appointment.AppointmentDate:dd.MM.yyyy HH:mm}",
                    ForeColor = Color.FromArgb(169, 223, 191),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(20, 75),
                    Size = new Size(400, 25)
                };
                this.Controls.Add(lblAppointment);
                yPos += 20;
            }
            else
            {
                yPos += 25;
            }

            // Диагноз
            var lblDiagnosis = new Label
            {
                Text = "Диагноз:*",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(20, yPos),
                Size = new Size(120, 25)
            };

            txtDiagnosis = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(430, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11),




            };

            // Эффекты при фокусе
            txtDiagnosis.Enter += (s, e) => txtDiagnosis.BackColor = Color.FromArgb(60, 60, 60);
            txtDiagnosis.Leave += (s, e) => txtDiagnosis.BackColor = Color.FromArgb(45, 45, 45);

            // Скругление для текстового поля
            txtDiagnosis.Paint += (s, e) => RoundTextBox(s as TextBox, 8);

            this.Controls.AddRange(new Control[] { lblDiagnosis, txtDiagnosis });
            yPos += spacing;

            // Лечение
            var lblTreatment = new Label
            {
                Text = "Лечение:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(20, yPos),
                Size = new Size(120, 25)
            };

            txtTreatment = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(430, 100),
                Multiline = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11)
            };

            txtTreatment.Enter += (s, e) => txtTreatment.BackColor = Color.FromArgb(60, 60, 60);
            txtTreatment.Leave += (s, e) => txtTreatment.BackColor = Color.FromArgb(45, 45, 45);
            txtTreatment.Paint += (s, e) => RoundTextBox(s as TextBox, 8);

            this.Controls.AddRange(new Control[] { lblTreatment, txtTreatment });
            yPos += 110;

            // Назначения
            var lblPrescription = new Label
            {
                Text = "Назначения:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(20, yPos),
                Size = new Size(120, 25)
            };

            txtPrescription = new TextBox
            {
                Location = new Point(150, yPos - 2),
                Size = new Size(430, 100),
                Multiline = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11)
            };

            txtPrescription.Enter += (s, e) => txtPrescription.BackColor = Color.FromArgb(60, 60, 60);
            txtPrescription.Leave += (s, e) => txtPrescription.BackColor = Color.FromArgb(45, 45, 45);
            txtPrescription.Paint += (s, e) => RoundTextBox(s as TextBox, 8);

            this.Controls.AddRange(new Control[] { lblPrescription, txtPrescription });
            yPos += 110;

            // Кнопка сохранения
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(180, yPos + 15),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(169, 223, 191), // Мятный
                ForeColor = Color.FromArgb(18, 18, 18), // Черный текст
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(149, 203, 171);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(169, 223, 191);

            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(340, yPos + 15),
                Size = new Size(120, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(18, 18, 18),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };





            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(230, 230, 230);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.White;





            // Кнопка закрытия (кастомная)
            Button btnClose = new Button
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



            // Добавляем все элементы управления на форму
            this.Controls.AddRange(new Control[] {
                lblTitle,
                btnSave,
                btnCancel,
                btnClose
            });
        }

        private void RoundTextBox(TextBox tb, int radius)
        {
            if (tb == null) return;
            using (var path = new GraphicsPath())
            {
                // Создаем скругленный путь для текстового поля
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(tb.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(tb.Width - radius, tb.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, tb.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                tb.Region = new Region(path);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiagnosis.Text))
            {
                MessageBox.Show("Введите диагноз!");
                return;
            }

            var record = new MedicalRecord
            {
                PetId = pet.Id,
                AppointmentId = appointment?.Id,
                Diagnosis = txtDiagnosis.Text,
                Treatment = txtTreatment.Text,
                Prescription = txtPrescription.Text,
                DoctorId = doctor.Id
            };

            if (dbHelper.AddMedicalRecord(record))
            {
                MessageBox.Show("Медицинская запись добавлена!");
                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении записи!");
            }
        }



    }
}




