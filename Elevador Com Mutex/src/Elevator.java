import java.util.List;
import java.util.ArrayList;

public class Elevator extends Thread
{	
	Predio predio;
	
	public float posY;
	public int currentFloor;
	public int passageiros;
	public int maxPassageiros = 1;
	public boolean isAvailable = true;
	
	// PASSAGEIROS
	public int andarDestino; // TODO: Transformar isso em uma lista para caber mais de 1 passageiro
	
	Main main = new Main();
	
	// Construtor da thread
	public Elevator(Predio pred)
	{		
		predio = pred;
		
		// Começa sempre no andar 0
		VisitarAndar(0);
	}
	
	@Override
	public void run()
	{
		super.run();
	}
	
	public void Embarcar(int proximoAndar)
	{
		passageiros++;
		VisitarAndar(proximoAndar);
	}
	
	public void Desembarcar()
	{
		AbrirPorta();
		passageiros--;
		FecharPorta();
		
		predio.ReleaseSemaphore();
		
		ChecarDisponibilidade();
		VisitarAndar(1);
	}
	
	public void ChecarDisponibilidade()
	{
		if(passageiros < maxPassageiros)
		{
			isAvailable = true;
		}
		else
		{
			isAvailable = false;
		}
	}
	
	public void AbrirPorta()
	{
		// TODO: Visual logic
		Log("Porta abriu.");
		
		if(currentFloor == andarDestino)
		
		FecharPorta();
	}
	
	public void FecharPorta()
	{
		// TODO: Visual logic
		Log("Porta fechou.");
		
	}
	
	public void VisitarAndar(int numAndar)
	{
		// TODO: Movement logic
		
		currentFloor = numAndar;
		
		Log("Elevador em: " + currentFloor);
		
		AbrirPorta();
	}
	
	public void Log(String message)
	{
		System.out.println(message);
	}
}
