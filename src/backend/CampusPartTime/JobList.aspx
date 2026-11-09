<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobList.aspx.cs" Inherits="CampusPartTime.JobList" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>岗位检索 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="JobList.aspx" class="active">岗位检索</a>
                <a href="MyApplication.aspx">我的申请</a>
                <a href="Logout.aspx">退出（<asp:Label ID="lblUser" runat="server"></asp:Label>）</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">岗位检索</h2>
                <div class="search-bar">
                    <asp:DropDownList ID="ddlJobType" runat="server" CssClass="input-select">
                        <asp:ListItem Text="全部类型" Value="" />
                        <asp:ListItem Text="家教" Value="家教" />
                        <asp:ListItem Text="餐饮" Value="餐饮" />
                        <asp:ListItem Text="促销" Value="促销" />
                        <asp:ListItem Text="文员" Value="文员" />
                        <asp:ListItem Text="其他" Value="其他" />
                    </asp:DropDownList>
                    <asp:TextBox ID="txtSalary" runat="server" CssClass="input-text input-short" placeholder="最低薪资"></asp:TextBox>
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="input-text" placeholder="岗位名称关键词"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" CssClass="btn-primary"
                        OnClientClick="return checkSalaryInput();" OnClick="btnSearch_Click" />
                </div>

                <asp:GridView ID="gvJobList" runat="server" CssClass="grid" AutoGenerateColumns="false"
                    EmptyDataText="暂无符合条件的岗位。" UseAccessibleHeader="true">
                    <Columns>
                        <asp:BoundField DataField="job_name" HeaderText="岗位名称" />
                        <asp:BoundField DataField="job_type" HeaderText="类型" />
                        <asp:TemplateField HeaderText="薪资">
                            <ItemTemplate><%# Eval("salary", "{0:F2}") %> 元/<%# Eval("salary_unit") %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="work_place" HeaderText="工作地点" />
                        <asp:BoundField DataField="work_time" HeaderText="工作时段" />
                        <asp:BoundField DataField="emp_name" HeaderText="商家" />
                        <asp:BoundField DataField="recruit_num" HeaderText="招聘人数" />
                        <asp:BoundField DataField="publish_date" HeaderText="发布日期" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:HyperLinkField Text="查看详情" DataNavigateUrlFormatString="JobDetail.aspx?id={0}"
                            DataNavigateUrlFields="job_id" />
                    </Columns>
                </asp:GridView>

                <div class="pager">
                    <asp:Button ID="btnPrev" runat="server" Text="上一页" CssClass="btn-ghost" OnClick="btnPrev_Click" />
                    <asp:Label ID="lblPageInfo" runat="server" CssClass="pager-info"></asp:Label>
                    <asp:Button ID="btnNext" runat="server" Text="下一页" CssClass="btn-ghost" OnClick="btnNext_Click" />
                </div>
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
