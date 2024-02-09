using lab1.DB;
using lab1.services;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        private DataGridViewRow selectedRow;
        private bool isAdding;

        private const int GENDER_COLUMN_INDEX = 5;
        private const int IS_RETIRED_COLUMN_INDEX = 18;
        private const int CONSCRIPT_COLUMN_INDEX = 20;
        private const int RELATIONAL_START_INDEX = 21;
        private const int CITY_OF_RESIDENCE_COL = 21;
        private const int FAMILY_STATUS_COL = 22;
        private const int CITIZENSHIP_COL = 23;
        private const int DISABILITY_COL = 24;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            dgvClients.Rows.Clear();
            dgvClients.Columns.Clear();
            DataTable dt = Queries.ExecuteQuery(Queries.GetAllClientsQuery);
            //dgvClients.DataSource = dt;
            /*foreach (DataRow row in dt.Rows)
            {
                dgvClients.Rows.Add(row.ItemArray);
            }
            dgvClients.Columns[0].ReadOnly = true;*/
            //DataTable dt = Queries.ExecuteQuery(Queries.GetAllClientsQuery);

            foreach (DataColumn column in dt.Columns)
            {
                dgvClients.Columns.Add(column.ColumnName, column.ColumnName);
            }

            foreach (DataRow row in dt.Rows)
            {
                dgvClients.Rows.Add(row.ItemArray);
            }

            dgvClients.Columns[0].ReadOnly = true;
            SetDataGridViewFont(dgvClients, 10);
        }

        private void dgvClients_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            SetDataGridViewFont(dgvClients, 10);
        }
        //private void SetDataGridViewRowFormat(DataGridViewRow)
        private void SetDataGridViewFont(DataGridView dataGridView, float fontSize)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    DataGridViewCell cell = row.Cells[i];

                    if (cell.Value is DateTime dateValue)
                    {
                        cell.Value = dateValue.ToShortDateString();
                    }

                    //string header = row.HeaderCell.Value.ToString();
                    if (i == IS_RETIRED_COLUMN_INDEX || i == CONSCRIPT_COLUMN_INDEX)
                    {
                        cell.Value = cell.Value.ToString() == "1" ? "yes" : "no";
                    }

                    if (i == GENDER_COLUMN_INDEX)
                    {
                        cell.Value = cell.Value.ToString() == "1" ? "m" : "f";
                    }

                    cell.Style.Font = new Font(dataGridView.Font.FontFamily, fontSize);
                }

            }
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.Style.Font = new Font(dataGridView.Font.FontFamily, fontSize, FontStyle.Bold);
            }
        }

        private void ClearCellColors(DataGridViewRow row)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.BackColor = dgvClients.DefaultCellStyle.BackColor;
            }
        }


        private void dgvClients_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            DataGridViewCell cell = dgvClients.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!isAdding)
            {
                object newValue = dgvClients.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                int clientId = Convert.ToInt32(dgvClients.Rows[e.RowIndex].Cells[0].Value);
                string field = dgvClients.Columns[e.ColumnIndex].HeaderCell.Value.ToString();
                ClearCellColors(dgvClients.Rows[e.RowIndex]);
                Validator.ErrorCode errorCode = Validator.ValidateRow(dgvClients.Rows[e.RowIndex]);
                if (errorCode != Validator.ErrorCode.OK)
                {
                    //cell.Style.BackColor = Color.Red;
                    if ((int)errorCode < dgvClients.Rows[e.RowIndex].Cells.Count)
                    {
                        dgvClients.Rows[e.RowIndex].Cells[(int)errorCode].Style.BackColor = Color.Red;
                    }
                    if (errorCode == Validator.ErrorCode.ALREADY_EXISTS_PASSPORT_SERIES_AND_NUMBER)
                    {
                        dgvClients.Rows[e.RowIndex].Cells[(int)Validator.ErrorCode.WRONG_PASSPORT_SERIES].Style.BackColor = Color.Red;
                        dgvClients.Rows[e.RowIndex].Cells[(int)Validator.ErrorCode.WRONG_PASSPORT_NUMBER].Style.BackColor = Color.Red;
                    }
                    return;
                }
                int i = e.ColumnIndex;
                if (i == IS_RETIRED_COLUMN_INDEX || i == CONSCRIPT_COLUMN_INDEX)
                {
                    newValue = newValue.ToString() == "yes" ? 1 : 0;
                }

                if (i == GENDER_COLUMN_INDEX)
                {
                    newValue = newValue.ToString() == "m" ? "1" : "0";
                }

                if (e.ColumnIndex >= RELATIONAL_START_INDEX)
                {
                    if (newValue == null)
                    {
                        return;
                    }
                    string columnName;
                    switch (e.ColumnIndex) {
                        case CITY_OF_RESIDENCE_COL:
                            columnName = "name";
                            break;
                        case FAMILY_STATUS_COL:
                            columnName = "status";
                            break;
                        case CITIZENSHIP_COL:
                            columnName = "citizenship";
                            break;
                        case DISABILITY_COL:
                            columnName = "type";
                            break;
                        default:
                            columnName = "Unknown";
                            break;

                    }
                    newValue = Queries.GetIdByValue(field, columnName, newValue.ToString());
                    field = field + "_id";
                }
                Queries.UpdateClient(clientId, field, newValue);
            }
            else
            {
                DataGridViewRow row = dgvClients.Rows[dgvClients.Rows.Count - 1];
                //Check everything before inserting
                ClearCellColors(dgvClients.Rows[e.RowIndex]);
                Validator.ErrorCode errorCode = Validator.ValidateRow(dgvClients.Rows[e.RowIndex]);
                if (errorCode != Validator.ErrorCode.OK)
                {
                    if ((int)errorCode < dgvClients.Rows[e.RowIndex].Cells.Count)
                    {
                        dgvClients.Rows[e.RowIndex].Cells[(int)errorCode].Style.BackColor = Color.Red;
                    }
                    if (errorCode == Validator.ErrorCode.ALREADY_EXISTS_PASSPORT_SERIES_AND_NUMBER)
                    {
                        dgvClients.Rows[e.RowIndex].Cells[(int)Validator.ErrorCode.WRONG_PASSPORT_SERIES].Style.BackColor = Color.Red;
                        dgvClients.Rows[e.RowIndex].Cells[(int)Validator.ErrorCode.WRONG_PASSPORT_NUMBER].Style.BackColor = Color.Red;
                    }
                    return;
                }
                string name = row.Cells["name"].Value.ToString();
                string surname = row.Cells["surname"].Value.ToString();
                string lastname = row.Cells["lastname"].Value.ToString();
                DateTime dateOfBirth = Convert.ToDateTime(row.Cells["dateOfBirth"].Value);
                int gender = row.Cells["gender"].Value.ToString() == "m" ? 1 : 0;
                string passportSeries = row.Cells["passportSeries"].Value.ToString();
                string passportNumber = row.Cells["passportNumber"].Value.ToString();
                string passportIssuedBy = row.Cells["passportIssuedBy"].Value.ToString();
                DateTime passportDateOfIssue = Convert.ToDateTime(row.Cells["passportDateOfIssue"].Value);
                string passportId = row.Cells["passportId"].Value.ToString();
                string birthPlace = row.Cells["birthPlace"].Value.ToString();
                string address = row.Cells["address"].Value.ToString();
                string phoneNumber = row.Cells["phoneNumber"].Value == null ? "" : row.Cells["phoneNumber"].Value.ToString();
                string stationaryPhoneNumber = row.Cells["stationaryPhoneNumber"].Value == null ? "" : row.Cells["stationaryPhoneNumber"].Value.ToString(); 
                string email = row.Cells["email"].Value == null ? "" : row.Cells["email"].Value.ToString(); 
                string placeOfWork = row.Cells["placeOfWork"].Value == null ? "" : row.Cells["placeOfWork"].Value.ToString(); 
                string jobTitle = row.Cells["jobTitle"].Value == null ? "" : row.Cells["jobTitle"].Value.ToString(); 
                int isRetired = row.Cells["isRetired"].Value.ToString() == "yes" ? 1 : 0;
                double? monthlyIncome;
                if (row.Cells["monthlyIncome"].Value != null)
                {
                    monthlyIncome = Convert.ToDouble(row.Cells["monthlyIncome"].Value);
                }
                else
                {
                    monthlyIncome = null;
                }
                int conscript = row.Cells["conscript"].Value.ToString() == "yes" ? 1 : 0;
                string cityOfResidence = row.Cells["CityOfResidence"].Value.ToString();
                string familyStatus = row.Cells["FamilyStatus"].Value.ToString();
                string citizenship = row.Cells["citizenship"].Value.ToString();
                string disability = row.Cells["Disability"].Value.ToString();

                int cityOfResidenceId = Queries.GetIdByValue("cityOfResidence", "name", cityOfResidence);
                int familyStatusId = Queries.GetIdByValue("familyStatus", "status", familyStatus);
                int citizenshipId = Queries.GetIdByValue("citizenship", "citizenship", citizenship);
                int disabilityId = Queries.GetIdByValue("disability", "type", disability);

                int id;
                try
                {
                    id = Queries.InsertClient(name, surname, lastname, dateOfBirth, gender, passportSeries, passportNumber,
                         passportIssuedBy, passportDateOfIssue, passportId, birthPlace, address, phoneNumber,
                         stationaryPhoneNumber, email, placeOfWork, jobTitle, isRetired, monthlyIncome, conscript,
                         cityOfResidenceId, familyStatusId, citizenshipId, disabilityId);
                }
                catch
                {
                    MessageBox.Show("Could not add the client");
                    return;
                }
                isAdding = false;
                foreach (DataGridViewRow r in dgvClients.Rows)
                {
                    r.ReadOnly = true;
                }
                //dgvClients.Rows[dgvClients.Rows.Count - 1].Cells["id"].Value = id;
                //SetDataGridViewFont(dgvClients, 10);
                Form1_Load(sender, e);
            }
        }

        private void dgvClients_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dgvClients.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    dgvClients.ClearSelection();
                    dgvClients.Rows[currentMouseOverRow].Selected = true;
                    selectedRow = dgvClients.Rows[currentMouseOverRow];
                    cmsClient.Show(dgvClients, new Point(e.X, e.Y));
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRow == null)
            {
                return;
            }
            int rowId = Convert.ToInt32(selectedRow.Cells[0].Value);
            Queries.DeleteClientById(rowId);
            dgvClients.Rows.Remove(selectedRow);
        }

        private void dgvClients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                dgvClients.ClearSelection();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isAdding)
            {
                return;
            }
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                row.ReadOnly = true;
            }

            dgvClients.Rows.Add();
            dgvClients.ClearSelection();
            dgvClients.Rows[dgvClients.Rows.Count - 1].Selected = true;
            isAdding = true;
        }
    }
}
