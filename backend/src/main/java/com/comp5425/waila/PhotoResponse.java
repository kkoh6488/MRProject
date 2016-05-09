package com.comp5425.waila;

/**
 * Created by marcuspaxton on 25/04/2016.
 */
public class PhotoResponse {

    private String name;
    private double score;

    public PhotoResponse() {
        this.name = "error";
        this.score = 0.0;
    }

    public PhotoResponse(String name, double score) {
        this.name = name;
        this.score = score;
    }

    public double getScore() {
        return score;
    }

    public void setScore(double score) {
        this.score = score;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
