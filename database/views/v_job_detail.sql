-- =====================================================
-- 视图 v_job_detail：岗位与商家关联视图（仅展示审核通过岗位）
-- =====================================================
USE CampusPartTime;
GO

IF OBJECT_ID('v_job_detail', 'V') IS NOT NULL
    DROP VIEW v_job_detail;
GO

CREATE VIEW v_job_detail
AS
SELECT
    j.job_id,
    j.emp_id,
    e.emp_name,
    j.job_title,
    j.job_type,
    j.salary,
    j.head_count,
    j.work_time,
    j.publish_time
FROM job j
INNER JOIN employer e ON j.emp_id = e.emp_id
WHERE j.status = 1;   -- 仅审核通过
GO
