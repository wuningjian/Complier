using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complier
{
    public interface IToken
    {
        TokenType getType();
        String getText();
    }
}
