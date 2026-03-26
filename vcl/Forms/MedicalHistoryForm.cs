using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using vc.Database;
using vc.Models;

namespace vc.Forms
{
    public class MedicalHistoryForm : Form
    {
        private Pet pet;
        private User currentUser ;
        private DatabaseHelper dbHelper;
        private ListBox lstRecords;
        private TextBox txtDetails;
        private Button btnClose;

        // Цветовая палитра из MainForm
        private readonly Color MintColor = Color.FromArgb(169, 223, 191);      // Основной акцентный цвет
        private readonly Color DarkMintColor = Color.FromArgb(149, 203, 171);  // Дополнительный акцентный цвет
        private readonly Color DarkBgColor = Color.FromArgb(18, 18, 18);       // Глубокий черный фон формы
        private readonly Color PanelBgColor = Color.FromArgb(30, 30, 30);      // Темно-серый фон панелей
        private readonly Color InputBgColor = Color.FromArgb(45, 45, 45);      // Серый фон для полей ввода
        private readonly Color TextColor = Color.FromArgb(237, 242, 247);      // Светлый текст
        private readonly Color CancelColor = Color.FromArgb(220, 80, 80);      // Красный для отмены/выхода

        public MedicalHistoryForm(Pet selectedPet, User user)
        {
            pet = selectedPet;
            currentUser = user;
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            LoadHistory();
        }
         



        private void InitializeComponent()
        {
            this.Text = $"История болезней - {pet.Name}";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None; // Убираем стандартный заголовок
            this.BackColor = DarkBgColor; // Глубокий черный фон
            this.ForeColor = TextColor; // Светлый текст




            // Создаем скругленные углы для формы
            this.Paint += (sender, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    int radius = 20;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    this.Region = new Region(path);
                }
            };




            // Верхняя панель с заголовком 
            Panel topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = PanelBgColor
            };





