using System;
using System.Web.Security;

namespace CampusPartTime
{
    /// <summary>
    /// 退出登录页：清空会话并删除表单认证票据，返回登录页。
    /// 该页作为辅助页存在，不计入设计文档第四章的 11 个业务页面。
    /// </summary>
    public partial class Logout : System.Web.UI.Page
    {
        /// <summary>
        /// 页面加载：立即执行退出处理。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Login.aspx", true);
        }
    }
}
