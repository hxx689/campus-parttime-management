<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyApplication.aspx.cs" Inherits="CampusPartTime.MyApplication" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>我的申请 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="JobList.aspx">岗位检索</a>
                <a href="MyApplication.aspx" class="active">我的申请</a>
                <a href="Logout.aspx">退出（<asp:Label ID="lblUser" runat="server"></asp:Label>）</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">我的申请</h2>
                <asp:Label ID="lblTip" runat="server" CssClass="msg-info"
                    Text="申请状态分为待处理、已录用、已拒绝；被录用后可对岗位提交评价，评价经管理员审核后公开展示。"></asp:Label>
                <asp:Label ID="lblMsg" runat="server"></asp:Label>

                <div class="filter-bar">
                    <label for="<%= ddlStatus.ClientID %>">申请状态</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="input-select input-short">
                        <asp:ListItem Text="全部状态" Value="" />
                        <asp:ListItem Text="待处理" Value="0" />
                        <asp:ListItem Text="已录用" Value="1" />
                        <asp:ListItem Text="已拒绝" Value="2" />
                    </asp:DropDownList>
                    <asp:Button ID="btnFilter" runat="server" Text="筛 选" CssClass="btn-primary"
                        OnClick="btnFilter_Click" />
                    <asp:Label ID="lblSummary" runat="server" CssClass="form-hint"></asp:Label>
                </div>

                <asp:GridView ID="gvApp" runat="server" CssClass="grid" AutoGenerateColumns="false"
                    EmptyDataText="您还没有提交过岗位申请，可前往“岗位检索”页面申请心仪岗位。" UseAccessibleHeader="true"
                    OnRowCommand="gvApp_RowCommand" OnRowDataBound="gvApp_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="app_id" HeaderText="编号" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="job_name" HeaderText="应聘岗位" />
                        <asp:BoundField DataField="job_type" HeaderText="类型" ItemStyle-Width="90px" />
                        <asp:BoundField DataField="emp_name" HeaderText="招聘商家" ItemStyle-Width="160px" />
                        <asp:TemplateField HeaderText="薪资待遇" ItemStyle-Width="120px">
                            <ItemTemplate><%# Eval("salary", "{0:F2}") + " " + Eval("salary_unit") %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="app_date" HeaderText="申请时间" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                            HtmlEncode="true" ItemStyle-Width="140px" />
                        <asp:TemplateField HeaderText="申请状态" ItemStyle-Width="90px">
                            <ItemTemplate><asp:Label ID="lblAppStatus" runat="server"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="岗位状态" ItemStyle-Width="90px">
                            <ItemTemplate><asp:Label ID="lblJobStatus" runat="server"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDetail" runat="server" CssClass="link-btn"
                                    CommandName="ViewJob" CommandArgument='<%# Eval("job_id") %>'>岗位详情</asp:LinkButton>
                                <asp:LinkButton ID="btnComment" runat="server" CssClass="link-btn"
                                    CommandName="WriteComment" CommandArgument='<%# Eval("job_id") %>'
                                    Visible="false">去评价</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
