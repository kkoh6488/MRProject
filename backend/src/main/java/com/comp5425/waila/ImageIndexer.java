package com.comp5425.waila;

import net.semanticmetadata.lire.builders.GlobalDocumentBuilder;
import net.semanticmetadata.lire.imageanalysis.features.global.CEDD;
import net.semanticmetadata.lire.imageanalysis.features.global.FCTH;
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

    public ImageIndexer(String indexDirName) throws IOException {
        Directory dir = new SimpleFSDirectory(FileSystems.getDefault().getPath(indexDirName));
        this.iw = LuceneUtils.createIndexWriter(dir, !DirectoryReader.indexExists(dir),
                LuceneUtils.AnalyzerType.StandardAnalyzer);
    }

    /***
     *
     * @param imageData: Byte array of image to be indexed.
     * @param name: Name of object being imaged in the database.
     * @return successString: Success message to be returned on successful index.
     * @throws Exception
     */
    public String index(byte[] imageData, String name) throws Exception {

        GlobalDocumentBuilder gdb = new GlobalDocumentBuilder(CEDD.class);
        gdb.addExtractor(FCTH.class);
        BufferedImage bi = ImageIO.read(new ByteArrayInputStream(imageData));

        Document document = gdb.createDocument(bi, name);
        iw.addDocument(document);

        iw.commit();
        iw.close();

        return "Image just indexed: " + name;
    }


}
