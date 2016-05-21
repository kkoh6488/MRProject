package com.comp5425.waila;

import net.semanticmetadata.lire.aggregators.BOVW;
import net.semanticmetadata.lire.builders.DocumentBuilder;
import net.semanticmetadata.lire.imageanalysis.features.global.CEDD;
import net.semanticmetadata.lire.imageanalysis.features.global.FCTH;
import net.semanticmetadata.lire.imageanalysis.features.local.simple.SimpleExtractor;
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
        //this.searcher = new GenericFastImageSearcher(5, CEDD.class);
        searcher = new GenericFastImageSearcher(1000, CEDD.class, SimpleExtractor.KeypointDetector.CVSURF, new BOVW(), 128, true, ir, dir + ".config");

    }

    public ArrayList<PhotoResponse> query(byte[] image) throws IOException {
        BufferedImage bi = ImageIO.read(new ByteArrayInputStream(image));
        ImageSearchHits ish = searcher.search(bi, this.ir);

        ArrayList<PhotoResponse> toReturn = new ArrayList<>();
        int bestMatchIndex = 0;
        double _lastScore = 0;
        PhotoResponse pr = null;
        for (int i = 0; i < ish.length(); i++)
        {
            String name = ir.document(ish.documentID(i)).getValues(DocumentBuilder.FIELD_NAME_IDENTIFIER)[0];
            double score = ish.score(i);
            System.out.println("Hit : " + name + ", " + score);
            if (score > _lastScore) {
                pr = new PhotoResponse(name, score);
                _lastScore = score;
                System.out.print("Found better score:" + name);
            }
            //toReturn.add(new PhotoResponse(name, score));
        }

        ir.close();
        toReturn.add(pr);
        System.out.println("toReturn size: " + toReturn.size() + ", " + toReturn.get(0).getName());
        return toReturn;
        //return
    }



}
