namespace Culebra.Interpreter.Treewalk;

using Culebra.Parsing;
public class TreewalkInterpreter {
    
    private List<Statement> statements;
    private VarEnv global, innermost;

    public TreewalkInterpreter(string filename) {
        statements = Parser.ParseFile(filename);
        global = new VarEnv();
        innermost = global;
        load();
    }

    private void load() {
        foreach (var s in statements) {
            if (s is FuncDeclarationStmt f) {
                global.defineFunc(f.name.identifierName, f);
            }
            else {
                ErrorReporter.reportError("ERROR: Top level statements can only be functions or module related.");
            }
        }
    }

    public void run(string functionName = "main") {
        try {
            runFunction(global.getFunc(functionName));
        }
        catch (ReturnValueContainer) {
            
        }
    }

    private void runFunction(FuncDeclarationStmt f) {
        runStatement(f.body);
    }

    private void runStatement(Statement s) {
        if (s is BlockStmt) {
            runBlockStmt(s);
        }
        else if (s is ExprStmt) {
            runExprStmt(s);
        }
        else if (s is ReturnStmt) {
            runReturnStmt(s);
        }
        else if (s is VarDeclarationStmt) {
            runVarDeclarationStmt(s);
        }
        else if (s is IfStmt) {
            runIfStmt(s);
        }
        else if (s is WhileStmt) {
            runWhileStmt(s);
        }
        else if (s is BreakStmt) {
            runBreakStmt();
        }
        else if (s is ContinueStmt) {
            runContinueStmt();
        }
        else {
            ErrorReporter.reportError("ERROR: Runtime error: Invalid statement type.");
        }
    }

    private void runBlockStmt(Statement s) {
        innermost = new(innermost);
        foreach (var bs in (s as BlockStmt).statements) {
            runStatement(bs);
        }
        innermost = innermost.enclosing;
    }

    private void runExprStmt(Statement s) {
        evaluateExpr((s as ExprStmt).expr);
    }

    private void runReturnStmt(Statement s) {
        var stmt = (s as ReturnStmt);
        throw new ReturnValueContainer(stmt.value == null ? null : evaluateExpr(stmt.value));
    }

    private void runVarDeclarationStmt(Statement s) {
        var v = (s as VarDeclarationStmt);
        innermost.defineVar(v.name.identifierName, evaluateExpr(v.value));
    }

    private void runIfStmt(Statement s) {
        var ifs = (s as IfStmt);
        if (isTrue(ifs.condition)) {
            runStatement(ifs.ifBody);
        }
        else if (ifs.elseBody != null) runStatement(ifs.elseBody);
    }

    private void runWhileStmt(Statement s) {
        var whs = (s as WhileStmt);

        while (isTrue(whs.condition)) {
            try {
                runStatement(whs.body);
            }
            catch (LoopControlException e) {
                if (!e.isContinue) return;
                if (!whs.isForLoop) continue;

                var block = (whs.body) as BlockStmt;
                runStatement(block.statements.Last());
            }
        }
    }

    private void runBreakStmt() {
        throw new LoopControlException(false);
    }

    private void runContinueStmt() {
        throw new LoopControlException(true);
    }

    public RuntimeVariable evaluateExpr(Expression e) {
        if (e is LiteralExpr) {
            return evalLiteralExpr(e);
        }
        if (e is IdentifierExpr) {
            return evalIdentExpr(e);
        }
        if (e is UnaryExpr) {
            return evalUnaryExpr(e);
        }
        if (e is AssignExpr) {
            return evalAssignExpr(e);
        }
        if (e is BinaryExpr) {
            return evalBinaryExpr(e);
        }
        if (e is ParenthesizedExpr) {
            return evalParenExpr(e);
        }
        if (e is CallExpr) {
            return evalCallExpr(e);
        }
        ErrorReporter.reportError("ERROR: Runtime error: Invalid expression type.");
        return null;
    }

