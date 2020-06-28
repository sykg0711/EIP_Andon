﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;

namespace TubeMarking_Data
{
    class EIP_DataWrite_On_SQLDB
    {

        /// <summary>
        /// SQLクエリを発行して書き込む
        /// </summary>
        /// <param name="columns">列名</param>
        /// <param name="datas">データ</param>
        public void SQLWrite(object[] columns, object[] datas)
        {
            StringBuilder sqlQuery = new StringBuilder();
            string DBTabel = ConfigurationManager.AppSettings["DBTabel1"];

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataSource"].ConnectionString);
            connection.Open();

            sqlQuery.Append("INSERT INTO " + DBTabel +" (");

            foreach(object str in columns)
            {
                if (str.ToString() == columns[columns.Length - 1].ToString())
                {
                    sqlQuery.Append(str.ToString() + ") VALUES (");
                    break;
                }
                else
                {
                    sqlQuery.Append(str.ToString() + ",");
                } 
            }

            foreach (object str in datas)
            {
                if (str.ToString() == datas[datas.Length - 1].ToString())
                {
                    if (str.GetType() == typeof(string))
                    {
                        sqlQuery.Append("'" + str.ToString() + "')");
                        break;
                    }
                    else
                    {
                        sqlQuery.Append(str.ToString() + ")");
                        break;
                    }

                }
                else
                {
                    if (str.GetType() == typeof(string))
                    {
                        sqlQuery.Append("'" + str.ToString() + "'" + ",");
                    }
                    else
                    {
                        sqlQuery.Append(str.ToString() + ",");
                    }
                }
            }

            new SqlCommand(sqlQuery.ToString(), connection).ExecuteNonQuery();
            connection.Close();
            connection.Dispose();

        }

    }
}
