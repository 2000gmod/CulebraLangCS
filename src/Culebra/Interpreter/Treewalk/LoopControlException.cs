namespace Culebra.Interpreter.Treewalk;

class LoopControlException : Exception {
    public readonly bool isContinue;

    public LoopControlException(bool isContinue) {
        this.isContinue = isContinue;
    }
}