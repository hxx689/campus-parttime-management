<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentRegister.aspx.cs" Inherits="CampusPartTime.StudentRegister" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>学生注册 - 校园兼职招聘管理系统</title>
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
                <h2 class="panel-title">学生注册</h2>
                <div class="form-grid">
                    <div class="form-row"><label>学号 <em>*</em></label>
                        <asp:TextBox ID="txtStuNo" runat="server" CssClass="input-text" MaxLength="20"></asp:TextBox></div>
                    <div class="form-row"><label>姓名 <em>*</em></label>
                        <asp:TextBox ID="txtStuName" runat="server" CssClass="input-text" MaxLength="20"></asp:TextBox></div>
                    <div class="form-row"><label>性别 <em>*</em></label>
                        <asp:RadioButton ID="rdoMale" runat="server" GroupName="sex" Text="男" Checked="true" />
                        <asp:RadioButton ID="rdoFemale" runat="server" GroupName="sex" Text="女" /></div>
                    <div class="form-row"><label>出生日期</label>
                        <asp:TextBox ID="txtBirth" runat="server" CssClass="input-text" TextMode="Date"></asp:TextBox></div>
                    <div class="form-row"><label>学院 <em>*</em></label>
                        <asp:TextBox ID="txtCollege" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></div>
                    <div class="form-row"><label>专业 <em>*</em></label>
                        <asp:TextBox ID="txtMajor" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></div>
                    <div class="form-row"><label>年级 <em>*</em></label>
                        <asp:TextBox ID="txtGrade" runat="server" CssClass="input-text" MaxLength="10" placeholder="如 2023级"></asp:TextBox></div>
                    <div class="form-row"><label>联系电话 <em>*</em></label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="input-text" MaxLength="11"></asp:TextBox></div>
                    <div class="form-row"><label>邮箱</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input-text" MaxLength="50"></asp:TextBox></div>
                    <div class="form-row"><label>密码 <em>*</em></label>
                        <asp:TextBox ID="txtPwd" runat="server" CssClass="input-text" TextMode="Password" MaxLength="30"></asp:TextBox></div>
                    <div class="form-row"><label>确认密码 <em>*</em></label>
                        <asp:TextBox ID="txtPwd2" runat="server" CssClass="input-text" TextMode="Password" MaxLength="30"></asp:TextBox></div>
                    <div class="form-row"><label>头像（可选）</label>
                        <asp:FileUpload ID="fuAvatar" runat="server" CssClass="input-file" />
                        <span class="field-tip">仅支持 jpg / jpeg / png，不超过 2MB</span></div>
                </div>
                <asp:Button ID="btnRegister" runat="server" Text="提交注册" CssClass="btn-primary"
                    OnClientClick="return checkStudentRegisterForm();" OnClick="btnRegister_Click" />
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-error"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
