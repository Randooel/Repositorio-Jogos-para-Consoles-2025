package com.elevadorComSemaforo;

import java.util.Random;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.files.FileHandle;

public class ThreadPassenger extends Thread {			
	
	// Visual
	Texture texture;
	Sprite sprite;
	Vector2 speed;
	
	// Atributos do passageiro
	public String passengerName;
	public int currentFloor;
	public int destinyFloor;
	
	public boolean chegouDestino;
	
	Predio predio;
	
	// Instância da classe random para gerar valores aleatórios
	Random random = new Random();
	
	// CONSTRUTOR
	public ThreadPassenger(Predio pred)
	{
		predio = pred;
		SetRandomFloor(pred.andares);
		
		// Associando imagem à textura e textura ao sprite
		//texture = new Texture("Passageiro.png");
		
		System.out.println("Internal files root: " + Gdx.files.getLocalStoragePath());

		FileHandle[] list = Gdx.files.internal("").list();
		for (FileHandle fh : list) {
		    System.out.println("Found in assets: " + fh.name());
		}
		
		sprite = new Sprite(new Texture("Passageiro.png"));
	}
	
	@Override
	public void run()
	{
		while (chegouDestino == false)
		{
			// TENTA ACESSAR O VISITAR ANDAR DO ELEVADOR
			predio.AddPassageriro(this, currentFloor, destinyFloor);
			
			try 
			{
			    Thread.sleep(1000);
			} 
			catch (InterruptedException e)
			{
			    e.printStackTrace();
			}
		}
		
	}
	
	// FUNÇÕES
	public void SetRandomFloor(int maxFloors)
	{
		currentFloor = random.nextInt(maxFloors + 1);
		
		destinyFloor = random.nextInt(maxFloors + 1);
		
		// Evita que o passageiro sorteie, como destino, o mesmo andar em que já está
		if(destinyFloor == currentFloor)
		{
			if(currentFloor < maxFloors)
			{
				destinyFloor++;
			}
			else
			{
				destinyFloor--;
			}
		}
	}
	
	public void PassengerLog()
	{
		System.out.println(passengerName + "     Piso atual: " + currentFloor 
				+ "     Destino: " + destinyFloor);
	}
}
