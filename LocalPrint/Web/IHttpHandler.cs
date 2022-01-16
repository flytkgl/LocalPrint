using System;
using System.Collections.Generic;
using System.Text;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint.Web
{
    //类似于MVC框架的ACTION或者CONTROLER，可派生此接口实现其他操作
    public interface IHttpHandler
    {
        string Handler(string txt);//简单的处理文本消息
    }
}
