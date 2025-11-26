
public class ElevatorBuffer {
	private int elevatorSlots;
	private boolean isElevatorFull;
	
	public synchronized void GetPassenger()
	{
		// Adiciona um passageiro
		elevatorSlots++;
		
		// Verifica se elevador está cheio
		if(elevatorSlots == 3)
		{
			// Se estiver, lida com isso
			isElevatorFull = true;
			
			// TODO: Lógica que lota o semaforo
		}
	}
	
	public void DeliverPassenger()
	{
		elevatorSlots--;
		
	}
}
