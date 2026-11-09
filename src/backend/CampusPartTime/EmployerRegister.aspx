<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmployerRegister.aspx.cs" Inherits="CampusPartTime.EmployerRegister" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>商家注册 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="Login.aspx">返回登录</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">商家注册</h2>
                <div class="form-grid">
                    <div class="form-row"><label>商家名称 <em>*</em></label>
                        <asp:TextBox ID="txtEmpName" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></div>
                    <div class="form-row"><label>联系人 <em>*</em></label>
                        <asp:TextBox ID="txtContact" runat="server" CssClass="input-text" MaxLength="20"></asp:TextBox></div>
                    <div class="form-row"><label>联系电话 <em>*</em></label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="input-text" MaxLength="11" placeholder="作为登录账号"></asp:TextBox></div>
                    <div class="form-row"><label>地址</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="input-text" MaxLength="100"></asp:TextBox></div>
                    <div class="form-row"><label>邮箱</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></div>
                    <div class="form-row form-row-wide"><label>商家简介</label>
                        <asp:TextBox ID="txtIntro" runat="server" CssClass="input-text input-area" TextMode="MultiLine" Rows="4" MaxLength="500"></asp:TextBox></div>
                    <div class="form-row"><label>密码 <em>*</em></label>
                        <asp:TextBox ID="txtPwd" runat="server" CssClass="input-text" TextMode="Password" MaxLength="30"></asp:TextBox></div>
                    <div class="form-row"><label>确认密码 <em>*</em></label>
                        <asp:TextBox ID="txtPwd2" runat="server" CssClass="input-text" TextMode="Password" MaxLength="30"></asp:TextBox></div>
                    <div class="form-row"><label>营业执照（可选）</label>
                        <asp:FileUpload ID="fuLicense" runat="server" CssClass="input-file" />
                        <span class="field-tip">仅支持 jpg / jpeg / png，不超过 2MB</span></div>
                </div>
                <asp:CheckBox ID="chkAgree" runat="server" Text="我已阅读并同意平台发布规范，承诺岗位信息真实有效" />
                <asp:Button ID="btnRegister" runat="server" Text="提交注册" CssClass="btn-primary"
                    OnClientClick="return checkEmployerRegisterForm();" OnClick="btnRegister_Click" />
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
