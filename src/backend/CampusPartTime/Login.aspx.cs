using System;
using System.Web.Security;
using CampusPartTime.BLL;

namespace CampusPartTime
{
    /// <summary>
    /// 登录页：按所选角色校验账号密码，成功后写入会话并按角色跳转。
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        // 以下控件字段由 ASP.NET 页面解析器在运行时按 ID 自动赋值，与设计器生成的字段等价
        protected System.Web.UI.WebControls.TextBox txtUser;
        protected System.Web.UI.WebControls.TextBox txtPwd;
        protected System.Web.UI.WebControls.DropDownList ddlRole;
        protected System.Web.UI.WebControls.Button btnLogin;
        protected System.Web.UI.WebControls.Label lblMsg;

        private readonly UserBLL _userBll = new UserBLL();

        /// <summary>
        /// 页面加载：已登录用户直接跳转对应首页。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Session["RoleId"] != null)
            {
                Response.Redirect(GetHomeUrl(Session["RoleId"].ToString()), true);
            }
        }

        /// <summary>
        /// 登录按钮事件：调用业务层完成 BCrypt 校验，成功后建立会话与表单认证票据。
        /// </summary>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string role = ddlRole.SelectedValue;
            int userId;
            string displayName;
            string message;

            bool ok = _userBll.Login(role, txtUser.Text, txtPwd.Text, out userId, out displayName, out message);
            if (!ok)
            {
                lblMsg.Text = message;
                return;
            }

            Session["RoleId"] = role;
            Session["UserName"] = displayName;
            switch (role)
            {
                case "student":
                    Session["StudentId"] = userId;
                    break;
                case "employer":
                    Session["EmployerId"] = userId;
                    break;
                case "admin":
                    Session["AdminId"] = userId;
                    break;
            }

            // 建立表单认证票据，与 Web.config 的 Forms 认证配合完成匿名访问拦截
            FormsAuthentication.SetAuthCookie(role + ":" + userId, false);
            Response.Redirect(GetHomeUrl(role), true);
        }

        /// <summary>
        /// 按角色返回登录后的默认首页地址。
        /// </summary>
        /// <param name="role">角色标识</param>
        /// <returns>页面相对地址</returns>
        private static string GetHomeUrl(string role)
        {
            switch (role)
            {
                case "student":
                    return "~/JobList.aspx";
                case "employer":
                    return "~/JobManage.aspx";
                case "admin":
                    return "~/AdminManage.aspx";
                default:
                    return "~/Login.aspx";
            }
        }
    }
}
