using Service.Models;
using Service.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TaskManagement
{
    public partial class ProjectForm : Form
    {
        StatusService _status = new StatusService();
        ProjectService _project = new ProjectService();
        UserService _user = new UserService();

        private int Mode;
        private string name;
        private string uname;
        private DashBoard dashBoard;   //truyền dữ liệu tham chiếu từ form Dashboard vào form Project
        public ProjectForm(int mode, string fullname, DashBoard dashBoard, string username)
        {
            InitializeComponent();

            uname = username;
            Mode = mode;
            name = fullname;
            this.dashBoard = dashBoard;
            btnAdd.BackColor = Color.White;
            rtbProjectName.Text = "";


        }

        private void ProjectForm_Load(object sender, EventArgs e)
        {
            var listStatus = _status.GetAll().Select(x => x.StatusName);
            cboStatus.Items.AddRange(listStatus.ToArray());
            cboStatus.SelectedIndex = 1;

            int userID = _user.GetAll().Where(u => u.Username == uname).Select(id => id.UserId).FirstOrDefault();
            var projects = _project.GetAll().Where(p => p.Uid == userID);

            string projectName = projects.Select(u => u.ProjectName).FirstOrDefault();
            DateTime deadline = projects.Select(u => (DateTime)u.Deadline).FirstOrDefault();
            string status = projects.Select(u => u.Status).FirstOrDefault();
            string descr = projects.Select(u => u.Description).FirstOrDefault();
            if (Mode == 2)
            {
                txtProjectName.Text = projectName.ToString();
                txtDescription.Text = descr.ToString();
                cboStatus.Text = status;
                dtpDeadline.Value = deadline;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;

            Random random = new Random();
            int ranNum = random.Next(100);

            User user = new User();
            var userID = _user.GetAll().Where(x => x.FullName == name).Select(u => u.UserId);
            int id = userID.FirstOrDefault();

            Project project = new Project();
            project.ProjectId = ranNum;
            project.ProjectName = txtProjectName.Text;
            project.Deadline = dtpDeadline.Value;
            project.Description = txtDescription.Text;
            project.Status = cboStatus.Text;
            project.Uid = id;

            bool check = Validation.checkProject(txtProjectName.Text, txtDescription.Text, dtpDeadline.Value);
            if (check && Mode == 1)
            {
                _project.Create(project);
                dashBoard.RefreshData();
                this.Close();
            }
            else if (Mode == 2)
            {
                MessageBox.Show("Đã update", this.Text);
            }
            else
            {
                btnAdd.BackColor = Color.IndianRed;
            }

        }

        private void txtProjectName_Leave(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnAdd.BackColor = Color.White;

            if (txtProjectName.Text.Trim() != "")
            {
                rtbProjectName.Text = (txtProjectName.Text).ToUpperInvariant() + " PROJECT";
            }
            else
            {
                rtbProjectName.Text = "";
            }
        }
    }
}
