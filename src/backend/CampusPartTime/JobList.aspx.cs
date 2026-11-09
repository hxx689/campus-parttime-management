using System;
using System.Data;
using System.Web.UI.WebControls;
using CampusPartTime.BLL;
using CampusPartTime.Common;

namespace CampusPartTime
{
    /// <summary>
    /// 岗位检索页：按类型、最低薪资、名称关键词组合检索，使用 OFFSET-FETCH 服务器端分页。
    /// </summary>
    public partial class JobList : PageBase
    {
        protected System.Web.UI.WebControls.Label lblUser;
        protected System.Web.UI.WebControls.DropDownList ddlJobType;
        protected System.Web.UI.WebControls.TextBox txtSalary;
        protected System.Web.UI.WebControls.TextBox txtKeyword;
        protected System.Web.UI.WebControls.Button btnSearch;
        protected System.Web.UI.WebControls.GridView gvJobList;
        protected System.Web.UI.WebControls.Button btnPrev;
        protected System.Web.UI.WebControls.Button btnNext;
        protected System.Web.UI.WebControls.Label lblPageInfo;
        protected System.Web.UI.WebControls.Label lblMsg;

        private readonly JobBLL _jobBll = new JobBLL();

        /// <summary>每页行数</summary>
        private const int PageSize = 10;

        /// <summary>允许访问的角色</summary>
        protected override string[] AllowRoles
        {
            get { return new[] { "student", "employer", "admin" }; }
        }

        /// <summary>当前页码，保存在视图状态中</summary>
        private int PageIndex
        {
            get { return ViewState["PageIndex"] == null ? 1 : (int)ViewState["PageIndex"]; }
            set { ViewState["PageIndex"] = value; }
        }

        /// <summary>
        /// 页面加载：首次进入绑定第一页数据。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            lblUser.Text = Server.HtmlEncode(Convert.ToString(Session["UserName"]));
            if (!IsPostBack)
            {
                PageIndex = 1;
                BindGrid();
            }
        }

        /// <summary>
        /// 查询按钮事件：条件变更后回到第一页重新检索。
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            PageIndex = 1;
            BindGrid();
        }

        /// <summary>
        /// 上一页。
        /// </summary>
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                BindGrid();
            }
        }

        /// <summary>
        /// 下一页。
        /// </summary>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            PageIndex++;
            int total;
            DataTable dt = Search(out total);
            if (dt.Rows.Count == 0 && PageIndex > 1)
            {
                PageIndex--;
                BindGrid();
                return;
            }
            BindGrid();
        }

        /// <summary>
        /// 按当前查询条件取数据。
        /// </summary>
        /// <param name="totalCount">输出参数：符合条件的总记录数</param>
        /// <returns>当前页数据表</returns>
        private DataTable Search(out int totalCount)
        {
            decimal salary;
            decimal? minSalary = decimal.TryParse(txtSalary.Text.Trim(), out salary) ? salary : (decimal?)null;
            return _jobBll.SearchJobs(ddlJobType.SelectedValue, minSalary, txtKeyword.Text.Trim(),
                PageIndex, PageSize, out totalCount);
        }

        /// <summary>
        /// 绑定当前页数据并刷新分页信息与按钮状态。
        /// </summary>
        private void BindGrid()
        {
            int totalCount;
            DataTable dt = Search(out totalCount);
            gvJobList.DataSource = dt;
            gvJobList.DataBind();

            int pageCount = totalCount == 0 ? 1 : (totalCount + PageSize - 1) / PageSize;
            lblPageInfo.Text = string.Format("第 {0} / {1} 页，共 {2} 条", PageIndex, pageCount, totalCount);
            btnPrev.Enabled = PageIndex > 1;
            btnNext.Enabled = PageIndex < pageCount;
        }
    }
}
