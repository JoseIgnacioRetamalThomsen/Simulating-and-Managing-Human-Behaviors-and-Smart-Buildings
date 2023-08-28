package com.ucd.hyperbuilding;
import astra.core.Agent;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;

import java.util.Optional;

public interface AgentEnhancer {
    boolean isDebug = true;

    Agent getAgent();

    default void log(String message, String... parameters) {
        if (!isDebug) {
            return;
        }
        if (message == null) {
            return;
        }

        if (parameters == null || parameters.length == 0) {
            System.out.println("[RP:" + getAgent().name() + "] " + message);
            return;
        }

        String[] parts = message.split("\\{\\}");
        StringBuilder formattedMessage = new StringBuilder(parts[0]);

        for (int i = 0; i < parameters.length; i++) {
            formattedMessage.append(parameters[i]);
            if (i + 1 < parts.length) {
                formattedMessage.append(parts[i + 1]);
            }
        }

        System.out.println("[RP:" + getAgent().name() + "] " + formattedMessage);
    }

    default void dropBelief(String predicate) {
        Optional<Predicate> isParsingPredicate = getAgent().beliefs().beliefs().stream()
                .filter(f -> f instanceof Predicate)
                .map(f -> (Predicate) f)
                .filter(p -> p.predicate().equals(predicate))
                .findFirst();

        isParsingPredicate.ifPresent(getAgent().beliefs()::dropBelief);
    }

    default void setBelief(String predicate, Object... primitives) {
        Term[] terms = new Term[primitives.length];

        for (int i = 0; i < primitives.length; i++) {
            terms[i] = toTerm(primitives[i]);
        }

        getAgent().beliefs().addBelief(new Predicate(predicate, terms));
    }

    default Term toTerm(Object value) {
        if (value instanceof String) {
            return Primitive.newPrimitive((String) value);
        } else if (value instanceof Boolean) {
            return Primitive.newPrimitive((Boolean) value);
        } else if (value instanceof Integer) {
            return Primitive.newPrimitive((Integer) value);
        } else if (value instanceof Float) {
            return Primitive.newPrimitive((Float) value);
        }

        throw new IllegalArgumentException("Unsupported type: " + value.getClass());
    }
}
