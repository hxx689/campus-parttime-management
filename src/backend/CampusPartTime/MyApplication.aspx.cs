using System;
using System.Data;
using System.Web.UI.WebControls;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 我的申请页：学生查看本人全部岗位申请及处理状态，支持按状态筛选；
    /// 被录用的申请可跳转到评价页提交评价。
    /// </summary>
    public partial class MyApplication : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：student。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "student" }; }
    }

        private readonly ApplicationBLL _appBll = new ApplicationBLL();

        /// <summary>
        /// 页面加载：仅学生可进入；首次进入显示当前登录学生姓名并绑定申请列表。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                lblUser.Text = SafeText(Convert.ToString(Session["UserName"]));
                BindAppList();
            }
        }

        /// <summary>
        /// 绑定当前学生的申请列表，并按状态下拉框过滤。
        /// </summary>
        private void BindAppList()
        {
            DataTable dt = _appBll.GetByStudent(CurrentUserId);

            DataView view = dt.DefaultView;
            string status = ddlStatus.SelectedValue;

            // 下拉选项为固定值，此处再显式校验一次，确保进入 RowFilter 的只有 0/1/2
            if (status == "0" || status == "1" || status == "2")
            {
                view.RowFilter = "app_status = " + status;
            }

            gvApp.DataSource = view;
            gvApp.DataBind();

            int pending = 0;
            int hired = 0;
            int rejected = 0;
            foreach (DataRow row in dt.Rows)
            {
                byte appStatus = Convert.ToByte(row["app_status"]);
                if (appStatus == 0)
                {
                    pending++;
                }
                else if (appStatus == 1)
                {
                    hired++;
                }
                else
                {
                    rejected++;
                }
            }

            lblSummary.Text = string.Format("共 {0} 条申请：待处理 {1} 条，已录用 {2} 条，已拒绝 {3} 条。",
                dt.Rows.Count, pending, hired, rejected);
        }

        /// <summary>
        /// 行数据绑定：把申请状态、岗位状态的数值翻译为中文；
        /// 只有状态为已录用（app_status=1）的申请才显示“去评价”入口。
        /// </summary>
        protected void gvApp_RowDataBound(object sender, GridViewRowEventArgs e)
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

            byte appStatus = Convert.ToByte(row["app_status"]);
            byte jobStatus = Convert.ToByte(row["job_status"]);

            Label lblAppStatus = (Label)e.Row.FindControl("lblAppStatus");
            Label lblJobStatus = (Label)e.Row.FindControl("lblJobStatus");
            LinkButton btnComment = (LinkButton)e.Row.FindControl("btnComment");

            lblAppStatus.Text = appStatus == 0 ? "待处理" : (appStatus == 1 ? "已录用" : "已拒绝");
            lblJobStatus.Text = jobStatus == 0 ? "在招" : (jobStatus == 1 ? "已招满" : "已下架");

            // 仅被录用的学生可以评价该岗位，此处只控制入口显示，
            // 真正的业务校验在 CommentBLL.Submit 中再次执行（已录用、未重复评价）。
            btnComment.Visible = appStatus == 1;
        }

        /// <summary>
        /// 行命令处理：查看岗位详情、跳转评价页。
        /// </summary>
        protected void gvApp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int jobId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobId))
            {
                return;
            }

            if (e.CommandName == "ViewJob")
            {
                Response.Redirect("JobDetail.aspx?jobId=" + jobId, false);
                return;
            }

            if (e.CommandName == "WriteComment")
            {
                Response.Redirect("CommentSubmit.aspx?jobId=" + jobId, false);
            }
        }

        /// <summary>
        /// 筛选按钮：按申请状态重新绑定列表。
        /// </summary>
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            lblMsg.CssClass = string.Empty;
            BindAppList();
        }
    }
}
