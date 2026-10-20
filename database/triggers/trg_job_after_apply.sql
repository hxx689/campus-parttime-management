-- =====================================================
-- 触发器 trg_job_after_apply：申请插入后自动置岗位为已招满
-- 当岗位申请人数达到招聘人数时，自动更新 job.status = 3
-- =====================================================
USE CampusPartTime;
GO

IF OBJECT_ID('trg_job_after_apply', 'TR') IS NOT NULL
    DROP TRIGGER trg_job_after_apply;
GO

CREATE TRIGGER trg_job_after_apply
ON application
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE j
    SET j.status = 3
    FROM job j
    INNER JOIN inserted i ON j.job_id = i.job_id
    WHERE j.status = 1
      AND j.head_count <= (
            SELECT COUNT(*)
            FROM application a
            WHERE a.job_id = i.job_id
          );
END
GO
