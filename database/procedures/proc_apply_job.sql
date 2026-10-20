-- =====================================================
-- 存储过程 proc_apply_job：学生在线申请岗位
-- 事务保证数据一致性，返回状态码：
--   -1 岗位不存在或已下架
--   -2 岗位已招满
--   -3 重复申请
--    1 申请成功
-- =====================================================
USE CampusPartTime;
GO

IF OBJECT_ID('proc_apply_job', 'P') IS NOT NULL
    DROP PROCEDURE proc_apply_job;
GO

CREATE PROCEDURE proc_apply_job
    @stu_id INT,
    @job_id INT,
    @result INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @result = 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. 岗位存在性检查
        IF NOT EXISTS (SELECT 1 FROM job WHERE job_id = @job_id AND status IN (1, 3))
        BEGIN
            SET @result = -1;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. 岗位是否已招满
        IF EXISTS (SELECT 1 FROM job WHERE job_id = @job_id AND status = 3)
        BEGIN
            SET @result = -2;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. 重复申请检查
        IF EXISTS (SELECT 1 FROM application WHERE stu_id = @stu_id AND job_id = @job_id)
        BEGIN
            SET @result = -3;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. 插入申请记录
        INSERT INTO application (stu_id, job_id) VALUES (@stu_id, @job_id);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        SET @result = -1;
    END CATCH
END
GO
