import java.util.concurrent.Semaphore;

public class ThreadManager {
	// Cria um semáforo e define seu limite para X threads
		public static int maxThreads = 5;
		public static Semaphore semaphore = new Semaphore(maxThreads);
}
