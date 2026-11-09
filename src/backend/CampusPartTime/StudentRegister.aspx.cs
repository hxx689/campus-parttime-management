using System;
using System.Configuration;
using System.IO;
using CampusPartTime.BLL;
using CampusPartTime.Model;

namespace CampusPartTime
{
    /// <summary>
    /// 学生注册页：采集学生基本信息，密码经业务层 BCrypt 哈希后入库；头像做扩展名白名单与大小校验。
    /// </summary>
    public partial class StudentRegister : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.TextBox txtStuNo;
        protected System.Web.UI.WebControls.TextBox txtStuName;
        protected System.Web.UI.WebControls.RadioButton rdoMale;
        protected System.Web.UI.WebControls.RadioButton rdoFemale;
        protected System.Web.UI.WebControls.TextBox txtBirth;
        protected System.Web.UI.WebControls.TextBox txtCollege;
        protected System.Web.UI.WebControls.TextBox txtMajor;
        protected System.Web.UI.WebControls.TextBox txtGrade;
        protected System.Web.UI.WebControls.TextBox txtPhone;
        protected System.Web.UI.WebControls.TextBox txtEmail;
        protected System.Web.UI.WebControls.TextBox txtPwd;
        protected System.Web.UI.WebControls.TextBox txtPwd2;
        protected System.Web.UI.WebControls.FileUpload fuAvatar;
        protected System.Web.UI.WebControls.Button btnRegister;
        protected System.Web.UI.WebControls.Label lblMsg;

        private readonly UserBLL _userBll = new UserBLL();

        /// <summary>
        /// 页面加载：首次进入时无需额外处理。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 提交注册：先做服务端二次校验，再调用业务层写入数据库。
        /// </summary>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStuNo.Text) || string.IsNullOrWhiteSpace(txtStuName.Text))
            {
                lblMsg.Text = "学号与姓名不能为空。";
                return;
            }
            if (txtPwd.Text.Length < 6)
            {
                lblMsg.Text = "密码长度不能少于 6 位。";
                return;
            }
            if (txtPwd.Text != txtPwd2.Text)
            {
                lblMsg.Text = "两次输入的密码不一致。";
                return;
            }
            if (!IsValidPhone(txtPhone.Text.Trim()))
            {
                lblMsg.Text = "联系电话须为 11 位手机号。";
                return;
            }

            string uploadError;
            if (!ValidateUpload(out uploadError))
            {
                lblMsg.Text = uploadError;
                return;
            }

            Student m = new Student
            {
                StuNo = txtStuNo.Text.Trim(),
                StuName = txtStuName.Text.Trim(),
                StuSex = rdoFemale.Checked ? "女" : "男",
                StuBirth = ParseDate(txtBirth.Text),
                StuCollege = txtCollege.Text.Trim(),
                StuMajor = txtMajor.Text.Trim(),
                StuGrade = txtGrade.Text.Trim(),
                StuPhone = txtPhone.Text.Trim(),
                StuEmail = txtEmail.Text.Trim(),
                StuPassword = txtPwd.Text
            };

            string message;
            bool ok = _userBll.RegisterStudent(m, out message);
            lblMsg.Text = message;
            if (ok)
            {
                lblMsg.CssClass = "msg-success";
                btnRegister.Enabled = false;
            }
        }

        /// <summary>
        /// 校验上传文件：扩展名白名单与 2MB 大小上限，符合要求时保存到 Uploads 目录。
        /// </summary>
        /// <param name="errorMessage">输出参数：校验失败原因</param>
        /// <returns>校验通过返回 true</returns>
        private bool ValidateUpload(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (fuAvatar == null || !fuAvatar.HasFile)
            {
                return true;
            }

            string allow = ConfigurationManager.AppSettings["UploadImageExtensions"] ?? ".jpg,.jpeg,.png";
            string ext = Path.GetExtension(fuAvatar.FileName).ToLower();
            string[] allowList = allow.Split(',');
            if (Array.IndexOf(allowList, ext) < 0)
            {
                errorMessage = "头像格式不允许，仅支持 " + allow + "。";
                return false;
            }

            int maxKb = 2048;
            int.TryParse(ConfigurationManager.AppSettings["UploadMaxSizeKB"], out maxKb);
            if (fuAvatar.PostedFile.ContentLength > maxKb * 1024)
            {
                errorMessage = "头像大小不能超过 " + maxKb + " KB。";
                return false;
            }

            string folder = Server.MapPath(ConfigurationManager.AppSettings["UploadFolder"] ?? "~/Uploads/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string newName = Guid.NewGuid().ToString("N") + ext;
            fuAvatar.SaveAs(Path.Combine(folder, newName));
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

        /// <summary>
        /// 把日期文本解析为可空日期。
        /// </summary>
        /// <param name="text">日期文本</param>
        /// <returns>解析成功返回日期，失败或空返回 null</returns>
        private static DateTime? ParseDate(string text)
        {
            DateTime dt;
            return DateTime.TryParse(text, out dt) ? dt : (DateTime?)null;
        }
    }
}
