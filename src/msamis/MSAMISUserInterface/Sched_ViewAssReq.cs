﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSAMISUserInterface {
    public partial class Sched_ViewAssReq : Form {
        public int RAID { get; set; }
        public MainForm reference;
        public Shadow refer;
        int numGuards;

        public Sched_ViewAssReq() {
            InitializeComponent();
            this.Opacity = 0;
        }

        private void FadeTMR_Tick(object sender, EventArgs e) {
            this.Opacity += 0.2;
            if (this.Opacity >= 1) { FadeTMR.Stop(); }
        }

        private void Sched_ViewAssReq_Load(object sender, EventArgs e) {
            RefreshData();
            this.Location = new Point(this.Location.X + 175, this.Location.Y);
            FadeTMR.Start();
        }
        private void RefreshData() {
            DataTable dt = Scheduling.GetAssignmentRequestDetails(RAID);
            ClientLBL.Text = dt.Rows[0]["name"].ToString();
            PermAddLBL.Text = "Location: " + dt.Rows[0]["location"].ToString();
            ContractStartLBL.Text = "Contract Start: " + dt.Rows[0]["contractstart"].ToString();
            ContractEndLBL.Text = "Contract End: " + dt.Rows[0]["contractend"].ToString();
            numGuards = int.Parse(dt.Rows[0]["noguards"].ToString());
            NoLBL.Text = "Guards Needed: " + numGuards.ToString();
            if (dt.Rows[0]["rstatus"].ToString().Equals(Enumeration.RequestStatus.Pending.ToString())) {
                AssignBTN.Text = "APPROVE";
                StatusLBL.Text = "Status: Pending";
            } else if (dt.Rows[0]["rstatus"].ToString().Equals(Enumeration.RequestStatus.Approved.ToString())) {
                AssignBTN.Text = "ASSIGN";
                StatusLBL.Text = "Status: Approved";
                AssignBTN.Location = new Point(220, 411);
                DeclineBTN.Visible = false;

            } else {
                AssignBTN.Visible = false;
                AvailablePNL.Visible = false;
                DeclineBTN.Visible = false;
                if (dt.Rows[0]["rstatus"].ToString().Equals(Enumeration.RequestStatus.Active.ToString())) StatusLBL.Text = "Status: Active";
                else if (dt.Rows[0]["rstatus"].ToString().Equals(Enumeration.RequestStatus.Inactive.ToString())) StatusLBL.Text = "Status: Inctive";
                else if (dt.Rows[0]["rstatus"].ToString().Equals(Enumeration.RequestStatus.Declined.ToString())) StatusLBL.Text = "Status: Decline";
            }
            if (numGuards > Scheduling.GetNumberOfUnassignedGuards()) NeededLBL.ForeColor = Color.Salmon;
            else NeededLBL.ForeColor = Color.OliveDrab;
            NeededLBL.Text = Scheduling.GetNumberOfUnassignedGuards().ToString() + " available guards";
        }

        private void Sched_ViewAssReq_FormClosing(object sender, FormClosingEventArgs e) {
            refer.Hide();
            reference.SCHEDLoadPage();
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void AssignBTN_Click(object sender, EventArgs e) {
            if (AssignBTN.Text.Equals("ASSIGN")) {
                try {
                    Sched_AssignGuards view = new Sched_AssignGuards();
                    view.RID = this.RAID;
                    view.NumberOfGuards = numGuards;
                    view.refer = this;
                    view.ClientName = ClientLBL.Text;
                    view.Location = this.Location;
                    view.ShowDialog();
                }
                catch (Exception) { }
            } else {
                Scheduling.UpdateRequestStatus(RAID, Enumeration.RequestStatus.Approved);
                AssignBTN.Text = "ASSIGN";
                AssignBTN.Location = new Point(220, 411);
                StatusLBL.Text = "Status: Approved";
                DeclineBTN.Visible = false;
            }
        }

        private void DeclineBTN_Click(object sender, EventArgs e) {
            Scheduling.DeclineRequest(RAID);
            AssignBTN.Visible = false;
            DeclineBTN.Visible = false;
            AvailablePNL.Visible = false;
            StatusLBL.Text = "Status: Declined";
        }
    }
}
