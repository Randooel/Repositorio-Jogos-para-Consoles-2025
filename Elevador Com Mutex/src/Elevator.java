import java.util.List;
import java.util.ArrayList;

public class Elevator extends Thread
{	
	Predio predio;
	
	public float posY;
	public int currentFloor;
	public boolean isAvailable = true;
	
	// PASSAGEIROS
	public int qtdPassageiros;
	public int maxPassageiros;
	
	public int indexPassageiro;
	public List<ThreadPassenger> listaPassageiros = new ArrayList<>();
	
	public int andarDestino; // TODO: Transformar isso em uma lista para caber mais de 1 passageiro
	public ThreadPassenger passageiro;
	
	Main main = new Main();
	
	// Construtor da thread
	public Elevator(Predio pred)
	{		
		predio = pred;
		maxPassageiros = pred.maxPassageiros;
		
		// Começa sempre no andar 0
		VisitarAndar(0);
	}
	
	@Override
	public void run()
	{
		super.run();
	}
	
	public void Embarcar(ThreadPassenger pas, int proximoAndar)
	{
		Log("PASSAGEIRO ADICIONADO: " + pas.passengerName + "     Em: " + currentFloor + "     Indo para o andar: " + proximoAndar);
		
		listaPassageiros.add(pas);
		
		qtdPassageiros++;
		indexPassageiro++;
		
		andarDestino = proximoAndar;
		VisitarAndar(andarDestino);
	}
	
	public void ChecarDisponibilidade()
	{
		if(qtdPassageiros < maxPassageiros)
		{
			isAvailable = true;
		}
		else
		{
			isAvailable = false;
		}
		
		// Log("isAvailable = " + isAvailable);
	}
	
	public void AbrirPorta()
	{
		// TODO: Visual logic
		Log("Porta abriu.");
		
		if(passageiro != null)
		{
			if(currentFloor == andarDestino)
			{
				Desembarcar();
			}	
		}
	}
	
	public void FecharPorta()
	{
		// TODO: Visual logic
		Log("Porta fechou.");
		
	}
	
	public void VisitarAndar(int numAndar)
	{
		FecharPorta();
		
		// TODO: Movement logic	
		currentFloor = numAndar;
		Log("Elevador em: " + currentFloor);
		
		AbrirPorta();
	}
	
	public void Desembarcar()
	{
		//AbrirPorta();
		passageiro.chegouDestino = true;
		qtdPassageiros--;
		Log("Passageiro desceu.");
		
		predio.ReleaseSemaphore();
		
		ChecarDisponibilidade();
		VisitarAndar(1);
	}
	
	public void Log(String message)
	{
		System.out.println(message);
	}
}
