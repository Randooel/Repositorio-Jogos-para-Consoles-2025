import java.util.Random;

public class ThreadPassenger extends Thread {			
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
