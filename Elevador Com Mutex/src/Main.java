
public class Main {
	
	// Define a quantidade de pisos que serão gerados
	public static int maxFloors = 5;
	
	public static void main(String args[])
	{
		// ATRIBUTOS
		// Define o número máximo de passageiros NOS ANDARES
		int maxPassengers = 5;
		
		
		// Cria as threads dos passageiros
		ThreadPassenger[] passengerThread = new ThreadPassenger[maxPassengers];
		
		for (int i = 0; i < passengerThread.length; i++)
		{
			passengerThread[i] = new ThreadPassenger();
			
			passengerThread[i].start();
		}
	}
}
