---
AIGC:
    Label: "1"
    ContentProducer: 001191440300708461136T1XGW3
    ProduceID: 47343d30e1a510aa6bac8eb1640e8c5c_b6beb0c0dc0111f1a0ca525400de85a5
    ReservedCode1: onGTn0uqwqnAN3tF1iiSUniChkKWXEQSouTLM5L/ZHdfx5Duao9V8zrUwdvxYu9qwMtfM3cB/zEv1Zn7HbsUKiF7bkgKMB5NzpMMqoGUbvvxxIR86TVyBZf0umsCXRRYuSP4KBV5iOkXYa+gILr1WhDjk64ryNaWos56GIYCf1754z8mB3rknNSt9Is=
    ContentPropagator: 001191440300708461136T1XGW3
    PropagateID: 47343d30e1a510aa6bac8eb1640e8c5c_b6beb0c0dc0111f1a0ca525400de85a5
    ReservedCode2: onGTn0uqwqnAN3tF1iiSUniChkKWXEQSouTLM5L/ZHdfx5Duao9V8zrUwdvxYu9qwMtfM3cB/zEv1Zn7HbsUKiF7bkgKMB5NzpMMqoGUbvvxxIR86TVyBZf0umsCXRRYuSP4KBV5iOkXYa+gILr1WhDjk64ryNaWos56GIYCf1754z8mB3rknNSt9Is=
---

# 校园兼职招聘管理系统 —— 运行说明

> 课程：互联网应用服务开发与数据库实践
> 题目：校园兼职招聘管理系统
> 学号 / 姓名：241240048 / 黄潇萱
> 技术栈：ASP.NET Web 窗体（.NET Framework 4.7.2）+ ADO.NET 三层架构 + SQL Server 2019

---

## 一、交付包结构

```
241240048黄潇萱/
├── 程序源代码/
│   ├── CampusPartTime.sln                 Visual Studio 解决方案文件
│   └── CampusPartTime/                    Web 应用程序项目
│       ├── Login.aspx / StudentRegister.aspx / EmployerRegister.aspx
│       ├── JobList.aspx / JobDetail.aspx
│       ├── JobPublish.aspx / JobManage.aspx / ApplicationManage.aspx
│       ├── MyApplication.aspx / CommentSubmit.aspx
│       ├── AdminManage.aspx / Logout.aspx
│       ├── BLL/       业务逻辑层（UserBLL、JobBLL、ApplicationBLL、CommentBLL、AdminBLL）
│       ├── DAL/       数据访问层（SqlHelper 与 6 张表 DAL）
│       ├── Model/     实体层（Student、Employer、Admin、Job、ApplicationRecord、Comment）
│       ├── Common/    PageBase 基类（角色控制、公共输出编码）
│       ├── Styles/style.css              全局样式表（遵循设计文档 1.4 UI 规范）
│       ├── Scripts/common.js             客户端表单校验脚本
│       ├── Uploads/                      岗位图片上传目录
│       ├── Web.config                    连接串、Forms 认证、上传限制配置
│       └── CampusPartTime.csproj
├── 数据库文件/
│   └── CampusPartTime.sql                数据库完整创建脚本（9 段，678 行）
├── 系统设计报告/
│   └── 241240048黄潇萱_系统设计文档.doc  系统设计文档（22 页）
└── 运行说明/
    ├── README.md                         本文档
    └── 重置测试账号口令.sql              测试账号口令统一重置辅助脚本（可选执行）
```

---

## 二、运行环境要求

| 项目 | 要求 |
|---|---|
| 操作系统 | Windows 10 / Windows 11 64 位（Windows Server 2016 及以上亦可） |
| Web 服务器 | IIS 10（需安装「ASP.NET 4.x」与「.NET 扩展性」功能） |
| 数据库 | Microsoft SQL Server 2019（含 SSMS 18 及以上管理工具） |
| 运行框架 | .NET Framework 4.7.2（含目标包与开发工具） |
| 开发工具 | Visual Studio 2019（工作负载：ASP.NET 和 Web 开发） |
| 第三方组件 | BCrypt.Net-Next 4.0.3（口令强哈希，NuGet 还原） |
| 浏览器 | Chrome / Firefox / Edge，分辨率 1366×768 及以上 |

