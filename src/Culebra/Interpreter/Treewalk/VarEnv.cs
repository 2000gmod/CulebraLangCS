namespace Culebra.Interpreter.Treewalk;

using Culebra.Parsing;

public class VarEnv {
    private Dictionary<string, RuntimeVariable> vars;
    private Dictionary<string, FuncDeclarationStmt> functions;

    public VarEnv enclosing;

    public VarEnv(VarEnv enclosing = null) {
        vars = new();
        functions = new();
        this.enclosing = enclosing;
    }

    public VarEnv parentEnv(int dist) {
        if (dist == 0) return this;
        return enclosing.parentEnv(dist - 1);
    }

    private bool isNameUsed(string name) {
        if (enclosing == null) return vars.ContainsKey(name) || functions.ContainsKey(name);
        return (vars.ContainsKey(name) || functions.ContainsKey(name) || enclosing.isNameUsed(name) );
    }

    public void defineFunc(string name, FuncDeclarationStmt val) {
        if (!functions.TryAdd(name, val)) {
            ErrorReporter.reportError($"ERROR: Runtime error: tried to add a function whose name is already defined ({name}).");
        }
    }

    public void defineVar(string name, RuntimeVariable val) {
        if (!vars.TryAdd(name, val)) {
            ErrorReporter.reportError($"ERROR: Runtime error: Tried to add a variable whose name is already defined ({name}).");
        }
    }

    public RuntimeVariable getVar(string name) {
        if (!vars.ContainsKey(name)) {
            if (enclosing == null) ErrorReporter.reportError($"ERROR: Runtime error: Variable not found: ({name}).");
            return enclosing.getVar(name);
        }
        return vars[name];
    }

    public FuncDeclarationStmt getFunc(string name) {
        if (!functions.ContainsKey(name)) {
            if (enclosing == null) ErrorReporter.reportError($"ERROR: Runtime error: Function not found: ({name}).");
            return enclosing.getFunc(name);
        }
        return functions[name];
    }

    public void assignVar(string name, RuntimeVariable val) {
        if (!vars.ContainsKey(name)) {
            if (enclosing == null) ErrorReporter.reportError($"ERROR: Runtime error: Variable not found (1): ({name}).");
            enclosing.assignVar(name, val);
        }
        vars[name] = val;
    }
}