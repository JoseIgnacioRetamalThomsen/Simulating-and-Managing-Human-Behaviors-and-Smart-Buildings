package com.ucd.hyperbuilding;

import astra.core.ActionParam;
import astra.core.Module;

import java.text.SimpleDateFormat;
import java.time.Instant;
import java.util.*;

public class MockDAOClass extends Module {

    private Map<String, User> userData = new HashMap<String, User>() {
        {
            put("001", new User("001", null, null));
            put("002", new User("002", null, null));
            put("003", new User("003", null, null));
            put("004", new User("004", null, null));
            put("005", new User("005", null, null));
            put("006", new User("006", null, null));
            put("007", new User("007", null, null));
            put("008", new User("008", null, null));
            put("009", new User("009", null, null));
            put("010", new User("010", null, null));
            put("123", new User("123", null, null));
        }
    };
    @ACTION
    public boolean checkIn(String id, String timeStamp, ActionParam<Boolean> accepted) {
        System.out.println("Check in, id=" + id + ", time=" + timeStamp);
        if (!userData.containsKey(id)) {
            accepted.set(false);
            return true;
        }
        User user = userData.get(id);
        boolean canEnter = user.checkIn(timeStamp);
        accepted.set(canEnter);
        User.prettyPrintUserMap(userData);
        return true;
    }

    @ACTION
    public boolean checkOut(String id, String timeStamp, ActionParam<Boolean> accepted) {
        if (!userData.containsKey(id)) {
            accepted.set(false);
            return true;
        }
        User user = userData.get(id);
        boolean canEnter = user.checkOut(timeStamp);
        accepted.set(canEnter);
        return true;
    }
}

