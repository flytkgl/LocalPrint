using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace LocalPrint.Web
{
    /// <summary>
    /// WEB发送过来的Json生成的实体类
    /// </summary>
    public class WebJson
    {
        // <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        // <summary>
        /// 
        /// </summary>
        public string template { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Dictionary<string, string>> list { get; set; }

        public WebJson()
        {

        }
        public WebJson(int type, string template, List<Dictionary<string, string>> list)
        {
            this.type = type;
            this.template = template;
            this.list = list;

        }
    }
}
