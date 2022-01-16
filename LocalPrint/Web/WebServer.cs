using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint.Web
{
    //本地微型HTTP服务器，可以理解网络请求处理的服务器处理流程
    class WebServer
    {
        HttpListener httpListener;
        public string Err;
        Dictionary<string, IHttpHandler> handlerMap = new Dictionary<string, IHttpHandler>();
        //启动本地HTTP服务器，相当于APACHE，NGINX之类的WEB服务器，接受浏览器发送过来的SOCKET请求
        public bool RunWeb(string url)
        {
            try
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add(url);
                httpListener.Start();
                var th = new Thread(Process);
                th.IsBackground = true;
                th.Start();
                return true;
            }
            catch (Exception ex)
            {
                Err = "启动本地服务器出现问题：" + ex.Message;
                return false;
            }
        }
        //设置相应路由和相应的处理类
        public bool AddHandler(string url,IHttpHandler httpHandler)
        {
            try
            {
                handlerMap.Add(url, httpHandler);
                return true;
            }
            catch (Exception ex)
            {
                Err = "添加路径出错：" + ex.Message;
                return false;
            }
        }
        //HTTP数据处理，这一部分相当于C#，JAVA和PHP和其他语言的功能，WEB服务器将请求转发给相应的语言处理，并把结果返回给浏览器
        void Process()
        {
            for (; ; )
            {
                var cnx = httpListener.GetContext();//获取浏览器请求上下文，串行处理，也可以改成并行
                var req = cnx.Request;
                var rep = cnx.Response;
                rep.ContentEncoding = Encoding.UTF8;
                rep.Headers.Add("Access-Control-Allow-Origin", "*");//允许浏览器跨域！非常重要
                rep.StatusCode = 404;
                var ret = "pag not found";
                //rep.ContentType = "text";//返回内容，这里为text，ajax里面的请求datatype也需要设置为text或者html，不然会为null
                //这一部分其实就是大部分MVC网络框架里的路由部分！这里简单的发送原始文本给handler处理
                foreach (var kv in handlerMap)
                {
                    if(System.Text.RegularExpressions.Regex.IsMatch(req.RawUrl,kv.Key))//正则匹配
                    {
                        var data = "";
                        if (req.HttpMethod == "GET")
                            data = req.RawUrl;//不做任何处理，直接将原始的http请求转发到handler。。。
                        else
                        using (var r = new StreamReader(req.InputStream, Encoding.UTF8))
                        {
                            data = r.ReadToEnd();
                        }
                        ret = kv.Value.Handler(data);
                        rep.StatusCode = 200;//ok
                        break;
                    }
                }
                //返回处理结果给浏览器
                using (var w = new StreamWriter(rep.OutputStream, Encoding.UTF8))
                {
                    w.WriteLine(ret);
                }
            }
        }

    }
}