---

## 三、数据库还原步骤

1. 启动 SQL Server 服务，使用 SSMS 以具备建库权限的账号（如 sa 或实例管理员）连接实例。
2. 「文件 → 打开 → 文件」，选择 `数据库文件\CampusPartTime.sql`，确认工具栏当前数据库为非限制状态后按 **F5** 整体执行。
3. 脚本按 9 段顺序执行，成功时「消息」窗口依次输出：数据库创建完成 → 6 张表创建完成 → 索引创建完成 → 视图创建完成 → 3 个存储过程创建完成 → 触发器创建完成 → 管理员与测试数据插入完成。脚本内所有对象均带存在性判断，可重复执行。
4. 脚本末尾会执行验证查询，应能看到 6 张表的行数统计（student 3、employer 2、admin 1、job 5、application 4、comment 2）。
5. 创建应用连接账号（脚本中授权语句为注释态，需手工执行一次）：

```sql
USE master;
GO
CREATE LOGIN cp_app_user WITH PASSWORD = '你的强口令', CHECK_POLICY = OFF;
GO
USE CampusPartTime;
GO
CREATE USER cp_app_user FOR LOGIN cp_app_user;
GO
EXEC sp_addrolemember N'db_datareader', N'cp_app_user';
EXEC sp_addrolemember N'db_datawriter', N'cp_app_user';
GRANT EXECUTE ON SCHEMA::dbo TO cp_app_user;
GO
```

> 说明：应用账号只需读写权限与存储过程执行权限，不授予 db_owner，符合设计文档第五章最小权限要求。

---

## 四、账号与口令

### 4.1 账号清单（与 `CampusPartTime.sql` 第 9 段测试数据一致）

| 角色 | 登录账号 | 说明 |
|---|---|---|
| 管理员 | `admin` | 后台审核与统计入口 |
| 学生 | `2024001`（张明） | 学号登录 |
| 学生 | `2024002`（李强） | 学号登录 |
| 学生 | `2024003`（王芳） | 学号登录 |
| 商家 | `13900000001`（校园书吧，联系人陈立） | 手机号登录 |
| 商家 | `13900000002`（优鲜餐饮，联系人刘敏） | 手机号登录 |

### 4.2 口令设置方式

数据库中 `admin_password`、`stu_password`、`emp_password` 三个字段存放的是 **BCrypt 哈希（cost = 10，22 位随机盐，60 位定长）**，不保存明文，因此脚本中不含任何明文口令。首次部署后若要确认或统一口令，请执行 `运行说明\重置测试账号口令.sql`：

```sql
USE CampusPartTime;
-- 执行该脚本后，上述 6 个测试账号与管理员账号口令统一重置为：
-- Cp@20240048
```

重置脚本的哈希由 BCrypt 在 cost=10 下现生成，执行后按上表账号 + 口令 `Cp@20240048` 即可登录。生产部署前必须改用强口令并删除该辅助脚本。

---

## 五、连接字符串与配置节加密

1. 打开 `程序源代码\CampusPartTime\Web.config`，定位 `<connectionStrings>` 节点：

```xml
<add name="CampusConn"
     connectionString="Data Source=.;Initial Catalog=CampusPartTime;User ID=cp_app_user;Password=__DB_PASSWORD__;MultipleActiveResultSets=True;Application Name=CampusPartTime"
     providerName="System.Data.SqlClient" />
```

2. 将 `__DB_PASSWORD__` 替换为第三章节中为 `cp_app_user` 设置的真实口令；若数据库不在本机，将 `Data Source=.` 改为 `Data Source=服务器名或IP,端口`。
3. 使用 ASP.NET IIS 注册工具加密连接串（以管理员身份运行命令提示符，路径按实际 .NET 版本调整）：

