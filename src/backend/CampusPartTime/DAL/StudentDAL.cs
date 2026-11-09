using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 学生表数据访问类，对应 dbo.student。
    /// </summary>
    public class StudentDAL
    {
        /// <summary>
        /// 按学号查询学生。
        /// </summary>
        /// <param name="stuNo">学号</param>
        /// <returns>学生实体；不存在返回 null</returns>
        public Student GetByNo(string stuNo)
        {
            const string sql = @"SELECT student_id, stu_no, stu_name, stu_sex, stu_birth, stu_college,
                                        stu_major, stu_grade, stu_phone, stu_email, stu_password,
                                        reg_date, stu_status
                                 FROM dbo.student WHERE stu_no = @stu_no";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@stu_no", stuNo));
            return dt.Rows.Count > 0 ? Map(dt.Rows[0]) : null;
        }

        /// <summary>
        /// 按主键查询学生。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <returns>学生实体；不存在返回 null</returns>
        public Student GetById(int studentId)
        {
            const string sql = @"SELECT student_id, stu_no, stu_name, stu_sex, stu_birth, stu_college,
                                        stu_major, stu_grade, stu_phone, stu_email, stu_password,
                                        reg_date, stu_status
                                 FROM dbo.student WHERE student_id = @student_id";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@student_id", studentId));
            return dt.Rows.Count > 0 ? Map(dt.Rows[0]) : null;
        }

        /// <summary>
        /// 判断学号或手机号是否已被占用。
        /// </summary>
        /// <param name="stuNo">学号</param>
        /// <param name="phone">手机号，可为空</param>
        /// <returns>已存在返回 true</returns>
        public bool ExistsNoOrPhone(string stuNo, string phone)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.student
                                 WHERE stu_no = @stu_no OR (@phone IS NOT NULL AND stu_phone = @phone)";
            object obj = SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@stu_no", stuNo),
                new SqlParameter("@phone", (object)phone ?? DBNull.Value));
            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>
        /// 新增学生。
        /// </summary>
        /// <param name="m">学生实体，StuPassword 须为 BCrypt 哈希</param>
        /// <returns>新增记录的主键，失败返回 0</returns>
        public int Insert(Student m)
        {
            const string sql = @"INSERT INTO dbo.student
                                     (stu_no, stu_name, stu_sex, stu_birth, stu_college, stu_major,
                                      stu_grade, stu_phone, stu_email, stu_password, reg_date, stu_status)
                                 VALUES
                                     (@stu_no, @stu_name, @stu_sex, @stu_birth, @stu_college, @stu_major,
                                      @stu_grade, @stu_phone, @stu_email, @stu_password, GETDATE(), 0);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object id = SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@stu_no", m.StuNo),
                new SqlParameter("@stu_name", m.StuName),
                new SqlParameter("@stu_sex", m.StuSex),
                new SqlParameter("@stu_birth", (object)m.StuBirth ?? DBNull.Value),
                new SqlParameter("@stu_college", m.StuCollege),
                new SqlParameter("@stu_major", m.StuMajor),
                new SqlParameter("@stu_grade", m.StuGrade),
                new SqlParameter("@stu_phone", (object)m.StuPhone ?? DBNull.Value),
                new SqlParameter("@stu_email", (object)m.StuEmail ?? DBNull.Value),
                new SqlParameter("@stu_password", m.StuPassword));
            return id == null ? 0 : Convert.ToInt32(id);
        }

        /// <summary>
        /// 查询学生列表（管理员用户审核页签使用）。
        /// </summary>
        /// <returns>学生数据表</returns>
        public DataTable GetAll()
        {
            const string sql = @"SELECT student_id, stu_no, stu_name, stu_sex, stu_college, stu_major,
                                        stu_grade, stu_phone, reg_date, stu_status
                                 FROM dbo.student ORDER BY reg_date DESC";
            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 修改学生账号状态（0 正常 / 1 禁用）。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="status">目标状态</param>
        /// <returns>受影响行数</returns>
        public int UpdateStatus(int studentId, byte status)
        {
            const string sql = "UPDATE dbo.student SET stu_status = @status WHERE student_id = @student_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@status", status),
                new SqlParameter("@student_id", studentId));
        }

        /// <summary>
        /// 统计学生总数（管理员数据统计使用）。
        /// </summary>
        /// <returns>学生总数</returns>
        public int CountAll()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(1) FROM dbo.student"));
        }

        /// <summary>
        /// 把数据行映射为学生实体。
        /// </summary>
        private static Student Map(DataRow row)
        {
            return new Student
            {
                StudentId = Convert.ToInt32(row["student_id"]),
                StuNo = SqlHelper.GetString(row, "stu_no"),
                StuName = SqlHelper.GetString(row, "stu_name"),
                StuSex = SqlHelper.GetString(row, "stu_sex"),
                StuBirth = row["stu_birth"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["stu_birth"]),
                StuCollege = SqlHelper.GetString(row, "stu_college"),
                StuMajor = SqlHelper.GetString(row, "stu_major"),
                StuGrade = SqlHelper.GetString(row, "stu_grade"),
                StuPhone = SqlHelper.GetString(row, "stu_phone"),
                StuEmail = SqlHelper.GetString(row, "stu_email"),
                StuPassword = SqlHelper.GetString(row, "stu_password"),
                RegDate = Convert.ToDateTime(row["reg_date"]),
                StuStatus = Convert.ToByte(row["stu_status"])
            };
        }
    }
}
