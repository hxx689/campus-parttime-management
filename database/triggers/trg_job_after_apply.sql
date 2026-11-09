CREATE TRIGGER dbo.trg_job_after_apply
ON dbo.application
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(app_status)
        RETURN;

    UPDATE j
    SET j.job_status = 1
    FROM dbo.job AS j
    INNER JOIN
    (
        SELECT DISTINCT i.job_id
        FROM inserted AS i
        INNER JOIN deleted AS d ON i.app_id = d.app_id
        WHERE i.app_status = 1 AND d.app_status <> 1
    ) AS t ON j.job_id = t.job_id
    WHERE j.job_status = 0
      AND j.hired_num >= j.recruit_num;
END
GO
