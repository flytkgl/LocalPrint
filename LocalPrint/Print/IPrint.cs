using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
//大萝卜卜 http://www.cnblogs.com/HelliX/ 2018年7月16日
namespace LocalPrint.Print
{
    //打印接口，需要打印格式，派生此接口即可
    public interface IPrint
    {
        bool Print(Graphics g);//返回值为是否还有打印需要打印的页内容，即是否打印结束
    }
}
