<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicationManage.aspx.cs" Inherits="CampusPartTime.ApplicationManage" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>申请处理 - 校园兼职招聘管理系统</title>
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
                <a href="ApplicationManage.aspx" class="active">申请处理</a>
                <a href="Logout.aspx">退出</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">申请处理</h2>
                <asp:Label ID="lblTip" runat="server" CssClass="msg-info"
                    Text="录用后岗位已录用人数自动加 1，达到招聘人数时岗位转为“已招满”，不再接受录用操作。"></asp:Label>
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
                    EmptyDataText="暂无学生申请记录。" UseAccessibleHeader="true"
                    OnRowCommand="gvApp_RowCommand" OnRowDataBound="gvApp_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="app_id" HeaderText="编号" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="job_name" HeaderText="应聘岗位" />
                        <asp:BoundField DataField="stu_name" HeaderText="学生姓名" ItemStyle-Width="90px" />
                        <asp:BoundField DataField="stu_college" HeaderText="所在学院" ItemStyle-Width="150px" />
                        <asp:BoundField DataField="stu_no" HeaderText="学号" ItemStyle-Width="110px" />
                        <asp:BoundField DataField="stu_phone" HeaderText="联系电话" ItemStyle-Width="120px" />
                        <asp:BoundField DataField="app_date" HeaderText="申请时间" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                            HtmlEncode="true" ItemStyle-Width="140px" />
                        <asp:TemplateField HeaderText="申请状态" ItemStyle-Width="90px">
                            <ItemTemplate><asp:Label ID="lblAppStatus" runat="server"></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="录用进度" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <%# Eval("hired_num") %> / <%# Eval("recruit_num") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作" ItemStyle-Width="130px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnHire" runat="server" CssClass="link-btn"
                                    CommandName="HireApp" CommandArgument='<%# Eval("app_id") %>'
                                    OnClientClick="return confirm('确认录用该学生吗？录用后不可撤销。');">录用</asp:LinkButton>
                                <asp:LinkButton ID="btnReject" runat="server" CssClass="link-btn link-danger"
                                    CommandName="RejectApp" CommandArgument='<%# Eval("app_id") %>'
                                    OnClientClick="return confirm('确认拒绝该申请吗？拒绝后学生将看到“已拒绝”状态。');">拒绝</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
