using System;
using System.Data;
using CampusPartTime.BLL;
using CampusPartTime.Common;
using CampusPartTime.Model;

namespace CampusPartTime
{
    /// <summary>
    /// 岗位评价页：仅当学生对该岗位的申请处于“已录用”状态时才开放评价，
    /// 评分 1～5 星、文字评价 5～500 字，提交后审核状态为待审核（audit_status=0）。
    /// </summary>
    public partial class CommentSubmit : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：student。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "student" }; }
    }

        private readonly JobBLL _jobBll = new JobBLL();
        private readonly ApplicationBLL _appBll = new ApplicationBLL();
        private readonly CommentBLL _commentBll = new CommentBLL();

        /// <summary>
        /// 当前页面操作的岗位编号，由查询字符串 jobId 传入。
        /// </summary>
        private int JobId
        {
            get
            {
                int jobId;
                if (int.TryParse(Request.QueryString["jobId"], out jobId) && jobId > 0)
                {
                    return jobId;
                }
                return 0;
            }
        }

        /// <summary>
        /// 页面加载：仅学生可进入；校验岗位存在性与申请状态，决定是否显示评价表单。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                lblUser.Text = SafeText(Convert.ToString(Session["UserName"]));
                LoadJobAndCheck();
            }
        }

        /// <summary>
        /// 读取岗位信息并校验当前学生是否可以评价该岗位。
        /// </summary>
        private void LoadJobAndCheck()
        {
            int jobId = JobId;
            if (jobId == 0)
            {
                ShowError("缺少岗位参数，请从“我的申请”页面进入评价。");
                return;
            }

            DataTable jobTable = _jobBll.GetJobDetail(jobId);
            DataRow job = (jobTable == null || jobTable.Rows.Count == 0) ? null : jobTable.Rows[0];
            if (job == null)
            {
                ShowError("岗位不存在或已被删除，无法评价。");
                return;
            }

            lblJobName.Text = SafeText(Convert.ToString(job["job_name"]));
            lblEmpName.Text = SafeText(Convert.ToString(job["emp_name"]));
            lblSalary.Text = SafeText(string.Format("{0:F2} {1}", Convert.ToDecimal(job["salary"]), job["salary_unit"]));
            lblWorkPlace.Text = SafeText(Convert.ToString(job["work_place"]));

            int status = _appBll.GetStatus(CurrentUserId, jobId);
            if (status == 1)
            {
                lblAppStatus.Text = "已录用，可以评价";
                pnlForm.Visible = true;
                return;
            }

            lblAppStatus.Text = status == 0 ? "待处理" : (status == 2 ? "已拒绝" : "未申请");
            ShowError("只有被该岗位录用的学生才能提交评价。");
        }

        /// <summary>
        /// 显示错误提示并隐藏评价表单，仅保留返回按钮。
        /// </summary>
        /// <param name="message">提示文本</param>
        private void ShowError(string message)
        {
            lblMsg.CssClass = "msg-error";
            lblMsg.Text = SafeText(message);
            pnlForm.Visible = false;
            pnlBack.Visible = true;
        }

        /// <summary>
        /// 提交评价：服务端对评分与内容做二次校验，业务规则（是否录用、是否重复评价）
        /// 由 CommentBLL.Submit 统一判定。
        /// </summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMsg.CssClass = string.Empty;
            lblMsg.Text = string.Empty;

            if (string.IsNullOrEmpty(rblScore.SelectedValue))
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = "请选择 1～5 星评分。";
                return;
            }

            string content = txtContent.Text.Trim();
            if (content.Length < 5)
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = "评价内容至少 5 个字符。";
                return;
            }
            if (content.Length > 500)
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = "评价内容不能超过 500 个字符。";
                return;
            }

            int jobId = JobId;
            if (jobId == 0)
            {
                ShowError("缺少岗位参数，请从“我的申请”页面进入评价。");
                return;
            }

            DataTable jobTable = _jobBll.GetJobDetail(jobId);
            DataRow job = (jobTable == null || jobTable.Rows.Count == 0) ? null : jobTable.Rows[0];
            if (job == null)
            {
                ShowError("岗位不存在或已被删除，无法评价。");
                return;
            }

            Comment model = new Comment();
            model.StudentId = CurrentUserId;
            model.JobId = jobId;
            model.EmployerId = Convert.ToInt32(job["employer_id"]);
            model.Score = Convert.ToInt32(rblScore.SelectedValue);
            model.Content = content;
            model.CommentDate = DateTime.Now;
            model.AuditStatus = 0;      // 0 表示待审核

            string message;
            bool ok = _commentBll.Submit(model, out message);

            lblMsg.CssClass = ok ? "msg-success" : "msg-error";
            lblMsg.Text = SafeText(message);

            if (ok)
            {
                pnlForm.Visible = false;
                pnlBack.Visible = true;
                lblTip.Visible = false;
            }
        }
    }
}
