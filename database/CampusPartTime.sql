/* =====================================================================================
   校园兼职招聘管理系统 —— 数据库完整创建脚本
   -------------------------------------------------------------------------------------
   课程名称 : 互联网应用服务开发与数据库实践
   设计题目 : 校园兼职招聘管理系统
   学    号 : 241240048
   姓    名 : 黄潇萱
   数据库   : Microsoft SQL Server 2019 / 数据库名 CampusPartTime
   配套文档 : 《241240048黄潇萱_系统设计文档.doc》第三章（表2～表13）、第四章第1节
   -------------------------------------------------------------------------------------
   脚本结构（共 9 个分段，可在 SSMS 中整体按 F5 顺序执行）：
     第 1 段  创建数据库 CampusPartTime（含数据文件与日志文件配置）
     第 2 段  创建数据表 student / employer / admin / job / application / comment
     第 3 段  创建索引（含唯一索引、筛选索引、复合索引）
     第 4 段  创建视图 v_job_detail
     第 5 段  创建存储过程 proc_apply_job
     第 6 段  创建存储过程 proc_hire_applicant
     第 7 段  创建存储过程 proc_reject_applicant
     第 8 段  创建触发器 trg_job_after_apply
     第 9 段  初始化数据（管理员初始账号、测试数据）与结果验证
   -------------------------------------------------------------------------------------
   执行环境说明：
     1. 需要 SQL Server 2019 及以上版本，使用 SSMS 18 及以上版本打开本脚本；
     2. 执行账号需具备创建数据库、创建登录名与授权的能力（首次部署可用实例管理员）；
     3. 脚本中的对象创建语句均带存在性判断，可重复执行而不会因对象已存在而报错；
     4. 数据文件与日志文件采用 SQL Server 默认数据目录，如需指定路径，可在
        CREATE DATABASE 语句的 NAME 子句中补充 FILENAME 参数；
     5. 本脚本不含任何生产口令明文，应用连接使用最小权限账号 cp_app_user，
        该账号口令在部署时由实施人员设定，详见第 9 段说明与《运行说明.txt》。
   ===================================================================================== */

USE master;
GO

/* =====================================================================================
   第 1 段  创建数据库 CampusPartTime
   说明：数据文件初始大小 20MB、按 10% 自动增长；日志文件初始大小 10MB、按 10% 自动增长
   ===================================================================================== */
IF DB_ID(N'CampusPartTime') IS NULL
BEGIN
    CREATE DATABASE CampusPartTime
    ON PRIMARY
    (
        NAME     = N'CampusPartTime',
        SIZE     = 20MB,
        FILEGROWTH = 10%
    )
    LOG ON
    (
        NAME     = N'CampusPartTime_log',
        SIZE     = 10MB,
        FILEGROWTH = 10%
    );
    PRINT N'数据库 CampusPartTime 创建完成。';
END
ELSE
    PRINT N'数据库 CampusPartTime 已存在，跳过创建。';
GO

USE CampusPartTime;
GO

/* =====================================================================================
   第 2 段  创建数据表（6 张表，共 56 个字段）
   对应设计文档：表2 学生信息表、表3 商家信息表、表4 管理员表、
                 表5 兼职岗位表、表6 申请记录表、表7 评价表
   约束类型：主键约束、外键约束 6 处、唯一约束 5 处、检查约束 15 处、默认值约束
   ===================================================================================== */

