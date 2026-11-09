using System;

namespace CampusPartTime.Model
{
    /// <summary>
    /// 兼职岗位实体，对应数据库表 job（16 个字段）；
    /// 另含视图 v_job_detail 返回的商家名称、联系人与联系电话三个扩展属性。
    /// </summary>
    public class Job
    {
        /// <summary>岗位编号（主键，自增）</summary>
        public int JobId { get; set; }
        /// <summary>商家编号（外键）</summary>
        public int EmployerId { get; set; }
        /// <summary>岗位名称</summary>
        public string JobName { get; set; }
        /// <summary>岗位类型（家教 / 餐饮服务 / 促销推广 / 营业员 / 行政助理等）</summary>
        public string JobType { get; set; }
        /// <summary>薪资</summary>
        public decimal Salary { get; set; }
        /// <summary>计薪单位（元/小时 或 元/天）</summary>
        public string SalaryUnit { get; set; }
        /// <summary>工作地点</summary>
        public string WorkPlace { get; set; }
        /// <summary>工作时段</summary>
        public string WorkTime { get; set; }
        /// <summary>招聘人数</summary>
        public int RecruitNum { get; set; }
        /// <summary>已录用人数</summary>
        public int HiredNum { get; set; }
        /// <summary>岗位描述</summary>
        public string JobDesc { get; set; }
        /// <summary>发布日期</summary>
        public DateTime PublishDate { get; set; }
        /// <summary>报名截止日期</summary>
        public DateTime Deadline { get; set; }
        /// <summary>审核状态（0 待审核 / 1 通过 / 2 驳回）</summary>
        public byte AuditStatus { get; set; }
        /// <summary>岗位状态（0 在招 / 1 已招满 / 2 已下架）</summary>
        public byte JobStatus { get; set; }
        /// <summary>商家名称（视图扩展列）</summary>
        public string EmpName { get; set; }
        /// <summary>联系人（视图扩展列）</summary>
        public string EmpContact { get; set; }
        /// <summary>联系电话（视图扩展列）</summary>
        public string EmpPhone { get; set; }
    }
}
