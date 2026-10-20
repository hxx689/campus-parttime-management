# 校园兼职招聘管理系统

## 项目简介

校园兼职招聘管理系统是一个面向在校大学生的兼职信息管理平台，采用 B/S 三层架构（表现层 / 业务逻辑层 / 数据访问层）开发，面向**学生、商家、管理员**三类用户，提供兼职岗位发布、审核、检索、在线申请、录用评价等全流程功能。

- 课程：互联网应用服务开发与数据库实践（2024-2025 第一学期）
- 班级 / 学号 / 姓名：24034202 / 241240048 / 黄潇萱
- 任课教师：邢艳

## 技术栈

| 层级 | 技术 |
| --- | --- |
| 前端 | HTML / CSS / JavaScript（ASP.NET Web 窗体） |
| 后端 | C# + ASP.NET（BLL / DAL 分层） |
| 数据访问 | ADO.NET |
| 数据库 | SQL Server 2019（库名 `CampusPartTime`） |

## 标准目录结构

```
校园兼职招聘管理系统/
├── docs/report/          # 大作业报告初稿
├── src/backend/          # 后端源码（BLL / DAL / Web）
├── src/frontend/         # 前端资源（css / js / pages）
├── database/             # 数据库脚本（建库建表 / 视图 / 存储过程 / 触发器）
├── tests/                # 测试用例
└── README.md
```

## 项目状态

- [x] 仓库初始化与标准目录结构
- [x] 大作业报告初稿（20+ 次 Git 迭代完成）
- [x] 数据库脚本（建库建表 / 视图 / 存储过程 / 触发器）
- [x] 后端分层骨架（BLL / DAL / Web）
- [x] 前端资源骨架（css / js / pages）
- [ ] 后端业务逻辑实现
- [ ] 前端页面实现

## 使用与部署

1. 执行 `database/CampusPartTime.sql` 建库建表；
2. 依次执行 `database/views/`、`database/procedures/`、`database/triggers/` 下脚本创建视图 / 存储过程 / 触发器；
3. 配置 Web.config 连接字符串后发布到 IIS；
4. 详细步骤见 [大作业报告](docs/report/05-部署与运行说明.md)。

## Git 提交规范

本项目使用 Conventional Commits 规范前缀进行提交：

- `feat:` 新增功能 / 代码
- `docs:` 文档变更
- `refactor:` 重构（不改变外部行为）
- `test:` 测试相关
- `chore:` 构建、杂项
