using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complier
{
    /**
     * AST的节点。
     * 属性包括AST的类型、文本值、下级子节点和父节点
     */
    public interface IASTNode
    {
        //父节点
        IASTNode getParent();

        // 子节点
        IList<IASTNode> getChildren();

        // AST类型
        ASTNodeType getType();

        // 文本值
        String getText();
    }
}
