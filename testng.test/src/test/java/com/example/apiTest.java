package com.example;

import io.restassured.RestAssured;
import io.restassured.response.Response;

import java.util.List;
import java.util.Map;

import org.testng.Assert;
import org.testng.annotations.Test;
import io.restassured.path.json.JsonPath;

public class apiTest {

    @Test
    public void testGetEndpoint() {
        RestAssured.baseURI = "https://jsonplaceholder.typicode.com";
        
        Response response = RestAssured.given()
                                        .when()
                                        .get("/posts");
        
        Assert.assertEquals(response.getStatusCode(), 200, "Status code is not 200");
        String responseBody = response.getBody().asString();
        JsonPath jsonPath = new JsonPath(responseBody);
        List<?> responseList = jsonPath.getList("$");
        Assert.assertTrue(responseList instanceof List, "Response data is not an array");

        Map<String, Object> expectedPost = Map.of(
            "userId", 8,
            "id", 73,
            "title", "consequuntur deleniti eos quia temporibus ab aliquid at",
            "body", "voluptatem cumque tenetur consequatur expedita ipsum nemo quia explicabo\naut eum minima consequatur\ntempore cumque quae est et\net in consequuntur voluptatem voluptates aut"
        );

        boolean containsExpectedPost = responseList.stream()
            .anyMatch(post -> post instanceof Map && ((Map<?, ?>) post).equals(expectedPost));
        
        Assert.assertTrue(containsExpectedPost, "The response does not contain the expected post");
     
    }
}
