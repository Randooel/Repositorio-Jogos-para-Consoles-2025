import java.util.Random;

public class ThreadPassenger extends Thread {			
	// Atributos do passageiro
	public String passengerName;
	public int currentFloor;
	public int destinyFloor;
	
	Predio predio;
	
	// Instância da classe random para gerar valores aleatórios
	Random random = new Random();
	
	// CONSTRUTOR
	public ThreadPassenger(Predio pred)
	{
		predio = pred;
		SetRandomFloor(pred.andares);
	}
	
	@Override
	public void run()
	{
		while (true)
		{
			// TENTA ACESSAR O VISITAR ANDAR DO ELEVADOR
			predio.AddPassageriro(this, currentFloor, destinyFloor);
			
			/*
			try {
			    Thread.sleep(1000);
			} catch (InterruptedException e) {
			    e.printStackTrace();
			}
			*/
		}
		
	}
	
	// FUNÇÕES
	public void SetRandomFloor(int maxFloors)
	{
		currentFloor = random.nextInt(maxFloors + 1);
		
		destinyFloor = random.nextInt(maxFloors + 1);
	}
	
	public void PassengerLog()
	{
		System.out.println(passengerName + "     Piso atual: " + currentFloor 
				+ "     Destino: " + destinyFloor);
	}
}
