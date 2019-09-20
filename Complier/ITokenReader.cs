using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complier
{
    /*
     * TokenReader设计思想类似于 c#的StringReader，StringReader是以字符为单位读取， TokenReader以token为单位读取。token有点自定义字符的意味。
     * *
     */
    interface ITokenReader
    {
        IToken read();
        IToken peek();
        void unread();
        int getPosition();
        void setPosition(int position);
    }
}
