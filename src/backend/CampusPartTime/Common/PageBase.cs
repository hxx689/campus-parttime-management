using System;
using System.Web.UI;

namespace CampusPartTime.Common
{
    /// <summary>
    /// 页面基类：统一完成登录状态与角色权限校验。
    /// 业务页面继承本类，并通过重写 AllowRoles 声明允许访问的角色标识（student / employer / admin）。
    /// 校验在 OnInit 阶段执行，先于页面 Load 与控件事件，无法通过直接构造 URL 绕过。
    /// </summary>
    public class PageBase : Page
    {
        /// <summary>
        /// 允许访问本页面的角色标识数组；返回空数组表示只要求登录、不限定角色。
        /// </summary>
        protected virtual string[] AllowRoles
        {
            get { return new string[] { }; }
        }

        /// <summary>
        /// 当前登录角色：student / employer / admin；未登录返回空串。
        /// </summary>
        protected string CurrentRole
        {
            get { return Session["RoleId"] == null ? string.Empty : Session["RoleId"].ToString(); }
        }

        /// <summary>
        /// 当前登录用户编号：学生取 StudentId、商家取 EmployerId、管理员取 AdminId；未登录返回 0。
        /// </summary>
        protected int CurrentUserId
        {
            get
            {
                switch (CurrentRole)
                {
                    case "student":
                        return Session["StudentId"] == null ? 0 : Convert.ToInt32(Session["StudentId"]);
                    case "employer":
                        return Session["EmployerId"] == null ? 0 : Convert.ToInt32(Session["EmployerId"]);
                    case "admin":
                        return Session["AdminId"] == null ? 0 : Convert.ToInt32(Session["AdminId"]);
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// 页面初始化：未登录跳转登录页，角色不符跳转登录页。
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (string.IsNullOrEmpty(CurrentRole))
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }

            string[] allow = AllowRoles;
            if (allow != null && allow.Length > 0 && Array.IndexOf(allow, CurrentRole) < 0)
            {
                Response.Redirect("~/Login.aspx", true);
            }
        }

        /// <summary>
        /// 统一输出编码：把用户输入中的尖括号等字符转义，防止跨站脚本注入。
        /// </summary>
        /// <param name="input">用户输入的原始文本</param>
        /// <returns>可直接输出到页面的安全文本</returns>
        protected string SafeText(string input)
        {
            return System.Web.HttpUtility.HtmlEncode(input ?? string.Empty);
        }

        /// <summary>
        /// 生成统一格式的错误提示文本。
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <returns>带前缀的提示文本</returns>
        protected string ErrorTip(string message)
        {
            return "操作失败：" + message;
        }
    }
}