```bat
cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319
aspnet_regiis -pef "connectionStrings" "D:\站点物理路径\CampusPartTime"
```

   - 解密命令：`aspnet_regiis -pdf "connectionStrings" "D:\站点物理路径\CampusPartTime"`。
   - 加密后配置节由本机 RSA 密钥保护，Web.config 中连接串变为密文，程序读取方式不变。
4. 其他关键配置项：Forms 认证 `timeout=60`、`slidingExpiration=true`；`UploadImageExtensions=.jpg,.jpeg,.png`、`UploadMaxSizeKB=2048`、`UploadFolder=~/Uploads/`、`PageSize=10`；`requestFiltering maxAllowedContentLength` 为 4MB。

---

## 六、IIS 部署步骤

1. 以管理员身份打开「启用或关闭 Windows 功能」，勾选：Internet Information Services → World Wide Web 服务 → 应用程序开发功能（ASP.NET 4.7、.NET 扩展性、ISAPI 扩展、ISAPI 筛选器）。
2. 将 `程序源代码\CampusPartTime` 整个文件夹复制到站点物理路径（例如 `D:\Sites\CampusPartTime`），确保 `App_Data`、`Uploads` 目录存在且 IIS_IUSRS 具备「修改」权限（上传岗位图片需要写入）。
3. 打开 IIS 管理器 → 应用程序池 → 新建应用程序池：
   - 名称：`CampusPartTimePool`；
   - **.NET CLR 版本：.NET CLR Version v4.0.30319**；
   - **托管管道模式：集成（Integrated）**。
4. 网站 → 添加网站：网站名称 `CampusPartTime`，物理路径指向第 2 步目录，端口绑定 `8081`（或 80，注意端口冲突），应用程序池选择 `CampusPartTimePool`。
5. 若使用 Visual Studio 调试：用 VS2019 打开 `CampusPartTime.sln`，右键项目 →「还原 NuGet 包」（拉取 BCrypt.Net-Next 4.0.3），生成解决方案后按 F5，默认地址 `http://localhost:8080/Login.aspx`。
6. 浏览器访问 `http://localhost:8081/Login.aspx`，使用第四章账号登录，按角色自动分流：
   - 学生 → 岗位检索页；
   - 商家 → 岗位发布页；
   - 管理员 → 后台管理页。

---

## 七、页面访问路径清单

| 序号 | 页面 | 访问路径 | 允许角色 | 主要功能 |
|---|---|---|---|---|
| 1 | 登录页 | `/Login.aspx` | 匿名 | 三角色统一登录入口，按角色分流 |
| 2 | 学生注册 | `/StudentRegister.aspx` | 匿名 | 学号、姓名、学院、专业、联系电话注册 |
| 3 | 商家注册 | `/EmployerRegister.aspx` | 匿名 | 单位名称、联系人、电话、地址注册 |
| 4 | 岗位检索 | `/JobList.aspx` | 学生 / 商家 / 管理员 | 按岗位类型、薪资范围、关键词组合检索与分页 |
| 5 | 岗位详情 | `/JobDetail.aspx` | 学生 / 商家 / 管理员 | 岗位完整信息、商家信息、已审核评价展示 |
| 6 | 发布岗位 | `/JobPublish.aspx` | 商家 | 岗位信息录入、薪资单位选择、截止日期、图片上传（白名单 2MB） |
| 7 | 岗位管理 | `/JobManage.aspx` | 商家 | 本人岗位列表、修改、下架、查看审核状态 |
| 8 | 申请处理 | `/ApplicationManage.aspx` | 商家 | 查看申请人信息，录用（调 proc_hire_applicant）/ 拒绝（调 proc_reject_applicant） |
| 9 | 我的申请 | `/MyApplication.aspx` | 学生 | 本人申请记录与状态（待处理 / 已录用 / 已拒绝）、按状态筛选、录用后可跳转评价 |
| 10 | 提交评价 | `/CommentSubmit.aspx` | 学生 | 仅对已录用岗位开放，1～5 星评分 + 文字评价，提交后进入待审核 |
| 11 | 后台管理 | `/AdminManage.aspx` | 管理员 | 四个页签：用户审核、岗位审核、评价审核、数据统计 |
| 12 | 退出登录 | `/Logout.aspx` | 学生 / 商家 / 管理员 | 清除会话与 Forms 认证票据，返回登录页 |