    private RuntimeVariable evalLiteralExpr(Expression e) {
        var tok = (e as LiteralExpr).value;
        switch (tok.type) {
            case TokenType.INT_LIT:
                return new PrimitiveVar(tok.intValue);
            case TokenType.DOUBLE_LIT:
                return new PrimitiveVar(tok.doubleValue);
            case TokenType.STRING_LIT:
                return new PrimitiveVar(tok.stringValue);
            case TokenType.BOOL_LIT:
                return new PrimitiveVar(tok.boolValue);
        }
        return null;
    }

    private RuntimeVariable evalIdentExpr(Expression e) {
        return innermost.getVar((e as IdentifierExpr).ident.identifierName);
    }

    private RuntimeVariable evalUnaryExpr(Expression e) {
        var ue = (e as UnaryExpr);
        PrimitiveVar res = evaluateExpr(ue.expr) as PrimitiveVar;

        switch (ue.op.type) {
            case TokenType.PLUS:
                return res;
            case TokenType.MINUS:
                return -res;
            case TokenType.NOT:
                return !res;
        }
        return null;
    }

    private RuntimeVariable evalAssignExpr(Expression e) {
        var ae = (e as AssignExpr);
        var res = evaluateExpr(ae.value);

        innermost.assignVar(ae.name.identifierName, res);
        return res;
    }

    private RuntimeVariable evalBinaryExpr(Expression e) {
        var be = (e as BinaryExpr);

        var left = evaluateExpr(be.left) as PrimitiveVar;
        var right = evaluateExpr(be.right) as PrimitiveVar;

        switch (be.op.type) {
            case TokenType.PLUS:
                return new PrimitiveVar(left + right);
            case TokenType.MINUS:
                return new PrimitiveVar(left - right);
            case TokenType.STAR:
                return new PrimitiveVar(left * right);
            case TokenType.SLASH:
                return new PrimitiveVar(left / right);
            case TokenType.MOD:
                return new PrimitiveVar(left % right);
            case TokenType.EQ:
                return new PrimitiveVar(left == right);
            case TokenType.NOT_EQ:
                return new PrimitiveVar(left != right);
            case TokenType.GT:
                return new PrimitiveVar(left > right);
            case TokenType.GEQ:
                return new PrimitiveVar(left >= right);
            case TokenType.LT:
                return new PrimitiveVar(left < right);
            case TokenType.LEQ:
                return new PrimitiveVar(left <= right);
            case TokenType.AND:
                return new PrimitiveVar(PrimitiveVar.logicalAnd(left, right));
            case TokenType.OR:
                return new PrimitiveVar(PrimitiveVar.logicalOr(left, right));
            default:
                ErrorReporter.reportError("ERROR: Runtime error: Invalid operator for binary expression.");
                return null;
        }
    }

    private RuntimeVariable evalParenExpr(Expression e) {
        return evaluateExpr((e as ParenthesizedExpr).expr);
    }

    private RuntimeVariable evalCallExpr(Expression e) {
        var ce = (e as CallExpr);

        if (!(ce.callee is IdentifierExpr)) {
            ErrorReporter.reportError("ERROR: Runtime error: Only identifiers are valid function callers.");
            return null;
        }

        var ide = (ce.callee as IdentifierExpr);
        var name = ide.ident.identifierName;

        try {
            interceptBuiltIns(name, ce.args);
        }
        catch (ReturnValueContainer rvc) {
            return rvc.value;
        }

        var func = innermost.getFunc(name);

        if (ce.args.Count() != func.parameters.Count) {
            ErrorReporter.reportError("ERROR: Runtime error: Invalid number of parameters.");
            return null;
        }

        innermost = new(innermost);
        RuntimeVariable res = null;

        try {
            int i = 0;
            foreach (var arg in ce.args) {
                innermost.defineVar(func.parameters[i].name.identifierName, evaluateExpr(arg));
                i++;
            }
            runFunction(func);
        }
        catch (ReturnValueContainer rvc) {
            res = rvc.value;
        }

        innermost = innermost.enclosing;
        return res;
    }

    private bool isTrue(Expression e) {
        return (evaluateExpr(e) as PrimitiveVar).isTrue();
    }

    private void interceptBuiltIns(string name, List<Expression> args) {
        if (name == "print") {
            throw StandardFunctions.print(this, args);
        }
    }
}
