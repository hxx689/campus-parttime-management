-- =====================================================
-- 校园兼职招聘管理系统 数据库建库建表脚本
-- 数据库：CampusPartTime  (SQL Server 2019)
-- 说明：大作业报告初稿配套脚本，逐步迭代完善
-- =====================================================

IF DB_ID('CampusPartTime') IS NULL
BEGIN
    CREATE DATABASE CampusPartTime;
END
GO

USE CampusPartTime;
GO

-- 1. 学生表
IF OBJECT_ID('student', 'U') IS NULL
BEGIN
    CREATE TABLE student (
        stu_id       INT IDENTITY(1,1) PRIMARY KEY,
        stu_name     NVARCHAR(50)  NOT NULL,
        stu_gender   CHAR(2)       NULL CHECK (stu_gender IN ('男','女')),
        stu_major    NVARCHAR(50)  NULL,
        stu_phone    VARCHAR(20)   NOT NULL UNIQUE,
        stu_password VARCHAR(128)  NOT NULL,   -- BCrypt 密文
        create_time  DATETIME      DEFAULT GETDATE()
    );
END
GO

-- 2. 商家表
IF OBJECT_ID('employer', 'U') IS NULL
BEGIN
    CREATE TABLE employer (
        emp_id       INT IDENTITY(1,1) PRIMARY KEY,
        emp_name     NVARCHAR(80)  NOT NULL,
        emp_contact  NVARCHAR(50)  NULL,
        emp_phone    VARCHAR(20)   NOT NULL UNIQUE,
        emp_password VARCHAR(128)  NOT NULL,
        create_time  DATETIME      DEFAULT GETDATE()
    );
END
GO

-- 3. 管理员表
IF OBJECT_ID('admin', 'U') IS NULL
BEGIN
    CREATE TABLE admin (
        adm_id       INT IDENTITY(1,1) PRIMARY KEY,
        adm_name     NVARCHAR(50)  NOT NULL,
        adm_password VARCHAR(128)  NOT NULL,
        create_time  DATETIME      DEFAULT GETDATE()
    );
END
GO

-- 4. 兼职岗位表
IF OBJECT_ID('job', 'U') IS NULL
BEGIN
    CREATE TABLE job (
        job_id      INT IDENTITY(1,1) PRIMARY KEY,
        emp_id      INT NOT NULL REFERENCES employer(emp_id),
        job_title   NVARCHAR(80)  NOT NULL,
        job_type    NVARCHAR(30)  NULL,
        salary      DECIMAL(10,2) NULL CHECK (salary >= 0),
        head_count  INT           DEFAULT 1,
        work_time   NVARCHAR(100) NULL,
        status      TINYINT       DEFAULT 0,  -- 0待审核 1审核通过 2已下架 3已招满
        publish_time DATETIME     DEFAULT GETDATE()
    );
    CREATE INDEX idx_job_type   ON job(job_type);
    CREATE INDEX idx_job_status ON job(status);
END
GO

-- 5. 申请记录表
IF OBJECT_ID('application', 'U') IS NULL
BEGIN
    CREATE TABLE application (
        app_id   INT IDENTITY(1,1) PRIMARY KEY,
        stu_id   INT NOT NULL REFERENCES student(stu_id),
        job_id   INT NOT NULL REFERENCES job(job_id),
        app_time DATETIME DEFAULT GETDATE(),
        status   TINYINT DEFAULT 0,   -- 0申请中 1已录用 2已拒绝
        CONSTRAINT uq_stu_job UNIQUE (stu_id, job_id)
    );
    CREATE INDEX idx_app_job ON application(job_id);
    CREATE INDEX idx_app_status ON application(status);
END
GO

-- 6. 评价表
IF OBJECT_ID('comment', 'U') IS NULL
BEGIN
    CREATE TABLE comment (
        com_id   INT IDENTITY(1,1) PRIMARY KEY,
        stu_id   INT NOT NULL REFERENCES student(stu_id),
        job_id   INT NOT NULL REFERENCES job(job_id),
        score    TINYINT NOT NULL CHECK (score BETWEEN 1 AND 5),
        content  NVARCHAR(500) NULL,
        com_time DATETIME DEFAULT GETDATE()
    );
    CREATE INDEX idx_comment_job ON comment(job_id);
END
GO
