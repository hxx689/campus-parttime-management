using System;
using System.Data;
using CampusPartTime.DAL;
using CampusPartTime.Model;

namespace CampusPartTime.BLL
{
    /// <summary>
    /// 岗位业务逻辑类：发布、修改、下架、审核与学生端检索分页。
    /// </summary>
    public class JobBLL
    {
        private readonly JobDAL _dal = new JobDAL();

        /// <summary>
        /// 发布岗位。业务规则：新岗位审核状态为待审核，通过后才对学生展示。
        /// </summary>
        /// <param name="m">岗位实体</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>发布成功返回 true</returns>
        public bool Publish(Job m, out string message)
        {
            if (string.IsNullOrWhiteSpace(m.JobName))
            {
                message = "岗位名称不能为空。";
                return false;
            }
            if (m.RecruitNum < 1 || m.RecruitNum > 100)
            {
                message = "招聘人数应在 1 到 100 之间。";
                return false;
            }
            if (m.Salary < 0)
            {
                message = "薪资不能为负数。";
                return false;
            }
            if (m.Deadline.Date < DateTime.Today)
            {
                message = "报名截止日期不能早于今天。";
                return false;
            }
            int id = _dal.Insert(m);
            if (id <= 0)
            {
                message = "岗位发布失败，请稍后重试。";
                return false;
            }
            message = "岗位发布成功，等待管理员审核！";
            return true;
        }

        /// <summary>
        /// 修改岗位。业务规则：修改后审核状态重置为待审核。
        /// </summary>
        /// <param name="m">岗位实体，JobId 与 EmployerId 必填</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>修改成功返回 true</returns>
        public bool Update(Job m, out string message)
        {
            if (m.Deadline.Date < DateTime.Today)
            {
                message = "报名截止日期不能早于今天。";
                return false;
            }
            int rows = _dal.Update(m);
            if (rows <= 0)
            {
                message = "修改失败：岗位不存在或无权修改。";
                return false;
            }
            message = "岗位信息修改成功，等待管理员重新审核！";
            return true;
        }

        /// <summary>
        /// 商家下架岗位（逻辑删除）。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <param name="employerId">商家编号</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>下架成功返回 true</returns>
        public bool Offline(int jobId, int employerId, out string message)
        {
            int rows = _dal.UpdateStatusToOffline(jobId, employerId);
            if (rows <= 0)
            {
                message = "下架失败：岗位不存在、无权操作或已下架。";
                return false;
            }
            message = "岗位已下架。";
            return true;
        }

        /// <summary>
        /// 学生端条件检索岗位并分页。
        /// </summary>
        /// <param name="jobType">岗位类型，空串表示不限</param>
        /// <param name="minSalary">最低薪资，null 表示不限</param>
        /// <param name="keyword">关键词，空串表示不限</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页行数</param>
        /// <param name="totalCount">输出参数：符合条件的总记录数</param>
        /// <returns>当前页数据表</returns>
        public DataTable SearchJobs(string jobType, decimal? minSalary, string keyword,
            int pageIndex, int pageSize, out int totalCount)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            totalCount = _dal.CountJob(jobType, minSalary, keyword);
            if (totalCount == 0)
            {
                return new DataTable();
            }
            return _dal.GetJobPage(jobType, minSalary, keyword, pageIndex, pageSize);
        }

        /// <summary>
        /// 按主键获取岗位。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>岗位实体或 null</returns>
        public Job GetJob(int jobId)
        {
            return _dal.GetById(jobId);
        }

        /// <summary>
        /// 查询某商家的全部岗位。
        /// </summary>
        /// <param name="employerId">商家编号</param>
        /// <returns>岗位数据表</returns>
        public DataTable GetByEmployer(int employerId)
        {
            return _dal.GetByEmployer(employerId);
        }

        /// <summary>
        /// 管理员审核岗位。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <param name="pass">true 通过，false 驳回</param>
        /// <param name="message">输出参数：提示文本</param>
        /// <returns>操作成功返回 true</returns>
        public bool Audit(int jobId, bool pass, out string message)
        {
            int rows = _dal.Audit(jobId, pass ? (byte)1 : (byte)2);
            message = rows > 0 ? (pass ? "岗位已审核通过。" : "岗位已驳回。") : "审核失败：岗位不存在。";
            return rows > 0;
        }

        /// <summary>
        /// 查询待审核岗位列表。
        /// </summary>
        /// <returns>岗位数据表</returns>
        public DataTable GetPendingAudit()
        {
            return _dal.GetPendingAudit();
        }

        /// <summary>
        /// 统计在招岗位数。
        /// </summary>
        /// <returns>在招岗位数</returns>
        public int CountOnSale()
        {
            return _dal.CountOnSale();
        }

        /// <summary>
        /// 查询岗位详情（来源为视图 v_job_detail，含商家联系信息）。
        /// </summary>
        /// <param name="jobId">岗位编号</param>
        /// <returns>单行岗位详情数据表，无匹配时为空表</returns>
        public System.Data.DataTable GetJobDetail(int jobId)
        {
            return _dal.GetDetailView(jobId);
        }
    }
}
