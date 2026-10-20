# 测试用例说明

本目录用于存放与数据库脚本、业务逻辑对应的验证用例。

## 数据库脚本验证用例

### 用例 1：视图 v_job_detail 验证

```sql
-- 预期：仅返回审核通过（status=1）的岗位及其商家信息
SELECT * FROM v_job_detail;
```

### 用例 2：存储过程 proc_apply_job 验证

```sql
DECLARE @r INT;
EXEC proc_apply_job @stu_id = 1, @job_id = 1, @result = @r OUTPUT;
-- 预期状态码：1 成功 / -1 岗位无效 / -2 已招满 / -3 重复申请
SELECT @r;
```

### 用例 3：触发器 trg_job_after_apply 验证

```sql
-- 连续插入达到 head_count 的申请后：
-- 预期：job.status 自动由 1 变为 3（已招满）
SELECT job_id, status FROM job WHERE job_id = 1;
```

> 用例将随开发迭代持续补充（含前端校验与接口级用例）。
