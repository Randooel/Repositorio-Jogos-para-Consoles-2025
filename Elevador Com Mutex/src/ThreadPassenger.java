import java.util.Random;

public class ThreadPassenger extends Thread {
	// Gets Main reference for floors
	Main main = new Main ();
			
	// Atributos do passageiro
	public int currentFloor;
	public int currentPosition;
	public int destinyFloor;
	
	// Instância da classe random para gerar valores aleatórios
	Random random = new Random();
	
	@Override
	public void run()
	{
		SetRandomFloor(main.maxFloors);
		PassengerLog();
	}
	
	// FUNCTIONS
	public void SetRandomFloor(int maxFloors)
	{
		currentFloor = random.nextInt(maxFloors + 1);
		
		destinyFloor = random.nextInt(maxFloors + 1);
	}
	
	public void PassengerLog()
	{
		System.out.println("Piso atual: " + currentFloor + "     Destino: " + destinyFloor);
	}
}
