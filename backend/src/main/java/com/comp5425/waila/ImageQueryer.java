package com.comp5425.waila;

import net.semanticmetadata.lire.builders.DocumentBuilder;
import net.semanticmetadata.lire.imageanalysis.features.global.FCTH;
import net.semanticmetadata.lire.searchers.GenericFastImageSearcher;
import net.semanticmetadata.lire.searchers.ImageSearchHits;
import net.semanticmetadata.lire.searchers.ImageSearcher;
import org.apache.lucene.index.DirectoryReader;
import org.apache.lucene.index.IndexReader;
import org.apache.lucene.store.Directory;
import org.apache.lucene.store.SimpleFSDirectory;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.nio.file.FileSystems;
import java.util.ArrayList;

/**
 * Created by marcuspaxton on 22/04/2016.
 */
public class ImageQueryer {

    private ImageSearcher searcher;
    private IndexReader ir;

    public ImageQueryer(String indexDirName) throws IOException {
        Directory dir = new SimpleFSDirectory(FileSystems.getDefault().getPath(indexDirName));
        this.ir = DirectoryReader.open(dir);
        this.searcher = new GenericFastImageSearcher(5, FCTH.class);
    }

    public ArrayList<PhotoResponse> query(byte[] image) throws IOException {
        BufferedImage bi = ImageIO.read(new ByteArrayInputStream(image));
        ImageSearchHits ish = searcher.search(bi, this.ir);

        ArrayList<PhotoResponse> toReturn = new ArrayList<>();

        for (int i = 0; i < ish.length(); i++) {
            String name = ir.document(ish.documentID(i)).getValues(DocumentBuilder.FIELD_NAME_IDENTIFIER)[0];
            double score = ish.score(i);
            toReturn.add(new PhotoResponse(name, score));
        }

        ir.close();

        return toReturn;
    }



}
