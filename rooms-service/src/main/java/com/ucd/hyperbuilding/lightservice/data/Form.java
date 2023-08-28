package com.ucd.hyperbuilding.lightservice.data;

import java.util.List;

public class Form {
    public List<String> op;
    public String Href;
    public String CovMethodName;
    public String Subprotocol;
    public String ContentType;

    @Override
    public String toString() {
        return "Form{" +
                "op=" + op +
                ", Href='" + Href + '\'' +
                ", CovMethodName='" + CovMethodName + '\'' +
                ", Subprotocol='" + Subprotocol + '\'' +
                ", ContentType='" + ContentType + '\'' +
                '}';
    }
}
