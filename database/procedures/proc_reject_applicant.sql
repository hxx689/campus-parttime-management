CREATE PROCEDURE dbo.proc_reject_applicant
    @app_id      INT,
    @employer_id INT,
    @result      INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE a
    SET a.app_status = 2
    FROM dbo.application AS a
    INNER JOIN dbo.job AS j ON a.job_id = j.job_id
    WHERE a.app_id = @app_id
      AND j.employer_id = @employer_id
      AND a.app_status = 0;

    IF @@ROWCOUNT = 1
        SET @result = 1;
    ELSE
        SET @result = -1;
END
GO
