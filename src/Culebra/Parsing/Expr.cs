namespace Culebra.Parsing;

public abstract class Expression { 
    public static void PrintExpr(Expression expr, int depth = 0) {
        for(int i = 0; i < depth; i++) Console.Write("    ");
        
        if (expr is LiteralExpr lit) {
            Console.WriteLine($"{lit.value}");
        }
        else if (expr is IdentifierExpr iden) {
            Console.WriteLine($"{iden.ident}");
        }
        else if (expr is UnaryExpr un) {
            Console.WriteLine($"Unary {un.op}");
            PrintExpr(un.expr, depth + 1);
        }
        else if (expr is BinaryExpr bin) {
            Console.WriteLine($"Binary {bin.op}");
            PrintExpr(bin.left, depth + 1);
            PrintExpr(bin.right, depth + 1);
        }
    }
}

public class LiteralExpr : Expression {
    public readonly Token value;

    public LiteralExpr(Token val) {
        value = val;
    }
};

public class IdentifierExpr : Expression {
    public readonly Token ident;

    public IdentifierExpr(Token id) {
        ident = id;
    }
}

public class UnaryExpr : Expression {
    public readonly Expression expr;
    public readonly Token op;

    public UnaryExpr(Expression e, Token op) {
        this.op = op;
        expr = e;
    }
}

public class BinaryExpr : Expression {
    public readonly Expression left, right;
    public readonly Token op;

    public BinaryExpr(Expression l, Expression r, Token op) {
        left = l;
        right = r;
        this.op = op;
    }
}

public class MemberAccessExpr : Expression {
    public readonly Expression parent;
    public readonly Token name;

    public MemberAccessExpr(Expression par, Token name) {
        parent = par;
        this.name = name;
    }
}

public class CallExpr : Expression {
    public readonly Expression callee;
    public readonly List<Expression> args;

    public CallExpr(Expression c, params Expression[] a) {
        callee = c;
        args = a.ToList();
    }

    public CallExpr(Expression c, List<Expression> a) {
        callee = c;
        args = a;
    }
}

public class ParenthesizedExpr : Expression {
    public readonly Expression expr;

    public ParenthesizedExpr(Expression e) {
        expr = e;
    }
}

public class AssignExpr : Expression {
    public readonly Token name;
    public readonly Expression value;

    public AssignExpr(Token name, Expression value) {
        this.name = name;
        this.value = value;
    }
}