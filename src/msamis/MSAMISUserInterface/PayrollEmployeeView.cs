﻿using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MSAMISUserInterface {
    public partial class PayrollEmployeeView : Form {
        private Label _currentLbl;
        private Panel _currentPnl;

        private DataGridViewRow _currentRow;
        private Payroll _pay;
        public Shadow Refer;
        public MainForm Reference;

        public PayrollEmployeeView() {
            InitializeComponent();
            Opacity = 0;
        }

        public int Gid { get; set; }

        private void Payroll_EmployeeView_Load(object sender, EventArgs e) {
            FadeTMR.Start();
            _currentLbl = OverviewLBL;
            _currentPnl = OverviewPNL;
            OverviewPNL.Visible = true;
            AdjPNL.Visible = false;
            OverviewPNL.Visible = true;

            RefreshPayrollList();

            if (Login.AccountType == 2) {
                BonusAddBTN.Visible = false;
                ApproveBTN.Visible = false;
            }
        }

        public void RefreshPayrollList() {
            EmpListGRD.DataSource = Payroll.GetGuardsPayrollMinimal();
            _currentRow = EmpListGRD.Rows[0];
            EmpListGRD.Columns[0].Visible = false;
            EmpListGRD.Columns[1].Width = 320;
            EmpListGRD.Sort(EmpListGRD.Columns[1], ListSortDirection.Ascending);

            foreach (DataGridViewRow row in EmpListGRD.Rows)
                if (row.Cells[0].Value.ToString().Equals(Gid.ToString())) {
                    row.Selected = true;
                    _currentRow = row;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    if (row.Index > 4) EmpListGRD.FirstDisplayedScrollingRowIndex = row.Index - 4;
                    else EmpListGRD.FirstDisplayedScrollingRowIndex = 0;
                    break;
                }
        }


        private void ChangePanel(Label newL, Panel newP) {
            _currentLbl.ForeColor = Color.Gray;
            _currentPnl.Visible = false;
            newL.ForeColor = Color.FromArgb(53, 64, 82);
            newP.Visible = true;
            _currentLbl = newL;
            _currentPnl = newP;
        }

        private void FadeTMR_Tick(object sender, EventArgs e) {
            Opacity += 0.2;
            if (Opacity >= 1) FadeTMR.Stop();
        }

        private void Payroll_EmployeeView_FormClosing(object sender, FormClosingEventArgs e) {
            Refer.Hide();
        }

        private void CloseBTN_Click(object sender, EventArgs e) {
            Close();
        }

        private void BonusesLBL_Click(object sender, EventArgs e) {
            ChangePanel(AdjustmentsLBL, AdjPNL);
        }

        private void AdjLBL_Click(object sender, EventArgs e) {
            ChangePanel(OverviewLBL, OverviewPNL);
        }

        private void BonusesLBL_MouseEnter(object sender, EventArgs e) {
            AdjustmentsLBL.ForeColor = Color.FromArgb(53, 64, 82);
        }

        private void AdjLBL_MouseEnter(object sender, EventArgs e) {
            OverviewLBL.ForeColor = Color.FromArgb(53, 64, 82);
        }

        private void BonusesLBL_MouseLeave(object sender, EventArgs e) {
            if (!AdjPNL.Visible) AdjustmentsLBL.ForeColor = Color.Gray;
        }

        private void AdjLBL_MouseLeave(object sender, EventArgs e) {
            if (!OverviewPNL.Visible) OverviewLBL.ForeColor = Color.Gray;
        }

        private void CloseBTN_MouseEnter(object sender, EventArgs e) {
            CloseBTN.ForeColor = Color.White;
        }

        private void CloseBTN_MouseLeave(object sender, EventArgs e) {
            CloseBTN.ForeColor = Color.FromArgb(53, 64, 82);
        }

        private void BonusAddBTN_Click_1(object sender, EventArgs e) {
            var view = new PayrollAddAdjustments {
                Pid = Gid,
                Location = new Point(Location.X + 350, Location.Y),
                Pay = _pay,
                Period = "for " + PeriodCMBX.Text
            };
            view.ShowDialog();
        }

        private void EmpListGRD_CellEnter(object sender, DataGridViewCellEventArgs e) {
            try {
                if (EmpListGRD.Rows.Count > 0) {
                    _currentRow.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                    EmpListGRD.SelectedRows[0].DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    _currentRow = EmpListGRD.SelectedRows[0];
                    Gid = int.Parse(EmpListGRD.SelectedRows[0].Cells[0].Value.ToString());
                }
            }
            catch { }
            LoadDetails();
            LoadComputations();
            CalcPNL.AutoScrollPosition = new Point(0, 0);
        }

        private void LoadDetails() {
            PeriodCMBX.Items.Clear();
            foreach (DataRow row in Attendance.GetPeriods(Gid).Rows)
                PeriodCMBX.Items.Add(new ComboBoxDays(int.Parse(row["month"].ToString()),
                    int.Parse(row["period"].ToString()), int.Parse(row["year"].ToString())));
            if (PeriodCMBX.Items.Count > 0) {
                PeriodCMBX.SelectedIndex = 0;
                NoPayrollPNL.Visible = false;
            }
            else {
                NoPayrollPNL.BringToFront();
                NoPayrollPNL.Visible = true;
            }
            if (OverviewPNL.Visible == false) ChangePanel(OverviewLBL, OverviewPNL);
        }

        private void LoadComputations() {
            if (PeriodCMBX.Items.Count > 0) {
                _pay = new Payroll(Gid, ((ComboBoxDays) PeriodCMBX.SelectedItem).Month,
                    ((ComboBoxDays) PeriodCMBX.SelectedItem).Period, ((ComboBoxDays) PeriodCMBX.SelectedItem).Year);
                UpdatePopUp("nsu_proper_day_normal", "nsu_overtime_day_normal", "nsu_proper_night_normal",
                    "nsu_overtime_night_normal", MondaySaturday);
                UpdatePopUp("sun_proper_day_normal", "sun_overtime_day_normal", "sun_proper_night_normal",
                    "sun_overtime_night_normal", Sundays);
                UpdatePopUp("nsu_proper_day_special", "nsu_overtime_day_special", "nsu_proper_night_special",
                    "nsu_overtime_night_special", SMond);
                UpdatePopUp("sun_proper_day_special", "sun_overtime_day_special", "sun_proper_night_special",
                    "sun_overtime_night_special", SSunds);
                UpdatePopUp("nsu_proper_day_regular", "nsu_overtime_day_regular", "nsu_proper_night_regular",
                    "nsu_overtime_night_regular", RMond);
                UpdatePopUp("sun_proper_day_regular", "sun_overtime_day_regular", "sun_proper_night_regular",
                    "sun_overtime_night_regular", RSunds);
                UpdateLbl("normal_nsu", OMLBL);
                UpdateLbl("normal_sun", OSLBL);
                UpdateLbl("regular_nsu", RMLBL);
                UpdateLbl("regular_sun", RSLBL);
                UpdateLbl("special_nsu", SMLBL);
                UpdateLbl("special_sun", SSLBL);
                UpdateLbl("normal", OTLBL);
                UpdateLbl("regular", RTLBL);
                UpdateLbl("special", STLBL);
                UpdateLbl("total", WorkTotalLBL);

                B13LBL.Text = CurrencyFormat(_pay.ThirteenthMonthPay);
                BAllowanceLBL.Text = CurrencyFormat(_pay.EmergencyAllowance);
                BBondsLBL.Text = CurrencyFormat(_pay.CashBond);
                BColaLBL.Text = CurrencyFormat(_pay.Cola);
                BTotalLBL.Text = CurrencyFormat(_pay.Bonuses);

                DCashAdvanceLBL.Text = CurrencyFormatNegative(_pay.CashAdvance);
                DPagIbigLBL.Text = CurrencyFormatNegative(_pay.PagIbig);
                DPhilHealthLBL.Text = CurrencyFormatNegative(_pay.PhilHealth);
                DSSSLBL.Text = CurrencyFormatNegative(_pay.Sss);
                DTotalLBL.Text = CurrencyFormatNegative(_pay.Deductions);

                NetPayLBL.Text = CurrencyFormat(_pay.NetPay);

                var wt = _pay.GetWithholdingTax();
                TaxPop.Items[1].Text = CurrencyFormat(wt.TaxbaseD);
                TaxPop.Items[3].Text = CurrencyFormat(wt.ExcessTax);
                TaxPop.Items[5].Text = CurrencyFormat(wt.total);

                DWithLBL.Text = CurrencyFormatNegative(wt.total);
            }
        }

        private void UpdatePopUp(string day, string dayO, string night, string nightO, ToolStrip cms) {
            var e = _pay.hc[day];
            cms.Items[1].Text = e.hour + " hr(s)" + " = " + CurrencyFormat(e.total);

            e = _pay.hc[dayO];
            cms.Items[3].Text = e.hour + " hr(s)" + " = " + CurrencyFormat(e.total);

            e = _pay.hc[night];
            cms.Items[5].Text = e.hour + " hr(s)" + " = " + CurrencyFormat(e.total);

            e = _pay.hc[nightO];
            cms.Items[7].Text = e.hour + " hr(s)" + " = " + CurrencyFormat(e.total);
        }

        private static string CurrencyFormat(double money) {
            return "₱ " + money.ToString("N2");
        }

        private static string CurrencyFormatNegative(double money) {
            return "₱ -" + money.ToString("N2");
        }

        private void UpdateLbl(string key, Control lbl) {
            var e = _pay.TotalSummary[key];
            lbl.Text = CurrencyFormat(e.total);
        }


        private void EmpListGRD_MouseEnter(object sender, EventArgs e) {
            var first = EmpListGRD.FirstDisplayedScrollingRowIndex;
            EmpListGRD.ScrollBars = ScrollBars.Vertical;
            EmpListGRD.FirstDisplayedScrollingRowIndex = first;
        }

        private void EmpListGRD_MouseLeave(object sender, EventArgs e) {
            var first = EmpListGRD.FirstDisplayedScrollingRowIndex;
            EmpListGRD.ScrollBars = ScrollBars.None;
            EmpListGRD.FirstDisplayedScrollingRowIndex = first;
        }

        private void PeriodCMBX_SelectedIndexChanged(object sender, EventArgs e) {
            LoadComputations();
            if (PeriodCMBX.SelectedIndex == 0) {
                BonusAddBTN.Visible = true;
                ApproveBTN.Location = new Point(227, 388);
            } else {
                BonusAddBTN.Visible = false;
                ApproveBTN.Location = new Point(186, 388);
            }
        }

        private void WithHoldingLBL_MouseEnter(object sender, EventArgs e) {
            ShowPopup(TaxPop, WithHoldingLBL);
        }

        private void WithHoldingLBL_MouseLeave(object sender, EventArgs e) {
            HidePop(TaxPop);
        }


        #region Popups  

        private static void ShowPopup(ToolStripDropDown cms, Control cntrl) {
            cms.Show(cntrl, new Point(cntrl.Size.Width + 5, 0));
        }

        private static void HidePop(Control cms) {
            cms.Hide();
        }

        private void MondaysLBL_MouseEnter(object sender, EventArgs e) {
            ShowPopup(MondaySaturday, OMondaysLBL);
        }

        private void MondaysLBL_MouseLeave(object sender, EventArgs e) {
            HidePop(MondaySaturday);
        }


        private void OSundays_MouseEnter(object sender, EventArgs e) {
            ShowPopup(Sundays, OSundays);
        }

        private void OSundays_MouseLeave(object sender, EventArgs e) {
            HidePop(Sundays);
        }

        private void RMondays_MouseEnter(object sender, EventArgs e) {
            ShowPopup(RMond, RMondays);
        }

        private void RMondays_MouseLeave(object sender, EventArgs e) {
            HidePop(RMond);
        }

        private void RSundays_MouseEnter(object sender, EventArgs e) {
            ShowPopup(RSunds, RSundays);
        }

        private void RSundays_MouseLeave(object sender, EventArgs e) {
            HidePop(RSunds);
        }

        private void SMondays_MouseEnter(object sender, EventArgs e) {
            ShowPopup(SMond, SMondays);
        }

        private void SMondays_MouseLeave(object sender, EventArgs e) {
            HidePop(SMond);
        }

        private void SSundays_MouseEnter(object sender, EventArgs e) {
            ShowPopup(SSunds, SSundays);
        }

        private void SSundays_MouseLeave(object sender, EventArgs e) {
            HidePop(SSunds);
        }

        #endregion

        private void SSSLBL_MouseEnter(object sender, EventArgs e) {
            ShowPopup(SSSPop, SSSLBL);
        }

        private void SSSLBL_MouseLeave(object sender, EventArgs e) {
            HidePop(SSSPop);
        }
    }
}