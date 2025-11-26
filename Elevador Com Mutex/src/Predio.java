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
		semaforo = new Semaphore(1);
		
		// Instancia 1 elevador e o inicia
		elevator = new Elevator(this);
		elevator.start();
	}
	
	public void AddPassageriro(ThreadPassenger passageiro, int andarAtual, int andarDestino)
	{
		elevator.ChecarDisponibilidade();
		
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
				elevator.VisitarAndar(andarAtual);
				elevator.Embarcar(passageiro, andarDestino);
				
				PredioLog("SEMÁFORO DO PRÉDIO: " + passageiro.passengerName + "     Em: " + andarAtual + "     Indo para o andar: " + andarDestino);
				
				// Release é chamado pela função ReleaseSemaphore, garantindo que só será chamada quando o elevador liberar 1 epaço
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
