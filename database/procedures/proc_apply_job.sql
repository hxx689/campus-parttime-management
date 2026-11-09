CREATE PROCEDURE dbo.proc_apply_job
    @student_id INT,
    @job_id     INT,
    @result     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @result = -4;

    DECLARE @v_audit TINYINT, @v_status TINYINT, @v_deadline DATE;

    SELECT @v_audit = j.audit_status, @v_status = j.job_status, @v_deadline = j.deadline
    FROM dbo.job AS j
    WHERE j.job_id = @job_id;

    IF @@ROWCOUNT = 0 OR @v_audit <> 1
    BEGIN
        SET @result = -1;
        RETURN;
    END

    IF @v_status <> 0 OR @v_deadline < CAST(GETDATE() AS DATE)
    BEGIN
        SET @result = -2;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.application AS a
               WHERE a.student_id = @student_id AND a.job_id = @job_id)
    BEGIN
        SET @result = -3;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
            INSERT INTO dbo.application (student_id, job_id, app_date, app_status)
            VALUES (@student_id, @job_id, GETDATE(), 0);
        COMMIT TRANSACTION;
        SET @result = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        SET @result = -4;
    END CATCH
END
GO
