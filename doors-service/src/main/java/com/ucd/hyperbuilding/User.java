package com.ucd.hyperbuilding;

import java.text.SimpleDateFormat;
import java.time.Instant;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;

public class User {
    public User(String id, List<Date> enterBuildingDate, List<Date> leaveBuildingDate) {
        this.id = id;
        this.enterBuildingDate = enterBuildingDate;
        this.leaveBuildingDate = leaveBuildingDate;
    }

    public String id;
    public List<Date> enterBuildingDate;
    public List<Date> leaveBuildingDate;

    public boolean checkOut(String timeStamp) {

        Date date = Date.from(Instant.ofEpochMilli(Long.parseLong(timeStamp)));
        Date lastIn = !enterBuildingDate.isEmpty() ? enterBuildingDate.get(enterBuildingDate.size() - 1) : null;
        Date lastOut = !leaveBuildingDate.isEmpty() ? leaveBuildingDate.get(leaveBuildingDate.size() - 1) : null;
        if (lastOut == null) {
            leaveBuildingDate.add(date);
            return true;
        }
        boolean isIn = isIn(lastIn, lastOut);
        System.out.println("Cheking card id out, date=" + date + ", lastIn= =" + lastIn + "," +
                " lastOut=" + lastOut + ", isIn=" + isIn);
        if (!isIn) {
            return false;
        }
        leaveBuildingDate.add(date);
        return true;

    }

    public boolean checkIn(String timeStamp) {
        if (enterBuildingDate == null) {
            enterBuildingDate = new ArrayList<>();
            leaveBuildingDate = new ArrayList<>();
        }
        long timeLong = Long.parseLong(timeStamp);
        Date date = Date.from(Instant.ofEpochMilli(timeLong));
        Date lastIn = !enterBuildingDate.isEmpty() ? enterBuildingDate.get(enterBuildingDate.size() - 1) : null;
        Date lastOut = !leaveBuildingDate.isEmpty() ? leaveBuildingDate.get(leaveBuildingDate.size() - 1) : null;
        if (lastIn == null && lastOut == null) {
            enterBuildingDate.add(date);
            return true;
        }
        boolean isIn = isIn(lastIn, lastOut);
        if (isIn) {
            return false;
        }
        boolean canEnter = lastIn == null || (lastOut.after(lastIn) && date.after(lastOut));
        System.out.println("Cheking card id in, date=" + date + ", lastIn= =" + lastIn + "," +
                " lastOut=" + lastOut + ", isIn=" + isIn);
        if (canEnter) {
            enterBuildingDate.add(date);
        }
        return canEnter;
    }

    private static boolean isIn(Date lastIn, Date lastOut) {
        if (lastIn == null) {
            return false;
        }
        if (lastOut == null) {
            return true;
        }
        return lastIn.after(lastOut);

    }

    public static void prettyPrintUserMap(Map<String, User> userMap) {
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

        for (Map.Entry<String, User> entry : userMap.entrySet()) {
            System.out.println("User ID: " + entry.getKey());
            User user = entry.getValue();

            System.out.println("Enter Building Dates:");
            if (user.enterBuildingDate != null) {
                for (Date date : user.enterBuildingDate) {
                    System.out.println("  " + dateFormat.format(date));
                }
            }

            if (user.leaveBuildingDate != null) {
                System.out.println("Leave Building Dates:");
                for (Date date : user.leaveBuildingDate) {
                    System.out.println("  " + dateFormat.format(date));
                }
            }
            if (user.enterBuildingDate != null && user.leaveBuildingDate != null) {
                boolean isIn = isIn(!user.enterBuildingDate.isEmpty() ? user.enterBuildingDate.get(0) : null,
                        !user.leaveBuildingDate.isEmpty() ? user.leaveBuildingDate.get(0) : null);
                System.out.println("Is In: " + isIn);
            }
            System.out.println();
        }
    }
}
