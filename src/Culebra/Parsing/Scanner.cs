namespace Culebra.Parsing;

using static Culebra.Parsing.TokenType;

public class Scanner {
    private List<Token> tokens = new List<Token>();
    private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();

    private int start = 0;
    private int current = 0;
    private int line = 1;
    private string src;

    static Scanner() {
        keywords.Add("and", AND);
        keywords.Add("or", OR);
        keywords.Add("not", NOT);

        keywords.Add("true", BOOL_LIT);
        keywords.Add("false", BOOL_LIT);

        keywords.Add("if", IF);
        keywords.Add("else", ELSE);
        keywords.Add("for", FOR);
        keywords.Add("while", WHILE);
        keywords.Add("break", BREAK);
        keywords.Add("continue", CONTINUE);
        keywords.Add("return", RETURN);
        keywords.Add("var", VAR);
        keywords.Add("func", FUNC);
        keywords.Add("include", INCLUDE);
    }
    
    public Scanner(in string src) {
        this.src = src;
    }

    public List<Token> tokenize() {
        while (!atEnd()) {
            start = current;
            scanToken();
        }
        tokens.Add(new Token(line){type = EOF});
        return tokens;
    }

    private void scanToken() {
        char c = advance();
        switch (c) {
            case '(':
                addToken(LEFT_PAREN);
                break;
            case ')':
                addToken(RIGHT_PAREN);
                break;
            case '{':
                addToken(LEFT_CUR);
                break;
            case '}':
                addToken(RIGHT_CUR);
                break;
            case '[':
                addToken(LEFT_SQR);
                break;
            case ']':
                addToken(RIGHT_SQR);
                break;

            case '+':
                addToken(PLUS);
                break;
            case '-':
                addToken(MINUS);
                break;
            case '*':
                addToken(STAR);
                break;
            case ',':
                addToken(COMMA);
                break;
            case '.':
                addToken(DOT);
                break;
            case '%':
                addToken(MOD);
                break;
            case ':':
                if (match(':')) {
                    addToken(DCOLON);
                }
                else addToken(COLON);
                break;
            case ';':
                addToken(SEMICOLON);
                break;
            case '=':
                addToken(match('=') ? EQ : ASSIGN);
                break;
            case '>':
                addToken(match('=') ? GEQ : GT);
                break;
            case '<':
                addToken(match('=') ? LEQ : LT);
                break;
            case '!':
                addToken(match('=') ? NOT_EQ : NOT);
                break;
            
            case '/':
                if (match('/')) {
                    while (peek() != '\n' && !atEnd()) advance();
                }
                else addToken(SLASH);
                break;
            
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;
            
            case '"':
                scanString();
                break;
            
            default:
                if (isDigit(c)) {
                    scanNumber();
                }
                else if (isAlpha(c)) {
                    scanIdentifier();
                }
                else {
                    ErrorReporter.reportError("Unexpected character");
                }
                break;
        }
    }

    private bool match(char expected) {
        if (atEnd()) return false;
        if (src[current] != expected) return false;

        current++;
        return true;
    }

    private char peek() {
        if (atEnd()) return '\0';
        return src[current];
    }

    private char peekNext() {
        if (current + 1 >= src.Length) return '\0';
        return src[current + 1];
    }

    private bool isAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool isDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private bool isAlphaNumeric(char c) {
        return isAlpha(c) || isDigit(c);
    }

    private char advance() {
        current++;
        return src[current - 1];
    }

    private bool atEnd() {
        return current >= src.Length;
    }

    private void addToken(TokenType type) {
        tokens.Add(new Token(line) {type = type});
    }

    private void addToken(Token tok) {
        tokens.Add(tok);
    }

    private void scanIdentifier() {
        while (isAlphaNumeric(peek())) advance();

        string text = src.Substring(start, current - start);

        TokenType type = ERROR;

        if (!keywords.TryGetValue(text, out type)) {
            addToken(new Token(line) {
                type = IDENTIFIER,
                identifierName = text
            });
            return;
        }

        if (type == BOOL_LIT) {
            addToken(new Token(line) {
                type = BOOL_LIT,
                boolValue = text == "true"
            });
            return;
        }

        addToken(type);
    }

    private void scanNumber() {
        while(isDigit(peek())) advance();

        if (peek() == '.' && isDigit(peekNext())) {
            advance();
            while(isDigit(peek())) advance();
            addToken(new Token(line) {
                type = DOUBLE_LIT,
                doubleValue = double.Parse(src.Substring(start, current - start))
            });
            return;
        } 

        addToken(new Token(line) {
            type = INT_LIT,
            intValue = int.Parse(src.Substring(start, current - start))
        });
        return;
    }

    private void scanString() {
        while (peek() != '"' && !atEnd()) {
            if (peek() == '\n') line++;
            advance();
        }

        if (atEnd()) {
            ErrorReporter.reportError("ERROR: File scanning error: Unterminated string");
        }

        advance();

        string val = src.Substring(start + 1, current - start - 2);
        val = formatEscapes(val);
        addToken(new Token(line) {
            type = STRING_LIT,
            stringValue = val
        });
    }

    private string formatEscapes(in string src) {
        string res = "";
        for (int i = 0; i < src.Length; i++) {
            if (src[i] == '\\' && i != src.Length - 1) {
                switch (src[i + 1]) {
                    case 'n':
                        res += "\n";
                        i += 1;
                        break;
                    case 't':
                        res += "\t";
                        i += 1;
                        break;
                    default:
                        break;
                }
            }
            else res += src[i];
        }
        return res;
    }
}