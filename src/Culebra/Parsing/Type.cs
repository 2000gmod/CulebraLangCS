namespace Culebra.Parsing;

public abstract class Type { }

public class ValueType : Type {
    public readonly Token name;

    public ValueType(Token n) {
        name = n;
    }
}

public class PointerType : Type {
    public readonly Type pointedType;

    public PointerType(Type pointedType) {
        this.pointedType = pointedType;
    }
}