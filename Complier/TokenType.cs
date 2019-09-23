
namespace Complier
{
    public enum TokenType
    {
        Empty,  // 空
        Plus,   // +
        Minus,  // -
        Star,   // *
        Slash,  // /

        GE,     // >=
        GT,     // >
        EQ,     // ==
        LE,     // <=
        LT,     // <

        SemiColon, // ;
        LeftParen, // (
        RightParen,// )

        Assignment,// =

        If,
        Else,

        Int,       

        Identifier,     //标识符 int age = 45; age就是标识符

        IntLiteral,     //整型字面量 int age = 45; 45就是字面量
        StringLiteral   //字符串字面量
    }
}