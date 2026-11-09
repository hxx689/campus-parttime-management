using System.Data;
using CampusPartTime.DAL;
using CampusPartTime.Model;

namespace CampusPartTime.BLL
{
    /// <summary>
    /// 评价业务逻辑类：评价提交前置校验与管理员审核。
    /// </summary>
    public class CommentBLL
    {
        private readonly CommentDAL _dal = new CommentDAL();
        private readonly ApplicationDAL _appDal = new ApplicationDAL();

        /// <summary>
        /// 提交评价。业务规则：
        /// ① 只有申请状态为已录用（app_status=1）的学生才能评价该岗位；
        /// ② 同一学生对同一岗位只能评价一次（对应唯一约束 UQ_comment_stu_job）；
        /// ③ 评分必须是 1～5 的整数；④ 新评价进入待审核状态。
        /// </summary>
        /// <param name="m">评价实体</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>提交成功返回 true</returns>
        public bool Submit(Comment m, out string message)
        {
            if (m.Score < 1 || m.Score > 5)
            {
                message = "评分必须是 1 到 5 之间的整数。";
                return false;
            }
            if (!_appDal.IsHired(m.StudentId, m.JobId))
            {
                message = "只有被该岗位录用的学生才能提交评价。";
                return false;
            }
            if (_dal.Exists(m.StudentId, m.JobId))
            {
                message = "您已评价过该岗位，请勿重复提交。";
                return false;
            }
            int rows = _dal.Insert(m);
            if (rows <= 0)
            {
                message = "评价提交失败，请稍后重试。";
                return false;
            }
            message = "评价提交成功，通过管理员审核后公开展示。";
            return true;
        }

        /// <summary>
        /// 查询某岗位已审核通过的评价。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>评价数据表</returns>
        public DataTable GetApprovedByJob(int jobId)
        {
            return _dal.GetApprovedByJob(jobId);
        }

        /// <summary>
        /// 查询待审核评价列表。
        /// </summary>
        /// <returns>评价数据表</returns>
        public DataTable GetPendingAudit()
        {
            return _dal.GetPendingAudit();
        }

        /// <summary>
        /// 审核评价。
        /// </summary>
        /// <param name="commentId">评价编号</param>
        /// <param name="pass">true 通过，false 驳回</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool Audit(int commentId, bool pass, out string message)
        {
            int rows = _dal.Audit(commentId, pass ? (byte)1 : (byte)2);
            message = rows > 0 ? (pass ? "评价已审核通过。" : "评价已驳回。") : "审核失败：评价不存在。";
            return rows > 0;
        }
    }
}
