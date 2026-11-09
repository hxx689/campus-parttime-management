using System;
using System.Data;
using System.IO;
using CampusPartTime.BLL;
using CampusPartTime.Common;
using CampusPartTime.Model;

namespace CampusPartTime
{
    /// <summary>
    /// 岗位发布页：商家发布新岗位或修改已有岗位。
    /// 新发布与修改后的岗位审核状态均重置为 0（待审核）。
    /// </summary>
    public partial class JobPublish : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：employer。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "employer" }; }
    }

        private readonly JobBLL _jobBll = new JobBLL();

        /// <summary>岗位图片扩展名白名单。</summary>
        private static readonly string[] AllowedImageExt = { ".jpg", ".jpeg", ".png", ".gif" };

        /// <summary>允许上传的最大字节数（2MB）。</summary>
        private const int MaxImageSize = 2 * 1024 * 1024;

        /// <summary>待修改的岗位编号，0 表示新增。</summary>
        private int EditJobId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["jobId"], out id) ? id : 0;
            }
        }

        /// <summary>
        /// 页面加载：仅商家与管理员可进入；编辑模式回填岗位数据。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (EditJobId > 0)
                {
                    LoadJob(EditJobId);
                    btnSubmit.Text = "保存修改";
                }
            }
        }

        /// <summary>
        /// 编辑模式回填：校验岗位归属，避免商家修改他人岗位。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        private void LoadJob(int jobId)
        {
            Job m = _jobBll.GetJob(jobId);
            if (m == null || m.EmployerId != CurrentUserId)
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = "岗位不存在或无权修改。";
                btnSubmit.Enabled = false;
                return;
            }

            txtJobName.Text = m.JobName;
            ddlJobType.SelectedValue = m.JobType;
            txtSalary.Text = m.Salary.ToString("F2");
            ddlSalaryUnit.SelectedValue = m.SalaryUnit;
            txtWorkPlace.Text = m.WorkPlace;
            txtWorkTime.Text = m.WorkTime;
            txtRecruitNum.Text = m.RecruitNum.ToString();
            txtDeadline.Text = m.Deadline.ToString("yyyy-MM-dd");
            txtJobDesc.Text = m.JobDesc;
        }

        /// <summary>
        /// 提交按钮：前端校验通过后在此做服务端校验并写入数据库。
        /// </summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string err = ValidateInput();
            if (err != null)
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = err;
                return;
            }

            string imagePath = null;
            string uploadErr = SaveImage(out imagePath);
            if (uploadErr != null)
            {
                lblMsg.CssClass = "msg-error";
                lblMsg.Text = uploadErr;
                return;
            }

            Job m = new Job();
            m.JobId = EditJobId;
            m.EmployerId = CurrentUserId;
            m.JobName = txtJobName.Text.Trim();
            m.JobType = ddlJobType.SelectedValue;
            m.Salary = decimal.Parse(txtSalary.Text.Trim());
            m.SalaryUnit = ddlSalaryUnit.SelectedValue;
            m.WorkPlace = txtWorkPlace.Text.Trim();
            m.WorkTime = txtWorkTime.Text.Trim();
            m.RecruitNum = int.Parse(txtRecruitNum.Text.Trim());
            m.Deadline = DateTime.Parse(txtDeadline.Text.Trim());
            m.JobDesc = txtJobDesc.Text.Trim();
            if (!string.IsNullOrEmpty(imagePath))
            {
                m.JobDesc = m.JobDesc + "（岗位图片：" + imagePath + "）";
            }

            string message;
            bool ok = EditJobId > 0 ? _jobBll.Update(m, out message) : _jobBll.Publish(m, out message);

            lblMsg.CssClass = ok ? "msg-success" : "msg-error";
            lblMsg.Text = SafeText(message);

            if (ok)
            {
                btnSubmit.Enabled = false;
            }
        }

        /// <summary>
        /// 服务端表单校验，返回 null 表示通过，否则返回错误提示。
        /// </summary>
        private string ValidateInput()
        {
            if (txtJobName.Text.Trim().Length == 0)
            {
                return "请输入岗位名称。";
            }
            if (ddlJobType.SelectedValue.Length == 0)
            {
                return "请选择岗位类型。";
            }

            decimal salary;
            if (!decimal.TryParse(txtSalary.Text.Trim(), out salary) || salary < 0 || salary > 999999.99m)
            {
                return "薪资必须为 0～999999.99 之间的数字。";
            }

            if (txtWorkPlace.Text.Trim().Length == 0 || txtWorkTime.Text.Trim().Length == 0)
            {
                return "请填写工作地点与工作时段。";
            }

            int num;
            if (!int.TryParse(txtRecruitNum.Text.Trim(), out num) || num < 1 || num > 100)
            {
                return "招聘人数必须为 1～100 之间的整数。";
            }

            DateTime deadline;
            if (!DateTime.TryParse(txtDeadline.Text.Trim(), out deadline))
            {
                return "请选择报名截止日期。";
            }
            if (deadline.Date < DateTime.Today)
            {
                return "报名截止日期不能早于今天。";
            }

            if (txtJobDesc.Text.Trim().Length > 1000)
            {
                return "岗位描述不能超过 1000 个字符。";
            }
            return null;
        }

        /// <summary>
        /// 保存岗位图片。扩展名白名单校验 + 2MB 大小限制，文件存放于 Uploads 目录。
        /// </summary>
        /// <param name="relativePath">输出参数：相对路径，未上传时为 null</param>
        /// <returns>错误提示，null 表示成功</returns>
        private string SaveImage(out string relativePath)
        {
            relativePath = null;

            if (!fuImage.HasFile)
            {
                return null;
            }

            string ext = Path.GetExtension(fuImage.FileName).ToLower();
            bool allowed = false;
            foreach (string item in AllowedImageExt)
            {
                if (item == ext)
                {
                    allowed = true;
                    break;
                }
            }
            if (!allowed)
            {
                return "岗位图片仅支持 jpg / jpeg / png / gif 格式。";
            }
            if (fuImage.PostedFile.ContentLength > MaxImageSize)
            {
                return "岗位图片大小不能超过 2MB。";
            }

            string folder = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "job_" + CurrentUserId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
            fuImage.SaveAs(Path.Combine(folder, fileName));
            relativePath = "Uploads/" + fileName;
            return null;
        }
    }
}
