package com.comp5425.waila;

/**
 * Created by marcuspaxton on 22/04/2016.
 */
public class PhotoRequest {
    // A simple wrapper for the photorequest data - used by Spring

    private String imageData;
    private String name;

    public PhotoRequest() {
        imageData = "Please provide a photo string.";
    }

    public PhotoRequest(String imageData) {
        this.imageData = imageData;
    }

    public PhotoRequest(String imageData, String name) {
        this.imageData = imageData;
        this.name = name;
    }

    public String getImageData() {
        return this.imageData;
    }

    public String getName() { return this.name; }
}
