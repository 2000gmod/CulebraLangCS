namespace Culebra.Parsing;

using static TokenType;

public class Parser {
    const int MAX_ARGUMENTS = 16;
    private string filepath;
    private static List<string> usedFiles = new();
    private List<Token> tokens;
    private int current = 0;

    public static List<Statement> ParseFile(in string filepath, Parser parent = null) {
        if (parent == null) usedFiles.Clear();
        
        var fullpath = Path.GetFullPath(filepath);
        if (Path.GetFullPath(parent?.filepath ?? "/") == fullpath) {
            ErrorReporter.reportWarning($"WARNING: file '{filepath}' is including itself.");
        }
        if(usedFiles.Contains(fullpath)) {
            if(parent != null) return new();
        }
        usedFiles.Add(fullpath);

        Parser parser = new Parser(filepath, parent);
        return parser.parse();
    }

    private Parser(in string filepath, Parser parent = null) {
        this.filepath = filepath;
        try {
            var text = File.ReadAllText(this.filepath);
            Scanner scanner = new Scanner(text);

            this.tokens = scanner.tokenize();
        }
        catch (FileNotFoundException e) {
            string includeText = parent != null ? $" (Caused by include statement on file '{parent.filepath}')" : "";
            ErrorReporter.reportError("Parser IO error: " + e.Message + includeText);
        }
        catch (Exception e) {
            ErrorReporter.reportError(e.Message);
        }
    }

    private List<Statement> parse() {
        List<Statement> statements = new();

        try {
            while(!isAtEnd()) {
                statements.Add(topLevelStatement(statements));
            }
            return statements;
        }
        catch(ParseException e) {
            ErrorReporter.reportError(e.Message);
            return null;
        }
    }

    private Statement topLevelStatement(in List<Statement> statements = null) {
        while (match(INCLUDE)) {
            if (statements == null) throw error(peek(), "Using 'include' statements outside of global scope is illegal.");
            var fname = consume(STRING_LIT, "Expected filename after include statement.").stringValue;

            string newfile = Path.GetDirectoryName(filepath) + "/" + fname;
            statements.AddRange(ParseFile(newfile, this));
            consume(SEMICOLON, "Expected ';' after include statement.");
        }
        return declaration();
    }

    private Statement declaration() {
        if (match(FUNC)) return funcDeclaration();
        if (match(IDENTIFIER)) return varHandling(previous());
        return statement();
    }

    private Statement funcDeclaration() {
        var name = consume(IDENTIFIER, "Expected identifier after function declaration.");
        consume(LEFT_PAREN, "Expected opening parenthesis after function identifier.");

        List<ParameterT> parameters = new();

        if (!check(RIGHT_PAREN)) {
            do {
                if (parameters.Count() >= MAX_ARGUMENTS) {
                    error(peek(), "Exceeded max parameter count.");
                }
                var pname = consume(IDENTIFIER, "Expected parameter name.");
                consume(COLON, "Expected ':' after parameter name.");
                var ptype = parseType();
                ParameterT param = new ParameterT(ptype, pname);
                parameters.Add(param);
            } while (match(COMMA));
        }
        consume(RIGHT_PAREN, "Expected ')' after function parameter list.");
        consume(COLON, $"Function {name.identifierName} is missing a return type.");
        var rtype = parseType();
        consume(LEFT_CUR, "Expected '{' after function declaration.");
        Statement functionBlock = blockStatement();
        return new FuncDeclarationStmt(rtype, name, parameters, functionBlock);
    }

    private Statement varHandling(Token name) {
        if (match(COLON)) return varDeclaration(name);
        current--;
        return expressionStatement();
    }

    private Statement varDeclaration(Token name, bool useSemicolon = true) {
        Type type = parseType();

        Expression expr = null;
        if (match(ASSIGN)) expr = expression();
        if (useSemicolon) consume(SEMICOLON, "Expected ';' after variable declaration.");
        return new VarDeclarationStmt(type, name, expr);
    }

