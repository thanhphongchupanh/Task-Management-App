using Service.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskManagement.notification_decor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TaskManagement
{
    public partial class DashBoard : Form
    {

        ProjectService _project = new ProjectService();
        UserService _user = new UserService();

        private string name;
        private string mail;
        private string Username;
        public DashBoard(string fullname, string email, string username)
        {

            InitializeComponent();

            name = fullname;
            mail = email;
            Username = username;

            int getUID = _user.GetAll().Where(x => x.Username == Username).Select(u => u.UserId).FirstOrDefault();
            var listProject = _project.GetAll().Where(u => u.Uid == getUID);
            var selectedProject = listProject.Select(p => new { p.ProjectName, p.Status, p.Deadline, p.Description }).ToList();

            dgvDashBoard.DataSource = new BindingSource
            {
                DataSource = selectedProject
            };
        }

        private void DashBoard_Load(object sender, EventArgs e)
        {
            labelFullname.Text = name;
            labelEmail.Text = mail;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            string search = txtSearch.Text.ToLower();
            var listProject = _project.GetAll().Where(x => x.ProjectName.ToLower().Contains(search));
            var selectedProject = listProject.Select(p => new { p.ProjectName, p.Status }).ToList();

            dgvDashBoard.DataSource = new BindingSource
            {
                DataSource = selectedProject
            };
            btnSearch.Enabled = true;
        }

        private void btnAddProject_Click(object sender, EventArgs e)
        {
            int mode = 1;
            Form frProject = new ProjectForm(mode, name, this, Username);
            frProject.Show();
        }

        public void RefreshData()
        {
            var listProject = _project.GetAll();
            var selectedProject = listProject.Select(p => new { p.ProjectName, p.Status, p.Deadline, p.Description });
            dgvDashBoard.DataSource = new BindingSource { DataSource = selectedProject };
        }

        private void dgvDashBoard_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvDashBoard.Rows[e.RowIndex].Selected = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this project ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                // chọn dòng cần xóa
                var project = _project.GetAll().ToList()[dgvDashBoard.CurrentRow.Index];
                int preRowDeleted = dgvDashBoard.CurrentRow.Index - 1;

                // xóa dòng dc chọn
                _project.Delete(project);

                // reload datagridview
                this.RefreshData();


                // row selected
                int numOfListRow = dgvDashBoard.Rows.Count;
                if (preRowDeleted >= 0 && preRowDeleted < numOfListRow)
                {
                    dgvDashBoard.Rows[preRowDeleted].Selected = true;
                    dgvDashBoard.CurrentCell = dgvDashBoard.Rows[preRowDeleted].Cells[0];  //chọn ô đầu tiên của dòng mún chọn
                    dgvDashBoard.Focus(); // Đặt trỏ chuột vào DataGridView
                }
            }

        }

        private void Inprogress_Click(object sender, EventArgs e)
        {
            string priority = "Inprogress";
            labelPriority.Text = priority;

            var listInprogress = _project.GetAll().Where(x => x.Status == "Inprogress");
            var selected = listInprogress.Select(p => new { p.ProjectName, p.Status, p.Deadline, p.Description });
            dgvDashBoard.DataSource = new BindingSource
            {
                DataSource = selected
            };
        }

        private void Planned_Click(object sender, EventArgs e)
        {
            string priority = "Planned";
            labelPriority.Text = priority;

            var listPlanned = _project.GetAll().Where(x => x.Status == "Not Yet");
            var selected = listPlanned.Select(p => new { p.ProjectName, p.Status, p.Deadline, p.Description });
            dgvDashBoard.DataSource = new BindingSource
            {
                DataSource = selected
            };
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.RefreshData();
            labelPriority.Text = "List of Project";
            txtSearch.Text = "";
        }

        private void UserDetail_Click(object sender, EventArgs e)
        {
            UserDetails userDetails = new UserDetails(Username);
            this.Hide();
            userDetails.ShowDialog();
        }


        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login lgLam = new Login();
            this.Opacity -= 0.8;
            lgLam.ShowDialog();
            if (Home.ActiveForm != null)
            {
                this.Opacity += 0.8;
            }
        }

        private void sIgnInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Signup signup = new Signup();
            this.Opacity -= 0.8;
            signup.ShowDialog();
            if (Home.ActiveForm != null)
            {
                this.Opacity += 0.8;
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Logout confirm", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoPage infoPage = new InfoPage();
            this.Opacity -= 0.8;
            //this.Hide();
            infoPage.ShowDialog();
            if (Home.ActiveForm != null)
            {
                this.Opacity += 0.8;
            }
            else this.Close();
        }

        private void dgvDashBoard_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvDashBoard.Rows[e.RowIndex].Selected = true;
            }
            var project = _project.GetAll().ToList()[dgvDashBoard.CurrentRow.Index];
            int preRowDeleted = dgvDashBoard.CurrentRow.Index - 1;

            int mode = 2;
            ProjectForm projectForm = new ProjectForm(mode, name, this, Username);
            projectForm.Show();

        }
    }
}