> 页面总计 12 个业务页面（另有 Global.asax 全局应用程序文件）。IIS 站点默认文档设为 `Login.aspx` 后，站点根路径 `/` 即第 13 个访问入口，直接跳转登录页；如需显式配置，可在 IIS「默认文档」中把 `Login.aspx` 置于首位。三角色业务闭环：
> 学生「注册 → 检索 → 申请 → 查看状态 → 评价」；
> 商家「注册 → 发布 → 管理 → 处理申请 → 被评价」；
> 管理员「登录 → 审核用户 → 审核岗位 → 审核评价 → 查看统计」。

---

## 八、常见问题排查

| 现象 | 可能原因 | 处理办法 |
|---|---|---|
| 打开站点报 500.19 / HTTP 错误 500.21 | 应用程序池 .NET 版本或管道模式不匹配，未注册 ASP.NET 4.0 | 应用程序池改为 v4.0 + 集成模式；管理员运行 `aspnet_regiis -i` |
| 提示「未能加载文件或程序集 BCrypt.Net-Next」 | NuGet 包未还原 | VS 中右键项目「还原 NuGet 包」，或 `nuget install BCrypt.Net-Next -Version 4.0.3` |
| 提示「用户 'cp_app_user' 登录失败」 | 密码未替换或登录名未创建 | 按第三章节创建登录名，Web.config 中替换真实口令后重试 |
| 登录提示账号或密码错误 | 预置哈希口令未知 | 执行 `运行说明\重置测试账号口令.sql`，使用口令 `Cp@20240048` 登录 |
| 岗位发布上传图片报错 | Uploads 目录无写权限，或文件超限 | 给 IIS_IUSRS 授予「修改」权限；单张图片不超过 2MB，仅允许 jpg/jpeg/png |
| 页面样式或校验脚本失效 | 静态资源被 Forms 认证拦截 | 确认 Web.config 中 `Styles`、`Scripts` 的 `<location>` 节点 allow users="*" |
| 访问业务页被重定向到 Login.aspx | 未登录或角色越权（PageBase 拦截） | 使用对应角色账号登录；越权访问属设计预期行为 |
| 录用后岗位未显示「已招满」 | 触发器或存储过程未创建 | 重新执行 SQL 脚本第 5～8 段，确认 `proc_hire_applicant` 与 `trg_job_after_apply` 存在 |

---

## 九、安全与交付说明

1. 全站数据访问统一经 `SqlHelper` 参数化执行，未拼接 SQL 字符串，防范注入。
2. 所有页面继承 `PageBase`，通过 `AllowRoles` 声明可访问角色，未授权访问自动跳转登录页；输出统一经 `HttpUtility.HtmlEncode`（`PageBase.SafeText`）处理，防范 XSS。
3. 口令仅以 BCrypt 哈希存储，登录使用 `BCrypt.Verify` 比对；数据库连接使用最小权限账号 `cp_app_user`。
4. 图片上传统一校验扩展名白名单（jpg/jpeg/png）与大小上限（2048KB），服务端二次校验，不依赖前端。
5. 交付包内不含任何明文生产口令；`PASSWORD = '你的强口令'` 与 `__DB_PASSWORD__` 均为占位符，首次部署时由实施人员填写。
*（内容由AI生成，仅供参考）*
