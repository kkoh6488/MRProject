package com.comp5425.waila;

import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.ArrayList;
import java.util.Base64;
import java.util.List;

import static org.springframework.web.bind.annotation.RequestMethod.GET;
import static org.springframework.web.bind.annotation.RequestMethod.POST;


/**
 * Created by marcuspaxton on 22/04/2016.
 */

@RestController
public class SubmitController {
    
    // Controller class defining API endpoints and behavious
    
    private final String submitPath = "/submit";
    private final String queryPath = "/query";
    private final String knowledgePath = "/knowledge";
    private final String testPath = "/test";

    // Endpoint used for testing image retrieval results
    @RequestMapping(value = testPath, method = POST)
    public String test(@RequestBody PhotoRequest pr) {
        try {
            String img = pr.getImageData();
            System.out.println(img);
            byte[] imgData = Base64.getDecoder().decode(img);
            ImageQueryer iq = new ImageQueryer("index");
            ArrayList<PhotoResponse> results = iq.query(imgData);

            for (PhotoResponse p : results) {
                System.out.println(String.format("Name: %s, Score; %f", p.getName(), p.getScore()));
            }

            return results.get(0).getName();

        } catch (Exception e) {
            e.printStackTrace();
            return "The server has encountered an error.";
        }


    }
    
    // Handles images being posted for indexing to the server.
    @RequestMapping(value = submitPath, method = POST)
    public String post(@RequestBody PhotoRequest pr) {
        try {

            String image = pr.getImageData();
            byte[] imgData = Base64.getDecoder().decode(image);
            String name = pr.getName();
            ImageIndexer ii = new ImageIndexer("index");

            return ii.index(imgData, name);

        } catch(Exception e) {
            e.printStackTrace();
            return "The server has encountered an error.";
        }
    }
    
    // Queries an image, returning the Google Knowledge Graph results
    @RequestMapping(value = queryPath, method = POST)
    public List<Object> query(@RequestBody PhotoRequest pr) {
        try {
            String img = pr.getImageData();
            System.out.println(img);
            byte[] imgData = Base64.getDecoder().decode(img);
            ImageQueryer iq = new ImageQueryer("index");
            ArrayList<PhotoResponse> results = iq.query(imgData);

            if (results.size() > 0) {
                // Query the info for the most relevant
                InfoQueryer info = new InfoQueryer();

                return info.search(results.get(0).getName());
            } else {
                return new ArrayList<>();
            }

        } catch (Exception e) {
            return new ArrayList<>();
        }

    }
    
    // Test endpoint for querying knowledge graph directly
    @RequestMapping(value = knowledgePath, method = GET )
    public List<Object> query(@RequestParam("entity") String entity) {
        try {
            InfoQueryer iq = new InfoQueryer();
            return iq.search(entity);
        } catch (Exception e) {
            e.printStackTrace();
            return new ArrayList<>();
        }
    }

}
