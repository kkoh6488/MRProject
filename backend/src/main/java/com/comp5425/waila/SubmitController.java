package com.comp5425.waila;

import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.File;
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

    private final String submitPath = "/submit";
    private final String queryPath = "/query";
    private final String knowledgePath = "/knowledge";

    @RequestMapping(value = submitPath, method = POST)
    public String post(@RequestBody PhotoRequest pr) {
        // this will eventually need to:
        // - decode base64 encoding of image data
        // - push byte array into the indexer
        // - retrieve image identifier, if any
        // - notify of success or failure.
        try {

            String image = pr.getImageData();
            byte[] imgData = Base64.getDecoder().decode(image);
            BufferedImage img = ImageIO.read(new ByteArrayInputStream(imgData));
            ImageIO.write(img, "JPG", new File("test.jpg"));
            System.out.println("Wrote to file");
            String name = pr.getName();
            ImageIndexer ii = new ImageIndexer("index");



            return ii.index(imgData, name);

        } catch(Exception e) {
            e.printStackTrace();
            return "The server has encountered an error.";
        }
    }

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
                System.out.println("Info query");
                return info.search(results.get(0).getName());
            } else {
                return new ArrayList<>();
            }

        } catch (Exception e) {
            e.printStackTrace();
            return new ArrayList<>();
        }

    }

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
