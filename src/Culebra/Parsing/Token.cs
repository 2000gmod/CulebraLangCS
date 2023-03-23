namespace Culebra.Parsing;

using static TokenType;

[Serializable]
public struct Token {
    public TokenType type;

    public string identifierName {get; set;}
    public string stringValue {get; set;}
    public int intValue {get; set;}
    public double doubleValue {get; set;}
    public bool boolValue {get; set;}
    public int line {get; set;}

    public Token() {
        type = TokenType.EOF;
        identifierName = "";
        stringValue = "";
        intValue = 0;
        doubleValue = 0.0;
        boolValue = false;
        line = 0;
    }

    public Token(int line) : this() {
        this.line = line;
    }

    public override string ToString() {
        string val = type.ToString();

        switch (type) {
            case INT_LIT:
                val += ": " + intValue;
                break;
            case DOUBLE_LIT:
                val += ": " + doubleValue;
                break;
            case STRING_LIT:
                string stringview = stringValue.Length > 20 ? $"\"{stringValue.Substring(0, 17)}\" [...] " : $"\"{stringValue}\"";
                val += ": " + stringview;
                break;
            case BOOL_LIT:
                val += ": " + boolValue;
                break;
            case IDENTIFIER:
                val += ": " + identifierName;
                break;
        }
        return val;
    }
}