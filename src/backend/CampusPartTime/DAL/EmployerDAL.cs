using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 商家表数据访问类，对应 dbo.employer。
    /// </summary>
    public class EmployerDAL
    {
        /// <summary>
        /// 按联系电话查询商家（商家登录使用手机号作为账号）。
        /// </summary>
        /// <param name="phone">联系电话</param>
        /// <returns>商家实体；不存在返回 null</returns>
        public Employer GetByPhone(string phone)
        {
            const string sql = @"SELECT employer_id, emp_name, emp_contact, emp_phone, emp_address,
                                        emp_intro, emp_email, emp_password, reg_date, emp_status
                                 FROM dbo.employer WHERE emp_phone = @emp_phone";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@emp_phone", phone));
            return dt.Rows.Count > 0 ? Map(dt.Rows[0]) : null;
        }

        /// <summary>
        /// 按主键查询商家。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <returns>商家实体；不存在返回 null</returns>
        public Employer GetById(int employerId)
        {
            const string sql = @"SELECT employer_id, emp_name, emp_contact, emp_phone, emp_address,
                                        emp_intro, emp_email, emp_password, reg_date, emp_status
                                 FROM dbo.employer WHERE employer_id = @employer_id";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@employer_id", employerId));
            return dt.Rows.Count > 0 ? Map(dt.Rows[0]) : null;
        }

        /// <summary>
        /// 判断联系电话是否已注册。
        /// </summary>
        /// <param name="phone">联系电话</param>
        /// <returns>已存在返回 true</returns>
        public bool ExistsPhone(string phone)
        {
            object obj = SqlHelper.ExecuteScalar(
                "SELECT COUNT(1) FROM dbo.employer WHERE emp_phone = @emp_phone",
                new SqlParameter("@emp_phone", phone));
            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>
        /// 新增商家。
        /// </summary>
        /// <param name="m">商家实体，EmpPassword 须为 BCrypt 哈希</param>
        /// <returns>新增记录的主键，失败返回 0</returns>
        public int Insert(Employer m)
        {
            const string sql = @"INSERT INTO dbo.employer
                                     (emp_name, emp_contact, emp_phone, emp_address, emp_intro,
                                      emp_email, emp_password, reg_date, emp_status)
                                 VALUES
                                     (@emp_name, @emp_contact, @emp_phone, @emp_address, @emp_intro,
                                      @emp_email, @emp_password, GETDATE(), 0);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object id = SqlHelper.ExecuteScalar(sql,
                new SqlParameter("@emp_name", m.EmpName),
                new SqlParameter("@emp_contact", m.EmpContact),
                new SqlParameter("@emp_phone", m.EmpPhone),
                new SqlParameter("@emp_address", (object)m.EmpAddress ?? DBNull.Value),
                new SqlParameter("@emp_intro", (object)m.EmpIntro ?? DBNull.Value),
                new SqlParameter("@emp_email", (object)m.EmpEmail ?? DBNull.Value),
                new SqlParameter("@emp_password", m.EmpPassword));
            return id == null ? 0 : Convert.ToInt32(id);
        }

        /// <summary>
        /// 查询商家列表（管理员用户审核页签使用）。
        /// </summary>
        /// <returns>商家数据表</returns>
        public DataTable GetAll()
        {
            const string sql = @"SELECT employer_id, emp_name, emp_contact, emp_phone, emp_address,
                                        reg_date, emp_status
                                 FROM dbo.employer ORDER BY reg_date DESC";
            return SqlHelper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 修改商家账号状态（0 正常 / 1 禁用）。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <param name="status">目标状态</param>
        /// <returns>受影响行数</returns>
        public int UpdateStatus(int employerId, byte status)
        {
            const string sql = "UPDATE dbo.employer SET emp_status = @status WHERE employer_id = @employer_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@status", status),
                new SqlParameter("@employer_id", employerId));
        }

        /// <summary>
        /// 统计商家总数。
        /// </summary>
        /// <returns>商家总数</returns>
        public int CountAll()
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(1) FROM dbo.employer"));
        }

        /// <summary>
        /// 把数据行映射为商家实体。
        /// </summary>
        private static Employer Map(DataRow row)
        {
            return new Employer
            {
                EmployerId = Convert.ToInt32(row["employer_id"]),
                EmpName = SqlHelper.GetString(row, "emp_name"),
                EmpContact = SqlHelper.GetString(row, "emp_contact"),
                EmpPhone = SqlHelper.GetString(row, "emp_phone"),
                EmpAddress = SqlHelper.GetString(row, "emp_address"),
                EmpIntro = SqlHelper.GetString(row, "emp_intro"),
                EmpEmail = SqlHelper.GetString(row, "emp_email"),
                EmpPassword = SqlHelper.GetString(row, "emp_password"),
                RegDate = Convert.ToDateTime(row["reg_date"]),
                EmpStatus = Convert.ToByte(row["emp_status"])
            };
        }
    }
}
