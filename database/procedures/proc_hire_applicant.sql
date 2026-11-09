CREATE PROCEDURE dbo.proc_hire_applicant
    @app_id      INT,
    @employer_id INT,
    @result      INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @result = -4;

    DECLARE @v_job_id INT, @v_app_status TINYINT, @v_recruit INT,
            @v_hired INT, @v_job_status TINYINT;

    BEGIN TRY
        BEGIN TRANSACTION;

            SELECT @v_job_id = a.job_id, @v_app_status = a.app_status
            FROM dbo.application AS a
            INNER JOIN dbo.job AS j ON a.job_id = j.job_id
            WHERE a.app_id = @app_id AND j.employer_id = @employer_id;

            IF @@ROWCOUNT = 0
            BEGIN
                ROLLBACK TRANSACTION;
                SET @result = -1;
                RETURN;
            END

            IF @v_app_status <> 0
            BEGIN
                ROLLBACK TRANSACTION;
                SET @result = -2;
                RETURN;
            END

            SELECT @v_recruit = j.recruit_num,
                   @v_hired = j.hired_num,
                   @v_job_status = j.job_status
            FROM dbo.job AS j WITH (UPDLOCK, ROWLOCK)
            WHERE j.job_id = @v_job_id;

            IF @v_job_status <> 0 OR @v_hired >= @v_recruit
            BEGIN
                ROLLBACK TRANSACTION;
                SET @result = -3;
                RETURN;
            END

            UPDATE dbo.application SET app_status = 1 WHERE app_id = @app_id;
            UPDATE dbo.job SET hired_num = hired_num + 1 WHERE job_id = @v_job_id;

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
