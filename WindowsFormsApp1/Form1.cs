using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EasyDox;
using LiteDB;
using System.Configuration;
using WindowsFormsApp1.Classes;
using WindowsFormsApp1.WinForms;
using System.Configuration;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            int maxWidth = 0, temp = 0;
            foreach (var item in comboBoxSex.Items)
            {
                temp = TextRenderer.MeasureText(item.ToString(), comboBoxSex.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            comboBoxSex.DropDownWidth = maxWidth;
            LoadMD();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForEerror();
            if (false == GetErrorProvider())
            {
                string medicalDoctors = String.Empty;
                int i = 0;
                for (; i < checkedListBoxMD.CheckedItems.Count - 1; i++)
                {
                    medicalDoctors += (checkedListBoxMD.CheckedItems[i] as MDoctor)?.FullName + ", ";
                }

                if(checkedListBoxMD.CheckedIndices.Count > 0) medicalDoctors += (checkedListBoxMD.CheckedItems[i] as MDoctor)?.FullName;
                var fieldValues = new Dictionary<string, string>
                {
                    {"D1", maskedTextBoxDateCheck.Text[0].ToString()},
                    {"D2", maskedTextBoxDateCheck.Text[1].ToString()},
                    {"M1", maskedTextBoxDateCheck.Text[3].ToString()},
                    {"M2", maskedTextBoxDateCheck.Text[4].ToString()},
                    {"Y1", maskedTextBoxDateCheck.Text[6].ToString()},
                    {"Y2", maskedTextBoxDateCheck.Text[7].ToString()},
                    {"Y3", maskedTextBoxDateCheck.Text[8].ToString()},
                    {"Y4", maskedTextBoxDateCheck.Text[9].ToString()},
                    {"H1", maskedTextBoxTimeCheck.Text[0].ToString()},
                    {"H2", maskedTextBoxTimeCheck.Text[1].ToString()},
                    {"m1", maskedTextBoxTimeCheck.Text[3].ToString()},
                    {"m2", maskedTextBoxTimeCheck.Text[4].ToString()},
                    {"FIO", textBoxFIO.Text},
                    {"D3", maskedTextBoxBoD.Text[0].ToString()},
                    {"D4", maskedTextBoxBoD.Text[1].ToString()},
                    {"M3", maskedTextBoxBoD.Text[3].ToString()},
                    {"M4", maskedTextBoxBoD.Text[4].ToString()},
                    {"Y5", maskedTextBoxBoD.Text[6].ToString()},
                    {"Y6", maskedTextBoxBoD.Text[7].ToString()},
                    {"Y7", maskedTextBoxBoD.Text[8].ToString()},
                    {"Y8", maskedTextBoxBoD.Text[9].ToString()},
                    {"S", comboBoxSex.SelectedItem.ToString()},
                    {"STREET", textBoxAddress.Text},
                    {
                        "WORK1",
                        textBoxResultMedCheck.Text
                    },
                    {"R1", comboBoxResultMedCheck.SelectedItem.ToString()},
                    {
                        "WORK2",
                        textBoxMedExam.Text
                    },
                    {"R2", comboBoxMedExam.SelectedItem.ToString()},
                    {"FIODOCTOR", textBoxMDFIO.Text.Length > 0 ? textBoxMDFIO.Text + ", " + medicalDoctors : medicalDoctors}
                };
                string dir = Application.StartupPath;
                string path = Path.Combine(dir + ConfigurationManager.AppSettings["templateDocx"]);
                string pathOut = Path.Combine(dir + ConfigurationManager.AppSettings["outputDoc"]);
                var engine = new Engine();
                engine.Merge(path, fieldValues, pathOut);
                System.Diagnostics.Process.Start(pathOut);
            }
        }

        private void LoadMD()
        {
            using (var db = new LiteDatabase(ConfigurationManager.ConnectionStrings["LiteDB"].ConnectionString))
            {
                //if (db.CollectionExists("MDoctors")) db.DropCollection("MDoctors");
                var doctors = db.GetCollection<Classes.MDoctor>("MDoctors");
                var result = doctors.Query().OrderBy(x => x.LastName).ToList();
                checkedListBoxMD.DataSource = result;
                checkedListBoxMD.DisplayMember = "FullName";
            }
        }
        private void buttonMD_Click(object sender, EventArgs e)
        {
            MedicalDoctor medicalDoctor = new MedicalDoctor();
            medicalDoctor.ShowDialog();
            LoadMD();
        }

        private bool GetErrorProvider()
        {
            foreach (Control control in groupBoxForm.Controls)
            {
                if (errorProvider1.GetError(control) != String.Empty) return true;
            }
            return false;
        }

        private void CheckForEerror()
        {
            Regex regex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])[.|\s|-|/](0[1-9]|1[012])[.|\s|-|для проверки коректности дат](19|20)\d\d$");
            if (false == regex.Match(this.maskedTextBoxDateCheck.Text).Success)
            {
                errorProvider1.SetError(this.maskedTextBoxDateCheck, "Неверная дата");
            }
            else
            {
                errorProvider1.SetError(this.maskedTextBoxDateCheck, string.Empty);
            }

            if (false == regex.Match(this.maskedTextBoxBoD.Text).Success)
            {
                errorProvider1.SetError(this.maskedTextBoxBoD, "Неверная дата");
            }
            else
            {
                errorProvider1.SetError(this.maskedTextBoxBoD, string.Empty);
            }
            regex = new Regex(@"^([0-5]?\d):([0-5]?\d)$");
            if (false == regex.Match(this.maskedTextBoxTimeCheck.Text).Success)
            {
                errorProvider1.SetError(this.maskedTextBoxTimeCheck, "Неверено указано время");
            }
            else
            {
                errorProvider1.SetError(this.maskedTextBoxTimeCheck, string.Empty);
            }

            if (textBoxMDFIO.Text.Length <=0 && checkedListBoxMD.CheckedItems.Count <= 0)
            {
                errorProvider1.SetError(this.checkedListBoxMD, "Укажите/выберите доктора");
            }
            else
            {
                errorProvider1.SetError(this.checkedListBoxMD, String.Empty);
            }

            if (comboBoxSex.SelectedIndex == -1)
            {
                errorProvider1.SetError(comboBoxSex, "Не выбран пол");
            }
            else
            {
                errorProvider1.SetError(comboBoxSex, String.Empty);
            }

            if (comboBoxMedExam.SelectedIndex == -1)
            {
                errorProvider1.SetError(comboBoxMedExam, "Выберите значение");
            }
            else
            {
                errorProvider1.SetError(comboBoxMedExam, String.Empty);
            }
            if (comboBoxResultMedCheck.SelectedIndex == -1)
            {
                errorProvider1.SetError(comboBoxResultMedCheck, "Выберите значение");
            }
            else
            {
                errorProvider1.SetError(comboBoxResultMedCheck, String.Empty);
            }
        }

        private void groupBoxForm_Enter(object sender, EventArgs e)
        {

        }

        private void textBoxMDFIO_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBoxMD_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}