package com.comp5425.waila;

import com.google.api.client.googleapis.javanet.GoogleNetHttpTransport;
import com.google.api.client.json.jackson2.JacksonFactory;
import com.google.api.services.kgsearch.v1.Kgsearch;
import com.google.api.services.kgsearch.v1.model.SearchResponse;

import java.io.IOException;
import java.security.GeneralSecurityException;
import java.util.List;

/**
 * Created by marcuspaxton on 25/04/2016.
 */
public class InfoQueryer {
    // Google API data
    private final String API_KEY = "AIzaSyBibE-WePXOpZ3lWfMXlW2_JDSrIbSYiq0";
    private final String APPLICATION_NAME = "multimedia-retrieval-1292";

    private Kgsearch search;

    public InfoQueryer() throws IOException, GeneralSecurityException {
        this.search = new Kgsearch.Builder(GoogleNetHttpTransport.newTrustedTransport(), new JacksonFactory(), null)
                .setApplicationName(APPLICATION_NAME)
                .build();
    }

    public List<Object> search(String entity) throws IOException {
        Kgsearch.Entities.Search request = this.search.entities().search();
        request = request.setQuery(entity);
        request = request.setKey(API_KEY);
        SearchResponse response = request.execute();
        // Get google response and return list of JSON objects
        return response.getItemListElement();
    }
}
