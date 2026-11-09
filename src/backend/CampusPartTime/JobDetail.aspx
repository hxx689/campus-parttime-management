<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobDetail.aspx.cs" Inherits="CampusPartTime.JobDetail" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>岗位详情 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="JobList.aspx">岗位检索</a>
                <a href="MyApplication.aspx">我的申请</a>
                <a href="Logout.aspx">退出</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">
                    <asp:Label ID="lblJobName" runat="server"></asp:Label></h2>
                <table class="detail-table">
                    <tr><th>岗位类型</th><td><asp:Label ID="lblJobType" runat="server"></asp:Label></td></tr>
                    <tr><th>薪资待遇</th><td><asp:Label ID="lblSalary" runat="server"></asp:Label></td></tr>
                    <tr><th>工作地点</th><td><asp:Label ID="lblWorkPlace" runat="server"></asp:Label></td></tr>
                    <tr><th>工作时段</th><td><asp:Label ID="lblWorkTime" runat="server"></asp:Label></td></tr>
                    <tr><th>招聘人数</th><td><asp:Label ID="lblRecruitNum" runat="server"></asp:Label></td></tr>
                    <tr><th>报名截止</th><td><asp:Label ID="lblDeadline" runat="server"></asp:Label></td></tr>
                    <tr><th>岗位状态</th><td><asp:Label ID="lblJobStatus" runat="server"></asp:Label></td></tr>
                    <tr><th>岗位描述</th><td><asp:Label ID="lblJobDesc" runat="server"></asp:Label></td></tr>
                    <tr><th>商家名称</th><td><asp:Label ID="lblEmpName" runat="server"></asp:Label></td></tr>
                    <tr><th>联系人</th><td><asp:Label ID="lblEmpContact" runat="server"></asp:Label></td></tr>
                    <tr><th>联系电话</th><td><asp:Label ID="lblEmpPhone" runat="server"></asp:Label></td></tr>
                </table>

                <div class="btn-bar">
                    <asp:Button ID="btnApply" runat="server" Text="申请该岗位" CssClass="btn-primary"
                        OnClientClick="return confirm('确认申请该岗位吗？');" OnClick="btnApply_Click" />
                    <asp:Button ID="btnToComment" runat="server" Text="评价该岗位" CssClass="btn-ghost" OnClick="btnToComment_Click" />
                    <a href="JobList.aspx" class="btn-ghost">返回列表</a>
                </div>
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>

                <h3 class="section-title">岗位评价</h3>
                <asp:GridView ID="gvComments" runat="server" CssClass="grid" AutoGenerateColumns="false"
                    EmptyDataText="该岗位暂无评价。" UseAccessibleHeader="true">
                    <Columns>
                        <asp:BoundField DataField="stu_name" HeaderText="评价学生" />
                        <asp:BoundField DataField="score" HeaderText="评分" />
                        <asp:BoundField DataField="content" HeaderText="评价内容" />
                        <asp:BoundField DataField="comment_date" HeaderText="评价日期" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
