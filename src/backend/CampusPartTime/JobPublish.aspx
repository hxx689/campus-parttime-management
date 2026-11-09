<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JobPublish.aspx.cs" Inherits="CampusPartTime.JobPublish" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>发布岗位 - 校园兼职招聘管理系统</title>
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
                <h2 class="panel-title">发布 / 修改岗位</h2>
                <asp:Label ID="lblTip" runat="server" CssClass="msg-info"
                    Text="新发布或修改后的岗位均需管理员审核，审核通过后才会在岗位列表中展示。"></asp:Label>
                <table class="form-table">
                    <tr><th><span class="required">*</span>岗位名称</th>
                        <td><asp:TextBox ID="txtJobName" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></td></tr>
                    <tr><th><span class="required">*</span>岗位类型</th>
                        <td><asp:DropDownList ID="ddlJobType" runat="server" CssClass="input-select">
                                <asp:ListItem Text="请选择" Value="" />
                                <asp:ListItem Text="家教" Value="家教" />
                                <asp:ListItem Text="餐饮服务" Value="餐饮服务" />
                                <asp:ListItem Text="促销推广" Value="促销推广" />
                                <asp:ListItem Text="行政助理" Value="行政助理" />
                                <asp:ListItem Text="其他" Value="其他" />
                            </asp:DropDownList></td></tr>
                    <tr><th><span class="required">*</span>薪资待遇</th>
                        <td><asp:TextBox ID="txtSalary" runat="server" CssClass="input-text input-short"></asp:TextBox>
                            <asp:DropDownList ID="ddlSalaryUnit" runat="server" CssClass="input-select input-short">
                                <asp:ListItem Text="元/小时" Value="元/小时" />
                                <asp:ListItem Text="元/天" Value="元/天" />
                            </asp:DropDownList></td></tr>
                    <tr><th><span class="required">*</span>工作地点</th>
                        <td><asp:TextBox ID="txtWorkPlace" runat="server" CssClass="input-text" MaxLength="100"></asp:TextBox></td></tr>
                    <tr><th><span class="required">*</span>工作时段</th>
                        <td><asp:TextBox ID="txtWorkTime" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></td></tr>
                    <tr><th><span class="required">*</span>招聘人数</th>
                        <td><asp:TextBox ID="txtRecruitNum" runat="server" CssClass="input-text input-short" Text="1"></asp:TextBox>
                            <span class="form-hint">可填 1～100</span></td></tr>
                    <tr><th><span class="required">*</span>报名截止</th>
                        <td><asp:TextBox ID="txtDeadline" runat="server" CssClass="input-text input-short" TextMode="Date"></asp:TextBox></td></tr>
                    <tr><th>岗位描述</th>
                        <td><asp:TextBox ID="txtJobDesc" runat="server" CssClass="input-textarea" TextMode="MultiLine" Rows="5" MaxLength="1000"></asp:TextBox></td></tr>
                    <tr><th>岗位图片</th>
                        <td><asp:FileUpload ID="fuImage" runat="server" CssClass="input-file" />
                            <span class="form-hint">可选，仅支持 jpg / jpeg / png / gif，大小不超过 2MB</span></td></tr>
                </table>
                <div class="btn-bar">
                    <asp:Button ID="btnSubmit" runat="server" Text="提交" CssClass="btn-primary"
                        OnClientClick="return checkJobForm();" OnClick="btnSubmit_Click" />
                    <a href="JobManage.aspx" class="btn-ghost">返回岗位管理</a>
                </div>
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