            // Кнопка закрытия 
            Button btnCloseWindow = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = MintColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 15),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnCloseWindow.Click += (s, e) => this.Close();
            btnCloseWindow.MouseEnter += (s, e) => btnCloseWindow.ForeColor = Color.White;
            btnCloseWindow.MouseLeave += (s, e) => btnCloseWindow.ForeColor = MintColor;
            




            // заголовок в верхней панели
            Label lblTitle = new Label
            {
                Text = $"Медицинская история: {pet.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = MintColor,
                Location = new Point(20, 18),
                AutoSize = true,
                BackColor = Color.Transparent
            };







            // Информация о питомце
            Label lblPetInfo = new Label
            {
                Text = $"{pet.Species} • {pet.Breed} • {pet.Age} лет",
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkMintColor,
                Location = new Point(20, 45),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            topPanel.Controls.AddRange(new Control[] { btnCloseWindow, lblTitle, lblPetInfo });







            // Основная панель 
            Panel contentPanel = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(660, 460),
                BackColor = PanelBgColor
            };

            // Скругляем углы панели 
            ApplyRoundedCorners(contentPanel, 15);



            // Подзаголовок "Записи"
            var lblRecords = new Label
            {
                Text = "📋 ЗАПИСИ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 15),
                Size = new Size(150, 25),
                ForeColor = MintColor,
                BackColor = Color.Transparent
            };





            // Список записей
            lstRecords = new ListBox
            {
                Location = new Point(15, 45),
                Size = new Size(280, 340),
                BackColor = InputBgColor,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            lstRecords.SelectedIndexChanged += LstRecords_SelectedIndexChanged;
            lstRecords.DrawMode = DrawMode.OwnerDrawFixed;
            lstRecords.DrawItem += LstRecords_DrawItem;
            lstRecords.ItemHeight = 35;






            // Подзаголовок "Детали"
            var lblDetails = new Label
            {
                Text = "📄 ДЕТАЛИ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(320, 15),
                Size = new Size(150, 25),
                ForeColor = MintColor,
                BackColor = Color.Transparent
            };





            // Текстовое поле для деталей
            txtDetails = new TextBox
            {
                Location = new Point(320, 45),
                Size = new Size(320, 340),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = InputBgColor,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };





            // Кнопка закрытия (стилизованная под MainForm)
            btnClose = new Button
            {
                Text = "✖ ЗАКРЫТЬ",
                Location = new Point(500, 400),
                Size = new Size(140, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = CancelColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };





            // скругляем углы кнопки
            btnClose.Paint += (sender, e) =>
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





            // Эффекты при наведении на кнопку
            btnClose.MouseEnter += (s, e) =>
            {
                btnClose.BackColor = ControlPaint.Light(CancelColor);
            };
            btnClose.MouseLeave += (s, e) =>
            {
                btnClose.BackColor = CancelColor;
            };
            btnClose.Click += (s, e) => this.Close();








            // Добавляем элементы на панель контента
            contentPanel.Controls.AddRange(new Control[] {
                lblRecords,
                lstRecords,
                lblDetails,
                txtDetails,
                btnClose
            });





            // Добавляем все на форму
            this.Controls.Add(topPanel);
            this.Controls.Add(contentPanel);





            // Обработчик изменения размера для кнопки закрытия
            this.Resize += (s, e) =>
            {
                btnCloseWindow.Location = new Point(this.Width - 50, 15);
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




                    using (var brush = new SolidBrush(panel.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }



                    panel.Region = new Region(path);
                }
            };
        }





        // Кастомная отрисовка элементов списка
        private void LstRecords_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = (ListBox)sender;
            var record = (MedicalRecord)listBox.Items[e.Index];



            e.DrawBackground();



            // Цвет фона для выбранного элемента
            Color backColor;
            Color itemTextColor;


            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = MintColor;
                itemTextColor = DarkBgColor;



            }
            else
            {
                backColor = InputBgColor;
                itemTextColor = TextColor;


            }

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);


            }






            // Рисуем небольшую мятную полоску слева
            using (var accentBrush = new SolidBrush(MintColor))
            {
                e.Graphics.FillRectangle(accentBrush, e.Bounds.Left, e.Bounds.Top, 3, e.Bounds.Height);
            }




            using (var brush = new SolidBrush(itemTextColor))
            using (var dateFont = new Font("Segoe UI", 10, FontStyle.Bold))

            //using (var doctorFont = new Font("Segoe UI", 8))
            {
                string dateStr = record.RecordDate.ToString("dd.MM.yyyy");
                //string timeStr = record.RecordDate.ToString("HH:mm");

                e.Graphics.DrawString(dateStr, dateFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 8);
                //e.Graphics.DrawString($"{timeStr} • {record.DoctorName}", doctorFont, brush, e.Bounds.Left + 15, e.Bounds.Top + 25);
            }

            e.DrawFocusRectangle();
        }





        private void LoadHistory()
        {
            var records = dbHelper.GetPetMedicalHistory(pet.Id);
            lstRecords.DataSource = records;
            lstRecords.DisplayMember = "RecordDate";

            if (lstRecords.Items.Count > 0)
            {
                lstRecords.SelectedIndex = 0;
            }



        }

        private void LstRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRecords.SelectedItem is MedicalRecord record)
            {
                txtDetails.Text = $"📅 ДАТА: {record.RecordDate:dd.MM.yyyy HH:mm}\r\n" +
                                 $"👨‍⚕️ ВРАЧ: {record.DoctorName}\r\n\r\n" +
                                 $"🔬 ДИАГНОЗ:\r\n{record.Diagnosis}\r\n\r\n" +
                                 $"💊 ЛЕЧЕНИЕ:\r\n{record.Treatment}\r\n\r\n" +
                                 $"📝 НАЗНАЧЕНИЯ:\r\n{record.Prescription}";





            }
        }
    }
}







