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
	
	public ThreadPassenger[] p;
	public int pA;
	
	public int andarDestino; // TODO: Transformar isso em uma lista para caber mais de 1 passageiro
	
	Main main = new Main();
	
	// Construtor da thread
	public Elevator(Predio pred)
	{		
		predio = pred;
		maxPassageiros = pred.maxPassageiros;
		
		p = new ThreadPassenger[maxPassageiros];
		pA = 0;
		
		// Começa sempre no andar 0
		VisitarAndar(0);
	}
	
	@Override
	public void run()
	{
		super.run();
	}
	
	// FUNÇÕES DO ELEVADOR
	
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
	
	public void Embarcar(ThreadPassenger pas, int proximoAndar)
	{
		Log("PASSAGEIRO ADICIONADO: " + pas.passengerName + "     Em: " + currentFloor + "     Indo para o andar: " + proximoAndar);
		
		AddPassageiro(pas);
		
		qtdPassageiros++;
		
		andarDestino = proximoAndar;
		VisitarAndar(andarDestino);
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
		
		qtdPassageiros--;
		RemoverPassageiro();
		ChecarDisponibilidade();
		
		Log("Passageiro desceu.");
		
		predio.ReleaseSemaphore();
		
		VisitarAndar(1);
	}
	
	
	
	void AddPassageiro(ThreadPassenger pas)
	{
		p[pA] = pas;
	}
	
	void RemoverPassageiro()
	{
		p[pA] = null;
		
		if(pA < p.length - 1)
		{
			pA++;
		}
		else
		{
			pA = 0;
		}
	}
	
	public void AbrirPorta()
	{
		// TODO: Visual logic
		Log("Porta abriu.");
		
		if(p[pA] != null)
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
	
	
	
	public void Log(String message)
	{
		System.out.println(message);
	}
}
