namespace Culebra.Interpreter.Treewalk;

public enum PVActiveType {
    INT,
    DOUBLE,
    STRING,
    BOOL
}

public class PrimitiveVar : RuntimeVariable {
    public PVActiveType atype;

    public int intValue;
    public double doubleValue;
    public string stringValue;
    public bool boolValue;

    public PrimitiveVar(PrimitiveVar other) {
        atype = other.atype;
        intValue = other.intValue;
        doubleValue = other.doubleValue;
        stringValue = other.stringValue;
        boolValue = other.boolValue;
    }

    public PrimitiveVar(int val) {
        atype = PVActiveType.INT;
        intValue = val;
    }

    public PrimitiveVar(double val) {
        atype = PVActiveType.DOUBLE;
        doubleValue = val;
    }

    public PrimitiveVar(string val) {
        atype = PVActiveType.STRING;
        stringValue = val;
    }

    public PrimitiveVar(bool val) {
        atype = PVActiveType.BOOL;
        boolValue = val;
    }

    public static PrimitiveVar operator +(PrimitiveVar l, PrimitiveVar r) {
        switch (l.atype) {
            case PVActiveType.INT:
                if (r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.intValue + r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.intValue + r.doubleValue);
                }
                else break;
            case PVActiveType.DOUBLE:
                if(r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.doubleValue + r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.doubleValue + r.doubleValue);
                }
                else break;
            case PVActiveType.STRING:
                if (r.atype == PVActiveType.STRING) {
                    return new PrimitiveVar(l.stringValue + r.stringValue);
                }
                else break;
            case PVActiveType.BOOL:
                break;
        }
        ErrorReporter.reportError("Invalid types for (+) operator.");
        return null;
    }

    public static PrimitiveVar operator -(PrimitiveVar l, PrimitiveVar r) {
        switch (l.atype) {
            case PVActiveType.INT:
                if (r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.intValue - r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.intValue - r.doubleValue);
                }
                else break;
            case PVActiveType.DOUBLE:
                if(r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.doubleValue - r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.doubleValue - r.doubleValue);
                }
                else break;
            case PVActiveType.STRING:
                break;
            case PVActiveType.BOOL:
                break;
        }
        ErrorReporter.reportError("Invalid types for (-) operator.");
        return null;
    }

    public static PrimitiveVar operator *(PrimitiveVar l, PrimitiveVar r) {
        switch (l.atype) {
            case PVActiveType.INT:
                if (r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.intValue * r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.intValue * r.doubleValue);
                }
                else break;
            case PVActiveType.DOUBLE:
                if(r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.doubleValue * r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.doubleValue * r.doubleValue);
                }
                else break;
            case PVActiveType.STRING:
                break;
            case PVActiveType.BOOL:
                break;
        }
        ErrorReporter.reportError("Invalid types for (*) operator.");
        return null;
    }

    public static PrimitiveVar operator /(PrimitiveVar l, PrimitiveVar r) {
        switch (l.atype) {
            case PVActiveType.INT:
                if (r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.intValue / r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.intValue / r.doubleValue);
                }
                else break;
            case PVActiveType.DOUBLE:
                if(r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.doubleValue / r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.doubleValue / r.doubleValue);
                }
                else break;
            case PVActiveType.STRING:
                break;
            case PVActiveType.BOOL:
                break;
        }
        ErrorReporter.reportError("Invalid types for (/) operator.");
        return null;
    }

    public static PrimitiveVar operator %(PrimitiveVar l, PrimitiveVar r) {
        switch (l.atype) {
            case PVActiveType.INT:
                if (r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.intValue % r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.intValue % r.doubleValue);
                }
                else break;
            case PVActiveType.DOUBLE:
                if(r.atype == PVActiveType.INT) {
                    return new PrimitiveVar(l.doubleValue % r.intValue);
                }
                else if (r.atype == PVActiveType.DOUBLE) {
                    return new PrimitiveVar(l.doubleValue % r.doubleValue);
                }
                else break;
            case PVActiveType.STRING:
                break;
            case PVActiveType.BOOL:
                break;
        }
        ErrorReporter.reportError("Invalid types for (%) operator.");
        return null;
    }

    public static PrimitiveVar operator -(PrimitiveVar r) {
        switch (r.atype) {
            case PVActiveType.INT:
                return new PrimitiveVar(-r.intValue);
            case PVActiveType.DOUBLE:
                return new PrimitiveVar(-r.doubleValue);
            default:
                break;
        }
        ErrorReporter.reportError("Invalid types for (unary -) operator.");
        return null;
    }

    public static PrimitiveVar operator !(PrimitiveVar r) {
        switch (r.atype) {
            case PVActiveType.BOOL:
                return new PrimitiveVar(!r.boolValue);
            default:
                break;
        }
        ErrorReporter.reportError("Invalid types for (unary !) operator.");
        return null;
    }

    public static bool operator ==(PrimitiveVar l, PrimitiveVar r) {
        if (l.atype != r.atype) {
            ErrorReporter.reportError("Comparing different types.");
        }

        switch (l.atype) {
            case PVActiveType.INT:
                return l.intValue == r.intValue;
            case PVActiveType.DOUBLE:
                return l.doubleValue == r.doubleValue;
            case PVActiveType.STRING:
                return l.stringValue == r.stringValue;
            case PVActiveType.BOOL:
                return l.boolValue == r.boolValue;
        }
        return false;
    }

    public static bool operator !=(PrimitiveVar l, PrimitiveVar r) {
        return !(l == r);
    }

    public static bool operator <(PrimitiveVar l, PrimitiveVar r) {
        if (!(l.atype == PVActiveType.INT || l.atype == PVActiveType.DOUBLE) && (r.atype == PVActiveType.INT || r.atype == PVActiveType.DOUBLE)) {
            ErrorReporter.reportError("Comparasion between incompatible types.");
        }

        if (l.atype == PVActiveType.INT) {
            if (r.atype == PVActiveType.INT) {
                return l.intValue < r.intValue;
            }
            else return l.intValue < r.doubleValue;
        }
        if (l.atype == PVActiveType.DOUBLE) {
            if (r.atype == PVActiveType.INT) {
                return l.doubleValue < r.intValue;
            }
            else return l.doubleValue < r.doubleValue;
        }
        return false;
    }

    public static bool operator >(PrimitiveVar l, PrimitiveVar r) {
        return !(l < r) && l != r;
    }

    public static bool operator >=(PrimitiveVar l, PrimitiveVar r) {
        return !(l < r);
    }

    public static bool operator <=(PrimitiveVar l, PrimitiveVar r) {
        return !(l > r);
    }

    public static bool logicalOr(PrimitiveVar l, PrimitiveVar r) {
        if (l.atype != PVActiveType.BOOL || r.atype != PVActiveType.BOOL) {
            ErrorReporter.reportError("Invalid types for (or) operator.");
            return false;
        }

        return l.boolValue || r.boolValue;
    }

    public static bool logicalAnd(PrimitiveVar l, PrimitiveVar r) {
        if (l.atype != PVActiveType.BOOL || r.atype != PVActiveType.BOOL) {
            ErrorReporter.reportError("Invalid types for (and) operator.");
            return false;
        }

        return l.boolValue && r.boolValue;
    }

    public override bool Equals(object obj){
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public bool isTrue() {
        return atype == PVActiveType.BOOL && boolValue;
    }

    public override string ToString() {
        switch (atype) {
            case PVActiveType.INT:
                return intValue.ToString();
            case PVActiveType.DOUBLE:
                return doubleValue.ToString();
            case PVActiveType.STRING:
                return stringValue.ToString();
            case PVActiveType.BOOL:
                return boolValue.ToString();
        }
        return "";
    }
}