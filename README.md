# 校园兼职招聘管理系统

互联网应用服务开发与数据库实践 课程大作业

| 项目 | 内容 |
|---|---|
| 学号 / 姓名 | 241240048 / 黄潇萱 |
| 班级 | 24034202 |
| 任课教师 | 邢艳 |
| 技术栈 | ASP.NET Web Forms（.NET Framework 4.7.2）+ C# 分层 + ADO.NET + SQL Server |
| 数据库 | `CampusPartTime`（6 表 / 1 视图 / 3 存储过程 / 1 触发器） |
| 提交记录 | 62 次规范提交（Conventional Commits） |

## 目录结构

```
campus-parttime-management/
├─ database/                       # 数据库脚本
│  ├─ CampusPartTime.sql           # 主脚本：建库、建表、索引、视图、存储过程、触发器、测试数据
│  ├─ views/                       # 视图单项重建脚本
│  ├─ procedures/                  # 存储过程单项重建脚本
│  └─ triggers/                    # 触发器单项重建脚本
├─ docs/
│  ├─ report/                      # 系统设计报告分章 Markdown + 最终版 Word/PDF
│  └─ deploy/                      # 部署与运行说明
├─ src/
│  ├─ backend/                     # ASP.NET Web 项目（BLL / DAL / Model / Common / 页面）
│  └─ frontend/                    # 早期前端校验原型
├─ tests/                          # 数据库脚本结构校验脚本
└─ README.md
```

## 快速开始

1. **建库**：在 SQL Server Management Studio 中新建查询，执行 `database/CampusPartTime.sql`；
2. **配置连接串**：打开 `src/backend/CampusPartTime/Web.config`，把 `connectionStrings` 中的
   `__DB_SERVER__` / `__DB_NAME__` / `__DB_USER__` / `__DB_PASSWORD__` 换成实际值；
3. **编译**：用 Visual Studio 打开 `src/backend/CampusPartTime.sln` 并生成；
4. **部署**：将发布目录挂到 IIS，应用池 .NET CLR 版本选 v4.0，启用 32 位应用程序可选；
5. **访问**：`http://localhost/Login.aspx`。

详细步骤见 `docs/deploy/README.md`。

## 功能概览

| 角色 | 主要功能 |
|---|---|
| 学生 | 注册登录、岗位检索与筛选、岗位详情、提交申请、查看申请状态、评价商家 |
| 商家 | 注册登录、发布岗位、岗位上下架与编辑、查看申请、录用 / 拒绝、查看评价 |
| 管理员 | 商家与岗位审核、用户管理、申请与评价管理、数据统计 |

## 提交规范

见 `COMMIT_CONVENTION.md`。本项目共 62 次提交，类型分布与完整链路见
`docs/report/08-Git实践记录-最终版.md`。
