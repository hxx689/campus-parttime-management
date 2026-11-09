using System.Data;
using CampusPartTime.DAL;

namespace CampusPartTime.BLL
{
    /// <summary>
    /// 系统管理业务逻辑类：用户账号审核与禁用、首页与后台数据统计。
    /// </summary>
    public class AdminBLL
    {
        private readonly StudentDAL _studentDal = new StudentDAL();
        private readonly EmployerDAL _employerDal = new EmployerDAL();
        private readonly JobDAL _jobDal = new JobDAL();
        private readonly ApplicationDAL _appDal = new ApplicationDAL();
        private readonly CommentDAL _commentDal = new CommentDAL();

        /// <summary>
        /// 查询全部学生。
        /// </summary>
        /// <returns>学生数据表</returns>
        public DataTable GetAllStudents()
        {
            return _studentDal.GetAll();
        }

        /// <summary>
        /// 查询全部商家。
        /// </summary>
        /// <returns>商家数据表</returns>
        public DataTable GetAllEmployers()
        {
            return _employerDal.GetAll();
        }

        /// <summary>
        /// 修改学生账号状态。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="disabled">true 禁用，false 恢复正常</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool SetStudentStatus(int studentId, bool disabled, out string message)
        {
            int rows = _studentDal.UpdateStatus(studentId, disabled ? (byte)1 : (byte)0);
            message = rows > 0 ? (disabled ? "该学生账号已禁用。" : "该学生账号已恢复正常。") : "操作失败：学生不存在。";
            return rows > 0;
        }

        /// <summary>
        /// 修改商家账号状态。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <param name="disabled">true 禁用，false 恢复正常</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool SetEmployerStatus(int employerId, bool disabled, out string message)
        {
            int rows = _employerDal.UpdateStatus(employerId, disabled ? (byte)1 : (byte)0);
            message = rows > 0 ? (disabled ? "该商家账号已禁用。" : "该商家账号已恢复正常。") : "操作失败：商家不存在。";
            return rows > 0;
        }

        /// <summary>
        /// 数据统计：一次取回注册用户数、在招岗位数与申请记录数。
        /// </summary>
        /// <param name="studentCount">输出参数：学生注册数</param>
        /// <param name="employerCount">输出参数：商家注册数</param>
        /// <param name="onSaleJobCount">输出参数：在招岗位数</param>
        /// <param name="appCount">输出参数：申请记录数</param>
        /// <param name="commentCount">输出参数：评价总数</param>
        public void GetStatistics(out int studentCount, out int employerCount,
            out int onSaleJobCount, out int appCount, out int commentCount)
        {
            studentCount = _studentDal.CountAll();
            employerCount = _employerDal.CountAll();
            onSaleJobCount = _jobDal.CountOnSale();
            appCount = _appDal.CountAll();
            commentCount = _commentDal.CountAll();
        }
    }
}
