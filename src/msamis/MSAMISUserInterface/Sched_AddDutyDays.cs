﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSAMISUserInterface {
    public partial class Sched_AddDutyDays : Form {
        public MainForm reference;
        public MySqlConnection conn;
        public String button = "ADD";
        public int AID { get; set; }
        public String GName;
        public String Client;
        Attendance A;

        #region Form Properties
        public Sched_AddDutyDays() {
            InitializeComponent();
            this.Opacity = 0;
        }
        private void SAddDutyDays_Load(object sender, EventArgs e) {
            LoadPage();
            FadeTMR.Start();
        }
        private void FadeTMR_Tick(object sender, EventArgs e) {
            this.Opacity += 0.2;
            if (reference.Opacity == 0.6 || this.Opacity >= 1) { FadeTMR.Stop(); }
            if (reference.Opacity > 0.7) { reference.Opacity -= 0.1; }
        }
        private void CloseBTN_Click(object sender, EventArgs e) {
            this.Close();
        }
        #endregion


        private void LoadPage() {
            A  = new Attendance(AID);
            NameLBL.Text = Name;
            ClientLBL.Text = Client;
            RefreshData();
        }
        private bool DataValidation() {
            bool ret = true;
            if (CertifiedBX.Text.Equals("")) {
                CertifiedTLTP.ToolTipTitle = "Certification";
                CertifiedTLTP.Show("Who certified this attendanca?", CertifiedBX);
                ret = false;
            }
            return ret;
        }
        private void ConfirmBTN_Click(object sender, EventArgs e) {
            if (DataValidation()) {
                foreach (DataGridViewRow row in DaysGRD.Rows) {
                    A.SetAttendance(int.Parse(row.Cells[0].Value.ToString()), int.Parse(row.Cells[2].Value.ToString()), int.Parse(row.Cells[4].Value.ToString()), row.Cells[5].Value.ToString(), int.Parse(row.Cells[6].Value.ToString()), int.Parse(row.Cells[8].Value.ToString()), row.Cells[9].Value.ToString());
                }
                A.SetCertifiedBy(AID, CertifiedBX.Text);
                this.Close();
            }
        }

        private void RefreshData() {
            if (A.GetAttendance().Rows.Count > 0) { 
                foreach (DataRow row in A.GetAttendance().Rows) {
                    DaysGRD.Rows.Add(row[0] , row[2], row[5].ToString().Split(':')[0], ":", row[5].ToString().Split(':')[1].Split(' ')[0], row[5].ToString().Split(':')[1].Split(' ')[1], row[6].ToString().Split(':')[0], ":", row[6].ToString().Split(':')[1].Split(' ')[0], row[6].ToString().Split(':')[1].Split(' ')[1], "0");
                }
                DaysGRD.CurrentCell = DaysGRD.Rows[0].Cells[1];
            }
        }

        private void DaysGRD_CellEnter(object sender, DataGridViewCellEventArgs e) {
            if (DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[DaysGRD.CurrentCell.ColumnIndex].ReadOnly == true) SendKeys.Send("{Tab}");

            float TimeIn = float.Parse(DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[2].Value.ToString()) + (float.Parse(DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[4].Value.ToString()) / 100);
            float TimeOut = float.Parse(DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[6].Value.ToString()) + (float.Parse(DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[8].Value.ToString()) / 100);
            if (DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[5].Value.ToString().Equals("PM")) TimeIn = TimeIn + 12;

            if (DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[5].Value.ToString().Equals("PM") && DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[8].Value.ToString().Equals("AM")) TimeOut = TimeOut + 24;
            else if (DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[9].Value.ToString().Equals("PM")) TimeOut = TimeOut + 12;
            float ans = TimeOut - TimeIn;
            if (ans - (int)(ans) >= .60) {
                ans = ans + 1;
                ans = (float)(ans - .60);
            }

            if (TimeOut - TimeIn < 0) DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[10].Value = "Invalid";
            else DaysGRD.Rows[DaysGRD.CurrentCell.RowIndex].Cells[10].Value = TimeOut - TimeIn;

        }
    }
}
