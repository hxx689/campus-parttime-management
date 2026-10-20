# 数据库脚本目录

| 路径 | 内容 |
| --- | --- |
| `CampusPartTime.sql` | 建库建表脚本（6 张表、索引、约束） |
| `views/v_job_detail.sql` | 视图：岗位与商家关联，仅展示审核通过岗位 |
| `procedures/proc_apply_job.sql` | 存储过程：事务化在线申请 |
| `triggers/trg_job_after_apply.sql` | 触发器：满员自动置位 |

## 执行顺序

1. `CampusPartTime.sql`
2. `views/v_job_detail.sql`
3. `procedures/proc_apply_job.sql`
4. `triggers/trg_job_after_apply.sql`
