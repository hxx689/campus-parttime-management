using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 评价表数据访问类，对应 dbo.comment。
    /// </summary>
    public class CommentDAL
    {
        /// <summary>
        /// 新增评价。新评价的审核状态为 0（待审核），管理员审核通过后才公开展示。
        /// </summary>
        /// <param name="m">评价实体</param>
        /// <returns>受影响行数</returns>
        public int Insert(Comment m)
        {
            const string sql = @"INSERT INTO dbo.comment
                                     (student_id, job_id, employer_id, score, content, comment_date, audit_status)
                                 VALUES
                                     (@student_id, @job_id, @employer_id, @score, @content, GETDATE(), 0)";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@student_id", m.StudentId),
                new SqlParameter("@job_id", m.JobId),
                new SqlParameter("@employer_id", m.EmployerId),
                new SqlParameter("@score", m.Score),
                new SqlParameter("@content", (object)m.Content ?? DBNull.Value));
        }

        /// <summary>
        /// 判断学生对某岗位是否已评价（唯一约束 UQ_comment_stu_job 的前置校验）。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <returns>已评价返回 true</returns>
        public bool Exists(int studentId, int jobId)
        {
            object obj = SqlHelper.ExecuteScalar(
                "SELECT COUNT(1) FROM dbo.comment WHERE student_id = @student_id AND job_id = @job_id",
                new SqlParameter("@student_id", studentId),
                new SqlParameter("@job_id", jobId));
            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>
        /// 查询某岗位已审核通过的评价（岗位详情页公开展示）。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>评价数据表，含学生姓名与评分</returns>
        public DataTable GetApprovedByJob(int jobId)
        {
            const string sql = @"SELECT c.comment_id, s.stu_name, c.score, c.content, c.comment_date
                                 FROM dbo.comment AS c
                                 INNER JOIN dbo.student AS s ON c.student_id = s.student_id
                                 WHERE c.job_id = @job_id AND c.audit_status = 1
                                 ORDER BY c.comment_date DESC";
            return SqlHelper.ExecuteDataTable(sql, new SqlParameter("@job_id", jobId));
        }

        /// <summary>
        /// 查询待审核评价列表（管理员评价审核页签使用）。
        /// </summary>
        /// <returns>评价数据表</returns>
        public DataTable GetPendingAudit()
        {
            const string sql = @"SELECT c.comment_id, s.stu_name, j.job_name, e.emp_name,
                                        c.score, c.content, c.comment_date
                                 FROM dbo.comment AS c
                                 INNER JOIN dbo.student AS s ON c.student_id = s.student_id
                                 INNER JOIN dbo.job AS j ON c.job_id = j.job_id
                                 INNER JOIN dbo.employer AS e ON c.employer_id = e.employer_id
                                 WHERE c.audit_status = 0
                                 ORDER BY c.comment_date DESC";
            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 审核评价。
        /// </summary>
        /// <param name="commentId">评价编号</param>
        /// <param name="auditStatus">审核结果（1 通过 / 2 驳回）</param>
        /// <returns>受影响行数</returns>
        public int Audit(int commentId, byte auditStatus)
        {
            const string sql = "UPDATE dbo.comment SET audit_status = @audit_status WHERE comment_id = @comment_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@audit_status", auditStatus),
                new SqlParameter("@comment_id", commentId));
        }

        /// <summary>
        /// 统计评价总数。
        /// </summary>
        /// <returns>评价总数</returns>
        public int CountAll()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(1) FROM dbo.comment"));
        }
    }
}
