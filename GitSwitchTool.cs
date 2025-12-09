using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.IO;

// ==========================================
// C# Git 身份切换工具 (UI 美化修复版)
// ==========================================

namespace GitSwitcher
{
    public class MainForm : Form
    {
        private List<UserProfile> userList = new List<UserProfile>();
        private readonly string configPath = "git_users.txt";

        private ComboBox cmbUsers;
        private Label lblCurrent;
        private ListView lvStats;
        private Button btnSwitch;
        private Button btnAdd;
        private Button btnDel;

        public MainForm()
        {
            this.Text = "Git 身份管理助手";
            this.Size = new Size(600, 580); // 稍微加大主窗口
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 固定边框
            this.MaximizeBox = false;
            this.BackColor = Color.White; // 纯白背景

            LoadUsers();
            InitializeUI();
            RefreshData();
        }

        private void InitializeUI()
        {
            Font mainFont = new Font("Microsoft YaHei", 10);
            Font titleFont = new Font("Microsoft YaHei", 10, FontStyle.Bold);

            // --- 区域1: 身份切换与管理 ---
            GroupBox grpSwitch = new GroupBox();
            grpSwitch.Text = " 身份控制台 ";
            grpSwitch.Location = new Point(20, 15);
            grpSwitch.Size = new Size(540, 160);
            grpSwitch.Font = mainFont;
            grpSwitch.FlatStyle = FlatStyle.System; // 原生风格
            this.Controls.Add(grpSwitch);

            // 当前状态标签
            lblCurrent = new Label();
            lblCurrent.Location = new Point(20, 35);
            lblCurrent.Size = new Size(500, 25);
            lblCurrent.Text = "读取中...";
            lblCurrent.ForeColor = Color.FromArgb(0, 102, 204); // 使用深蓝色高亮
            lblCurrent.Font = new Font("Microsoft YaHei", 9, FontStyle.Bold);
            grpSwitch.Controls.Add(lblCurrent);

            // 下拉框
            cmbUsers = new ComboBox();
            cmbUsers.Location = new Point(20, 80);
            cmbUsers.Size = new Size(280, 30);
            cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUsers.Font = new Font("Microsoft YaHei", 10);
            grpSwitch.Controls.Add(cmbUsers);

            // 按钮样式辅助函数
            int btnY = 78;
            
            // 按钮: [+] 添加
            btnAdd = new Button();
            btnAdd.Text = "添加";
            btnAdd.Location = new Point(310, btnY);
            btnAdd.Size = new Size(50, 32);
            btnAdd.FlatStyle = FlatStyle.System; 
            btnAdd.Click += new EventHandler(BtnAdd_Click);
            grpSwitch.Controls.Add(btnAdd);

            // 按钮: [-] 删除
            btnDel = new Button();
            btnDel.Text = "删除";
            btnDel.Location = new Point(365, btnY);
            btnDel.Size = new Size(50, 32);
            btnDel.FlatStyle = FlatStyle.System;
            btnDel.Click += new EventHandler(BtnDel_Click);
            grpSwitch.Controls.Add(btnDel);

            // 按钮: 切换 (高亮显示)
            btnSwitch = new Button();
            btnSwitch.Text = "立即切换身份";
            btnSwitch.Location = new Point(425, btnY);
            btnSwitch.Size = new Size(100, 32);
            btnSwitch.BackColor = Color.FromArgb(40, 167, 69); // 绿色背景
            btnSwitch.ForeColor = Color.Black; 
            btnSwitch.FlatStyle = FlatStyle.Standard; // 保持 Standard 以显示颜色
            btnSwitch.Click += new EventHandler(BtnSwitch_Click);
            grpSwitch.Controls.Add(btnSwitch);

            // 提示文本
            Label lblHint = new Label();
            lblHint.Text = "提示: 切换后请重新打开 IDE 提交窗口";
            lblHint.Location = new Point(20, 125);
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.Gray;
            lblHint.Font = new Font("Microsoft YaHei", 8);
            grpSwitch.Controls.Add(lblHint);

            // --- 区域2: 统计表格 (ListView) ---
            GroupBox grpStats = new GroupBox();
            grpStats.Text = " 工作量审计看板 (Top Contributors) ";
            grpStats.Location = new Point(20, 190);
            grpStats.Size = new Size(540, 320);
            grpStats.Font = mainFont;
            grpStats.FlatStyle = FlatStyle.System;
            this.Controls.Add(grpStats);

            lvStats = new ListView();
            lvStats.Location = new Point(15, 30);
            lvStats.Size = new Size(510, 275);
            lvStats.View = View.Details;
            lvStats.GridLines = true;
            lvStats.FullRowSelect = true;
            lvStats.BorderStyle = BorderStyle.FixedSingle;
            
            // 美化表头
            lvStats.Columns.Add("排名", 60, HorizontalAlignment.Center);
            lvStats.Columns.Add("提交次数", 100, HorizontalAlignment.Center);
            lvStats.Columns.Add("提交人 (Author)", 320, HorizontalAlignment.Left);
            
            grpStats.Controls.Add(lvStats);

            UpdateCombo();
        }

