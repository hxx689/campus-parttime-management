using System;
using System.Web;

namespace CampusPartTime
{
    /// <summary>
    /// 全局应用程序类：负责应用启动时的基础校验与全局未处理异常记录。
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// 应用启动事件：校验连接字符串是否已配置。
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["CampusConn"];
            if (cs == null || string.IsNullOrEmpty(cs.ConnectionString))
            {
                throw new ApplicationException("未找到数据库连接字符串 CampusConn，请检查 Web.config 配置。");
            }
        }

        /// <summary>
        /// 全局异常处理：记录异常并转到登录页，避免向客户端暴露堆栈信息。
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Server.ClearError();
                Response.Redirect("~/Login.aspx", true);
            }
        }

        /// <summary>
        /// 会话开始时的事件占位，便于后续扩展登录日志。
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
        }
    }
}
