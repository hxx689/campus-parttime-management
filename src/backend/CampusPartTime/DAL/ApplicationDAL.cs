using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 申请记录表数据访问类，对应 dbo.application。
    /// 申请、录用、拒绝三类写操作均通过存储过程完成，保证事务与并发一致性。
    /// </summary>
    public class ApplicationDAL
    {
        /// <summary>
        /// 学生申请岗位，调用存储过程 proc_apply_job。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <returns>1 成功；-1 岗位不存在或未通过审核；-2 不在招或已过截止日期；
        /// -3 已申请过；-4 写入失败</returns>
        public int Apply(int studentId, int jobId)
        {
            SqlParameter[] ps =
            {
                new SqlParameter("@student_id", studentId),
                new SqlParameter("@job_id", jobId),
                SqlHelper.CreateOutputParameter("@result")
            };
            return SqlHelper.ExecuteProcedure("dbo.proc_apply_job", ps, "@result");
        }

        /// <summary>
        /// 商家录用申请，调用存储过程 proc_hire_applicant。
        /// </summary>
        /// <param name="appId">申请编号</param>
        /// <param name="employerId">商家编号</param>
        /// <returns>1 成功；-1 申请不存在或不属于该商家；-2 已处理；-3 岗位已招满或不在招；-4 执行异常</returns>
        public int Hire(int appId, int employerId)
        {
            SqlParameter[] ps =
            {
                new SqlParameter("@app_id", appId),
                new SqlParameter("@employer_id", employerId),
                SqlHelper.CreateOutputParameter("@result")
            };
            return SqlHelper.ExecuteProcedure("dbo.proc_hire_applicant", ps, "@result");
        }

        /// <summary>
        /// 商家拒绝申请，调用存储过程 proc_reject_applicant。
        /// </summary>
        /// <param name="appId">申请编号</param>
        /// <param name="employerId">商家编号</param>
        /// <returns>1 成功；-1 申请不存在、不属于该商家或已处理</returns>
        public int Reject(int appId, int employerId)
        {
            SqlParameter[] ps =
            {
                new SqlParameter("@app_id", appId),
                new SqlParameter("@employer_id", employerId),
                SqlHelper.CreateOutputParameter("@result")
            };
            return SqlHelper.ExecuteProcedure("dbo.proc_reject_applicant", ps, "@result");
        }

        /// <summary>
        /// 按商家查询其全部岗位收到的申请（申请管理页使用）。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <returns>申请数据表，含学生姓名、学号与岗位名称</returns>
        public DataTable GetByEmployer(int employerId)
        {
            const string sql = @"SELECT a.app_id, a.job_id, j.job_name, s.stu_name, s.stu_no,
                                        s.stu_phone, a.app_date, a.app_status, a.app_remark,
                                        j.recruit_num, j.hired_num
                                 FROM dbo.application AS a
                                 INNER JOIN dbo.job AS j ON a.job_id = j.job_id
                                 INNER JOIN dbo.student AS s ON a.student_id = s.student_id
                                 WHERE j.employer_id = @employer_id
                                 ORDER BY a.app_date DESC";
            return SqlHelper.ExecuteDataTable(sql, new SqlParameter("@employer_id", employerId));
        }

        /// <summary>
        /// 按学生查询其全部申请（我的申请页使用）。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <returns>申请数据表，含岗位名称、商家名称与岗位状态</returns>
        public DataTable GetByStudent(int studentId)
        {
            const string sql = @"SELECT a.app_id, a.job_id, j.job_name, j.job_type, j.salary,
                                        j.salary_unit, e.emp_name, j.job_status,
                                        a.app_date, a.app_status, a.app_remark
                                 FROM dbo.application AS a
                                 INNER JOIN dbo.job AS j ON a.job_id = j.job_id
                                 INNER JOIN dbo.employer AS e ON j.employer_id = e.employer_id
                                 WHERE a.student_id = @student_id
                                 ORDER BY a.app_date DESC";
            return SqlHelper.ExecuteDataTable(sql, new SqlParameter("@student_id", studentId));
        }

        /// <summary>
        /// 查询指定学生与岗位的申请状态，用于详情页显示按钮或“已申请”提示。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <returns>申请状态；未申请返回 -1</returns>
        public int GetStatus(int studentId, int jobId)
        {
            object obj = SqlHelper.ExecuteScalar(
                "SELECT app_status FROM dbo.application WHERE student_id = @student_id AND job_id = @job_id",
                new SqlParameter("@student_id", studentId),
                new SqlParameter("@job_id", jobId));
            return obj == null || obj == DBNull.Value ? -1 : Convert.ToInt32(obj);
        }

        /// <summary>
        /// 判断学生是否已被该岗位录用（评价前置条件）。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <returns>已录用返回 true</returns>
        public bool IsHired(int studentId, int jobId)
        {
            object obj = SqlHelper.ExecuteScalar(
                @"SELECT COUNT(1) FROM dbo.application
                  WHERE student_id = @student_id AND job_id = @job_id AND app_status = 1",
                new SqlParameter("@student_id", studentId),
                new SqlParameter("@job_id", jobId));
            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>
        /// 统计申请记录总数（管理员数据统计使用）。
        /// </summary>
        /// <returns>申请记录总数</returns>
        public int CountAll()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(1) FROM dbo.application"));
        }
    }
}
