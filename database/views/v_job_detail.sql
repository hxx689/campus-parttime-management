CREATE VIEW dbo.v_job_detail
AS
SELECT j.job_id, j.job_name, j.job_type, j.salary, j.salary_unit, j.work_place,
       j.work_time, j.recruit_num, j.hired_num, j.job_desc, j.publish_date,
       j.deadline, j.job_status, e.employer_id, e.emp_name, e.emp_contact, e.emp_phone
FROM dbo.job AS j
INNER JOIN dbo.employer AS e ON j.employer_id = e.employer_id
WHERE j.audit_status = 1;
GO
