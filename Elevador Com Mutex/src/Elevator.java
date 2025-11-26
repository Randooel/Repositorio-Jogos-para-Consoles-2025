import java.util.List;
import java.util.ArrayList;

public class Elevator extends Thread
{	
	public float posY;
	public int currentFloor;
	public int passageiros;
	public boolean isAvailable = true;
	
	Main main = new Main();
	
	// Construtor da thread
	public Elevator(Predio predio)
	{		
		// Começa sempre no andar 0
		VisitarAndar(0);
		AbrirPorta();
	}
	
	@Override
	public void run()
	{
		super.run();
	}
	
	public void Embarcar(int proximoAndar)
	{
		
	}
	
	public void Desembarcar()
	{
		AbrirPorta();
		passageiros--;
		FecharPorta();
		VisitarAndar(1);
	}
	
	public void AbrirPorta()
	{
		// TODO: Visual logic
		LogElevador("Porta abriu.");
		
		FecharPorta();
	}
	
	public void FecharPorta()
	{
		// TODO: Visual logic
		LogElevador("Porta fechou.");
		
	}
	
	public void VisitarAndar(int numAndar)
	{
		// TODO: Movement logic
		
		if(isAvailable)
		{
			currentFloor = numAndar;
		}
		
		LogElevador("Elevador em: " + currentFloor);
		
		AbrirPorta();
	}
	
	public void LogElevador(String message)
	{
		System.out.println(message);
	}
}
