package com.elevadorComSemaforo;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.ScreenUtils;

/** {@link com.badlogic.gdx.ApplicationListener} implementation shared by all platforms. */
public class Main extends ApplicationAdapter {
    private SpriteBatch batch;
    private Texture image;
    
 // Define a quantidade de pisos que serão gerados
 	public static int maxPassengers = 10;
 	
 	public static void main(String args[])
 	{
 		// THREADS DE PASSAGEIROS
 		ThreadPassenger[] passengerThread = new ThreadPassenger[10];
 		
 		// PREDIO
 		Predio predio = new Predio();
 				
 		for (int i = 0; i < passengerThread.length; i++)
 		{
 			passengerThread[i] = new ThreadPassenger(predio);
 			
 			passengerThread[i].passengerName = "Passageiro_0" + i;			
 			passengerThread[i].start();
 		}
 		// FIM DE THREADS DE PASSAGEIROS
 	}

    @Override
    public void create() {
        batch = new SpriteBatch();
        image = new Texture("libgdx.png");
    }

    @Override
    public void render() {
        ScreenUtils.clear(0.15f, 0.15f, 0.2f, 1f);
        batch.begin();
        batch.draw(image, 140, 210);
        batch.end();
    }

    @Override
    public void dispose() {
        batch.dispose();
        image.dispose();
    }
}
