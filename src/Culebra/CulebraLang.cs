namespace Culebra;

using Culebra.Interpreter.Treewalk;


public class CulebraLang {
    public static int Main(string[] args) {
        TreewalkInterpreter treewalk = new("examples/hello.clb");
        treewalk.run();
        return 0;
    }
}