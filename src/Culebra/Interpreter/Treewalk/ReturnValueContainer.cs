namespace Culebra.Interpreter.Treewalk;

public class ReturnValueContainer : Exception {
    public RuntimeVariable value {get; private set;}

    public ReturnValueContainer(RuntimeVariable val) {
        value = val;
    }
}