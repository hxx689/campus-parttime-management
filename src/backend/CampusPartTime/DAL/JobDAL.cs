using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 岗位表数据访问类，对应 dbo.job 与视图 dbo.v_job_detail。
    /// </summary>
    public class JobDAL
    {
        /// <summary>
        /// 发布岗位。新增记录的审核状态与岗位状态均为 0（待审核 / 在招）。
        /// </summary>
        /// <param name="m">岗位实体</param>
        /// <returns>新增记录的主键，失败返回 0</returns>
        public int Insert(Job m)
        {
            const string sql = @"INSERT INTO dbo.job
                                     (employer_id, job_name, job_type, salary, salary_unit, work_place,
                                      work_time, recruit_num, job_desc, publish_date, deadline,
                                      audit_status, job_status)
                                 VALUES
                                     (@employer_id, @job_name, @job_type, @salary, @salary_unit, @work_place,
                                      @work_time, @recruit_num, @job_desc, GETDATE(), @deadline, 0, 0);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object id = SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@employer_id", m.EmployerId),
                new SqlParameter("@job_name", m.JobName),
                new SqlParameter("@job_type", m.JobType),
                new SqlParameter("@salary", m.Salary),
                new SqlParameter("@salary_unit", m.SalaryUnit),
                new SqlParameter("@work_place", m.WorkPlace),
                new SqlParameter("@work_time", m.WorkTime),
                new SqlParameter("@recruit_num", m.RecruitNum),
                new SqlParameter("@job_desc", (object)m.JobDesc ?? DBNull.Value),
                new SqlParameter("@deadline", m.Deadline));
            return id == null ? 0 : Convert.ToInt32(id);
        }

        /// <summary>
        /// 修改岗位信息。修改后审核状态重置为待审核（0），需管理员重新审核。
        /// 条件中包含 employer_id，防止越权修改他商家的岗位。
        /// </summary>
        /// <param name="m">岗位实体</param>
        /// <returns>受影响行数，0 表示无权限或岗位不存在</returns>
        public int Update(Job m)
        {
            const string sql = @"UPDATE dbo.job
                                 SET job_name = @job_name, job_type = @job_type, salary = @salary,
                                     salary_unit = @salary_unit, work_place = @work_place,
                                     work_time = @work_time, recruit_num = @recruit_num,
                                     job_desc = @job_desc, deadline = @deadline, audit_status = 0
                                 WHERE job_id = @job_id AND employer_id = @employer_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@job_name", m.JobName),
                new SqlParameter("@job_type", m.JobType),
                new SqlParameter("@salary", m.Salary),
                new SqlParameter("@salary_unit", m.SalaryUnit),
                new SqlParameter("@work_place", m.WorkPlace),
                new SqlParameter("@work_time", m.WorkTime),
                new SqlParameter("@recruit_num", m.RecruitNum),
                new SqlParameter("@job_desc", (object)m.JobDesc ?? DBNull.Value),
                new SqlParameter("@deadline", m.Deadline),
                new SqlParameter("@job_id", m.JobId),
                new SqlParameter("@employer_id", m.EmployerId));
        }

        /// <summary>
        /// 商家下架岗位（逻辑删除，岗位状态置 2）。
        /// 保留历史申请与评价数据，不做物理删除。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <param name="employerId">商家编号</param>
        /// <returns>受影响行数，0 表示无权限或岗位不存在</returns>
        public int UpdateStatusToOffline(int jobId, int employerId)
        {
            const string sql = @"UPDATE dbo.job SET job_status = 2
                                 WHERE job_id = @job_id AND employer_id = @employer_id AND job_status <> 2";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@job_id", jobId),
                new SqlParameter("@employer_id", employerId));
        }

        /// <summary>
        /// 管理员审核岗位。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <param name="auditStatus">审核结果（1 通过 / 2 驳回）</param>
        /// <returns>受影响行数</returns>
        public int Audit(int jobId, byte auditStatus)
        {
            const string sql = "UPDATE dbo.job SET audit_status = @audit_status WHERE job_id = @job_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@audit_status", auditStatus),
                new SqlParameter("@job_id", jobId));
        }

        /// <summary>
        /// 按主键查询岗位（商家编辑页回填使用，不限定审核状态）。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>岗位实体；不存在返回 null</returns>
        public Job GetById(int jobId)
        {
            const string sql = @"SELECT job_id, employer_id, job_name, job_type, salary, salary_unit,
                                        work_place, work_time, recruit_num, hired_num, job_desc,
                                        publish_date, deadline, audit_status, job_status
                                 FROM dbo.job WHERE job_id = @job_id";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@job_id", jobId));
            return dt.Rows.Count > 0 ? Map(dt.Rows[0]) : null;
        }

        /// <summary>
        /// 按商家查询其全部岗位。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <returns>岗位数据表</returns>
        public DataTable GetByEmployer(int employerId)
        {
            const string sql = @"SELECT job_id, job_name, job_type, salary, salary_unit, work_place,
                                        work_time, recruit_num, hired_num, publish_date, deadline,
                                        audit_status, job_status
                                 FROM dbo.job WHERE employer_id = @employer_id
                                 ORDER BY publish_date DESC";
            return SqlHelper.ExecuteDataTable(sql, new SqlParameter("@employer_id", employerId));
        }

        /// <summary>
        /// 学生条件检索岗位，使用 OFFSET-FETCH 实现服务器端分页。
        /// </summary>
        /// <param name="jobType">岗位类型，空串表示不限</param>
        /// <param name="minSalary">最低薪资，null 表示不限</param>
        /// <param name="keyword">岗位名称关键词，空串表示不限</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页行数</param>
        /// <returns>当前页岗位数据表，列为视图 v_job_detail 的 17 列</returns>
        public DataTable GetJobPage(string jobType, decimal? minSalary, string keyword, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT job_id, job_name, job_type, salary, salary_unit, work_place,
                                        work_time, recruit_num, hired_num, job_desc, publish_date,
                                        deadline, job_status, employer_id, emp_name, emp_contact, emp_phone
                                 FROM dbo.v_job_detail
                                 WHERE (@job_type = '' OR job_type = @job_type)
                                   AND (@salary IS NULL OR salary >= @salary)
                                   AND (@kw = '' OR job_name LIKE '%' + @kw + '%')
                                   AND job_status = 0
                                 ORDER BY publish_date DESC
                                 OFFSET (@page_index - 1) * @page_size ROWS
                                 FETCH NEXT @page_size ROWS ONLY";
            return SqlHelper.ExecuteDataTable(sql,
                new SqlParameter("@job_type", jobType ?? string.Empty),
                new SqlParameter("@salary", (object)minSalary ?? DBNull.Value),
                new SqlParameter("@kw", keyword ?? string.Empty),
                new SqlParameter("@page_index", pageIndex),
                new SqlParameter("@page_size", pageSize));
        }

        /// <summary>
        /// 统计符合条件的岗位总数，用于分页控件计算总页数。
        /// </summary>
        /// <param name="jobType">岗位类型，空串表示不限</param>
        /// <param name="minSalary">最低薪资，null 表示不限</param>
        /// <param name="keyword">岗位名称关键词，空串表示不限</param>
        /// <returns>记录总数</returns>
        public int CountJob(string jobType, decimal? minSalary, string keyword)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.v_job_detail
                                 WHERE (@job_type = '' OR job_type = @job_type)
                                   AND (@salary IS NULL OR salary >= @salary)
                                   AND (@kw = '' OR job_name LIKE '%' + @kw + '%')
                                   AND job_status = 0";
            object obj = SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@job_type", jobType ?? string.Empty),
                new SqlParameter("@salary", (object)minSalary ?? DBNull.Value),
                new SqlParameter("@kw", keyword ?? string.Empty));
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 查询待审核岗位列表（管理员岗位审核页签使用）。
        /// </summary>
        /// <returns>岗位数据表</returns>
        public DataTable GetPendingAudit()
        {
            const string sql = @"SELECT j.job_id, j.job_name, j.job_type, j.salary, j.salary_unit,
                                        j.work_place, j.work_time, j.recruit_num, j.publish_date,
                                        e.emp_name
                                 FROM dbo.job AS j
                                 INNER JOIN dbo.employer AS e ON j.employer_id = e.employer_id
                                 WHERE j.audit_status = 0
                                 ORDER BY j.publish_date DESC";
            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 统计在招岗位数（管理员数据统计与首页展示使用）。
        /// </summary>
        /// <returns>在招岗位数</returns>
        public int CountOnSale()
        {
            object obj = SqlHelper.ExecuteScalar(
                "SELECT COUNT(1) FROM dbo.job WHERE audit_status = 1 AND job_status = 0");
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 统计岗位总数。
        /// </summary>
        /// <returns>岗位总数</returns>
        public int CountAll()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(1) FROM dbo.job"));
        }

        /// <summary>
        /// 把数据行映射为岗位实体。
        /// </summary>
        private static Job Map(DataRow row)
        {
            return new Job
            {
                JobId = Convert.ToInt32(row["job_id"]),
                EmployerId = Convert.ToInt32(row["employer_id"]),
                JobName = SqlHelper.GetString(row, "job_name"),
                JobType = SqlHelper.GetString(row, "job_type"),
                Salary = Convert.ToDecimal(row["salary"]),
                SalaryUnit = SqlHelper.GetString(row, "salary_unit"),
                WorkPlace = SqlHelper.GetString(row, "work_place"),
                WorkTime = SqlHelper.GetString(row, "work_time"),
                RecruitNum = Convert.ToInt32(row["recruit_num"]),
                HiredNum = Convert.ToInt32(row["hired_num"]),
                JobDesc = SqlHelper.GetString(row, "job_desc"),
                PublishDate = Convert.ToDateTime(row["publish_date"]),
                Deadline = Convert.ToDateTime(row["deadline"]),
                AuditStatus = Convert.ToByte(row["audit_status"]),
                JobStatus = Convert.ToByte(row["job_status"])
            };
        }

        /// <summary>
        /// 按岗位编号查询视图 v_job_detail 的单条完整信息（含商家名称与联系方式）。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>含 17 列的单行数据表，无匹配时返回空表</returns>
        public DataTable GetDetailView(int jobId)
        {
            const string sql = @"SELECT job_id, job_name, job_type, salary, salary_unit, work_place,
                                        work_time, recruit_num, hired_num, job_desc, publish_date,
                                        deadline, job_status, employer_id, emp_name, emp_contact, emp_phone
                                 FROM dbo.v_job_detail
                                 WHERE job_id = @job_id";
            return SqlHelper.ExecuteDataTable(sql, new SqlParameter("@job_id", jobId));
        }
    }
}
