using System.Data;
using CampusPartTime.DAL;

namespace CampusPartTime.BLL
{
    /// <summary>
    /// 申请业务逻辑类：把存储过程返回码翻译为业务提示，并屏蔽界面层对返回码语义的依赖。
    /// </summary>
    public class ApplicationBLL
    {
        private readonly ApplicationDAL _dal = new ApplicationDAL();

        /// <summary>
        /// 学生申请岗位。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>申请成功返回 true</returns>
        public bool Apply(int studentId, int jobId, out string message)
        {
            int code = _dal.Apply(studentId, jobId);
            switch (code)
            {
                case 1:
                    message = "申请成功，请等待商家处理。";
                    return true;
                case -1:
                    message = "岗位不存在或尚未通过审核。";
                    return false;
                case -2:
                    message = "该岗位已招满、已下架或已过报名截止日期。";
                    return false;
                case -3:
                    message = "您已申请过该岗位，请勿重复申请。";
                    return false;
                case -4:
                    message = "申请失败，请稍后重试。";
                    return false;
                default:
                    message = "申请失败，系统返回未知状态码：" + code + "。";
                    return false;
            }
        }

        /// <summary>
        /// 商家录用申请。
        /// </summary>
        /// <param name="appId">申请编号</param>
        /// <param name="employerId">商家编号</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool Hire(int appId, int employerId, out string message)
        {
            int code = _dal.Hire(appId, employerId);
            switch (code)
            {
                case 1:
                    message = "已录用该学生。";
                    return true;
                case -1:
                    message = "申请记录不存在或不属于本商家。";
                    return false;
                case -2:
                    message = "该申请已处理，请勿重复操作。";
                    return false;
                case -3:
                    message = "岗位已招满或不在招，无法继续录用。";
                    return false;
                case -4:
                    message = "录用失败，请稍后重试。";
                    return false;
                default:
                    message = "录用失败，系统返回未知状态码：" + code + "。";
                    return false;
            }
        }

        /// <summary>
        /// 商家拒绝申请。
        /// </summary>
        /// <param name="appId">申请编号</param>
        /// <param name="employerId">商家编号</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool Reject(int appId, int employerId, out string message)
        {
            int code = _dal.Reject(appId, employerId);
            if (code == 1)
            {
                message = "已拒绝该申请。";
                return true;
            }
            message = "拒绝失败：申请记录不存在、不属于本商家或已处理。";
            return false;
        }

        /// <summary>
        /// 查询商家收到的全部申请。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <returns>申请数据表</returns>
        public DataTable GetByEmployer(int employerId)
        {
            return _dal.GetByEmployer(employerId);
        }

        /// <summary>
        /// 查询学生的全部申请。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <returns>申请数据表</returns>
        public DataTable GetByStudent(int studentId)
        {
            return _dal.GetByStudent(studentId);
        }

        /// <summary>
        /// 查询学生对指定岗位的申请状态。
        /// </summary>
        /// <param name="studentId">学生编号</param>
        /// <param name="jobId">岗位编号</param>
        /// <returns>-1 未申请；0 待处理；1 已录用；2 已拒绝</returns>
        public int GetStatus(int studentId, int jobId)
        {
            return _dal.GetStatus(studentId, jobId);
        }
    }
}
