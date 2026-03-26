using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using vc.Database;
using vc.Models;

namespace vc.Forms
{
    public class PharmacyForm : Form
    {
        private User currentUser;
        private DatabaseHelper dbHelper;
        private ListBox lstMedicines;
        private TextBox txtSearch;

        private Button btnAddMedicine;
        private Button btnEditMedicine;
        private Button btnUpdateStock;
        private Button btnDeleteMedicine;
        private Button btnRefresh;

        private Label lblLowStock;
        private Label lblTotalMedicines;

        private List<Medicine> medicines;
        private Panel panelButtons;
        private Panel panelInfo;

       
        private readonly Color MintLight = Color.FromArgb(230, 255, 250);  // Основной фон
        private readonly Color MintSoft = Color.FromArgb(178, 235, 220);   // Панели и контейнеры
        private readonly Color MintMedium = Color.FromArgb(102, 204, 178); // Акцентные кнопки
        private readonly Color MintDark = Color.FromArgb(38, 139, 120);    // Заголовки, важные элементы
        private readonly Color MintDarker = Color.FromArgb(22, 102, 88);   // Критичные действия
        private readonly Color TextColor = Color.FromArgb(20, 60, 50);     // Основной текст







        public PharmacyForm(User user)
        {
            currentUser = user;
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            LoadMedicines();

            // Проверка прав доступа
            if (currentUser.Role != "Admin")
            {
                
                
                MessageBox.Show("У вас нет прав доступа к аптеке!", "Доступ запрещен",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Аптека - Управление лекарственными средствами";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(900, 600);
            this.BackColor = MintLight; // Основной фон





            // Заголовок
            var lblTitle = new Label
            {
                Text = "УПРАВЛЕНИЕ АПТЕЧНЫМ СКЛАДОМ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = MintDark, // Заголовки темно-мятные
                Location = new Point(20, 20),
                Size = new Size(450, 35),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };





            // Панель поиска
            var searchPanel = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(960, 60),
                BackColor = MintSoft, // Панели мятные
                BorderStyle = BorderStyle.None
            };





            var lblSearch = new Label
            {
                Text = "🔍 Поиск:",
                Location = new Point(15, 18),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = TextColor, // Основной текст
                BackColor = Color.Transparent
            };





            txtSearch = new TextBox
            {
                Location = new Point(90, 15),
                Size = new Size(300, 27),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;





            var btnClearSearch = new Button
            {
                Text = "✕ Очистить",
                Location = new Point(400, 14),
                Size = new Size(90, 28),
                BackColor = MintMedium, // Акцентные кнопки
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = MintDark }
            };
            btnClearSearch.Click += (s, e) => txtSearch.Clear();





            // Статистика в панели поиска
            lblTotalMedicines = new Label
            {
                Text = "Всего: 0",
                Location = new Point(520, 18),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = MintDark, // Заголовки темно-мятные
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };





            lblLowStock = new Label
            {
                Text = "Низкий остаток: 0",
                Location = new Point(630, 18),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = MintDarker, 
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            searchPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnClearSearch, lblTotalMedicines, lblLowStock
            });




            // Панель со списком лекарств
            var listPanel = new Panel
            {
                Location = new Point(20, 140),
                Size = new Size(600, 450),
                BackColor = MintSoft, // Панели мятные
                BorderStyle = BorderStyle.None
            };





            var lblListHeader = new Label
            {
                Text = "📋 СПИСОК ЛЕКАРСТВ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(580, 30),
                ForeColor = MintDark, 
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };



            lstMedicines = new ListBox
            {
                Location = new Point(10, 45),
                Size = new Size(580, 395),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 70,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ScrollAlwaysVisible = true,
                BackColor = Color.White,
                ForeColor = TextColor
            };
            lstMedicines.DrawItem += LstMedicines_DrawItem;
            lstMedicines.SelectedIndexChanged += LstMedicines_SelectedIndexChanged;
            lstMedicines.MouseDoubleClick += LstMedicines_MouseDoubleClick;
            



            listPanel.Controls.AddRange(new Control[] { lblListHeader, lstMedicines });






            // Панель кнопок для администратора
            panelButtons = new Panel
            {
                Location = new Point(640, 140),
                Size = new Size(340, 450),
                BackColor = MintSoft, // Панели мятные
                BorderStyle = BorderStyle.None
            };





            var lblActionsHeader = new Label
            {
                Text = "⚡ ДЕЙСТВИЯ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(320, 30),
                ForeColor = MintDark, // Заголовки темно-мятные
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };





            int buttonY = 50;
            int buttonSpacing = 55;






            // Стилизованные кнопки
            btnAddMedicine = CreateStyledButton("➕ ДОБАВИТЬ ПРЕПАРАТ", 20, buttonY, 300, 45,
                MintMedium, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
            btnAddMedicine.Click += BtnAddMedicine_Click;





            btnEditMedicine = CreateStyledButton("✏️ РЕДАКТИРОВАТЬ", 20, buttonY + buttonSpacing, 300, 45,
                MintDark, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
            btnEditMedicine.Click += BtnEditMedicine_Click;
            btnEditMedicine.Enabled = false;





            btnUpdateStock = CreateStyledButton("📦 ОБНОВИТЬ ОСТАТОК", 20, buttonY + buttonSpacing * 2, 300, 45,
                MintMedium, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
            btnUpdateStock.Click += BtnUpdateStock_Click;

            btnUpdateStock.Enabled = false;




            btnDeleteMedicine = CreateStyledButton("🗑️ УДАЛИТЬ", 20, buttonY + buttonSpacing * 3, 300, 45,
                MintDarker, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
            btnDeleteMedicine.Click += BtnDeleteMedicine_Click;
            btnDeleteMedicine.Enabled = false;





            //// Кнопка обновления
            //btnRefresh = CreateStyledButton("🔄 ОБНОВИТЬ", 20, buttonY + buttonSpacing * 4, 145, 40,
            //    MintMedium, Color.White, new Font("Segoe UI", 9, FontStyle.Bold));
            //btnRefresh.Click += (s, e) => LoadMedicines();

            //// Кнопка закрытия
            //var btnClose = CreateStyledButton("✖ ЗАКРЫТЬ", 175, buttonY + buttonSpacing * 4, 145, 40,
            //    MintDarker, Color.White, new Font("Segoe UI", 9, FontStyle.Bold));
            //btnClose.Click += (s, e) => this.Close();










            panelButtons.Controls.AddRange(new Control[] {
                lblActionsHeader, btnAddMedicine, btnEditMedicine,
                btnUpdateStock, btnDeleteMedicine
                //btnUpdateStock, btnDeleteMedicine, btnRefresh, btnClose
            });





            // Добавление всех элементов на форму
            this.Controls.AddRange(new Control[] {
                lblTitle, searchPanel, listPanel, panelButtons
            });
        }




        private Button CreateStyledButton(string text, int x, int y, int width, int height,
            Color backColor, Color foreColor, Font font)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = font,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                FlatAppearance = { BorderSize = 0 }
            };







            // Добавляем эффект при наведении
            button.MouseEnter += (s, e) => button.BackColor = MintDark;
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

        private void LoadMedicines()
        {
            try
            {
                medicines = dbHelper.GetAllMedicines();
                FilterMedicines();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






        private void UpdateStatistics()
        {
            if (medicines != null)
            {
                int total = medicines.Count;
                int lowStock = medicines.Count(m => m.Quantity < 10);

                lblTotalMedicines.Text = $"📊 Всего: {total}";
                lblLowStock.Text = lowStock > 0 ? $"⚠️ Низкий остаток: {lowStock}" : "";
            }
        }





        private void FilterMedicines()
        {
            var filtered = medicines;
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filtered = medicines.Where(m =>
                    m.Name.ToLower().Contains(searchText) ||
                    (m.Description != null && m.Description.ToLower().Contains(searchText))
                ).ToList();
            }

            lstMedicines.DataSource = null;
            lstMedicines.DataSource = filtered;
            lstMedicines.DisplayMember = "Name";
        }





        private void LstMedicines_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = (ListBox)sender;
            var medicine = (Medicine)listBox.Items[e.Index];

            e.DrawBackground();

            // Определяем цвет фона в зависимости от статуса
            Color backColor;
            Color borderColor = MintSoft;


            if (medicine.Quantity == 0)
                backColor = Color.FromArgb(255, 240, 240 );  // Очень светлый красный для фона
            else if (medicine.Quantity < 10)
                backColor = Color.FromArgb(255, 250, 230 ); //  Очень светлый желтый для фона
            else if (medicine.ExpiryDate < DateTime.Now )
                backColor = Color.FromArgb(255, 240, 240 ); // Очень светлый красный для фона
            else
                backColor = Color.White; // Белый фон для нормального состояния

            using (var backBrush = new SolidBrush(backColor))
            {

                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }






            // Рисуем рамку
            using (var pen = new Pen(MintSoft))
            {
                e.Graphics.DrawRectangle(pen, e.Bounds.Left, e.Bounds.Top,
                    e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            using (var brush = new SolidBrush(TextColor))
            using (var boldFont = new Font(e.Font, FontStyle.Bold))
            using (var smallFont = new Font(e.Font.FontFamily, 8))
            {

                // Название лекарства
                e.Graphics.DrawString(medicine.Name, boldFont, brush,
                    e.Bounds.Left + 15, e.Bounds.Top + 8);




                // Информация о количестве и цене
                string quantityInfo = $"Количество: {medicine.Quantity} шт. | Цена: {medicine.Price:C}";
                e.Graphics.DrawString(quantityInfo, e.Font, brush,
                    e.Bounds.Left + 15, e.Bounds.Top + 30);




                // Срок годности
                string expiryInfo = $"Годен до: {medicine.ExpiryDate:dd.MM.yyyy}";
                using (var expiryBrush = new SolidBrush(
                    medicine.ExpiryDate < DateTime.Now ? MintDarker : MintDark))
                {
                    e.Graphics.DrawString(expiryInfo, smallFont, expiryBrush,
                        e.Bounds.Left + 15, e.Bounds.Top + 48);
                }





                // Статус
                string status = "";
                Color statusColor = Color.Transparent;




                if (medicine.Quantity == 0)
                {
                    status = "НЕТ В НАЛИЧИИ";
                    statusColor = MintDarker;
                }
                else if (medicine.Quantity < 10)
                {
                    status = "МАЛО";
                    statusColor = MintDark;
                }
                else if (medicine.ExpiryDate < DateTime.Now)
                {
                    status = "ПРОСРОЧЕНО";
                    statusColor = MintDarker;
                }

                if (!string.IsNullOrEmpty(status))
                {
                    using (var statusBrush = new SolidBrush(statusColor))
                    using (var statusFont = new Font(e.Font.FontFamily, 8, FontStyle.Bold))
                    {
                        var statusSize = e.Graphics.MeasureString(status, statusFont);
                        e.Graphics.DrawString(status, statusFont, statusBrush,
                            e.Bounds.Right - statusSize.Width - 15, e.Bounds.Top + 30);
                    }
                }
            }

            e.DrawFocusRectangle();
        }








        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterMedicines();
        }





        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FilterMedicines();
                e.SuppressKeyPress = true;
            }
        }





        private void LstMedicines_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelection = lstMedicines.SelectedItem != null;
            btnEditMedicine.Enabled = hasSelection;
            btnUpdateStock.Enabled = hasSelection;
            btnDeleteMedicine.Enabled = hasSelection;
        }





        private void LstMedicines_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstMedicines.SelectedItem != null)
            {
                BtnEditMedicine_Click(sender, e);
            }
        }





