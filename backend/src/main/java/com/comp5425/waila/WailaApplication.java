package com.comp5425.waila;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.ComponentScan;

@ComponentScan
@SpringBootApplication
public class WailaApplication {
    // Runs the server using Sprint Boot
	public static void main(String[] args) {
		SpringApplication.run(WailaApplication.class, args);
	}
}
