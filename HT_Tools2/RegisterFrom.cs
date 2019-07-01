using System;
using System.Configuration;
using System.Windows.Forms;

namespace HT_Tools2
{
    public partial class RegisterFrom : Form
    {
        public RegisterFrom()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //校验原始字符串

            var key = PFunc.GetCheckStr();


            if (PFunc.GetKey(textBox2.Text) == key)
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["Reg"].Value = textBox2.Text;
                configuration.Save(ConfigurationSaveMode.Modified);
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show(@"输入信息有误,请联系管理员");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = $"请将串号全部复制并发送给管理员" +
                          $"{Environment.NewLine}{Environment.NewLine}待管理员将注册码发来时粘贴进注册码文本框" +
                          $"{Environment.NewLine}{Environment.NewLine}然后点击注册即可.";

            textBox1.Text = PFunc.GetCheckStr();
        }
    }
}