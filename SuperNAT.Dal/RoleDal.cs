﻿using Dapper;
using SuperNAT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperNAT.Dal
{
    public class RoleDal : BaseDal<Role>
    {
        public ReturnResult<Role> GetRole(Role model, Trans t = null)
        {
            var rst = new ReturnResult<Role>() { Message = "暂无记录" };

            try
            {
                conn = CreateMySqlConnection(t);
                rst.Data = conn.Get<Role>(model.id);
                if (rst.Data != null)
                {
                    //获取角色列表
                    rst.Data.menu_ids = conn.GetList<Authority>("where role_id=@role_id", new { model.role_id }, t?.DbTrans).Select(c => c.menu_id).ToList();
                    rst.Result = true;
                    rst.Message = "获取成功";
                }
            }
            catch (Exception ex)
            {
                rst.Message = $"获取失败：{ex.InnerException ?? ex}";
                Log4netUtil.Error($"{ex.InnerException ?? ex}");
            }

            return rst;
        }

        public ReturnResult<List<Role>> GetList(Role model, Trans t = null)
        {
            var rst = new ReturnResult<List<Role>>() { Message = "暂无记录" };

            try
            {
                conn = CreateMySqlConnection(t);
                var sql = new StringBuilder(@"select * from role ");
                if (model.page_index > 0)
                {
                    if (!string.IsNullOrWhiteSpace(model.search))
                    {
                        model.search = $"%{model.search}%";
                        sql.Append("where name like @search ");
                        sql.Append("or remark like @search ");
                    }
                    rst.Data = conn.GetListPaged<Role>(model.page_index, model.page_size, sql.ToString(), out int totalCount, "id asc", model, t?.DbTrans).ToList();
                    rst.PageInfo = new PageInfo()
                    {
                        PageIndex = model.page_index,
                        PageSize = model.page_size,
                        TotalCount = totalCount
                    };
                    rst.Result = true;
                    rst.Message = "获取成功";
                }
                else
                {
                    sql.Append("order by id ");
                    rst.Data = conn.Query<Role>(sql.ToString(), null, t?.DbTrans).ToList();
                }
                if (rst.Data != null)
                {
                    rst.Result = true;
                    rst.Message = "获取成功";
                }
            }
            catch (Exception ex)
            {
                rst.Message = $"获取失败：{ex.InnerException ?? ex}";
                Log4netUtil.Error($"{ex.InnerException ?? ex}");
            }

            return rst;
        }
    }
}
