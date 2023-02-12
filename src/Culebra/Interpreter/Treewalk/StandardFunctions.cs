namespace Culebra.Interpreter.Treewalk;

using Culebra.Parsing;
public static partial class StandardFunctions {
    public static ReturnValueContainer print(TreewalkInterpreter trw, List<Expression> args) {
        foreach (var arg in args) {
            Console.Write(trw.evaluateExpr(arg));
        }
        return new ReturnValueContainer(null);
    }
}