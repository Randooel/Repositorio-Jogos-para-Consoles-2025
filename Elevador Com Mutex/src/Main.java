import java.util.concurrent.Semaphore;

public class Main {
	
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
}
