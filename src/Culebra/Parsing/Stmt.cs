namespace Culebra.Parsing;

public abstract class Statement { }

public class BlockStmt : Statement {
    public readonly List<Statement> statements;

    public BlockStmt(params Statement[] s) {
        statements = s.ToList();
    }

    public BlockStmt(List<Statement> s) {
        statements = s;
    }
}

public class ExprStmt : Statement {
    public readonly Expression expr;

    public ExprStmt(Expression e) {
        expr = e;
    }
}

public struct ParameterT {
    public Type type;
    public Token name;

    public ParameterT(Type t, Token n) {
        type = t;
        name = n;
    }
}

public class FuncDeclarationStmt : Statement {
    public readonly Type type;
    public readonly Token name;
    public readonly List<ParameterT> parameters;
    public readonly Statement body;

    public FuncDeclarationStmt(Type t, Token n, List<ParameterT> p, Statement b) {
        type = t;
        name = n;
        parameters = p;
        body = b;
    }
}

public class VarDeclarationStmt : Statement {
    public readonly Type type;
    public readonly Token name;
    public readonly Expression value;

    public VarDeclarationStmt(Type type, Token name, Expression value) {
        this.type = type;
        this.name = name;
        this.value = value;
    }
}

public class ReturnStmt : Statement {
    public readonly Expression value;

    public ReturnStmt(Expression value) {
        this.value = value;
    }
}

public class IfStmt : Statement {
    public readonly Expression condition;
    public readonly Statement ifBody, elseBody;

    public IfStmt(Expression condition, Statement ifBody, Statement elseBody) {
        this.condition = condition;
        this.ifBody = ifBody;
        this.elseBody = elseBody;
    }
}

public class WhileStmt : Statement {
    public readonly Expression condition;
    public readonly Statement body;
    public readonly bool isForLoop;

    public WhileStmt(Expression condition, Statement body, bool isForLoop) {
        this.condition = condition;
        this.body = body;
        this.isForLoop = isForLoop;
    }
}

public class BreakStmt : Statement { }

public class ContinueStmt : Statement { }
