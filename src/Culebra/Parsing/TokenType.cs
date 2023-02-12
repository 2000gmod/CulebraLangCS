namespace Culebra.Parsing;

public enum TokenType {
    PLUS,
    MINUS,
    STAR,
    SLASH,
    COMMA,
    DOT,
    MOD,
    COLON,
    DCOLON,

    ASSIGN,
    EQ,
    NOT_EQ,
    LT,
    GT,
    LEQ,
    GEQ,
    SEMICOLON,

    AND,
    NOT,
    OR,

    LEFT_PAREN,
    RIGHT_PAREN,
    LEFT_SQR,
    RIGHT_SQR,
    LEFT_CUR,
    RIGHT_CUR,

    IF,
    ELSE,
    FOR,
    WHILE,
    BREAK,
    CONTINUE,
    RETURN,
    VAR,
    FUNC,

    INCLUDE,

    INT_LIT,
    DOUBLE_LIT,
    BOOL_LIT,
    STRING_LIT,
    IDENTIFIER,

    ERROR,
    EOF
}

public static class TokenTypeH {
    public static Array values {
        get {
            return Enum.GetValues<TokenType>();
        }
    }
}