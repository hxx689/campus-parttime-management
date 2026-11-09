using System;
using System.Data;
using System.Web.UI.WebControls;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 岗位管理页：商家查看本人发布的全部岗位，支持修改与下架（逻辑删除）。
    /// </summary>
    public partial class JobManage : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：employer。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "employer" }; }
    }

        private readonly JobBLL _jobBll = new JobBLL();

        /// <summary>
        /// 页面加载：仅商家可进入；首次进入绑定岗位列表。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindJobList();
            }
        }

        /// <summary>
        /// 绑定当前商家的岗位列表。
        /// </summary>
        private void BindJobList()
        {
            DataTable dt = _jobBll.GetByEmployer(CurrentUserId);
            gvJob.DataSource = dt;
            gvJob.DataBind();
        }

        /// <summary>
        /// 行数据绑定：把审核状态、岗位状态的数值翻译为中文。
        /// </summary>
        protected void gvJob_RowDataBound(object sender, GridViewRowEventArgs e)
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

            Label lblAudit = (Label)e.Row.FindControl("lblAudit");
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");

            byte audit = Convert.ToByte(row["audit_status"]);
            byte status = Convert.ToByte(row["job_status"]);

            lblAudit.Text = audit == 0 ? "待审核" : (audit == 1 ? "已通过" : "已驳回");
            lblStatus.Text = status == 0 ? "在招" : (status == 1 ? "已招满" : "已下架");

            // 已下架岗位不再提供操作按钮
            LinkButton btnOffline = (LinkButton)e.Row.FindControl("btnOffline");
            LinkButton btnEdit = (LinkButton)e.Row.FindControl("btnEdit");
            if (status == 2)
            {
                btnOffline.Visible = false;
                btnEdit.Visible = false;
            }
        }

        /// <summary>
        /// 行命令处理：修改跳转到发布页，下架调用业务层逻辑删除。
        /// </summary>
        protected void gvJob_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int jobId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobId))
            {
                return;
            }

            if (e.CommandName == "EditJob")
            {
                Response.Redirect("JobPublish.aspx?jobId=" + jobId, false);
                return;
            }

            if (e.CommandName == "OfflineJob")
            {
                string message;
                bool ok = _jobBll.Offline(jobId, CurrentUserId, out message);

                lblMsg.CssClass = ok ? "msg-success" : "msg-error";
                lblMsg.Text = SafeText(message);

                if (ok)
                {
                    BindJobList();
                }
            }
        }
    }
}
