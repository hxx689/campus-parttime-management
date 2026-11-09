using System;
using System.Data;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 岗位详情页：展示岗位完整信息与商家联系方式，并处理学生申请入口。
    /// 岗位数据统一来源于视图 v_job_detail，避免页面重复编写多表连接。
    /// </summary>
    public partial class JobDetail : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：student。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "student", "employer", "admin" }; }
    }

        private readonly JobBLL _jobBll = new JobBLL();
        private readonly ApplicationBLL _appBll = new ApplicationBLL();
        private readonly CommentBLL _commentBll = new CommentBLL();

        /// <summary>当前请求的岗位编号，非法时返回 0。</summary>
        protected int JobId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["jobId"], out id) ? id : 0;
            }
        }

        /// <summary>
        /// 页面加载：首次进入时绑定岗位详情、评价列表与按钮可用状态。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (JobId <= 0)
                {
                    Response.Redirect("JobList.aspx", false);
                    return;
                }
                BindDetail();
            }
        }

        /// <summary>
        /// 绑定岗位详情。数据取自视图 v_job_detail（仅含审核通过的岗位）。
        /// </summary>
        private void BindDetail()
        {
            DataTable dt = _jobBll.GetJobDetail(JobId);

            if (dt == null || dt.Rows.Count == 0)
            {
                lblJobName.Text = "岗位信息不可用";
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = "该岗位不存在或尚未通过审核。";
                btnApply.Enabled = false;
                btnToComment.Enabled = false;
                gvComments.DataSource = null;
                gvComments.DataBind();
                return;
            }

            DataRow r = dt.Rows[0];

            lblJobName.Text = SafeText(Convert.ToString(r["job_name"]));
            lblJobType.Text = SafeText(Convert.ToString(r["job_type"]));
            lblSalary.Text = string.Format("{0:F2} {1}",
                Convert.ToDecimal(r["salary"]), SafeText(Convert.ToString(r["salary_unit"])));
            lblWorkPlace.Text = SafeText(Convert.ToString(r["work_place"]));
            lblWorkTime.Text = SafeText(Convert.ToString(r["work_time"]));
            lblRecruitNum.Text = string.Format("招聘 {0} 人，已录用 {1} 人",
                Convert.ToInt32(r["recruit_num"]), Convert.ToInt32(r["hired_num"]));
            lblDeadline.Text = Convert.ToDateTime(r["deadline"]).ToString("yyyy-MM-dd");
            lblJobStatus.Text = GetJobStatusText(Convert.ToByte(r["job_status"]));

            string desc = Convert.ToString(r["job_desc"]);
            lblJobDesc.Text = SafeText(string.IsNullOrEmpty(desc) ? "（暂无岗位描述）" : desc);
            lblJobDesc.Style["white-space"] = "pre-wrap";

            lblEmpName.Text = SafeText(Convert.ToString(r["emp_name"]));
            lblEmpContact.Text = SafeText(Convert.ToString(r["emp_contact"]));
            lblEmpPhone.Text = SafeText(Convert.ToString(r["emp_phone"]));

            gvComments.DataSource = _commentBll.GetApprovedByJob(JobId);
            gvComments.DataBind();

            byte jobStatus = Convert.ToByte(r["job_status"]);

            // 非学生角色不提供申请与评价入口
            if (CurrentRole != "student")
            {
                btnApply.Enabled = false;
                btnApply.Text = "仅学生可申请";
                btnToComment.Enabled = false;
                return;
            }

            int status = _appBll.GetStatus(CurrentUserId, JobId);
            if (status >= 0)
            {
                btnApply.Enabled = false;
                btnApply.Text = "已申请";
                lblMsg.CssClass = "msg-success";
                lblMsg.Text = "您已申请过该岗位，当前状态：" + GetApplyStatusText(status);
            }

            if (jobStatus != 0)
            {
                btnApply.Enabled = false;
                btnApply.Text = "岗位不可申请";
            }

            // 仅“已录用”的学生可以对该岗位提交评价
            btnToComment.Enabled = (status == 1);
        }

        /// <summary>
        /// 申请按钮：调用业务层申请方法（内部使用 proc_apply_job 存储过程）。
        /// </summary>
        protected void btnApply_Click(object sender, EventArgs e)
        {
            string message;
            bool ok = _appBll.Apply(CurrentUserId, JobId, out message);

            lblMsg.CssClass = ok ? "msg-success" : "msg-error";
            lblMsg.Text = SafeText(message);

            if (ok)
            {
                btnApply.Enabled = false;
                btnApply.Text = "已申请";
            }
        }

        /// <summary>
        /// 评价按钮：跳转到评价提交页，并携带岗位编号。
        /// </summary>
        protected void btnToComment_Click(object sender, EventArgs e)
        {
            Response.Redirect("CommentSubmit.aspx?jobId=" + JobId, false);
        }

        /// <summary>岗位状态文字。0 在招 / 1 已招满 / 2 已下架。</summary>
        private string GetJobStatusText(byte status)
        {
            switch (status)
            {
                case 0: return "在招";
                case 1: return "已招满";
                case 2: return "已下架";
                default: return "未知";
            }
        }

        /// <summary>申请状态文字。0 待处理 / 1 已录用 / 2 已拒绝。</summary>
        private string GetApplyStatusText(int status)
        {
            switch (status)
            {
                case 0: return "待处理";
                case 1: return "已录用";
                case 2: return "已拒绝";
                default: return "未申请";
            }
        }
    }
}
