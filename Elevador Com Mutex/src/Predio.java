import java.util.concurrent.Semaphore;

public class Predio 
{
	// Referências a outras classes
	Elevator elevator;
	
	Semaphore semaforo;
	
	// Propriedades
	public int andares;
	
	// Construtor do prédio
	public Predio()
	{
		// Configura propriedades do predio
		andares = 5;
		semaforo = new Semaphore (1);
		
		// Instancia 1 elevador e o inicia
		elevator = new Elevator(this);
		elevator.start();
	}
	
	public void AddPassageriro(ThreadPassenger passageiro, int andar)
	{
		if(elevator.isAvailable)
		{
			if(elevator.isAvailable)
			{
				try
				{
					semaforo.acquire();
				}
				catch (InterruptedException e)
				{
					e.printStackTrace();
				}
				
				elevator.passageiros++;
				elevator.VisitarAndar(andar);
				
				PredioLog("SEMÁFORO DO PRÉDIO: " + passageiro.passengerName + "     Indo para o andar: " + andar);
			}
		}
	}
	
	public void ReleaseSemaphore()
	{
		semaforo.release();
	}
	
	public void PredioLog(String message)
	{
		System.out.println(message);
	}
}