/* ---------- 2.1 学生信息表 student（13 个字段） ---------- */
IF OBJECT_ID(N'dbo.student', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.student
    (
        student_id   INT           IDENTITY(1,1) NOT NULL,
        stu_no       VARCHAR(20)   NOT NULL,
        stu_name     VARCHAR(20)   NOT NULL,
        stu_sex      NCHAR(1)      NOT NULL,
        stu_birth    DATE          NULL,
        stu_college  VARCHAR(50)   NOT NULL,
        stu_major    VARCHAR(50)   NOT NULL,
        stu_grade    VARCHAR(10)   NOT NULL,
        stu_phone    VARCHAR(11)   NULL,
        stu_email    VARCHAR(50)   NULL,
        stu_password VARCHAR(60)   NOT NULL,
        reg_date     DATETIME      NOT NULL CONSTRAINT DF_student_reg_date DEFAULT (GETDATE()),
        stu_status   TINYINT       NOT NULL CONSTRAINT DF_student_status   DEFAULT (0),
        CONSTRAINT PK_student PRIMARY KEY CLUSTERED (student_id),
        CONSTRAINT UQ_student_no UNIQUE (stu_no),
        CONSTRAINT CK_student_sex    CHECK (stu_sex IN (N'男', N'女')),
        CONSTRAINT CK_student_status CHECK (stu_status IN (0, 1)),
        CONSTRAINT CK_student_phone  CHECK (stu_phone IS NULL
                                            OR stu_phone LIKE '1[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
    );
    PRINT N'表 student 创建完成。';
END
GO

/* ---------- 2.2 商家信息表 employer（10 个字段） ---------- */
IF OBJECT_ID(N'dbo.employer', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.employer
    (
        employer_id  INT           IDENTITY(1,1) NOT NULL,
        emp_name     VARCHAR(50)   NOT NULL,
        emp_contact  VARCHAR(20)   NOT NULL,
        emp_phone    VARCHAR(11)   NOT NULL,
        emp_address  VARCHAR(100)  NULL,
        emp_intro    NVARCHAR(500) NULL,
        emp_email    VARCHAR(50)   NULL,
        emp_password VARCHAR(60)   NOT NULL,
        reg_date     DATETIME      NOT NULL CONSTRAINT DF_employer_reg_date DEFAULT (GETDATE()),
        emp_status   TINYINT       NOT NULL CONSTRAINT DF_employer_status   DEFAULT (0),
        CONSTRAINT PK_employer PRIMARY KEY CLUSTERED (employer_id),
        CONSTRAINT UQ_employer_phone UNIQUE (emp_phone),
        CONSTRAINT CK_employer_phone  CHECK (emp_phone LIKE '1[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
        CONSTRAINT CK_employer_status CHECK (emp_status IN (0, 1))
    );
    PRINT N'表 employer 创建完成。';
END
GO

/* ---------- 2.3 管理员表 admin（3 个字段） ---------- */
IF OBJECT_ID(N'dbo.admin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.admin
    (
        admin_id       INT         IDENTITY(1,1) NOT NULL,
        admin_name     VARCHAR(20) NOT NULL,
        admin_password VARCHAR(60) NOT NULL,
        CONSTRAINT PK_admin PRIMARY KEY CLUSTERED (admin_id),
        CONSTRAINT UQ_admin_name UNIQUE (admin_name)
    );
    PRINT N'表 admin 创建完成。';
END
GO

/* ---------- 2.4 兼职岗位表 job（16 个字段） ---------- */
IF OBJECT_ID(N'dbo.job', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.job
    (
        job_id       INT           IDENTITY(1,1) NOT NULL,
        employer_id  INT           NOT NULL,
        job_name     VARCHAR(50)   NOT NULL,
        job_type     VARCHAR(20)   NOT NULL,
        salary       DECIMAL(8,2)  NOT NULL CONSTRAINT DF_job_salary       DEFAULT (0),
        salary_unit  VARCHAR(10)   NOT NULL CONSTRAINT DF_job_salary_unit  DEFAULT ('元/小时'),
        work_place   VARCHAR(100)  NOT NULL,
        work_time    VARCHAR(50)   NOT NULL,
        recruit_num  INT           NOT NULL CONSTRAINT DF_job_recruit_num  DEFAULT (1),
        hired_num    INT           NOT NULL CONSTRAINT DF_job_hired_num    DEFAULT (0),
        job_desc     NVARCHAR(1000) NULL,
        publish_date DATETIME      NOT NULL CONSTRAINT DF_job_publish_date DEFAULT (GETDATE()),
        deadline     DATE          NOT NULL,
        audit_status TINYINT       NOT NULL CONSTRAINT DF_job_audit_status DEFAULT (0),
        job_status   TINYINT       NOT NULL CONSTRAINT DF_job_job_status   DEFAULT (0),
        CONSTRAINT PK_job PRIMARY KEY CLUSTERED (job_id),
        CONSTRAINT FK_job_employer  FOREIGN KEY (employer_id) REFERENCES dbo.employer(employer_id),
        CONSTRAINT CK_job_salary       CHECK (salary >= 0),
        CONSTRAINT CK_job_salary_unit  CHECK (salary_unit IN ('元/小时', '元/天')),
        CONSTRAINT CK_job_recruit_num  CHECK (recruit_num BETWEEN 1 AND 100),
        CONSTRAINT CK_job_hired        CHECK (hired_num >= 0 AND hired_num <= recruit_num),
        CONSTRAINT CK_job_audit_status CHECK (audit_status IN (0, 1, 2)),
        CONSTRAINT CK_job_job_status   CHECK (job_status IN (0, 1, 2)),
        CONSTRAINT CK_job_deadline     CHECK (deadline >= CAST(publish_date AS DATE))
    );
    PRINT N'表 job 创建完成。';
END
GO

/* ---------- 2.5 申请记录表 application（6 个字段） ---------- */
IF OBJECT_ID(N'dbo.application', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.application
    (
        app_id     INT           IDENTITY(1,1) NOT NULL,
        student_id INT           NOT NULL,
        job_id     INT           NOT NULL,
        app_date   DATETIME      NOT NULL CONSTRAINT DF_app_date DEFAULT (GETDATE()),
        app_status TINYINT       NOT NULL CONSTRAINT DF_app_status DEFAULT (0),
        app_remark NVARCHAR(200) NULL,
        CONSTRAINT PK_application PRIMARY KEY CLUSTERED (app_id),
        CONSTRAINT UQ_app_student_job UNIQUE (student_id, job_id),
        CONSTRAINT FK_app_student FOREIGN KEY (student_id) REFERENCES dbo.student(student_id),
        CONSTRAINT FK_app_job     FOREIGN KEY (job_id)     REFERENCES dbo.job(job_id),
        CONSTRAINT CK_app_status  CHECK (app_status IN (0, 1, 2))
    );
    PRINT N'表 application 创建完成。';
END
GO

/* ---------- 2.6 评价表 comment（8 个字段） ---------- */
IF OBJECT_ID(N'dbo.comment', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.comment
    (
        comment_id   INT           IDENTITY(1,1) NOT NULL,
        student_id   INT           NOT NULL,
        job_id       INT           NOT NULL,
        employer_id  INT           NOT NULL,
        score        INT           NOT NULL CONSTRAINT DF_comment_score DEFAULT (5),
        content      NVARCHAR(500) NULL,
        comment_date DATETIME      NOT NULL CONSTRAINT DF_comment_date DEFAULT (GETDATE()),
        audit_status TINYINT       NOT NULL CONSTRAINT DF_comment_audit_status DEFAULT (0),
        CONSTRAINT PK_comment PRIMARY KEY CLUSTERED (comment_id),
        CONSTRAINT UQ_comment_stu_job UNIQUE (student_id, job_id),
        CONSTRAINT FK_comment_student  FOREIGN KEY (student_id)  REFERENCES dbo.student(student_id),
        CONSTRAINT FK_comment_job      FOREIGN KEY (job_id)      REFERENCES dbo.job(job_id),
        CONSTRAINT FK_comment_employer FOREIGN KEY (employer_id) REFERENCES dbo.employer(employer_id),
        CONSTRAINT CK_comment_score        CHECK (score BETWEEN 1 AND 5),
        CONSTRAINT CK_comment_audit_status CHECK (audit_status IN (0, 1, 2))
    );
    PRINT N'表 comment 创建完成。';
END
GO

/* =====================================================================================
   第 3 段  创建索引
   对应设计文档第三章 3 节“数据库索引的设计说明”：
     （1）student：学号唯一索引（由 UQ_student_no 自动创建）、手机号筛选唯一索引、
                   注册日期普通索引；
     （2）employer：商家名称普通索引、联系电话唯一索引（由 UQ_employer_phone 自动创建）；
     （3）job：商家+状态的复合索引、岗位类型索引、发布日期倒序索引、外键索引、
              待审核筛选索引（仅索引 audit_status = 0 的行）；
     （4）application：学生外键索引、岗位+申请状态复合索引；
     （5）comment：岗位+审核状态复合索引、商家+审核状态复合索引、待审核筛选索引。
   说明：筛选索引用于加速管理员“待审核”列表查询，仅索引符合条件的少量行。
   ===================================================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_student_phone' AND object_id = OBJECT_ID(N'dbo.student'))
    CREATE UNIQUE INDEX UQ_student_phone ON dbo.student(stu_phone) WHERE stu_phone IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_student_reg_date' AND object_id = OBJECT_ID(N'dbo.student'))
    CREATE INDEX IX_student_reg_date ON dbo.student(reg_date);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_employer_name' AND object_id = OBJECT_ID(N'dbo.employer'))
    CREATE INDEX IX_employer_name ON dbo.employer(emp_name);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_job_employer' AND object_id = OBJECT_ID(N'dbo.job'))
    CREATE INDEX IX_job_employer ON dbo.job(employer_id);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_job_employer_status' AND object_id = OBJECT_ID(N'dbo.job'))
    CREATE INDEX IX_job_employer_status ON dbo.job(employer_id, job_status);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_job_type' AND object_id = OBJECT_ID(N'dbo.job'))
    CREATE INDEX IX_job_type ON dbo.job(job_type);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_job_publish_date' AND object_id = OBJECT_ID(N'dbo.job'))
    CREATE INDEX IX_job_publish_date ON dbo.job(publish_date DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_job_audit' AND object_id = OBJECT_ID(N'dbo.job'))
    CREATE INDEX IX_job_audit ON dbo.job(audit_status) WHERE audit_status = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_app_student' AND object_id = OBJECT_ID(N'dbo.application'))
    CREATE INDEX IX_app_student ON dbo.application(student_id);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_app_job_status' AND object_id = OBJECT_ID(N'dbo.application'))
    CREATE INDEX IX_app_job_status ON dbo.application(job_id, app_status);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_comment_job' AND object_id = OBJECT_ID(N'dbo.comment'))
    CREATE INDEX IX_comment_job ON dbo.comment(job_id, audit_status);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_comment_employer' AND object_id = OBJECT_ID(N'dbo.comment'))
    CREATE INDEX IX_comment_employer ON dbo.comment(employer_id, audit_status);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_comment_audit' AND object_id = OBJECT_ID(N'dbo.comment'))
    CREATE INDEX IX_comment_audit ON dbo.comment(audit_status) WHERE audit_status = 0;
GO
PRINT N'第 3 段：索引创建完成（唯一索引 1 个、筛选索引 2 个、普通索引 9 个，另由唯一约束自动生成 4 个唯一索引）。';
GO

/* =====================================================================================
   第 4 段  创建视图 v_job_detail（文档第四章 1 节（1））
   作用：将岗位表与商家表按商家编号连接，对外提供含商家名称与联系方式的完整岗位信息，
         并在视图层过滤未通过审核的岗位（audit_status = 1），应用层无需重复编写多表连接。
   ===================================================================================== */
IF OBJECT_ID(N'dbo.v_job_detail', N'V') IS NOT NULL
    DROP VIEW dbo.v_job_detail;
GO
CREATE VIEW dbo.v_job_detail
AS
SELECT j.job_id, j.job_name, j.job_type, j.salary, j.salary_unit, j.work_place,
       j.work_time, j.recruit_num, j.hired_num, j.job_desc, j.publish_date,
       j.deadline, j.job_status, e.employer_id, e.emp_name, e.emp_contact, e.emp_phone
FROM dbo.job AS j
INNER JOIN dbo.employer AS e ON j.employer_id = e.employer_id
WHERE j.audit_status = 1;
GO
PRINT N'第 4 段：视图 v_job_detail 创建完成（17 列）。';
GO

/* =====================================================================================
   第 5 段  创建存储过程 proc_apply_job（文档第四章 1 节（2））
   功能：学生申请岗位。集中完成岗位审核状态校验、报名截止校验、重复申请校验与申请写入，
         并使用显式事务保证原子性；只写入待处理申请，不修改已录用人数。
   入参：@student_id 学生编号、@job_id 岗位编号、@result 返回码（OUTPUT）
   返回码：1 成功；-1 岗位不存在或未通过审核；-2 岗位不在招或已过报名截止日期；
           -3 已申请过该岗位；-4 写入失败
   ===================================================================================== */
IF OBJECT_ID(N'dbo.proc_apply_job', N'P') IS NOT NULL
    DROP PROCEDURE dbo.proc_apply_job;
GO
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
PRINT N'第 5 段：存储过程 proc_apply_job 创建完成。';
GO

/* =====================================================================================
   第 6 段  创建存储过程 proc_hire_applicant（文档第四章 1 节（3））
   功能：商家录用申请。校验申请归属与待处理状态，在事务内以 UPDLOCK 重新读取招聘人数与
         已录用人数，未满员时把申请置为已录用并把已录用人数加一，保证并发一致性。
         录用是“已录用人数”唯一累加入口，申请环节不累加。
   入参：@app_id 申请编号、@employer_id 商家编号、@result 返回码（OUTPUT）
   返回码：1 成功；-1 申请不存在或不属于该商家；-2 该申请已处理；-3 岗位已招满或不在招；-4 执行异常
   ===================================================================================== */
IF OBJECT_ID(N'dbo.proc_hire_applicant', N'P') IS NOT NULL
    DROP PROCEDURE dbo.proc_hire_applicant;
GO
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
PRINT N'第 6 段：存储过程 proc_hire_applicant 创建完成。';
GO

/* =====================================================================================
   第 7 段  创建存储过程 proc_reject_applicant（文档第四章 1 节（4））
   功能：商家拒绝申请。校验申请归属后把待处理申请置为已拒绝，不改变已录用人数；
         以一条带条件的 UPDATE 同时完成校验与更新，避免“先查后改”的并发窗口。
   入参：@app_id 申请编号、@employer_id 商家编号、@result 返回码（OUTPUT）
   返回码：1 成功；-1 申请不存在、不属于该商家或已处理
   ===================================================================================== */
IF OBJECT_ID(N'dbo.proc_reject_applicant', N'P') IS NOT NULL
    DROP PROCEDURE dbo.proc_reject_applicant;
GO
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
PRINT N'第 7 段：存储过程 proc_reject_applicant 创建完成。';
GO

/* =====================================================================================
   第 8 段  创建触发器 trg_job_after_apply（文档第四章 1 节（5））
   功能：申请状态更新后自动维护岗位状态。采用 AFTER UPDATE 集合式写法，通过 inserted 与
         deleted 表一次性取出本次由其他状态变为“已录用”的岗位编号并去重，再以一条 UPDATE
         批量修正为“已招满”，批量录用时不会遗漏行；已下架岗位的状态不被覆盖。
   ===================================================================================== */
IF OBJECT_ID(N'dbo.trg_job_after_apply', N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_job_after_apply;
GO
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
PRINT N'第 8 段：触发器 trg_job_after_apply 创建完成。';
GO

/* =====================================================================================
   第 9 段  初始化数据与结果验证
   -------------------------------------------------------------------------------------
   9.1 管理员初始账号：admin_name = 'admin'，口令为 BCrypt(cost = 10) 哈希，库中不存明文；
   9.2 测试数据：学生 3 条、商家 2 条、岗位 5 条、申请 4 条、评价 2 条；
   9.3 最小权限账号 cp_app_user 的创建与授权语句（口令由实施人员设定，脚本中用占位符）；
   9.4 结果验证查询：输出各表行数与对象清单，便于确认脚本执行结果。
   -------------------------------------------------------------------------------------
   注意：以下哈希值均由 BCrypt 算法在 cost = 10 下生成，60 位定长字符串，
         对应明文口令见《运行说明.txt》，生产部署前必须全部重置。
   ===================================================================================== */

/* ---------- 9.1 管理员初始账号 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.admin WHERE admin_name = 'admin')
BEGIN
    INSERT INTO dbo.admin (admin_name, admin_password)
    VALUES ('admin', '$2b$10$muvNnJZ4izLgjy/5qvSe8usw3fLFGJu.UaJ7V.R2rWI5mZMp7w/lq');
    PRINT N'管理员初始账号 admin 插入完成。';
END
GO

/* ---------- 9.2 测试数据 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.student)
BEGIN
    SET IDENTITY_INSERT dbo.student ON;
    INSERT INTO dbo.student (student_id, stu_no, stu_name, stu_sex, stu_birth, stu_college,
                             stu_major, stu_grade, stu_phone, stu_email, stu_password,
                             reg_date, stu_status)
    VALUES
    (1, '2024001', '张明', N'女', '2004-03-12', '安全科学与工程学院', '安全工程', '2024级',
     '13800000001', 'zhangming@example.com', '$2b$10$m2fj8sslSZtaezn/0s5B6e9sYx3LoB.9.WbpFRLQ.5n34Aq1z52W2',
     DATEADD(DAY, -40, GETDATE()), 0),
    (2, '2024002', '李强', N'男', '2003-11-05', '计算机科学与技术学院', '软件工程', '2023级',
     '13800000002', 'liqiang@example.com', '$2b$10$HX5EJfqKh5phJ9nVHe2yBOztGoPMdwCpbDZQxLAC6f/49Lg7rsJi2',
     DATEADD(DAY, -35, GETDATE()), 0),
    (3, '2024003', '王芳', N'女', '2004-07-21', '经济管理学院', '市场营销', '2024级',
     '13800000003', 'wangfang@example.com', '$2b$10$yVK57u2F74kke2sxNXmXV.fbw4rE5CkL2hlQHEia9s.VYF2Y8NCbu',
     DATEADD(DAY, -30, GETDATE()), 0);
    SET IDENTITY_INSERT dbo.student OFF;
    PRINT N'学生测试数据 3 条插入完成。';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.employer)
BEGIN
    SET IDENTITY_INSERT dbo.employer ON;
    INSERT INTO dbo.employer (employer_id, emp_name, emp_contact, emp_phone, emp_address,
                              emp_intro, emp_email, emp_password, reg_date, emp_status)
    VALUES
    (1, '校园书吧', '陈立', '13900000001', '图书馆一层东侧',
     N'经营图书与自习空间，长期招聘兼职店员，工作时间灵活。', 'bookbar@example.com',
     '$2b$10$dLUOR6FftoYCZcnydOKol.4TGxYuIEHvwPGnVdT4yec7tQ26Q/qWy',
     DATEADD(DAY, -38, GETDATE()), 0),
    (2, '优鲜餐饮', '刘敏', '13900000002', '学生二食堂三层',
     N'校园连锁餐饮门店，主营快餐与饮品，招聘前台与后厨帮工。', 'youxian@example.com',
     '$2b$10$yqcXFsHfgvJ4QYJwSXeWeObfF.oLBYz7JApZfxTBJcRNGHPahggT2',
     DATEADD(DAY, -32, GETDATE()), 0);
    SET IDENTITY_INSERT dbo.employer OFF;
    PRINT N'商家测试数据 2 条插入完成。';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.job)
BEGIN
    SET IDENTITY_INSERT dbo.job ON;
    INSERT INTO dbo.job (job_id, employer_id, job_name, job_type, salary, salary_unit,
                         work_place, work_time, recruit_num, hired_num, job_desc,
                         publish_date, deadline, audit_status, job_status)
    VALUES
    (1, 1, '书店店员（周末班）', '营业员', 25.00, '元/小时', '校园书吧门店', '周六周日 9:00-18:00',
     3, 1, N'负责图书上架、收银与读者引导，可长期做。',
     DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, 20, CAST(GETDATE() AS DATE)), 1, 0),
    (2, 1, '自习室管理助理', '行政助理', 22.50, '元/小时', '图书馆自习区', '周一至周五 18:00-22:00',
     2, 0, N'负责自习区秩序维护、座位登记与设备巡检。',
     DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, 25, CAST(GETDATE() AS DATE)), 1, 0),
    (3, 2, '食堂前台服务员', '餐饮服务', 20.00, '元/小时', '学生二食堂三层', '午间 11:00-14:00',
     1, 1, N'负责点单、配餐与餐台清洁，提供工作餐。',
     DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, 10, CAST(GETDATE() AS DATE)), 1, 1),
    (4, 2, '校园推广专员', '促销推广', 120.00, '元/天', '校园内各宣传点', '按活动排班，每日 4 小时',
     5, 0, N'负责门店活动宣传、扫码引导与小礼品发放。',
     DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, 30, CAST(GETDATE() AS DATE)), 0, 0),
    (5, 2, '饮品制作学徒', '餐饮服务', 18.00, '元/小时', '学生二食堂饮品档口', '每日 15:00-19:00',
     2, 0, N'学习饮品制作与设备清洗，可培训上岗。',
     DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, -5, CAST(GETDATE() AS DATE)), 1, 2);
    SET IDENTITY_INSERT dbo.job OFF;
    PRINT N'岗位测试数据 5 条插入完成。';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.application)
BEGIN
    SET IDENTITY_INSERT dbo.application ON;
    INSERT INTO dbo.application (app_id, student_id, job_id, app_date, app_status, app_remark)
    VALUES
    (1, 1, 1, DATEADD(DAY, -9, GETDATE()), 0, N'周末两天均可出勤，希望录用。'),
    (2, 2, 1, DATEADD(DAY, -8, GETDATE()), 1, N'有书店兼职经验。'),
    (3, 3, 2, DATEADD(DAY, -6, GETDATE()), 2, N'晚间课程冲突，本次未通过。'),
    (4, 1, 3, DATEADD(DAY, -18, GETDATE()), 1, N'住校，午间可准时到岗。');
    SET IDENTITY_INSERT dbo.application OFF;
    PRINT N'申请测试数据 4 条插入完成。';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.comment)
BEGIN
    SET IDENTITY_INSERT dbo.comment ON;
    INSERT INTO dbo.comment (comment_id, student_id, job_id, employer_id, score, content,
                             comment_date, audit_status)
    VALUES
    (1, 2, 1, 1, 5, N'排班合理，店长很好沟通，工资按周结算，很守时。',
     DATEADD(DAY, -3, GETDATE()), 1),
    (2, 1, 3, 2, 4, N'工作量适中，提供工作餐，就是午高峰比较忙。',
     DATEADD(DAY, -2, GETDATE()), 0);
    SET IDENTITY_INSERT dbo.comment OFF;
    PRINT N'评价测试数据 2 条插入完成。';
END
GO

/* ---------- 9.3 最小权限数据库账号与授权 ----------
   说明：应用不得使用实例管理员账号连接数据库。cp_app_user 仅具备业务数据的读写权限
         与三个存储过程的执行权限，如需变更口令请在部署时替换占位符后单独执行本段。
         为安全起见，本段默认不执行，实施时按《运行说明.txt》第 3 节的步骤手工执行。
------------------------------------------------------------------------------ */
-- USE master;
-- GO
-- IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'cp_app_user')
--     CREATE LOGIN cp_app_user WITH PASSWORD = N'<部署时设定的强口令，16位以上>',
--         CHECK_POLICY = ON, DEFAULT_DATABASE = CampusPartTime;
-- GO
-- USE CampusPartTime;
-- GO
-- IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'cp_app_user')
--     CREATE USER cp_app_user FOR LOGIN cp_app_user;
-- GO
-- ALTER ROLE db_datareader ADD MEMBER cp_app_user;
-- ALTER ROLE db_datawriter ADD MEMBER cp_app_user;
-- GRANT EXECUTE ON dbo.proc_apply_job       TO cp_app_user;
-- GRANT EXECUTE ON dbo.proc_hire_applicant  TO cp_app_user;
-- GRANT EXECUTE ON dbo.proc_reject_applicant TO cp_app_user;
-- DENY ALTER, CONTROL, TAKE OWNERSHIP TO cp_app_user;
-- GO
-- PRINT N'最小权限账号 cp_app_user 创建并授权完成。';
-- GO

/* ---------- 9.4 执行结果验证 ---------- */
PRINT N'==== 各表行数核对 ====';
SELECT N'student'     AS 表名, COUNT(1) AS 行数 FROM dbo.student
UNION ALL SELECT N'employer',    COUNT(1) FROM dbo.employer
UNION ALL SELECT N'admin',       COUNT(1) FROM dbo.admin
UNION ALL SELECT N'job',         COUNT(1) FROM dbo.job
UNION ALL SELECT N'application', COUNT(1) FROM dbo.application
UNION ALL SELECT N'comment',     COUNT(1) FROM dbo.comment;

PRINT N'==== 约束清单核对 ====';
SELECT OBJECT_NAME(parent_object_id) AS 所属表, name AS 约束名, type_desc AS 约束类型
FROM sys.objects
WHERE parent_object_id IN (OBJECT_ID(N'dbo.student'), OBJECT_ID(N'dbo.employer'),
                           OBJECT_ID(N'dbo.admin'), OBJECT_ID(N'dbo.job'),
                           OBJECT_ID(N'dbo.application'), OBJECT_ID(N'dbo.comment'))
  AND type IN ('PK', 'F', 'U', 'C', 'D')
ORDER BY 所属表, 约束类型, 约束名;

PRINT N'==== 索引清单核对 ====';
SELECT OBJECT_NAME(i.object_id) AS 所属表, i.name AS 索引名, i.type_desc AS 索引类型
FROM sys.indexes AS i
WHERE i.object_id IN (OBJECT_ID(N'dbo.student'), OBJECT_ID(N'dbo.employer'),
                      OBJECT_ID(N'dbo.admin'), OBJECT_ID(N'dbo.job'),
                      OBJECT_ID(N'dbo.application'), OBJECT_ID(N'dbo.comment'))
  AND i.name IS NOT NULL
ORDER BY 所属表, 索引名;

PRINT N'==== 数据库对象清单（视图/存储过程/触发器） ====';
SELECT name AS 对象名, type_desc AS 对象类型
FROM sys.objects
WHERE type IN ('V', 'P', 'TR') AND is_ms_shipped = 0
ORDER BY type_desc, name;

PRINT N'==== 脚本执行结束 ====';
GO