    private Statement statement() {
        if (match(FOR)) return forStatement();
        if (match(IF)) return ifStatement();
        if (match(RETURN)) return returnStatement();
        if (match(WHILE)) return whileStatement();
        if (match(LEFT_CUR)) return blockStatement();
        if (match(BREAK)) return breakStatement();
        if (match(CONTINUE)) return continueStatement();

        return expressionStatement();
    }

    private Statement forStatement() {
        consume(LEFT_PAREN, "Expected '(' after 'for' token.");
        Statement init;
        if (checkForm(IDENTIFIER, COLON)) {
            Token name = consume(IDENTIFIER, "");
            consume(COLON, "");
            Type type = parseType();
            consume(ASSIGN, "Expected assignment in for-loop initializer");
            init = new VarDeclarationStmt(type, name, expression());
            consume(SEMICOLON, "Expected ';' after initializer declaration in for-loop.");
        }
        else if (match(SEMICOLON)) {
            init = null;
        }
        else {
            init = expressionStatement();
        }

        Expression condition = null;
        if (!check(SEMICOLON)) {
            condition = expression();
        }

        consume(SEMICOLON, "Expected ';' after for-loop condition.");

        Expression increment = null;

        if (!check(RIGHT_PAREN)) increment = expression();

        consume(RIGHT_PAREN, "Expected ')' after for-loop increment expression.");

        Statement body = statement();

        if (increment != null) {
            body = new BlockStmt(body, new ExprStmt(increment));
        }

        if (condition == null) {
            condition = new LiteralExpr(new Token{type = BOOL_LIT, boolValue = true});
        }

        body = new WhileStmt(condition, body, true);
        if (init != null) {
            body = new BlockStmt(init, body);
        }
        return body;
    }

    private Statement ifStatement() {
        consume(LEFT_PAREN, "Expected '(' after 'if' token.");
        Expression condition = expression();
        consume(RIGHT_PAREN, "Expected ')' after if condition.");

        Statement ifBody = statement();
        Statement elseBody = null;

        if (match(ELSE)) elseBody = statement();

        return new IfStmt(condition, ifBody, elseBody);
    }

    private Statement returnStatement() {
        Expression value = null;

        if (!check(SEMICOLON)) {
            value = expression();
        }
        consume(SEMICOLON, "Expected ';' after return statement.");

        return new ReturnStmt(value);
    }

    private Statement whileStatement() {
        consume(LEFT_PAREN, "Expected '(' after 'while' token.");
        Expression condition = expression();
        consume(RIGHT_PAREN, "Expected ')' after while-loop condition.");
        Statement body = statement();

        return new WhileStmt(condition, body, false);
    }

    private Statement blockStatement() {
        List<Statement> body = new();
        while (!check(RIGHT_CUR) && !isAtEnd()) {
            body.Add(topLevelStatement());
        }
        consume(RIGHT_CUR, "Expected '}' after a block statement.");
        return new BlockStmt(body);
    }

    private Statement breakStatement() {
        consume(SEMICOLON, "Expected ';' after 'break' token.");
        return new BreakStmt();
    }

    private Statement continueStatement() {
        consume(SEMICOLON, "Expected ';' after 'continue' token.");
        return new ContinueStmt();
    }

    private Statement expressionStatement() {
        Expression expr = expression();
        consume(SEMICOLON, "Expected ';' after expression.");
        return new ExprStmt(expr);
    }

    private Expression expression() {
        return assignment();
    }

    private Expression assignment() {
        Expression expr = orExpr();

        if (match(ASSIGN)) {
            Token equals = previous();
            Expression value = assignment();

            if (expr is IdentifierExpr) {
                Token name = ((IdentifierExpr) expr).ident;
                return new AssignExpr(name, value);
            }
            throw error(equals, "Invalid assignment target.");
        }
        return expr;
    }

