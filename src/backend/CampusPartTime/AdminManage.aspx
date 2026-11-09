<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminManage.aspx.cs" Inherits="CampusPartTime.AdminManage" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8" />
    <title>后台管理 - 校园兼职招聘管理系统</title>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav">
            <div class="nav-inner">
                <span class="nav-logo">校园兼职招聘管理系统</span>
                <a href="AdminManage.aspx" class="active">后台管理</a>
                <a href="Logout.aspx">退出（<asp:Label ID="lblUser" runat="server"></asp:Label>）</a>
            </div>
        </div>
        <div class="content">
            <div class="panel">
                <h2 class="panel-title">后台管理</h2>
                <asp:Label ID="lblMsg" runat="server"></asp:Label>

                <div class="tabs">
                    <asp:LinkButton ID="tabUsers" runat="server" CssClass="tab" CommandArgument="users"
                        OnClick="tab_Click">用户审核</asp:LinkButton>
                    <asp:LinkButton ID="tabJobs" runat="server" CssClass="tab" CommandArgument="jobs"
                        OnClick="tab_Click">岗位审核</asp:LinkButton>
                    <asp:LinkButton ID="tabComments" runat="server" CssClass="tab" CommandArgument="comments"
                        OnClick="tab_Click">评价审核</asp:LinkButton>
                    <asp:LinkButton ID="tabStats" runat="server" CssClass="tab" CommandArgument="stats"
                        OnClick="tab_Click">数据统计</asp:LinkButton>
                </div>

                <%-- 页签一：用户审核 --%>
                <asp:Panel ID="pnlUsers" runat="server">
                    <h3 class="section-title">学生用户</h3>
                    <asp:Label ID="lblStuTip" runat="server" CssClass="msg-info"
                        Text="禁用后该学生将无法登录系统，已提交的申请记录保留不变。"></asp:Label>
                    <asp:GridView ID="gvStudent" runat="server" CssClass="grid" AutoGenerateColumns="false"
                        EmptyDataText="暂无学生用户。" UseAccessibleHeader="true"
                        OnRowCommand="gvStudent_RowCommand" OnRowDataBound="gvStudent_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="student_id" HeaderText="编号" ItemStyle-Width="60px" />
                            <asp:BoundField DataField="stu_no" HeaderText="学号" ItemStyle-Width="110px" />
                            <asp:BoundField DataField="stu_name" HeaderText="姓名" ItemStyle-Width="90px" />
                            <asp:BoundField DataField="stu_college" HeaderText="学院" ItemStyle-Width="150px" />
                            <asp:BoundField DataField="stu_major" HeaderText="专业" />
                            <asp:BoundField DataField="stu_grade" HeaderText="年级" ItemStyle-Width="70px" />
                            <asp:BoundField DataField="stu_phone" HeaderText="手机号" ItemStyle-Width="120px" />
                            <asp:BoundField DataField="reg_date" HeaderText="注册时间" DataFormatString="{0:yyyy-MM-dd}"
                                HtmlEncode="true" ItemStyle-Width="110px" />
                            <asp:TemplateField HeaderText="状态" ItemStyle-Width="80px">
                                <ItemTemplate><asp:Label ID="lblStuStatus" runat="server"></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnStuStatus" runat="server" CssClass="link-btn"
                                        CommandName="ToggleStudent" CommandArgument='<%# Eval("student_id") + "|" + Eval("stu_status") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <h3 class="section-title">商家用户</h3>
                    <asp:Label ID="lblEmpTip" runat="server" CssClass="msg-info"
                        Text="禁用后该商家将无法登录系统，其已发布岗位不再对学生显示。"></asp:Label>
                    <asp:GridView ID="gvEmployer" runat="server" CssClass="grid" AutoGenerateColumns="false"
                        EmptyDataText="暂无商家用户。" UseAccessibleHeader="true"
                        OnRowCommand="gvEmployer_RowCommand" OnRowDataBound="gvEmployer_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="employer_id" HeaderText="编号" ItemStyle-Width="60px" />
                            <asp:BoundField DataField="emp_name" HeaderText="商家名称" />
                            <asp:BoundField DataField="emp_contact" HeaderText="联系人" ItemStyle-Width="90px" />
                            <asp:BoundField DataField="emp_phone" HeaderText="联系电话" ItemStyle-Width="120px" />
                            <asp:BoundField DataField="emp_address" HeaderText="地址" />
                            <asp:BoundField DataField="reg_date" HeaderText="注册时间" DataFormatString="{0:yyyy-MM-dd}"
                                HtmlEncode="true" ItemStyle-Width="110px" />
                            <asp:TemplateField HeaderText="状态" ItemStyle-Width="80px">
                                <ItemTemplate><asp:Label ID="lblEmpStatus" runat="server"></asp:Label></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEmpStatus" runat="server" CssClass="link-btn"
                                        CommandName="ToggleEmployer" CommandArgument='<%# Eval("employer_id") + "|" + Eval("emp_status") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>

                <%-- 页签二：岗位审核 --%>
                <asp:Panel ID="pnlJobs" runat="server" Visible="false">
                    <h3 class="section-title">待审核岗位</h3>
                    <asp:Label ID="lblJobTip" runat="server" CssClass="msg-info"
                        Text="审核通过的岗位才会出现在学生端的岗位检索结果中；驳回后岗位不予发布。"></asp:Label>
                    <asp:GridView ID="gvJob" runat="server" CssClass="grid" AutoGenerateColumns="false"
                        EmptyDataText="暂无待审核岗位。" UseAccessibleHeader="true"
                        OnRowCommand="gvJob_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="job_id" HeaderText="编号" ItemStyle-Width="60px" />
                            <asp:BoundField DataField="job_name" HeaderText="岗位名称" />
                            <asp:BoundField DataField="job_type" HeaderText="类型" ItemStyle-Width="90px" />
                            <asp:BoundField DataField="emp_name" HeaderText="招聘商家" ItemStyle-Width="160px" />
                            <asp:TemplateField HeaderText="薪资待遇" ItemStyle-Width="120px">
                                <ItemTemplate><%# Eval("salary", "{0:F2}") + " " + Eval("salary_unit") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="work_place" HeaderText="工作地点" ItemStyle-Width="160px" />
                            <asp:BoundField DataField="recruit_num" HeaderText="招聘人数" ItemStyle-Width="80px" />
                            <asp:BoundField DataField="publish_date" HeaderText="发布时间" DataFormatString="{0:yyyy-MM-dd}"
                                HtmlEncode="true" ItemStyle-Width="110px" />
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="110px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnJobPass" runat="server" CssClass="link-btn"
                                        CommandName="PassJob" CommandArgument='<%# Eval("job_id") %>'
                                        OnClientClick="return confirm('确认通过该岗位的发布申请吗？');">通过</asp:LinkButton>
                                    <asp:LinkButton ID="btnJobReject" runat="server" CssClass="link-btn link-danger"
                                        CommandName="RejectJob" CommandArgument='<%# Eval("job_id") %>'
                                        OnClientClick="return confirm('确认驳回该岗位的发布申请吗？');">驳回</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>

                <%-- 页签三：评价审核 --%>
                <asp:Panel ID="pnlComments" runat="server" Visible="false">
                    <h3 class="section-title">待审核评价</h3>
                    <asp:Label ID="lblCommentTip" runat="server" CssClass="msg-info"
                        Text="审核通过的评价才会显示在岗位详情页；驳回后评价不予公开。"></asp:Label>
                    <asp:GridView ID="gvComment" runat="server" CssClass="grid" AutoGenerateColumns="false"
                        EmptyDataText="暂无待审核评价。" UseAccessibleHeader="true"
                        OnRowCommand="gvComment_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="comment_id" HeaderText="编号" ItemStyle-Width="60px" />
                            <asp:BoundField DataField="stu_name" HeaderText="学生姓名" ItemStyle-Width="90px" />
                            <asp:BoundField DataField="job_name" HeaderText="评价岗位" />
                            <asp:BoundField DataField="emp_name" HeaderText="招聘商家" ItemStyle-Width="160px" />
                            <asp:BoundField DataField="score" HeaderText="评分" ItemStyle-Width="60px" />
                            <asp:BoundField DataField="content" HeaderText="评价内容" HtmlEncode="true" />
                            <asp:BoundField DataField="comment_date" HeaderText="提交时间" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                                HtmlEncode="true" ItemStyle-Width="140px" />
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="110px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnCommentPass" runat="server" CssClass="link-btn"
                                        CommandName="PassComment" CommandArgument='<%# Eval("comment_id") %>'
                                        OnClientClick="return confirm('确认通过该评价吗？通过后将在岗位详情页公开显示。');">通过</asp:LinkButton>
                                    <asp:LinkButton ID="btnCommentReject" runat="server" CssClass="link-btn link-danger"
                                        CommandName="RejectComment" CommandArgument='<%# Eval("comment_id") %>'
                                        OnClientClick="return confirm('确认驳回该评价吗？');">驳回</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>

                <%-- 页签四：数据统计 --%>
                <asp:Panel ID="pnlStats" runat="server" Visible="false">
                    <h3 class="section-title">平台数据统计</h3>
                    <div class="stat-grid">
                        <div class="stat-box">
                            <asp:Label ID="lblStuCount" runat="server" CssClass="stat-num" Text="0"></asp:Label>
                            <span class="stat-label">注册学生数</span>
                        </div>
                        <div class="stat-box">
                            <asp:Label ID="lblEmpCount" runat="server" CssClass="stat-num" Text="0"></asp:Label>
                            <span class="stat-label">注册商家数</span>
                        </div>
                        <div class="stat-box">
                            <asp:Label ID="lblJobCount" runat="server" CssClass="stat-num" Text="0"></asp:Label>
                            <span class="stat-label">在招岗位数</span>
                        </div>
                        <div class="stat-box">
                            <asp:Label ID="lblAppCount" runat="server" CssClass="stat-num" Text="0"></asp:Label>
                            <span class="stat-label">申请记录数</span>
                        </div>
                        <div class="stat-box">
                            <asp:Label ID="lblCommentCount" runat="server" CssClass="stat-num" Text="0"></asp:Label>
                            <span class="stat-label">评价总数</span>
                        </div>
                    </div>
                    <p class="form-hint">统计口径：注册数为学生 / 商家表全部记录；在招岗位数指审核通过且岗位状态为“在招”的岗位。</p>
                    <div class="btn-bar">
                        <asp:Button ID="btnRefreshStats" runat="server" Text="刷新统计" CssClass="btn-primary"
                            OnClick="btnRefreshStats_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
