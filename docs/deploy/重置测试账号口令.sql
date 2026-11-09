/* =====================================================================================
   校园兼职招聘管理系统 —— 测试账号口令统一重置脚本（辅助脚本，可选执行）
   -------------------------------------------------------------------------------------
   用    途 : CampusPartTime.sql 第 9 段预置的 BCrypt 哈希由课程环境生成，其明文口令
              不在脚本中出现。若首次部署后无法确认预置账号口令，执行本脚本可将全部
              测试账号统一重置为下述演示口令，便于验收演示。
   演示口令 : Cp@20240048
   适用账号 : 管理员 admin；学生 2024001 / 2024002 / 2024003；
              商家 13900000001 / 13900000002
   安全说明 : 1. 库中仍只存 BCrypt 哈希（cost = 10，22 位随机盐），不存明文；
              2. 本脚本仅供课程验收演示使用，生产部署前必须改为强口令并妥善保管；
              3. 脚本可重复执行，多次执行只会把哈希替换为同一条新哈希，不影响登录。
   执行方式 : SSMS 连接实例后，选中 CampusPartTime 数据库，按 F5 整体执行。
   ===================================================================================== */

USE CampusPartTime;
GO

DECLARE @demo_hash VARCHAR(60) = '$2b$10$XeF/l8CPG/78nsx8/xw.Te4kQ2lde2r3JZvb0jwOknPXbPnpr3a9e';

/* ---------- 1. 管理员账号 ---------- */
UPDATE dbo.admin
SET    admin_password = @demo_hash
WHERE  admin_name = 'admin';
PRINT N'管理员 admin 口令已重置，影响行数：' + CAST(@@ROWCOUNT AS VARCHAR(10));

/* ---------- 2. 学生测试账号 ---------- */
UPDATE dbo.student
SET    stu_password = @demo_hash
WHERE  stu_no IN ('2024001', '2024002', '2024003');
PRINT N'学生测试账号口令已重置，影响行数：' + CAST(@@ROWCOUNT AS VARCHAR(10));

/* ---------- 3. 商家测试账号 ---------- */
UPDATE dbo.employer
SET    emp_password = @demo_hash
WHERE  emp_phone IN ('13900000001', '13900000002');
PRINT N'商家测试账号口令已重置，影响行数：' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

/* ---------- 4. 结果验证：哈希长度应为 60 位，且均已不是明文 ---------- */
SELECT '管理员' AS 角色类别, admin_name AS 登录账号, LEN(admin_password) AS 哈希长度 FROM dbo.admin
UNION ALL
SELECT '学生', stu_no, LEN(stu_password) FROM dbo.student
UNION ALL
SELECT '商家', emp_phone, LEN(emp_password) FROM dbo.employer;
GO