    private Expression orExpr() {
        Expression expr = andExpr();

        while (match(OR)) {
            Token op = previous();
            Expression right = andExpr();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression andExpr() {
        Expression expr = eqExpr();
        while (match(AND)) {
            Token op = previous();
            Expression right = eqExpr();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression eqExpr() {
        Expression expr = compExpr();
        while (match(EQ, NOT_EQ)) {
            Token op = previous();
            Expression right = compExpr();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression compExpr() {
        Expression expr = addition();
        while (match(GT, GEQ, LT, LEQ)) {
            Token op = previous();
            Expression right = addition();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression addition() {
        Expression expr = multiplication();
        while (match(PLUS, MINUS)) {
            Token op = previous();
            Expression right = multiplication();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression multiplication() {
        Expression expr = unary();
        while (match(STAR, SLASH, MOD)) {
            Token op = previous();
            Expression right = unary();
            expr = new BinaryExpr(expr, right, op);
        }
        return expr;
    }

    private Expression unary() {
        if (match(NOT, MINUS)) {
            Token op = previous();
            Expression right = unary();
            return new UnaryExpr(right, op);
        }
        return memberOperationExpr();
    }

    private Expression memberOperationExpr() {
        Expression expr = primaryExpr();

        while (true) {
            if (match(LEFT_PAREN)) expr = callExpr(expr);
            else if (match(DOT)){
                Token name = consume(IDENTIFIER, "Expected member name after '.' token.");
                expr = new MemberAccessExpr(expr, name);
            }
            else break;
        }
        return expr;
    }

    private Expression callExpr(Expression callee) {
        List<Expression> arguments = new();
        if (!check(RIGHT_PAREN)) {
            do {
                if (arguments.Count() > MAX_ARGUMENTS) {
                    throw error(peek(), "Exceeded max argument count.");
                }
                arguments.Add(expression());
            } while (match(COMMA));
        }
        consume(RIGHT_PAREN, "Expected ')' after call expression.");
        return new CallExpr(callee, arguments);
    }

    private Expression primaryExpr() {
        if (match(BOOL_LIT, INT_LIT, DOUBLE_LIT, STRING_LIT)) return new LiteralExpr(previous());

        if (match(IDENTIFIER)) {
            return new IdentifierExpr(previous());
        }

        if (match(LEFT_PAREN)) {
            Expression expr = expression();
            consume(RIGHT_PAREN, "Expected ')' after expression.");
            return new ParenthesizedExpr(expr);
        }
        throw error(peek(), "Expected expression.");
    }

    private Type parseType() {
        Type type = ptrType();
        return type;
    }

    private Type ptrType() {
        Type type = vType();
        while(match(STAR)) {
            type = new PointerType(type);
        }
        return type;
    }

    private Type vType() {
        Token name = consume(IDENTIFIER, "Expected type name.");
        return new ValueType(name);
    }

    private bool isAtEnd() {
        return peek().type == EOF;
    }

    private Token advance() {
        if (!isAtEnd()) current++;
        return previous();
    }

    private Token consume(TokenType type, string msg) {
        if (check(type)) return advance();
        throw error(peek(), msg);
    }

    private ParseException error(Token token, string msg) {
        string message = $"(at token: {token}): \n\t{msg}";
        ErrorReporter.reportError($"ERROR: Parsing error (file: '{filepath}') at line {token.line} {message}");

        return new ParseException(message);
    }

    bool check(TokenType type) {
        if (isAtEnd()) return false;
        return type == peek().type;
    }

    bool match(params TokenType[] types) {
        foreach (var type in types) {
            if (check(type)) {
                advance();
                return true;
            }
        }
        return false;
    }

    private Token previous() {
        return tokens[current - 1];
    }

    private Token peek() {
        return tokens[current];
    }

    private bool checkForm(params TokenType[] types) {
        int prevCurrent = current;

        foreach (var a in types) {
            if (!match(a)) {
                current = prevCurrent;
                return false;
            }
        }

        current = prevCurrent;
        return true;
    }
}