package com.comp5425.waila;

import net.semanticmetadata.lire.builders.DocumentBuilder;
import net.semanticmetadata.lire.builders.GlobalDocumentBuilder;
import net.semanticmetadata.lire.builders.LocalDocumentBuilder;
import net.semanticmetadata.lire.imageanalysis.features.global.AutoColorCorrelogram;
import net.semanticmetadata.lire.imageanalysis.features.global.CEDD;
import net.semanticmetadata.lire.imageanalysis.features.global.FCTH;
import net.semanticmetadata.lire.imageanalysis.features.global.Gabor;
import net.semanticmetadata.lire.imageanalysis.features.local.opencvfeatures.CvSurfExtractor;
import net.semanticmetadata.lire.imageanalysis.features.local.simple.SimpleExtractor;
import net.semanticmetadata.lire.indexers.parallel.ParallelIndexer;
import net.semanticmetadata.lire.utils.LuceneUtils;
import org.apache.lucene.document.Document;
import org.apache.lucene.index.DirectoryReader;
import org.apache.lucene.index.IndexWriter;
import org.apache.lucene.store.Directory;
import org.apache.lucene.store.SimpleFSDirectory;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.nio.file.FileSystems;


class ImageIndexer {

    private IndexWriter iw;
    private GlobalDocumentBuilder gdb;
    private LocalDocumentBuilder ldb;

    public ImageIndexer(String indexDirName) throws IOException {
        Directory dir = new SimpleFSDirectory(FileSystems.getDefault().getPath(indexDirName));
        this.iw = LuceneUtils.createIndexWriter(dir, !DirectoryReader.indexExists(dir),
                LuceneUtils.AnalyzerType.StandardAnalyzer);
        gdb = new GlobalDocumentBuilder(CEDD.class);
        gdb.addExtractor(FCTH.class);

        int[] numClusters = new int[] {128, 256};
        int numBOVW = 300;

        ParallelIndexer p = new ParallelIndexer(DocumentBuilder.NUM_OF_THREADS, "index", "test/", numClusters, numBOVW);
        p.addExtractor(CEDD.class);
        p.addExtractor(FCTH.class);

        p.addExtractor(CvSurfExtractor.class);

        p.addExtractor(CEDD.class, SimpleExtractor.KeypointDetector.CVSURF);

        p.run();
        System.out.println("Finished indexing");
        //gdb.addExtractor(CEDD.class, SimpleExtractor.KeypointDetector.CVSURF);

        //ldb = new LocalDocumentBuilder();

        //gdb.addExtractor(AutoColorCorrelogram.class);
    }

    /***
     *
     * @param imageData: Byte array of image to be indexed.
     * @param name: Name of object being imaged in the database.
     * @return successString: Success message to be returned on successful index.
     * @throws Exception
     */
    public String index(byte[] imageData, String name) throws Exception {
        //gdb.addExtractor(FCTH.class);
        //Global
        //gdb.addExtractor(CEDD.class);


        //Local
        //gdb.addExtractor(CvSurfExtractor.class);
        //gdb.addExtractor(CvSiftExtractor.class);
        //Simple
        //gdb.addExtractor(CEDD.class, SimpleExtractor.KeypointDetector.CVSURF);
        //gdb.addExtractor(JCD.class, SimpleExtractor.KeypointDetector.Random);

        BufferedImage bi = ImageIO.read(new ByteArrayInputStream(imageData));

        Document document = gdb.createDocument(bi, name);
        iw.addDocument(document);

        iw.commit();
        iw.close();

        return "Image just indexed: " + name;
    }


}
