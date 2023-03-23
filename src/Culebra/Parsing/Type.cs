namespace Culebra.Parsing;

[Serializable]
public abstract class Type { }

[Serializable]
public class ValueType : Type {
    public readonly Token name;

    public ValueType(Token n) {
        name = n;
    }
}

[Serializable]
public class PointerType : Type {
    public readonly Type pointedType;

    public PointerType(Type pointedType) {
        this.pointedType = pointedType;
    }
}