<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CampusPartTime.Login" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>登录 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body class="page-login">
    <form id="form1" runat="server">
        <div class="login-container">
            <h2>校园兼职招聘管理系统</h2>
            <div class="login-box">
                <div class="form-row">
                    <label for="txtUser">账号</label>
                    <asp:TextBox ID="txtUser" runat="server" CssClass="input-text" MaxLength="30" placeholder="学生填学号，商家填联系电话"></asp:TextBox>
                </div>
                <div class="form-row">
                    <label for="txtPwd">密码</label>
                    <asp:TextBox ID="txtPwd" runat="server" CssClass="input-text" TextMode="Password" MaxLength="30"></asp:TextBox>
                </div>
                <div class="form-row">
                    <label for="ddlRole">用户类型</label>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-select">
                        <asp:ListItem Text="学生" Value="student" />
                        <asp:ListItem Text="商家" Value="employer" />
                        <asp:ListItem Text="管理员" Value="admin" />
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnLogin" runat="server" Text="登 录" CssClass="btn-primary btn-block"
                    OnClientClick="return checkLoginForm();" OnClick="btnLogin_Click" />
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>
                <p class="login-tip">
                    还没有账号？<a href="StudentRegister.aspx">学生注册</a> ｜ <a href="EmployerRegister.aspx">商家注册</a>
                </p>
            </div>
        </div>
    </form>
</body>
</html>
