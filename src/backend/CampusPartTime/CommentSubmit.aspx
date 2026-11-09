<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommentSubmit.aspx.cs" Inherits="CampusPartTime.CommentSubmit" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>岗位评价 - 校园兼职招聘管理系统</title>
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
                <h2 class="panel-title">岗位评价</h2>
                <asp:Label ID="lblMsg" runat="server"></asp:Label>

                <table class="detail-table">
                    <tr>
                        <th>岗位名称</th>
                        <td><asp:Label ID="lblJobName" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <th>招聘商家</th>
                        <td><asp:Label ID="lblEmpName" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <th>薪资待遇</th>
                        <td><asp:Label ID="lblSalary" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <th>工作地点</th>
                        <td><asp:Label ID="lblWorkPlace" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <th>我的申请状态</th>
                        <td><asp:Label ID="lblAppStatus" runat="server"></asp:Label></td>
                    </tr>
                </table>

                <asp:Panel ID="pnlForm" runat="server" Visible="false">
                    <h3 class="section-title">填写评价</h3>
                    <asp:Label ID="lblTip" runat="server" CssClass="msg-info"
                        Text="评价提交后进入待审核状态，管理员审核通过后才会在岗位详情页公开展示。"></asp:Label>
                    <table class="form-table">
                        <tr>
                            <th><span class="required">*</span>综合评分</th>
                            <td>
                                <asp:RadioButtonList ID="rblScore" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow" CssClass="star-list">
                                    <asp:ListItem Text="1 星" Value="1" />
                                    <asp:ListItem Text="2 星" Value="2" />
                                    <asp:ListItem Text="3 星" Value="3" />
                                    <asp:ListItem Text="4 星" Value="4" />
                                    <asp:ListItem Text="5 星" Value="5" />
                                </asp:RadioButtonList>
                                <span class="form-hint">1 星最差，5 星最好</span>
                            </td>
                        </tr>
                        <tr>
                            <th><span class="required">*</span>评价内容</th>
                            <td>
                                <asp:TextBox ID="txtContent" runat="server" CssClass="input-textarea input-area"
                                    TextMode="MultiLine" Rows="5" MaxLength="500"
                                    placeholder="请描述工作内容、工作环境、薪资发放等实际情况，5～500 字"></asp:TextBox>
                                <span class="form-hint">5～500 字，请勿填写联系方式等隐私信息</span>
                            </td>
                        </tr>
                    </table>
                    <div class="btn-bar">
                        <asp:Button ID="btnSubmit" runat="server" Text="提交评价" CssClass="btn-primary"
                            OnClientClick="return checkCommentForm();" OnClick="btnSubmit_Click" />
                        <a href="MyApplication.aspx" class="btn-ghost">返回我的申请</a>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlBack" runat="server" CssClass="btn-bar" Visible="false">
                    <a href="MyApplication.aspx" class="btn-ghost">返回我的申请</a>
                    <a href="JobList.aspx" class="btn-ghost">继续浏览岗位</a>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
