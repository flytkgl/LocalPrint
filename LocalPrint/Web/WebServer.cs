using Newtonsoft.Json;
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
        // 替换 Process 方法中 resOject 的定义和赋值方式，避免匿名类型属性只读问题
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
                rep.ContentType = "application/json;charset=UTF-8";
                rep.AppendHeader("Content-Type", "application/json;charset=UTF-8");

                // 使用可变对象代替匿名类型
                var resObject = new ResponseObject
                {
                    success = false,
                    code = 404,
                    data = "未找到相关服务"
                };

                foreach (var kv in handlerMap)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(req.RawUrl, kv.Key))//正则匹配
                    {
                        var data = "";
                        if (req.HttpMethod == "GET")
                            data = req.RawUrl;//不做任何处理，直接将原始的http请求转发到handler。。。
                        else
                            using (var r = new StreamReader(req.InputStream, Encoding.UTF8))
                            {
                                data = r.ReadToEnd();
                            }
                        rep.StatusCode = 200;//ok
                        resObject.success = true;
                        resObject.code = 200;
                        resObject.data = kv.Value.Handler(data);
                        break;
                    }
                }
                string responseString = JsonConvert.SerializeObject(resObject,
                    new JsonSerializerSettings()
                    {
                        StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                    });
                //返回处理结果给浏览器
                using (StreamWriter writer = new StreamWriter(rep.OutputStream, Encoding.UTF8))
                {
                    writer.Write(responseString);
                    writer.Close();
                    rep.Close();
                }
            }
        }

        // 在文件末尾或合适位置添加 ResponseObject 类
        class ResponseObject
        {
            public bool success { get; set; }
            public int code { get; set; }
            public string data { get; set; }
        }

    }
}
