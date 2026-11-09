using System;
using System.Data;
using System.Web.UI.WebControls;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 后台管理页：管理员四个工作页签——用户审核、岗位审核、评价审核、数据统计。
    /// 用户禁用 / 恢复走 AdminBLL.SetStudentStatus、SetEmployerStatus；
    /// 岗位审核走 JobBLL.Audit；评价审核走 CommentBLL.Audit；统计走 AdminBLL.GetStatistics。
    /// 页签切换状态保存在 ViewState 中，避免每次回发都退回第一个页签。
    /// </summary>
    public partial class AdminManage : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：admin。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "admin" }; }
    }

        private readonly AdminBLL _adminBll = new AdminBLL();
        private readonly JobBLL _jobBll = new JobBLL();
        private readonly CommentBLL _commentBll = new CommentBLL();

        /// <summary>
        /// 当前页签标识：users / jobs / comments / stats。
        /// </summary>
        private string CurrentTab
        {
            get
            {
                string tab = Convert.ToString(ViewState["Tab"]);
                return string.IsNullOrEmpty(tab) ? "users" : tab;
            }
        }

        /// <summary>
        /// 页面加载：仅管理员可进入；首次进入显示操作员名称并默认打开“用户审核”页签。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                lblUser.Text = SafeText(Convert.ToString(Session["UserName"]));
                SwitchTab("users");
            }
        }

        /// <summary>
        /// 切换页签：更新面板可见性、页签高亮并加载对应数据。
        /// </summary>
        /// <param name="tab">页签标识</param>
        private void SwitchTab(string tab)
        {
            ViewState["Tab"] = tab;

            pnlUsers.Visible = tab == "users";
            pnlJobs.Visible = tab == "jobs";
            pnlComments.Visible = tab == "comments";
            pnlStats.Visible = tab == "stats";

            tabUsers.CssClass = tab == "users" ? "tab tab-active" : "tab";
            tabJobs.CssClass = tab == "jobs" ? "tab tab-active" : "tab";
            tabComments.CssClass = tab == "comments" ? "tab tab-active" : "tab";
            tabStats.CssClass = tab == "stats" ? "tab tab-active" : "tab";

            BindTab(tab);
        }

        /// <summary>
        /// 按页签加载数据。
        /// </summary>
        /// <param name="tab">页签标识</param>
        private void BindTab(string tab)
        {
            switch (tab)
            {
                case "users":
                    BindStudents();
                    BindEmployers();
                    break;
                case "jobs":
                    BindPendingJobs();
                    break;
                case "comments":
                    BindPendingComments();
                    break;
                case "stats":
                    BindStatistics();
                    break;
            }
        }

        /// <summary>
        /// 绑定学生用户列表。
        /// </summary>
        private void BindStudents()
        {
            gvStudent.DataSource = _adminBll.GetAllStudents();
            gvStudent.DataBind();
        }

        /// <summary>
        /// 绑定商家用户列表。
        /// </summary>
        private void BindEmployers()
        {
            gvEmployer.DataSource = _adminBll.GetAllEmployers();
            gvEmployer.DataBind();
        }

        /// <summary>
        /// 绑定待审核岗位列表。
        /// </summary>
        private void BindPendingJobs()
        {
            gvJob.DataSource = _jobBll.GetPendingAudit();
            gvJob.DataBind();
        }

        /// <summary>
        /// 绑定待审核评价列表。
        /// </summary>
        private void BindPendingComments()
        {
            gvComment.DataSource = _commentBll.GetPendingAudit();
            gvComment.DataBind();
        }

        /// <summary>
        /// 读取平台统计数据。
        /// </summary>
        private void BindStatistics()
        {
            int stuCount;
            int empCount;
            int jobCount;
            int appCount;
            int commentCount;
            _adminBll.GetStatistics(out stuCount, out empCount, out jobCount, out appCount, out commentCount);

            lblStuCount.Text = stuCount.ToString();
            lblEmpCount.Text = empCount.ToString();
            lblJobCount.Text = jobCount.ToString();
            lblAppCount.Text = appCount.ToString();
            lblCommentCount.Text = commentCount.ToString();
        }

        /// <summary>
        /// 解析命令参数中携带的“主键|当前状态”。
        /// </summary>
        /// <param name="argument">命令参数</param>
        /// <param name="id">主键</param>
        /// <param name="status">当前状态值，解析失败时为 0</param>
        /// <returns>解析成功返回 true</returns>
        private static bool ParseArgument(string argument, out int id, out int status)
        {
            id = 0;
            status = 0;

            string[] parts = Convert.ToString(argument).Split('|');
            if (parts.Length != 2 || !int.TryParse(parts[0], out id))
            {
                id = 0;
                return false;
            }

            int.TryParse(parts[1], out status);
            return true;
        }

        /// <summary>
        /// 显示操作结果提示。
        /// </summary>
        /// <param name="ok">是否成功</param>
        /// <param name="message">提示文本</param>
        private void ShowResult(bool ok, string message)
        {
            lblMsg.CssClass = ok ? "msg-success" : "msg-error";
            lblMsg.Text = SafeText(message);
        }

        /// <summary>
        /// 页签点击：切换到目标页签并清空上一次的提示。
        /// </summary>
        protected void tab_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            lblMsg.CssClass = string.Empty;

            LinkButton link = sender as LinkButton;
            SwitchTab(link == null ? "users" : link.CommandArgument);
        }

        /// <summary>
        /// 学生列表行绑定：显示状态中文并按状态给出“禁用/恢复”按钮文案。
        /// </summary>
        protected void gvStudent_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = e.Row.DataItem as DataRowView;
            if (row == null)
            {
                return;
            }

            byte status = Convert.ToByte(row["stu_status"]);
            bool disabled = status == 1;

            Label lblStuStatus = (Label)e.Row.FindControl("lblStuStatus");
            LinkButton btnStuStatus = (LinkButton)e.Row.FindControl("btnStuStatus");

            lblStuStatus.Text = disabled ? "已禁用" : "正常";
            btnStuStatus.Text = disabled ? "恢复" : "禁用";
            btnStuStatus.CssClass = disabled ? "link-btn" : "link-btn link-danger";

            if (!disabled)
            {
                btnStuStatus.OnClientClick = "return confirm('确认禁用该学生账号吗？禁用后该学生将无法登录。');";
            }
            else
            {
                btnStuStatus.OnClientClick = "return confirm('确认恢复该学生账号的登录权限吗？');";
            }
        }

        /// <summary>
        /// 商家列表行绑定：显示状态中文并按状态给出“禁用/恢复”按钮文案。
        /// </summary>
        protected void gvEmployer_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = e.Row.DataItem as DataRowView;
            if (row == null)
            {
                return;
            }

            byte status = Convert.ToByte(row["emp_status"]);
            bool disabled = status == 1;

            Label lblEmpStatus = (Label)e.Row.FindControl("lblEmpStatus");
            LinkButton btnEmpStatus = (LinkButton)e.Row.FindControl("btnEmpStatus");

            lblEmpStatus.Text = disabled ? "已禁用" : "正常";
            btnEmpStatus.Text = disabled ? "恢复" : "禁用";
            btnEmpStatus.CssClass = disabled ? "link-btn" : "link-btn link-danger";

            if (!disabled)
            {
                btnEmpStatus.OnClientClick = "return confirm('确认禁用该商家账号吗？禁用后该商家将无法登录，其岗位不再对学生展示。');";
            }
            else
            {
                btnEmpStatus.OnClientClick = "return confirm('确认恢复该商家账号的登录权限吗？');";
            }
        }

        /// <summary>
        /// 学生列表操作：按当前状态取反，执行禁用或恢复。
        /// </summary>
        protected void gvStudent_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ToggleStudent")
            {
                return;
            }

            int studentId;
            int status;
            if (!ParseArgument(Convert.ToString(e.CommandArgument), out studentId, out status))
            {
                return;
            }

            string message;
            bool ok = _adminBll.SetStudentStatus(studentId, status != 1, out message);

            ShowResult(ok, message);
            BindStudents();
        }

        /// <summary>
        /// 商家列表操作：按当前状态取反，执行禁用或恢复。
        /// </summary>
        protected void gvEmployer_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ToggleEmployer")
            {
                return;
            }

            int employerId;
            int status;
            if (!ParseArgument(Convert.ToString(e.CommandArgument), out employerId, out status))
            {
                return;
            }

            string message;
            bool ok = _adminBll.SetEmployerStatus(employerId, status != 1, out message);

            ShowResult(ok, message);
            BindEmployers();
        }

        /// <summary>
        /// 岗位审核：通过或驳回待审核岗位。
        /// </summary>
        protected void gvJob_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool pass;
            if (e.CommandName == "PassJob")
            {
                pass = true;
            }
            else if (e.CommandName == "RejectJob")
            {
                pass = false;
            }
            else
            {
                return;
            }

            int jobId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobId))
            {
                return;
            }

            string message;
            bool ok = _jobBll.Audit(jobId, pass, out message);

            ShowResult(ok, message);
            BindPendingJobs();
        }

        /// <summary>
        /// 评价审核：通过或驳回待审核评价。
        /// </summary>
        protected void gvComment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool pass;
            if (e.CommandName == "PassComment")
            {
                pass = true;
            }
            else if (e.CommandName == "RejectComment")
            {
                pass = false;
            }
            else
            {
                return;
            }

            int commentId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out commentId))
            {
                return;
            }

            string message;
            bool ok = _commentBll.Audit(commentId, pass, out message);

            ShowResult(ok, message);
            BindPendingComments();
        }

        /// <summary>
        /// 刷新统计按钮：重新读取平台统计数据。
        /// </summary>
        protected void btnRefreshStats_Click(object sender, EventArgs e)
        {
            BindStatistics();
            ShowResult(true, "统计数据已刷新。");
        }
    }
}
