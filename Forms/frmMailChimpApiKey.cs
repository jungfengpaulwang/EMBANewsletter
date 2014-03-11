using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;

namespace Newsletter.Forms
{
    public partial class frmMailChimpApiKey : BaseForm
    {
        private AccessHelper Access;
        private ErrorProvider ErrorProvider1;
        private UDT.MailChimpAPIKey MailChimpAPIKey;

        public frmMailChimpApiKey()
        {
            InitializeComponent();

            Access = new AccessHelper();
            ErrorProvider1 = new ErrorProvider();
            MailChimpAPIKey = new UDT.MailChimpAPIKey();
            this.Load += new EventHandler(Form_Load);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            List<UDT.MailChimpAPIKey> MailChimpAPIKeys = Access.Select<UDT.MailChimpAPIKey>();
            if (MailChimpAPIKeys.Count > 0)
            {
                this.MailChimpAPIKey = MailChimpAPIKeys.ElementAt(0);

                this.txtAPIKey.Text = this.MailChimpAPIKey.APIKey;
            }
        }            

        private void Save_Click(object sender, EventArgs e)
        {
            bool is_validated = true;
            ErrorProvider1.Clear();
            if (string.IsNullOrWhiteSpace(this.txtAPIKey.Text))
            {
                is_validated = false;
                ErrorProvider1.SetError(this.txtAPIKey, "必填");
            }
            if (!is_validated)
                return;

            this.MailChimpAPIKey.APIKey = this.txtAPIKey.Text.Trim();

            try
            {
                this.MailChimpAPIKey.Save();
                MessageBox.Show("儲存成功。");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
