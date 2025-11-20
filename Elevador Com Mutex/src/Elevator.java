import java.util.List;
import java.util.ArrayList;

public class Elevator {
	
	public float posY;
	
	public int currentFloor;
	public List<Integer> nextFloor = new ArrayList<>();
	public Integer currentNextFloorIndex = 0;
	
	public boolean isAvailable = true;
	
	Main main = new Main();
	
	public void GetPassenger(ThreadPassenger passenger)
	{
		// TODO: Movement logic
		
		if(nextFloor.size() < 3)
		{
			currentFloor = passenger.currentFloor;
			nextFloor.add(passenger.destinyFloor);
			
			currentNextFloorIndex++;
			
			System.out.println("Elevador em: " + currentFloor 
					+ "     Próximo Destino: " + nextFloor
					+ "     Passageiro Atual: " + passenger.passengerName);
		}
		else
		{
			isAvailable = false;
			System.out.println("ELEVADOR ESTÁ LOTADO!");
		}
		
	}
	
	public void DeliverPassenger()
	{
		// TODO: Movement logic
		
		currentFloor = nextFloor.get(currentNextFloorIndex);
	}
}