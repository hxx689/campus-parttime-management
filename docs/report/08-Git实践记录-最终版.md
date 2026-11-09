# 08 Git 实践记录（最终版）

## 一、仓库信息

| 项目 | 内容 |
|---|---|
| 远端仓库 | https://github.com/hxx689/campus-parttime-management |
| 主分支 | `main` |
| 提交总数 | 62 次 |
| 纳管文件数 | 96 个 |
| 提交规范 | Conventional Commits（`feat` / `fix` / `docs` / `test` / `refactor` / `chore` / `style`） |

## 二、提交类型分布

| 类型 | 次数 |
|---|---|
| feat | 39 |
| docs | 15 |
| chore | 4 |
| test | 3 |
| refactor | 1 |

## 三、提交范围（scope）分布

| 范围 | 次数 |
|---|---|
| web | 13 |
| db | 7 |
| dal | 5 |
| bll | 3 |
| ui | 2 |
| model | 2 |
| config | 1 |
| backend | 1 |
| repo | 1 |

## 四、提交信息规范

```
<type>(<scope>): <简要说明>
```

- `type` 表示变更性质，`scope` 表示影响模块，冒号后为一句中文陈述，控制在 50 字以内；
- 一次提交只做一件事，禁止把数据库脚本、页面代码、文档混在同一次提交中；
- 说明中不写"修改""更新了"这类空泛措辞，要写清"做了什么、达到什么效果"。

## 五、完整提交链路（按时间顺序）

- `b22d359` chore: 初始化项目仓库与标准目录结构骨架
- `839054c` docs: 添加项目 README 说明文档
- `35a9d75` chore: 添加 .gitignore 忽略规则
- `2452db9` docs: 大作业报告初稿-封面与文档说明
- `5eb9a3b` docs: 大作业报告初稿-系统需求分析
- `e196c9d` docs: 大作业报告初稿-系统总体设计
- `25fd0cf` docs: 大作业报告初稿-数据库设计
- `a07f9b4` docs: 大作业报告初稿-系统详细设计与实现
- `13aea3e` docs: 大作业报告初稿-系统部署与运行说明
- `bfc9850` docs: 大作业报告初稿-课程设计总结
- `67d2f1a` docs: 大作业报告初稿-Git版本控制实践记录
- `ca29435` feat: 搭建后端项目骨架（BLL/DAL/Web 分层）
- `e6e49ea` feat: 搭建前端资源骨架（css/js/pages）
- `12e40af` feat: 添加数据库建库建表脚本 CampusPartTime
- `649a3e8` feat: 添加视图 v_job_detail（岗位与商家关联视图）
- `25a8320` feat: 添加存储过程 proc_apply_job（事务申请）
- `c9fd5a7` feat: 添加触发器 trg_job_after_apply（满员自动置位）
- `ffba531` refactor: 补充数据库脚本目录说明与执行顺序
- `2f5387f` test: 添加数据库脚本验证用例说明
- `f0a5919` docs: 添加文档目录索引
- `f9b36af` feat: 添加登录页 HTML/CSS/JS 前端校验示例
- `d0c2eb1` docs: 添加 Git 提交规范说明
- `a945b43` chore(repo): 移除后端分层占位目录，统一为完整 Web 项目结构
- `28bf57f` feat(backend): 添加 CampusPartTime 解决方案与 Web 项目定义文件
- `972cca2` feat(db): 补全视图 v_job_detail 完整定义（关联岗位与商家，17 列）
- `069bd94` feat(db): 补全存储过程 proc_apply_job（事务提交，防重复申请）
- `20a34e4` feat(db): 新增存储过程 proc_hire_applicant（录用并自动置满员状态）
- `5919e28` feat(db): 新增存储过程 proc_reject_applicant（拒绝申请并回写状态）
- `5df1bbe` feat(db): 补全触发器 trg_job_after_apply（集合式更新已招人数）
- `209a95e` feat(db): 更新建库主脚本为完整版本（6 表/12 索引/1 视图/3 存储过程/1 触发器）
- `b2952f2` docs(db): 补充数据库脚本执行顺序与验证说明
- `100628d` feat(model): 添加用户实体类（学生 / 商家 / 管理员）
- `630fd20` feat(model): 添加业务实体类（岗位 / 申请记录 / 评价）
- `56b95a3` feat(dal): 添加 SqlHelper 通用数据访问组件（参数化查询与事务执行）
- `f05a600` feat(dal): 实现学生与商家数据访问（注册、登录、资料维护）
- `7b4c9ce` feat(dal): 实现岗位数据访问（检索、详情、发布、审核）
- `77f3659` feat(dal): 实现申请数据访问（提交、状态流转、我的申请）
- `7f1442d` feat(dal): 实现评价与后台管理数据访问
- `aa8017f` feat(bll): 添加用户业务逻辑层（BCrypt 密码哈希与登录校验）
- `b7674e3` feat(bll): 添加岗位与申请业务逻辑层（状态机与权限校验）
- `aaffd33` feat(bll): 添加评价与后台管理业务逻辑层
- `cff9d28` feat(web): 添加 PageBase 基类，实现 Forms 认证与角色拦截
- `49db52f` feat(web): 添加全局应用类与统一异常处理
- `6ecc248` feat(web): 实现登录页与安全退出
- `12cdb7a` feat(web): 实现学生注册页（含头像上传与格式校验）
- `2a735a8` feat(web): 实现商家注册页
- `a2bf34a` feat(web): 实现岗位检索页（分页、类型筛选、发布时间排序）
- `c821477` feat(web): 实现岗位详情页与申请入口
- `a10e1de` feat(web): 实现学生我的申请页
- `b021549` feat(web): 实现商家岗位发布页
- `0a06ccf` feat(web): 实现商家岗位管理页（上架、下架、编辑）
- `b5d7095` feat(web): 实现商家申请管理页（录用 / 拒绝）
- `e66be0a` feat(web): 实现学生评价提交页
- `801ed40` feat(web): 实现管理员后台审核与管理页
- `4fb355d` chore(config): 添加 Web.config 配置模板（连接字符串占位、认证模式、上传限制）
- `8ef5b20` feat(ui): 添加全站响应式样式表 style.css
- `ddc02e1` feat(ui): 添加前端表单校验与交互脚本 common.js
- `6c3693a` test: 添加数据库脚本结构校验脚本（表/视图/存储过程/触发器/索引/外键）
- `d434130` test: 补充测试用例说明与执行方式
- `3a1ce7b` docs: 添加最终版系统设计报告（Word 文档）
- `91a0897` docs: 添加最终版系统设计报告（PDF 版）
- `45e6537` docs: 添加系统部署与运行说明及测试账号口令重置脚本

## 六、分支与同步

- 本地 `main` 与 `origin/main` 保持一致，采用线性提交，无合并冲突；
- 每次功能完成后立即提交并推送，保证远端仓库即为项目真实演进过程。