        // --- 事件处理 ---
        private void BtnAdd_Click(object sender, EventArgs e) { OpenAddDialog(); }
        private void BtnDel_Click(object sender, EventArgs e) { DeleteUser(); }
        private void BtnSwitch_Click(object sender, EventArgs e) { SwitchUser(); }

        // --- 逻辑部分 (保持不变，仅 string.Format 兼容) ---
        private void LoadUsers()
        {
            userList.Clear();
            if (File.Exists(configPath))
            {
                try {
                    string[] lines = File.ReadAllLines(configPath, Encoding.UTF8);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 3)
                            userList.Add(new UserProfile(parts[0], parts[1], parts[2]));
                    }
                } catch { }
            }

            if (userList.Count == 0)
            {
                userList.Add(new UserProfile("邓国欣(主负责人)", "502249", "dengguoxin@comac.intra"));
                userList.Add(new UserProfile("赵林强", "502224", "zhaolinqiang@comac.intra"));
                SaveUsers();
            }
        }

        private void SaveUsers()
        {
            List<string> lines = new List<string>();
            foreach (var u in userList)
                lines.Add(string.Format("{0}|{1}|{2}", u.Label, u.Name, u.Email));
            File.WriteAllLines(configPath, lines.ToArray(), Encoding.UTF8);
        }

        private void UpdateCombo()
        {
            cmbUsers.Items.Clear();
            foreach (var u in userList) cmbUsers.Items.Add(u.Label);
            if (cmbUsers.Items.Count > 0) cmbUsers.SelectedIndex = 0;
        }

        private void OpenAddDialog()
        {
            AddUserForm dlg = new AddUserForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                userList.Add(dlg.NewUser);
                SaveUsers();
                UpdateCombo();
                cmbUsers.SelectedIndex = cmbUsers.Items.Count - 1;
                MessageBox.Show("用户已保存！", "成功");
            }
        }

        private void DeleteUser()
        {
            if (cmbUsers.SelectedIndex < 0) return;
            string label = cmbUsers.SelectedItem.ToString();
            string msg = string.Format("确定要删除用户 [{0}] 吗？", label);
            
            if (MessageBox.Show(msg, "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                userList.RemoveAt(cmbUsers.SelectedIndex);
                SaveUsers();
                UpdateCombo();
            }
        }

        private void RefreshData()
        {
            string name = RunGitCommand("config user.name");
            string email = RunGitCommand("config user.email");
            
            if (string.IsNullOrEmpty(name)) 
                lblCurrent.Text = "当前状态: 未检测到 Git 仓库配置 (请将此工具放在项目根目录)";
            else 
                lblCurrent.Text = string.Format("当前生效: {0}  <{1}>", name, email);

            RefreshStatsTable();
        }

        private void RefreshStatsTable()
        {
            lvStats.Items.Clear();
            string rawOutput = RunGitCommand("shortlog -sn --all");

            if (string.IsNullOrEmpty(rawOutput) || rawOutput.Trim().Length == 0) return;

            string[] lines = rawOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int rank = 1;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                int firstSpaceIndex = trimmed.IndexOfAny(new char[] { ' ', '\t' });
                
                if (firstSpaceIndex > 0)
                {
                    string countStr = trimmed.Substring(0, firstSpaceIndex).Trim();
                    string authorName = trimmed.Substring(firstSpaceIndex).Trim();

                    ListViewItem item = new ListViewItem(rank.ToString());
                    item.SubItems.Add(countStr);
                    item.SubItems.Add(authorName);
                    lvStats.Items.Add(item);
                    rank++;
                }
            }
        }

        private void SwitchUser()
        {
            if (cmbUsers.SelectedIndex < 0) return;
            var target = userList[cmbUsers.SelectedIndex];
            
            string cmdName = string.Format("config --local user.name \"{0}\"", target.Name);
            string cmdEmail = string.Format("config --local user.email \"{0}\"", target.Email);

            RunGitCommand(cmdName);
            RunGitCommand(cmdEmail);
            
            string msg = string.Format("✅ 身份已切换为：{0}", target.Name);
            MessageBox.Show(msg, "切换成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshData();
        }

        private string RunGitCommand(string args)
        {
            try {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "git";
                psi.Arguments = args;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.StandardOutputEncoding = Encoding.UTF8;
                using (Process p = Process.Start(psi)) {
                    string o = p.StandardOutput.ReadToEnd(); 
                    p.WaitForExit(); 
                    return o.Trim();
                }
            } catch { return ""; }
        }

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class UserProfile {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserProfile(string l, string n, string e) { Label = l; Name = n; Email = e; }
    }

    // ==========================================
    // 修复后的添加用户弹窗 (针对截图优化布局)
    // ==========================================
    public class AddUserForm : Form {
        public UserProfile NewUser { get; private set; }
        private TextBox txtLabel, txtName, txtEmail;
        private Button btnOk, btnCancel;

        public AddUserForm() {
            this.Text = "添加新用户"; 
            // 1. 加大窗口高度，防止按钮被遮挡
            this.Size = new Size(420, 320); 
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; 
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White; // 纯白背景

            Font uiFont = new Font("Microsoft YaHei", 10);
            int startX = 30;
            int inputWidth = 340;
            int currentY = 25;

            // 标签1
            Label lbl1 = new Label(); 
            lbl1.Text = "显示名称 (例如: 张三-项目经理):"; 
            lbl1.Location = new Point(startX, currentY); 
            lbl1.AutoSize = true;
            lbl1.Font = uiFont;
            lbl1.ForeColor = Color.DarkSlateGray;
            this.Controls.Add(lbl1);
            
            // 输入框1
            txtLabel = new TextBox(); 
            txtLabel.Location = new Point(startX, currentY + 25); 
            txtLabel.Size = new Size(inputWidth, 28);
            txtLabel.Font = uiFont;
            this.Controls.Add(txtLabel);

            currentY += 70; // 增加垂直间距，更宽松

            // 标签2
            Label lbl2 = new Label(); 
            lbl2.Text = "Git User Name (例如: Zhang San):"; 
            lbl2.Location = new Point(startX, currentY); 
            lbl2.AutoSize = true;
            lbl2.Font = uiFont;
            lbl2.ForeColor = Color.DarkSlateGray;
            this.Controls.Add(lbl2);
            
            // 输入框2
            txtName = new TextBox(); 
            txtName.Location = new Point(startX, currentY + 25); 
            txtName.Size = new Size(inputWidth, 28);
            txtName.Font = uiFont;
            this.Controls.Add(txtName);

            currentY += 70;

            // 标签3
            Label lbl3 = new Label(); 
            lbl3.Text = "Git User Email (邮箱):"; 
            lbl3.Location = new Point(startX, currentY); 
            lbl3.AutoSize = true;
            lbl3.Font = uiFont;
            lbl3.ForeColor = Color.DarkSlateGray;
            this.Controls.Add(lbl3);
            
            // 输入框3
            txtEmail = new TextBox(); 
            txtEmail.Location = new Point(startX, currentY + 25); 
            txtEmail.Size = new Size(inputWidth, 28);
            txtEmail.Font = uiFont;
            this.Controls.Add(txtEmail);

            // 底部按钮区域
            // 使用 FlatStyle.System 让按钮看起来像 Win10 原生按钮
            btnOk = new Button(); 
            btnOk.Text = "保存"; 
            btnOk.Location = new Point(200, 230); // 调整位置，防止遮挡
            btnOk.Size = new Size(80, 32); 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatStyle = FlatStyle.System; 

            btnCancel = new Button(); 
            btnCancel.Text = "取消"; 
            btnCancel.Location = new Point(290, 230); // 放在保存按钮右侧
            btnCancel.Size = new Size(80, 32); 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatStyle = FlatStyle.System;
            
            btnOk.Click += new EventHandler(BtnOk_Click);

            this.Controls.Add(btnOk); 
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk; // 回车键默认触发保存
            this.CancelButton = btnCancel; // ESC键触发取消
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (txtLabel.Text.Trim().Length == 0 || txtName.Text.Trim().Length == 0) {
                MessageBox.Show("显示名称和 Git Name 不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return;
            }
            NewUser = new UserProfile(txtLabel.Text, txtName.Text, txtEmail.Text);
            this.Close();
        }
    }
}