        private void BtnAddMedicine_Click(object sender, EventArgs e)
        {
            var addForm = new Form
            {
                Text = "➕ Добавление нового лекарства",
                Size = new Size(550, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = MintLight,
                ForeColor = TextColor
            };

            int yPos = 30;
            int spacing = 50;






            // Создаем элементы управления
            var txtName = CreateTextBox("", 150, yPos, 350);
            var txtDescription = CreateTextBox("", 150, yPos + spacing, 350, true);
            txtDescription.Height = 80;

            var nudQuantity = CreateNumericUpDown(150, yPos + spacing * 3, 350, 0, 10000);
            var nudPrice = CreateNumericUpDown(150, yPos + spacing * 4, 350, 0, 100000);
            nudPrice.DecimalPlaces = 2;

            var dtpExpiry = new DateTimePicker
            {
                Location = new Point(150, yPos + spacing * 5),
                Size = new Size(350, 25),
                MinDate = DateTime.Now,
                BackColor = Color.White,
                ForeColor = TextColor
            };






            // Добавляем метки
            addForm.Controls.AddRange(new Control[] {
                CreateLabel("Название:*", 20, yPos, 120),
                txtName,
                CreateLabel("Описание:", 20, yPos + spacing, 120),
                txtDescription,
                CreateLabel("Количество:*", 20, yPos + spacing * 3, 120),
                nudQuantity,
                CreateLabel("Цена:*", 20, yPos + spacing * 4, 120),
                nudPrice,
                CreateLabel("Срок годности:*", 20, yPos + spacing * 5, 120),
                dtpExpiry
            });





            var btnSave = CreateStyledButton("💾 СОХРАНИТЬ", 150, yPos + spacing * 6 + 30, 170, 40,
                MintMedium, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));





            btnSave.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название лекарства!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }




