using System;
using System.Configuration;
using System.IO;
using CampusPartTime.BLL;
using CampusPartTime.Model;

namespace CampusPartTime
{
    /// <summary>
    /// 商家注册页：采集商家基本信息，联系电话作为后续登录账号。
    /// </summary>
    public partial class EmployerRegister : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.TextBox txtEmpName;
        protected System.Web.UI.WebControls.TextBox txtContact;
        protected System.Web.UI.WebControls.TextBox txtPhone;
        protected System.Web.UI.WebControls.TextBox txtAddress;
        protected System.Web.UI.WebControls.TextBox txtEmail;
        protected System.Web.UI.WebControls.TextBox txtIntro;
        protected System.Web.UI.WebControls.TextBox txtPwd;
        protected System.Web.UI.WebControls.TextBox txtPwd2;
        protected System.Web.UI.WebControls.FileUpload fuLicense;
        protected System.Web.UI.WebControls.CheckBox chkAgree;
        protected System.Web.UI.WebControls.Button btnRegister;
        protected System.Web.UI.WebControls.Label lblMsg;

        private readonly UserBLL _userBll = new UserBLL();

        /// <summary>
        /// 页面加载：首次进入无需处理。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 提交注册：完成服务端校验后调用业务层写入数据库。
        /// </summary>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpName.Text) || string.IsNullOrWhiteSpace(txtContact.Text))
            {
                lblMsg.Text = "商家名称与联系人不能为空。";
                return;
            }
            if (!IsValidPhone(txtPhone.Text.Trim()))
            {
                lblMsg.Text = "联系电话须为 11 位手机号。";
                return;
            }
            if (string.IsNullOrEmpty(txtPwd.Text) || txtPwd.Text.Length < 6)
            {
                lblMsg.Text = "密码长度不能少于 6 位。";
                return;
            }
            if (txtPwd.Text != txtPwd2.Text)
            {
                lblMsg.Text = "两次输入的密码不一致。";
                return;
            }
            if (!chkAgree.Checked)
            {
                lblMsg.Text = "请先勾选同意平台发布规范。";
                return;
            }

            string uploadError;
            if (!ValidateUpload(out uploadError))
            {
                lblMsg.Text = uploadError;
                return;
            }

            Employer m = new Employer
            {
                EmpName = txtEmpName.Text.Trim(),
                EmpContact = txtContact.Text.Trim(),
                EmpPhone = txtPhone.Text.Trim(),
                EmpAddress = txtAddress.Text.Trim(),
                EmpIntro = txtIntro.Text.Trim(),
                EmpEmail = txtEmail.Text.Trim(),
                EmpPassword = txtPwd.Text
            };

            string message;
            bool ok = _userBll.RegisterEmployer(m, out message);
            lblMsg.Text = message;
            if (ok)
            {
                lblMsg.CssClass = "msg-success";
                btnRegister.Enabled = false;
            }
        }

        /// <summary>
        /// 校验营业执照上传：扩展名白名单与 2MB 大小上限。
        /// </summary>
        /// <param name="errorMessage">输出参数：校验失败原因</param>
        /// <returns>校验通过返回 true</returns>
        private bool ValidateUpload(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (fuLicense == null || !fuLicense.HasFile)
            {
                return true;
            }

            string allow = ConfigurationManager.AppSettings["UploadImageExtensions"] ?? ".jpg,.jpeg,.png";
            string ext = Path.GetExtension(fuLicense.FileName).ToLower();
            if (Array.IndexOf(allow.Split(','), ext) < 0)
            {
                errorMessage = "文件格式不允许，仅支持 " + allow + "。";
                return false;
            }

            int maxKb = 2048;
            int.TryParse(ConfigurationManager.AppSettings["UploadMaxSizeKB"], out maxKb);
            if (fuLicense.PostedFile.ContentLength > maxKb * 1024)
            {
                errorMessage = "文件大小不能超过 " + maxKb + " KB。";
                return false;
            }

            string folder = Server.MapPath(ConfigurationManager.AppSettings["UploadFolder"] ?? "~/Uploads/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            fuLicense.SaveAs(Path.Combine(folder, Guid.NewGuid().ToString("N") + ext));
            return true;
        }

        /// <summary>
        /// 判断是否为 11 位手机号。
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>合法返回 true</returns>
        private static bool IsValidPhone(string phone)
        {
            if (phone.Length != 11)
            {
                return false;
            }
            foreach (char c in phone)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
