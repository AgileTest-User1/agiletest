package com.example;

import org.testng.Assert;
import org.testng.annotations.Test;

public class calculateTest {

    public int subtract(int a, int b) {
        return a - b;
    }
    public int add(int a, int b) {
        return a + b;
    }

    @Test
    public void testSubtract() {
        calculateTest app = new calculateTest();
        int result = app.subtract(10, 5);
        Assert.assertEquals(result, 5, "Subtraction result is incorrect");
    }

    @Test
    public void testAdd() {
        calculateTest app = new calculateTest();
        int result = app.add(10, 5);
        Assert.assertEquals(result, 15, "Add result is incorrect");
    }
}
