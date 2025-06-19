using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MyWinFormsApp
{
    public class Form1 : Form
    {
        // Text fields for employee data input
        private TextBox txtID, txtName, txtPosition, txtAge, txtSalary;

        // Table to display the employee list
        private DataGridView dataGrid;

        // Button to add new employee
        private Button btnAdd;

        // File path where employee data will be saved
        private string filePath = "employees.csv";

        public Form1()
        {
            // Set up the form title and size
            this.Text = "Employee Registry";
            this.Width = 600;
            this.Height = 500;

            int top = 10;

            // Create all the input fields and labels
            txtID = AddTextBox("ID:", ref top);
            txtName = AddTextBox("Name:", ref top);
            txtPosition = AddTextBox("Position:", ref top);
            txtAge = AddTextBox("Age:", ref top);
            txtSalary = AddTextBox("Salary:", ref top);

            // Create the Add button and assign its click event
            btnAdd = new Button { Left = 12, Top = top + 10, Text = "Add Employee", Width = 550 };
            btnAdd.Click += BtnAdd_Click;

            // Set up the DataGridView to show the employee data
            dataGrid = new DataGridView
            {
                Left = 12,
                Top = btnAdd.Bottom + 10,
                Width = 550,
                Height = 250,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Add a "Delete" button column
            var btnColumn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 60
            };

            // Add columns to the DataGridView
            dataGrid.Columns.Add(btnColumn);
            dataGrid.Columns.Add("ID", "ID");
            dataGrid.Columns.Add("Name", "Name");
            dataGrid.Columns.Add("Position", "Position");
            dataGrid.Columns.Add("Age", "Age");
            dataGrid.Columns.Add("Salary", "Salary");

            // Event to handle clicks on delete button
            dataGrid.CellClick += DataGrid_CellClick;

            // Add controls to the form
            this.Controls.Add(btnAdd);
            this.Controls.Add(dataGrid);

            // Set up form load and closing events
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        // Helper function to create label and textbox together
        private TextBox AddTextBox(string label, ref int top)
        {
            var lbl = new Label { Text = label, Left = 12, Top = top, Width = 80 };
            var txt = new TextBox { Left = 100, Top = top, Width = 160 };
            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            top += 30;
            return txt;
        }

        // Load data from the CSV file into the table
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath).Skip(1); // Skip header
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 5)
                    {
                        dataGrid.Rows.Add("Delete", parts[0], parts[1], parts[2], parts[3], parts[4]);
                    }
                }
            }
        }

        // Save data from the table into the CSV file
        private void SaveToFile()
        {
            var lines = new List<string>
            {
                "ID,Name,Position,Age,Salary" // Header
            };

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (!row.IsNewRow)
                {
                    var values = row.Cells.Cast<DataGridViewCell>().Skip(1).Select(c => c.Value?.ToString() ?? "");
                    lines.Add(string.Join(",", values));
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        // Save data when the form is closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveToFile();
        }

        // When "Add Employee" button is clicked
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Check if any field is empty
            if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtName.Text)
                || string.IsNullOrWhiteSpace(txtPosition.Text) || string.IsNullOrWhiteSpace(txtAge.Text)
                || string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            // Add new employee data to the table
            dataGrid.Rows.Add("Delete", txtID.Text, txtName.Text, txtPosition.Text, txtAge.Text, txtSalary.Text);

            // Save to file
            SaveToFile();

            // Clear the text fields
            txtID.Text = txtName.Text = txtPosition.Text = txtAge.Text = txtSalary.Text = "";
        }

        // Handle click on "Delete" button in a row
        private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // First column is the delete button
            {
                var result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dataGrid.Rows.RemoveAt(e.RowIndex);
                    SaveToFile();
                }
            }
        }
    }
}