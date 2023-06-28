using Service.Models;
using Service.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TaskManagement.notification_decor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TaskManagement
{
        public partial class UserDetails : Form
        {
            UserService _userService = new UserService();
            User _user = new User();
            private string Username;
            AccountService _account = new AccountService();

            public UserDetails(string username)
            {
                InitializeComponent();
                Username = username;
            }

            private void UserDetails_Load(object sender, EventArgs e)
            {
                var getAllInfo = _userService.GetAll().Where(u => u.Username == Username);

                string username = getAllInfo.Select(u => u.Username).FirstOrDefault();
                string fullname = getAllInfo.Select(f => f.FullName).FirstOrDefault();
                string email = getAllInfo.Select(e => e.Email).FirstOrDefault();
                int phone = getAllInfo.Select(p => (int)p.PhoneNumber).FirstOrDefault();
                DateTime birthday = getAllInfo.Select(b => (DateTime)b.Birthday).FirstOrDefault();

                txtEmail.Text = email.ToString();
                txtFullname.Text = fullname.ToString();
                dtBirthday.Text = birthday.ToString("yyyy-MM-dd");
                txtPhone.Text = phone.ToString();
            }

            private void btnBack_Click(object sender, EventArgs e)
            {
                btnBack.Visible = false;
                DialogResult result = MessageBox.Show("Do you want to save?", "Notification", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    var selectedFullnames = _userService.GetAll().Where(u => u.Username == Username).Select(u => u.FullName);
                    string fullname = selectedFullnames.FirstOrDefault();

                    var selectedEmail = _userService.GetAll().Where(u => u.Username == Username).Select(u => u.Email);
                    string email = selectedEmail.FirstOrDefault();

                //Update
                updateUserDetail();
                DashBoard dashBoard = new DashBoard(fullname, email, Username);
                    
                    this.Close();
                    dashBoard.ShowDialog();
                }
                else if (result == DialogResult.No)
                {
                DashBoard dashBoard = new DashBoard(txtFullname.Text, txtEmail.Text, Username);
                dashBoard.Show();
                this.Close();
                }
                else if (result == DialogResult.Cancel)
                {
                    //không thực hiện bất kỳ thao tác nào
                }
            this.DialogResult = DialogResult.OK;
            this.Close();
            btnBack.Enabled = true;
                }

            private void button3_Click(object sender, EventArgs e)
            {
                Change_Password change_Password = new Change_Password(Username);
                change_Password.ShowDialog();
            }

            private void btnUpdateUD_Click(object sender, EventArgs e)
            {
            updateUserDetail();
            }

        private void updateUserDetail()
        {
            btnUpdateUD.Enabled = false;
            string fullName = txtFullname.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            DateTime birthday = dtBirthday.Value;

            var newUser = _userService.GetAll().FirstOrDefault(u => u.Username == Username);
            bool checkInfo = ValidationLam.checkInfo(email, phone, fullName, birthday);
            if (checkInfo)
            {
                newUser.Email = txtEmail.Text;
                newUser.PhoneNumber = Convert.ToInt32(txtPhone.Text);
                newUser.FullName = txtFullname.Text;
                newUser.Birthday = dtBirthday.Value;
                _userService.Update(newUser);
                new Success("Update successful").Show();
                DashBoard db = new DashBoard(newUser.FullName, newUser.Email, newUser.Username);
                this.Hide();
                db.ShowDialog();
                this.Close();
            }
            else
            {
                btnUpdateUD.BackColor = Color.IndianRed;
            }
            btnUpdateUD.Enabled = true;
        }
    }
}

