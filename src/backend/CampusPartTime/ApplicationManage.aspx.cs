using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 申请处理页：商家查看本人岗位收到的学生申请，逐条执行录用或拒绝。
    /// 录用调用 ApplicationBLL.Hire（底层存储过程 proc_hire_applicant，同时把岗位已录用人数加 1）；
    /// 拒绝调用 ApplicationBLL.Reject（底层存储过程 proc_reject_applicant）。
    /// </summary>
    public partial class ApplicationManage : PageBase
    {
    /// <summary>
    /// 允许访问本页的角色：employer。
    /// </summary>
    protected override string[] AllowRoles
    {
        get { return new string[] { "employer" }; }
    }

        private readonly ApplicationBLL _appBll = new ApplicationBLL();

        // 申请记录视图不包含学生学院字段，复用用户查询接口一次性取出学生基础信息，
        // 建立“学号 → 学院”映射，避免逐行查询数据库。
        private readonly AdminBLL _adminBll = new AdminBLL();

        /// <summary>
        /// 页面加载：仅商家可进入；首次进入绑定申请列表。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindAppList();
            }
        }

        /// <summary>
        /// 绑定当前商家收到的申请列表，并按状态下拉框过滤。
        /// </summary>
        private void BindAppList()
        {
            DataTable dt = _appBll.GetByEmployer(CurrentUserId);
            FillCollege(dt);

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
            }

            lblSummary.Text = string.Format("共 {0} 条申请，待处理 {1} 条，已录用 {2} 条。", dt.Rows.Count, pending, hired);
        }

        /// <summary>
        /// 为申请列表补全学生学院列。
        /// </summary>
        /// <param name="dt">申请记录数据表</param>
        private void FillCollege(DataTable dt)
        {
            if (dt.Columns["stu_college"] == null)
            {
                dt.Columns.Add("stu_college", typeof(string));
            }

            if (dt.Rows.Count == 0)
            {
                return;
            }

            Dictionary<string, string> map = new Dictionary<string, string>();
            DataTable students = _adminBll.GetAllStudents();
            foreach (DataRow row in students.Rows)
            {
                string stuNo = Convert.ToString(row["stu_no"]);
                if (!string.IsNullOrEmpty(stuNo) && !map.ContainsKey(stuNo))
                {
                    map.Add(stuNo, Convert.ToString(row["stu_college"]));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string stuNo = Convert.ToString(row["stu_no"]);
                string college;
                row["stu_college"] = map.TryGetValue(stuNo, out college) ? college : "—";
            }
        }

        /// <summary>
        /// 行数据绑定：把申请状态数值翻译为中文，并按状态隐藏不可用操作按钮。
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
            int recruitNum = Convert.ToInt32(row["recruit_num"]);
            int hiredNum = Convert.ToInt32(row["hired_num"]);

            Label lblAppStatus = (Label)e.Row.FindControl("lblAppStatus");
            lblAppStatus.Text = appStatus == 0 ? "待处理" : (appStatus == 1 ? "已录用" : "已拒绝");

            LinkButton btnHire = (LinkButton)e.Row.FindControl("btnHire");
            LinkButton btnReject = (LinkButton)e.Row.FindControl("btnReject");

            // 只有待处理的申请才能操作；岗位已招满时不再提供录用按钮
            if (appStatus != 0)
            {
                btnHire.Visible = false;
                btnReject.Visible = false;
                return;
            }

            if (hiredNum >= recruitNum)
            {
                btnHire.Visible = false;
            }
        }

        /// <summary>
        /// 行命令处理：录用或拒绝申请。业务校验（是否本人岗位、是否已处理、是否招满）
        /// 统一在 ApplicationBLL 中完成，页面只负责展示结果。
        /// </summary>
        protected void gvApp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int appId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out appId))
            {
                return;
            }

            string message;
            bool ok;

            if (e.CommandName == "HireApp")
            {
                ok = _appBll.Hire(appId, CurrentUserId, out message);
            }
            else if (e.CommandName == "RejectApp")
            {
                ok = _appBll.Reject(appId, CurrentUserId, out message);
            }
            else
            {
                return;
            }

            if (ok && e.CommandName == "HireApp")
            {
                message += " 录用结果已同步更新岗位已录用人数。";
            }

            lblMsg.CssClass = ok ? "msg-success" : "msg-error";
            lblMsg.Text = SafeText(message);

            // 无论成功与否都重新绑定：成功时刷新状态，失败时修正可能已过期的列表
            BindAppList();
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
