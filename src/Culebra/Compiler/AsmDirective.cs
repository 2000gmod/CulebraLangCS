record AsmDirective {
    public AsmDirectiveType Type {get; private set;}
    public string LabelName {get; private set;}
    public UInt64 arg0 {get; private set;}
    public UInt64 arg1 {get; private set;}
    public UInt64 arg2 {get; private set;}

    public AsmDirective(string label) {
        Type = AsmDirectiveType.LABEL;
        LabelName = label;
    }

    public AsmDirective(AsmDirectiveType type, UInt64 a0 = 0, UInt64 a1 = 0, UInt64 a2 = 0) {
        Type = type;
        arg0 = a0;
        arg1 = a1;
        arg2 = a2;
    }
}