                var medicine = new Medicine
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Quantity = (int)nudQuantity.Value,
                    Price = nudPrice.Value,
                    ExpiryDate = dtpExpiry.Value
                };




                if (dbHelper.AddMedicine(medicine))
                {
                    MessageBox.Show("Лекарство успешно добавлено!", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    addForm.Close();
                    LoadMedicines();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении лекарства!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };





            var btnCancel = CreateStyledButton("✖ ОТМЕНА", 330, yPos + spacing * 6 + 30, 170, 40,
                MintDarker, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
            btnCancel.DialogResult = DialogResult.Cancel;

            addForm.Controls.AddRange(new Control[] { btnSave, btnCancel });
            addForm.ShowDialog();
        }

        private void BtnEditMedicine_Click(object sender, EventArgs e)
        {
            if (lstMedicines.SelectedItem is Medicine selected)
            {
                var editForm = new Form
                {
                    Text = $"✏️ Редактирование: {selected.Name}",
                    Size = new Size(550, 500),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = MintLight,
                    ForeColor = TextColor
                };





                //int yPos = 80;
                int yPos = 30;
                int spacing = 50;





                var txtName = CreateTextBox(selected.Name, 150, yPos, 350);
                var txtDescription = CreateTextBox(selected.Description, 150, yPos + spacing, 350, true);
                txtDescription.Height = 80;




                var nudQuantity = CreateNumericUpDown(150, yPos + spacing * 3, 350, 0, 10000, selected.Quantity);
                var nudPrice = CreateNumericUpDown(150, yPos + spacing * 4, 350, 0, 100000, selected.Price);
                nudPrice.DecimalPlaces = 2;





                var dtpExpiry = new DateTimePicker
                {
                    Value = selected.ExpiryDate,
                    Location = new Point(150, yPos + spacing * 5),
                    Size = new Size(350, 25),
                    MinDate = DateTime.Now,
                    BackColor = Color.White,
                    ForeColor = TextColor
                };




                editForm.Controls.AddRange(new Control[] {
                    CreateLabel("Название:*", 20, yPos, 120),
                    txtName,
                    CreateLabel("Описание:", 20, yPos + spacing, 120),
                    txtDescription,
                    CreateLabel("Количество:*", 20, yPos + spacing * 3, 120),
                    nudQuantity,
                    CreateLabel("Цена:*", 20, yPos + spacing * 4, 120),
                    nudPrice,
                    CreateLabel("Срок годности:*", 20, yPos + spacing * 5, 120),
                    dtpExpiry
                });





                var btnSave = CreateStyledButton("💾 СОХРАНИТЬ", 150, yPos + spacing * 6 + 30, 170, 40,
                    MintMedium, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));

                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show("Введите название лекарства!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }





                    selected.Name = txtName.Text;
                    selected.Description = txtDescription.Text;

                    selected.Quantity = (int)nudQuantity.Value;
                    selected.Price = nudPrice.Value;
                    selected.ExpiryDate = dtpExpiry.Value;

                    if (dbHelper.UpdateMedicine(selected))
                    {
                        MessageBox.Show("Лекарство успешно обновлено!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        editForm.Close();
                        LoadMedicines();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении лекарства!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };





                var btnCancel = CreateStyledButton("✖ ОТМЕНА", 330, yPos + spacing * 6 + 30, 170, 40,
                    MintDarker, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
                btnCancel.DialogResult = DialogResult.Cancel;

                editForm.Controls.AddRange(new Control[] { btnSave, btnCancel });
                editForm.ShowDialog();
            }
        }




        private void BtnUpdateStock_Click(object sender, EventArgs e)
        {
            if (lstMedicines.SelectedItem is Medicine selected)
            {
                var updateForm = new Form
                {
                    Text = $"📦 Обновление остатка: {selected.Name}",
                    Size = new Size(400, 200),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = MintLight,
                    ForeColor = TextColor
                };

                var nudNewQuantity = new NumericUpDown
                {
                    Location = new Point(20, 50),
                    Size = new Size(340, 25),
                    Minimum = 0,
                    Maximum = 10000,
                    Value = selected.Quantity,
                    BackColor = Color.White,
                    ForeColor = TextColor
                };

                var btnSave = CreateStyledButton("📦 ОБНОВИТЬ", 20, 100, 170, 35,
                    MintMedium, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));

                btnSave.Click += (s, args) =>
                {
                    if (dbHelper.UpdateMedicineQuantity(selected.Id, (int)nudNewQuantity.Value))
                    {
                        MessageBox.Show("Остаток успешно обновлен!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        updateForm.Close();
                        LoadMedicines();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении остатка!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                var btnCancel = CreateStyledButton("✖ ОТМЕНА", 200, 100, 160, 35,
                    MintDarker, Color.White, new Font("Segoe UI", 10, FontStyle.Bold));
                btnCancel.DialogResult = DialogResult.Cancel;

                updateForm.Controls.AddRange(new Control[] {
                    CreateLabel("Новое количество:", 20, 20, 340),
                    nudNewQuantity,
                    btnSave,
                    btnCancel
                });

                updateForm.ShowDialog();
            }
        }






        private void BtnDeleteMedicine_Click(object sender, EventArgs e)
        {
            if (lstMedicines.SelectedItem is Medicine selected)
            {
                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить лекарство '{selected.Name}'?\n\n" +
                    $"Количество: {selected.Quantity} шт.\n" +
                    $"Цена: {selected.Price:C}\n" +
                    $"Срок годности: {selected.ExpiryDate:dd.MM.yyyy}",
                    "⚠️ Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (dbHelper.DeleteMedicine(selected.Id))
                    {
                        MessageBox.Show("Лекарство успешно удалено!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMedicines();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении лекарства!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Вспомогательные методы для создания элементов
        private Label CreateLabel(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = TextColor,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10)
            };
        }








        private TextBox CreateTextBox(string text, int x, int y, int width, bool multiline = false)
        {
            return new TextBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, multiline ? 80 : 25),
                Multiline = multiline,
                BackColor = Color.White,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
        }







        private NumericUpDown CreateNumericUpDown(int x, int y, int width, decimal min, decimal max, decimal value = 0)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Minimum = min,
                Maximum = max,
                Value = value,
                BackColor = Color.White,
                ForeColor = TextColor
            };
        }
    }
}