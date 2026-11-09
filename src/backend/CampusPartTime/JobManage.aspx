<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobManage.aspx.cs" Inherits="CampusPartTime.JobManage" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>岗位管理 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="JobPublish.aspx">发布岗位</a>
                <a href="JobManage.aspx">岗位管理</a>
                <a href="ApplicationManage.aspx">申请处理</a>
                <a href="Logout.aspx">退出</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">岗位管理</h2>
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-success"></asp:Label>
                <asp:GridView ID="gvJob" runat="server" CssClass="grid" AutoGenerateColumns="false"
                    EmptyDataText="暂无岗位，请先发布岗位。" UseAccessibleHeader="true"
                    OnRowCommand="gvJob_RowCommand" OnRowDataBound="gvJob_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="job_id" HeaderText="编号" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="job_name" HeaderText="岗位名称" />
                        <asp:BoundField DataField="job_type" HeaderText="类型" ItemStyle-Width="90px" />
                        <asp:BoundField DataField="salary" HeaderText="薪资" DataFormatString="{0:F2}" ItemStyle-Width="90px" />
                        <asp:BoundField DataField="salary_unit" HeaderText="计薪单位" ItemStyle-Width="90px" />
                        <asp:BoundField DataField="work_place" HeaderText="工作地点" />
                        <asp:BoundField DataField="recruit_num" HeaderText="招聘" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="hired_num" HeaderText="已录用" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="deadline" HeaderText="报名截止" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="100px" />
                        <asp:TemplateField HeaderText="审核状态" ItemStyle-Width="90px">
                            <ItemTemplate><asp:Label ID="lblAudit" runat="server"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="岗位状态" ItemStyle-Width="90px">
                            <ItemTemplate><asp:Label ID="lblStatus" runat="server"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="link-btn"
                                    CommandName="EditJob" CommandArgument='<%# Eval("job_id") %>'>修改</asp:LinkButton>
                                <asp:LinkButton ID="btnOffline" runat="server" CssClass="link-btn link-danger"
                                    CommandName="OfflineJob" CommandArgument='<%# Eval("job_id") %>'
                                    OnClientClick="return confirm('确认下架该岗位吗？下架后学生将无法申请。');">下架</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
